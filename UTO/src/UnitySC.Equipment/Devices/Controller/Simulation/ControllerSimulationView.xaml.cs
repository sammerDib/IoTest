using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Devices.Controller.Activities;

using UnitsNet;
using UnitsNet.Units;

using UnitySC.Equipment.Abstractions.Devices.Aligner.Enums;
using UnitySC.Equipment.Abstractions.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Communication.CommunicatingDevice;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Activities;
using UnitySC.Equipment.Abstractions.Vendor.Material;

namespace UnitySC.Equipment.Devices.Controller.Simulation
{
    /// <summary>
    ///     Interaction logic for ControllerSimulationView.xaml
    /// </summary>
    public partial class ControllerSimulationView : ISimDeviceView
    {
        public ControllerSimulationView(Controller controller)
        {
            InitializeComponent();
            DataContext = this;
            _controller = controller;
            RefreshLpaControls();
            RefreshUpaControls();

            controller.MaterialManager.MaterialMoved += MaterialManager_MaterialMoved;
            controller.ActivityStarted += Controller_ActivityStarted;
            controller.ActivityDone += Controller_ActivityDone;


            foreach (var lp in LoadPorts)
            {
                lp.PropertyChanged += LoadPort_PropertyChanged;
            }

            foreach (var pm in ProcessModules)
            {
                pm.PropertyChanged += ProcessModule_PropertyChanged;
            }
        }

        private bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        #region Fields

        private readonly Controller _controller;
        private readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text

        #endregion Fields

        #region Devices Handlers

        private void Controller_ActivityDone(object sender, ActivityEventArgs e)
        {
            if (Dispatcher.Thread != Thread.CurrentThread)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    new EventHandler<ActivityEventArgs>(Controller_ActivityDone), sender, e);
                return;
            }

