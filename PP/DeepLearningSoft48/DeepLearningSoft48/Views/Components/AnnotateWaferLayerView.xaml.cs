using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

using DeepLearningSoft48.Services;
using DeepLearningSoft48.ViewModels;
using DeepLearningSoft48.ViewModels.DefectAnnotations;
using DeepLearningSoft48.Views.Components.DesignerCanvasComponents;

using UnitySC.Shared.UI.Converters;

namespace DeepLearningSoft48.Views.Components
{
    /// <summary>
    /// Interaction logic for AnnotateWaferLayerView.xaml
    /// </summary>
    public partial class AnnotateWaferLayerView : UserControl
    {
        public Cursor CustomCrossHair = new Cursor(Application.GetResourceStream(new Uri("../../Resources/cross_black.cur", UriKind.Relative)).Stream);

        /// <summary>
        /// Constructor
        /// </summary>
        public AnnotateWaferLayerView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets the canvas element from the ZoomAndPan control.
        /// </summary>
        private Canvas DrawingCanvas
        {
            get
            {
                if (zoomAndPan.FindName("contentPresenter") is ContentPresenter cp)
                {
                    if (cp.Content is Grid mainGrid)
                    {
                        if (mainGrid.Children[1] is Canvas canvas)
                            return canvas;
                    }
                }
                return new Canvas();
            }
        }

        /// <summary>
        /// Gets the VM associed to the current view.
        /// </summary>
        private AnnotateWaferLayerViewModel viewModel
        {
            get { return DataContext as AnnotateWaferLayerViewModel; }
        }

        #region Drawing Item Collection Manamgement

        /// <summary>
        /// The <see cref="DrawingItems" /> dependency property's name.
        /// </summary>
        public const string DrawingItemsPropertyName = "DrawingItems";

        /// <summary>
        /// Gets or sets the value of the <see cref="DrawingItems" />
        /// property. This is a dependency property.
        /// </summary>
        public ObservableCollection<DefectAnnotationVM> DrawingItems
        {
            get
            {
                return (ObservableCollection<DefectAnnotationVM>)GetValue(DrawingItemsProperty);
            }
            set
            {
                SetValue(DrawingItemsProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="DrawingItems" /> dependency property.
        /// Collection binded from the VM.
        /// </summary>
        public static readonly DependencyProperty DrawingItemsProperty = DependencyProperty.Register(
            DrawingItemsPropertyName,
            typeof(ObservableCollection<DefectAnnotationVM>),
            typeof(AnnotateWaferLayerView),
            new UIPropertyMetadata(null, OnDrawingItemsChanged));

        /// <summary>
        /// Catch when we change the given collection or when a RaisePropChange is raised on the collection.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnDrawingItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;

            var drawingItems = (ObservableCollection<DefectAnnotationVM>)e.NewValue;

            // Subscribe to CollectionChanged on the new collection
            drawingItems.CollectionChanged += (d as AnnotateWaferLayerView).DrawingItems_CollectionChanged;

            foreach (var drawingItem in drawingItems)
            {
                (d as AnnotateWaferLayerView)?.Add(drawingItem);
            }
        }

        /// <summary>
        /// Catch whenever an item is added or removed from the given collection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawingItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // If the collection has been clared, we reset the canvas.
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                DrawingCanvas.Children.Clear();
                return;
            }

            // If an element has been remove we break.
            if (e.NewItems == null || e.Action == NotifyCollectionChangedAction.Remove)
                return;

            // If an element has been added, we add it to the canvas.
            foreach (var newItem in e.NewItems)
                Add(newItem as DefectAnnotationVM);
        }

        /// <summary>
        /// Adds an element accoring to the type of annotation.
        /// </summary>
        /// <param name="drawingItem"></param>
        private void Add(DefectAnnotationVM drawingItem)
        {
            if (drawingItem is BoundingBoxVM)
                Add(drawingItem as BoundingBoxVM);

            if (drawingItem is PolylineAnnotationVM)
                Add(drawingItem as PolylineAnnotationVM);

            if (drawingItem is LineAnnotationVM)
                Add(drawingItem as LineAnnotationVM);

            if (drawingItem is PolygonAnnotationVM)
                Add(drawingItem as PolygonAnnotationVM);
        }

