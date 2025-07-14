using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace UnitySC.GUI.Common.Equipment.LoadPort
{
    /// <summary>
    /// Interaction logic for CarrierIdDisplayer.xaml
    /// </summary>
    public partial class CarrierIdDisplayer
    {
        public CarrierIdDisplayer()
        {
            InitializeComponent();
        }

        #region Properties

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            nameof(Orientation),
            typeof(Orientation),
            typeof(CarrierIdDisplayer),
            new PropertyMetadata(Orientation.Vertical));

        [Category("Main")]
        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public static readonly DependencyProperty HeaderPaddingProperty = DependencyProperty.Register(
            nameof(HeaderPadding),
            typeof(Thickness),
            typeof(CarrierIdDisplayer),
            new PropertyMetadata(default(Thickness)));

        [Category("Main")]
        public Thickness HeaderPadding
        {
            get => (Thickness)GetValue(HeaderPaddingProperty);
            set => SetValue(HeaderPaddingProperty, value);
        }

        #endregion
    }
}
