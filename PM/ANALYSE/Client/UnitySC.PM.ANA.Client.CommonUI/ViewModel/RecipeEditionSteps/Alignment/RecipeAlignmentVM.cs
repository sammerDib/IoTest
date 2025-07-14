using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Client.Proxy.Recipe;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Recipe.Alignment;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Controls;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps
{
    public class RecipeAlignmentVM : RecipeWizardStepBaseVM, IDisposable
    {
        private ANARecipeVM _editedRecipe;

        public INavigationManager NavigationManager => ServiceLocator.NavigationManager;

        public RecipeAlignmentVM(ANARecipeVM editedRecipe)
        {
            Name = "Preparation";
            IsEnabled = true;
            IsMeasure = false;
            IsValidated = false;

            _editedRecipe = editedRecipe;
            LoadRecipeData();
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return;

            ServiceLocator.ProbesSupervisor.SubscribeToProbeChanges();
        }

        #region INavigable

        public override async Task PrepareToDisplay()
        {
            FocusStep.PropertyChanged += Step_PropertyChanged;
            LightStep.PropertyChanged += Step_PropertyChanged;
            EdgesStep.PropertyChanged += Step_PropertyChanged;
            AlignmentMarksStep.PropertyChanged += Step_PropertyChanged;

            ServiceLocator.CamerasSupervisor.GetMainCamera()?.StartStreaming();

            // We go to the center of the wafer and Z bottom goes as far as possible from the wafer
            await GotoTheCenterOfTheWafer();

            // We select the main objective
            var objectiveToUse = ServiceLocator.CamerasSupervisor.MainObjective;
            ServiceLocator.CamerasSupervisor.Objective = objectiveToUse;
            await Task.Run(() => ServiceLocator.AxesSupervisor.WaitMotionEnd(20_000));

            if (_editedRecipe.Alignment?.AutoFocusLise is null)
            {
                var probeLiseUp = ServiceLocator.ProbesSupervisor.ProbeLiseUp;
                var afLiseSettings = ClassLocator.Default.GetInstance<AlgosSupervisor>().GetAFLiseSettings(new AFLiseInput(probeLiseUp.DeviceID), objectiveToUse)?.Result;
                if (!(afLiseSettings is null))
                {
                    if (!(afLiseSettings.ZMax is null))
                        FocusStep.ZMaxParameter = afLiseSettings.ZMax.Millimeters;
                    if (!(afLiseSettings.ZMin is null))
                        FocusStep.ZMinParameter = afLiseSettings.ZMin.Millimeters;
                }
            }

            if (!IsValidated)
                StartAutoAlignment.Execute(null);
        }

        private static async Task GotoTheCenterOfTheWafer()
        {
            // do we still need to move zbottom far away espicially if in future we also need to find auti Z focus of the bottom ?
            //var minZBottomPosition = ServiceLocator.AxesSupervisor.GetAxesConfiguration()?.Result.AxisConfigs.Find(a => a.MovingDirection == MovingDirection.ZBottom).PositionMin.Millimeters + 0.1 ?? double.NaN;
            //ServiceLocator.AxesSupervisor.GotoPosition(new XYZTopZBottomPosition(new StageReferential(), chuckcenterX, chuckcenterY, double.NaN, minZBottomPosition), AxisSpeed.Fast);

            var waferDiameter = ServiceLocator.ChuckSupervisor.ChuckVM?.SelectedWaferCategory?.DimentionalCharacteristic?.Diameter;
            ServiceLocator.AxesSupervisor.GoToChuckCenter(waferDiameter, AxisSpeed.Fast);
            await Task.Run(() => ServiceLocator.AxesSupervisor.WaitMotionEnd(20_000));
        }

        public override bool CanLeave(INavigable nextPage, bool forceClose = false)
        {
            StopAutoAlignment.Execute(null);
            ServiceLocator.AxesSupervisor.AxesVM.IsLocked = false;
            ServiceLocator.LightsSupervisor.LightsAreLocked = false;
            ServiceLocator.CamerasSupervisor.GetMainCamera()?.StopStreaming();

            FocusStep.PropertyChanged -= Step_PropertyChanged;
            LightStep.PropertyChanged -= Step_PropertyChanged;
            EdgesStep.PropertyChanged -= Step_PropertyChanged;
            AlignmentMarksStep.PropertyChanged -= Step_PropertyChanged;

            return true;
        }

        #endregion INavigable

        private void Step_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AlignmentStepBaseVM.StepState))
            {
                OnPropertyChanged(nameof(GlobalAlignmentState));
                OnPropertyChanged(nameof(IsManualEditInProgress));
            }
            if (e.PropertyName == nameof(AlignmentStepBaseVM.IsInAutomaticMode))
            {
                SetNotDoneAutomaticSteps();
                OnPropertyChanged(nameof(IsManualEditInProgress));
                if ((sender as AlignmentStepBaseVM).IsInAutomaticMode)
                {
                    // "_ =" to disable warning CS4014
                    _ = ExecuteAutoAlignment();
                }
                else
                {
                    UnlockAxesAndLights();
                }
            }
            UpdateAllCanExecutes();
        }

        #region Method Members

        private void LoadRecipeData()
        {
            if (_editedRecipe.Alignment is null)
                return;

            var autolight = _editedRecipe.Alignment.AutoLight;
            LightStep.ResultLightPower = autolight.LightIntensity;
            LightStep.IsInAutomaticMode = !autolight.LightIntensityIsDefinedByUser;
            LightStep.ObjectiveToUse = ServiceLocator.ProbesSupervisor.GetOjectiveConfig(autolight.ObjectiveContext?.ObjectiveId);
            if (!LightStep.IsInAutomaticMode)
                LightStep.StepState = StepStates.Done;

            var autofocus = _editedRecipe.Alignment.AutoFocusLise;
            FocusStep.IsInAutomaticMode = !autofocus.ZIsDefinedByUser;
            FocusStep.AreParametersUsed = autofocus.LiseParametersAreDefinedByUser;

            FocusStep.GainParameter = autofocus.LiseGain;
            FocusStep.ZMaxParameter = autofocus.ZMax.Millimeters;
            FocusStep.ZMinParameter = autofocus.ZMin.Millimeters;

            FocusStep.ObjectiveToUse = ServiceLocator.ProbesSupervisor.GetOjectiveConfig(autofocus.LiseObjectiveContext.ObjectiveId);
            if (!FocusStep.IsInAutomaticMode)
                FocusStep.StepState = StepStates.Done;

            var bwa = _editedRecipe.Alignment.BareWaferAlignment;
            if (bwa.CustomImagePositions.Any())
            {
                // TO DO GERER LES MANUEAL POSITION --- avec le wafersize et le chuck cneter
                // attention ça devrait être du stagereferential non au lieu du WaferReferential ? (check with FDS)

                var topEdge = bwa.CustomImagePositions.FirstOrDefault(cip => cip.EdgePosition == Service.Interface.Algo.WaferEdgePositions.Top);
                if (!(topEdge is null))
                {
                    EdgesStep.EdgeTop.IsManuallyEdited = true;
                    EdgesStep.EdgeTop.ManualPosition = new XYPosition(new WaferReferential(), topEdge.X.Millimeters, topEdge.Y.Millimeters);
                }

                var rightEdge = bwa.CustomImagePositions.FirstOrDefault(cip => cip.EdgePosition == Service.Interface.Algo.WaferEdgePositions.Right);
                if (!(rightEdge is null))
                {
                    EdgesStep.EdgeRight.IsManuallyEdited = true;
                    EdgesStep.EdgeRight.ManualPosition = new XYPosition(new WaferReferential(), rightEdge.X.Millimeters, rightEdge.Y.Millimeters);
                }

                var bottomEdge = bwa.CustomImagePositions.FirstOrDefault(cip => cip.EdgePosition == Service.Interface.Algo.WaferEdgePositions.Bottom);
                if (!(bottomEdge is null))
                {
                    EdgesStep.EdgeBottom.IsManuallyEdited = true;
                    EdgesStep.EdgeBottom.ManualPosition = new XYPosition(new WaferReferential(), bottomEdge.X.Millimeters, bottomEdge.Y.Millimeters);
                }

                var leftEdge = bwa.CustomImagePositions.FirstOrDefault(cip => cip.EdgePosition == Service.Interface.Algo.WaferEdgePositions.Left);
                if (!(leftEdge is null))
                {
                    EdgesStep.EdgeLeft.IsManuallyEdited = true;
                    EdgesStep.EdgeLeft.ManualPosition = new XYPosition(new WaferReferential(), leftEdge.X.Millimeters, leftEdge.Y.Millimeters);
                }
                EdgesStep.IsInAutomaticMode = false;
                EdgesStep.StepState = StepStates.NotDone;
            }
            else
            {
                EdgesStep.IsInAutomaticMode = true;
            }
            EdgesStep.ObjectiveToUse = ServiceLocator.ProbesSupervisor.GetOjectiveConfig(bwa.ObjectiveContext?.ObjectiveId);

            var alignmentMarks = _editedRecipe.AlignmentMarks;
            AlignmentMarksStep.IsInAutomaticMode = true;
            AlignmentMarksStep.AlignmentMarksSettings = alignmentMarks;
            if (!AlignmentMarksStep.IsInAutomaticMode)
                AlignmentMarksStep.StepState = StepStates.Done;

            IsModified = false;
        }

        private void SaveRecipeData()
        {
            var alignmentSettings = new AlignmentSettings();

            var autoLightParameters = new AutoLightParameters()
            {
                LightIntensity = LightStep.ResultLightPower,
                LightIntensityIsDefinedByUser = !LightStep.IsInAutomaticMode,
                ObjectiveContext = new ObjectiveContext(LightStep.ObjectiveToUse.DeviceID)
            };

            var autoFocusLiseParameters = new AutoFocusLiseParameters()
            {
                LiseGain = FocusStep.GainParameter,
                ZIsDefinedByUser = !FocusStep.IsInAutomaticMode,
                LiseParametersAreDefinedByUser = FocusStep.AreParametersUsed,
                ZMax = FocusStep.ZMaxParameter.Millimeters(),
                ZMin = FocusStep.ZMinParameter.Millimeters(),
                ZTopFocus = FocusStep.ResultZPosition.Millimeters(),
                LiseObjectiveContext = new ObjectiveContext(FocusStep.ObjectiveToUse.DeviceID)
            };

            var bareWaferAlignmentParameters = new BareWaferAlignmentParameters() { ObjectiveContext = new ObjectiveContext(EdgesStep.ObjectiveToUse.DeviceID) };
            bareWaferAlignmentParameters.CustomImagePositions = new System.Collections.Generic.List<BareWaferAlignmentImagePosition>();
            if (EdgesStep.EdgeTop.IsManuallyEdited)
            {
                var bareWaferAlignmentImagePosition = new BareWaferAlignmentImagePosition() { EdgePosition = Service.Interface.Algo.WaferEdgePositions.Top, X = EdgesStep.EdgeTop.ManualPosition.ToXYPosition().X.Millimeters(), Y = EdgesStep.EdgeTop.ManualPosition.ToXYPosition().Y.Millimeters() };
                bareWaferAlignmentParameters.CustomImagePositions.Add(bareWaferAlignmentImagePosition);
            }
            if (EdgesStep.EdgeRight.IsManuallyEdited)
            {
                var bareWaferAlignmentImagePosition = new BareWaferAlignmentImagePosition() { EdgePosition = Service.Interface.Algo.WaferEdgePositions.Right, X = EdgesStep.EdgeRight.ManualPosition.ToXYPosition().X.Millimeters(), Y = EdgesStep.EdgeRight.ManualPosition.ToXYPosition().Y.Millimeters() };
                bareWaferAlignmentParameters.CustomImagePositions.Add(bareWaferAlignmentImagePosition);
            }
            if (EdgesStep.EdgeBottom.IsManuallyEdited)
            {
                var bareWaferAlignmentImagePosition = new BareWaferAlignmentImagePosition() { EdgePosition = Service.Interface.Algo.WaferEdgePositions.Bottom, X = EdgesStep.EdgeBottom.ManualPosition.ToXYPosition().X.Millimeters(), Y = EdgesStep.EdgeBottom.ManualPosition.ToXYPosition().Y.Millimeters() };
                bareWaferAlignmentParameters.CustomImagePositions.Add(bareWaferAlignmentImagePosition);
            }
            if (EdgesStep.EdgeLeft.IsManuallyEdited)
            {
                var bareWaferAlignmentImagePosition = new BareWaferAlignmentImagePosition() { EdgePosition = Service.Interface.Algo.WaferEdgePositions.Left, X = EdgesStep.EdgeLeft.ManualPosition.ToXYPosition().X.Millimeters(), Y = EdgesStep.EdgeLeft.ManualPosition.ToXYPosition().Y.Millimeters() };
                bareWaferAlignmentParameters.CustomImagePositions.Add(bareWaferAlignmentImagePosition);
            }

            alignmentSettings.AutoFocusLise = autoFocusLiseParameters;
            alignmentSettings.AutoLight = autoLightParameters;
            alignmentSettings.BareWaferAlignment = bareWaferAlignmentParameters;
            _editedRecipe.Alignment = alignmentSettings;
            OnPropertyChanged(nameof(CanCancelAutoAlignment));
            IsModified = false;
        }

        #endregion Method Members

        #region Properties

        public StepStates GlobalAlignmentState
        {
            get
            {
                if ((LightStep.StepState == StepStates.Error) || (FocusStep.StepState == StepStates.Error) || (EdgesStep.StepState == StepStates.Error))
                {
                    return StepStates.Error;
                }
                if ((LightStep.StepState == StepStates.InProgress) || (FocusStep.StepState == StepStates.InProgress) || (EdgesStep.StepState == StepStates.InProgress) || (AlignmentMarksStep.StepState == StepStates.InProgress))
                {
                    return StepStates.InProgress;
                }
                if ((LightStep.StepState == StepStates.Done) && (FocusStep.StepState == StepStates.Done) && (EdgesStep.StepState == StepStates.Done))
                {
                    return StepStates.Done;
                }
                return StepStates.NotDone;
            }
        }

        private bool _isAutomaticAlignmentInProgress = false;

        public bool IsAutomaticAlignmentInProgress
        {
            get => _isAutomaticAlignmentInProgress; set { if (_isAutomaticAlignmentInProgress != value) { _isAutomaticAlignmentInProgress = value; OnPropertyChanged(); } }
        }

        public bool IsManualEditInProgress
        {
            get
            {
                return (FocusStep.IsManualInProgress || LightStep.IsManualInProgress || EdgesStep.IsManualInProgress || AlignmentMarksStep.IsManualInProgress);
            }
        }

        private AlignmentFocusStepVM _focusStep;

        public AlignmentFocusStepVM FocusStep
        {
            get
            {
                if (_focusStep is null)
                    _focusStep = new AlignmentFocusStepVM(this);
                return _focusStep;
            }
        }

        private AlignmentLightStepVM _lightStep;

        public AlignmentLightStepVM LightStep
        {
            get
            {
                if (_lightStep is null)
                    _lightStep = new AlignmentLightStepVM(this);
                return _lightStep;
            }
        }

        private AlignmentEdgesStepVM _edgesStep;

        public AlignmentEdgesStepVM EdgesStep
        {
            get
            {
                if (_edgesStep is null)
                    _edgesStep = new AlignmentEdgesStepVM(this);
                return _edgesStep;
            }
        }

        private AlignmentMarksStepVM _alignmentMarksStep;

        public AlignmentMarksStepVM AlignmentMarksStep
        {
            get
            {
                if (_alignmentMarksStep is null)
                    _alignmentMarksStep = new AlignmentMarksStepVM(this);
                return _alignmentMarksStep;
            }
        }

        private bool _isModified = false;

        public bool IsModified
        {
            get => _isModified; set { if (_isModified != value) { _isModified = value; OnPropertyChanged(); } }
        }

        public List<SpecificPositions> AvailablePositions
        {
            get => new List<SpecificPositions> { SpecificPositions.PositionWaferCenter };
        }

        public SpecificPositions DefaultSpecificPosition
        {
            get => SpecificPositions.PositionWaferCenter;
        }

        #endregion Properties

        #region RelayCommands

        private AutoRelayCommand _startAutoAlignment;

        public AutoRelayCommand StartAutoAlignment
        {
            get
            {
                return _startAutoAlignment ?? (_startAutoAlignment = new AutoRelayCommand(
                    async () =>
                    {
                        await ExecuteAutoAlignment();
                    },
                    () =>
                    {
                        // We can start an autoalignment only if there is no manual step in progress
                        var isManualStepInProgress = (FocusStep.IsManualInProgress || LightStep.IsManualInProgress || EdgesStep.IsManualInProgress || AlignmentMarksStep.IsManualInProgress);

                        return !IsAutomaticAlignmentInProgress && !isManualStepInProgress;
                    }
                ));
            }
        }

        public async Task ExecuteAutoAlignment()
        {
            IsAutomaticAlignmentInProgress = true;
            DeleteWaferReferentialSettings();
            SetNotDoneAutomaticSteps();

            // We lock the axes and the light to prevent the user to change them
            ServiceLocator.AxesSupervisor.AxesVM.IsLocked = true;
            ServiceLocator.LightsSupervisor.LightsAreLocked = true;

            bool result = true;
            if (!IsAutomaticAlignmentInProgress)
                return;

            await GotoTheCenterOfTheWafer();

            if (FocusStep.IsInAutomaticMode)
            {
                FocusStep.ObjectiveToUse = FocusStep?.ObjectiveToUse ?? ServiceLocator.CamerasSupervisor.MainObjective;
                result = await FocusStep.ExecuteAutoAsync();
                if (!result)
                {
                    IsAutomaticAlignmentInProgress = false;
                    return;
                }
            }

            if (!IsAutomaticAlignmentInProgress)
                return;

            if (LightStep.IsInAutomaticMode)
            {
                LightStep.ObjectiveToUse = LightStep?.ObjectiveToUse ?? ServiceLocator.CamerasSupervisor.MainObjective;
                result = await LightStep.ExecuteAutoAsync();
                if (!result)
                {
                    IsAutomaticAlignmentInProgress = false;
                    return;
                }
            }

            if (!IsAutomaticAlignmentInProgress)
                return;

            // The edge step is always executed even if it is in manual mode
            EdgesStep.ObjectiveToUse = EdgesStep?.ObjectiveToUse ?? ServiceLocator.CamerasSupervisor.MainObjective;
            result = await EdgesStep.ExecuteAutoAsync();
            if (!result)
            {
                IsAutomaticAlignmentInProgress = false;
                return;
            }

            if (!IsAutomaticAlignmentInProgress)
                return;

            if (AlignmentMarksStep.IsInAutomaticMode && _editedRecipe.AlignmentMarks != null)
            {
                // AlignmentMarks has been saved in wafer referential.
                // We need to create a new one with the current BWA result.
                AlignmentMarksStep.ObjectiveToUse = AlignmentMarksStep?.ObjectiveToUse ?? ServiceLocator.CamerasSupervisor.MainObjective;
                ApplyBWAWaferReferentialSettings();
                try
                {
                    result = await AlignmentMarksStep.ExecuteAutoAsync();
                    if (!result)
                    {
                        IsAutomaticAlignmentInProgress = false;
                        return;
                    }
                }
                finally
                {
                    DeleteWaferReferentialSettings();
                }
            }
            IsAutomaticAlignmentInProgress = false;
            UnlockAxesAndLights();
        }

        private void UnlockAxesAndLights()
        {
            // We unlock the Axes and the light to allow the user to change them
            ServiceLocator.AxesSupervisor.AxesVM.IsLocked = false;
            ServiceLocator.LightsSupervisor.LightsAreLocked = false;
        }

        private void SetNotDoneAutomaticSteps()
        {
            // All the automatic modes are switched to the NotDone State
            if (FocusStep.IsInAutomaticMode)
                FocusStep.StepState = StepStates.NotDone;
            if (LightStep.IsInAutomaticMode)
                LightStep.StepState = StepStates.NotDone;
            if (EdgesStep.IsInAutomaticMode)
                EdgesStep.StepState = StepStates.NotDone;
            if (AlignmentMarksStep.IsInAutomaticMode)
                AlignmentMarksStep.StepState = StepStates.NotDone;
        }

        private void ApplyBWAWaferReferentialSettings()
        {
            var waferReferentialSettings = new WaferReferentialSettings()
            {
                ObjectiveIdForTopFocus = FocusStep.ObjectiveIdUsed,
                ZTopFocus = FocusStep.ResultZPosition.Millimeters(),
                ShiftX = EdgesStep.ResultShiftStageX,
                ShiftY = EdgesStep.ResultShiftStageY,
                WaferAngle = EdgesStep.ResultAngle
            };
            ServiceLocator.ReferentialSupervisor.SetSettings(waferReferentialSettings);
        }

        private void DeleteWaferReferentialSettings()
        {
            ServiceLocator.ReferentialSupervisor.DeleteSettings(ReferentialTag.Wafer);
        }

        private AutoRelayCommand _stopAutoAlignment;

        public AutoRelayCommand StopAutoAlignment
        {
            get
            {
                return _stopAutoAlignment ?? (_stopAutoAlignment = new AutoRelayCommand(
                    () =>
                    {
                        DoStopAutoAlignment();
                    },
                    () => { return IsAutomaticAlignmentInProgress; }
                ));
            }
        }

        private void DoStopAutoAlignment()
        {
            FocusStep.StopExecutionAuto();
            LightStep.StopExecutionAuto();
            EdgesStep.StopExecutionAuto();
            AlignmentMarksStep.StopExecutionAuto();
            IsAutomaticAlignmentInProgress = false;
            UnlockAxesAndLights();
        }

        private AutoRelayCommand _validateAutoAlignment;

        public AutoRelayCommand ValidateAutoAlignment
        {
            get
            {
                return _validateAutoAlignment ?? (_validateAutoAlignment = new AutoRelayCommand(
                    () =>
                    {
                        var waferReferentialSettings = new WaferReferentialSettings()
                        {
                            ObjectiveIdForTopFocus = FocusStep.ObjectiveIdUsed,
                            ZTopFocus = FocusStep.ResultZPosition.Millimeters(),
                            ShiftX = EdgesStep.ResultShiftStageX + (AlignmentMarksStep?.ResultShiftX ?? 0.Millimeters()),
                            ShiftY = EdgesStep.ResultShiftStageY + (AlignmentMarksStep?.ResultShiftY ?? 0.Millimeters()),
                            WaferAngle = EdgesStep.ResultAngle + (AlignmentMarksStep?.ResultRotationAngle ?? 0.Degrees()),
                        };
                        ServiceLocator.ReferentialSupervisor.SetSettings(waferReferentialSettings);
                        IsValidated = true;

                        SaveRecipeData();

                        _editedRecipe.IsModified = true;

                        NavigationManager.NavigateToNextPage();
                    },
                    () => { return GlobalAlignmentState == StepStates.Done; }
                ));
            }
        }

        private AutoRelayCommand _cancelAutoAlignment;

        public AutoRelayCommand CancelAutoAlignment
        {
            get
            {
                return _cancelAutoAlignment ?? (_cancelAutoAlignment = new AutoRelayCommand(
                    () =>
                    {
                        DoStopAutoAlignment();
                        LoadRecipeData();
                    },
                    () => { return !(_editedRecipe.Alignment is null) && IsModified; }
                ));
            }
        }

        public bool CanCancelAutoAlignment
        {
            get => (!(_editedRecipe.Alignment is null));
        }

        #endregion RelayCommands

        public void Dispose()
        {
            FocusStep.PropertyChanged -= Step_PropertyChanged;
            LightStep.PropertyChanged -= Step_PropertyChanged;
            EdgesStep.PropertyChanged -= Step_PropertyChanged;
            AlignmentMarksStep.PropertyChanged -= Step_PropertyChanged;

            FocusStep.Dispose();
            LightStep.Dispose();
            EdgesStep.Dispose();
            AlignmentMarksStep.Dispose();
        }
    }
}
