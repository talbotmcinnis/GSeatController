using GSeatControllerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSeatInfrastructure
{
    public class DebugRelays : IRelays
    {
        public DebugRelays(int expectedRelays)
        {
            for (int i = 1; i <= expectedRelays; i++)
                relayStates[i] = false;

        }

        public void SetRelayState(int relayNumber, bool state)
        {
            System.Diagnostics.Debug.WriteLine($"Relay {relayNumber} -> {state}");

            relayStates[relayNumber] = state;
        }

        public Dictionary<int, bool> relayStates = new Dictionary<int, bool>();
    }
}
