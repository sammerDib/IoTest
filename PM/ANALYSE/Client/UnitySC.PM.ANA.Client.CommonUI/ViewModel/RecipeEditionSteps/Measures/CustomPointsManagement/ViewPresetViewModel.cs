using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;

using MvvmDialogs;

using UnitySC.Shared.Data;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures.CustomPointsManagement
{
    public class ViewPresetViewModel : ObservableObject, IModalDialogViewModel
    {
        #region Fields

        private readonly IDialogOwnerService _dialogService;
        private bool? _dialogResult;

        #endregion Fields

        #region Public methods

        public ViewPresetViewModel()
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


        private WaferDimensionalCharacteristic _dimentionalCharacteristic ;

        public WaferDimensionalCharacteristic DimentionalCharacteristic
        {
            get => _dimentionalCharacteristic;
            set => SetProperty(ref _dimentionalCharacteristic, value);
        }


        private List<Point> _presetPoints ;

        public List<Point> PresetPoints
        {
            get 
            {
                if (_presetPoints is null)
                    _presetPoints = new List<Point>();
                return _presetPoints;
            }
            set => SetProperty(ref _presetPoints, value);
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
                    () => { return true; }
                    ));
            }
        }



        

        #endregion Public methods
    }
}
