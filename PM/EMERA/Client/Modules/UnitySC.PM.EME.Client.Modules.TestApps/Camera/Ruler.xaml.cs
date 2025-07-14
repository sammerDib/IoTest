using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace UnitySC.PM.EME.Client.Modules.TestApps.Camera
{
    public partial class Ruler
    {
        public Ruler()
        {
            InitializeComponent();
            UpdateTextPosition();
        }

        private void UpdateTextPosition()
        {
            DistanceTextBlock.SetValue(Canvas.LeftProperty, (StartPoint.X + EndPoint.X) / 2);
            DistanceTextBlock.SetValue(Canvas.TopProperty, (StartPoint.Y + EndPoint.Y) / 2);
        }

        public static readonly DependencyProperty StartPointProperty =
            DependencyProperty.Register(nameof(StartPoint), typeof(Point), typeof(Ruler),
                new PropertyMetadata(new Point(100, 300)));

        public Point StartPoint
        {
            get => (Point)GetValue(StartPointProperty);
            set
            {
                SetValue(StartPointProperty, value);
                UpdateTextPosition();
            }
        }

        public static readonly DependencyProperty EndPointProperty =
            DependencyProperty.Register(nameof(EndPoint), typeof(Point), typeof(Ruler),
                new PropertyMetadata(new Point(400, 300)));

        public Point EndPoint
        {
            get => (Point)GetValue(EndPointProperty);
            set
            {
                SetValue(EndPointProperty, value);
                UpdateTextPosition();
            }
        }

        public static readonly DependencyProperty DistanceValueTextProperty =
            DependencyProperty.Register(nameof(DistanceValueText), typeof(string), typeof(Ruler),
                new PropertyMetadata(default(string)));

        public string DistanceValueText
        {
            get => (string)GetValue(DistanceValueTextProperty);
            set => SetValue(DistanceValueTextProperty, value);
        }

        private void StartPointOnDragDelta(object sender, DragDeltaEventArgs e)
        {
            double updatedX = GetDraggedPointX(StartPoint.X, e.HorizontalChange);
            double updatedY = GetDraggedPointY(StartPoint.Y, e.VerticalChange);
            StartPoint = new Point(updatedX, updatedY);
        }

        private void EndPointOnDragDelta(object sender, DragDeltaEventArgs e)
        {
            double updatedX = GetDraggedPointX(EndPoint.X, e.HorizontalChange);
            double updatedY = GetDraggedPointY(EndPoint.Y, e.VerticalChange);
            EndPoint = new Point(updatedX, updatedY);
        }

        private double GetDraggedPointX(double previousX, double horizontalChange)
        {
            double updatedX = previousX + horizontalChange;
            if (updatedX < 0)
            {
                updatedX = 0;
            }
            else if (updatedX > ActualWidth)
            {
                updatedX = ActualWidth;
            }

            return updatedX;
        }

        private double GetDraggedPointY(double previousY, double verticalChange)
        {
            double updatedY = previousY + verticalChange;
            if (updatedY < 0)
            {
                updatedY = 0;
            }
            else if (updatedY > ActualHeight)
            {
                updatedY = ActualHeight;
            }

            return updatedY;
        }

        private void OnMouseLefButtonDown(object sender, MouseButtonEventArgs e)
        {
            StartPoint = e.GetPosition(RulerCanvas);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;
            if (e.OriginalSource is Thumb) return; // avoid changing end point when dragging any thumb element 
            EndPoint = e.GetPosition(RulerCanvas);
        }
    }
}
