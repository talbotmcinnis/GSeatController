using System;
using System.Collections.Generic;
using System.Text;

namespace GSeatControllerCore
{
    public interface IRelays
    {
        void SetRelayState(int relayNumber, bool state);
    }
}
