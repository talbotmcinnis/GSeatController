using GSeatControllerCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GSeatInfrastructure
{
    public class McPitPneumatic : ISingleAxisPneumatic
    {
        IRelays relayController;
        int inflationRelayNumber;
        int deflationRelayNumber;

        public McPitPneumatic(IRelays relayController, int inflationRelayNumber, int deflationRelayNumber, double inflationRate, double deflationRate)
        {
            this.relayController = relayController;
            this.inflationRelayNumber = inflationRelayNumber;
            this.deflationRelayNumber = deflationRelayNumber;
            this.InflationRate = inflationRate;
            this.DeflationRate = deflationRate;
        }

        public async Task Inflate(double durationS)
        {
            await PulseRelay(inflationRelayNumber, durationS);
        }

        public async Task Deflate(double durationS)
        {
            await PulseRelay(deflationRelayNumber, durationS);
        }

        private async Task PulseRelay(int relayNumber, double durationS)
        {
            // Activate the relay
            this.relayController.SetRelayState(relayNumber, true);
            await Task.Delay((int)(durationS * 1e3));
            this.relayController.SetRelayState(relayNumber, false);
        }

        public double InflationRate { get; private set; }
        public double DeflationRate { get; private set; }

    }
}