        /// <summary>
        /// Creates and adds a bounding box to the canvas.
        /// </summary>
        /// <param name="boxItem"></param>
        public void Add(BoundingBoxVM boxItem)
        {
            var rectangle = new Rectangle();
            rectangle.StrokeThickness = 3;
            rectangle.Stroke = new SolidColorBrush(ColorToSolidBrushConverter.ColorToColor(boxItem.Category.Color));
            var rectangleComponent = new RectangleDesignerComponent(rectangle, boxItem);
            AddDrawingCanvasChild(rectangleComponent);
        }

        /// <summary>
        /// Creates and adds a free hand line to the canvas.
        /// </summary>
        /// <param name="polylineItem"></param>
        public void Add(PolylineAnnotationVM polylineItem)
        {
            var polyline = new Polyline();
            polyline.StrokeThickness = 3;
            polyline.Stroke = new SolidColorBrush(ColorToSolidBrushConverter.ColorToColor(polylineItem.Category.Color));
            polyline.Points = polylineItem.Points;
            var polylineComponent = new PolylineDesignerComponent(polyline, polylineItem);
            AddDrawingCanvasChild(polylineComponent);
        }

        /// <summary>
        /// Creates and adds a line to the canvas.
        /// </summary>
        /// <param name="lineItem"></param>
        public void Add(LineAnnotationVM lineItem)
        {
            var line = new Line();
            line.StrokeThickness = 3;
            line.Stroke = new SolidColorBrush(ColorToSolidBrushConverter.ColorToColor(lineItem.Category.Color));
            line.X1 = lineItem.X1;
            line.Y1 = lineItem.Y1;
            line.X2 = lineItem.X2;
            line.Y2 = lineItem.Y2;
            var lineComponent = new LineDesignerComponent(line, lineItem);
            AddDrawingCanvasChild(lineComponent);
        }

        /// <summary>
        /// Creates and adds a polygon to the canvas.
        /// </summary>
        /// <param name="polygonItem"></param>
        public void Add(PolygonAnnotationVM polygonItem)
        {
            var polygon = new Polygon();
            polygon.StrokeThickness = 3;
            polygon.Stroke = new SolidColorBrush(ColorToSolidBrushConverter.ColorToColor(polygonItem.Category.Color));
            polygon.Points = polygonItem.Points;
            var lineComponent = new PolygonDesignerComponent(polygon, polygonItem);
            AddDrawingCanvasChild(lineComponent);
        }

        /// <summary>
        /// Adds an element to the canvas.
        /// </summary>
        /// <param name="uiElement"></param>
        private void AddDrawingCanvasChild(UIElement uiElement)
        {
            DrawingCanvas.Children.Add(uiElement);
            uiElement.MouseEnter += Child_MouseEnter;
            uiElement.MouseMove += Child_MouseMove;
            uiElement.MouseLeave += Child_MouseLeave;
        }

        #endregion


        #region Visual Drawing Events Management

        /// <summary>
        /// Drawing Properties
        /// </summary>
        private Point _startPoint;
        private Point _endPoint;

        private Line _line;
        private Polyline _polyLine;
        private Path _path;

        private RectangleGeometry _rectangleGeometry;

        private PathFigure _pathFigurePolygon;

        private LineSegment _segment;


        /// <summary>
        /// When the user begins to draw a shape.
        /// </summary>
        private void DrawingCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Get the position where the mouse downed
            _startPoint = e.GetPosition(DrawingCanvas);

