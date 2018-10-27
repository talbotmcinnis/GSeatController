using GSeatControllerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSeatInfrastructure
{
    public delegate void RelayChangeHandler(int relayNumber,bool state);

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
            System.Diagnostics.Debug.Assert(relayStates[relayNumber] != state);
            relayStates[relayNumber] = state;
            if (OnRelayChanged != null)
                OnRelayChanged(relayNumber, state);
        }

        public Dictionary<int, bool> relayStates = new Dictionary<int, bool>();

        public event RelayChangeHandler OnRelayChanged;
    }
}
