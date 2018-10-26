using GSeatControllerCore;
using GSeatInfrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace McPitGSeat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var relays = new DenkoviRelays(0);
            var simulator = new DCSA_10();
            var shoulderPneumatic = new McPitPneumatic(relays, 1, 2, 50, 50);
            var shoulderTransferCurve = new TransferCurve(new List<Vector2>() { new Vector2(0, 0), new Vector2(100, 100) });

            var leftLegPneumatic = new McPitPneumatic(relays, 3, 4, 25, 15);
            var rightLegPneumatic = new McPitPneumatic(relays, 5, 6, 25, 15);
            var legTransferCurve = new TransferCurve(new List<Vector2>() { new Vector2(0, 0), new Vector2(100, 100) });

            var core = new GSeatControllerCore.GSeatControllerCore(simulator, shoulderPneumatic, leftLegPneumatic, rightLegPneumatic, shoulderTransferCurve, legTransferCurve);
        }
    }
}
