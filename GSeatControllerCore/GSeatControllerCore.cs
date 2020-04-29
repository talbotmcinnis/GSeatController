using System;
using System.Threading.Tasks;

namespace GSeatControllerCore
{
    public class GSeatControllerCore
    {
        ISimulator simulator;
        ISingleAxisPneumatic shoulderPneumatic;
        ISingleAxisPneumatic leftLegPneumatic;
        ISingleAxisPneumatic rightLegPneumatic;

        TransferCurve shoulderTransferCurve;
        TransferCurve legTransferCurve;

        public TimedPressureController ShoulderTPC { get; private set; }
        public TimedPressureController LeftLegTPC { get; private set; }
        public TimedPressureController RightLegTPC { get; private set; }

        HysteresisBuffer shoulderBuffer;
        HysteresisBuffer leftLegBuffer;
        HysteresisBuffer rightLegBuffer;

        public GSeatControllerCore(ISimulator simulator,
            ISingleAxisPneumatic shoulderPneumatic,
            ISingleAxisPneumatic leftLegPneumatic,
            ISingleAxisPneumatic rightLegPneumatic,
            TransferCurve shoulderTransferCurve,
            TransferCurve legTransferCurve,
            float hysteresisPct)
        {
            this.simulator = simulator;
            this.shoulderPneumatic = shoulderPneumatic;
            this.leftLegPneumatic = leftLegPneumatic;
            this.rightLegPneumatic = rightLegPneumatic;

            this.ShoulderTPC = new TimedPressureController(this.shoulderPneumatic);
            this.LeftLegTPC = new TimedPressureController(this.leftLegPneumatic);
            this.RightLegTPC = new TimedPressureController(this.rightLegPneumatic);

            this.shoulderBuffer = new HysteresisBuffer(hysteresisPct);
            this.leftLegBuffer = new HysteresisBuffer(hysteresisPct);
            this.rightLegBuffer = new HysteresisBuffer(hysteresisPct);

            this.shoulderTransferCurve = shoulderTransferCurve;
            this.legTransferCurve = legTransferCurve;
        }

        public async Task SyncSeatToSim()
        {
            if (EmergencyStop || isZeroing)
                return;

            // Step: Read from the Sim
            var sample = simulator.GetSample;

            if (sample == null)
                return;

            switch (sample.Command)
            {
                case "Start":
                    this.EmergencyStop = false;
                    await this.ZeroPneumatics();
                    return;
                case "Stop":
                    this.EmergencyStop = true;
                    return;
            }

            // Step: Apply Overrides
            if (override_leftLeg != null || override_rightLeg != null || override_shoulder != null)
            {
                System.Diagnostics.Debug.WriteLine("Applying override");
                // Apply the calculated pressures
                if(override_leftLeg == OverrideMode.inflate)
                    await this.LeftLegTPC.SetPressurePercent(1.0);
                else if( override_leftLeg == OverrideMode.deflate)
                    await this.LeftLegTPC.SetPressurePercent(0);

                if (override_shoulder == OverrideMode.inflate)
                    await this.ShoulderTPC.SetPressurePercent(1.0);
                else if (override_shoulder == OverrideMode.deflate)
                    await this.ShoulderTPC.SetPressurePercent(0);

                if (override_rightLeg == OverrideMode.inflate)
                    await this.RightLegTPC.SetPressurePercent(1.0);
                else if (override_rightLeg == OverrideMode.deflate)
                    await this.RightLegTPC.SetPressurePercent(0);

                return;
            }

            // Step: Convert sim data sample to pressures
            // Shoulder pressure linear with G force for acceleration, or inverted, or Pure Gs
            // Roll pressure linear with orientation roll, reduced by Z-Gs
            var accelShoulder = sample.Acceleration.Z < 0 ? -sample.Acceleration.Z / 2 : 0;
            float gForceShoulder = 0;
            if( sample.Acceleration.Y > 1 )
                gForceShoulder = (sample.Acceleration.Y - 1) / 4;  // Positive Gs
            else if( sample.Acceleration.Y < 1 )
                gForceShoulder = (sample.Acceleration.Y - 1) / -2;  // Negative Gs

            var pitchShoulderForce = sample.Pitch < 0 ? -sample.Pitch / 90 : 0; // Add force when descending, 0 to 1

            var rollShoulderForce = Math.Abs(sample.Roll) > 120 ? (Math.Abs(sample.Roll)-120)/(180-120): 0; // If rolled inverted, add shoulder force 0-1
            var desiredShoulderPressure = accelShoulder + gForceShoulder + pitchShoulderForce + rollShoulderForce;

            var normalizedRoll = sample.Roll;
            if (normalizedRoll > 90)
                normalizedRoll = 180 - normalizedRoll;
            else if (normalizedRoll < -90)
                normalizedRoll = -180 - normalizedRoll;

            var desiredLeftLegPressure = normalizedRoll < 0 ? -normalizedRoll/ 90 : 0;
            var desiredRightLegPressure = normalizedRoll > 0 ? normalizedRoll / 90 : 0;

            // If high/low pitch, roll shouldn't have an effect
            const int pitchRollDeadzone = 75;
            if (Math.Abs(sample.Pitch) > pitchRollDeadzone)
            {
                desiredLeftLegPressure -= (Math.Abs(sample.Pitch) - pitchRollDeadzone) /(90-pitchRollDeadzone);
                desiredRightLegPressure -= (Math.Abs(sample.Pitch) - pitchRollDeadzone) / (90 -pitchRollDeadzone);
            }

            // Step: Translate the raw data through a transfer curve
            desiredShoulderPressure = shoulderTransferCurve.Transfer(desiredShoulderPressure);
            desiredLeftLegPressure = legTransferCurve.Transfer(desiredLeftLegPressure);
            desiredRightLegPressure = legTransferCurve.Transfer(desiredRightLegPressure);

            // Step: Apply a hysteresis
            desiredShoulderPressure = shoulderBuffer.Buffer(desiredShoulderPressure);
            desiredLeftLegPressure = leftLegBuffer.Buffer(desiredLeftLegPressure);
            desiredRightLegPressure = rightLegBuffer.Buffer(desiredRightLegPressure);

            // Step: Apply the pressure tweaks
            desiredShoulderPressure *= _pressureTweak;
            desiredLeftLegPressure *= _pressureTweak;
            desiredRightLegPressure *= _pressureTweak;

            // Step: Apply the calculated pressures
            await this.ShoulderTPC.SetPressurePercent(desiredShoulderPressure);
            await this.LeftLegTPC.SetPressurePercent(desiredLeftLegPressure);
            await this.RightLegTPC.SetPressurePercent(desiredRightLegPressure);
        }

