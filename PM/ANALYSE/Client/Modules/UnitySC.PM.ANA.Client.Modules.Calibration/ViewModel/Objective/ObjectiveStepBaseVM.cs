using UnitySC.PM.ANA.Client.Proxy.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.Controls;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.PM.ANA.Client.Modules.Calibration.ViewModel.Objective
{
    public abstract class ObjectiveStepBaseVM : ViewModelBaseExt
    {
        protected ObjectiveStepBaseVM(ObjectiveToCalibrateVM objectiveToCalibrateVM)
        {
            ObjectiveVM = objectiveToCalibrateVM;
        }

        public ObjectiveToCalibrateVM ObjectiveVM { get; private set; }

        private StepStates _stepState = StepStates.NotDone;

        private StepStates _savedStepState;
        private bool _savedValidation;
        private LengthVM _savedPixelSizeResult;
        private ImageParametersVM _savedCentricityResult;
        private XYPosition _savedRefPos;

        public StepStates StepState
        {
            get => _stepState;
            set
            {
                if (_stepState != value)
                {
                    _stepState = value;
                    if (_stepState != StepStates.Error)
                    {
                        ErrorMessage = null;
                    }
                    UpdateAllCanExecutes();
                    OnPropertyChanged();
                }
            }
        }

        protected virtual void SaveResultAndState()
        {
            _savedPixelSizeResult = ObjectiveVM?.Image?.PixelSize == null ? null : new LengthVM(ObjectiveVM.Image.PixelSize.Length);

            if (ObjectiveVM?.Image != null)
            {
                _savedCentricityResult = new ImageParametersVM()
                {
                    CentricitiesRefPosition = ObjectiveVM?.Image?.CentricitiesRefPosition,
                    XOffset = ObjectiveVM?.Image?.XOffset,
                    YOffset = ObjectiveVM?.Image?.YOffset
                };
            }
            _savedRefPos = (XYPosition)ObjectiveVM.CentricityStep.RefPos?.Clone();

            switch (StepState)
            {
                case StepStates.InProgress:
                    _savedStepState = StepStates.InProgress;
                    break;

                case StepStates.Error:
                    _savedStepState = StepStates.Error;
                    break;

                case StepStates.Done:
                    _savedStepState = StepStates.Done;
                    break;

                case StepStates.NotDone:
                    _savedStepState = StepStates.NotDone;
                    break;
            }
            _savedValidation = ObjectiveVM.IsValidated;
        }

        protected virtual void RestoreResultAndState()
        {
            if (_savedPixelSizeResult != null)
            {
                ObjectiveVM.Image.PixelSize.Length = _savedPixelSizeResult.Length;
                _savedPixelSizeResult = null;
            }

            if (_savedCentricityResult != null)
            {
                ObjectiveVM.Image.CentricitiesRefPosition = _savedCentricityResult.CentricitiesRefPosition;
                ObjectiveVM.Image.XOffset = _savedCentricityResult.XOffset;
                ObjectiveVM.Image.YOffset = _savedCentricityResult.YOffset;
                _savedCentricityResult = null;
            }
            if (_savedRefPos != null)
            {
                ObjectiveVM.CentricityStep.RefPos = _savedRefPos;
                ObjectiveVM.CentricityStep.RefPosX = new LengthVM(ObjectiveVM.CentricityStep.RefPos.X, LengthUnit.Millimeter);
                ObjectiveVM.CentricityStep.RefPosY = new LengthVM(ObjectiveVM.CentricityStep.RefPos.Y, LengthUnit.Millimeter);
                var correctedPosition = (XYPosition)ObjectiveVM.CentricityStep.RefPos.Clone();
                correctedPosition.X += ObjectiveVM?.Image?.XOffset?.Value ?? 0;
                correctedPosition.Y += ObjectiveVM?.Image?.YOffset?.Value ?? 0;
                ClassLocator.Default.GetInstance<AxesSupervisor>().GotoPosition(correctedPosition, AxisSpeed.Normal);
                _savedRefPos = null;
            }
            ObjectiveVM.IsValidated = _savedValidation;
            StepState = _savedStepState;
        }

        private string _errorMessage = null;

        public string ErrorMessage
        {
            get => _errorMessage; set { if (_errorMessage != value) { _errorMessage = value; OnPropertyChanged(); } }
        }
    }
}
