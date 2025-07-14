using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UnitySC.Shared.DieCutUpUI.Common.Controls
{
    /// <summary>
    ///     Interaction logic for DieCutUpZoomableImage.xaml
    /// </summary>
    public partial class DieCutUpZoomableImage : UserControl
    {
        private const byte ReticleAlpha = 85;

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(BitmapSource), typeof(DieCutUpZoomableImage),
                new PropertyMetadata(null, ImageSourceChanged));

        public static readonly DependencyProperty ImageWidthProperty =
            DependencyProperty.Register("ImageWidth", typeof(double), typeof(DieCutUpZoomableImage),
                new PropertyMetadata(0.0));

        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.Register("ImageHeight", typeof(double), typeof(DieCutUpZoomableImage),
                new PropertyMetadata(0.0));

        public static readonly DependencyProperty SelectedGridProperty =
            DependencyProperty.Register("SelectedGrid", typeof(GridVM), typeof(DieCutUpZoomableImage),
                new PropertyMetadata(new GridVM("Default Grid", Color.FromRgb(255, 0, 0)), SelectedGridChanged));

        public static readonly DependencyProperty GridsProperty =
            DependencyProperty.Register("Grids", typeof(ObservableCollection<GridVM>), typeof(DieCutUpZoomableImage),
                new PropertyMetadata(new ObservableCollection<GridVM>(), GridsChanged));

        private Point _imagePosition;

        private Point? _refReticleSelectStart;

        private double _scale;

        private CellIndex _selectStart;

        public DieCutUpZoomableImage()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                if (SelectedGrid != null)
                {
                    SelectedGrid.PropertyChanged += SelectedGridPropertyChanged;
                }

                if (Grids != null)
                {
                    foreach (var grid in Grids)
                    {
                        grid.PropertyChanged += GridPropertyChanged;
                    }
                }
            };
            customZoomableImage.PreviewMouseLeftButtonDown += Zoombox_MouseLeftButtonDown;
            customZoomableImage.MouseLeftButtonUp += Zoombox_MouseLeftButtonUp;
            customZoomableImage.MouseMove += Zoombox_MouseMove;
        }

        public double Scale
        {
            get => _scale;
            set
            {
                if (!_scale.Equals(value))
                {
                    _scale = value;
                    highlightedReticlesCanvas?.Children?.Clear();
                    DrawGrid();
                    DrawSelectedReticles();
                }
            }
        }

        public Point ImagePosition
        {
            get => _imagePosition;
            set
            {
                if (_imagePosition != value)
                {
                    _imagePosition = value;
                    highlightedReticlesCanvas.Children.Clear();
                    DrawGrid();
                    DrawSelectedReticles();
                }
            }
        }


        //=================================================================
        // Dependency properties
        //=================================================================
        public BitmapSource ImageSource
        {
            get => (BitmapSource)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        public double ImageWidth
        {
            get => (double)GetValue(ImageWidthProperty);
            set => SetValue(ImageWidthProperty, value);
        }

        public double ImageHeight
        {
            get => (double)GetValue(ImageHeightProperty);
            set => SetValue(ImageHeightProperty, value);
        }

        public GridVM SelectedGrid
        {
            get => (GridVM)GetValue(SelectedGridProperty);
            set => SetValue(SelectedGridProperty, value);
        }

        public ObservableCollection<GridVM> Grids
        {
            get => (ObservableCollection<GridVM>)GetValue(GridsProperty);
            set => SetValue(GridsProperty, value);
        }

        private void SelectedGridPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GridVM.OffsetX) ||
                e.PropertyName == nameof(GridVM.OffsetY) ||
                e.PropertyName == nameof(GridVM.BoxWidth) ||
                e.PropertyName == nameof(GridVM.BoxHeight))
            {
                DrawGrid();
                DrawSelectedReticles();
                highlightedReticlesCanvas.Children.Clear();
            }

            if (e.PropertyName == nameof(GridVM.IsSelectingReferenceReticle) &&
                SelectedGrid.IsSelectingReferenceReticle)
            {
                SelectedGrid.OffsetX = 0;
                SelectedGrid.OffsetY = 0;
                SelectedGrid.BoxWidth = 0;
                SelectedGrid.BoxHeight = 0;
                SelectedGrid.ClearSelectedCells();
            }

            if (e.PropertyName == nameof(GridVM.IsReticleVisible))
            {
                DrawSelectedReticles();
            }
        }

        private void DrawGrid()
        {
            if (customZoomableImage.ImageSource == null)
            {
                return;
            }

            if (gridCanvas == null)
            {
                return;
            }

            gridCanvas.Children.Clear();

            if (SelectedGrid == null) { return; }

            DrawGridVerticalLines();
            DrawGridHorizontalLines();
        }

        private Brush GridColor(byte a, GridVM grid)
        {
            return new SolidColorBrush(Color.FromArgb(a, grid.Color.R, grid.Color.G, grid.Color.B));
        }

        private void DrawGridVerticalLines()
        {
            double xStartInUnscaledImg = Math.Max(-ImagePosition.X / Scale, 0);
            double xEndInUnscaledImg =
                Math.Min(xStartInUnscaledImg + (ActualWidth - GetVerticalScrollBarSize()) / Scale, ImageWidth);

            foreach (double lineX in SelectedGrid.VerticalLinesBetween(xStartInUnscaledImg, xEndInUnscaledImg))
            {
                double xLineOnCanvas = GetScaled(lineX) + ImagePosition.X;
                var lineOnCanvas = new Line
                {
                    X1 = xLineOnCanvas,
                    Y1 = Math.Max(0, ImagePosition.Y),
                    X2 = xLineOnCanvas,
                    Y2 = Math.Min(
                        ImagePosition.Y + GetScaled(customZoomableImage.ImageHeight),
                        ActualHeight - GetHorizontalScrollBarSize()),
                    Stroke = GridColor(255, SelectedGrid),
                    StrokeThickness = 1,
                    IsHitTestVisible = false
                };
                gridCanvas.Children.Add(lineOnCanvas);
            }
        }

        private void DrawGridHorizontalLines()
        {
            double yStartInUnscaledImg = Math.Max(-ImagePosition.Y / Scale, 0);
            double yEndInUnscaledImg =
                Math.Min(yStartInUnscaledImg + (ActualHeight - GetHorizontalScrollBarSize()) / Scale, ImageHeight);

            foreach (double lineY in SelectedGrid.HorizontalLinesBetween(yStartInUnscaledImg, yEndInUnscaledImg))
            {
                double yLineOnCanvas = GetScaled(lineY) + ImagePosition.Y;
                var lineOnCanvas = new Line
                {
                    X1 = Math.Max(0, ImagePosition.X),
                    Y1 = yLineOnCanvas,
                    X2 = Math.Min(
                        ImagePosition.X + GetScaled(customZoomableImage.ImageWidth),
                        ActualWidth - GetVerticalScrollBarSize()),
                    Y2 = yLineOnCanvas,
                    Stroke = GridColor(255, SelectedGrid),
                    StrokeThickness = 1,
                    IsHitTestVisible = false
                };
                gridCanvas.Children.Add(lineOnCanvas);
            }
        }

        private Rect ScaleAndClampRect(Rect rect)
        {
            return ClampRectangleInsideView(new Rect
            {
                X = GetScaled(rect.X) + ImagePosition.X,
                Y = GetScaled(rect.Y) + ImagePosition.Y,
                Width = GetScaled(rect.Width),
                Height = GetScaled(rect.Height)
            });
        }

        private void DrawSelectedReticles()
        {
            if (selectedReticlesCanvas == null)
            {
                return;
            }

            if (SelectedGrid == null)
            {
                selectedReticlesCanvas.Children.Clear();
                return;
            }

            selectedReticlesCanvas.Children.Clear();

            foreach (var grid in Grids)
            {
                if (grid.IsReticleVisible)
                {
                    foreach (var cell in grid.SelectedCells())
                    {
                        var rect = ScaleAndClampRect(cell.GetRect());
                        if (rect.IsEmpty)
                        {
                            continue;
                        }

                        var rectangle = new Rectangle { Width = rect.Width, Height = rect.Height };

                        Canvas.SetLeft(rectangle, rect.X);
                        Canvas.SetTop(rectangle, rect.Y);
                        rectangle.Fill = GridColor(ReticleAlpha, grid);
                        rectangle.IsHitTestVisible = false;
                        selectedReticlesCanvas.Children.Add(rectangle);
                    }
                }
            }
        }


        private Point ScaleAndShiftPoint(Point p)
        {
            return new Point(GetScaled(p.X) + _imagePosition.X, GetScaled(p.Y) + _imagePosition.Y);
        }

        private Point UnscaleAndUnshiftPoint(Point p)
        {
            return (Point)((p - _imagePosition) / Scale);
        }

        private void Zoombox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var position = e.GetPosition(highlightedReticlesCanvas);
            //Cursor outside the image : we don't want to draw any highlighted reticles
            if (IsPositionOutsideImage(position))
            {
                return;
            }

            customZoomableImage.CaptureMouse();

            if (SelectedGrid.IsSelectingReferenceReticle)
            {
                StartSelectingReferenceReticle(position);
            }
            else
            {
                StartSelectingReticles(position);
            }
        }

        private void StartSelectingReferenceReticle(Point position)
        {
            _refReticleSelectStart = UnscaleAndUnshiftPoint(position);
        }

        private void StartSelectingReticles(Point position)
        {
            if (SelectedGrid.BoxWidth == 0 || SelectedGrid.BoxHeight == 0)
            {
                return;
            }

            _selectStart = SelectedGrid.CellAtPosition(UnscaleAndUnshiftPoint(position));
        }

        private void Zoombox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            customZoomableImage.ReleaseMouseCapture();
            var position = e.GetPosition(highlightedReticlesCanvas);

            if (IsPositionOutsideImage(position))
            {
                _selectStart = null;
                return;
            }

            if (SelectedGrid.IsSelectingReferenceReticle)
            {
                FinishSelectingReferenceReticle(position);
            }
            else
            {
                FinishSelectingReticles(position);
            }

            DrawSelectedReticles();
        }

        private void FinishSelectingReticles(Point position)
        {
            if (_selectStart == null)
            {
                return;
            }

            if (SelectedGrid.BoxWidth == 0 || SelectedGrid.BoxHeight == 0)
            {
                return;
            }

            var selectEnd = SelectedGrid.CellAtPosition(UnscaleAndUnshiftPoint(position));

            bool deleteSelection = (_selectStart ?? selectEnd).IsSelected();
            foreach (var cell in SelectedGrid.CellsInBetween(_selectStart ?? selectEnd, selectEnd))
            {
                if (deleteSelection)
                {
                    cell.UnSelect();
                }
                else
                {
                    cell.Select();
                }
            }

            _selectStart = null;
            highlightedReticlesCanvas.Children.Clear();
        }

        private void FinishSelectingReferenceReticle(Point position)
        {
            if (_refReticleSelectStart == null)
            {
                return;
            }


            var refReticleSelectEnd = UnscaleAndUnshiftPoint(position);

            double boxWidth = refReticleSelectEnd.X - _refReticleSelectStart.Value.X;
            double boxHeight = refReticleSelectEnd.Y - _refReticleSelectStart.Value.Y;
            if (boxHeight < -200)
            {
                int o = 0;
            }

            double gridOffsetX = Math.Min(_refReticleSelectStart.Value.X, _refReticleSelectStart.Value.X + boxWidth);
            double gridOffsetY = Math.Min(_refReticleSelectStart.Value.Y, _refReticleSelectStart.Value.Y + boxHeight);

            SelectedGrid.OffsetX = (int)Math.Floor(gridOffsetX);
            SelectedGrid.OffsetY = (int)Math.Floor(gridOffsetY);
            SelectedGrid.BoxWidth = (int)Math.Floor(Math.Abs(boxWidth));
            SelectedGrid.BoxHeight = (int)Math.Floor(Math.Abs(boxHeight));

            var refReticleSelectStartCell = SelectedGrid.CellAtPosition(new Point(gridOffsetX, gridOffsetY));

            refReticleSelectStartCell.Select();
            refReticleSelectStartCell.SetAsReference();

            _refReticleSelectStart = null;
            SelectedGrid.IsSelectingReferenceReticle = false;
        }

        private void Zoombox_MouseMove(object sender, MouseEventArgs e)
        {
            highlightedReticlesCanvas.Children.Clear();

            var mousePos = e.GetPosition(highlightedReticlesCanvas);
            //Cursor outside the image : we don't want to draw any highlighted reticles
            if (IsPositionOutsideImage(mousePos))
            {
                return;
            }

            if (SelectedGrid == null) { return; }

            if (SelectedGrid.IsSelectingReferenceReticle)
            {
                ContinueSelectingReferenceReticle(mousePos);
            }
            else
            {
                ContinueSelectingReticles(mousePos);
            }
        }

        private void ContinueSelectingReticles(Point mousePos)
        {
            if (SelectedGrid.BoxWidth == 0 || SelectedGrid.BoxHeight == 0)
            {
                return;
            }

            var selectEnd = SelectedGrid.CellAtPosition(UnscaleAndUnshiftPoint(mousePos));

            var localSelectStart = _selectStart ?? selectEnd;

            var color = localSelectStart.IsSelected()
                ? new SolidColorBrush(Color.FromArgb(51, 255, 0, 0))
                : GridColor(ReticleAlpha, SelectedGrid);

            var startRect = localSelectStart.GetRect();
            var endRect = selectEnd.GetRect();

            double bigRectX = Math.Min(startRect.X, endRect.X);
            double bigRectY = Math.Min(startRect.Y, endRect.Y);
            double bigRectWidth = Math.Abs(startRect.X - endRect.X) + endRect.Width;
            double bigRectHeight = Math.Abs(startRect.Y - endRect.Y) + endRect.Height;

            var bigRect = ScaleAndClampRect(new Rect
            {
                X = bigRectX, Y = bigRectY, Width = bigRectWidth, Height = bigRectHeight
            });

            if (bigRect.IsEmpty)
            {
                return;
            }

            var displayRect = new Rectangle();
            displayRect.Width = bigRect.Width;
            displayRect.Height = bigRect.Height;

            Canvas.SetLeft(displayRect, bigRect.X);
            Canvas.SetTop(displayRect, bigRect.Y);
            displayRect.Fill = color;
            displayRect.IsHitTestVisible = false;
            highlightedReticlesCanvas.Children.Add(displayRect);
        }

        private void ContinueSelectingReferenceReticle(Point mousePos)
        {
            if (_refReticleSelectStart == null)
            {
                return;
            }

            var rectStart = ScaleAndShiftPoint(_refReticleSelectStart.Value);
            var rectEnd = mousePos;
            double rectWidth = rectEnd.X - rectStart.X;
            double rectHeight = rectEnd.Y - rectStart.Y;

            double rectStartX = Math.Min(rectStart.X, rectStart.X + rectWidth);
            double rectStartY = Math.Min(rectStart.Y, rectStart.Y + rectHeight);

            var displayRect = new Rectangle { Width = Math.Abs(rectWidth), Height = Math.Abs(rectHeight) };
            Canvas.SetLeft(displayRect, rectStartX);
            Canvas.SetTop(displayRect, rectStartY);
            displayRect.Stroke = GridColor(ReticleAlpha, SelectedGrid);
            displayRect.StrokeThickness = 3;
            displayRect.IsHitTestVisible = false;
            highlightedReticlesCanvas.Children.Add(displayRect);
        }

        private static void SelectedGridChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) { return; }

            var view = (DieCutUpZoomableImage)d;
            if (e.OldValue is GridVM oldGrid)
            {
                oldGrid.PropertyChanged -= view.SelectedGridPropertyChanged;
            }

            if (e.NewValue is GridVM newGrid)
            {
                newGrid.PropertyChanged += view.SelectedGridPropertyChanged;
            }

            view.DrawGrid();
            view.DrawSelectedReticles();
            view.highlightedReticlesCanvas.Children.Clear();
        }

        private static void GridsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (DieCutUpZoomableImage)d;
            if (e.OldValue is INotifyCollectionChanged oldGrids)
            {
                oldGrids.CollectionChanged -= view.GridsCollectionChanged;
            }

            if (e.NewValue is INotifyCollectionChanged newGrids)
            {
                newGrids.CollectionChanged += view.GridsCollectionChanged;
            }
        }

        private void GridsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var oldGrids = e.OldItems?.OfType<GridVM>();
            var newGrids = e.NewItems?.OfType<GridVM>();

            if (oldGrids != null)
            {
                foreach (var oldGrid in oldGrids)
                {
                    oldGrid.PropertyChanged -= GridPropertyChanged;
                }
            }

            if (newGrids != null)
            {
                foreach (var newGrid in newGrids)
                {
                    newGrid.PropertyChanged += GridPropertyChanged;
                }
            }

            DrawGrid();
            DrawSelectedReticles();
            highlightedReticlesCanvas.Children.Clear();
        }

        private void GridPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GridVM.IsReticleVisible))
            {
                DrawSelectedReticles();
            }
        }

        private static void ImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (DieCutUpZoomableImage)d;
            if (view.ImageSource == null)
            {
                return;
            }

            view.ImageWidth = view.ImageSource.Width;
            view.ImageHeight = view.ImageSource.Height;
        }

        private double GetScaled(double value)
        {
            return Scale * value;
        }

        private double GetHorizontalScrollBarSize()
        {
            bool isHorizontalScrollBarVisible =
                customZoomableImage.scrollViewer.ComputedHorizontalScrollBarVisibility == Visibility.Visible;
            return isHorizontalScrollBarVisible ? SystemParameters.HorizontalScrollBarHeight : 0.0;
        }

        private double GetVerticalScrollBarSize()
        {
            bool isVerticalScrollBarVisible = customZoomableImage.scrollViewer.ComputedVerticalScrollBarVisibility ==
                                              Visibility.Visible;
            return isVerticalScrollBarVisible ? SystemParameters.VerticalScrollBarWidth : 0.0;
        }

        private Point ClampPointInsideView(Point point)
        {
            double clampedX = Math.Max(0.0, Math.Min(point.X, ActualWidth - GetVerticalScrollBarSize()));
            double clampedY = Math.Max(0.0, Math.Min(point.Y, ActualHeight - GetHorizontalScrollBarSize()));
            return new Point(clampedX, clampedY);
        }

        private Point ClampPointInsideImage(Point point)
        {
            double imageWidth = GetScaled(customZoomableImage.ImageWidth);
            double imageHeight = GetScaled(customZoomableImage.ImageHeight);
            double clampedX = Math.Max(ImagePosition.X, Math.Min(point.X, ImagePosition.X + imageWidth));
            double clampedY = Math.Max(ImagePosition.Y, Math.Min(point.Y, ImagePosition.Y + imageHeight));
            return new Point(clampedX, clampedY);
        }

        private Rect ClampRectangleInsideView(Rect rect)
        {
            double imageWidth = GetScaled(customZoomableImage.ImageWidth);
            double imageHeight = GetScaled(customZoomableImage.ImageWidth);

            var rectStart = new Point(rect.X, rect.Y);
            var rectEnd = new Point(rect.X + rect.Width, rect.Y + rect.Height);

            //Clamping the values so they remain inside the view
            var clampedRectStart = ClampPointInsideView(ClampPointInsideImage(rectStart));
            var clampedRectEnd = ClampPointInsideView(ClampPointInsideImage(rectEnd));


            double clampedWidth = clampedRectEnd.X - clampedRectStart.X;
            double clampedHeight = clampedRectEnd.Y - clampedRectStart.Y;

            clampedWidth = Math.Min(clampedWidth,
                Math.Min(ImagePosition.X + imageWidth - clampedRectStart.X,
                    ActualWidth - GetVerticalScrollBarSize() - clampedRectStart.X));
            clampedHeight = Math.Min(clampedHeight,
                Math.Min(ImagePosition.Y + imageHeight - clampedRectStart.Y,
                    ActualHeight - GetHorizontalScrollBarSize() - clampedRectStart.Y));

            if (clampedHeight <= 0 || clampedWidth <= 0)
            {
                return Rect.Empty;
            }

            return new Rect
            {
                X = clampedRectStart.X, Y = clampedRectStart.Y, Width = clampedWidth, Height = clampedHeight
            };
        }

        private bool IsPositionOutsideImage(Point pos)
        {
            double imageWidth = GetScaled(customZoomableImage.ImageWidth);
            double imageHeight = GetScaled(customZoomableImage.ImageWidth);
            return pos.X < ImagePosition.X ||
                   pos.X < 0 ||
                   pos.X > ImagePosition.X + imageWidth ||
                   pos.X > ActualWidth - GetVerticalScrollBarSize() ||
                   pos.Y < ImagePosition.Y ||
                   pos.Y < 0 ||
                   pos.Y > ImagePosition.Y + imageHeight ||
                   pos.Y > ActualHeight - GetHorizontalScrollBarSize();
        }
    }
}
