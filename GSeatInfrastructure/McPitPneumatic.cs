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

        public McPitPneumatic(IRelays relayController, int inflationRelayNumber, int deflationRelayNumber, double inflationRatePctPerS, double deflationRatePctPerS)
        {
            this.relayController = relayController;
            this.inflationRelayNumber = inflationRelayNumber;
            this.deflationRelayNumber = deflationRelayNumber;
            this.InflationRatePctPerS = inflationRatePctPerS;
            this.DeflationRatePctPerS = deflationRatePctPerS;
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

        public double InflationRatePctPerS { get; private set; }
        public double DeflationRatePctPerS { get; private set; }

    }
}
