using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GSeatControllerCore
{
    public class TimedPressureController
    {
        public TimedPressureController(ISingleAxisPneumatic pneumatic)
        {
            this.pneumatic = pneumatic;
            this.pressurePercent = 0;
        }

        ISingleAxisPneumatic pneumatic;
        double pressurePercent;

        public async Task SetPressurePercent(double newPercent)
        {
            var deltaPercent = newPercent - pressurePercent;

            if (deltaPercent > 0)
            {
                // Requires inflation
                var inflationDurationS = deltaPercent / pneumatic.InflationRate;
                await pneumatic.Inflate(inflationDurationS);
            }
            else if (deltaPercent < 0)
            {
                var deflationDurationS = (-deltaPercent) / pneumatic.DeflationRate;
                await pneumatic.Deflate(deflationDurationS);
            }
        }
    }

}
