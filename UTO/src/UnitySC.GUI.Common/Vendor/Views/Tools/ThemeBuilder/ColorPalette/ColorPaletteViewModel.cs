using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;

using Agileo.GUI.Commands;
using Agileo.GUI.Components;

using UnitySC.GUI.Common.Vendor.Helpers;

namespace UnitySC.GUI.Common.Vendor.Views.Tools.ThemeBuilder.ColorPalette
{
    public class ColorPaletteViewModel : Notifier
    {
        #region Commands

        public ICommand ChangeThemeCommand => _setColorCommand ??= new DelegateCommand<Color[]>(ChangeThemeCommandExecute);

        private ICommand _setColorCommand;

        private static void ChangeThemeCommandExecute(Color[] colors)
        {
            if (colors.Length < 12) return;

            var dictionary = new Dictionary<string, Color>
            {
                { "WindowBackground", colors[0] },
                { "PanelForeground", colors[1] },
                { "PanelBackground", colors[2] },
                { "ControlActionForeground", colors[3] },
                { "HeaderForeground", colors[4] },
                { "ControlActionBackground", colors[5] },
                { "HorizontalCanvasBackground", colors[6] },
                { "HeaderBackground", colors[7] },
                { "VerticalCanvasBackground", colors[8] },
                { "HorizontalCanvasForeground", colors[9] },
                { "VerticalCanvasForeground", colors[10] },
                { "ControlInputBackground", colors[11] }
            };

            ThemeHelper.ApplyColors(dictionary, true);
        }

        private ICommand _changeAccentCommand;

        public ICommand ChangeAccentCommand => _changeAccentCommand ??= new DelegateCommand<Color[]>(ChangeAccentCommandExecute);

        private static void ChangeAccentCommandExecute(Color[] colors)
        {
            var dictionary = new Dictionary<string, Color>
            {
                { "SelectionForeground", colors[0] },
                { "SelectionBackground", colors[1] }
            };

            ThemeHelper.ApplyColors(dictionary, true);
        }

        #endregion Commands
    }
}
