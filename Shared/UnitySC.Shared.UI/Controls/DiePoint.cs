using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
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
    ///     <MyNamespace:DiePoint/>
    ///
    /// </summary>
    public class DiePoint : Control
    {
        static DiePoint()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiePoint), new FrameworkPropertyMetadata(typeof(DiePoint)));
        }

        public DiePoint()
        {
            Width = 20;
            Height = 20;

            var descriptorLeft = DependencyPropertyDescriptor.FromProperty(Canvas.LeftProperty, typeof(DiePoint));
            descriptorLeft.AddValueChanged(this, OnCanvasLeftChanged);

            var descriptorTop = DependencyPropertyDescriptor.FromProperty(Canvas.TopProperty, typeof(DiePoint));
            descriptorTop.AddValueChanged(this, OnCanvasTopChanged);
        }

        private void OnCanvasLeftChanged(object sender, EventArgs e)
        {
            UpdateLinksPositions();
        }

        private void OnCanvasTopChanged(object sender, EventArgs e)
        {
            UpdateLinksPositions();
        }

        private void UpdateLinksPositions()
        {
            if (PreviousLink != null)
            {
                PreviousLink.X2 = Position.X;
                PreviousLink.Y2 = Position.Y;
            }

            if (NextLink != null)
            {
                NextLink.X1 = Position.X;
                NextLink.Y1 = Position.Y;
            }
        }

        public Point Position
        {
            get
            {
                var Y = Canvas.GetTop(this) + Height / 2;
                var X = Canvas.GetLeft(this) + Width / 2;
                return new Point(X, Y);
            }
            set
            {
                Canvas.SetTop(this, value.Y - Width / 2);
                Canvas.SetLeft(this, value.X - Height / 2);
            }
        }

        public Line PreviousLink;

        public Line NextLink;
    }
}
