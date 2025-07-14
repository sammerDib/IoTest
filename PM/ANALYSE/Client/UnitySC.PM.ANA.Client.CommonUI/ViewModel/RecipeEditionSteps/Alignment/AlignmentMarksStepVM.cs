using System;
using System.Threading.Tasks;

using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Recipe.AlignmentMarks;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.Controls;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps
{
    public class AlignmentMarksStepVM : AlignmentStepBaseVM, IDisposable
    {
        private TaskCompletionSource<bool> _taskExecuteAuto;

        private AlignmentMarksSettings _alignmentMarksSettings = null;

        public AlignmentMarksSettings AlignmentMarksSettings
        {
            get => _alignmentMarksSettings; set { if (_alignmentMarksSettings != value) { _alignmentMarksSettings = value; OnPropertyChanged(); } }
        }
        private bool _displayAlignmentMarksResult = false;
        public bool DisplayAlignmentMarksResult
        {
            get => _displayAlignmentMarksResult; set { if (_displayAlignmentMarksResult != value) { _displayAlignmentMarksResult = value; OnPropertyChanged(); } }
        }
        private Length _resultShiftX = null;

        public Length ResultShiftX
        {
            get => _resultShiftX; set { if (_resultShiftX != value) { _resultShiftX = value; OnPropertyChanged(); } }
        }

        private Length _resultShiftY = null;

        public Length ResultShiftY
        {
            get => _resultShiftY; set { if (_resultShiftY != value) { _resultShiftY = value; OnPropertyChanged(); } }
        }
        private Angle _resultRotationAngle = null;
        public Angle ResultRotationAngle
        {
            get => _resultRotationAngle; set { if (_resultRotationAngle != value) { _resultRotationAngle = value; OnPropertyChanged(); } }
        }


        public AlignmentMarksStepVM(RecipeAlignmentVM recipeAlignmentVM) : base(recipeAlignmentVM)
        {
            ServiceLocator.AlgosSupervisor.AlignmentMarksChangedEvent += AlgosSupervisor_AlignmentMarksChangedEvent;
        }

        private void AlgosSupervisor_AlignmentMarksChangedEvent(AlignmentMarksResult alignmentMarksResult)
        {
            if (_taskExecuteAuto is null)
                return;

            if (_taskExecuteAuto.Task.Status == TaskStatus.RanToCompletion)
                return;
            StepState = GetStepStateFromFlowState(alignmentMarksResult.Status.State);
            ErrorMessage = (alignmentMarksResult.Status.State == FlowState.Error) ? alignmentMarksResult.Status.Message : string.Empty;
            if (alignmentMarksResult.Status.State == FlowState.Success)
            {
                Score = (int)(alignmentMarksResult.Confidence * 100);
                ResultShiftX = alignmentMarksResult.ShiftX;
                ResultShiftY = alignmentMarksResult.ShiftY;
                ResultRotationAngle = alignmentMarksResult.RotationAngle;
                DisplayAlignmentMarksResult = true;
                _taskExecuteAuto.TrySetResult(true);

                return;
            }
            if (alignmentMarksResult.Status.State == FlowState.Error)
            {
                DisplayAlignmentMarksResult = false;
                _taskExecuteAuto.TrySetResult(false);
            }
        }

        public async Task<bool> ExecuteAutoAsync()
        {
            StepState = StepStates.InProgress;
            DisplayAlignmentMarksResult = false;
            _taskExecuteAuto = new TaskCompletionSource<bool>();
            try
            {
                AlignmentMarksInput alignmentMarksInput = new AlignmentMarksInput(AlignmentMarksSettings?.AlignmentMarksSite1, AlignmentMarksSettings?.AlignmentMarksSite2, AlignmentMarksSettings?.AutoFocus);
                alignmentMarksInput.InitialContext = AlignmentMarksSettings?.ObjectiveContext;

                ClassLocator.Default.GetInstance<AlgosSupervisor>().StartAlignmentMarks(alignmentMarksInput);
            }
            catch (Exception)
            {
                StepState = StepStates.Error;
                ErrorMessage = "Failed to start the Alignment Marks";
                return false;
            }

            return await _taskExecuteAuto.Task;
        }

        public void StopExecutionAuto()
        {
            if ((IsInAutomaticMode) && (StepState == StepStates.InProgress))
            {
                Task.Run(() => ClassLocator.Default.GetInstance<AlgosSupervisor>().CancelAlignmentMarks());

                _taskExecuteAuto.TrySetResult(false);
                StepState = StepStates.Error;
                ErrorMessage = "The Alignment Marks has been canceled";
            }
        }

        public void Dispose()
        {
            ServiceLocator.AlgosSupervisor.AlignmentMarksChangedEvent -= AlgosSupervisor_AlignmentMarksChangedEvent;
        }

        protected override Task SubmitManualSettings()
        {
            StepState = StepStates.Done;
            return Task.CompletedTask;
        }
    }
}
