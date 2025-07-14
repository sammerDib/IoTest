using System;
using System.Threading.Tasks;
using System.Windows;

using UnitySC.DataAccess.Dto;
using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Client.Proxy.Chuck;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.Controls;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps
{
    public class AlignmentEdgesStepVM : AlignmentStepBaseVM, IDisposable
    {
        private TaskCompletionSource<bool> _taskExecuteAuto;

        public AlignmentEdgesStepVM(RecipeAlignmentVM recipeAlignmentVM) : base(recipeAlignmentVM)
        {
            ClassLocator.Default.GetInstance<AlgosSupervisor>().BWAChangedEvent += AlignmentEdgesVM_BWAChangedEvent;
            ClassLocator.Default.GetInstance<AlgosSupervisor>().BWAImageChangedEvent += AlignmentEdgesVM_BWAImageChangedEvent;
            PropertyChanged += AlignmentEdgesVM_PropertyChanged;
        }



        private Angle _resultAngle = null;

        public Angle ResultAngle
        {
            get => _resultAngle; set { if (_resultAngle != value) { _resultAngle = value; OnPropertyChanged(); } }
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

        private Length _resultShiftStageX = null;

        public Length ResultShiftStageX
        {
            get => _resultShiftStageX; set { if (_resultShiftStageX != value) { _resultShiftStageX = value; OnPropertyChanged(); } }
        }

        private Length _resultShiftStageY = null;

        public Length ResultShiftStageY
        {
            get => _resultShiftStageY; set { if (_resultShiftStageY != value) { _resultShiftStageY = value; OnPropertyChanged(); } }
        }

        private Length _resultDiameter = null;

        public Length ResultDiameter
        {
            get => _resultDiameter; set { if (_resultDiameter != value) { _resultDiameter = value; OnPropertyChanged(); } }
        }

        private bool _isEdgeAlignmentInProgress = false;

        public bool IsEdgeAlignmentInProgress
        {
            get => _isEdgeAlignmentInProgress; set { if (_isEdgeAlignmentInProgress != value) { _isEdgeAlignmentInProgress = value; UpdateAllCanExecutes(); OnPropertyChanged(); } }
        }

        private BareWaferAlignmentSettings _settings = null;

        public BareWaferAlignmentSettings Settings
        {
            get
            {
                if (_settings is null)
                {
                    string mainCameraId = ClassLocator.Default.GetInstance<CamerasSupervisor>().GetMainCamera().Configuration.DeviceID;

                    var bareWaferAlignmentInput = new BareWaferAlignmentInput(ClassLocator.Default.GetInstance<ChuckSupervisor>().ChuckVM.SelectedWaferCategory.DimentionalCharacteristic, mainCameraId);

                    // We move to the theoretical position
                    _settings = ClassLocator.Default.GetInstance<AlgosSupervisor>().GetBWASettings(bareWaferAlignmentInput)?.Result;
                }

                return _settings;
            }

            set { if (_settings != value) { _settings = value; OnPropertyChanged(); } }
        }

        private void AlignmentEdgesVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // If we switch back to automatic mode or if the new state is NotDone we set the status of each Edge to NotDone
            if (((e.PropertyName == nameof(IsInAutomaticMode)) && (IsInAutomaticMode))
                || ((e.PropertyName == nameof(StepState)) && (StepState == StepStates.NotDone) && (IsInAutomaticMode)))
            {
                EdgeTop.StepState = StepStates.NotDone;
                EdgeTop.IsManuallyEdited = false;
                EdgeRight.StepState = StepStates.NotDone;
                EdgeRight.IsManuallyEdited = false;
                EdgeBottom.StepState = StepStates.NotDone;
                EdgeBottom.IsManuallyEdited = false;
                EdgeLeft.StepState = StepStates.NotDone;
                EdgeLeft.IsManuallyEdited = false;
            }
        }

        public async Task<bool> ExecuteAutoAsync()
        {

            StepState = StepStates.InProgress;
            IsEdgeAlignmentInProgress = true;

            _taskExecuteAuto = new TaskCompletionSource<bool>();

            try
            {
                string mainCameraId = ServiceLocator.CamerasSupervisor.GetMainCamera().Configuration.DeviceID;
                var bareWaferAlignmentInput = new BareWaferAlignmentInput(ClassLocator.Default.GetInstance<ChuckSupervisor>().ChuckVM.SelectedWaferCategory.DimentionalCharacteristic, mainCameraId);
                bareWaferAlignmentInput.InitialContext = new ObjectiveContext(ObjectiveToUse.DeviceID);

                if (!IsInAutomaticMode)
                {
                    if (EdgeTop.IsManuallyEdited)
                    {
                        bareWaferAlignmentInput.ImageTop = EdgeTop.GetManualServiceImageWithPosition();
                    }
                    else
                    {
                        EdgeTop.Image = null;
                        EdgeTop.StepState = StepStates.NotDone;
                    }
                    if (EdgeRight.IsManuallyEdited)
                    {
                        bareWaferAlignmentInput.ImageRight = EdgeRight.GetManualServiceImageWithPosition();
                    }
                    else
                    {
                        EdgeRight.Image = null;
                        EdgeRight.StepState = StepStates.NotDone;
                    }
                    if (EdgeBottom.IsManuallyEdited)
                    {
                        bareWaferAlignmentInput.ImageBottom = EdgeBottom.GetManualServiceImageWithPosition();
                    }
                    else
                    {
                        EdgeBottom.Image = null;
                        EdgeBottom.StepState = StepStates.NotDone;
                    }
                    if (EdgeLeft.IsManuallyEdited)
                    {
                        bareWaferAlignmentInput.ImageLeft = EdgeLeft.GetManualServiceImageWithPosition();
                    }
                    else
                    {
                        EdgeLeft.Image = null;
                        EdgeLeft.StepState = StepStates.NotDone;
                    }
                }
                ClassLocator.Default.GetInstance<AlgosSupervisor>().StartBWA(bareWaferAlignmentInput);
            }
            catch (Exception)
            {
                StepState = StepStates.Error;
                ErrorMessage = "Failed to start the Bare Wafer Alignment";
                return false;
            }

            return await _taskExecuteAuto.Task;
        }

        public async Task<bool> ExecuteBWAImageAsync(WaferCategory waferCategory, PositionBase position, WaferEdgePositions edgePosition)
        {
            _taskExecuteAuto = new TaskCompletionSource<bool>();

            string mainCameraId = ClassLocator.Default.GetInstance<CamerasSupervisor>().GetMainCamera().Configuration.DeviceID;

            ClassLocator.Default.GetInstance<AlgosSupervisor>().StartBWAImage(new BareWaferAlignmentImageInput(waferCategory.DimentionalCharacteristic, mainCameraId, position, edgePosition));

            return await _taskExecuteAuto.Task;
        }

        private void AlignmentEdgesVM_BWAChangedEvent(BareWaferAlignmentChangeInfo bwaChangeInfo)
        {
            if ((_taskExecuteAuto is null) || (_taskExecuteAuto.Task.Status == TaskStatus.RanToCompletion))
                return;
            StepState = GetStepStateFromFlowState(bwaChangeInfo.Status.State);

            //Image status received
            if (bwaChangeInfo.Status.State == FlowState.InProgress && (bwaChangeInfo is BareWaferAlignmentImage bareWaferAlignmentImage))
            {
                switch (bareWaferAlignmentImage.ImageState)
                {
                    case FlowState.InProgress:
                        var curEdge = GetEdgeFromEdgePosition(bareWaferAlignmentImage.EdgePosition);
                        curEdge.StepState = StepStates.InProgress;
                        break;

                    case FlowState.Success:
                        UpdateImage(bareWaferAlignmentImage);
                        break;
                }
            }

            //BWA terminated with success
            if (bwaChangeInfo.Status.State == FlowState.Success)
            {
                if (bwaChangeInfo is BareWaferAlignmentResult bwaResult)
                {
                    Score = (int)(bwaResult.Confidence * 100);
                    ResultAngle = bwaResult.Angle;
                    ResultShiftX = bwaResult.ShiftX;
                    ResultShiftY = bwaResult.ShiftY;
                    ResultShiftStageX = bwaResult.ShiftStageX;
                    ResultShiftStageY = bwaResult.ShiftStageY;
                    ResultDiameter = bwaResult.Diameter;

                    _taskExecuteAuto.TrySetResult(true);
                    IsEdgeAlignmentInProgress = false;
                }
                return;
            }

            //Error
            if (bwaChangeInfo.Status.State == FlowState.Error)
            {
                Score = 0;
                ErrorMessage = (bwaChangeInfo.Status.State == FlowState.Error) ? bwaChangeInfo.Status.Message : string.Empty;
                _taskExecuteAuto.TrySetResult(false);
                IsEdgeAlignmentInProgress = false;
            }

            UpdateAllCanExecutes();
        }

        private void AlignmentEdgesVM_BWAImageChangedEvent(BareWaferAlignmentChangeInfo bwaChangeInfo)
        {
            if ((_taskExecuteAuto is null) || (_taskExecuteAuto.Task.Status == TaskStatus.RanToCompletion))
                return;
            ErrorMessage = (bwaChangeInfo.Status.State == FlowState.Error) ? bwaChangeInfo.Status.Message : string.Empty;

            if (bwaChangeInfo.Status.State == FlowState.Success)
            {
                if (bwaChangeInfo is BareWaferAlignmentImage)
                {
                    var bareWaferAlignmentImage = bwaChangeInfo as BareWaferAlignmentImage;

                    UpdateImage(bareWaferAlignmentImage);
                    _taskExecuteAuto.TrySetResult(true);
                }
                return;
            }
            if (bwaChangeInfo.Status.State == FlowState.Error)
                _taskExecuteAuto.TrySetResult(false);

            UpdateAllCanExecutes();
        }

        private void UpdateImage(BareWaferAlignmentImage bareWaferAlignmentImage)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                var bitmapSource = bareWaferAlignmentImage.Image?.ConvertToWpfBitmapSource();

                var curEdge = GetEdgeFromEdgePosition(bareWaferAlignmentImage.EdgePosition);
                if (!(curEdge is null))
                {
                    curEdge.ServiceImage = bareWaferAlignmentImage.Image;
                    curEdge.Image = bitmapSource;
                    curEdge.EdgePoints = bareWaferAlignmentImage.EdgePoints;
                    curEdge.NotchLines = bareWaferAlignmentImage.NotchLines;
                    curEdge.StepState = StepStates.Done;
                }
            }));
        }

        private EdgeDetectionVM GetEdgeFromEdgePosition(WaferEdgePositions edgePosition)
        {
            EdgeDetectionVM edgeDetectionVM = null;
            switch (edgePosition)
            {
                case WaferEdgePositions.Top:
                    edgeDetectionVM = EdgeTop;
                    break;

                case WaferEdgePositions.Right:
                    edgeDetectionVM = EdgeRight;
                    break;

                case WaferEdgePositions.Bottom:
                    edgeDetectionVM = EdgeBottom;
                    break;

                case WaferEdgePositions.Left:
                    edgeDetectionVM = EdgeLeft;
                    break;
            }

            return edgeDetectionVM;
        }

        public void StopExecutionAuto()
        {
            if ((IsInAutomaticMode) && (StepState == StepStates.InProgress))
            {
                Task.Run(() => ClassLocator.Default.GetInstance<AlgosSupervisor>().CancelBWA());

                _taskExecuteAuto.TrySetResult(false);
                StepState = StepStates.Error;

                // The edges inProgress are now not done
                EdgeTop.StepState = (EdgeTop.StepState == StepStates.InProgress) ? StepStates.NotDone : EdgeTop.StepState;
                EdgeRight.StepState = (EdgeRight.StepState == StepStates.InProgress) ? StepStates.NotDone : EdgeRight.StepState;
                EdgeBottom.StepState = (EdgeBottom.StepState == StepStates.InProgress) ? StepStates.NotDone : EdgeBottom.StepState;
                EdgeLeft.StepState = (EdgeLeft.StepState == StepStates.InProgress) ? StepStates.NotDone : EdgeLeft.StepState;

                ErrorMessage = "The Blank Wafer Alignment has been canceled";
            }
        }

        public void Dispose()
        {
            ClassLocator.Default.GetInstance<AlgosSupervisor>().BWAChangedEvent -= AlignmentEdgesVM_BWAChangedEvent;
            ClassLocator.Default.GetInstance<AlgosSupervisor>().BWAImageChangedEvent -= AlignmentEdgesVM_BWAImageChangedEvent;
        }

        private EdgeDetectionVM _edgeTop;

        public EdgeDetectionVM EdgeTop
        {
            get
            {
                if (_edgeTop is null)
                {
                    _edgeTop = new EdgeDetectionVM(RecipeAlignment, WaferEdgePositions.Top);
                }
                return _edgeTop;
            }
        }

        private EdgeDetectionVM _edgeRight;

        public EdgeDetectionVM EdgeRight
        {
            get
            {
                if (_edgeRight is null)
                {
                    _edgeRight = new EdgeDetectionVM(RecipeAlignment, WaferEdgePositions.Right);
                }
                return _edgeRight;
            }
        }

        private EdgeDetectionVM _edgeBottom;

        public EdgeDetectionVM EdgeBottom
        {
            get
            {
                if (_edgeBottom is null)
                {
                    _edgeBottom = new EdgeDetectionVM(RecipeAlignment, WaferEdgePositions.Bottom);
                }
                return _edgeBottom;
            }
        }

        private EdgeDetectionVM _edgeLeft;

        public EdgeDetectionVM EdgeLeft
        {
            get
            {
                if (_edgeLeft is null)
                {
                    _edgeLeft = new EdgeDetectionVM(RecipeAlignment, WaferEdgePositions.Left);
                }
                return _edgeLeft;
            }
        }

        protected override bool CanSubmit()
        {
            return (!IsInAutomaticMode) &&
                    (!IsEdgeAlignmentInProgress) &&
                    (StepState == StepStates.InProgress) &&
                    (EdgeTop.StepState == StepStates.Done) &&
                    (EdgeRight.StepState == StepStates.Done) &&
                    (EdgeBottom.StepState == StepStates.Done) &&
                    (EdgeLeft.StepState == StepStates.Done);
        }

        protected override Task SubmitManualSettings()
        {
            // When manual settings are submitted we set stepState to to not Done because there is still an automatic part to do
            StepState = StepStates.NotDone;
            RecipeAlignment.IsModified = true;
            return Task.CompletedTask;
        }
    }
}
