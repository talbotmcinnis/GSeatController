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

        HisteresisBuffer shoulderBuffer;
        HisteresisBuffer leftLegBuffer;
        HisteresisBuffer rightLegBuffer;

        public GSeatControllerCore(ISimulator simulator,
            ISingleAxisPneumatic shoulderPneumatic,
            ISingleAxisPneumatic leftLegPneumatic,
            ISingleAxisPneumatic rightLegPneumatic,
            TransferCurve shoulderTransferCurve,
            TransferCurve legTransferCurve )
        {
            this.simulator = simulator;
            this.shoulderPneumatic = shoulderPneumatic;
            this.leftLegPneumatic = leftLegPneumatic;
            this.rightLegPneumatic = rightLegPneumatic;

            this.ShoulderTPC = new TimedPressureController(this.shoulderPneumatic);
            this.LeftLegTPC = new TimedPressureController(this.leftLegPneumatic);
            this.RightLegTPC = new TimedPressureController(this.rightLegPneumatic);

            this.shoulderBuffer = new HisteresisBuffer(.15);
            this.leftLegBuffer = new HisteresisBuffer(.15);
            this.rightLegBuffer = new HisteresisBuffer(.15);

            this.shoulderTransferCurve = shoulderTransferCurve;
            this.legTransferCurve = legTransferCurve;
        }

        public async Task SyncSeatToSim()
        {
            var sample = simulator.GetSample;

            // Shoulder pressure linear with G force for acceleration, or inverted, or Pure Gs
            // Roll pressure linear with orientation roll, reduced by Z-Gs
            var desiredShoulderPressure = (sample.Acceleration.Y / 4) + (sample.Acceleration.Z / 4) + Math.Abs(sample.Pitch)/90;
            var desiredLeftLegPressure = sample.Roll < 0 ? -sample.Roll/90 : 0;
            var desiredRightLegPressure = sample.Roll > 0 ? sample.Roll / 90 : 0;

            // Translate the raw data through a transfer curve
            desiredShoulderPressure = shoulderTransferCurve.Transfer(desiredShoulderPressure);
            desiredLeftLegPressure = legTransferCurve.Transfer(desiredLeftLegPressure);
            desiredRightLegPressure = legTransferCurve.Transfer(desiredRightLegPressure);

            // Apply a histeresis
            desiredShoulderPressure = shoulderBuffer.Buffer(desiredShoulderPressure);
            desiredLeftLegPressure = leftLegBuffer.Buffer(desiredLeftLegPressure);
            desiredRightLegPressure = rightLegBuffer.Buffer(desiredRightLegPressure);

            // Apply the calculated pressures
            await this.ShoulderTPC.SetPressurePercent(desiredShoulderPressure);
            await this.LeftLegTPC.SetPressurePercent(desiredLeftLegPressure);
            await this.RightLegTPC.SetPressurePercent(desiredRightLegPressure);
        }
    }
}
