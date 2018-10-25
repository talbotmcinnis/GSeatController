using GSeatInfrastructure;
using GSeatControllerCore;
using System;

namespace McPitGSeat
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var relays = new DenkoviRelays(0);
            var simulator = new DCSA_10();
            var shoulderPneumatic = new McPitPneumatic(relays, 1, 2, 50, 50);
            var shoulderTransferCurve = new TransferCurve(new List<Point2>() { new Point2(0, 0), new Point2(100, 100) });

            var leftLegPneumatic = new McPitPneumatic(relays, 3, 4, 25, 15);
            var rightLegPneumatic = new McPitPneumatic(relays, 5, 6, 25, 15);
            var legTransferCurve = new TransferCurve(new List<Point2>() { new Point2(0, 0), new Point2(100, 100) });

            var core = new GSeatControllerCore.GSeatControllerCore(simulator, shoulderPneumatic, leftLegPneumatic, rightLegPneumatic, shoulderTransferCurve, legTransferCurve);
        }
    }
}
