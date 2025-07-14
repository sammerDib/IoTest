using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using LightningChartLib.WPF.ChartingMVVM;

using UnitySC.PM.DMT.CommonUI.Proxy;
using UnitySC.PM.DMT.CommonUI.ViewModel.Measure;
using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.PM.DMT.Shared;
using UnitySC.PM.DMT.Shared.UI.Proxy;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.UI;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;
using UnitySC.Shared.UI.ViewModel.Navigation;

using Xceed.Wpf.Toolkit.Zoombox;

namespace UnitySC.PM.DMT.CommonUI.ViewModel.ExposureSettings
{
    internal abstract class ManualExposureSettingsVM : PageNavigationVM
    {
        protected CameraSupervisor CameraSupervisor;
        protected ScreenSupervisor ScreenSupervisor;
        protected CalibrationSupervisor CalibrationSupervisor;
        protected AlgorithmsSupervisor AlgorithmsSupervisor;
        protected IDialogOwnerService DialogService;
        protected Mapper Mapper;
        protected MainRecipeEditionVM MainRecipeEditionVM;
        private readonly string _title = "Manual Exposure";
        private Side _side => Measure.Side;
        protected MeasureVM Measure;
        private long _imageId = -1;
        private readonly object _lock = new object();

        private ManualExposureSettingsVM()
        {
            //Set Deployment Key for Arction components -- à offusquer
            string deploymentKey = "lgCAAMDQWCaC7NkBJABVcGRhdGVhYmxlVGlsbD0yMDI1LTEwLTE1I1JldmlzaW9uPTAI/z8AAAAA/AEy9k+WSPYWl8F5j8DR3/xHV8rmwsD6bgDd0+Kv9Aki7UvzB0KJxrUPqzZmcZ1hEhm6bFr+fezEYuukPWEkI7pybF86LTroOuA934Gci/KuDUrhHiaqtxFeaR30Gcgr25NjTyEpauRATjQ4BFk32TnkLwotmJoCv+HYAJkkvd85VCzS0o5fd4w99JHK3/XtyJSYL8/OCCrqumTQZm5A8s7q95M8AfxmeLTEUjPFJp/k+m0oTPHF4er+PTE/m1R/r1+yL6ZeiCzkuFB5m4vLE1vxa7ZEp0aRQ01Xw+0LPPBusgBj4089eXfVWH3DsnFfDmPrFn63MByaFqpzT/hK4J0EiXGqHRaGz8CCiRVxAO3mAT7DirAypxLrrF+142Z3f3iQnd88mRsFiTN2rqfbZDFmPPaK2j4LwDwqKiaVCOz6ISQpG8W7UOMSZjX1KnMiS+FdQRYJJPuuE0WGRMutSyrNHGawAsMY6J4hOh4hDsJsRgN3onrFG+pCHwFG/fUD154=";           // Setting Deployment Key for fully bindable chart
            LightningChartLib.WPF.ChartingMVVM.LightningChart.SetDeploymentKey(deploymentKey);
            LightningChartLib.WPF.Charting.LightningChart.SetDeploymentKey(deploymentKey);
        }

        protected ManualExposureSettingsVM(string title, MeasureVM measure, CameraSupervisor cameraSupervisor, ScreenSupervisor screenSupervisor, CalibrationSupervisor calibrationSupervisor,
            AlgorithmsSupervisor algorithmsSupervisor, IDialogOwnerService dialogService, Mapper mapper, MainRecipeEditionVM mainRecipeEditionVM) : this()
        {
            CameraSupervisor = cameraSupervisor;
            ScreenSupervisor = screenSupervisor;
            CalibrationSupervisor = calibrationSupervisor;
            AlgorithmsSupervisor = algorithmsSupervisor;
            DialogService = dialogService;
            Mapper = mapper;
            MainRecipeEditionVM = mainRecipeEditionVM;

            _title = $"{measure.Title} - {title}";
            Measure = measure;
            ROI.CameraId = measure.Side;
            ROI.WaferRadius = measure.WaferDimensions.Diameter.Micrometers / 2;          
            DialogService = dialogService;

        }      

