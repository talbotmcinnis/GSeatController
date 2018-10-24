using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GSeatControllerCore
{
    public class Sample
    {
        public float Pitch { get; set; }
        public float Bank { get; set; }
        public Vector3 Acceleration { get; set; }
        public float GForce { get; set; }
    }

    public interface ISimulator
    {

        Sample GetSample { get; }
    }
}
