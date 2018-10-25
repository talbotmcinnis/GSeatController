using System;
using System.Collections.Generic;
using System.Text;

namespace GSeatControllerCore
{
    public class TransferCurve
    {
        List<Vector2> points;
        public TransferCurve(List<Vector2> points)
        {
            this.points = points.OrderBy(p => p.X);
        }

        float Transfer(float input)
        {
            if (input < points.First().X)
                return points.First().Y;
            else if (input > points.Last().X)
                return points.Last().Y;

            var rangeStart = points.FirstOrDefault(p => p.X >= input)??points.First();
            var rangeEnd = points.Reverse().FirstOrDefault(p => p.X <= input) ?? points.Last();

            // We have a range, now simply interpolate'
            var rangeXPos = input / (rangeEnd.X - rangeStart.X);

            var result = rangeStart.Y + rangeXPos * (rangeEnd.Y - rangeEnd.Y);

            return result;
        }
    }
}
