using FTD2XX_NET;
using GSeatControllerCore;
using System;

namespace GSeatInfrastructure
{
    public class DenkoviRelays : IRelays
    {
        private FTDI myFtdiDevice;
        private uint deviceIndex;

        byte[] relayState = new byte[1];

        public DenkoviRelays(uint deviceIndex)
        {
            this.myFtdiDevice = new FTDI();
            this.deviceIndex = deviceIndex;

            relayState[0] = 0;

            var status = myFtdiDevice.OpenByIndex(deviceIndex); // TODO: Consider by serial number or something more... sticky
            if (status != FTDI.FT_STATUS.FT_OK)
                throw new ApplicationException($"Error opening device: {status.ToString()}");

            status = myFtdiDevice.ResetDevice();
            if (status != FTDI.FT_STATUS.FT_OK)
                throw new ApplicationException($"Error resetting device: {status.ToString()}");

            //Set Baud Rate
            status = myFtdiDevice.SetBaudRate(921600);
            if (status != FTDI.FT_STATUS.FT_OK)
                throw new ApplicationException($"Error configuring device baud rate: {status.ToString()}");

            //Set Bit Bang
            status = myFtdiDevice.SetBitMode(255, FTD2XX_NET.FTDI.FT_BIT_MODES.FT_BIT_MODE_SYNC_BITBANG);
            if (status != FTDI.FT_STATUS.FT_OK)
                throw new ApplicationException($"Error configuring device bit bang: {status.ToString()}");

            uint receivedBytes = 0;
            myFtdiDevice.Write(relayState, 1, ref receivedBytes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relayNumber">1 based index of the relay to set state for</param>
        /// <param name="state">The desired state, true = closed(active), false = open(inactive)</param>
        public void SetRelayState(int relayNumber, bool state)
        {
            System.Diagnostics.Debug.Assert(relayNumber >= 1 && relayNumber <= 8);

            int v = (relayNumber - 1);
            byte bitMask = (byte)(0x01 << v);
            if (state)
                relayState[0] |= bitMask;
            else
                relayState[0] &= (byte)~bitMask;

            uint receivedBytes = 0;
            myFtdiDevice.Write(relayState, 1, ref receivedBytes);
            if (OnRelayChanged != null)
                OnRelayChanged(relayNumber, state);
        }

        public void Dispose()
        {
            this.myFtdiDevice.Close();
            this.myFtdiDevice = null;
        }

        public event RelayChangeHandler OnRelayChanged;
    }
}
