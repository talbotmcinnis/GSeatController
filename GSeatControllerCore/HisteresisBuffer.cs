using System;
using System.Collections.Generic;
using System.Text;

namespace GSeatControllerCore
{
    class HisteresisBuffer<T>
    {
        T lastValue;
        T buffer;

        HisteresisBuffer(T buffer)
        {
            this.buffer = buffer;
            this.lastValue = 0;
        }

        T Buffer(T setPoint)
        {
            if (Math.Absolute(setPoint - this.lastValue) > buffer)
                this.lastValue = setPoint;

            return this.lastValue;
        }
    }
}
