using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

using UnitySC.Shared.ResultUI.Common.ViewModel.ImageViewer;

using Xceed.Wpf.Toolkit.Core.Input;
using Xceed.Wpf.Toolkit.Zoombox;

namespace UnitySC.Shared.ResultUI.Common.View.ImageViewer
{
    public enum ImageViewerTool
    {
        Move,
        LineProfile,
        CrossProfile
    }

    /// <summary>
    /// Interaction logic for ImageViewerView.xaml
    /// </summary>
    public partial class ImageViewerView
    {
        public enum DragDropManipulation
        {
            None,
            StartPoint,
            EndPoint,
            CrossHorizontal,
            CrossVertical,
            DrawProfile
        }

        #region Fields

        private DragDropManipulation _currentManipulation = DragDropManipulation.None;

        #endregion

        private ImageViewerViewModel ViewModel => DataContext as ImageViewerViewModel;

        public ImageViewerView()
        {
            InitializeComponent();

            // Link Profile Points with ViewModel Properties
            SetBinding(StartPointXProperty, new Binding(nameof(ViewModel.StartPointX)));
            SetBinding(StartPointYProperty, new Binding(nameof(ViewModel.StartPointY)));
            SetBinding(EndPointXProperty, new Binding(nameof(ViewModel.EndPointX)));
            SetBinding(EndPointYProperty, new Binding(nameof(ViewModel.EndPointY)));
            SetBinding(MarkerXProperty, new Binding(nameof(ViewModel.MarkerX)));
            SetBinding(MarkerYProperty, new Binding(nameof(ViewModel.MarkerY)));

            SetBinding(CrossProfileXProperty, new Binding(nameof(ViewModel.CrossProfileX)));
            SetBinding(CrossProfileYProperty, new Binding(nameof(ViewModel.CrossProfileY)));
            SetBinding(CrossProfileMarkerXProperty, new Binding(nameof(ViewModel.HorizontalMarkerX)));
            SetBinding(CrossProfileMarkerYProperty, new Binding(nameof(ViewModel.VerticalMarkerY)));

            SetBinding(CurrentToolProperty, new Binding(nameof(ViewModel.CurrentTool)));

            SetBinding(ViewRectProperty, new Binding
            {
                Source = Zoombox, 
                Path = new PropertyPath(Zoombox.ViewportProperty),
                Mode = BindingMode.OneWay
            });

            SetBinding(InternalViewRectProperty, new Binding(nameof(ViewModel.ViewRect)));

            CurrentToolChangedCallback(this, new DependencyPropertyChangedEventArgs());
        }

        #region Handlers

        private void OnZoomboxSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateMinScale();
        }

        private void OnImageSizeChanged(object sender, SizeChangedEventArgs e)
        {
            InternalImageSizeChanged(e.NewSize);
        }

        #endregion

        #region Dependency Properties

        #region Coordinates

        public static readonly DependencyProperty StartPointXProperty = DependencyProperty.Register(
            nameof(StartPointX), typeof(int?), typeof(ImageViewerView), new FrameworkPropertyMetadata(default(int?), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnStartPointChangedCallback));

        public int? StartPointX
        {
            get { return (int?)GetValue(StartPointXProperty); }
            set { SetValue(StartPointXProperty, value); }
        }

        public static readonly DependencyProperty StartPointYProperty = DependencyProperty.Register(
            nameof(StartPointY), typeof(int?), typeof(ImageViewerView), new FrameworkPropertyMetadata(default(int?), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnStartPointChangedCallback));

        public int? StartPointY
        {
            get { return (int?)GetValue(StartPointYProperty); }
            set { SetValue(StartPointYProperty, value); }
        }