            if (viewModel.SelectedCategory != null && !viewModel.SelectedCategory.Color.Equals(Color.FromScRgb(0, 0, 0, 0)))
            {
                // Creates shape's element according to the chosen type
                if (viewModel.ToolMode == ToolMode.POLYGON)
                {
                    if (_path == null || _pathFigurePolygon == null)
                    {
                        _path = new Path();
                        _path.Stroke = new SolidColorBrush(ColorToSolidBrushConverter.ColorToColor(viewModel.SelectedCategory.Color));
                        _path.StrokeThickness = viewModel.SelectedThickness;

                        _pathFigurePolygon = new PathFigure();
                        _pathFigurePolygon.StartPoint = _startPoint;

                        DrawingCanvas.Children.Add(_path);
                    }

                    _segment = new LineSegment();
                    _segment.Point = _startPoint;
                    _pathFigurePolygon.Segments.Add(_segment);

                    PathGeometry geometry = new PathGeometry();
                    geometry.Figures.Add(_pathFigurePolygon);
                    _path.Data = geometry;
                }
                else if (viewModel.ToolMode == ToolMode.BOX)
                {
                    _path = new Path();
                    _path.Stroke = new SolidColorBrush(ColorToSolidBrushConverter.ColorToColor(viewModel.SelectedCategory.Color));
                    _path.StrokeThickness = viewModel.SelectedThickness;

                    _rectangleGeometry = new RectangleGeometry();
                    _path.Data = _rectangleGeometry;

                    DrawingCanvas.Children.Add(_path);
                }
                else if (viewModel.ToolMode == ToolMode.LINE)
                {
                    _line = new Line();
                    _line.Stroke = new SolidColorBrush(ColorToSolidBrushConverter.ColorToColor(viewModel.SelectedCategory.Color));
                    _line.StrokeThickness = viewModel.SelectedThickness;
                    DrawingCanvas.Children.Add(_line);

                    _line.X1 = _startPoint.X;
                    _line.Y1 = _startPoint.Y;
                }
                else if (viewModel.ToolMode == ToolMode.PENCIL)
                {
                    _polyLine = new Polyline();
                    _polyLine.Stroke = new SolidColorBrush(ColorToSolidBrushConverter.ColorToColor(viewModel.SelectedCategory.Color));
                    _polyLine.StrokeThickness = viewModel.SelectedThickness;

                    _polyLine.Points.Add(_startPoint);

                    DrawingCanvas.Children.Add(_polyLine);
                }
            }
        }

