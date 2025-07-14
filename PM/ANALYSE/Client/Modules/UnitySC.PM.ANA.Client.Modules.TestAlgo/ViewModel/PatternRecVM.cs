using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Client.Proxy.Axes;
using UnitySC.PM.ANA.Client.Shared.Helpers;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Controls;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.ANA.Client.Modules.TestAlgo.ViewModel
{
    public class PatternRecVM : AlgoBaseVM
    {
        private CamerasSupervisor _camerasSupervisor;
        private AxesSupervisor _axesSupervisor;
        private IDialogOwnerService _dialogService;
        private PositionWithPatternRec _refPatternRec;
        private XYZTopZBottomPosition _testPosition;

        public PatternRecVM() : base("Pattern REC")
        {
            _camerasSupervisor = ClassLocator.Default.GetInstance<CamerasSupervisor>();
            _axesSupervisor = ClassLocator.Default.GetInstance<AxesSupervisor>();
            _dialogService = ClassLocator.Default.GetInstance<IDialogOwnerService>();
        }

        private void AlgosSupervisor_PatternRecChangedEvent(PatternRecResult patternRecResult)
        {
            UpdateResult(patternRecResult);
        }

        private void AlgosSupervisor_ImagePreprocessingChangedEvent(ImagePreprocessingResult imagePreprocessingResult)
        {
            UpdateResult(imagePreprocessingResult);
        }

        private void AlgosSupervisor_CheckPatternRecChangedEvent(CheckPatternRecResult checkPatternRecResult)
        {
            UpdateResult(checkPatternRecResult);
        }

        private PatternRecResult _result;

        public PatternRecResult Result
        {
            get => _result; set { if (_result != value) { _result = value; OnPropertyChanged(); } }
        }

        private CheckPatternRecResult _currentCheckPatternRecResult = null;

        public CheckPatternRecResult CurrentCheckPatternRecResult
        {
            get => _currentCheckPatternRecResult; set { if (_currentCheckPatternRecResult != value) { _currentCheckPatternRecResult = value; OnPropertyChanged(); } }
        }

        private ImagePreprocessingResult _currentImagePreprocessingResult = null;

        public ImagePreprocessingResult CurrentImagePreprocessingResult
        {
            get => _currentImagePreprocessingResult; set { if (_currentImagePreprocessingResult != value) { _currentImagePreprocessingResult = value; OnPropertyChanged(); } }
        }

        private bool _useImagePreprocessingPatternRec = true;

        public bool UseImagePreprocessingPatternRec
        {
            get => _useImagePreprocessingPatternRec; set { if (_useImagePreprocessingPatternRec != value) { _useImagePreprocessingPatternRec = value; OnPropertyChanged(); } }
        }

        private double _patternRecGamma = 0.3;

        public double PatternRecGamma
        {
            get => _patternRecGamma; set { if (_patternRecGamma != value) { _patternRecGamma = value; OnPropertyChanged(); } }
        }

        private double _imagePreprocessingGamma = 0.3;

        public double ImagePreprocessingGamma
        {
            get => _imagePreprocessingGamma; set { if (_imagePreprocessingGamma != value) { _imagePreprocessingGamma = value; OnPropertyChanged(); } }
        }

        private bool _useImagePreprocessingCheckPatternRec = true;

        public bool UseImagePreprocessingCheckPatternRec
        {
            get => _useImagePreprocessingCheckPatternRec; set { if (_useImagePreprocessingCheckPatternRec != value) { _useImagePreprocessingCheckPatternRec = value; OnPropertyChanged(); } }
        }

        private double _checkPatternRecGamma = 0.3;

        public double CheckPatternRecGamma
        {
            get => _checkPatternRecGamma; set { if (_checkPatternRecGamma != value) { _checkPatternRecGamma = value; OnPropertyChanged(); } }
        }


        private StepStates _checkPatternRecStepState = StepStates.NotDone;

        public StepStates CheckPatternRecStepState
        {
            get => _checkPatternRecStepState;
            set
            {
                if (_checkPatternRecStepState != value)
                {
                    _checkPatternRecStepState = value;
                    if (_checkPatternRecStepState != StepStates.Error)
                    {
                        CheckPatternRecErrorMessage = null;
                    }
                    OnPropertyChanged();
                }
            }
        }

        private string _checkPatternRecErrorMessage = null;

        public string CheckPatternRecErrorMessage
        {
            get => _checkPatternRecErrorMessage; set { if (_checkPatternRecErrorMessage != value) { _checkPatternRecErrorMessage = value; OnPropertyChanged(); } }
        }


        private BitmapSource _refImageSource;

        public BitmapSource RefImageSource
        {
            get => _refImageSource; set { if (_refImageSource != value) { _refImageSource = value; OnPropertyChanged(); StartPatternRec.NotifyCanExecuteChanged(); DeleteImage.NotifyCanExecuteChanged(); } }
        }

        private AutoRelayCommand _startPatternRec;

        public AutoRelayCommand StartPatternRec
        {
            get
            {
                return _startPatternRec ?? (_startPatternRec = new AutoRelayCommand(
              () =>
              {
                  if (!IsCameraGrabbing())
                      return;
                  Result = null;
                  IsBusy = true;

                  _testPosition = _axesSupervisor.GetCurrentPosition()?.Result as XYZTopZBottomPosition;

                  var gammaToUse = UseImagePreprocessingPatternRec ? _patternRecGamma : double.NaN;

                  _refPatternRec.PatternRec.Gamma = gammaToUse;

                  var patternRecInput = new PatternRecInput(_refPatternRec.PatternRec);

                  ServiceLocator.AlgosSupervisor.PatternRecChangedEvent += AlgosSupervisor_PatternRecChangedEvent;
                  ServiceLocator.AlgosSupervisor.StartPatternRec(patternRecInput);
              },
              () => { return RefImageSource != null; }));
            }
        }

        private bool IsCameraGrabbing()
        {
            if (!_camerasSupervisor.Camera.IsGrabbing)
            {
                _dialogService.ShowMessageBox("Camera must be started", "Camera", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.None);
                return false;
            }
            return true;
        }

        private AutoRelayCommand _startImagePreprocessing;

        public AutoRelayCommand StartImagePreprocessing
        {
            get
            {
                return _startImagePreprocessing ?? (_startImagePreprocessing = new AutoRelayCommand(
                    () =>
                    {
                        if (!IsCameraGrabbing())
                            return;
                        CurrentImagePreprocessingResult = null;
                        IsBusy = true;
                        var curPosition = (XYZTopZBottomPosition)ServiceLocator.AxesSupervisor.GetCurrentPosition()?.Result;
                        if (curPosition != null)
                        {
                            var cameraId = ServiceLocator.CamerasSupervisor.Camera.Configuration.DeviceID;
                            ImagePreprocessingInput checkPreprocessingIlageInput = new ImagePreprocessingInput(cameraId, curPosition, null, ImagePreprocessingGamma);
                            ServiceLocator.AlgosSupervisor.ImagePreprocessingChangedEvent += AlgosSupervisor_ImagePreprocessingChangedEvent;
                            ServiceLocator.AlgosSupervisor.StartImagePreprocessing(checkPreprocessingIlageInput);
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
                        CurrentCheckPatternRecResult = null;
                        IsBusy = true;

                        var checkPatternRecSettings = ServiceLocator.AlgosSupervisor.GetCheckPatternRecSettings()?.Result;

                        double pixelSizemm = ServiceLocator.CamerasSupervisor.PixelSizeXmm;
                        double ImageWidth = RefImageSource.PixelWidth;
                        Length Shift = (ImageWidth * pixelSizemm * checkPatternRecSettings.ShiftRatio).Millimeters();

                        if (checkPatternRecSettings != null)
                        {
                            var positionsToCheck = new List<XYZTopZBottomPosition>();
                            // Create the list of the positions to check on a circle of a radius checkPatternRecSettings.CheckDistance
                            for (int i = 0; i < checkPatternRecSettings.NbChecks; i++)
                            {
                                var angle = i * 2 * Math.PI / checkPatternRecSettings.NbChecks;
                                var x = _refPatternRec.Position.X + Math.Cos(angle) * Shift.Millimeters;
                                var y = _refPatternRec.Position.Y + Math.Sin(angle) * Shift.Millimeters;

                                var newXYPosition = new XYZTopZBottomPosition(_refPatternRec.Position.Referential, x, y, _refPatternRec.Position.ZTop, _refPatternRec.Position.ZBottom);

                                positionsToCheck.Add(newXYPosition);
                            }

                            _refPatternRec.PatternRec.Gamma = UseImagePreprocessingCheckPatternRec ? CheckPatternRecGamma : double.NaN;

                            CheckPatternRecInput checkPatternRecInput = new CheckPatternRecInput(_refPatternRec, positionsToCheck, 0.1.Millimeters());
                            ServiceLocator.AlgosSupervisor.CheckPatternRecChangedEvent += AlgosSupervisor_CheckPatternRecChangedEvent;
                            ServiceLocator.AlgosSupervisor.StartCheckPatternRec(checkPatternRecInput);
                        }
                    },
                    () => { return RefImageSource != null; }
                ));
            }
        }

        private AutoRelayCommand _deleteImage;

        public AutoRelayCommand DeleteImage
        {
            get
            {
                return _deleteImage ?? (_deleteImage = new AutoRelayCommand(
              () =>
              {
                  RefImageSource = null;
                  Result = null;
                  CurrentCheckPatternRecResult = null;
                  CurrentImagePreprocessingResult = null;
              },
              () => { return RefImageSource != null; }));
            }
        }

        private AutoRelayCommand _takeImage;

        public AutoRelayCommand TakeImage
        {
            get
            {
                return _takeImage ?? (_takeImage = new AutoRelayCommand(
              () =>
              {
                  try
                  {
                      if (!_camerasSupervisor.Camera.IsGrabbing)
                      {
                          _dialogService.ShowMessageBox("Camera must be started", "Camera", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.None);
                          return;
                      }

                      var camId = _camerasSupervisor.Camera.Configuration.DeviceID;
                      var roiSize = new System.Windows.Size(ServiceLocator.CamerasSupervisor.Camera.CameraInfo.Width, ServiceLocator.CamerasSupervisor.Camera.CameraInfo.Height);

                      if (ServiceLocator.CamerasSupervisor.Camera.Configuration.ModulePosition == ModulePositions.Up)
                          _refPatternRec = PatternRecHelpers.CreatePositionWithTopPatternRec(RoiRect, IsCenteredROI);
                      else
                          _refPatternRec = PatternRecHelpers.CreatePositionWithBottomPatternRec(RoiRect, IsCenteredROI);

                      int width = (int)RoiRect.Width;
                      int height = (int)RoiRect.Height;
                      RefImageSource = _refPatternRec.PatternRec.PatternReference.WpfBitmapSource;
                      OnPropertyChanged(nameof(RoiRect));
                  }
                  catch
                  {
                  }
              },
              () => { return RefImageSource == null; }));
            }
        }
        private AutoRelayCommand _cancelCommand;

        public AutoRelayCommand CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new AutoRelayCommand(
              () =>
              {
                  ClassLocator.Default.GetInstance<AlgosSupervisor>().CancelPatternRec();
              },
              () => { return true; }));
            }
        }

        private AutoRelayCommand _gotoShiftPosCommand;

        public AutoRelayCommand GotoShiftPosCommand
        {
            get
            {
                return _gotoShiftPosCommand ?? (_gotoShiftPosCommand = new AutoRelayCommand(
                  async () =>
                 {
                     await MoveToShiftedPosition();
                 },
                 () => { return Result != null; }));
            }
        }

        protected async Task MoveToShiftedPosition()
        {
            await Task.Run(() =>
            {
                if (Result != null && _testPosition != null)
                {
                    var posWithOffset = (XYZTopZBottomPosition)_testPosition.Clone();
                    posWithOffset.X += Result.ShiftX.Millimeters;
                    posWithOffset.Y += Result.ShiftY.Millimeters;
                    _axesSupervisor.GotoPosition(posWithOffset, PM.Shared.Hardware.Service.Interface.Axes.AxisSpeed.Normal);
                    _axesSupervisor.WaitMotionEnd(3000);
                }
            });
        }

        public void UpdateResult(PatternRecResult patternRecResult)
        {
            Result = patternRecResult;
            if (Result.Status.IsFinished)
            {
                ServiceLocator.AlgosSupervisor.PatternRecChangedEvent -= AlgosSupervisor_PatternRecChangedEvent;
                IsBusy = false;
            }
        }

        public override void Dispose()
        {
        }

        private void UpdateResult(ImagePreprocessingResult imagePreprocessingResult)
        {
            CurrentImagePreprocessingResult = imagePreprocessingResult;
            if (CurrentImagePreprocessingResult.Status.IsFinished)
            {
                ServiceLocator.AlgosSupervisor.ImagePreprocessingChangedEvent -= AlgosSupervisor_ImagePreprocessingChangedEvent;
                IsBusy = false;
            }
        }

        private void UpdateResult(CheckPatternRecResult checkPatternRecResult)
        {
            CurrentCheckPatternRecResult = checkPatternRecResult;
            if (CurrentCheckPatternRecResult.Status.IsFinished)
            {
                ServiceLocator.AlgosSupervisor.CheckPatternRecChangedEvent -= AlgosSupervisor_CheckPatternRecChangedEvent;
                IsBusy = false;
            }

            switch (checkPatternRecResult.Status.State)
            {
                case FlowState.Waiting:
                case FlowState.InProgress:
                    CheckPatternRecStepState = StepStates.InProgress;
                    break;

                case FlowState.Error:
                    {
                        CheckPatternRecStepState = StepStates.Error;
                        CheckPatternRecErrorMessage = checkPatternRecResult.Status.Message;
                        break;
                    }
                case FlowState.Canceled:
                    CheckPatternRecStepState = StepStates.NotDone;
                    break;

                case FlowState.Success:
                    {
                        if (checkPatternRecResult.Succeeded)
                            CheckPatternRecStepState = StepStates.Done;
                        else
                        {
                            CheckPatternRecStepState = StepStates.Error;

                            CheckPatternRecErrorMessage = "The pattern check failed, please change the gamma value, the reference position or the reference region and retry";
                        }

                        CurrentCheckPatternRecResult = checkPatternRecResult;

                        break;
                    }
                default:
                    break;
            }
        }
    }
}
