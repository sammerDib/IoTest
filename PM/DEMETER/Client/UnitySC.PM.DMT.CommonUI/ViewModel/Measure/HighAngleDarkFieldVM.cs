using UnitySC.PM.DMT.CommonUI.Proxy;
using UnitySC.PM.DMT.CommonUI.ViewModel.ExposureSettings;
using UnitySC.PM.DMT.Shared.UI.Proxy;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;
using UnitySC.Shared.UI.Enum;

namespace UnitySC.PM.DMT.CommonUI.ViewModel.Measure
{
    public class HighAngleDarkFieldVM : MeasureVM
    {
        public override HelpTag HelpTag => HelpTag.DMT_ObliqueLight;

        public override bool IsTargetSaturationEditable
        {
            get { return AutoExposureTimeTrigger != Service.Interface.AutoExposure.AutoExposureTimeTrigger.Never; }
        }

        private AutoRelayCommand<string> _tuneExposureTimeCommand;

        private readonly CameraSupervisor _cameraSupervisor;
        private readonly ScreenSupervisor _screenSupervisor;
        private readonly CalibrationSupervisor _calibrationSupervisor;
        private readonly AlgorithmsSupervisor _algorithmsSupervisor;
        private readonly IDialogOwnerService _dialogService;
        private readonly Mapper _mapper;
        private readonly MainRecipeEditionVM _mainRecipeEditionVM;

        public HighAngleDarkFieldVM(CameraSupervisor cameraSupervisor, ScreenSupervisor screenSupervisor, CalibrationSupervisor calibrationSupervisor,
            AlgorithmsSupervisor algorithmsSupervisor, IDialogOwnerService dialogService, Mapper mapper,
            MainRecipeEditionVM mainRecipeEditionVM)
        {
            _cameraSupervisor = cameraSupervisor;
            _screenSupervisor = screenSupervisor;
            _calibrationSupervisor = calibrationSupervisor;
            _algorithmsSupervisor = algorithmsSupervisor;
            _dialogService = dialogService;
            _mapper = mapper;
            _mainRecipeEditionVM = mainRecipeEditionVM;
            _canApplyHighAngleDarkFieldCalibration = false;

            Title = "High angle dark-field";
        }

        public AutoRelayCommand<string> TuneExposureTimeCommand
        {
            get
            {
                return _tuneExposureTimeCommand ?? (_tuneExposureTimeCommand = new AutoRelayCommand<string>(
                title =>
                {
                    ManualExposureSettingsVMForHighAngleDarkFieldVM vm =
                    new ManualExposureSettingsVMForHighAngleDarkFieldVM(title, this, _cameraSupervisor, _screenSupervisor, _calibrationSupervisor,
                                                                _algorithmsSupervisor, _dialogService, _mapper, _mainRecipeEditionVM);
                    _mainRecipeEditionVM.EditedRecipe.Navigate(vm);
                }));
            }
        }

        private bool _canApplyHighAngleDarkFieldCalibration;

        public bool CanApplyHighAngleDarkFieldCalibration
        {
            get => _canApplyHighAngleDarkFieldCalibration;
            set { if (_canApplyHighAngleDarkFieldCalibration != value) { _canApplyHighAngleDarkFieldCalibration = value; OnPropertyChanged(); } }
        }

        public override void PrepareDisplay()
        {
            CanApplyHighAngleDarkFieldCalibration = _calibrationSupervisor.IsHighAngleDarkFieldMaskAvailableForSide(Side);
        }
    }
}
