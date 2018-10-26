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

        TimedPressureController shoulderTPC;
        TimedPressureController leftLegTPC;
        TimedPressureController rightLegTPC;

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

            this.shoulderTPC = new TimedPressureController(this.shoulderPneumatic);
            this.leftLegTPC = new TimedPressureController(this.leftLegPneumatic);
            this.rightLegTPC = new TimedPressureController(this.rightLegPneumatic);

            this.shoulderBuffer = new HisteresisBuffer(.15);
            this.leftLegBuffer = new HisteresisBuffer(.15);
            this.rightLegBuffer = new HisteresisBuffer(.15);

            this.shoulderTransferCurve = shoulderTransferCurve;
            this.legTransferCurve = legTransferCurve;
        }

        public async Task SyncSeatToSim()
        {
            var sample = simulator.GetSample;

            // TODO: Translate sim data into pressures
            //sample.Acceleration;
            //sample.Bank;
            //sample.Pitch;

            // Shoulder pressure linear with G force for acceleration, or inverted, or Pure Gs
            // Roll pressure linear with orientation roll, reduced by Z-Gs
            var desiredShoulderPressure = 0d;
            var desiredLeftLegPressure = 0d;
            var desiredRightLegPressure = 0d;

            // TODO: Translate the raw data through a transfer curve
            desiredShoulderPressure = shoulderTransferCurve.Transfer(desiredShoulderPressure);
            desiredLeftLegPressure = legTransferCurve.Transfer(desiredLeftLegPressure);
            desiredRightLegPressure = legTransferCurve.Transfer(desiredRightLegPressure);

            // Apply a histeresis
            desiredShoulderPressure = shoulderBuffer.Buffer(desiredShoulderPressure);
            desiredLeftLegPressure = leftLegBuffer.Buffer(desiredLeftLegPressure);
            desiredRightLegPressure = rightLegBuffer.Buffer(desiredRightLegPressure);

            // Apply the calculated pressures
            await this.shoulderTPC.SetPressurePercent(desiredShoulderPressure);
            await this.leftLegTPC.SetPressurePercent(desiredLeftLegPressure);
            await this.rightLegTPC.SetPressurePercent(desiredRightLegPressure);
        }
    }
}
