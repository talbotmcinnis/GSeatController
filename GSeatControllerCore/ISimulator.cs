using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GSeatControllerCore
{
    public class Sample
    {
        public double Pitch { get; set; }
        public double Roll { get; set; }
        public Vector3 Acceleration { get; set; }

        public string Command { get; set; }
    }

    public interface ISimulator : IDisposable
    {
        Sample GetSample { get; }
    }
}
