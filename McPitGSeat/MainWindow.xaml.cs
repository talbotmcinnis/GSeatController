using GSeatControllerCore;
using GSeatInfrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Windows;

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
            
            var vm = new GSeatVM();
            this.DataContext = vm;
        }


        public class GSeatVM : INotifyPropertyChanged
        {
            GSeatControllerCore.GSeatControllerCore core;
            DebugRelays relays;

            public GSeatVM()
            {
                this.GY = 1.0f;
                //var relays = new DenkoviRelays(0);
                relays = new DebugRelays(6);
                
                //var simulator = new DCSA_10();
                var simulator = new UIDrivenSimSim(this);

                var shoulderPneumatic = new McPitPneumatic(relays, 1, 2, 50, 50);
                var shoulderTransferCurve = new TransferCurve(new List<Vector2>() { new Vector2(0, 0), new Vector2(0.20f, 0), new Vector2(1, 1) });

                var leftLegPneumatic = new McPitPneumatic(relays, 3, 4, 25, 15);
                var rightLegPneumatic = new McPitPneumatic(relays, 5, 6, 25, 15);
                var legTransferCurve = new TransferCurve(new List<Vector2>() { new Vector2(0, 0), new Vector2(0.20f, 0), new Vector2(1, 1) });

                core = new GSeatControllerCore.GSeatControllerCore(simulator, shoulderPneumatic, leftLegPneumatic, rightLegPneumatic, shoulderTransferCurve, legTransferCurve);

                var dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
                dispatcherTimer.Interval = TimeSpan.FromMilliseconds(100);
                dispatcherTimer.Tick += DispatcherTimer_Tick;
                dispatcherTimer.Start();
            }

            private async void DispatcherTimer_Tick(object sender, System.EventArgs e)
            {
                await core.SyncSeatToSim();

                this.R1 = relays.relayStates[1];
                this.R2 = relays.relayStates[2];
                this.R3 = relays.relayStates[3];
                this.R4 = relays.relayStates[4];
                this.R5 = relays.relayStates[5];
                this.R6 = relays.relayStates[6];

                this.ShoulderPressurePercent = core.ShoulderTPC.PressurePercent;
                this.LeftLegPressurePercent = core.LeftLegTPC.PressurePercent;
                this.RightLegPressurePercent = core.RightLegTPC.PressurePercent;
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
                    OnPropertyChanged("Roll");
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
                    OnPropertyChanged("Pitch");
                }
            }

            public float GY { get; set; }
            public float GZ { get; set; }

            bool r1;
            public bool R1
            {
                get { return r1; }

                set
                {
                    r1 = value;
                    OnPropertyChanged("R1");
                }
            }

            bool r2;
            public bool R2
            {
                get { return r2; }

                set
                {
                    r2 = value;
                    OnPropertyChanged("R2");
                }
            }

            bool r3;
            public bool R3
            {
                get { return r3; }

                set
                {
                    r3 = value;
                    OnPropertyChanged("R3");
                }
            }

            bool r4;
            public bool R4
            {
                get { return r4; }

                set
                {
                    r4 = value;
                    OnPropertyChanged("R4");
                }
            }

            bool r5;
            public bool R5
            {
                get { return r5; }

                set
                {
                    r5 = value;
                    OnPropertyChanged("R5");
                }
            }

            bool r6;
            public bool R6
            {
                get { return r6; }

                set
                {
                    r6 = value;
                    OnPropertyChanged("R6");
                }
            }

            double shoulderPressurePercent;
            public double ShoulderPressurePercent
            {
                get { return shoulderPressurePercent; }

                set
                {
                    shoulderPressurePercent = value;
                    OnPropertyChanged("ShoulderPressurePercent");
                }
            }

            double leftLegPressurePercent;
            public double LeftLegPressurePercent
            {
                get { return leftLegPressurePercent; }

                set
                {
                    leftLegPressurePercent = value;
                    OnPropertyChanged("LeftLegPressurePercent");
                }
            }

            double rightLegPressurePercent;
            public double RightLegPressurePercent
            {
                get { return rightLegPressurePercent; }

                set
                {
                    rightLegPressurePercent = value;
                    OnPropertyChanged("RightLegPressurePercent");
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
    }
}
