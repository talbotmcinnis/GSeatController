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

        public McPitPneumatic(IRelays relayController, int inflationRelayNumber, int deflationRelayNumber, float inflationRate, float deflationRate)
        {
            this.relayController = relayController;
            this.inflationRelayNumber = inflationRelayNumber;
            this.deflationRelayNumber = deflationRelayNumber;
            this.InflationRate = inflationRate;
            this.DeflationRate = deflationRate;
        }

        public async Task Inflate(float durationS)
        {
            await PulseRelay(inflationRelayNumber, durationS);
        }

        public async Task Deflate(float durationS)
        {
            await PulseRelay(deflationRelayNumber, durationS);
        }

        private async Task PulseRelay(int relayNumber, float durationS)
        {
            // Activate the relay
            this.relayController.SetRelayState(relayNumber, true);
            await Task.Delay((int)(durationS * 1e3));
            this.relayController.SetRelayState(relayNumber, false);
        }

        public float InflationRate { get; private set; }
        public float DeflationRate { get; private set; }

    }
}
