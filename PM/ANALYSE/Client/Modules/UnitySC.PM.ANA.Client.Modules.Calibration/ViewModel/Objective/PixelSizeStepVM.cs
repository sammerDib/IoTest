using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.Shared.UI.Controls;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.PM.ANA.Client.Modules.Calibration.ViewModel.Objective
{
    public class PixelSizeStepVM : ObjectiveStepBaseVM
    {
        public PixelSizeStepVM(ObjectiveToCalibrateVM objectiveVM) : base(objectiveVM)
        {
        }

        private LengthVM _pixelSize;

        public LengthVM PixelSize
        {
            get => _pixelSize;
            set { if (_pixelSize != value) { _pixelSize = value; OnPropertyChanged(); } }
        }

        private bool _isEditing = false;

        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                if (_isEditing != value)
                {
                    _isEditing = value;
                    OnPropertyChanged();
                }
            }
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
                  StepState = StepStates.InProgress;
              },
              () => { return ObjectiveVM.FocusPositionStep.StepState == StepStates.Done && StepState != StepStates.InProgress; }));
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
                  StepState = StepStates.Done;
                  IsEditing = false;
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
                  RestoreResultAndState();
                  IsEditing = false;
              },

              () => { return true; }));
            }
        }
    }
}
