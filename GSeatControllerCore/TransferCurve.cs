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
            if (input <= points.First().X)
                return points.First().Y;
            else if (input >= points.Last().X)
                return points.Last().Y;

            var rangeStart = points.GetEnumerator();
            var rangeEnd = points.GetEnumerator();
            rangeStart.MoveNext();
            rangeEnd.MoveNext(); rangeEnd.MoveNext();

            do
            {
                if (rangeStart.Current.X <= input && input < rangeEnd.Current.X)
                {
                    // We have a range, now simply interpolate'
                    var rangeXPos = input / (rangeEnd.Current.X - rangeStart.Current.X);

                    var result = rangeStart.Current.Y + rangeXPos * (rangeEnd.Current.Y - rangeStart.Current.Y);

                    return result;
                }

                rangeStart.MoveNext();
                rangeEnd.MoveNext();
            } while (true);
        }
    }
}