        enum OverrideMode
        {
            inflate,
            deflate
        };

        OverrideMode? override_leftLeg = null;
        OverrideMode? override_shoulder = null;
        OverrideMode? override_rightLeg = null;
        public void SetOverrides(bool leftLegInflate, bool leftLegDeflate, bool shoulderInflate, bool shoulderDeflate, bool rightLegInflate, bool rightLegDeflate)
        {
            if (leftLegDeflate)
                override_leftLeg = OverrideMode.deflate;
            else if (leftLegInflate)
                override_leftLeg = OverrideMode.inflate;
            else
                override_leftLeg = null;

            if (shoulderDeflate)
                override_shoulder = OverrideMode.deflate;
            else if (shoulderInflate)
                override_shoulder = OverrideMode.inflate;
            else
                override_shoulder = null;

            if (rightLegDeflate)
                override_rightLeg = OverrideMode.deflate;
            else if (rightLegInflate)
                override_rightLeg = OverrideMode.inflate;
            else
                override_rightLeg = null;
        }

        bool emergencyStop = false;
        public bool EmergencyStop
        {
            get { return emergencyStop; }
            set {
                if (value && !emergencyStop)
                {
                    Task.Factory.StartNew(() => this.shoulderPneumatic.Deflate(5));
                    Task.Factory.StartNew(() => this.leftLegPneumatic.Deflate(5));
                    Task.Factory.StartNew(() => this.rightLegPneumatic.Deflate(5));

                }
                emergencyStop = value;
            }
        }

        bool isZeroing = false;
        public async Task ZeroPneumatics()
        {
            isZeroing = true;

            // Let all pneumatics drain, in parallel
            var T1 = Task.Factory.StartNew(() => this.shoulderPneumatic.Deflate(5));
            var T2 = Task.Factory.StartNew(() => this.leftLegPneumatic.Deflate(5));
            var T3 = Task.Factory.StartNew(() => this.rightLegPneumatic.Deflate(5));

            await Task.WhenAll(new Task[] { T1, T2, T3 });
            this.ShoulderTPC.Reset();
            this.LeftLegTPC.Reset();
            this.RightLegTPC.Reset();

            isZeroing = false;
        }

        public static readonly double TWEAK_MIN = 0.75;
        public static readonly double TWEAK_MAX = 1.25;
        private double _pressureTweak = 1.0d;
        public void SetPressureTweak(double pressureTweak)
        {
            if (pressureTweak < TWEAK_MIN || pressureTweak > TWEAK_MAX)
                throw new ArgumentOutOfRangeException($"Pressure tweak constrained to {TWEAK_MIN:F2} to {TWEAK_MAX:F2}");

            System.Diagnostics.Debug.WriteLine($"Pressure Tweak: {pressureTweak:F2}");
            _pressureTweak = pressureTweak;
        }
    }
}
