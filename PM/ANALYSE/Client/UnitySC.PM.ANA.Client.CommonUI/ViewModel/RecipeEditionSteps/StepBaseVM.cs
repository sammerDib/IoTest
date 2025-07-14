using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.DataAccess.Dto;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.UI.Controls;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps
{
    public class StepBaseVM : VMSharedBase
    {
        private StepStates _stepState = StepStates.NotDone;

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

        private string _errorMessage = null;

        public string ErrorMessage
        {
            get => _errorMessage; set { if (_errorMessage != value) { _errorMessage = value; OnPropertyChanged(); } }
        }

        public static StepStates GetStepStateFromFlowState(FlowState flowState)
        {
            switch (flowState)
            {
                case FlowState.Canceled:
                case FlowState.Waiting:
                    return StepStates.NotDone;

                case FlowState.InProgress:
                    return StepStates.InProgress;

                case FlowState.Error:
                    return StepStates.Error;

                case FlowState.Success:
                    return StepStates.Done;

                default:
                    return StepStates.NotDone;
            }
        }
    }
}
