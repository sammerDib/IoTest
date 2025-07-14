using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UnitySC.Equipment.Abstractions.Vendor.Devices.Simulation
{
    /// <summary>
    /// Logique d'interaction pour SimulatedRampUserControl.xaml
    /// </summary>
    public partial class SimulatedRampUserControl : UserControl
    {
        public SimulatedRampUserControl()
        {
            InitializeComponent();
        }

        public float StatusName
        {
            get { return (float)GetValue(StatusNameProperty); }
            set { SetValue(StatusNameProperty, value); }
        }

        public static readonly DependencyProperty StatusNameProperty =
            DependencyProperty.Register("StatusName", typeof(string), typeof(SimulatedRampUserControl), new PropertyMetadata(string.Empty));




        public float InitialValue
        {
            get { return (float)GetValue(InitialValueProperty); }
            set { SetValue(InitialValueProperty, value); }
        }

        public static readonly DependencyProperty InitialValueProperty =
            DependencyProperty.Register("InitialValue", typeof(float), typeof(SimulatedRampUserControl), new PropertyMetadata(float.NaN));




        public int RefreshPeriod
        {
            get { return (int)GetValue(RefreshPeriodProperty); }
            set { SetValue(RefreshPeriodProperty, value); }
        }

        public static readonly DependencyProperty RefreshPeriodProperty =
            DependencyProperty.Register("RefreshPeriod", typeof(int), typeof(SimulatedRampUserControl), new PropertyMetadata(0));




        public bool DeactivateRamp
        {
            get { return (bool)GetValue(DeactivateRampProperty); }
            set { SetValue(DeactivateRampProperty, value); }
        }

        public static readonly DependencyProperty DeactivateRampProperty =
            DependencyProperty.Register("DeactivateRamp", typeof(bool), typeof(SimulatedRampUserControl), new PropertyMetadata(false));




        public float SetPoint
        {
            get { return (float)GetValue(SetPointProperty); }
            set { SetValue(SetPointProperty, value); }
        }

        public static readonly DependencyProperty SetPointProperty =
            DependencyProperty.Register("SetPoint", typeof(float), typeof(SimulatedRampUserControl), new PropertyMetadata(float.NaN));




        public float Speed
        {
            get { return (float)GetValue(SpeedProperty); }
            set { SetValue(SpeedProperty, value); }
        }

        public static readonly DependencyProperty SpeedProperty =
            DependencyProperty.Register("Speed", typeof(float), typeof(SimulatedRampUserControl), new PropertyMetadata(float.NaN));





        public float Value
        {
            get { return (float)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(float), typeof(SimulatedRampUserControl), new PropertyMetadata(float.NaN));




        public bool IsStarted
        {
            get { return (bool)GetValue(IsStartedProperty); }
            set { SetValue(IsStartedProperty, value); }
        }

        public static readonly DependencyProperty IsStartedProperty =
            DependencyProperty.Register("IsStarted", typeof(bool), typeof(SimulatedRampUserControl), new PropertyMetadata(false));





        public ICommand CmdStart
        {
            get { return (ICommand)GetValue(CmdStartProperty); }
            set { SetValue(CmdStartProperty, value); }
        }

        public static readonly DependencyProperty CmdStartProperty =
            DependencyProperty.Register("CmdStart", typeof(ICommand), typeof(SimulatedRampUserControl));




        public ICommand CmdStop
        {
            get { return (ICommand)GetValue(CmdStopProperty); }
            set { SetValue(CmdStopProperty, value); }
        }

        public static readonly DependencyProperty CmdStopProperty =
            DependencyProperty.Register("CmdStop", typeof(ICommand), typeof(SimulatedRampUserControl));
    }
}
