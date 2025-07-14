using System;
using System.Windows;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.UI.AutoRelayCommandExt;

using MvvmDialogs.FrameworkDialogs.SaveFile;

using UnitySC.Shared.ResultUI.Common.View.ImageViewer;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.Shared.ResultUI.Common.ViewModel.ImageViewer
{
    /// <summary>
    /// Template Model representing the ViewModel (DataContext) of <see cref="ImageViewerView"/>
    /// </summary>
    public class ImageViewerViewModel : ObservableObject, IDisposable
    {
        private readonly Action<string> _exportAction;
        private readonly string _extension;
        private readonly string _fileName;

        #region Properties

        private ImageSource _image;

        public ImageSource Image
        {
            get => _image;
            private set => SetProperty(ref _image, value);
        }

        public bool IsProfileDrawingEnabled { get; }

        private int? _mouseOverX;

        public int? MouseOverX
        {
            get => _mouseOverX;
            set
            {
                SetProperty(ref _mouseOverX, value);
                OnMousePosChanged();
            }
        }

        private int? _mouseOverY;

        public int? MouseOverY
        {
            get => _mouseOverY;
            set
            {
                SetProperty(ref _mouseOverY, value);
                OnMousePosChanged();
            }
        }

        private string _currentValue;

        public string CurrentValue
        {
            get { return _currentValue; }
            set { SetProperty(ref _currentValue, value); }
        }

        private string _mouseOverXInformation;

        public string MouseOverXInformation
        {
            get { return _mouseOverXInformation; }
            set { SetProperty(ref _mouseOverXInformation, value); }
        }

        private string _mouseOverYInformation;

        public string MouseOverYInformation
        {
            get { return _mouseOverYInformation; }
            set { SetProperty(ref _mouseOverYInformation, value); }
        }

        private string _currentValueInformation;

        public string CurrentValueInformation
        {
            get { return _currentValueInformation; }
            set { SetProperty(ref _currentValueInformation, value); }
        }

        #region Profile Coordinates

        private int? _startPointX;

        public int? StartPointX
        {
            get { return _startPointX; }
            set
            {
                if (SetProperty(ref _startPointX, value))
                {
                    UpdateProfile();
                }
            }
        }

        private int? _startPointY;

        public int? StartPointY
        {
            get { return _startPointY; }
            set
            {
                if (SetProperty(ref _startPointY, value))
                {
                    UpdateProfile();
                }
            }
        }

        private int? _endPointX;

        public int? EndPointX
        {
            get { return _endPointX; }
            set
            {
                if (SetProperty(ref _endPointX, value))
                {
                    UpdateProfile();
                }
            }
        }

        private int? _endPointY;

        public int? EndPointY
        {
            get { return _endPointY; }
            set
            {
                if (SetProperty(ref _endPointY, value))
                {
                    UpdateProfile();
                }
            }
        }

        private int? _crossProfileX;

        public int? CrossProfileX
        {
            get { return _crossProfileX; }
            set
            {
                if (SetProperty(ref _crossProfileX, value))
                {
                    UpdateCrossProfile();
                }
            }
        }

        private int? _crossProfileY;

        public int? CrossProfileY
        {
            get { return _crossProfileY; }
            set
            {
                if (SetProperty(ref _crossProfileY, value))
                {
                    UpdateCrossProfile();
                }
            }
        }

        private double _markerX;

        public double MarkerX
        {
            get { return _markerX; }
            private set { SetProperty(ref _markerX, value); }
        }

        private double _markerY;

        public double MarkerY
        {
            get { return _markerY; }
            private set { SetProperty(ref _markerY, value); }
        }

        private double _horizontalMarkerX;

        public double HorizontalMarkerX
        {
            get { return _horizontalMarkerX; }
            set { SetProperty(ref _horizontalMarkerX, value); }
        }

        private double _verticalMarkerY;

        public double VerticalMarkerY
        {
            get { return _verticalMarkerY; }
            set { SetProperty(ref _verticalMarkerY, value); }
        }

        #endregion

        #region Profile Tools

        private ImageViewerTool _currentTool = ImageViewerTool.Move;

        public ImageViewerTool CurrentTool
        {
            get { return _currentTool; }
            set
            {
                if (SetProperty(ref _currentTool, value) && _currentTool != ImageViewerTool.Move)
                {
                    CurrentProfileTool = _currentTool;
                }
            }
        }

        private ImageViewerTool _currentProfileTool = ImageViewerTool.LineProfile;

        public ImageViewerTool CurrentProfileTool
        {
            get { return _currentProfileTool; }
            private set
            {
                if (SetProperty(ref _currentProfileTool, value))
                {
                    if (_currentProfileTool == ImageViewerTool.LineProfile)
                    {
                        LineProfileVisibility = Visibility.Visible;
                        CrossProfileVisibility = Visibility.Collapsed;
                        UpdateProfile();
                    }
                    else if (_currentProfileTool == ImageViewerTool.CrossProfile)
                    {
                        LineProfileVisibility = Visibility.Collapsed;
                        CrossProfileVisibility = Visibility.Visible;
                        UpdateCrossProfile();
                    }
                }
            }
        }

        private Visibility _lineProfileVisibility = Visibility.Visible;

        public Visibility LineProfileVisibility
        {
            get { return _lineProfileVisibility; }
            private set { SetProperty(ref _lineProfileVisibility, value); }
        }

        private Visibility _crossProfileVisibility = Visibility.Collapsed;

        public Visibility CrossProfileVisibility
        {
            get { return _crossProfileVisibility; }
            private set { SetProperty(ref _crossProfileVisibility, value); }
        }

        #endregion

        #endregion

        public ImageViewerViewModel(ImageSource image, Action<string> exportAction, string extension, string fileName, bool enableDrawProfile)
        {
            _exportAction = exportAction;
            _extension = extension;
            _fileName = fileName;
            Image = image;
            IsProfileDrawingEnabled = enableDrawProfile;

            if (image != null)
            {
                _viewRect = new Rect(new Point(0, 0), new Point(image.Width, image.Height));
            }
        }

        #region Actions

        public Action<int?, int?> OnMousePosChangedFunc { get; set; }

        private void OnMousePosChanged()
        {
            OnMousePosChangedFunc?.Invoke(MouseOverX, MouseOverY);
        }

        public Func<int, int, bool> OnMouseDownFunc { get; set; }

        public bool OnMouseDown(int x, int y) => OnMouseDownFunc != null && OnMouseDownFunc.Invoke(x, y);

        #endregion

        #region Commands

        private AutoRelayCommand _exportCommand;

        public AutoRelayCommand ExportCommand => _exportCommand ?? (_exportCommand = new AutoRelayCommand(ExportCommandExecute));

        private void ExportCommandExecute()
        {
            var dialogService = ClassLocator.Default.GetInstance<IDialogOwnerService>();
            var saveFileDialogSettings = new SaveFileDialogSettings()
            {
                Title = "Export thumbnail",
                AddExtension = true,
                DefaultExt = _extension,
                FileName = _fileName,
                CheckFileExists = false,
                Filter = "Png file (*.png)|*.png"
            };
            if (dialogService.ShowSaveFileDialog(saveFileDialogSettings) == true)
            {
                _exportAction(saveFileDialogSettings.FileName);
            }
        }

        private AutoRelayCommand _resetProfileCommand;

        public AutoRelayCommand ResetProfileCommand => _resetProfileCommand ?? (_resetProfileCommand = new AutoRelayCommand(ClearProfile));

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Image = null;
            GC.Collect();
        }

        #endregion

        #region Profile Events

        public event EventHandler<ProfileDrawnEventArgs> ProfileDrawn;

        private void UpdateProfile()
        {
            bool hasValues = StartPointX.HasValue && StartPointY.HasValue && EndPointX.HasValue && EndPointY.HasValue;
            if (!hasValues)
            {
                return;
            }

            ProfileDrawn?.Invoke(this, new ProfileDrawnEventArgs()
            {
                StartX = StartPointX.Value,
                StartY = StartPointY.Value,
                EndX = EndPointX.Value,
                EndY = EndPointY.Value
            });
        }

        public event EventHandler<CrossProfileDrawnEventArgs> CrossProfileDrawn;

        private void UpdateCrossProfile()
        {
            bool hasValues = CrossProfileY.HasValue && CrossProfileX.HasValue;
            if (!hasValues)
            {
                return;
            }

            CrossProfileDrawn?.Invoke(this, new CrossProfileDrawnEventArgs()
            {
                Horizontal = CrossProfileX.Value,
                Vertical = CrossProfileY.Value
            });
        }

        public event EventHandler ProfileCleared;

        #endregion

        #region Public Methods

        public void SetImage(ImageSource image)
        {
            Image = image;

            if (image != null)
            {
                ViewRect = new Rect(new Point(0, 0), new Point(image.Width, image.Height));
            }
        }

        public void ClearProfile()
        {
            SetProperty(ref _startPointX, null, nameof(StartPointX));
            SetProperty(ref _startPointY, null, nameof(StartPointY));
            SetProperty(ref _endPointX, null, nameof(EndPointX));
            SetProperty(ref _endPointY, null, nameof(EndPointY));
            SetProperty(ref _crossProfileX, null, nameof(CrossProfileX));
            SetProperty(ref _crossProfileY, null, nameof(CrossProfileY));
            SetProperty(ref _markerX, double.NaN, nameof(MarkerX));
            SetProperty(ref _markerY, double.NaN, nameof(MarkerY));

            ProfileCleared?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Private Methods

        public void UpdateLineProfileMarkerPosition(Point? point)
        {
            if (point == null)
            {
                MarkerX = double.NaN;
                MarkerY = double.NaN;
            }
            else
            {
                MarkerX = point.Value.X;
                MarkerY = point.Value.Y;
            }
        }

        public void UpdateCrossProfileHorizontalMarkerPosition(double? xPosition)
        {
            HorizontalMarkerX = xPosition ?? double.NaN;
        }

        public void UpdateCrossProfileVerticalMarkerPosition(double? yPosition)
        {
            VerticalMarkerY = yPosition ?? double.NaN;
        }

        #endregion

        public event EventHandler<Rect> ViewRectChanged;

        private Rect _viewRect;

        public Rect ViewRect
        {
            get { return _viewRect; }
            set { SetProperty(ref _viewRect, value); }
        }

        public void RaiseViewRectChanged(Rect viewRect)
        {
            // Used by the view to notify the change of the rect
            ViewRect = viewRect;
            ViewRectChanged?.Invoke(this, viewRect);
        }

        public void SetViewRect(Rect viewRect)
        {
            // Used by external ViewModels to synchronize image zoom.
            ViewRect = viewRect;
        }
    }

    public class ProfileDrawnEventArgs : EventArgs
    {
        public int StartX { get; set; }
        public int StartY { get; set; }
        public int EndX { get; set; }
        public int EndY { get; set; }
    }

    public class CrossProfileDrawnEventArgs : EventArgs
    {
        public int Horizontal { get; set; }
        public int Vertical { get; set; }
    }
}