        private void DrawingCanvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            foreach (FrameworkElement fElem in DrawingCanvas.Children)
            {
                if (fElem is RectangleDesignerComponent)
                    ((RectangleDesignerComponent)fElem).IsSelected = true;

                if (fElem is PolylineDesignerComponent)
                    ((PolylineDesignerComponent)fElem).IsSelected = true;

                if (fElem is LineDesignerComponent)
                    ((LineDesignerComponent)fElem).IsSelected = true;

                if (fElem is PolygonDesignerComponent)
                    ((PolygonDesignerComponent)fElem).IsSelected = true;
            }
        }

        /// <summary>
        /// When the user is drawing while dragging the shape.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawingCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_startPoint != null)
            {
                // Cursor Update according to the chosen tool 
                if (viewModel.ToolMode == ToolMode.PENCIL)
                    DrawingCanvas.Cursor = Cursors.Pen;
                else if (viewModel.ToolMode == ToolMode.LINE || viewModel.ToolMode == ToolMode.BOX || viewModel.ToolMode == ToolMode.POLYGON)
                    DrawingCanvas.Cursor = CustomCrossHair;
                else if (viewModel.ToolMode == ToolMode.ERASER)
                    DrawingCanvas.Cursor = Cursors.Hand;
                else
                    DrawingCanvas.Cursor = Cursors.Arrow;

                // Get the current point of the cursor on the canvas
                Point currentPoint = e.GetPosition(DrawingCanvas);

                // Check that the left button is pressed to enable drawing
                if (Mouse.LeftButton == MouseButtonState.Pressed && viewModel.SelectedCategory != null && !viewModel.SelectedCategory.Color.Equals(Color.FromScRgb(0, 0, 0, 0)))
                {
                    if (viewModel.ToolMode == ToolMode.PENCIL)
                    {
                        _polyLine.Points.Add(currentPoint);
                    }
                    else if (viewModel.ToolMode == ToolMode.BOX)
                    {
                        _rectangleGeometry.Rect = new Rect(_startPoint, currentPoint);
                    }
                    else if (viewModel.ToolMode == ToolMode.LINE)
                    {
                        _line.X2 = currentPoint.X;
                        _line.Y2 = currentPoint.Y;
                    }
                    else if (viewModel.ToolMode == ToolMode.POLYGON)
                    {
                        _segment.Point = currentPoint;
                    }
                }
            }
        }

        /// <summary>
        /// When the user finishes its shape.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawingCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_startPoint != null)
            {
                _endPoint = e.GetPosition(DrawingCanvas);

                if (viewModel.SelectedCategory != null && !viewModel.SelectedCategory.Color.Equals(Color.FromScRgb(0, 0, 0, 0)))
                {
                    // Create the shape (frameworkElement) and Set its position on the canvas
                    if (viewModel.ToolMode == ToolMode.POLYGON)
                    {
                        if (_pathFigurePolygon.Segments.Count > 1)
                        {
                            // If the user clicks very close from the start point, we consider that they want to close it
                            if (_endPoint.X >= (_pathFigurePolygon.StartPoint.X - 10) && _endPoint.X <= (_pathFigurePolygon.StartPoint.X + 10)
                                && _endPoint.Y >= (_pathFigurePolygon.StartPoint.Y - 10) && _endPoint.Y <= (_pathFigurePolygon.StartPoint.Y + 10))
                            {

                                // We start with the start point of the path figure
                                double minX = _pathFigurePolygon.StartPoint.X;
                                double minY = _pathFigurePolygon.StartPoint.Y;

                                double maxX = _pathFigurePolygon.StartPoint.X;
                                double maxY = _pathFigurePolygon.StartPoint.Y;

                                // Then, for each segment of the path figure, we compare the point with the minX and minY
                                // to determinate the X and Y position to which the polygon shape has to be set on the canvas
                                foreach (LineSegment segment in _pathFigurePolygon.Segments)
                                {
                                    if (segment.Point.X < minX) minX = segment.Point.X;
                                    if (segment.Point.Y < minY) minY = segment.Point.Y;

                                    if (segment.Point.X > maxX) maxX = segment.Point.X;
                                    if (segment.Point.Y > maxY) maxY = segment.Point.Y;
                                }

                                PolygonAnnotationVM polygon = new PolygonAnnotationVM(minX, minY, maxX - minX, maxY - minY, viewModel.SelectedCategory, viewModel.SelectedLayerProperties.Key);

                                // Now we can add all points to the Polygon points list, removing the offset and make it clipped to bounds
                                polygon.Points.Add(new Point(_pathFigurePolygon.StartPoint.X - minX, _pathFigurePolygon.StartPoint.Y - minY));
                                foreach (LineSegment segment in _pathFigurePolygon.Segments)
                                {
                                    double x = segment.Point.X - minX;
                                    double y = segment.Point.Y - minY;

                                    polygon.Points.Add(new Point(x, y));
                                }

                                // We don't forget to remove the last point made by the user while in a polygon, the start equals the end point (closed shape)
                                polygon.Points.RemoveAt(polygon.Points.Count - 1);

                                // Finally, we remove the path
                                DrawingCanvas.Children.Remove(_path);
                                _pathFigurePolygon = null;

                                // Add the defect to the wafer defects list and update canvas.
                                WaferService.SelectedWafer?.DefectsAnnotationsCollection.Add(polygon);
                                viewModel.DefectsAnnotationsCollection.Add(polygon);
                                WaferService.AddDefectAnnotation(polygon); // Add to Selected Wafer's DefectsAnnotations Collection
                            }
                        }
                    }
                    else if (viewModel.ToolMode == ToolMode.LINE)
                    {
                        // We determinate the width and the height of the Line
                        _line.Width = Math.Abs(_startPoint.X - _endPoint.X);
                        _line.Height = Math.Abs(_startPoint.Y - _endPoint.Y);

                        // Then, according to the direction toward which the Line has been drawn,
                        // we determinate the origin point to set the position of the Line shape on the canvas
                        if (_line.X1 < _line.X2)
                        {
                            Canvas.SetLeft(_line, _startPoint.X);
                            _line.X1 = 0;
                            _line.X2 = _line.Width;
                        }
                        else
                        {
                            Canvas.SetLeft(_line, _endPoint.X);
                            _line.X2 = 0;
                            _line.X1 = _line.Width;
                        }

                        if (_line.Y1 < _line.Y2)
                        {
                            Canvas.SetTop(_line, _startPoint.Y);
                            _line.Y1 = 0;
                            _line.Y2 = _line.Height;
                        }
                        else
                        {
                            Canvas.SetTop(_line, _endPoint.Y);
                            _line.Y2 = 0;
                            _line.Y1 = _line.Height;
                        }

                        // We remove the path
                        DrawingCanvas.Children.Remove(_line);

                        // Add the defect to the wafer defects list and update canvas.
                        LineAnnotationVM line = new LineAnnotationVM(Canvas.GetLeft(_line), Canvas.GetTop(_line), _line.X1, _line.Y1, _line.X2, _line.Y2, _line.Width, _line.Height, viewModel.SelectedCategory, viewModel.SelectedLayerProperties.Key);
                        WaferService.SelectedWafer?.DefectsAnnotationsCollection.Add(line);
                        viewModel.DefectsAnnotationsCollection.Add(line);
                        WaferService.AddDefectAnnotation(line); // Add to Selected Wafer's DefectsAnnotations Collection
                    }
                    else if (viewModel.ToolMode == ToolMode.BOX)
                    {
                        if (_rectangleGeometry.Rect.Width > 0 && _rectangleGeometry.Rect.Height > 0)
                        {
                            // We remove the path
                            DrawingCanvas.Children.Remove(_path);

                            // Add the defect to the wafer defects list and update canvas.
                            BoundingBoxVM bbox = new BoundingBoxVM(_rectangleGeometry.Rect.Left, _rectangleGeometry.Rect.Top, _rectangleGeometry.Rect.Width, _rectangleGeometry.Rect.Height, viewModel.SelectedCategory, viewModel.SelectedLayerProperties.Key);
                            viewModel.DefectsAnnotationsCollection.Add(bbox);
                            WaferService.AddDefectAnnotation(bbox); // Add to Selected Wafer's DefectsAnnotations Collection
                        }
                    }
                    else if (viewModel.ToolMode == ToolMode.PENCIL)
                    {
                        // We remove the path
                        DrawingCanvas.Children.Remove(_polyLine);

                        // Add the defect to the wafer defects list and update canvas.
                        PolylineAnnotationVM pencil = new PolylineAnnotationVM(Canvas.GetLeft(_polyLine), Canvas.GetTop(_polyLine), _polyLine.Points, _polyLine.Width, _polyLine.Height, viewModel.SelectedCategory, viewModel.SelectedLayerProperties.Key);
                        WaferService.SelectedWafer?.DefectsAnnotationsCollection.Add(pencil);
                        viewModel.DefectsAnnotationsCollection.Add(pencil);
                        WaferService.AddDefectAnnotation(pencil); // Add to Selected Wafer's DefectsAnnotations Collection
                    }
                }

                Debug.WriteLine("Mask contains " + DrawingCanvas.Children.Count + " elements.");
            }
        }

        #endregion

        #region Canvas Children Event Handlers
        private void Child_MouseEnter(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed && viewModel.ToolMode == ToolMode.ERASER)
            {
                // When the mouse enter the element while mouse left's button is downed,
                // if we are in EraseMode, it will delete the element.
                if (sender is UIElement designerComponent)
                {
                    int index = DrawingCanvas.Children.IndexOf(designerComponent);
                    DefectAnnotationVM targetDefectAnnotation = viewModel.DefectsAnnotationsCollection[index];

                    // Remove the targetted DefectAnnotation for erasure
                    if (WaferService.SelectedWafer.DefectsAnnotationsCollection.Contains(targetDefectAnnotation))
                    {
                        viewModel.DefectsAnnotationsCollection.RemoveAt(index);
                        WaferService.RemoveDefectAnnotation(targetDefectAnnotation);
                        DrawingCanvas.Children.Remove(designerComponent);
                    }

                    // We unsubscribe of all associated events.
                    designerComponent.MouseEnter -= Child_MouseEnter;
                    designerComponent.MouseMove -= Child_MouseMove;
                    designerComponent.MouseLeave -= Child_MouseLeave;
                }
            }
            else if (viewModel.ToolMode == ToolMode.SELECT)
            {
                // If we are in SelectMode, it will give the possibility to the user to move it.
                Cursor = Cursors.SizeAll;
            }
        }
        private void Child_MouseMove(object sender, MouseEventArgs e)
        {
            // While the mouse is over an element, the cursor give the possibility to the user to move it.
            Cursor = Cursors.SizeAll;
        }
        private void Child_MouseLeave(object sender, MouseEventArgs e)
        {
            // When the mouse leave the element, we reset the cursor.
            Cursor = Cursors.Arrow;
        }
        #endregion
    }
}
