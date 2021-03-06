﻿using GSeatControllerCore;
using Newtonsoft.Json;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace GSeatInfrastructure
{
    public class DCSA_10 : ISimulator
    {
        UdpClient receiver;
        Task receiveTask;
        public DCSA_10(int port = 8804)
        {
            receiver = new UdpClient(port);
            receiveTask = Task.Factory.StartNew(() => ReceiveTaskBody());
        }

        private async void ReceiveTaskBody()
        {
            while (true)
            {
                try
                {
                    var result = await receiver.ReceiveAsync();
                    var payloadString = System.Text.Encoding.UTF8.GetString(result.Buffer);

                    var payload = JsonConvert.DeserializeObject<Sample>(payloadString);
                    // DCS gives radians, but my brain likes degrees
                    payload.Pitch = payload.Pitch * 180 / Math.PI;
                    payload.Roll = payload.Roll * 180 / Math.PI;
                    this.lastDCSPayload = payload;
                }
                catch {
                    if (disposeNow)
                        break;
                }
            }
        }

        bool disposeNow = false;
        public void Dispose()
        {
            this.disposeNow = true;
            this.receiver.Close();
            this.receiver.Dispose();

        }

        Sample lastDCSPayload;

        public Sample GetSample
        {
            get
            {
                return lastDCSPayload;
            }

        }

        /* http://www.digitalcombatsimulator.com/en/dev_journal/lua-export/
         * https://github.com/sprhawk/dcs_scripts/blob/master/Export.lua
         * https://wiki.hoggitworld.com/view/DCS_Export_Script
         * ORRRRR https://www.xsimulator.net/simtools-motion-simulator-software/ with a plugin for DCS?
         * LoGetADIPitchBankYaw()   -- (args - 0, results - 3 (rad))
         *  Not ideal as it depends on the ADI working
         * LoGetAccelerationUnits() -- (args - 0, results - table {x = Nx,y = NY,z = NZ} 1 (G))
         *  Y = G force.  Not sure if this is word coordinates or relative to wings?
         * LoGetVerticalVelocity()  -- (args - 0, results - 1(m/s))
         *  Would have a little effect but can safely ignore it
         * LoGetVectorVelocity		  =  {x,y,z} -- vector of self velocity (world axis)
         *  Velocity doesn't matter
         * LoGetAngularVelocity	  =  {x,y,z} -- angular velocity euler angles , rad per se
         *  Angular velocity doesn't matter
         * local o = LoGetSelfData() -- the ownship data set  
         *  Contains Pitch and Bank which are better than ADI, and yaw doesn't matter
     */

        /* local pitch,roll,yaw = LoGetADIPitchBankYaw()
   o.LatLongAlt.Lat -- Latitude in degress
   o.LatLongAlt.Long -- Longitude in degress
   o.LatLongAlt.Alt -- Altitude in meters MSL
   o.Heading -- Heading in radians
   o.Pitch -- Pitch in radians
   o.Bank -- Bank in radians
   */
    }
}
