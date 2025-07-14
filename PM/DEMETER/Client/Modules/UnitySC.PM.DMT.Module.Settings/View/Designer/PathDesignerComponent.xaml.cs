using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;

namespace UnitySC.PM.DMT.Modules.Settings.View.Designer
{
    /// <summary>
    /// Interaction logic for DesignerComponent.xaml
    /// </summary>
    public partial class PathDesignerComponent : UserControl,INotify
    {
        private static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(PathDesignerComponent), new FrameworkPropertyMetadata(false));

        private readonly double _minHeight;
        private readonly double _minWidth;

        private readonly double _maxHeight;
        private readonly double _maxWidth;

        public bool CanVResize { get; private set; }
        public bool CanHResize { get; private set; }

        public PolygonDrawingItem PolygonDrawingItem { get; private set; }

        private const int ThumbSize= 20;

        public event EventHandler DragCompleted;

        public PathDesignerComponent(FrameworkElement content, PolygonDrawingItem polygonDrawingItem)
        {
            InitializeComponent();

            //Check is the content element can resize
            //If cannot resize H or V than the corresponding resize handles will not be displayed.
            //Minimum height/width must be 30.

            //Check if min/max values are set for content
            _minWidth = content.MinWidth < 10.0 ? 10.0 : content.MinWidth;
            _minHeight = content.MinHeight < 10.0 ? 10.0 : content.MinHeight;
            _maxWidth = content.MaxWidth;
            _maxHeight = content.MaxHeight;

            SetValue(Canvas.TopProperty, (double)0);
            SetValue(Canvas.LeftProperty, (double)0);
     
            PolygonDrawingItem = polygonDrawingItem;

            CanHResize = false;
            CanVResize = false;

            //Set the actual content. Note that "Content" property is a new property. See below
            Content = content;

            UpdatePath();
            UpdateThumbsPositions();
        }

        private void UpdateThumbsPositions()
        {
            PointThumb0.SetValue(Canvas.LeftProperty, PolygonDrawingItem.Points[0].X - ThumbSize/2);
            PointThumb0.SetValue(Canvas.TopProperty, PolygonDrawingItem.Points[0].Y - ThumbSize / 2);
            PointThumb1.SetValue(Canvas.LeftProperty, PolygonDrawingItem.Points[1].X - ThumbSize / 2);
            PointThumb1.SetValue(Canvas.TopProperty, PolygonDrawingItem.Points[1].Y - ThumbSize / 2);
            PointThumb2.SetValue(Canvas.LeftProperty, PolygonDrawingItem.Points[2].X - ThumbSize / 2);
            PointThumb2.SetValue(Canvas.TopProperty, PolygonDrawingItem.Points[2].Y - ThumbSize / 2);
            PointThumb3.SetValue(Canvas.LeftProperty, PolygonDrawingItem.Points[3].X - ThumbSize / 2);
            PointThumb3.SetValue(Canvas.TopProperty, PolygonDrawingItem.Points[3].Y - ThumbSize / 2);
        }

        private void UpdatePath()
        {
            PathFigure pathFigure = new PathFigure();

            pathFigure.StartPoint = PolygonDrawingItem.Points[0];

            LineSegment lineSegment1 = new LineSegment();
            lineSegment1.Point = PolygonDrawingItem.Points[1];
            pathFigure.Segments.Add(lineSegment1);

            LineSegment lineSegment2 = new LineSegment();
            lineSegment2.Point = PolygonDrawingItem.Points[2];
            pathFigure.Segments.Add(lineSegment2);

            LineSegment lineSegment3 = new LineSegment();
            lineSegment3.Point = PolygonDrawingItem.Points[3];
            pathFigure.Segments.Add(lineSegment3);

            pathFigure.IsClosed = true;

            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures = new PathFigureCollection();

            pathGeometry.Figures.Add(pathFigure);

            (Content as Path).Data = pathGeometry;
        }

