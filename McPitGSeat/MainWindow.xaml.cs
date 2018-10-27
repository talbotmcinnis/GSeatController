using GSeatControllerCore;
using GSeatInfrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Threading.Tasks;
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
            public GSeatControllerCore.GSeatControllerCore core;
            DebugRelays relays;

            public GSeatVM()
            {
                this.GY = 1.0f;
                //var relays = new DenkoviRelays(0);
                relays = new DebugRelays(6);
                relays.OnRelayChanged += Relays_OnRelayChanged;
                
                //var simulator = new DCSA_10();
                var simulator = new UIDrivenSimSim(this);

                var shoulderPneumatic = new McPitPneumatic(relays, 1, 2, 5, 5);
                var shoulderTransferCurve = new TransferCurve(new List<Vector2>() { new Vector2(0, 0), new Vector2(0.20f, 0), new Vector2(1, 1) });
                var leftLegPneumatic = new McPitPneumatic(relays, 3, 4, .50, .25);
                var rightLegPneumatic = new McPitPneumatic(relays, 5, 6, .50, .25);
                var legTransferCurve = new TransferCurve(new List<Vector2>() { new Vector2(0, 0), new Vector2(0.20f, 0), new Vector2(1, 1) });

                core = new GSeatControllerCore.GSeatControllerCore(simulator, shoulderPneumatic, leftLegPneumatic, rightLegPneumatic, shoulderTransferCurve, legTransferCurve);

                var dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
                dispatcherTimer.Interval = TimeSpan.FromMilliseconds(50);
                dispatcherTimer.Tick += DispatcherTimer_Tick;
                dispatcherTimer.Start();
            }

            private void Relays_OnRelayChanged(int relayNumber, bool state)
            {
                switch (relayNumber)
                {
                    case 1: this.R1 = state; break;
                    case 2: this.R2 = state; break;
                    case 3: this.R3 = state; break;
                    case 4: this.R4 = state; break;
                    case 5: this.R5 = state; break;
                    case 6: this.R6 = state; break;
                }
            }

            private async void DispatcherTimer_Tick(object sender, System.EventArgs e)
            {
                await core.SyncSeatToSim();

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

            float gY;
            public float GY
            {
                get
                {
                    return gY;
                }

                set
                {
                    gY = value;
                    OnPropertyChanged("GY");
                }
            }

            float gZ;
            public float GZ
            {
                get
                {
                    return gZ;
                }

                set
                {
                    gZ = value;
                    OnPropertyChanged("GZ");
                }
            }

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

        private void resetSim_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as GSeatVM;
            vm.Pitch = 0;
            vm.Roll = 0;
            vm.GY = 1;
            vm.GZ = 0;
        }

        private void btnReleasePneumatics_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as GSeatVM).core.EmergencyStop = true;
        }

        private async void btnZeroPneumatics_Click(object sender, RoutedEventArgs e)
        {
            await (this.DataContext as GSeatVM).core.ZeroPneumatics();
        }
    }
}
