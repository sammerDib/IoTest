using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.ANA.Client.Proxy.Camera;
using UnitySC.PM.ANA.Service.Interface.Camera;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.ViewModel;

using Application = System.Windows.Application;
using Message = UnitySC.Shared.Tools.Service.Message;

namespace UnitySC.PM.ANA.Client.Proxy
{
    public class CameraVM : ViewModelBaseExt
    {
        #region Fields

        private const int ExpectedFramerate = 24;
        private const double FramePeriod = 1000 / ExpectedFramerate;

        private readonly ILogger _logger;

        private ICameraServiceEx _cameraSupervisor;
        private GlobalStatusSupervisor _globalStatusSupervisor;

        private string _cameraID;
        private string _cameraName;
        private CameraInfo _cameraInfo;
        private BitmapSource _cameraBitmapSource;
        private int _realFramerate;
        private bool _isNormalised = false;

        private bool _inputParametersChange = false;

        private bool _isGrabbing = false;
        private bool _stopStreamingAsked = false;
        private object _grabSynchro = new object();

        private AutoRelayCommand _setSettings;
        private AutoRelayCommand _startStreamingCommand;
        private AutoRelayCommand _stopStreamingCommand;
        private AutoRelayCommand _singleShotCommand;
        private CancellationTokenSource _tokenSource;

        #endregion Fields

        #region Constructors

        public CameraVM(ICameraServiceEx cameraSupervisor, CameraConfigBase cameraConfig, IMessenger messenger, ILogger logger)
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return;

            _logger = logger;

            _cameraSupervisor = cameraSupervisor;
            _globalStatusSupervisor = ClassLocator.Default.GetInstance<SharedSupervisors>().GetGlobalStatusSupervisor(ActorType.ANALYSE);

            _cameraID = cameraConfig.DeviceID;
            _cameraName = cameraConfig.Name;
            _cameraInfo = _cameraSupervisor.GetCameraInfo(_cameraID)?.Result;
            IsMainCamera = cameraConfig.IsMainCamera;
            Configuration = cameraConfig;
            _tokenSource = new CancellationTokenSource();

            InputParameters = GetSettings();

            messenger.Register<ImageGrabbedMessage>(this, (r, m) =>
                Application.Current?.Dispatcher.Invoke(() => CameraBitmapSource = m.ServiceImage.WpfBitmapSource));
        }

        #endregion Constructors

        #region Public methods

        // TODO : Methode dupliquer depuis AxesVM : deplacer la méthode dans ObservableRecipientExt ?
        public void AddMessagesOrExceptionToErrorsList<T>(Response<T> response, Exception e)
        {
            // If there is at least one message
            if ((response != null) && response.Messages.Any())
            {
                foreach (var message in response.Messages)
                {
                    if (!string.IsNullOrEmpty(message.UserContent))
                        _globalStatusSupervisor.SendUIMessage(message);
                }
            }
            else
            {
                if (e != null)
                    _globalStatusSupervisor.SendUIMessage(new Message(MessageLevel.Error, e.Message));
            }
        }

        // TODO : Methode présente dans plusieurs CameraVM de différents modules. Refactoring ?
        public void SaveBitmapSource(BitmapSource bitmap, PathString filename)
        {
            using (var fileStream = new FileStream(filename, FileMode.Create))
            {
                BitmapEncoder encoder;
                string ext = filename.Extension.ToLower();
                switch (ext)
                {
                    case ".png":
                        encoder = new PngBitmapEncoder();
                        break;

                    case ".tif":
                    case ".tiff":
                        encoder = new TiffBitmapEncoder();
                        break;

                    case ".jpg":
                    case ".jpeg":
                        encoder = new JpegBitmapEncoder();
                        break;

                    case ".bmp":
                    default:
                        encoder = new BmpBitmapEncoder();
                        break;
                }
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                encoder.Save(fileStream);
            }
        }

        #endregion Public methods

        #region Private methods

