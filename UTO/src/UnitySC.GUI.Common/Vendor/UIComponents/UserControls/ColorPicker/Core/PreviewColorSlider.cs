using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.UserControls.ColorPicker.Models;

namespace UnitySC.GUI.Common.Vendor.UIComponents.UserControls.ColorPicker.Core
{
    internal abstract class PreviewColorSlider : Slider, INotifyPropertyChanged
    {
        #region Fields

        private SolidColorBrush _leftCapColor = new();
        private SolidColorBrush _rightCapColor = new();

        private readonly LinearGradientBrush _backgroundBrush = new();

        #endregion

        protected PreviewColorSlider()
        {
            PreviewMouseWheel += OnPreviewMouseWheel;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        #region Properties

        public GradientStopCollection BackgroundGradient
        {
            get => _backgroundBrush.GradientStops;
            set => _backgroundBrush.GradientStops = value;
        }

        public SolidColorBrush LeftCapColor
        {
            get => _leftCapColor;
            set
            {
                _leftCapColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LeftCapColor)));
            }
        }

        public SolidColorBrush RightCapColor
        {
            get => _rightCapColor;
            set
            {
                _rightCapColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RightCapColor)));
            }
        }

        #endregion

        #region Dependency properties

        public static readonly DependencyProperty CurrentColorStateProperty =
            DependencyProperty.Register(nameof(CurrentColorState), typeof(ColorState), typeof(PreviewColorSlider),
                new PropertyMetadata(ColorStateChangedCallback));

        protected static void ColorStateChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PreviewColorSlider slider)
            {
                slider.GenerateBackground();
            }
        }

        public ColorState CurrentColorState
        {
            get => (ColorState)GetValue(CurrentColorStateProperty);
            set => SetValue(CurrentColorStateProperty, value);
        }

        public static readonly DependencyProperty SmallChangeBindableProperty =
            DependencyProperty.Register(nameof(SmallChangeBindable), typeof(double), typeof(PreviewColorSlider),
                new PropertyMetadata(1.0, SmallChangeBindableChangedCallback));

        private static void SmallChangeBindableChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PreviewColorSlider slider)
            {
                slider.SmallChange = slider.SmallChangeBindable;
            }
        }

        public double SmallChangeBindable
        {
            get => (double)GetValue(SmallChangeBindableProperty);
            set => SetValue(SmallChangeBindableProperty, value);
        }

        #endregion

        #region Event handlers

        private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs args)
        {
            Value = MathHelper.Clamp(Value + SmallChange * args.Delta / 120, Minimum, Maximum);
            args.Handled = true;
        }

        #endregion
        
        public override void EndInit()
        {
            base.EndInit();
            Background = _backgroundBrush;
            GenerateBackground();
        }

        protected abstract void GenerateBackground();
    }
}
