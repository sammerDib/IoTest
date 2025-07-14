using System.ComponentModel;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;

using MvvmDialogs;

using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures.CustomPointsManagement
{
    public class InputPresetNameViewModel : ObservableObject, IModalDialogViewModel
    {
        #region Fields

        private readonly IDialogOwnerService _dialogService;
        private bool? _dialogResult;

        #endregion Fields

        #region Public methods

        public InputPresetNameViewModel()
        {
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                _dialogService = ClassLocator.Default.GetInstance<IDialogOwnerService>();
            }
        }

        public bool? DialogResult
        {
            get => _dialogResult;
            private set => SetProperty(ref _dialogResult, value, nameof(DialogResult));
        }

        private string _presetName;

        public string PresetName
        {
            get => _presetName;
            set => SetProperty(ref _presetName, value);
        }

        private AutoRelayCommand _okCommand;
        public AutoRelayCommand OkCommand
        {
            get
            {
                return _okCommand
                    ?? (_okCommand = new AutoRelayCommand(
                    () =>
                    {
                        DialogResult = true;
                    },
                    () => !PresetName.IsNullOrEmpty()
                    ));
            }
        }

        #endregion Public methods
    }
}
