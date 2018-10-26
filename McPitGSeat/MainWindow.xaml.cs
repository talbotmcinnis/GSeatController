using GSeatControllerCore;
using GSeatInfrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

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

            var vm = new MyViewModel();
            vm.PilotColor = new SolidColorBrush(Colors.Green);
            //myViewport.Children[0]..Add(CreatePilotBody());
            this.DataContext = vm;

            var dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            //dispatcherTimer.Start();
        }

        private void DispatcherTimer_Tick(object sender, System.EventArgs e)
        {
            (this.DataContext as MyViewModel).PilotColor = new SolidColorBrush(Colors.Pink);
        }

        public class MyViewModel : INotifyPropertyChanged
        {
            SolidColorBrush pilotColor;
            public SolidColorBrush PilotColor
            {
                get { return pilotColor; }
                set
                {
                    pilotColor = value;
                    OnPropertyChanged("PilotColor");
                }
            }

            double roll;
            public double Roll
            {
                get
                {
                    return roll;
                }

                set
                {
                    roll = value;
                    this.PilotColor = new SolidColorBrush(new Color() { B = (byte)(roll * byte.MaxValue), A=Byte.MaxValue });
                }
            }

            double pitch;
            public double Pitch
            {
                get
                {
                    return pitch;
                }

                set
                {
                    pitch = value;
                    this.PilotColor = new SolidColorBrush(new Color() { G = (byte)(pitch*byte.MaxValue), A = Byte.MaxValue });
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string name)
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null)
                    handler(this, new PropertyChangedEventArgs(name));
            }
        }

        //private Model3D CreatePilotBody()
        //{
        //    var myModel = new GeometryModel3D();
        //    var myMeshGeometry = new MeshGeometry3D();

        //    myMeshGeometry.Positions = new Point3DCollection(
        //        new List<Point3D>()
        //        {
        //            new Point3D(0.293893,-0.5,0.404509),
        //            new Point3D(0.475528,-0.5,0.154509),
        //            new Point3D(0,0.5,0),
        //            new Point3D(0.475528,-0.5,0.154509),
        //            new Point3D(0,0.5,0),
        //            new Point3D(0,0.5,0),
        //            new Point3D(0.475528,-0.5,0.154509),
        //            new Point3D(0.475528,-0.5,-0.154509),
        //            new Point3D(0,0.5,0),
        //            new Point3D(0.475528,-0.5,-0.154509),
        //            new Point3D(0,0.5,0),
        //            new Point3D(0,0.5,0),
        //            new Point3D(0.475528,-0.5,-0.154509),
        //            new Point3D(0.293893,-0.5,-0.404509),
        //            new Point3D(0,0.5,0),
        //            new Point3D(0.293893,-0.5,-0.404509),
        //            new Point3D(0,0.5,0),
        //            new Point3D(0,0.5,0),
        //            new Point3D(0.293893,-0.5,-0.404509),
        //            new Point3D(0,-0.5,-0.5),
        //            new Point3D(0,0.5,0),
        //            new Point3D(0,-0.5,-0.5),
        //            new Point3D(0,0.5,0),
        //            new Point3D(0,0.5,0),
        //            new Point3D(0,-0.5,-0.5),
        //            new Point3D(-0.293893,-0.5,-0.404509),
        //            new Point3D(0,0.5,0),
        //            new Point3D(-0.293893,-0.5,-0.404509),
        //            new Point3D(0,0.5,0),
        //            new Point3D(0,0.5,0),
        //            new Point3D(-0.293893,-0.5,-0.404509),
        //            new Point3D(-0.475528,-0.5,-0.154509),
        //            new Point3D(0,0.5,0),
        //            new Point3D(-0.475528,-0.5,-0.154509),
        //            new Point3D(0,0.5,0),
        //            new Point3D(0,0.5,0),
        //            new Point3D(-0.475528,-0.5,-0.154509),
        //            new Point3D(-0.475528,-0.5,0.154509),
        //            new Point3D(0,0.5,0),
        //            new Point3D(-0.475528,-0.5,0.154509),
        //            new Point3D(0,0.5,0),
        //            new Point3D(0,0.5,0),
        //            new Point3D(-0.475528,-0.5,0.154509),
        //            new Point3D(-0.293892,-0.5,0.404509),
        //            new Point3D(0,0.5,0),
        //            new Point3D(-0.293892,-0.5,0.404509),
        //            new Point3D(0,0.5,0),
        //            new Point3D(0,0.5,0),
        //            new Point3D(-0.293892,-0.5,0.404509),
        //            new Point3D(0,-0.5,0.5),
        //            new Point3D(0,0.5,0),
        //            new Point3D(0,-0.5,0.5),
        //            new Point3D(0,0.5,0),
        //            new Point3D(0,0.5,0),
        //            new Point3D(0,-0.5,0.5),
        //            new Point3D(0.293893,-0.5,0.404509),
        //            new Point3D(0,0.5,0),
        //            new Point3D(0.293893,-0.5,0.404509),
        //            new Point3D(0,0.5,0),
        //            new Point3D(0,0.5,0)
        //        });
        //    myMeshGeometry.Normals = new Vector3DCollection(
        //        new List<Vector3D>()
        //        {
        //            new Vector3D(0.7236065,0.4472139,0.5257313), new Vector3D(0.2763934,0.4472138,0.8506507), new Vector3D(0.5308242,0.4294462,0.7306172), new Vector3D(0.2763934,0.4472138,0.8506507), new Vector3D(0,0.4294458,0.9030925), new Vector3D(0.5308242,0.4294462,0.7306172), new Vector3D(0.2763934,0.4472138,0.8506507), new Vector3D(-0.2763934,0.4472138,0.8506507), new Vector3D(0,0.4294458,0.9030925), new Vector3D(-0.2763934,0.4472138,0.8506507), new Vector3D(-0.5308242,0.4294462,0.7306172), new Vector3D(0,0.4294458,0.9030925), new Vector3D(-0.2763934,0.4472138,0.8506507), new Vector3D(-0.7236065,0.4472139,0.5257313), new Vector3D(-0.5308242,0.4294462,0.7306172), new Vector3D(-0.7236065,0.4472139,0.5257313), new Vector3D(-0.858892,0.429446,0.279071), new Vector3D(-0.5308242,0.4294462,0.7306172), new Vector3D(-0.7236065,0.4472139,0.5257313), new Vector3D(-0.8944269,0.4472139,0), new Vector3D(-0.858892,0.429446,0.279071), new Vector3D(-0.8944269,0.4472139,0), new Vector3D(-0.858892,0.429446,-0.279071), new Vector3D(-0.858892,0.429446,0.279071), new Vector3D(-0.8944269,0.4472139,0), new Vector3D(-0.7236065,0.4472139,-0.5257313), new Vector3D(-0.858892,0.429446,-0.279071), new Vector3D(-0.7236065,0.4472139,-0.5257313), new Vector3D(-0.5308242,0.4294462,-0.7306172), new Vector3D(-0.858892,0.429446,-0.279071), new Vector3D(-0.7236065,0.4472139,-0.5257313), new Vector3D(-0.2763934,0.4472138,-0.8506507), new Vector3D(-0.5308242,0.4294462,-0.7306172), new Vector3D(-0.2763934,0.4472138,-0.8506507), new Vector3D(0,0.4294458,-0.9030925), new Vector3D(-0.5308242,0.4294462,-0.7306172), new Vector3D(-0.2763934,0.4472138,-0.8506507), new Vector3D(0.2763934,0.4472138,-0.8506507), new Vector3D(0,0.4294458,-0.9030925), new Vector3D(0.2763934,0.4472138,-0.8506507), new Vector3D(0.5308249,0.4294459,-0.7306169), new Vector3D(0,0.4294458,-0.9030925), new Vector3D(0.2763934,0.4472138,-0.8506507), new Vector3D(0.7236068,0.4472141,-0.5257306), new Vector3D(0.5308249,0.4294459,-0.7306169), new Vector3D(0.7236068,0.4472141,-0.5257306), new Vector3D(0.8588922,0.4294461,-0.27907), new Vector3D(0.5308249,0.4294459,-0.7306169), new Vector3D(0.7236068,0.4472141,-0.5257306), new Vector3D(0.8944269,0.4472139,0), new Vector3D(0.8588922,0.4294461,-0.27907), new Vector3D(0.8944269,0.4472139,0), new Vector3D(0.858892,0.429446,0.279071), new Vector3D(0.8588922,0.4294461,-0.27907), new Vector3D(0.8944269,0.4472139,0), new Vector3D(0.7236065,0.4472139,0.5257313), new Vector3D(0.858892,0.429446,0.279071), new Vector3D(0.7236065,0.4472139,0.5257313), new Vector3D(0.5308242,0.4294462,0.7306172), new Vector3D(0.858892,0.429446,0.279071)
        //        });

        //    myMeshGeometry.TriangleIndices = new Int32Collection(
        //        new List<int>()
        //        {
        //            0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59
        //        }
        //        );

        //    myModel.Geometry = myMeshGeometry;
        //    var myBrush = new SolidColorBrush(Colors.Blue);
        //    myBrush.Opacity = 0.8;
        //    var myMaterial = new DiffuseMaterial(Brushes.Blue);
        //    myModel.Material = myMaterial;

        //    return myModel;
        //}
    }
}
