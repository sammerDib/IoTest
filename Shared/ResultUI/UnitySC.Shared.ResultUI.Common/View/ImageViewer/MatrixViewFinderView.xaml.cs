using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

using UnitySC.Shared.ResultUI.Common.ViewModel.ImageViewer;

namespace UnitySC.Shared.ResultUI.Common.View.ImageViewer
{
    public partial class MatrixViewFinderView
    {
        public enum DragDropManipulation
        {
            None,
            BorderLeft,
            BorderTop,
            BorderRight,
            BorderBottom,
            Move,
            Zoom
        }

        #region Fields

        private DragDropManipulation _currentManipulation = DragDropManipulation.None;

        #endregion

        private MatrixViewFinderVM ViewModel => DataContext as MatrixViewFinderVM;

        public MatrixViewFinderView()
        {
            InitializeComponent();

            // Link Profile Points with ViewModel Properties
            SetBinding(StartPointXProperty, new Binding(nameof(ViewModel.PercentStartX)));
            SetBinding(StartPointYProperty, new Binding(nameof(ViewModel.PercentStartY)));
            SetBinding(EndPointXProperty, new Binding(nameof(ViewModel.PercentEndX)));
            SetBinding(EndPointYProperty, new Binding(nameof(ViewModel.PercentEndY)));
            
            SetBinding(ManipulationInProgressProperty, new Binding(nameof(ViewModel.ManipulationInProgress)));
        }

        #region Properties

        public static readonly DependencyProperty StartPointXProperty = DependencyProperty.Register(
            nameof(StartPointX), typeof(double), typeof(MatrixViewFinderView), new FrameworkPropertyMetadata(default(double), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnAreaChangedPropertyChangedCallback));
        
        public double StartPointX
        {
            get { return (double)GetValue(StartPointXProperty); }
            set { SetValue(StartPointXProperty, value); }
        }

        public static readonly DependencyProperty StartPointYProperty = DependencyProperty.Register(
            nameof(StartPointY), typeof(double), typeof(MatrixViewFinderView), new FrameworkPropertyMetadata(default(double), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnAreaChangedPropertyChangedCallback));

        public double StartPointY
        {
            get { return (double)GetValue(StartPointYProperty); }
            set { SetValue(StartPointYProperty, value); }
        }

        public static readonly DependencyProperty EndPointXProperty = DependencyProperty.Register(
            nameof(EndPointX), typeof(double), typeof(MatrixViewFinderView), new FrameworkPropertyMetadata(default(double), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnAreaChangedPropertyChangedCallback));

        public double EndPointX
        {
            get { return (double)GetValue(EndPointXProperty); }
            set { SetValue(EndPointXProperty, value); }
        }

        public static readonly DependencyProperty EndPointYProperty = DependencyProperty.Register(
            nameof(EndPointY), typeof(double), typeof(MatrixViewFinderView), new FrameworkPropertyMetadata(default(double), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnAreaChangedPropertyChangedCallback));

        public double EndPointY
        {
            get { return (double)GetValue(EndPointYProperty); }
            set { SetValue(EndPointYProperty, value); }
        }