        public new object Content
        {
            get
            {
                return this.ContentComponent.Content;
            }
            protected set
            {
                this.ContentComponent.Content = value;
            }
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set 
            { 
                
                SetValue(IsSelectedProperty, value);
                if (value)
                    Canvas.SetZIndex(this, 100);
                else
                    Canvas.SetZIndex(this, 0);
            }
        }

        private void DesignerComponent_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.IsSelected = true;
        }

        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            string name = ((Thumb)sender).Name;
            if (name.Contains("Top"))
            {
                double newHeight = this.Height - e.VerticalChange;
                if (newHeight >= _minHeight && newHeight <= _maxHeight)
                {
                    this.Height = newHeight;
                    SetValue(Canvas.TopProperty, (double)GetValue(Canvas.TopProperty) + e.VerticalChange);
                }
            }
            if (name.Contains("Right"))
            {
                double newWidth = this.Width + e.HorizontalChange;
                if (newWidth >= _minWidth && newWidth <= _maxWidth)
                    this.Width = newWidth;
            }
            if (name.Contains("Bottom"))
            {
                double newHeight = this.Height + e.VerticalChange;
                if (newHeight >= _minHeight && newHeight <= _maxHeight)
                    this.Height = newHeight;
            }
            if (name.Contains("Left"))
            {
                double newWidth = this.Width - e.HorizontalChange;
                if (newWidth >= _minWidth && newWidth <= _maxWidth)
                {
                    this.Width = newWidth;
                    SetValue(Canvas.LeftProperty, (double)GetValue(Canvas.LeftProperty) + e.HorizontalChange);
                }
            }
        }

        private void SelectionThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            SetValue(Canvas.LeftProperty, (double)GetValue(Canvas.LeftProperty) + e.HorizontalChange);
            SetValue(Canvas.TopProperty, (double)GetValue(Canvas.TopProperty) + e.VerticalChange);
        }

        private void PathThumb_DragDelta0(object sender, DragDeltaEventArgs e)
        {
            
            // Clip the movement
            var point = new Point(PolygonDrawingItem.Points[0].X + e.HorizontalChange, PolygonDrawingItem.Points[0].Y + e.VerticalChange);
            PolygonDrawingItem.Points[0]=ClipPointsPositions(point);
            UpdatePath();
            UpdateThumbsPositions();
        }

        private Point ClipPointsPositions(Point point)
        {
            if (point.X < 0)
                point.X = 0;
            if (point.Y < 0)
                point.Y = 0;
            if (point.X > ActualWidth)
                point.X = ActualWidth;
            if (point.Y > ActualHeight)
                point.Y = ActualHeight;
            return point;
        }

        private void PathThumb_DragDelta1(object sender, DragDeltaEventArgs e)
        {

            // Clip the movement

            var point = new Point(PolygonDrawingItem.Points[1].X + e.HorizontalChange, PolygonDrawingItem.Points[1].Y + e.VerticalChange);
            PolygonDrawingItem.Points[1] = ClipPointsPositions(point);
            UpdatePath();
            UpdateThumbsPositions();
        }

        private void PathThumb_DragDelta2(object sender, DragDeltaEventArgs e)
        {

            // Clip the movement

            var point = new Point(PolygonDrawingItem.Points[2].X + e.HorizontalChange, PolygonDrawingItem.Points[2].Y + e.VerticalChange);
            PolygonDrawingItem.Points[2] = ClipPointsPositions(point);
            UpdatePath();
            UpdateThumbsPositions();
        }

        private void PathThumb_DragDelta3(object sender, DragDeltaEventArgs e)
        {

            // Clip the movement

            var point = new Point(PolygonDrawingItem.Points[3].X + e.HorizontalChange, PolygonDrawingItem.Points[3].Y + e.VerticalChange);
            PolygonDrawingItem.Points[3] = ClipPointsPositions(point);
            UpdatePath();
            UpdateThumbsPositions();
        }

        private void ThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            OnDragCompleted();
        }

        public void OnDragCompleted()
        {
            EventHandler eh = DragCompleted;
            if (eh != null)
            {
                eh(this, new EventArgs());
            }
        }
    }
}
