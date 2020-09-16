using GSeatControllerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static McPitGSeat.MainWindow;

namespace McPitGSeat
{
    class UIDrivenSimSim : ISimulator
    {
        GSeatViewModel vm;
        public UIDrivenSimSim(GSeatViewModel vm)
        {
            this.vm = vm;
        }

        public Sample GetSample
        {
            get
            {
                var result = new Sample();
                result.Roll = vm.Roll;
                result.Pitch = -vm.Pitch;
                result.Acceleration = new System.Numerics.Vector3(0, vm.GY, vm.GZ);
                result.AltAGLM = 5000;
                result.Velocity = new System.Numerics.Vector3(20, 20, 20);
                return result;
            }
        }

        public void Dispose()
        {
        }
    }
}
