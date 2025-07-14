using System.ComponentModel;
using System.Windows;

namespace UnitySC.GUI.Common.Equipment.LoadPort
{
    /// <summary>
    /// Interaction logic for LoadPortLedStatesView.xaml
    /// </summary>
    public partial class LoadPortLedStatesView
    {
        public LoadPortLedStatesView()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty LoadPortProperty = DependencyProperty.Register(
            nameof(LoadPort), typeof(UnitySC.Equipment.Abstractions.Devices.LoadPort.LoadPort), typeof(LoadPortLedStatesView), new PropertyMetadata(default(UnitySC.Equipment.Abstractions.Devices.LoadPort.LoadPort)));

        public UnitySC.Equipment.Abstractions.Devices.LoadPort.LoadPort LoadPort
        {
            get { return (UnitySC.Equipment.Abstractions.Devices.LoadPort.LoadPort)GetValue(LoadPortProperty); }
            set { SetValue(LoadPortProperty, value); }
        }

        public static readonly DependencyProperty IsSimplifiedProperty = DependencyProperty.Register(
            nameof(IsSimplified), typeof(bool), typeof(LoadPortLedStatesView), new PropertyMetadata(false));

        [Category("Main")]
        public bool IsSimplified
        {
            get => (bool)GetValue(IsSimplifiedProperty);
            set => SetValue(IsSimplifiedProperty, value);
        }

        public static readonly DependencyProperty ColumnNumberProperty = DependencyProperty.Register(
            nameof(ColumnNumber), typeof(int), typeof(LoadPortLedStatesView), new PropertyMetadata(5));

        [Category("Main")]
        public int ColumnNumber
        {
            get => (int)GetValue(ColumnNumberProperty);
            set => SetValue(ColumnNumberProperty, value);
        }

    }
}
