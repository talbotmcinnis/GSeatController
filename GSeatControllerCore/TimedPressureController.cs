using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GSeatControllerCore
{
    public class TimedPressureController
    {
        System.Threading.SemaphoreSlim pneumaticSemaphore = new System.Threading.SemaphoreSlim(1,1);
        public TimedPressureController(ISingleAxisPneumatic pneumatic)
        {
            this.pneumatic = pneumatic;
            this.PressurePercent = 0;
        }

        public void Reset()
        {
            this.PressurePercent = 0;
        }

        ISingleAxisPneumatic pneumatic;
        public double PressurePercent { get; private set; }

        public async Task SetPressurePercent(double newPercent)
        {
            await pneumaticSemaphore.WaitAsync();
            try
            {
                var deltaPercent = newPercent - PressurePercent;

                if (deltaPercent > 0)
                {
                    // Requires inflation
                    var inflationDurationS = deltaPercent / pneumatic.InflationRatePctPerS;
                    if (newPercent >= 1)
                        inflationDurationS += 0.25 / pneumatic.InflationRatePctPerS;    // Extra padding to ensure maximum inflation
                    await pneumatic.Inflate(inflationDurationS);
                }
                else if (deltaPercent < 0)
                {
                    var deflationDurationS = (-deltaPercent) / pneumatic.DeflationRatePctPerS;
                    if (newPercent <= 0)
                        deflationDurationS += 0.25 / pneumatic.DeflationRatePctPerS;    // Extra padding to ensure absolute zero
                    await pneumatic.Deflate(deflationDurationS);
                }
                this.PressurePercent = newPercent;
            }
            finally
            {
                pneumaticSemaphore.Release();
            }
        }
    }

}
