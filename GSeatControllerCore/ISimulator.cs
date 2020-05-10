using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GSeatControllerCore
{
    public class Sample
    {
        public Sample()
        {
            Pitch = 0;
            Roll = 0;
            Acceleration = new Vector3(0, 0, 0);
            Velocity = new Vector3(0, 0, 0);
        }

        public double Pitch { get; set; }
        public double Roll { get; set; }
        public Vector3 Acceleration { get; set; }
        public double AltAGLM { get; set;  }
        public Vector3 Velocity { get; set; }

        public string Command { get; set; }
    }

    public interface ISimulator : IDisposable
    {
        Sample GetSample { get; }
    }
}
