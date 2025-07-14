using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.ANA.Service.Interface.Recipe.Execution;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps
{
    
    public class RecipeRunSettingsVM : ObservableObject
    {
        private bool _isAutofocusUsed = false;

        public bool IsAutofocusUsed
        {
            get => _isAutofocusUsed;
            set
            {
                if (_isAutofocusUsed != value)
                {
                    _isAutofocusUsed = value;
                    AutoFocusState.StepState = UnitySC.Shared.UI.Controls.StepStates.InProgress;
                    OnPropertyChanged();
                }
            }
        }

        private StepBaseVM _autofocusState = new StepBaseVM();

        public StepBaseVM AutoFocusState
        {
            get => _autofocusState; set { if (_autofocusState != value) { _autofocusState = value; OnPropertyChanged(); } }
        }

        private bool _isEdgeAlignmentUsed = false;

        public bool IsEdgeAlignmentUsed
        {
            get => _isEdgeAlignmentUsed; set { if (_isEdgeAlignmentUsed != value) { _isEdgeAlignmentUsed = value; OnPropertyChanged(); } }
        }

        private StepBaseVM _edgeAlignmentState = new StepBaseVM();

        public StepBaseVM EdgeAlignmentState
        {
            get => _edgeAlignmentState; set { if (_edgeAlignmentState != value) { _edgeAlignmentState = value; OnPropertyChanged(); } }
        }

        private bool _isMarkAlignmentUsed = false;

        public bool IsMarkAlignmentUsed
        {
            get => _isMarkAlignmentUsed; set { if (_isMarkAlignmentUsed != value) { _isMarkAlignmentUsed = value; OnPropertyChanged(); } }
        }

        private StepBaseVM _markAlignmentState = new StepBaseVM();

        public StepBaseVM MarkAlignmentState
        {
            get => _markAlignmentState; set { if (_markAlignmentState != value) { _markAlignmentState = value; OnPropertyChanged(); } }
        }

        private MeasurementStrategy _currentMeasurementStrategy = MeasurementStrategy.PerMeasurementType;

        public MeasurementStrategy CurrentMeasurementStrategy
        {
            get => _currentMeasurementStrategy; set { if (_currentMeasurementStrategy != value) { _currentMeasurementStrategy = value; OnPropertyChanged(); } }
        }
    }
}
