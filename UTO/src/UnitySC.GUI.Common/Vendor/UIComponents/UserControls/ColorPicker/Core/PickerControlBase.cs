using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using UnitySC.GUI.Common.Vendor.UIComponents.UserControls.ColorPicker.Models;

namespace UnitySC.GUI.Common.Vendor.UIComponents.UserControls.ColorPicker.Core
{
    public class PickerControlBase : UserControl, IColorStateStorage
    {
        #region Fields

        private bool _ignoreColorPropertyChange;
        private bool _ignoreColorChange;
        private Color _previousColor = System.Windows.Media.Color.FromArgb(5, 5, 5, 5);

        #endregion
        
        public PickerControlBase()
        {
            Color = new NotifiableColor(this);
            Color.PropertyChanged += OnColorPropertyChanged;
            ColorChanged += OnColorChanged;
        }

        #region Properties

        public NotifiableColor Color
        {
            get;
            set;
        }

        #endregion

        #region Event handlers

        private void OnColorPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var newColor = System.Windows.Media.Color.FromArgb(
                (byte)Math.Round(Color.A),
                (byte)Math.Round(Color.RgbR),
                (byte)Math.Round(Color.RgbG),
                (byte)Math.Round(Color.RgbB));

            if (newColor != _previousColor)
            {
                RaiseEvent(new ColorRoutedEventArgs(ColorChangedEvent, newColor));
                _previousColor = newColor;
            }
        }

        private void OnColorChanged(object sender, RoutedEventArgs e)
        {
            if (!_ignoreColorChange && e is ColorRoutedEventArgs colorEvent)
            {
                _ignoreColorPropertyChange = true;
                SelectedColor = colorEvent.Color;
                _ignoreColorPropertyChange = false;
            }
        }

        #endregion

        #region Dependency properties

        public static readonly DependencyProperty ColorStateProperty =
            DependencyProperty.Register(nameof(ColorState), typeof(ColorState), typeof(PickerControlBase),
                new PropertyMetadata(new ColorState(0, 0, 0, 1, 0, 0, 0, 0, 0, 0), OnColorStatePropertyChange));

        private static void OnColorStatePropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (d is PickerControlBase self && args.OldValue is ColorState oldColorState)
            {
                self.Color.UpdateEverything(oldColorState);
            }
        }

        public ColorState ColorState
        {
            get => (ColorState)GetValue(ColorStateProperty);
            set => SetValue(ColorStateProperty, value);
        }

        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register(nameof(SelectedColor), typeof(Color), typeof(PickerControlBase),
                new PropertyMetadata(Colors.Black, OnSelectedColorPropertyChange));

        private static void OnSelectedColorPropertyChange(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (d is PickerControlBase self && args.NewValue is Color newColor)
            {
                if (self._ignoreColorPropertyChange)
                {
                    return;
                }
                
                self._ignoreColorChange = true;
                self.Color.A = newColor.A;
                self.Color.RgbR = newColor.R;
                self.Color.RgbG = newColor.G;
                self.Color.RgbB = newColor.B;
                self._ignoreColorChange = false;
            }
        }

        public Color SelectedColor
        {
            get => (Color)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        public static readonly RoutedEvent ColorChangedEvent =
            EventManager.RegisterRoutedEvent(nameof(ColorChanged),
                RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PickerControlBase));

        public event RoutedEventHandler ColorChanged
        {
            add => AddHandler(ColorChangedEvent, value);
            remove => RemoveHandler(ColorChangedEvent, value);
        }

        #endregion
    }
}