        public override void Loaded()
        {
            base.Loaded();

            SetScreenImage();           
            _isGrabbing = true;
            StartGrab();
        }

        public override void Unloading()
        {
            base.Unloading();
            StopGrab();
            ClearScreen();
        }

        private bool _isGrabbing = true;

        private void DisplayTask()
        {
            try
            {
                while (_isGrabbing)
                {
                    double scale = ZoomboxScale == 0 ? 1 : Math.Min(1, ZoomboxScale * 2);
                    ROI roi = Mapper.AutoMap.Map<ROI>(ROI);
                    ServiceImageWithStatistics svcimg = CameraSupervisor.GetCalibratedImageWithStatistics(_side, Int32Rect.Empty, scale, roi);

                    var app = Application.Current;
                    if (app == null)
                        _isGrabbing = false;    // bidouille pour arrêter le thread quand l'application est fermée

                    if (_isGrabbing)
                        app.Dispatcher.Invoke(new Action(() => { DisplayCameraImage(svcimg); }));
                }
            }
            catch
            {
                // ignored
            }
        }

        public void StartGrab()
        {
            lock (_lock)
            {
                CameraInfo info = CameraSupervisor.GetCameraInfo(_side);
                if (info == null)
                    return;
                CameraWidth = info.Width;
                CameraHeight = info.Height;
                MinExposureTimeMs = info.MinExposureTimeMs;
                MaxExposureTimeMs = info.MaxExposureTimeMs;

                System.Drawing.Size size = CameraSupervisor.GetCalibratedImageSize(_side);
                ImageWidth = size.Width;
                ImageHeight = size.Height;

                _isGrabbing = true;
                SetExposureTime(ExposureTimeMs);
                CameraSupervisor.StartContinuousAcquisition(_side);
                Task.Factory.StartNew(DisplayTask, TaskCreationOptions.LongRunning);
            }
        }

        public void StopGrab()
        {
            lock (_lock)
            {
                BusyHourglass.SetBusyState();
                _isGrabbing = false;
                CameraSupervisor.StopContinuousAcquisition(_side);
            }
        }

        public void ClearScreen()
        {
            ScreenSupervisor.SetScreenColor(_side, Colors.Black, false);
        }

        public virtual void SetExposureTime(double exposureTimeMs)
        {
            CameraSupervisor.SetExposureTime(Measure.Side, exposureTimeMs);
        }

        public override string PageName => _title;

        private bool _canChangeScreenDisplay = true;

        public bool CanChangeScreenDisplay
        {
            get => _canChangeScreenDisplay; set { if (_canChangeScreenDisplay != value) { _canChangeScreenDisplay = value; OnPropertyChanged(); } }
        }

        public bool HasToolCalibration { get => CalibrationSupervisor.HasPerspectiveCalibration(_side); }

        private BitmapSource _cameraBitmapSource;

        public BitmapSource CameraBitmapSource
        { get => _cameraBitmapSource; set { if (_cameraBitmapSource != value) { _cameraBitmapSource = value; OnPropertyChanged(); } } }

        private int _cameraWidth;

        public int CameraWidth
        { get => _cameraWidth; set { if (_cameraWidth != value) { _cameraWidth = value; OnPropertyChanged(); } } }

        private int _cameraHeight;

        public int CameraHeight
        { get => _cameraHeight; set { if (_cameraHeight != value) { _cameraHeight = value; OnPropertyChanged(); } } }

        private int _imageWidth;

        public int ImageWidth
        {
            get => Math.Max(10, _imageWidth); set { if (_imageWidth != value) { _imageWidth = value; OnPropertyChanged(); } }
        }

