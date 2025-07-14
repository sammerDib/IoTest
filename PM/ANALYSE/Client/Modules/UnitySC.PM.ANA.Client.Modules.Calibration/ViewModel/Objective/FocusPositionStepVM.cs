using System.Linq;

using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Client.Proxy.Calibration;
using UnitySC.PM.ANA.Client.Proxy.Probe;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.Controls;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.ANA.Client.Modules.Calibration.ViewModel.Objective
{
    public class FocusPositionStepVM : ObjectiveStepBaseVM
    {
        private CalibrationSupervisor _calibrationSupervisor;
        private ProbesSupervisor _probesSupervisor;

        public FocusPositionStepVM(ObjectiveToCalibrateVM objective) : base(objective)
        {
            _calibrationSupervisor = ClassLocator.Default.GetInstance<CalibrationSupervisor>();
            _probesSupervisor = ClassLocator.Default.GetInstance<ProbesSupervisor>();
        }

        private string _information;

        public string Information
        {
            get => _information; set { if (_information != value) { _information = value; OnPropertyChanged(); } }
        }

        private bool _isRunning;

        public bool IsRunning
        {
            get => _isRunning;
            set { if (_isRunning != value) { _isRunning = value; OnPropertyChanged(); } }
        }

        private bool _isEditing = false;

        public bool IsEditing
        {
            get => _isEditing;
            set { if (_isEditing != value) { _isEditing = value; OnPropertyChanged(); } }
        }

        private AutoRelayCommand _edit;

        public AutoRelayCommand Edit
        {
            get
            {
                return _edit ?? (_edit = new AutoRelayCommand(
              () =>
              {
                  SaveResultAndState();
                  IsEditing = true;
                  ErrorMessage = null;
                  Information = null;
                  StepState = StepStates.InProgress;
                  ObjectiveVM.FlowResult = null;
              },
              () => { return StepState != StepStates.InProgress; }));
            }
        }

        private AutoRelayCommand _submit;

        public AutoRelayCommand Submit
        {
            get
            {
                return _submit ?? (_submit = new AutoRelayCommand(
                () =>
                {
                    ErrorMessage = null;

                    var input = new ObjectiveCalibrationInput(
                            objectiveId: ObjectiveVM.Id,
                            probeId: null,
                            previousCalibration: null,
                            gain: double.NaN);
                    var probe = _probesSupervisor.Probes.OfType<ProbeLiseVM>().Single(x => x.Configuration.ModulePosition == ObjectiveVM.Position);
                    if (probe == null)
                    {
                        StepState = StepStates.Error;
                        ErrorMessage = $"Lise doesn't exit for {ObjectiveVM.Position}";
                        return;
                    }

                    if (ObjectiveVM.Position == PM.Shared.Hardware.Service.Interface.ModulePositions.Up && !ObjectiveVM.IsMain)
                    {
                        if (ObjectiveVM.OpticalReferenceElevationFromStandardWafer == null)
                        {
                            StepState = StepStates.Error;
                            ErrorMessage = $"Optical reference elevation from wafer is missing for main objective";
                            return;
                        }
                        input.OpticalReferenceElevationFromStandardWafer = ObjectiveVM.OpticalReferenceElevationFromStandardWafer;
                    }
                    input.ProbeId = probe.DeviceID;
                    input.Gain = probe.InputParametersLise.Gain;
                    ObjectiveVM.VMToCalibrationData();
                    input.PreviousCalibration = ObjectiveVM.CalibrationData;

                    _calibrationSupervisor.ObjectiveCalibrationEvent += ObjectiveCalibrationEvent;
                    _calibrationSupervisor.StartObjectiveCalibration(input);
                    StepState = StepStates.InProgress;
                    IsRunning = true;

                    Edit.NotifyCanExecuteChanged();
                    ObjectiveVM.Update();
                },
                () => { return true; }));
            }
        }

        private AutoRelayCommand _cancel;

        public AutoRelayCommand Cancel
        {
            get
            {
                return _cancel ?? (_cancel = new AutoRelayCommand(
              () =>
              {
                  IsRunning = false;
                  IsEditing = false;
                  RestoreResultAndState();
                  Information = null;
                  ErrorMessage = null;
                  _calibrationSupervisor.CancelObjectiveCalibration();
                  _calibrationSupervisor.ObjectiveCalibrationEvent -= ObjectiveCalibrationEvent;
              },

              () => { return true; }));
            }
        }

        private void ObjectiveCalibrationEvent(ObjectiveCalibrationResult objectiveCalibrationResult)
        {
            if (objectiveCalibrationResult.Status.IsFinished)
            {
                IsRunning = false;
                if (objectiveCalibrationResult.Status.State == FlowState.Success)
                {
                    StepState = StepStates.Done;
                    ObjectiveVM.FlowResult = objectiveCalibrationResult;
                    Information = null;
                    ErrorMessage = null;
                }
                else
                {
                    StepState = StepStates.Error;
                    ErrorMessage = objectiveCalibrationResult.Status.Message;
                    Information = ErrorMessage;
                    ObjectiveVM.FlowResult = null;
                }
                IsEditing = false;
                _calibrationSupervisor.ObjectiveCalibrationEvent -= ObjectiveCalibrationEvent;
            }
            else if (objectiveCalibrationResult.Status.State == FlowState.InProgress)
            {
                Information = objectiveCalibrationResult.Status.Message;
            }

            Edit.NotifyCanExecuteChanged();
        }
    }
}
