using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.DMT.CommonUI.Proxy;
using UnitySC.PM.DMT.CommonUI.ViewModel.Measure;
using UnitySC.PM.DMT.Service.Interface.AutoExposure;
using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.PM.DMT.Service.Interface.Recipe;
using UnitySC.PM.DMT.Shared;
using UnitySC.PM.DMT.Shared.UI.Message;
using UnitySC.PM.DMT.Shared.UI.Proxy;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.ClientProxy.Camera;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;
using UnitySC.Shared.UI.ViewModel.Navigation;

namespace UnitySC.PM.DMT.CommonUI.ViewModel.ExposureSettings
{
    public class ComputeExposureSettingsVM : PageNavigationVM, IRecipient<ImageGrabbedMessage>, IRecipient<RecipeMessage>
    {
        private readonly Mapper _mapper;
        private readonly Shared.UI.Proxy.CameraSupervisor _cameraSupervisor;
        private readonly ScreenSupervisor _screenSupervisor;
        private readonly AlgorithmsSupervisor _algorithmsSupervisor;
        private readonly IDialogOwnerService _dialogService;

        private static readonly Brush s_defaultProgessBarColor =
            new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF06B025"));

        private AutoRelayCommand _abortCommand;

        private AutoRelayCommand _autoSetCommand;

        private BitmapSource _cameraBitmapSource;

        private int _currentStep;

        private double _exposureTimeMs;

        private bool _isRecipeRunning;

        private bool _isSuccessfull;

        private string _message;
        private Brush _progessBarColor = s_defaultProgessBarColor;

        private int _totalSteps = 1;

        protected MeasureVM Measure;
        private int _selectedColorIndex;

        public ComputeExposureSettingsVM(MeasureVM measure, Mapper mapper, Shared.UI.Proxy.CameraSupervisor cameraSupervisor, ScreenSupervisor screenSupervisor, AlgorithmsSupervisor algorithmsSupervisor,
            IDialogOwnerService dialogService, int selectedColorIndex)
        {
            _mapper = mapper;
            _cameraSupervisor = cameraSupervisor;
            _screenSupervisor = screenSupervisor;
            _algorithmsSupervisor = algorithmsSupervisor;
            _dialogService = dialogService;
            Measure = measure;
            _selectedColorIndex = selectedColorIndex;
        }

        public override string PageName => "Compute Exposure";

        public double ExposureTimeMs
        {
            get => _exposureTimeMs;
            set
            {
                if (_exposureTimeMs != value)
                {
                    _exposureTimeMs = value;
                    OnPropertyChanged();
                }
            }
        }

        public BitmapSource CameraBitmapSource
        {
            get => _cameraBitmapSource;
            set
            {
                if (_cameraBitmapSource != value)
                {
                    _cameraBitmapSource = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsRecipeRunning
        {
            get => _isRecipeRunning;
            set
            {
                if (_isRecipeRunning != value)
                {
                    Logger.Error("IsRecipeRunning: " + value);
                    _isRecipeRunning = value;
                    CanNavigate = !_isRecipeRunning;
                    OnPropertyChanged();
                }
            }
        }

        public int CurrentStep
        {
            get => _currentStep;
            set
            {
                if (_currentStep != value)
                {
                    _currentStep = value;
                    OnPropertyChanged();
                }
            }
        }

        public int TotalSteps
        {
            get => _totalSteps;
            set
            {
                if (_totalSteps != value)
                {
                    _totalSteps = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Message
        {
            get => _message;
            set
            {
                if (_message != value)
                {
                    _message = value;
                    OnPropertyChanged();
                }
            }
        }

        public Brush ProgessBarColor
        {
            get => _progessBarColor;
            set
            {
                if (_progessBarColor != value)
                {
                    _progessBarColor = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsSuccessful
        {
            get => _isSuccessfull;
            set
            {
                if (_isSuccessfull != value)
                {
                    _isSuccessfull = value;
                    OnPropertyChanged();
                }
            }
        }

        public AutoRelayCommand AutoSetCommand =>
            _autoSetCommand ?? (_autoSetCommand = new AutoRelayCommand(
                () =>
                {
                    IsRecipeRunning = true;
                    IsSuccessful = false;
                    CurrentStep = 0;
                    Message = "";
                    CameraBitmapSource = null;
                    ExposureTimeMs = 0;
                    ProgessBarColor = s_defaultProgessBarColor;

                    try
                    {
                        var measure = _mapper.AutoMap.Map<MeasureVM, MeasureBase>(Measure);
                        _algorithmsSupervisor.StartAutoExposure(measure);
                    }
                    catch (Exception ex)
                    {
                        _dialogService.ShowException(ex, "Failed to start auto exposure");
                    }
                }));

        public AutoRelayCommand AbortCommand =>
            _abortCommand ?? (_abortCommand = new AutoRelayCommand(
                () =>
                {
                    _algorithmsSupervisor.CancelAutoExposure();
                }));

        public override void Loaded()
        {
            base.Loaded();
            IsActive = true;
        }

        public override void Unloading()
        {
            base.Unloading();
            IsActive = false;
        }

        private void HandleRecipeMessage(RecipeMessage message)
        {
            TotalSteps = message.Status.TotalSteps + 1;
            CurrentStep = message.Status.CurrentStep;
            Message = message.Status.Message;

            if (message.Status is AutoExposureStatus)
                ExposureTimeMs = ((AutoExposureStatus)message.Status).ExposureTimeMs;
            if (message.Status.State == DMTRecipeState.Failed ||
                message.Status.State == DMTRecipeState.Aborted)
                ProgessBarColor = Brushes.Red;
            else
                ProgessBarColor = s_defaultProgessBarColor;
            IsSuccessful = message.Status.State == DMTRecipeState.ExecutionComplete;

            switch (message.Status.State)
            {
                case DMTRecipeState.ExecutionComplete:
                    Task.Run(() =>
                    {
                        SetScreenImage();
                        // We could take into account the zoombox scale like in the ManualExposureSettingsView   Math.Min(1, ZoomboxScale * 2);
                        double scale = 1;
                        ServiceImage svcimg = _cameraSupervisor.GetCalibratedImageWithStatistics(Measure.Side,
                                Int32Rect.Empty, scale, new ROI());
                        Application.Current?.Dispatcher.Invoke(() => CameraBitmapSource = svcimg.WpfBitmapSource);
                        CurrentStep = TotalSteps;
                        Measure.ExposureTimeMs = Math.Round(ExposureTimeMs, 4);
                        IsRecipeRunning = false;
                        UpdateAllCanExecutes();
                    });
                    break;

                case DMTRecipeState.Executing:
                    break;

                default:
                    IsRecipeRunning = false;
                    UpdateAllCanExecutes();
                    break;
            }
        }

        public void Receive(ImageGrabbedMessage message)
        {
            Application.Current?.Dispatcher.Invoke(() => CameraBitmapSource = message.ServiceImage.WpfBitmapSource);
        }

        public void Receive(RecipeMessage message)
        {
            Application.Current?.Dispatcher.Invoke(() => HandleRecipeMessage(message));
        }

        private void SetScreenImage()
        {
            if (_selectedColorIndex == 0)
                _screenSupervisor.SetScreenColor(Measure.Side, Measure.Color);
            else if (_selectedColorIndex == 1)
                _screenSupervisor.DisplayFringe(Measure.Side, Measure.Fringe, 0, Measure.Color);
            else
                _screenSupervisor.DisplayHighAngleDarkFieldMaskOnSide(Measure.Side, Measure.Color);
        }
    }
}
