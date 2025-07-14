using System;
using System.Windows.Input;

using Agileo.GUI.Commands;
using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Setup.Appearance
{
    public class ThemeViewModel : Notifier
    {
        private readonly Action<string> _applyTheme;

        public string Theme { get; set; }

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetAndRaiseIfChanged(ref _isSelected, value); }
        }

        public ThemeViewModel(string theme, bool isSelected, Action<string> applyTheme)
        {
            _applyTheme = applyTheme;
            Theme = theme;
            IsSelected = isSelected;
            ApplyThemeCommand = new DelegateCommand(ApplyThemeCommandExecute);
        }

        public ICommand ApplyThemeCommand { get; }

        private void ApplyThemeCommandExecute()
        {
            _applyTheme.Invoke(Theme);
        }
    }
}