        private int _imageHeight;

        public int ImageHeight
        {
            get => Math.Max(10, _imageHeight); set { if (_imageHeight != value) { _imageHeight = value; OnPropertyChanged(); } }
        }

        private int _imageCount;

        public int ImageCount
        { get => _imageCount; set { if (_imageCount != value) { _imageCount = value; OnPropertyChanged(); } } }

        public double ExposureTimeMs
        {
            get => Measure.ExposureTimeMs;
            set
            {
                if (Measure.ExposureTimeMs != value)
                {
                    Measure.ExposureTimeMs = value;
                    OnPropertyChanged();
                }
            }
        }

        public int IlluminationTarget
        {
            get => Measure.AutoExposureTargetSaturation;
            set
            {
                if (Measure.AutoExposureTargetSaturation != value)
                {
                    Measure.AutoExposureTargetSaturation = value;
                    OnPropertyChanged();
                }
            }
        }

        private double _minExposureTimeMs = 10;

        public double MinExposureTimeMs
        { get => _minExposureTimeMs; set { if (_minExposureTimeMs != value) { _minExposureTimeMs = value; OnPropertyChanged(); } } }

        private double _maxExposureTimeMs = 500;

        public double MaxExposureTimeMs
        { get => _maxExposureTimeMs; set { if (_maxExposureTimeMs != value) { _maxExposureTimeMs = value; OnPropertyChanged(); } } }

        private int _selectedScreenColorIndex;

        public int SelectedScreenColorIndex
        {
            get => _selectedScreenColorIndex;
            set
            {
                if (_selectedScreenColorIndex != value)
                {
                    _selectedScreenColorIndex = value;
                    SetScreenImage();
                    OnPropertyChanged();
                }
            }
        }

        public Color Color
        {
            get => Measure.Color;
            set
            {
                if (value != Measure.Color)
                {
                    Measure.Color = value;
                    SetScreenImage();
                    OnPropertyChanged();
                }
            }
        }

        public string ColorName => Color.ToString();
        public Fringe Fringe => Measure.Fringe;
        public RoiVM ROI => Measure.ROI;

        private bool _isShowingHistogram = true;

        public bool IsShowingHistogram
        {
            get => _isShowingHistogram;
            set { if (_isShowingHistogram != value) { _isShowingHistogram = value; _isShowingProfile = !value; OnPropertyChanged(); OnPropertyChanged(nameof(IsShowingProfile)); } }
        }

        private bool _isShowingProfile = false;

        public bool IsShowingProfile
        {
            get => _isShowingProfile;
            set { if (_isShowingProfile != value) { _isShowingProfile = value; _isShowingHistogram = !value; OnPropertyChanged(); OnPropertyChanged(nameof(IsShowingHistogram)); } }
        }

        private double _histogramMinX = 0;

        public double HistogramMinX
        { get => _histogramMinX; set { if (_histogramMinX != value) { _histogramMinX = value; OnPropertyChanged(); } } }

        private double _histogramMaxX = 255;

        public double HistogramMaxX
        { get => _histogramMaxX; set { if (_histogramMaxX != value) { _histogramMaxX = value; OnPropertyChanged(); } } }

        private double _histogramMaxY = 100;

        public double HistogramMaxY
        { get => _histogramMaxY; set { if (_histogramMaxY != value) { _histogramMaxY = value; OnPropertyChanged(); } } }

        private string _axisXTitle = "";

        public string AxisXTitle
        { get => _axisXTitle; set { if (_axisXTitle != value) { _axisXTitle = value; OnPropertyChanged(); } } }

        private string _axisYTitle = "";

        public string AxisYTitle
        { get => _axisYTitle; set { if (_axisYTitle != value) { _axisYTitle = value; OnPropertyChanged(); } } }

        private BarSeriesValue[] _histogramBars;

