using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UnitySC.Shared.UI.Controls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:UnitySC.Shared.UI.Controls"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:UnitySC.Shared.UI.Controls;assembly=UnitySC.Shared.UI.Controls"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:DiePointsSelection/>
    ///
    /// </summary>
    public class DiePointsSelection : Control
    {
        private Canvas _pointsCanvas;


        private List<DiePoint> _diePoints;

        static DiePointsSelection()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiePointsSelection), new FrameworkPropertyMetadata(typeof(DiePointsSelection)));
        }


        public DiePointsSelection()
        {
            _diePoints = new List<DiePoint>();
        }


        public override void OnApplyTemplate()
        {
            Canvas theCanvas = Template.FindName("pointsCanvas", this) as Canvas;

            if (theCanvas != null)
            {
                //<-- Save a reference to the canvas
                _pointsCanvas = theCanvas;
                //_pointsCanvas.PreviewMouseUp += _mainCanvas_PreviewMouseUp;
                _pointsCanvas.MouseUp += _mainCanvas_MouseUp;
                //_pointsCanvas.PreviewMouseDown += _mainCanvas_PreviewMouseDown;
                //<-- Do some stuff.
            }
        }

        private void _mainCanvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Retrieve the coordinates of the mouse button event.
            var pt = e.GetPosition((UIElement)sender);

            AddPoint(pt);


        }

        private void AddPoint(Point pt)
        {
            var point = new DiePoint();
            point.Position = pt;

            //Canvas.SetTop(point, pt.Y- point.Width/2);
            //Canvas.SetLeft(point, pt.X- point.Height/2);
            _pointsCanvas.Children.Add(point);
            _diePoints.Add(point);

            var line = new Line();
            line.Stroke = new SolidColorBrush(Colors.Red);
            line.StrokeThickness = 4;
            if (_diePoints.Count > 1)
            {
                var diePointFrom = _diePoints[_diePoints.Count - 2];
                var diePointTo = _diePoints[_diePoints.Count - 1];
                line.X1 = diePointFrom.Position.X;
                line.Y1 = diePointFrom.Position.Y;
                line.X2 = diePointTo.Position.X;
                line.Y2 = diePointTo.Position.Y;
                line.IsHitTestVisible = false;
                line.StrokeStartLineCap = PenLineCap.Round;
                line.StrokeEndLineCap = PenLineCap.Round;
                _pointsCanvas.Children.Add(line);
                diePointFrom.NextLink = line;
                diePointTo.PreviousLink = line;

            }
        }

        private void _mainCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // Retrieve the coordinates of the mouse button event.
            var pt = e.GetPosition((UIElement)sender);

            AddPoint(pt);
        }


    }
}