        private static void OnStartPointChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageViewerView self)
            {
                self.UpdateStartPointVisibility();
            }
        }

        public static readonly DependencyProperty EndPointXProperty = DependencyProperty.Register(
            nameof(EndPointX), typeof(int?), typeof(ImageViewerView), new FrameworkPropertyMetadata(default(int?), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnEndPointChangedCallback));

        public int? EndPointX
        {
            get { return (int?)GetValue(EndPointXProperty); }
            set { SetValue(EndPointXProperty, value); }
        }

        public static readonly DependencyProperty EndPointYProperty = DependencyProperty.Register(
            nameof(EndPointY), typeof(int?), typeof(ImageViewerView), new FrameworkPropertyMetadata(default(int?), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnEndPointChangedCallback));

        public int? EndPointY
        {
            get { return (int?)GetValue(EndPointYProperty); }
            set { SetValue(EndPointYProperty, value); }
        }

        private static void OnEndPointChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageViewerView self)
            {
                self.UpdateEndPointVisibility();
            }
        }

        public static readonly DependencyProperty MarkerXProperty = DependencyProperty.Register(
            nameof(MarkerX), typeof(double), typeof(ImageViewerView), new PropertyMetadata(default(double)));

        public double MarkerX
        {
            get { return (double)GetValue(MarkerXProperty); }
            set { SetValue(MarkerXProperty, value); }
        }

        public static readonly DependencyProperty MarkerYProperty = DependencyProperty.Register(
            nameof(MarkerY), typeof(double), typeof(ImageViewerView), new PropertyMetadata(default(double)));

        public double MarkerY
        {
            get { return (double)GetValue(MarkerYProperty); }
            set { SetValue(MarkerYProperty, value); }
        }

        public static readonly DependencyProperty CrossProfileXProperty = DependencyProperty.Register(
            nameof(CrossProfileX), typeof(int?), typeof(ImageViewerView), new FrameworkPropertyMetadata(default(int?), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnCrossPointChangedCallback));

        public int? CrossProfileX
        {
            get { return (int?)GetValue(CrossProfileXProperty); }
            set { SetValue(CrossProfileXProperty, value); }
        }

        public static readonly DependencyProperty CrossProfileYProperty = DependencyProperty.Register(
            nameof(CrossProfileY), typeof(int?), typeof(ImageViewerView), new FrameworkPropertyMetadata(default(int?), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnCrossPointChangedCallback));

        public int? CrossProfileY
        {
            get { return (int?)GetValue(CrossProfileYProperty); }
            set { SetValue(CrossProfileYProperty, value); }
        }

        private static void OnCrossPointChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageViewerView self)
            {
                self.UpdateCrossLinesVisibility();
            }
        }

        public static readonly DependencyProperty CrossProfileMarkerXProperty = DependencyProperty.Register(
            nameof(CrossProfileMarkerX), typeof(double), typeof(ImageViewerView), new PropertyMetadata(default(double)));

        public double CrossProfileMarkerX
        {
            get { return (double)GetValue(CrossProfileMarkerXProperty); }
            set { SetValue(CrossProfileMarkerXProperty, value); }
        }

        public static readonly DependencyProperty CrossProfileMarkerYProperty = DependencyProperty.Register(
            nameof(CrossProfileMarkerY), typeof(double), typeof(ImageViewerView), new PropertyMetadata(default(double)));

        public double CrossProfileMarkerY
        {
            get { return (double)GetValue(CrossProfileMarkerYProperty); }
            set { SetValue(CrossProfileMarkerYProperty, value); }
        }

        #endregion

        #region Scales

        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register(
            nameof(Scale), typeof(double), typeof(ImageViewerView), new PropertyMetadata(default(double), ScaleChangedCallback));

        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        private static void ScaleChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageViewerView self)
            {
                self.ProfilePointSize = 20 * (1 / self.Scale);
                self.TranslateTransform = -(self.ProfilePointSize / 2);

                self.MarkerSize = 10 * (1 / self.Scale);
                self.MarkerTranslateTransform = -(self.MarkerSize / 2);

                self.ProfileLineThickness = 2 * (1 / self.Scale);
                self.NegativeZoomTransform = 1 / self.Scale;

                if (!self.IsInitialized) return;

                // Add a margin to cross profile lines to increase the drag area.
                double manipulationMargin = 10 * (1 / self.Scale);
                self.CrossProfileHorizontalBorder.Margin = new Thickness(-0.5, -manipulationMargin, 0, 0);
                self.CrossProfileHorizontalLine.Margin = new Thickness(0, manipulationMargin, 0, manipulationMargin);
                self.CrossProfileVerticalBorder.Margin = new Thickness(-manipulationMargin, -0.5, 0, 0);
                self.CrossProfileVerticalLine.Margin = new Thickness(manipulationMargin, 0, manipulationMargin, 0);
            }
        }

        public static readonly DependencyProperty ProfilePointSizeProperty = DependencyProperty.Register(
            nameof(ProfilePointSize), typeof(double), typeof(ImageViewerView), new PropertyMetadata(default(double)));

        public double ProfilePointSize
        {
            get { return (double)GetValue(ProfilePointSizeProperty); }
            set { SetValue(ProfilePointSizeProperty, value); }
        }

        public static readonly DependencyProperty TranslateTransformProperty = DependencyProperty.Register(
            nameof(TranslateTransform), typeof(double), typeof(ImageViewerView), new PropertyMetadata(default(double)));

        public double TranslateTransform
        {
            get { return (double)GetValue(TranslateTransformProperty); }
            set { SetValue(TranslateTransformProperty, value); }
        }

        public static readonly DependencyProperty MarkerSizeProperty = DependencyProperty.Register(
            nameof(MarkerSize), typeof(double), typeof(ImageViewerView), new PropertyMetadata(default(double)));

        public double MarkerSize
        {
            get { return (double)GetValue(MarkerSizeProperty); }
            set { SetValue(MarkerSizeProperty, value); }
        }

        public static readonly DependencyProperty MarkerTranslateTransformProperty = DependencyProperty.Register(
            nameof(MarkerTranslateTransform), typeof(double), typeof(ImageViewerView), new PropertyMetadata(default(double)));

        public double MarkerTranslateTransform
        {
            get { return (double)GetValue(MarkerTranslateTransformProperty); }
            set { SetValue(MarkerTranslateTransformProperty, value); }
        }

        public static readonly DependencyProperty NegativeZoomTransformProperty = DependencyProperty.Register(
            nameof(NegativeZoomTransform), typeof(double), typeof(ImageViewerView), new PropertyMetadata(default(double)));

        public double NegativeZoomTransform
        {
            get { return (double)GetValue(NegativeZoomTransformProperty); }
            set { SetValue(NegativeZoomTransformProperty, value); }
        }

        public static readonly DependencyProperty ProfileLineThicknessProperty = DependencyProperty.Register(
            nameof(ProfileLineThickness), typeof(double), typeof(ImageViewerView), new PropertyMetadata(default(double)));

        public double ProfileLineThickness
        {
            get { return (double)GetValue(ProfileLineThicknessProperty); }
            set { SetValue(ProfileLineThicknessProperty, value); }
        }

        #endregion

        // This field stores the last profile tool to keep in memory which profile to view when using the move tool
        private ImageViewerTool _lastProfileTool = ImageViewerTool.LineProfile;

        public static readonly DependencyProperty CurrentToolProperty = DependencyProperty.Register(
            nameof(CurrentTool), typeof(ImageViewerTool), typeof(ImageViewerView), new PropertyMetadata(default(ImageViewerTool), CurrentToolChangedCallback));
        
        public ImageViewerTool CurrentTool
        {
            get { return (ImageViewerTool)GetValue(CurrentToolProperty); }
            set { SetValue(CurrentToolProperty, value); }
        }

        private static void CurrentToolChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ImageViewerView self)
            {
                self.SetupProfileTool();
            }
        }

        private void SetupProfileTool()
        {
            if (CurrentTool == ImageViewerTool.Move)
            {
                Zoombox.DragModifiers = new KeyModifierCollection { KeyModifier.None };
            }
            else
            {
                Zoombox.DragModifiers = new KeyModifierCollection { KeyModifier.Ctrl };
                _lastProfileTool = CurrentTool;
            }

            UpdateStartPointVisibility();
            UpdateEndPointVisibility();
            UpdateCrossLinesVisibility();
        }

        private void UpdateStartPointVisibility()
        {
            ProfileStartPoint.Visibility = StartPointX.HasValue && StartPointY.HasValue && _lastProfileTool == ImageViewerTool.LineProfile ? Visibility.Visible : Visibility.Collapsed;
            UpdateProfileLineVisibility();
        }

        private void UpdateEndPointVisibility()
        {
            ProfileEndPoint.Visibility = EndPointX.HasValue && EndPointY.HasValue && _lastProfileTool == ImageViewerTool.LineProfile ? Visibility.Visible : Visibility.Collapsed;
            UpdateProfileLineVisibility();
        }

        private void UpdateProfileLineVisibility()
        {
            bool isVisible = StartPointX.HasValue && StartPointY.HasValue && EndPointX.HasValue && EndPointY.HasValue;
            var visibility = isVisible && _lastProfileTool == ImageViewerTool.LineProfile ? Visibility.Visible : Visibility.Collapsed;
            ProfileLine.Visibility = visibility;
            EllipseMarker.Visibility = visibility;
        }

        private void UpdateCrossLinesVisibility()
        {
            CrossProfileHorizontalLine.Visibility = CrossProfileY.HasValue && _lastProfileTool == ImageViewerTool.CrossProfile ? Visibility.Visible : Visibility.Collapsed;
            HorizontalEllipseMarker.Visibility = CrossProfileHorizontalLine.Visibility;

            CrossProfileVerticalLine.Visibility = CrossProfileX.HasValue && _lastProfileTool == ImageViewerTool.CrossProfile ? Visibility.Visible : Visibility.Collapsed;
            VerticalEllipseMarker.Visibility = CrossProfileVerticalLine.Visibility;
        }

        #endregion

        private void UpdateMinScale()
        {
            if (!IsLoaded) return;
            double imageWidth = Image.ActualWidth;
            double imageHeight = Image.ActualHeight;

            // Remove the size of a scrollbar to avoid truncating the image
            const int scrollBarSize = 17;

            if (imageWidth != 0 && imageHeight != 0)
            {
                Zoombox.MinScale = Math.Min((Zoombox.ActualWidth - scrollBarSize) / imageWidth, (Zoombox.ActualHeight - scrollBarSize) / imageHeight);
            }
        }

        // Stores the size of the image for which the last SetInitialZoom was performed.
        private double _lastFitWidth;
        private double _lastFitHeight;

        private void InternalImageSizeChanged(Size newSize)
        {
            // [TLa] Ignore the application of the initial zoom in case the new image has not yet been displayed
            // because we want to keep the current location and zoom.
            if (newSize.Height == 0.0 && newSize.Width == 0.0) return;

            // // If the new image is not the same size as the image displayed during the previous SetInitialZoom,
            // reset the zoom because that means that the data is no longer the same.
            if (Math.Abs(newSize.Height - _lastFitHeight) > 0.01 || Math.Abs(newSize.Width - _lastFitWidth) > 0.01)
            {
                SetInitialZoom();
            }
        }

        private void SetInitialZoom()
        {
            if (!IsLoaded) return;

            Zoombox.MinScale = 0;

            // If the InternalViewRect property coming from the ViewModel is not empty, apply the zoom to the Zoombox,
            // otherwise apply the image with the largest possible size.
            if (!InternalViewRect.IsEmpty)
            {
                _preventViewRectChangedEvent = true;
                Zoombox.ZoomTo(InternalViewRect);
            }
            else
            {
                Zoombox.FitToBounds();
            }

            UpdateMinScale();

            _lastFitHeight = Image.ActualHeight;
            _lastFitWidth = Image.ActualWidth;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            SetInitialZoom();
        }
        
        #region Profile Canvas Handlers

        private void OnProfileCanvasMouseDown(object sender, MouseButtonEventArgs e)
        {
            var startPoint = e.GetPosition(ProfileCanvas);
            int startPointX = (int)(startPoint.X + 0.5);
            int startPointY = (int)(startPoint.Y + 0.5);

            if (ViewModel != null && ViewModel.OnMouseDown(startPointX, startPointY))
            {
                // Gesture is on ViewModel
                return;
            }

            if (_currentManipulation != DragDropManipulation.None) return;

            if (CurrentTool == ImageViewerTool.Move) return;

            if (!VerifyModifiers()) return;
            if (e.ChangedButton != MouseButton.Left) return;

            _currentManipulation = DragDropManipulation.DrawProfile;
            ProfileCanvas.CaptureMouse();

            if (CurrentTool == ImageViewerTool.LineProfile)
            {
                StartPointX = startPointX;
                StartPointY = startPointY;
                EndPointX = null;
                EndPointY = null;
            }
            else
            {
                CrossProfileX = startPointX;
                CrossProfileY = startPointY;
            }
        }

        private void OnProfileCanvasMouseLeave(object sender, MouseEventArgs e)
        {
            var viewModel = ViewModel;
            if (viewModel == null) return;

            viewModel.MouseOverX = null;
            viewModel.MouseOverY = null;
        }

        private void OnProfileCanvasMouseMove(object sender, MouseEventArgs e)
        {
            var viewModel = ViewModel;
            if (viewModel == null) return;

            var currentPoint = e.GetPosition(ProfileCanvas);

            int currentPointX = (int)(currentPoint.X + 0.5);
            int currentPointY = (int)(currentPoint.Y + 0.5);

            // No need to create a dependencyProperty for this because it corresponds to a OneWayToSource binding (View to ViewModel)
            if (currentPoint.X < 0 ||
                currentPoint.Y < 0 ||
                currentPoint.X > ProfileCanvas.ActualWidth ||
                currentPoint.Y > ProfileCanvas.ActualHeight)
            {
                viewModel.MouseOverX = null;
                viewModel.MouseOverY = null;
            }
            else
            {
                viewModel.MouseOverX = currentPointX;
                viewModel.MouseOverY = currentPointY;
            }
            
            if (CurrentTool == ImageViewerTool.LineProfile)
            {
                switch (_currentManipulation)
                {
                    case DragDropManipulation.StartPoint:
                        StartPointX = currentPointX;
                        StartPointY = currentPointY;
                        break;
                    case DragDropManipulation.DrawProfile:
                    case DragDropManipulation.EndPoint:
                        EndPointX = currentPointX;
                        EndPointY = currentPointY;
                        break;
                }
            }
            else if (CurrentTool == ImageViewerTool.CrossProfile)
            {
                switch (_currentManipulation)
                {
                    case DragDropManipulation.DrawProfile:
                        CrossProfileX = currentPointX;
                        CrossProfileY = currentPointY;
                        break;
                    case DragDropManipulation.CrossHorizontal:
                        CrossProfileY = currentPointY;
                        break;
                    case DragDropManipulation.CrossVertical:
                        CrossProfileX = currentPointX;
                        break;
                }
            }
        }

        public event EventHandler ProfileDrawn;

        private void OnProfileCanvasMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_currentManipulation != DragDropManipulation.DrawProfile) return;

            ProfileCanvas.ReleaseMouseCapture();
            _currentManipulation = DragDropManipulation.None;

            ProfileDrawn?.Invoke(this, EventArgs.Empty);
        }


        #endregion

        #region Profile elements manipulation

        private void ProfileStartPointDrag(object sender, MouseButtonEventArgs e)
        {
            if (!VerifyModifiers()) return;
            e.Handled = true;
            _currentManipulation = DragDropManipulation.StartPoint;
            ProfileStartPoint.CaptureMouse();
        }

        private void ProfileStartPointDrop(object sender, MouseButtonEventArgs e)
        {
            if (_currentManipulation != DragDropManipulation.StartPoint) return;

            _currentManipulation = DragDropManipulation.None;
            ProfileStartPoint.ReleaseMouseCapture();
        }

        private void ProfileEndPointDrag(object sender, MouseButtonEventArgs e)
        {
            if (!VerifyModifiers()) return;
            e.Handled = true;
            _currentManipulation = DragDropManipulation.EndPoint;
            ProfileEndPoint.CaptureMouse();
        }

        private void ProfileEndPointDrop(object sender, MouseButtonEventArgs e)
        {
            if (_currentManipulation != DragDropManipulation.EndPoint) return;

            _currentManipulation = DragDropManipulation.None;
            ProfileEndPoint.ReleaseMouseCapture();
        }

        private void HorizontalCrossProfileDrag(object sender, MouseButtonEventArgs e)
        {
            if (!VerifyModifiers()) return;
            e.Handled = true;
            _currentManipulation = DragDropManipulation.CrossHorizontal;
            CrossProfileHorizontalLine.CaptureMouse();
        }

        private void HorizontalCrossProfileDrop(object sender, MouseButtonEventArgs e)
        {
            if (_currentManipulation != DragDropManipulation.CrossHorizontal) return;

            _currentManipulation = DragDropManipulation.None;
            CrossProfileHorizontalLine.ReleaseMouseCapture();
        }

        private void VerticalCrossProfileDrag(object sender, MouseButtonEventArgs e)
        {
            if (!VerifyModifiers()) return;
            e.Handled = true;
            _currentManipulation = DragDropManipulation.CrossVertical;
            CrossProfileVerticalLine.CaptureMouse();
        }

        private void VerticalCrossProfileDrop(object sender, MouseButtonEventArgs e)
        {
            if (_currentManipulation != DragDropManipulation.CrossVertical) return;

            _currentManipulation = DragDropManipulation.None;
            CrossProfileVerticalLine.ReleaseMouseCapture();
        }

        #endregion

        private bool VerifyModifiers()
        {
            if (Keyboard.IsKeyDown(Key.LeftShift) ||
                Keyboard.IsKeyDown(Key.RightShift) ||
                Keyboard.IsKeyDown(Key.LeftCtrl) ||
                Keyboard.IsKeyDown(Key.RightCtrl))
            {
                return false;
            }

            return true;
        }

        #region Zoom rect synchronization

        /// <summary>
        /// Property synchronized with <see cref="Zoombox"/>.ViewportProperty.
        /// </summary>
        public static readonly DependencyProperty ViewRectProperty = DependencyProperty.Register(
            nameof(ViewRect), typeof(Rect), typeof(ImageViewerView), new PropertyMetadata(default(Rect), PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // When the zoombox zoom changes, notifies the ViewModel.
            if (d is ImageViewerView self)
            {
                // Prevent raise view rect changed if initial ViewModel value is not loaded.
                if (!self._initialViewRectLoaded)
                {
                    return;
                }

                // Prevents notifying the ViewModel when the zoom change comes from it.
                if (self._preventViewRectChangedEvent)
                {
                    self._preventViewRectChangedEvent = false;
                    return;
                }
                
                self.ViewModel?.RaiseViewRectChanged(self.ViewRect);
            }
        }

        public Rect ViewRect
        {
            get { return (Rect)GetValue(ViewRectProperty); }
            set { SetValue(ViewRectProperty, value); }
        }

        /// <summary>
        /// Dependency property used to change Zoombox zoom from a ViewModel property.
        /// </summary>
        public static readonly DependencyProperty InternalViewRectProperty = DependencyProperty.Register(
            nameof(InternalViewRect), typeof(Rect), typeof(ImageViewerView), new PropertyMetadata(default(Rect), InternalViewRectPropertyChangedCallback));

        private bool _preventViewRectChangedEvent;
        private bool _initialViewRectLoaded;

        private static void InternalViewRectPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // When the ViewModel property changes, applies the new zoom area to the Zoombox.
            if (d is ImageViewerView self)
            {
                self._preventViewRectChangedEvent = true;
                self._initialViewRectLoaded = true;
                self.Zoombox.ZoomTo(self.InternalViewRect);
            }
        }

        public Rect InternalViewRect
        {
            get { return (Rect)GetValue(InternalViewRectProperty); }
            set { SetValue(InternalViewRectProperty, value); }
        }

        #endregion
    }
}