        public BarSeriesValue[] HistogramBars
        { get => _histogramBars; set { if (_histogramBars != value) { _histogramBars = value; OnPropertyChanged(); } } }

        private SeriesPoint[] _profilePoints;

        public SeriesPoint[] ProfilePoints
        { get => _profilePoints; set { if (_profilePoints != value) { _profilePoints = value; OnPropertyChanged(); } } }

        private string _statInfo;

        public string StatInfo
        { get => _statInfo; set { if (_statInfo != value) { _statInfo = value; OnPropertyChanged(); } } }

        private ZoomboxView _zoomboxView;

        public ZoomboxView ZoomboxView
        { get => _zoomboxView; set { if (_zoomboxView != value) { _zoomboxView = value; OnPropertyChanged(); } } }

        private ZoomboxView _fullscreenZoomboxView;

        public ZoomboxView FullscreenZoomboxView
        { get => _fullscreenZoomboxView; set { if (_fullscreenZoomboxView != value) { _fullscreenZoomboxView = value; OnPropertyChanged(); } } }

        private double _zoomboxScale = 1;

        public double ZoomboxScale
        { get => _zoomboxScale; set { if (_zoomboxScale != value) { _zoomboxScale = value; OnPropertyChanged(); } } }



        private AutoRelayCommand _applyExposureTimeCommand;

        public AutoRelayCommand ApplyExposureTimeCommand
        {
            get
            {
                return _applyExposureTimeCommand ?? (_applyExposureTimeCommand = new AutoRelayCommand(
              () =>
              {
                  SetExposureTime(ExposureTimeMs);
              }));
            }
        }

        private AutoRelayCommand _fullScreenCommand;

        public AutoRelayCommand FullScreenCommand
        {
            get
            {
                return _fullScreenCommand ?? (_fullScreenCommand = new AutoRelayCommand(
              () =>
              {
                  Window popup = new Window();
                  popup.DataContext = this;
                  popup.Content = new View.ExposureSettings.ManualExposureSettingsFullScreenView();
                  popup.WindowState = WindowState.Maximized;
                  popup.WindowStyle = WindowStyle.None;
                  popup.Owner = Application.Current.MainWindow;
                  popup.ShowDialog();
              }));
            }
        }



        private AutoRelayCommand _autoSetExposureTimeCommand;

        public AutoRelayCommand AutoSetExposureTimeCommand
        {
            get
            {
                return _autoSetExposureTimeCommand ?? (_autoSetExposureTimeCommand = new AutoRelayCommand(() =>
                    {
                        var vm = new ComputeExposureSettingsVM(Measure, Mapper, CameraSupervisor, ScreenSupervisor, AlgorithmsSupervisor, DialogService, SelectedScreenColorIndex);
                        MainRecipeEditionVM.EditedRecipe.Navigate(vm);
                    }));
            }
        }


        private AutoRelayCommand _applyRoiSettingsToAllMeasuresCommand;

        public AutoRelayCommand ApplyRoiSettingsToAllMeasuresCommand
        {
            get
            {
                return _applyRoiSettingsToAllMeasuresCommand ?? (_applyRoiSettingsToAllMeasuresCommand = new AutoRelayCommand(
              () =>
              {
                  var result = DialogService.ShowMessageBox("The ROI settings will be applied across all Measures. Are you sure you want to proceed?", "Warning: ROI Changes", MessageBoxButton.YesNo, MessageBoxImage.Question);
                  if (result == MessageBoxResult.Yes)
                  {
                      ApplyRoiSettingsToAllMeasures();

                  }
                  else { return;}
              }));
            }
        }
        private void DisplayCameraImage(ServiceImageWithStatistics svcimage)
        {
            if (!_isGrabbing || svcimage == null)
                return;

            CameraBitmapSource = svcimage.WpfBitmapSource;

            if (svcimage.ImageId != _imageId)
            {
                _imageId = svcimage.ImageId;
                ImageCount++;
            }

            if (svcimage.Histogram != null)
            {
                StatInfo = $"min={svcimage.Min} max={svcimage.Max} mean={Math.Round(svcimage.Mean, 1)} stddev={Math.Round(svcimage.StandardDeviation, 1)}";
                if (IsShowingHistogram)
                    UpdateHistogramBars(svcimage.Histogram);
                else
                    UpdateProfilePoints(svcimage.Profile);
            }
            else
            {
                StatInfo = "";
                ProfilePoints = null;
            }
        }

