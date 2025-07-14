using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

using Agileo.UserControls;

namespace UnitySC.Equipment.Abstractions.Devices.LoadPort.Simulation.Controls
{
    /// <summary>
    /// Interaction logic for MappingView.xaml
    /// </summary>
    public partial class MappingView
    {
        #region DependencyProperty

        public static readonly DependencyProperty ClampProperty = DependencyProperty.Register(nameof(ClampLed), typeof(LedState), typeof(MappingView), new PropertyMetadata(LedState.Off));
        public static readonly DependencyProperty ItemUiProperty = DependencyProperty.Register(nameof(ItemUi), typeof(IEnumerable<ItemUi>), typeof(MappingView), new PropertyMetadata(new List<ItemUi>()));
        public static readonly DependencyProperty PresenceLedColorProperty = DependencyProperty.Register(nameof(PresenceLedColor), typeof(Color), typeof(MappingView), new PropertyMetadata(Colors.Black));
        public static readonly DependencyProperty PresenceLedProperty = DependencyProperty.Register(nameof(PresenceLed), typeof(LedState), typeof(MappingView), new PropertyMetadata(LedState.Off));
        public static readonly DependencyProperty MappingVisibilityProperty = DependencyProperty.Register(nameof(MappingVisibility), typeof(Visibility), typeof(MappingView), new PropertyMetadata(Visibility.Hidden));

        #endregion DependencyProperty

        public MappingView()
        {
            InitializeComponent();
        }

        public LedState ClampLed
        {
            get { return (LedState)GetValue(ClampProperty); }
            set { SetValue(ClampProperty, value); }
        }

        public IEnumerable<ItemUi> ItemUi
        {
            get { return (IEnumerable<ItemUi>)GetValue(ItemUiProperty); }
            set { SetValue(ItemUiProperty, value); }
        }

        public LedState PresenceLed
        {
            get { return (LedState)GetValue(PresenceLedProperty); }
            set { SetValue(PresenceLedProperty, value); }
        }

        public Color PresenceLedColor
        {
            get { return (Color)GetValue(PresenceLedColorProperty); }
            set { SetValue(PresenceLedColorProperty, value); }
        }

        public Visibility MappingVisibility
        {
            get { return (Visibility)GetValue(MappingVisibilityProperty); }
            set { SetValue(MappingVisibilityProperty, value); }
        }
    }
}
