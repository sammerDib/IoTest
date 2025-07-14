using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UnitySC.GUI.Common.Vendor.Views.Tools.ThemeBuilder
{
    public partial class ThemeBuilderToolView
    {
        private ThemeBuilderTool ViewModel => DataContext as ThemeBuilderTool;

        public ThemeBuilderToolView()
        {
            InitializeComponent();
        }

        #region Dependency properties

        public static readonly DependencyProperty ColorPickerEnabledProperty = DependencyProperty.Register(
            nameof(ColorPickerEnabled),
            typeof(bool),
            typeof(ThemeBuilderToolView),
            new PropertyMetadata(default(bool), ColorPickerEnabledChanged));

        private static void ColorPickerEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ThemeBuilderToolView self)
            {
                if (self.ColorPickerEnabled)
                {
                    self.CaptureMouse();
                    Mouse.OverrideCursor = Cursors.Cross;
                }
                else
                {
                    self.ReleaseMouseCapture();
                    Mouse.OverrideCursor = null;
                }
            }
        }

        public bool ColorPickerEnabled
        {
            get { return (bool)GetValue(ColorPickerEnabledProperty); }
            set { SetValue(ColorPickerEnabledProperty, value); }
        }

        #endregion

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.SelectedColors = new List<ColorResource>(RapidColorslistbox.SelectedItems.Cast<ColorResource>());
        }

        private void RapidColorToolView_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            ViewModel?.StopColorPicker();
        }

        private void RapidColorToolView_OnMouseMove(object sender, MouseEventArgs e)
        {
            ViewModel?.ColorPicker();
        }
    }
}
