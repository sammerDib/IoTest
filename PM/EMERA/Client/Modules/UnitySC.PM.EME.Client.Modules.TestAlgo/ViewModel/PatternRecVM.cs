using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

using CommunityToolkit.Mvvm.Input;

using UnitySC.PM.EME.Client.Proxy.Algo;
using UnitySC.PM.EME.Client.Proxy.Axes;
using UnitySC.PM.EME.Client.Proxy.Camera;
using UnitySC.PM.EME.Client.Shared.Helpers;
using UnitySC.PM.EME.Service.Core.Shared;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.PM.EME.Service.Interface.Axes;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.EME.Client.Modules.TestAlgo.ViewModel
{
    public class PatternRecVM : AlgoBaseVM
    {
        private readonly CameraBench _camera;
        private readonly IEmeraMotionAxesService _motionAxesSupervisor;
        private readonly AlgoSupervisor _algoSupervisor;
        private readonly IDialogOwnerService _dialogService;
        private PositionWithPatternRec _refPatternRec;
        private XYZPosition _testPosition;

        public PatternRecVM() : base("Pattern REC")
        {
            _camera = ClassLocator.Default.GetInstance<CameraBench>();
            _motionAxesSupervisor = ClassLocator.Default.GetInstance<EmeraMotionAxesSupervisor>();
            _algoSupervisor = ClassLocator.Default.GetInstance<AlgoSupervisor>();
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

        private PatternRecResult _result;

        public PatternRecResult Result
        {
            get => _result; set { if (_result != value) { _result = value; OnPropertyChanged(); } }
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
                  Result = null;
                  IsBusy = true;

                  _testPosition = _motionAxesSupervisor.GetCurrentPosition()?.Result as XYZPosition;

                  var gammaToUse = UseImagePreprocessingPatternRec ? _patternRecGamma : double.NaN;

                  _refPatternRec.PatternRec.Gamma = gammaToUse;

                  var patternRecInput = new PatternRecInput(_refPatternRec.PatternRec);

                  _algoSupervisor.PatternRecChangedEvent += AlgosSupervisor_PatternRecChangedEvent;
                  _algoSupervisor.StartPatternRec(patternRecInput);
              },
              () => { return RefImageSource != null; }));
            }
        }


        private AutoRelayCommand _startImagePreprocessing;

        public AutoRelayCommand StartImagePreprocessing
        {
            get
            {
                return _startImagePreprocessing ?? (_startImagePreprocessing = new AutoRelayCommand(
                    () =>
                    {
                        CurrentImagePreprocessingResult = null;
                        IsBusy = true;
                        var curPosition = (XYZPosition)_motionAxesSupervisor.GetCurrentPosition()?.Result;
                        if (curPosition != null)
                        {
                            var cameraId = _camera.CameraId;
                            var refServiceImage = new ServiceImage(_refPatternRec.PatternRec.PatternReference);
                            ImagePreprocessingInput checkPreprocessingIlageInput = new ImagePreprocessingInput(cameraId, curPosition, _refPatternRec.PatternRec.RegionOfInterest, ImagePreprocessingGamma, refServiceImage);
                            _algoSupervisor.ImagePreprocessingChangedEvent += AlgosSupervisor_ImagePreprocessingChangedEvent;
                            _algoSupervisor.StartImagePreprocessing(checkPreprocessingIlageInput);
                        }
                    },
                    () => { return true; }
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
                  TakeImage.NotifyCanExecuteChanged();
                  Result = null;
                  CurrentImagePreprocessingResult = null;
              },
              () => { return RefImageSource != null; }));
            }
        }

        private AsyncRelayCommand _takeImage;

        public AsyncRelayCommand TakeImage
        {
            get
            {
                return _takeImage ?? (_takeImage = new AsyncRelayCommand(
              async () =>
              {
                  if (!_camera.IsStreaming)
                  {
                      _dialogService.ShowMessageBox("Camera must be started", "Camera", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.None);
                      return;
                  }

                  double imageResolutionScale = _algoSupervisor.GetFlowImageScale().Result;
                  var serviceImage = await _camera.GetScaledCameraImageAsync(Int32Rect.Empty, imageResolutionScale);
                  serviceImage = AlgorithmLibraryUtils.Convert16BitServiceImageTo8Bit(serviceImage);

                  if (serviceImage == null)
                  {
                      throw new Exception("Impossible to get Camera image");
                  }

                  var pixelSize = _camera.PixelSize;
                  _refPatternRec = PatternRecHelpers.CreatePositionWithPatternRec(serviceImage, pixelSize, imageResolutionScale, RoiRect, IsCenteredROI);

                  RefImageSource = _refPatternRec.PatternRec.PatternReference.WpfBitmapSource;
                  OnPropertyChanged(nameof(RoiRect));
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
                  _algoSupervisor.CancelPatternRec();
                  _algoSupervisor.PatternRecChangedEvent -= AlgosSupervisor_PatternRecChangedEvent;
                  IsBusy = false;
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
                    var posWithOffset = (XYZPosition)_testPosition.Clone();
                    posWithOffset.X += Result.ShiftX.Millimeters;
                    posWithOffset.Y += Result.ShiftY.Millimeters;

                    _motionAxesSupervisor.GoToPosition(posWithOffset, AxisSpeed.Normal);
                    _motionAxesSupervisor.WaitMotionEnd(300);
                }
            });
        }

        public void UpdateResult(PatternRecResult patternRecResult)
        {
            Result = patternRecResult;
            if (Result.Status.IsFinished)
            {
                _algoSupervisor.PatternRecChangedEvent -= AlgosSupervisor_PatternRecChangedEvent;
                IsBusy = false;
            }
        }

        private void UpdateResult(ImagePreprocessingResult imagePreprocessingResult)
        {
            CurrentImagePreprocessingResult = imagePreprocessingResult;
            if (CurrentImagePreprocessingResult.Status.IsFinished)
            {
                _algoSupervisor.ImagePreprocessingChangedEvent -= AlgosSupervisor_ImagePreprocessingChangedEvent;
                IsBusy = false;
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _algoSupervisor.ImagePreprocessingChangedEvent -= AlgosSupervisor_ImagePreprocessingChangedEvent;
                CurrentImagePreprocessingResult = null;
                _algoSupervisor.PatternRecChangedEvent -= AlgosSupervisor_PatternRecChangedEvent;
                Result = null;
            }
        }        
    }
}