        private void UpdateHistogramBars(long[] data)
        {
            ProfilePoints = null;
            if (data == null)
            {
                //HistogramBars.RemoveAllBefore(double.PositiveInfinity);
                return;
            }

            var bars = new BarSeriesValue[data.Length];
            for (int i = 0; i < data.Length; i++)
                bars[i] = new BarSeriesValue() { Value = data[i], Location = i };
            HistogramBars = bars;

            SetHistogramXRange(-1, data.Length + 1);
            HistogramMaxY = data.Max() * 1.05;
            AxisXTitle = "Grey Level";
            AxisYTitle = "Nb Pixels";
        }

        private void UpdateProfilePoints(long[] data)
        {
            //HistogramBars.RemoveAllBefore(double.PositiveInfinity);
            if (data == null)
            {
                ProfilePoints = null;
                return;
            }

            var points = new SeriesPoint[data.Length];
            double xmin = ROI.MicronX;
            double xmax = xmin + ROI.MicronWidth;
            double dx = ROI.MicronWidth / (data.Length - 1);
            for (int i = 0; i < data.Length; i++)
                points[i] = new SeriesPoint() { X = xmin + i * dx, Y = data[i] };

            ProfilePoints = points;
            SetHistogramXRange(xmin, xmax);
            HistogramMaxY = 255;
            AxisXTitle = "X (µm)";
            AxisYTitle = "Grey Level";
        }

        private void SetHistogramXRange(double xmin, double xmax)
        {
            if (HistogramMaxX < xmin)
            {
                HistogramMaxX = xmax;
                HistogramMinX = xmin;
            }
            else
            {
                HistogramMinX = xmin;
                HistogramMaxX = xmax;
            }
        }

        public void SetScreenImage()
        {
            if (SelectedScreenColorIndex == 0)
                ScreenSupervisor.SetScreenColor(_side, Color);
            else if (SelectedScreenColorIndex == 1)
                ScreenSupervisor.DisplayFringe(_side, Fringe, 0, Color);
            else
                ScreenSupervisor.DisplayHighAngleDarkFieldMaskOnSide(_side, Color);
        }

        private void ApplyRoiSettingsToAllMeasures()
        {

            foreach (MeasureVM targetMeasure in MainRecipeEditionVM.EditedRecipe.Measures)
            {
                if (targetMeasure.Side == Measure.Side && targetMeasure.Title != Measure.Title  && targetMeasure.IsEnabled)
                {
                    CopyRoiSettings(Measure.ROI, targetMeasure.ROI);
                }
            }
        }

        private void CopyRoiSettings(RoiVM sourceRoi, RoiVM targetRoi)
        {
            if (sourceRoi == null || targetRoi == null)
                return;
            // Copy each property from source to target ROI
            targetRoi.CameraId = sourceRoi.CameraId;
            targetRoi.WaferRadius = sourceRoi.WaferRadius;
            targetRoi.EdgeExclusion = sourceRoi.EdgeExclusion;
            targetRoi.RoiType = sourceRoi.RoiType;
            targetRoi.MicronX = sourceRoi.MicronX;
            targetRoi.MicronY = sourceRoi.MicronY;
            targetRoi.MicronWidth = sourceRoi.MicronWidth;
            targetRoi.MicronHeight = sourceRoi.MicronHeight;
            OnPropertyChanged(nameof(ROI));
        }
    }
}
