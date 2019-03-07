﻿using GSeatControllerCore;
using GSeatInfrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Controls.Primitives;

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

            if (Properties.Settings.Default.LastPosX != 0 && Properties.Settings.Default.LastPosY != 0)
            {
                this.Left = Properties.Settings.Default.LastPosX;
                this.Top = Properties.Settings.Default.LastPosY;
            }

            var vm = new GSeatViewModel();
            this.DataContext = vm;
        }

        private void EnableToggle_Click(object sender, RoutedEventArgs e)
        {
            if ((e.Source as ToggleButton).IsChecked == true)
            {
                (this.DataContext as GSeatViewModel).Enable();
                round_locking_down_png.Visibility = Visibility.Hidden;
                round_locking_up_png.Visibility = Visibility.Visible;
            }
            else
            {
                (this.DataContext as GSeatViewModel).Disable();
                round_locking_down_png.Visibility = Visibility.Visible;
                round_locking_up_png.Visibility = Visibility.Hidden;
            }
        }

        private void BtnEmergencyStop_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as GSeatViewModel).EmergencyStop();
        }
        
        private void Override_TouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {
            (this.DataContext as GSeatViewModel).SetOverrides(
                Override_LeftLegInflate.AreAnyTouchesDirectlyOver,
                Override_LeftLegDeflate.AreAnyTouchesDirectlyOver,
                Override_ShoulderInflate.AreAnyTouchesDirectlyOver,
                Override_ShoulderDeflate.AreAnyTouchesDirectlyOver,
                Override_RightLegInflate.AreAnyTouchesDirectlyOver,
                Override_RighgtLegDeflate.AreAnyTouchesDirectlyOver
                );
        }
        private void Override_TouchUp(object sender, System.Windows.Input.TouchEventArgs e)
        {
            (this.DataContext as GSeatViewModel).SetOverrides(
                false, false,
                false, false,
                false, false
                );
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.LastPosX = this.Left;
            Properties.Settings.Default.LastPosY = this.Top;
            Properties.Settings.Default.Save();
        }
    }
}