            ConnectBtn.IsEnabled = true;
            StartInitActivityBtn.IsEnabled = true;
            StartLoadPmActivityBtn.IsEnabled = true;
            StartUnloadPmActivityBtn.IsEnabled = true;
            StartClearActivityBtn.IsEnabled = true;
        }

        private void Controller_ActivityStarted(object sender, ActivityEventArgs e)
        {
            if (Dispatcher.Thread != Thread.CurrentThread)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    new EventHandler<ActivityEventArgs>(Controller_ActivityStarted), sender, e);
                return;
            }

            ConnectBtn.IsEnabled = false;
            StartInitActivityBtn.IsEnabled = false;
            StartLoadPmActivityBtn.IsEnabled = false;
            StartUnloadPmActivityBtn.IsEnabled = false;
            StartClearActivityBtn.IsEnabled = false;
        }

        private void MaterialManager_MaterialMoved(object sender, MaterialMovedEventArgs e)
        {
            if (Dispatcher.Thread != Thread.CurrentThread)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    new EventHandler<MaterialMovedEventArgs>(MaterialManager_MaterialMoved), sender, e);
                return;
            }

            if (e.NewLocation != null && e.NewLocation.Name.Contains("dock") ||
                e.OldLocation != null && e.OldLocation.Name.Contains("dock"))
            {
                RefreshAvailableLoadPorts();
            }

            if (e.NewLocation != null && e.NewLocation.Name.Contains(".") ||
                e.OldLocation != null && e.OldLocation.Name.Contains("."))
            {
                RefreshLpaSourceSlotCmb();
                RefreshUpaDestinationSlotCmb();
            }

            if (e.NewLocation != null && e.NewLocation.Name.Contains("ProcessModule") ||
                e.OldLocation != null && e.OldLocation.Name.Contains("ProcessModule"))
            {
                RefreshAvailableProcessModules();
            }
        }

        private void ProcessModule_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Dispatcher.Thread != Thread.CurrentThread)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    new EventHandler<PropertyChangedEventArgs>(ProcessModule_PropertyChanged), sender, e);
                return;
            }

            var pm = (Abstractions.Devices.DriveableProcessModule.DriveableProcessModule)sender;
            switch (e.PropertyName)
            {
                case nameof(pm.Location):
                case nameof(pm.Location.Material):
                    RefreshAvailableProcessModules();
                    break;
            }
        }

        private void LoadPort_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Dispatcher.Thread != Thread.CurrentThread)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    new EventHandler<PropertyChangedEventArgs>(LoadPort_PropertyChanged), sender, e);
                return;
            }

            switch (e.PropertyName)
            {
                case nameof(Abstractions.Devices.LoadPort.LoadPort.IsDoorOpen):
                    RefreshAvailableLoadPorts();
                    break;
            }
        }

        #endregion Devices Handlers

        #region HMI Handlers

        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (var devices in _controller.GetEquipment().AllDevices<CommunicatingDevice>())
            {
                devices.Connect();
            }
        }

        private void StartClearActivityBtn_Click(object sender, RoutedEventArgs e)
        {
            _controller.StartClearActivity();
        }

        private void StartInitActivityBtn_Click(object sender, RoutedEventArgs e)
        {
            _controller.InitializeAsync(IsColdInitChk.IsChecked ?? false);
        }

        private void StartLoadPmActivityBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadToPmConfiguration config = new LoadToPmConfiguration(
                _controller.GetEquipment(),
                LpaSelectedSourceSlot,
                (EffectorType)Enum.Parse(
                    typeof(EffectorType),
                    LpaEffectorTypeCmb.SelectedValue.ToString()));
            config.SetLoadPort(LpaSelectedLoadPort);
            config.SetRobotArm((RobotArm)Enum.Parse(typeof(RobotArm), LpaRobotArmCmb.SelectedValue.ToString()));
            config.SetAlignAngle(new Angle(Convert.ToDouble(LpaAngleTb.Text), AngleUnit.Degree));
            config.SetAlignType((AlignType)Enum.Parse(typeof(AlignType), LpaAlignTypeCmb.SelectedValue.ToString()));
            config.SetProcessModule(LpaSelectedProcessModule);

            _controller.StartLoadProcessModuleActivity(config);
        }

        private void StartUnloadPmActivityBtn_Click(object sender, RoutedEventArgs e)
        {
            UnloadFromPmConfiguration config = new UnloadFromPmConfiguration(
                _controller.GetEquipment(),
                UpaSelectedDestinationSlot,
                (EffectorType)Enum.Parse(
                    typeof(EffectorType),
                    LpuEffectorTypeCmb.SelectedValue.ToString()));
            config.SetProcessModule(UpaSelectedProcessModule);
            config.SetRobotArm((RobotArm)Enum.Parse(typeof(RobotArm), UpaRobotArmCmb.SelectedValue.ToString()));
            config.SetLoadPort(UpaSelectedLoadPort);

            _controller.StartUnloadProcessModuleActivity(config);
        }

        private void LpaLoadPortCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshLpaSourceSlotCmb();
        }

        private void UpaLoadPortCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshUpaDestinationSlotCmb();
        }

        private void LpaAngleTb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        /// <summary>
        /// Handle the OnValueChanged event from a slider.
        /// </summary>
        /// <param name="sender">The Slider that sent the event</param>
        /// <param name="e">The <see cref="RoutedPropertyChangedEventArgs{T}"/> argument.</param>
        private void RangeBase_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Update the simulation based on the new value
            Agileo.EquipmentModeling.Simulation.Instance.AssignSpeed(Ratio.FromPercent(e.NewValue));

            // Update the label that display the value.
            if (SimulationSpeedLb != null)
            {
                SimulationSpeedLb.Content = ((int)e.NewValue).ToString(CultureInfo.InvariantCulture) + " %";
            }
        }

        /// <summary>
        /// Handle the OnClick event of a button
        /// </summary>
        /// <param name="sender">The button that sent the event</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> argument.</param>
        private void ResetSimulationSpeed_OnClick(object sender, RoutedEventArgs e)
        {
            SimulationSpeedSlider.Value = 100;
        }

        #endregion HMI Handlers

        #region Helpers

        private IEnumerable<Abstractions.Devices.LoadPort.LoadPort> LoadPorts
        {
            get
            {
                var efem = _controller.TryGetDevice<Abstractions.Devices.Efem.Efem>();
                return efem.AllDevices<Abstractions.Devices.LoadPort.LoadPort>();
            }
        }

        private IEnumerable<Abstractions.Devices.DriveableProcessModule.DriveableProcessModule> ProcessModules
        {
            get { return _controller.AllDevices<Abstractions.Devices.DriveableProcessModule.DriveableProcessModule>(); }
        }

        private Abstractions.Devices.LoadPort.LoadPort LpaSelectedLoadPort
        {
            get
            {
                return LoadPorts.SingleOrDefault(lp => lp.Name == LpaLoadPortCmb.SelectedValue?.ToString());
            }
        }

        private byte LpaSelectedSourceSlot
        {
            get
            {
                return Convert.ToByte(LpaSourceSlotCmb.SelectedValue?.ToString());
            }
        }

        private Abstractions.Devices.DriveableProcessModule.DriveableProcessModule LpaSelectedProcessModule
        {
            get
            {
                return ProcessModules.SingleOrDefault(pm => pm.Name == LpaProcessModuleCmb.SelectedValue?.ToString());
            }
        }

        private Abstractions.Devices.LoadPort.LoadPort UpaSelectedLoadPort
        {
            get
            {
                return LoadPorts.SingleOrDefault(lp => lp.Name == UpaLoadPortCmb.SelectedValue?.ToString());
            }
        }

        private byte UpaSelectedDestinationSlot
        {
            get
            {
                return Convert.ToByte(UpaDestinationSlotCmb.SelectedValue?.ToString());
            }
        }

        private Abstractions.Devices.DriveableProcessModule.DriveableProcessModule UpaSelectedProcessModule
        {
            get
            {
                return ProcessModules.SingleOrDefault(pm => pm.Name == UpaProcessModuleCmb.SelectedValue?.ToString());
            }
        }

        #endregion Helpers

        #region Refresh control Helpers

        private void RefreshAvailableLoadPorts()
        {
            LpaLoadPortCmb.ItemsSource = LoadPorts
                .Where(lp =>
                    lp.IsDoorOpen && lp.Carrier != null)
                .Select(lp => lp.Name);

            UpaLoadPortCmb.ItemsSource = LoadPorts
                .Where(lp =>
                    lp.IsDoorOpen && lp.Carrier != null)
                .Select(lp => lp.Name);
        }

        private void RefreshAvailableProcessModules()
        {
            LpaProcessModuleCmb.ItemsSource = ProcessModules
                .Where(pm => pm.Location.Material == null)
                .Select(pm => pm.Name);

            UpaProcessModuleCmb.ItemsSource = ProcessModules
                .Where(pm => pm.Location.Material != null)
                .Select(pm => pm.Name);
        }

        private void RefreshLpaControls()
        {
            RefreshAvailableLoadPorts();
            LpaRobotArmCmb.ItemsSource = new List<string> { RobotArm.Arm1.ToString(), RobotArm.Arm2.ToString() };
            LpaRobotArmCmb.SelectedItem = LpaRobotArmCmb.Items[0];
            LpaAngleTb.Text = "0";
            LpaAlignTypeCmb.ItemsSource = new List<string>
            {
                AlignType.AlignWaferForMainO_FlatCheckingSubO_FlatLocation.ToString(),
                AlignType.AlignWaferForSubO_FlatCheckingSubO_FlatLocation.ToString(),
                AlignType.AlignWaferWithoutCheckingSubO_FlatLocation.ToString()
            };
            LpaAlignTypeCmb.SelectedItem = LpaAlignTypeCmb.Items[0];

            LpaEffectorTypeCmb.ItemsSource = new List<string>
            {
                EffectorType.None.ToString(),
                EffectorType.VacuumI.ToString(),
                EffectorType.VacuumU.ToString(),
                EffectorType.EdgeGrid.ToString(),
                EffectorType.FilmFrame.ToString()
            };
            LpaEffectorTypeCmb.SelectedItem = LpaEffectorTypeCmb.Items[0];
            RefreshAvailableProcessModules();
        }

        private void RefreshLpaSourceSlotCmb()
        {
            LpaSourceSlotCmb.ItemsSource = LpaSelectedLoadPort?.Carrier?.MaterialLocations
                .Where(loc => loc.Material != null)
                .Select(loc => LpaSelectedLoadPort?.Carrier?.MaterialLocations.IndexOf(loc) + 1).ToList();
        }

        private void RefreshUpaControls()
        {
            RefreshAvailableProcessModules();
            UpaRobotArmCmb.ItemsSource = new List<string> { RobotArm.Arm1.ToString(), RobotArm.Arm2.ToString() };
            UpaRobotArmCmb.SelectedItem = UpaRobotArmCmb.Items[0];
            RefreshAvailableLoadPorts();
        }

        private void RefreshUpaDestinationSlotCmb()
        {
            UpaDestinationSlotCmb.ItemsSource = UpaSelectedLoadPort?.Carrier?.MaterialLocations
                .Where(loc => ((SubstrateLocation)loc).Substrate == null)
                .Select(loc => UpaSelectedLoadPort?.Carrier?.MaterialLocations.IndexOf(loc) + 1).ToList();
        }

        #endregion Refresh control Helpers
    }
}
