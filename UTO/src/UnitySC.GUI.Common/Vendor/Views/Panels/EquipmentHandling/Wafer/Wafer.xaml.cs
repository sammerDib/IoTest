using System.ComponentModel;
using System.Windows;

using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.EquipmentHandling.Wafer
{
    /// <summary>
    ///     Interaction logic for Wafer.xaml
    /// </summary>
    public partial class Wafer
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            nameof(Source),
            typeof(string),
            typeof(Wafer),
            new PropertyMetadata(""));

        public static readonly DependencyProperty WaferSizeProperty = DependencyProperty.Register(
            nameof(WaferSize),
            typeof(SampleDimension),
            typeof(Wafer),
            new PropertyMetadata(SampleDimension.NoDimension));

        public static readonly DependencyProperty WaferHorizontalAlignmentProperty = DependencyProperty.Register(
            nameof(WaferHorizontalAlignment),
            typeof(HorizontalAlignment),
            typeof(Wafer),
            new PropertyMetadata(HorizontalAlignment.Center));

        public static readonly DependencyProperty HasShadowEffectProperty = DependencyProperty.Register(
            nameof(HasShadowEffect),
            typeof(bool),
            typeof(Wafer),
            new PropertyMetadata(false));

        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register(
            nameof(Status),
            typeof(WaferStatus),
            typeof(Wafer),
            new PropertyMetadata(WaferStatus.None));

        public static readonly DependencyProperty DetectionErrorProperty = DependencyProperty.Register(
            nameof(DetectionError),
            typeof(bool),
            typeof(Wafer),
            new PropertyMetadata(default(bool)));

        public Wafer()
        {
            InitializeComponent();
        }

        [Category("Main")]
        public string Source
        {
            get => (string)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        [Category("Main")]
        public SampleDimension WaferSize
        {
            get => (SampleDimension)GetValue(WaferSizeProperty);
            set => SetValue(WaferSizeProperty, value);
        }

        [Category("Main")]
        public HorizontalAlignment WaferHorizontalAlignment
        {
            get => (HorizontalAlignment)GetValue(WaferHorizontalAlignmentProperty);
            set => SetValue(WaferHorizontalAlignmentProperty, value);
        }

        [Category("Main")]
        public bool HasShadowEffect
        {
            get => (bool)GetValue(WaferHorizontalAlignmentProperty);
            set => SetValue(WaferHorizontalAlignmentProperty, value);
        }

        [Category("Main")]
        public WaferStatus Status
        {
            get => (WaferStatus)GetValue(StatusProperty);
            set => SetValue(StatusProperty, value);
        }

        [Category("Main")]
        public bool DetectionError
        {
            get { return (bool)GetValue(DetectionErrorProperty); }
            set { SetValue(DetectionErrorProperty, value); }
        }
    }
}
