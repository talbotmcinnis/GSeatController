using GSeatControllerCore;
using GSeatInfrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;

namespace McPitGSeat
{
    public class GSeatViewModel : INotifyPropertyChanged
    {
        public GSeatControllerCore.GSeatControllerCore core;
        ISimulator simulator;
        IRelays relays;
        public GSeatViewModel()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }

        public void Enable()
        {
            try
            {
                this.GY = 1.0f;
#if xxDEBUG
                this.relays = new DebugRelays(6);
                simulator = new UIDrivenSimSim(this);
#else
                this.relays = new DenkoviRelays(0);
                simulator = new DCSA_10();
#endif

                relays.OnRelayChanged += Relays_OnRelayChanged;

                const double pistonDurationS = 0.2; // Guestmated for 20 PSI

                var shoulderPneumatic = new McPitPneumatic(relays,
                                                inflationRelayNumber: 2,
                                                deflationRelayNumber: 1,
                                                inflationRatePctPerS: 1.0 / pistonDurationS,
                                                deflationRatePctPerS: 0.8 / pistonDurationS);
                var shoulderTransferCurve = new TransferCurve(new List<Vector2>() { new Vector2(0, 0), new Vector2(0.20f, 0), new Vector2(1, 1) });

                const double legInflationDurationS = 2.3;   // 2.3s@20PSI
                const double legDeflationDurationS = 3.3;   // 3.3s@20PSI

                var leftLegPneumatic = new McPitPneumatic(relays,
                    inflationRelayNumber: 6,
                                                deflationRelayNumber: 4,
                                                inflationRatePctPerS: 1.0 / legInflationDurationS,
                                                deflationRatePctPerS: 1.0 / legDeflationDurationS);
                var rightLegPneumatic = new McPitPneumatic(relays,
                    inflationRelayNumber: 3,
                                                deflationRelayNumber: 5,
                                                inflationRatePctPerS: 1.0 / legInflationDurationS,
                                                deflationRatePctPerS: 1.0 / legDeflationDurationS);
                // Want a decent deadzone for horizontal roll, and then inverted flight is the same
                var legTransferCurve = new TransferCurve(new List<Vector2>() {
                    new Vector2(0, 0), // Identity zero
                    new Vector2(0.22f, 0), // Deadzone near the bottom
                    new Vector2(1, 0.75f), // Max output to limit leg pressures
                });

                core = new GSeatControllerCore.GSeatControllerCore(simulator, shoulderPneumatic, leftLegPneumatic, rightLegPneumatic, shoulderTransferCurve, legTransferCurve);

                dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
                dispatcherTimer.Interval = TimeSpan.FromMilliseconds(50);
                dispatcherTimer.Tick += DispatcherTimer_Tick;
                dispatcherTimer.Start();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString(), ex.Message);
            }
        }

        internal void SetOverrides(bool leftLegInflate, bool leftLegDeflate, bool shoulderInflate, bool shoulderDeflate, bool rightLegInflate, bool rightLegDeflate)
        {
            System.Diagnostics.Debug.WriteLine(
                "Overrides: " +
                (leftLegDeflate ? "LLD" : "") +
                (leftLegInflate ? "LLI" : "") +
                (shoulderDeflate ? "SD" : "") +
                (shoulderInflate ? "SI" : "") +
                (rightLegDeflate ? "RLD" : "") +
                (rightLegInflate ? "RLI" : "")
                );
            if (this.core != null)
                this.core.SetOverrides(leftLegInflate, leftLegDeflate, shoulderInflate, shoulderDeflate, rightLegInflate, rightLegDeflate);
        }

        System.Windows.Threading.DispatcherTimer dispatcherTimer;

        public void EmergencyStop()
        {
            if (this.core != null)
                this.core.EmergencyStop = true;
        }

        public async void Disable()
        {
            try
            {
                if (this.dispatcherTimer != null)
                {
                    dispatcherTimer.Stop();
                    dispatcherTimer = null;
                }

                if (this.core != null)
                {
                    await this.core.ZeroPneumatics();
                    this.core = null;
                }

                if (this.simulator != null)
                {
                    this.simulator.Dispose();
                    this.simulator = null;
                }

                if (this.relays != null)
                {
                    this.relays.Dispose();
                    this.relays = null;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString(), ex.Message);
            }
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
            if (simulator is DCSA_10)
            {
                var sample = simulator.GetSample;
                if (sample != null)
                {
                    this.Pitch = sample.Pitch;
                    this.Roll = sample.Roll;
                    this.GY = sample.Acceleration.Y;
                    this.GZ = sample.Acceleration.Z;
                }
            }

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

        double pressureTweak;
        public double PressureTweak
        {
            get { return pressureTweak; }

            set
            {
                //System.Diagnostics.Debug.WriteLine($"PressureTweak: {0.5 + (pressureTweak / 5.0):F2}");
                if (value != pressureTweak && this.core != null)
                {
                    this.core.SetPressureTweak(GSeatControllerCore.GSeatControllerCore.TWEAK_MIN + (pressureTweak / 5.0/(GSeatControllerCore.GSeatControllerCore.TWEAK_MAX- GSeatControllerCore.GSeatControllerCore.TWEAK_MIN)));
                }

                pressureTweak = value;
                OnPropertyChanged("PressureTweak");
            }
        }
    }
}
