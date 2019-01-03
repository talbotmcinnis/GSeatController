using System;
using System.Collections.Generic;
using System.Text;

namespace GSeatControllerCore
{
    public delegate void RelayChangeHandler(int relayNumber, bool state);

    public interface IRelays
    {
        void SetRelayState(int relayNumber, bool state);

        event RelayChangeHandler OnRelayChanged;
    }
}