        private void InputParameters_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            InputParametersChange = true;
        }

        private async Task GrabTask()
        {
            _cameraSupervisor.StartAcquisition(_cameraID);
            IsGrabbing = true;
            _stopStreamingAsked = false;
            var cancellationToken = _tokenSource.Token;
            try
            {
                while (!_stopStreamingAsked)
                {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();

                    var svcimg = _cameraSupervisor.GetCameraImage(_cameraID)?.Result;
                    var msg = new ImageGrabbedMessage() { ServiceImage = svcimg };

                    Application.Current?.Dispatcher.Invoke(new Action(() => DisplayCameraImage(msg)), System.Windows.Threading.DispatcherPriority.Normal, cancellationToken);

                    stopwatch.Stop();
                    int wait = (int)(FramePeriod - stopwatch.ElapsedMilliseconds);
                    if (wait > 0)
                    {
                        await Task.Delay(wait * 3);
                    }
                    RealFramerate = (int)(1000 / (stopwatch.ElapsedMilliseconds + wait));
                }
            }
            catch (Exception e)
            {
                if (!(e is TaskCanceledException))
                {
                    throw;
                }
            }
            finally
            {
                // In case of TaskCanceledException, we simply stop the GrabTask
                _tokenSource = new CancellationTokenSource();
                Application.Current?.Dispatcher.BeginInvoke(new Action(() => CameraBitmapSource = null));
                _cameraSupervisor.StopAcquisition(_cameraID);
                IsGrabbing = false;
                _stopStreamingAsked = false;
            }
        }

        private void DisplayCameraImage(ImageGrabbedMessage message)
        {
            var svcimage = message.ServiceImage;
            if (!(svcimage is null))
            {
                // For Debug
                //CameraBitmapSource = new BitmapImage(new Uri("c:\\Temp\\Test.bmp"));

                if (IsNormalised)
                    CameraBitmapSource = PerformNormalization(message.ImageSource);
                else
                    CameraBitmapSource = message.ImageSource;
            }
        }

        private ICameraInputParams GetInputParams()
        {
            var mapper = ClassLocator.Default.GetInstance<Mapper>();
            var inputParams = mapper.AutoMap.Map<CameraInputParams>(InputParameters);
            return inputParams;
        }

        private CameraInputParametersVM GetSettings()
        {
            var tempParams = _cameraSupervisor.GetSettings(_cameraID)?.Result;
            var inputParameters = new CameraInputParametersVM()
            {
                Gain = tempParams?.Gain ?? 0.0,
                ExposureTimeMs = tempParams?.ExposureTimeMs ?? 0.0,
                FrameRate = tempParams?.FrameRate ?? 0.0,
                ColorMode = tempParams?.ColorMode ?? string.Empty,
            };
            inputParameters.PropertyChanged += InputParameters_PropertyChanged;
            return inputParameters;
        }

        private BitmapSource PerformNormalization(BitmapSource imageSource)
        {
            if (imageSource == null)
                return null;

            var width = imageSource.PixelWidth;
            var height = imageSource.PixelHeight;
            var bytesPerPixel = (imageSource.Format.BitsPerPixel + 7) / 8;
            var bufstride = width * bytesPerPixel;

            if (bytesPerPixel != 1)
                return null;

            var bytes = new byte[height * bufstride];
            imageSource.CopyPixels(bytes, bufstride, 0);
            var lut = new byte[256];

            byte min = 255;
            byte max = 0;
            unsafe
            {
                var totalPixels = width * height;
                fixed (byte* pixels = bytes)
                {
                    byte* p = pixels;
                    int k;
                    for (k = 0; k < totalPixels; k++)
                    {
                        if (min > *p)
                            min = *p;
                        if (max < *p)
                            max = *p;
                        ++p;
                    }

                    if ((max - min) == 0)
                    {
                        max++;
                        min--;
                        max = Math.Min((byte)255, max);
                        min = Math.Max((byte)0, min);
                    }

                    float coef = 255.0f / (float)(max - min);
                    for (k = 0; k != min; k++)
                    {
                        lut[k] = 0;
                    }
                    for (; k != max; k++)
                    {
                        lut[k] = (byte)((float)(k - min) * coef + 0.5f);
                    }
                    for (; k != 256; k++)
                    {
                        lut[k] = 255;
                    }

                    // feed reset back to start and apply lut
                    p = pixels;
                    for (k = 0; k < totalPixels; k++)
                    {
                        *p = lut[*p];
                        ++p;
                    }
                }
            }

            var normedbitmap = new WriteableBitmap(width, height, imageSource.DpiX, imageSource.DpiY, imageSource.Format, null);
            normedbitmap.WritePixels(new Int32Rect(0, 0, width, height), bytes, bufstride, 0);
            return normedbitmap;
        }

        #endregion Private methods

        #region Properties

        public string Name
        {
            get => _cameraName; set { if (_cameraName != value) { _cameraName = value; OnPropertyChanged(); } }
        }

        public int RealFramerate
        {
            get => _realFramerate; set { if (_realFramerate != value) { _realFramerate = value; OnPropertyChanged(); } }
        }

        public bool IsMainCamera { get; private set; }

        public CameraInputParametersVM InputParameters { get; set; }

        public bool InputParametersChange
        {
            get => _inputParametersChange;
            set
            {
                if (_inputParametersChange == value)
                    return;
                _inputParametersChange = value;
                OnPropertyChanged();
            }
        }

        public CameraInfo CameraInfo
        {
            get => _cameraInfo;
            set
            {
                if (_cameraInfo == value)
                    return;
                _cameraInfo = value;
                OnPropertyChanged();
            }
        }

        public CameraConfigBase Configuration { get; set; }

        public BitmapSource CameraBitmapSource
        {
            get
            {
                return _cameraBitmapSource;
            }

            set
            {
                if (_cameraBitmapSource != value)
                {
                    var pixelWidthChanged = _cameraBitmapSource?.PixelWidth != value?.PixelWidth;
                    var pixelHeightChanged = _cameraBitmapSource?.PixelHeight != value?.PixelHeight;
                    _cameraBitmapSource = value;
                    OnPropertyChanged();
                    if (pixelWidthChanged) OnPropertyChanged(nameof(ImageWidth));
                    if (pixelHeightChanged) OnPropertyChanged(nameof(ImageHeight));
                }
            }
        }

        public long ImageWidth => _cameraBitmapSource?.PixelWidth ?? 0;

        public long ImageHeight => _cameraBitmapSource?.PixelHeight ?? 0;

        public bool IsGrabbing
        {
            get => _isGrabbing;
            protected set
            {
                if (_isGrabbing == value)
                    return;
                _isGrabbing = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsReady));
            }
        }

        public bool IsReady { get => !_isGrabbing; }

        public bool IsNormalised
        {
            get => _isNormalised;
            set
            {
                if (_isNormalised == value)
                    return;
                _isNormalised = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties

        #region RelayCommands

        public AutoRelayCommand SetSettings
        {
            get
            {
                return _setSettings ?? (_setSettings = new AutoRelayCommand(
                    () =>
                    {
                        Response<bool> response = null;
                        try
                        {
                            var resSetSettings = _cameraSupervisor.SetSettings(_cameraID, GetInputParams())?.Result;
                            if (resSetSettings == null || resSetSettings == false)
                                InputParameters = GetSettings();

                            InputParametersChange = false;
                        }
                        catch (Exception e)
                        {
                            AddMessagesOrExceptionToErrorsList(response, e);
                        }
                        finally
                        {
                            CameraInfo = _cameraSupervisor.GetCameraInfo(_cameraID)?.Result;
                        }
                    }));
            }
        }

        public AutoRelayCommand StartStreamingCommand
        {
            get
            {
                return _startStreamingCommand ?? (_startStreamingCommand = new AutoRelayCommand(
                    () =>
                    {
                        StartStreaming();
                    }));
            }
        }

        public bool StartStreaming()
        {
            lock (_grabSynchro)
            {
                if (_stopStreamingAsked)
                {
                    bool success = true;
                    Task.Run(() => success = SpinWait.SpinUntil(() => _isGrabbing == false, 20000)).Wait();

                    if (!success)
                    {
                        _logger?.Error("Failed to start the camera streaming");
                    }
                }
                if (!_isGrabbing)
                {
                    if (_inputParametersChange)
                    {
                        SetSettings.Execute(null);
                    }

                    Task.Run(async () =>
                    {
                        try
                        {
                            await GrabTask();
                        }
                        catch (Exception e)
                        {
                            _logger?.Error(e, "Error during camera acquisition");
                            _globalStatusSupervisor.SendUIMessage(new Message(MessageLevel.Error, "Error during camera acquisition"));
                        }
                    });
                }
                else
                {
                    _logger?.Information("Camera acquisition already started");
                }

                return true;
            }
        }

        public AutoRelayCommand StopStreamingCommand
        {
            get
            {
                return _stopStreamingCommand ?? (_stopStreamingCommand = new AutoRelayCommand(
                    () =>
                    {
                        StopStreaming();
                    }));
            }
        }

        public void StopStreaming()
        {
            lock (_grabSynchro)
            {
                if (_isGrabbing)
                {
                    _stopStreamingAsked = true;
                    _tokenSource.Cancel();
                }
            }
        }

        public AutoRelayCommand SingleShotCommand
        {
            get
            {
                return _singleShotCommand ?? (_singleShotCommand = new AutoRelayCommand(
                    () =>
                    {
                        if (_inputParametersChange)
                        {
                            SetSettings.Execute(null);
                        }
                        var svcimg = _cameraSupervisor.GetCameraImage(_cameraID)?.Result;
                        var msg = new ImageGrabbedMessage() { ServiceImage = svcimg };
                        if (msg.ImageSource != null)
                        {
                            var bmpSource = IsNormalised ? msg.ImageSource : PerformNormalization(msg.ImageSource);

                            var saveFileDialog = new SaveFileDialog();
                            saveFileDialog.FileName = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                            saveFileDialog.Filter = "png|*.png|bmp|*.bmp|jpeg|*.jpg";
                            if (DialogResult.OK == saveFileDialog.ShowDialog())
                            {
                                if (!String.IsNullOrEmpty(saveFileDialog.FileName))
                                    SaveBitmapSource(bmpSource, new PathString(saveFileDialog.FileName));
                            }
                        }
                    }));
            }
        }

        #endregion RelayCommands
    }
}