        private static void OnAreaChangedPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MatrixViewFinderView self)
            {
                self.UpdateZoomFactor();
            }
        }

        public static readonly DependencyProperty ManipulationInProgressProperty = DependencyProperty.Register(
            nameof(ManipulationInProgress), typeof(bool), typeof(MatrixViewFinderView), new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool ManipulationInProgress
        {
            get { return (bool)GetValue(ManipulationInProgressProperty); }
            set { SetValue(ManipulationInProgressProperty, value); }
        }

        public static readonly DependencyProperty ZoomFactorProperty = DependencyProperty.Register(
            nameof(ZoomFactor), typeof(double), typeof(MatrixViewFinderView), new PropertyMetadata(1.0));

        public double ZoomFactor
        {
            get { return (double)GetValue(ZoomFactorProperty); }
            set { SetValue(ZoomFactorProperty, value); }
        }

        #endregion

        private double _shiftX;
        private double _shiftY;

        private void OnLineMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;

            if (ReferenceEquals(sender, LeftBorder))
            {
                _currentManipulation = DragDropManipulation.BorderLeft;
                Mouse.OverrideCursor = LeftBorder.Cursor;
            }
            else if (ReferenceEquals(sender, TopBorder))
            {
                _currentManipulation = DragDropManipulation.BorderTop;
                Mouse.OverrideCursor = TopBorder.Cursor;
            }
            else if (ReferenceEquals(sender, RightBorder))
            {
                _currentManipulation = DragDropManipulation.BorderRight;
                Mouse.OverrideCursor = RightBorder.Cursor;
            }
            else if (ReferenceEquals(sender, BottomBorder))
            {
                _currentManipulation = DragDropManipulation.BorderBottom;
                Mouse.OverrideCursor = BottomBorder.Cursor;
            }
            else if (ReferenceEquals(sender, CenterRectangle))
            {
                _currentManipulation = DragDropManipulation.Move;
                Mouse.OverrideCursor = CenterRectangle.Cursor;
                var initialMovePoint = e.GetPosition(ViewFinderCanvas);

                int initialPointX = (int)(initialMovePoint.X + 0.5);
                int initialPointY = (int)(initialMovePoint.Y + 0.5);

                double initialPercentPointX = initialPointX / ViewFinderCanvas.ActualWidth;
                double initialPercentPointY = initialPointY / ViewFinderCanvas.ActualHeight;

                _shiftX = initialPercentPointX - StartPointX;
                _shiftY = initialPercentPointY - StartPointY;
            }
            else return;

            ManipulationInProgress = true;
            e.Handled = true;
            ViewFinderCanvas.CaptureMouse();
        }

        private void OnCanvasMouseMove(object sender, MouseEventArgs e)
        {
            if (_currentManipulation == DragDropManipulation.None) return;

            var currentPoint = e.GetPosition(ViewFinderCanvas);

            int currentPointX = (int)(currentPoint.X + 0.5);
            int currentPointY = (int)(currentPoint.Y + 0.5);

            double percentPointX = currentPointX / ViewFinderCanvas.ActualWidth;
            double percentPointY = currentPointY / ViewFinderCanvas.ActualHeight;

            if (percentPointX < 0) percentPointX = 0;
            if (percentPointX > 1) percentPointX = 1;
            if (percentPointY < 0) percentPointY = 0;
            if (percentPointY > 1) percentPointY = 1;

            switch (_currentManipulation)
            {
                case DragDropManipulation.None:
                    break;
                case DragDropManipulation.BorderLeft:
                    StartPointX = percentPointX;
                    break;
                case DragDropManipulation.BorderTop:
                    StartPointY = percentPointY;
                    break;
                case DragDropManipulation.BorderRight:
                    EndPointX = percentPointX;
                    break;
                case DragDropManipulation.BorderBottom:
                    EndPointY = percentPointY;
                    break;
                case DragDropManipulation.Move:
                    {
                        // Applying the shift (initial click point - TopLeft point)
                        percentPointX -= _shiftX;
                        percentPointY -= _shiftY;

                        double areaX = EndPointX - StartPointX;
                        double areaY = EndPointY - StartPointY;

                        if (percentPointX < 0) percentPointX = 0;
                        if (percentPointY < 0) percentPointY = 0;
                        if (percentPointX + areaX > 1) percentPointX = 1 - areaX;
                        if (percentPointY + areaY > 1) percentPointY = 1 - areaY;

                        StartPointX = percentPointX;
                        StartPointY = percentPointY;

                        EndPointX = percentPointX + areaX;
                        EndPointY = percentPointY + areaY;
                    }
                    break;
            }
        }

        private void OnCanvasMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_currentManipulation == DragDropManipulation.None) return;

            ManipulationInProgress = false;
            Mouse.OverrideCursor = null;
            ViewFinderCanvas.ReleaseMouseCapture();
            _currentManipulation = DragDropManipulation.None;
        }

        private DispatcherTimer _dispatcherTimer;

        private void OnCanvasMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (_currentManipulation != DragDropManipulation.None && _currentManipulation != DragDropManipulation.Zoom) return;

            _currentManipulation = DragDropManipulation.Zoom;
            ManipulationInProgress = true;

            _dispatcherTimer?.Stop();

            if (_dispatcherTimer == null)
            {
                _dispatcherTimer = new DispatcherTimer(TimeSpan.FromSeconds(1), DispatcherPriority.ApplicationIdle, MouseWhellTimeCallback, Dispatcher);
            }
            else
            {
                _dispatcherTimer.Start();
            }
            
            double deltaRate = 0.0002 * e.Delta;
            double ratio = (EndPointX - StartPointX) / (EndPointY - StartPointY);

            double newStartX = StartPointX + deltaRate;
            double newStartY = StartPointY + deltaRate / ratio;
            double newEndX = EndPointX - deltaRate;
            double newEndY = EndPointY - deltaRate / ratio;

            if (newStartX < 0) newStartX = 0;
            if (newStartX > 1) newStartX = 1;
            if (newStartY < 0) newStartY = 0;
            if (newStartY > 1) newStartY = 1;
            if (newEndX < 0) newEndX = 0;
            if (newEndX > 1) newEndX = 1;
            if (newEndY < 0) newEndY = 0;
            if (newEndY > 1) newEndY = 1;

            if (newStartX > newEndX || newStartY > newEndY) return;

            StartPointX = newStartX;
            StartPointY = newStartY;
            EndPointX = newEndX;
            EndPointY = newEndY;
        }

        private void UpdateZoomFactor()
        {
            double width = EndPointX - StartPointX;
            double height = EndPointY - StartPointY;
            ZoomFactor = 1 / Math.Max(width, height);
        }

        private void MouseWhellTimeCallback(object sender, EventArgs e)
        {
            if (_currentManipulation == DragDropManipulation.Zoom && ManipulationInProgress)
            {
                ManipulationInProgress = false;
                _currentManipulation = DragDropManipulation.None;
            }
            _dispatcherTimer.Stop();
        }
    }
}
