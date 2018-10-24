using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GSeatControllerCore
{
    public interface ISingleAxisPneumatic
    {
        Task Inflate(float durationS);
        Task Deflate(float durationS);

        /// <summary>
        /// Percentage Inflation per second
        /// </summary>
        float InflationRate { get; }

        /// <summary>
        /// Percentage Deflation per second
        /// </summary>
        float DeflationRate { get; }
    }
}
