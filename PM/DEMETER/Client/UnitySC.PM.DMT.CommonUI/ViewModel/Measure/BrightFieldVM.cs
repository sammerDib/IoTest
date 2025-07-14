using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media;

using UnitySC.PM.DMT.CommonUI.Proxy;
using UnitySC.PM.DMT.CommonUI.ViewModel.ExposureSettings;
using UnitySC.PM.DMT.Service.Interface.AutoExposure;
using UnitySC.PM.DMT.Shared.UI.Proxy;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;
using UnitySC.Shared.UI.Enum;

namespace UnitySC.PM.DMT.CommonUI.ViewModel.Measure
{
    public class BrightFieldVM : MeasureVM
    {
        private readonly CameraSupervisor _cameraSupervisor;
        private readonly ScreenSupervisor _screenSupervisor;
        private readonly CalibrationSupervisor _calibrationSupervisor;
        private readonly AlgorithmsSupervisor _algorithmsSupervisor;
        private readonly IDialogOwnerService _dialogService;
        private readonly Mapper _mapper;
        private readonly MainRecipeEditionVM _mainRecipeEditionVM;

        public BrightFieldVM(CameraSupervisor cameraSupervisor, ScreenSupervisor screenSupervisor, CalibrationSupervisor calibrationSupervisor,
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

            Title = "Bright-field";
        }

        public override HelpTag HelpTag => HelpTag.DMT_Reflecto;

        private bool _applyUniformityCorrection;

        public bool ApplyUniformityCorrection
        {
            get => _applyUniformityCorrection; set { if (_applyUniformityCorrection != value) { _applyUniformityCorrection = value; OnPropertyChanged(); } }
        }

        private bool _canApplyUniformityCorrection = false;

        public bool CanApplyUniformityCorrection
        {
            get => _canApplyUniformityCorrection;
            set { if (_canApplyUniformityCorrection != value) { _canApplyUniformityCorrection = value; OnPropertyChanged(); } }
        }

        private Color _color;

        public override Color Color
        {
            get => _color; set { if (_color != value) { _color = value; OnPropertyChanged(); } }
        }

        private IEnumerable<Color> _availableColors;
        public IEnumerable<Color> AvailableColors => _availableColors ?? (_availableColors = _screenSupervisor.GetAvailableColors());

        private AutoRelayCommand<string> _tuneExposureTimeCommand;

        public AutoRelayCommand<string> TuneExposureTimeCommand
        {
            get
            {
                return _tuneExposureTimeCommand ?? (_tuneExposureTimeCommand = new AutoRelayCommand<string>(
                title =>
                {
                    var vm = new ManualExposureSettingsVMForBrightField(title, this, _cameraSupervisor, _screenSupervisor, _calibrationSupervisor,
                                                                        _algorithmsSupervisor, _dialogService, _mapper, _mainRecipeEditionVM);
                    _mainRecipeEditionVM.EditedRecipe.Navigate(vm);
                }));
            }
        }

        public override void PrepareDisplay()
        {
            CanApplyUniformityCorrection = _calibrationSupervisor.HasUniformityCorrectionCalibration(Side);
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName == nameof(AutoExposureTimeTrigger))
            {
                if (AutoExposureTimeTrigger == AutoExposureTimeTrigger.Never)
                {
                    ApplyUniformityCorrection = false;
                }
            }
        }
    }
}
