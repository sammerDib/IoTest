using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;

using Agileo.Common.Localization;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Components.Tools;
using Agileo.GUI.Services.Icons;

using Microsoft.Win32;

using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables;
using UnitySC.GUI.Common.Vendor.Views.Tools.ThemeBuilder.ColorPalette;

namespace UnitySC.GUI.Common.Vendor.Views.Tools.ThemeBuilder
{
    public class ThemeBuilderTool : Tool
    {
        #region Properties

        private List<ColorResource> _selectedColors;

        public List<ColorResource> SelectedColors
        {
            get { return _selectedColors; }
            set
            {
                _selectedColors = value;
                if (value.Count > 0)
                {
                    SelectedColor = CloneColor(value[0].Color);
                }

                OnPropertyChanged();
                OnPropertyChanged(nameof(CanModifyColor));
            }
        }

        private Color _selectedColor;

        public Color SelectedColor
        {
            get => _selectedColor;
            set => SetAndRaiseIfChanged(ref _selectedColor, value);
        }

        private bool _isColorPickerEnabled;

        public bool IsColorPickerEnabled
        {
            get => _isColorPickerEnabled;
            set => SetAndRaiseIfChanged(ref _isColorPickerEnabled, value);
        }

        public ColorPaletteViewModel ColorPalette { get; set; }

        public static DataTableSource<ColorResource> ColorsSource { get; } = new();

        public bool CanModifyColor => SelectedColors is { Count: > 0 };

        #endregion Properties

        static ThemeBuilderTool()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(ThemeBuilderResources)));
        }

        public ThemeBuilderTool(string toolName, IIcon icon) : base(toolName, icon)
        {
            ColorsSource.Search.AddSearchDefinition(new InvariantText("Key"), resource => resource.ResourceKey);
            ColorsSource.Search.AddSearchDefinition(new InvariantText("Color"), resource => HexConverter(resource.Color));
            ColorPalette = new ColorPaletteViewModel();
        }

        #region Event handlers

        private void OnThemeApplied(object sender = null, EventArgs e = null)
        {
            ColorsSource.Clear();

            foreach (var color in ThemeHelper.GetColors())
            {
                ColorsSource.Add(new ColorResource(color.Key, color.Value));
            }

            SelectedColors = new List<ColorResource>();
        }

        #endregion

        #region Private methods

        private static string HexConverter(Color c) => $"#{c.A:X2}{c.R:X2}{c.G:X2}{c.B:X2}";

        #endregion

        public void StopColorPicker()
        {
            if (IsColorPickerEnabled && CanModifyColor)
            {
                IsColorPickerEnabled = false;
                ColorPicker();
                ApplyColors();
            }
        }

        /// <summary>
        /// Apply the selected colors in the current ResourceDictionary
        /// </summary>
        public void ApplyColors()
        {
            if (!CanModifyColor) return;

            foreach (var selectedColor in SelectedColors)
            {
                selectedColor.Color = CloneColor(SelectedColor);
            }

            DispatcherHelper.DoInUiThread(() =>
            {
                var dictionary = ColorsSource.ToDictionary(resource => resource.ResourceKey, resource => resource.Color);
                ThemeHelper.ApplyColors(dictionary, false);
            });
        }

        public void ColorPicker()
        {
            if (IsColorPickerEnabled && CanModifyColor)
            {
                SelectedColor = GetColor();
            }
        }

        protected static Color CloneColor(Color value)
        {
            return new Color
            {
                A = value.A,
                R = value.R,
                G = value.G,
                B = value.B
            };
        }

        #region Commands

        private ICommand _saveCommand;

        public ICommand SaveCommand => _saveCommand ??= new DelegateCommand(SaveCommandExecute);

        private void SaveCommandExecute()
        {
            var settings = new XmlWriterSettings { Indent = true };
            var dlg = new SaveFileDialog
            {
                Title = "Export theme",
                Filter = "XAML File|*.xaml"
            };
            if (dlg.ShowDialog() != true) return;
            string path = dlg.FileName;
            var writer = XmlWriter.Create(path, settings);
            XamlWriter.Save(ThemeHelper.GetCurrent(), writer);
            writer.Close();
        }

        private ICommand _loadCommand;

        public ICommand LoadCommand => _loadCommand ??= new DelegateCommand(LoadCommandExecute);

        private void LoadCommandExecute()
        {
            try
            {
                var dlg = new OpenFileDialog
                {
                    Title = "Import theme",
                    Filter = "XAML File|*.xaml"
                };

                if (dlg.ShowDialog() != true) return;

                var myStream = dlg.OpenFile();
                var newDictionary = (ResourceDictionary)XamlReader.Load(myStream);
                if (newDictionary != null)
                {
                    ThemeHelper.Apply(newDictionary);
                }
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private ICommand _colorPickerCommand;

        public ICommand ColorPickerCommand => _colorPickerCommand ??= new DelegateCommand(ColorPickerCommandExecute, () => CanModifyColor);

        private void ColorPickerCommandExecute()
        {
            IsColorPickerEnabled = true;
        }

        private ICommand _cancelColorPickerCommand;

        public ICommand CancelColorPickerCommand => _cancelColorPickerCommand ??= new DelegateCommand(CancelColorPickerCommandExecute);

        private void CancelColorPickerCommandExecute()
        {
            IsColorPickerEnabled = false;
            if (SelectedColors == null || SelectedColors.Count < 1) return;
            SelectedColor = CloneColor(SelectedColors[0].Color);
        }

        private ICommand _applyCommand;

        public ICommand ApplyCommand => _applyCommand ??= new DelegateCommand(ApplyColors, () => CanModifyColor);

        #endregion Commands

        #region Color Picker

        [DllImport("gdi32")]
        private static extern uint GetPixel(IntPtr hDc, int xPos, int yPos);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetCursorPos(out Point pt);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetWindowDC(IntPtr hWnd);

        /// <summary>
        /// Gets the System.Drawing.Color from under the mouse cursor.
        /// </summary>
        /// <returns>The color value.</returns>
        private static Color GetColor()
        {
            var dc = GetWindowDC(IntPtr.Zero);
            GetCursorPos(out var point);
            long color = GetPixel(dc, point.X, point.Y);
            var bytes = LongToBytes(color);
            return Color.FromRgb(bytes[7], bytes[6], bytes[5]);
        }

        private static byte[] LongToBytes(long longValue)
        {
            var result = new byte[8];
            for (var i = 7; i >= 0; i--)
            {
                result[i] = (byte)(longValue & 0xFF);
                longValue >>= 8;
            }
            return result;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public readonly int X;
            public readonly int Y;
            public Point(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        #endregion Color Picker

        #region Overrides of Tool

        public override void OnSetup()
        {
            base.OnSetup();

            ThemeHelper.ThemeApplied += OnThemeApplied;
            OnThemeApplied();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                ThemeHelper.ThemeApplied -= OnThemeApplied;
            }
        }

        #endregion
    }
}
