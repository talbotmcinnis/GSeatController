namespace GSeatControllerCore
{
    public class GSeatControllerCore
    {
        ISimulator simulator;
        ISingleAxisPneumatic shoulderPneumatic;
        ISingleAxisPneumatic leftLegPneumatic;
        ISingleAxisPneumatic rightLegPneumatic;
        TimedPressureController shoulderTPC;
        TimedPressureController leftLegTPC;
        TimedPressureController rightLegTPC;

        public GSeatControllerCore(ISimulator simulator,
            ISingleAxisPneumatic shoulderPneumatic,
            ISingleAxisPneumatic leftLegPneumatic,
            ISingleAxisPneumatic rightLegPneumatic)
        {
            this.simulator = simulator;
            this.shoulderPneumatic = shoulderPneumatic;
            this.leftLegPneumatic = leftLegPneumatic;
            this.rightLegPneumatic = rightLegPneumatic;

            this.shoulderTPC = new TimedPressureController(this.shoulderPneumatic);
            this.leftLegTPC = new TimedPressureController(this.leftLegPneumatic);
            this.rightLegTPC = new TimedPressureController(this.rightLegPneumatic);
        }

        public async Task SyncSeatToSim()
        {
            var sample = simulator.GetSample;

            // TODO: Translate sim data into pressures
            sample.GForce;
            sample.Orientation;
            sample.Acceleration;

            // Shoulder pressure linear with G force for acceleration, or inverted, or Pure Gs

            // Roll pressure linear with orientation roll, reduced by Z-Gs



            await this.shoulderTPC.SetPressurePercent(desiredShoulderPressure);
            await this.leftLegTPC.SetPressurePercent(desiredLeftLegPressure);
            await this.rightLegTPC.SetPressurePercent(desiredRightLegPressure);
        }
    }
}
