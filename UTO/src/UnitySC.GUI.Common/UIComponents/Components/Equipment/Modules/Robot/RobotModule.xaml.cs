using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using UnitySC.Equipment.Abstractions.Devices.Robot.Enums;
using UnitySC.GUI.Common.Vendor.Views.Panels.EquipmentHandling.Wafer;

namespace UnitySC.GUI.Common.UIComponents.Components.Equipment.Modules.Robot
{
    /// <summary>
    /// Logique d'interaction pour Robot.xaml
    /// </summary>
    public partial class RobotModule
    {
        private readonly TransformGroup _upperWaferRenderTransformGroup = new();
        private readonly TransformGroup _lowerWaferRenderTransformGroup = new();

        private readonly TranslateTransform UpperWaferTranslateTransform = new();
        private readonly TranslateTransform LowerWaferTranslateTransform = new();
        private readonly ScaleTransform _scaleTransform = new();

        public static readonly DependencyProperty LowerArmStateProperty = DependencyProperty.Register(
            nameof(LowerArmState),
            typeof(ArmState),
            typeof(RobotModule),
            new PropertyMetadata(default(ArmState), LowerArmStateChangedCallback));

        private static void LowerArmStateChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RobotModule self)
            {
                self.RefreshLowerWaferPos();
            }
        }

        public ArmState LowerArmState
        {
            get { return (ArmState)GetValue(LowerArmStateProperty); }
            set { SetValue(LowerArmStateProperty, value); }
        }

        public static readonly DependencyProperty UpperArmStateProperty = DependencyProperty.Register(
            nameof(UpperArmState),
            typeof(ArmState),
            typeof(RobotModule),
            new PropertyMetadata(default(ArmState), UpperArmStateChangedCallback));

        private static void UpperArmStateChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RobotModule self)
            {
                self.RefreshUpperWaferPos();
            }
        }

        public ArmState UpperArmState
        {
            get { return (ArmState)GetValue(UpperArmStateProperty); }
            set { SetValue(UpperArmStateProperty, value); }
        }

        public RobotModule()
        {
            InitializeComponent();

            _upperWaferRenderTransformGroup.Children.Add(_scaleTransform);
            _upperWaferRenderTransformGroup.Children.Add(UpperWaferTranslateTransform);

            _lowerWaferRenderTransformGroup.Children.Add(_scaleTransform);
            _lowerWaferRenderTransformGroup.Children.Add(LowerWaferTranslateTransform);

            WaferLower.RenderTransformOrigin = new Point(0.5, 0.5);
            WaferLower.RenderTransform = _lowerWaferRenderTransformGroup;

            WaferUpper.RenderTransformOrigin = new Point(0.5, 0.5);
            WaferUpper.RenderTransform = _upperWaferRenderTransformGroup;

            CenterWafer(WaferLower);
            CenterWafer(WaferUpper);
        }

        private void Viewbox_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var scale = Math.Min(
                ((FrameworkElement)Viewbox.Child).ActualWidth / Viewbox.ActualWidth,
                ((FrameworkElement)Viewbox.Child).ActualHeight / Viewbox.ActualHeight);

            _scaleTransform.ScaleX = _scaleTransform.ScaleY = scale;
        }

        private static void CenterWafer(Wafer wafer)
        {
            var margin = wafer.Margin;

            margin.Left = -wafer.ActualWidth / 2;
            margin.Top = -wafer.ActualHeight / 2;
            wafer.Margin = margin;
        }

        private void WaferUpper_OnSizeChanged(object sender, SizeChangedEventArgs e)
            => CenterWafer((Wafer)sender);

        private void WaferLower_OnSizeChanged(object sender, SizeChangedEventArgs e)
            => CenterWafer((Wafer)sender);

        public void SetAngle(double angle)
        {
            var rotateTransform = (RobotCanvas.RenderTransform as TransformGroup)?.Children
                .OfType<RotateTransform>()
                .Single();

            if (rotateTransform == null || double.IsNaN(angle))
            {
                return;
            }

            rotateTransform.Angle = Normalize(angle, -180, 180);
        }

        /// <summary>
        /// Normalizes any number to an arbitrary range 
        /// by assuming the range wraps around when going below min or above max
        /// </summary>
        private static double Normalize(double value, double start, double end)
        {
            double width = end - start;
            double offsetValue = value - start;   // value relative to 0

            return offsetValue - (Math.Floor(offsetValue / width) * width) + start;
            // + start to reset back to start of original range
        }

        private void RefreshUpperWaferPos()
        {
            UpperWaferTranslateTransform.Y = UpperArmState == ArmState.Extended
                ? -45
                : 0;
        }

        private void RefreshLowerWaferPos()
        {
            LowerWaferTranslateTransform.Y = LowerArmState == ArmState.Extended
                ? -40
                : 0;
        }
    }
}
