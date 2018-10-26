using System;
using System.Collections.Generic;
using System.Text;

namespace GSeatControllerCore
{
    class HisteresisBuffer
    {
        double lastValue;
        double buffer;

        public HisteresisBuffer(double buffer)
        {
            this.buffer = buffer;
            this.lastValue = 0;
        }

        public double Buffer(double setPoint)
        {
            if (Math.Abs(setPoint - this.lastValue) > buffer)
                this.lastValue = setPoint;

            return this.lastValue;
        }
    }
}
