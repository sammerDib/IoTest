using System.Threading.Tasks;

using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.PM.ANA.Service.Interface;
using UnitySC.Shared.UI.Controls;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps
{
    public abstract class AlignmentStepBaseVM : StepBaseVM
    {
        private bool _isInAutomaticMode = true;

        // Automatic or Manual Mode
        public bool IsInAutomaticMode
        {
            get => _isInAutomaticMode;
            set
            {
                if (_isInAutomaticMode != value)
                {
                    RecipeAlignment.StopAutoAlignment.Execute(null);
                    _isInAutomaticMode = value;
                    if (!_isInAutomaticMode)
                    {
                        StepState = StepStates.InProgress;
                    }
                    if (_isInAutomaticMode)
                        StepState = StepStates.NotDone;

                    UpdateAllCanExecutes();
                    OnPropertyChanged();
                }
            }
        }

        public bool IsManualInProgress => !IsInAutomaticMode && StepState == StepStates.InProgress;


        private int _score = 0;

        public int Score
        {
            get => _score; set { if (_score != value) { _score = value; OnPropertyChanged(); } }
        }

        private ObjectiveConfig _objectiveToUse;

        public ObjectiveConfig ObjectiveToUse
        {
            get => _objectiveToUse; set { if (_objectiveToUse != value) { _objectiveToUse = value; OnPropertyChanged(); } }
        }

        private AutoRelayCommand _edit;

        public AutoRelayCommand Edit
        {
            get
            {
                return _edit ?? (_edit = new AutoRelayCommand(
                    () =>
                    {
                        RecipeAlignment.StopAutoAlignment.Execute(null);
                        StepState = StepStates.InProgress;
                    },
                    () => { return ((!IsInAutomaticMode) && (StepState != StepStates.InProgress)); }
                ));
            }
        }

        private AutoRelayCommand _submit;

        public AlignmentStepBaseVM(RecipeAlignmentVM recipeAlignmentVM)
        {
            RecipeAlignment = recipeAlignmentVM;
        }

        public AutoRelayCommand Submit
        {
            get
            {
                return _submit ?? (_submit = new AutoRelayCommand(
                    async () =>
                    {
                        await SubmitManualSettings();
                        // We restart the auto alignment
                        // "_ =" to disable warning CS4014
                        _ = RecipeAlignment.ExecuteAutoAlignment();
                    },
                    () => { return CanSubmit(); }
                ));
            }
        }

        protected virtual bool CanSubmit()
        {
            return ((!IsInAutomaticMode) && (StepState == StepStates.InProgress));
        }

        public RecipeAlignmentVM RecipeAlignment { get; }

        protected abstract Task SubmitManualSettings();
    }
}
