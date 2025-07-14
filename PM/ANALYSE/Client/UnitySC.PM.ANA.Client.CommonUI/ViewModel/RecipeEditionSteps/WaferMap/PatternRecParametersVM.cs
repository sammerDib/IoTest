using System;
using System.Collections.Generic;

using UnitySC.Shared.UI.AutoRelayCommandExt;

using MvvmDialogs;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.Controls;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Client.Proxy;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps
{
    public class PatternRecParametersVM : StepBaseVM, IModalDialogViewModel
    {
        private const double DefaultGamma = 0.3;

        private bool? _dialogResult;

        private string _objectiveID;
        public PatternRecParametersVM(PositionWithPatternRec positionPatternRec)
        {
            PositionPatternRec = positionPatternRec;
            ServiceLocator.AlgosSupervisor.CheckPatternRecChangedEvent += AlgosSupervisor_CheckPatternRecChangedEvent;
            ServiceLocator.AlgosSupervisor.ImagePreprocessingChangedEvent += AlgosSupervisor_ImagePreprocessingChangedEvent;
            _objectiveID = (PositionPatternRec.Context as TopImageAcquisitionContext).TopObjectiveContext.ObjectiveId;
            _objectiveName = CamerasSupervisor.GetObjectiveFromId(_objectiveID).Name;
        }

        private PositionWithPatternRec _positionPatternRec = null;

        public PositionWithPatternRec PositionPatternRec
        {
            get => _positionPatternRec;
            set
            {
                if (_positionPatternRec != value)
                {
                    _positionPatternRec = value;
                    if (double.IsNaN(_positionPatternRec.PatternRec.Gamma))
                    {
                        UseImagePreprocessing = false;
                        CurrentGamma = DefaultGamma;
                    }
                    else
                    {
                        UseImagePreprocessing = true;
                        CurrentGamma = _positionPatternRec.PatternRec.Gamma;
                    }
                    UpdateRoiPositionInPixels();
                    OnPropertyChanged();
                }
            }
        }

        private void UpdateRoiPositionInPixels()
        {
            var pixelSizemm = ServiceLocator.CalibrationSupervisor.GetObjectiveCalibration(ServiceLocator.CamerasSupervisor.Objective.DeviceID).Image.PixelSizeX.Millimeters;
            if (PositionPatternRec.PatternRec.RegionOfInterest != null)
            {
                DisplayROI = true;
                RoiWidth = PositionPatternRec.PatternRec.RegionOfInterest.Width.ToPixels(pixelSizemm.Millimeters());
                RoiHeight = PositionPatternRec.PatternRec.RegionOfInterest.Height.ToPixels(pixelSizemm.Millimeters());
                RoiLeft = PositionPatternRec.PatternRec.RegionOfInterest.X.ToPixels(pixelSizemm.Millimeters());
                RoiTop = PositionPatternRec.PatternRec.RegionOfInterest.Y.ToPixels(pixelSizemm.Millimeters());
            }
            else
            {
                DisplayROI = false;
                RoiWidth = PositionPatternRec.PatternRec.PatternReference.DataWidth;
                RoiHeight = PositionPatternRec.PatternRec.PatternReference.DataHeight;
                RoiLeft = 0;
                RoiTop = 0;

            }
        }
        private double _roiLeft = 0;

        public double RoiLeft
        {
            get => _roiLeft; set { if (_roiLeft != value) { _roiLeft = value; OnPropertyChanged(); } }
        }

        private double _roiTop = 0;

        public double RoiTop
        {
            get => _roiTop; set { if (_roiTop != value) { _roiTop = value; OnPropertyChanged(); } }
        }

        private double _roiWidth = 0;

        public double RoiWidth
        {
            get => _roiWidth; set { if (_roiWidth != value) { _roiWidth = value; OnPropertyChanged(); } }
        }

        private double _roiHeight = 0;

        public double RoiHeight
        {
            get => _roiHeight; set { if (_roiHeight != value) { _roiHeight = value; OnPropertyChanged(); } }
        }
        private bool _displayROI = false;

        public bool DisplayROI
        {
            get => _displayROI; set { if (_displayROI != value) { _displayROI = value; OnPropertyChanged(); } }
        }
        private double _currentGamma = DefaultGamma;

        public double CurrentGamma
        {
            get => _currentGamma;
            set
            {
                if (_currentGamma != value)
                {
                    _currentGamma = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _useImagePreprocessing = true;
        public bool UseImagePreprocessing
        {
            get => _useImagePreprocessing; set { if (_useImagePreprocessing != value) { _useImagePreprocessing = value; OnPropertyChanged(); } }
        }
        private bool _displayCross = true;
        public bool DisplayCross
        {
            get => _displayCross; set { if (_displayCross != value) { _displayCross = value; OnPropertyChanged(); } }
        }
        private string _objectiveName;
        public string ObjectiveName
        {
            get => _objectiveName; set { if (_objectiveName != value) { _objectiveName = value; OnPropertyChanged(); } }
        }

        private StepStates _checkParametersState = StepStates.NotDone;

        public StepStates CheckParametersState
        {
            get => _checkParametersState; set { if (_checkParametersState != value) { _checkParametersState = value; OnPropertyChanged(); } }
        }

        private CheckPatternRecResult _currentCheckPatternRecResult = null;

        public CheckPatternRecResult CurrentCheckPatternRecResult
        {
            get => _currentCheckPatternRecResult; set { if (_currentCheckPatternRecResult != value) { _currentCheckPatternRecResult = value; OnPropertyChanged(); } }
        }

        private bool _displayImagePreprocessingResult = false;

        public bool DisplayImagePreprocessingResult
        {
            get => _displayImagePreprocessingResult; set { if (_displayImagePreprocessingResult != value) { _displayImagePreprocessingResult = value; OnPropertyChanged(); } }
        }

        private ImagePreprocessingResult _currentImagePreprocessingResult = null;

        public ImagePreprocessingResult CurrentImagePreprocessingResult
        {
            get => _currentImagePreprocessingResult; set { if (_currentImagePreprocessingResult != value) { _currentImagePreprocessingResult = value; OnPropertyChanged(); } }
        }

        private PatternRecResult _selectedSingleResult = null;

        public PatternRecResult SelectedSingleResult
        {
            get => _selectedSingleResult; set { if (_selectedSingleResult != value) { _selectedSingleResult = value; DisplayImagePreprocessingResult = false; OnPropertyChanged(); } }
        }

        public double ImageWidth => PositionPatternRec?.PatternRec?.PatternReference?.DataWidth ?? 0;

        public double ImageHeight => PositionPatternRec?.PatternRec?.PatternReference?.DataHeight ?? 0;

        public double Gamma => PositionPatternRec?.PatternRec?.Gamma ?? DefaultGamma;

        #region RelayCommands

        private AutoRelayCommand _startCheckPreprocessingImage;

        public AutoRelayCommand StartCheckPreprocessingImage
        {
            get
            {
                return _startCheckPreprocessingImage ?? (_startCheckPreprocessingImage = new AutoRelayCommand(
                    () =>
                    {
                        Console.WriteLine("Start : " + Environment.TickCount);
                        CheckParametersState = StepStates.InProgress;
                        var curPosition = (XYZTopZBottomPosition)ServiceLocator.AxesSupervisor.GetCurrentPosition()?.Result;
                        if (curPosition != null)
                        {
                            var cameraId = ServiceLocator.CamerasSupervisor.Camera.Configuration.DeviceID;
                            ImagePreprocessingInput checkPreprocessingIlageInput = new ImagePreprocessingInput(cameraId, curPosition, null, CurrentGamma);
                            ServiceLocator.AlgosSupervisor.StartImagePreprocessing(checkPreprocessingIlageInput);
                        }
                        else
                        {
                            Logger.Error("StartCheckPreprocessingImage error : Impossible to get current axes position");
                        }
                    },
                    () => { return true; }
                ));
            }
        }

        private AutoRelayCommand _startCheckPatternRec;

        public AutoRelayCommand StartCheckPatternRec
        {
            get
            {
                return _startCheckPatternRec ?? (_startCheckPatternRec = new AutoRelayCommand(
                    () =>
                    {
                        StepState = StepStates.InProgress;
                        CurrentCheckPatternRecResult = null;

                        double pixelSizemm = ServiceLocator.CalibrationSupervisor.GetObjectiveCalibration(_objectiveID).Image.PixelSizeX.Millimeters;
                        var checkPatternRecSettings = ServiceLocator.AlgosSupervisor.GetCheckPatternRecSettings()?.Result;
                        Length Shift = (ImageWidth * pixelSizemm * checkPatternRecSettings.ShiftRatio).Millimeters();

                        if (checkPatternRecSettings != null)
                        {

                            var positionsToCheck = new List<XYZTopZBottomPosition>();
                            // Create the list of the positions to check on a circle of a radius checkPatternRecSettings.CheckDistance
                            for (int i = 0; i < checkPatternRecSettings.NbChecks; i++)
                            {
                                var angle = i * 2 * Math.PI / checkPatternRecSettings.NbChecks;
                                var x = PositionPatternRec.Position.X + Math.Cos(angle) * Shift.Millimeters;
                                var y = PositionPatternRec.Position.Y + Math.Sin(angle) * Shift.Millimeters;

                                var newXYPosition = new XYZTopZBottomPosition(PositionPatternRec.Position.Referential, x, y, PositionPatternRec.Position.ZTop, PositionPatternRec.Position.ZBottom);
                                positionsToCheck.Add(newXYPosition);
                            }

                            var previousPatternRecGamma = PositionPatternRec.PatternRec.Gamma;
                            if (UseImagePreprocessing)
                                PositionPatternRec.PatternRec.Gamma = CurrentGamma;
                            else
                                PositionPatternRec.PatternRec.Gamma = double.NaN;


                            CheckPatternRecInput checkPatternRecInput = new CheckPatternRecInput(PositionPatternRec, positionsToCheck, 0.1.Millimeters());

                            ServiceLocator.AlgosSupervisor.StartCheckPatternRec(checkPatternRecInput);
                            PositionPatternRec.PatternRec.Gamma = previousPatternRecGamma;
                        }
                        else
                        {
                            Logger.Error("StartCheckPatternRec error : Impossible to get pattern rec settings");
                        }
                    },
                    () => { return StepState != StepStates.InProgress; }
                ));
            }
        }

        private void AlgosSupervisor_ImagePreprocessingChangedEvent(ImagePreprocessingResult imagePreprocessingResult)
        {
            switch (imagePreprocessingResult.Status.State)
            {
                case FlowState.Waiting:
                case FlowState.InProgress:
                    CheckParametersState = StepStates.InProgress;
                    break;

                case FlowState.Error:
                    {
                        CheckParametersState = StepStates.Error;
                        ErrorMessage = imagePreprocessingResult.Status.Message;
                        break;
                    }
                case FlowState.Canceled:
                    CheckParametersState = StepStates.NotDone;
                    break;

                case FlowState.Success:
                    {
                        CheckParametersState = StepStates.Done;
                        CurrentImagePreprocessingResult = imagePreprocessingResult;
                        SelectedSingleResult = null;
                        DisplayImagePreprocessingResult = true;

                        Console.WriteLine("Result : " + Environment.TickCount);
                        break;
                    }
                default:
                    break;
            }
        }

        private AutoRelayCommand _cancelCheckPatternRec;

        public AutoRelayCommand CancelCheckPatternRec
        {
            get
            {
                return _cancelCheckPatternRec ?? (_cancelCheckPatternRec = new AutoRelayCommand(
                    () =>
                    {
                        if (StepState == StepStates.InProgress)
                        {
                            ServiceLocator.AlgosSupervisor.CancelCheckPatternRec();
                        }
                        CleanupBeforeClosing();
                        DialogResult = false;
                    },
                    () => { return true; }
                ));
            }
        }

        private void CleanupBeforeClosing()
        {
            ServiceLocator.AxesSupervisor.GotoPosition(PositionPatternRec.Position, PM.Shared.Hardware.Service.Interface.Axes.AxisSpeed.Fast);
            ServiceLocator.AlgosSupervisor.CheckPatternRecChangedEvent -= AlgosSupervisor_CheckPatternRecChangedEvent;
            ServiceLocator.AlgosSupervisor.ImagePreprocessingChangedEvent -= AlgosSupervisor_ImagePreprocessingChangedEvent;
        }

        private void AlgosSupervisor_CheckPatternRecChangedEvent(CheckPatternRecResult checkPatternRecResult)
        {
            switch (checkPatternRecResult.Status.State)
            {
                case FlowState.Waiting:
                case FlowState.InProgress:
                    StepState = StepStates.InProgress;
                    break;

                case FlowState.Error:
                    {
                        StepState = StepStates.Error;
                        ErrorMessage = checkPatternRecResult.Status.Message;
                        break;
                    }
                case FlowState.Canceled:
                    StepState = StepStates.NotDone;
                    break;

                case FlowState.Success:
                    {
                        if (checkPatternRecResult.Succeeded)
                            StepState = StepStates.Done;
                        else
                        {
                            StepState = StepStates.Error;

                            ErrorMessage = "The pattern check failed, please change the gamma value, the reference position or the reference region and retry";
                        }

                        CurrentCheckPatternRecResult = checkPatternRecResult;
                        // We select the first one
                        if (checkPatternRecResult.SingleResults.Count > 0)
                            SelectedSingleResult = checkPatternRecResult.SingleResults[0];
                        DisplayImagePreprocessingResult = false;
                        break;
                    }
                default:
                    break;
            }
        }

        private AutoRelayCommand _validateParameters;

        public AutoRelayCommand ValidateParameters
        {
            get
            {
                return _validateParameters ?? (_validateParameters = new AutoRelayCommand(
                    () =>
                    {
                        if (UseImagePreprocessing)
                            PositionPatternRec.PatternRec.Gamma = CurrentGamma;
                        else
                            PositionPatternRec.PatternRec.Gamma = double.NaN;
                        CleanupBeforeClosing();
                        DialogResult = true;
                    },
                    () => { return true; }
                ));
            }
        }

        #endregion RelayCommands

        public bool? DialogResult
        {
            get => _dialogResult;
            private set
            {
                _dialogResult = value;
                OnPropertyChanged();
            }
        }
    }
}
