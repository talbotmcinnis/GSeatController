using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace GSeatControllerCore
{
    public class TransferCurve
    {
        IEnumerable<Vector2> points;
        public TransferCurve(List<Vector2> points)
        {
            this.points = points.OrderBy(p => p.X);
        }

        public double Transfer(double input)
        {
            if (input < points.First().X)
                return points.First().Y;
            else if (input > points.Last().X)
                return points.Last().Y;

            var rangeStart = points.FirstOrDefault(p => p.X >= input);
            if( rangeStart == null )
                rangeStart = points.First();
            var rangeEnd = points.Reverse().FirstOrDefault(p => p.X <= input);
            if( rangeEnd == null )
                rangeEnd = points.Last();

            // We have a range, now simply interpolate'
            var rangeXPos = input / (rangeEnd.X - rangeStart.X);

            var result = rangeStart.Y + rangeXPos * (rangeEnd.Y - rangeEnd.Y);

            return result;
        }
    }
}
