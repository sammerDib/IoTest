using System.Windows.Input;

using Agileo.GUI.Commands;
using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.UIComponents
{
    public class SubNavigationManager : Notifier
    {
        private SubNavigationManager()
        {
        }

        private static SubNavigationManager _instance;

        public static SubNavigationManager Instance => _instance ??= new SubNavigationManager();

        private bool _collapsed;

        public bool Collapsed
        {
            get => _collapsed;
            set => SetAndRaiseIfChanged(ref _collapsed, value);
        }

        private ICommand _toggleCommand;

        public ICommand ToggleCommand => _toggleCommand ??= new DelegateCommand(ToggleCommandExecute);

        private void ToggleCommandExecute() => Collapsed = !Collapsed;
    }
}
