using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GSeatControllerCore
{
    public interface ISingleAxisPneumatic
    {
        Task Inflate(double durationS);
        Task Deflate(double durationS);

        void Zeroize();

        /// <summary>
        /// Percentage Inflation per second
        /// </summary>
        double InflationRatePctPerS { get; }

        /// <summary>
        /// Percentage Deflation per second
        /// </summary>
        double DeflationRatePctPerS { get; }
    }
}
