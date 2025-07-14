using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace DeepLearningSoft48.Views.Components.DesignerCanvasComponents
{
    /// <summary>
    /// Interaction logic for DesignerComponent.xaml
    /// </summary>
    public partial class DesignerComponent : UserControl
    {
        private static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(DesignerComponent), new FrameworkPropertyMetadata(false));

        private readonly double minHeight;
        private readonly double minWidth;

        private readonly double maxHeight;
        private readonly double maxWidth;

        public bool CanVResize { get; private set; }
        public bool CanHResize { get; private set; }

        public DesignerComponent(FrameworkElement content)
        {
            InitializeComponent();

            //Check is the content element can resize
            //If cannot resize H or V than the corresponding resize handles will not be displayed.
            //Minimum height/width must be 30.
            if (!double.IsNaN(content.Width))
            {
                CanHResize = false;
                Width = content.Width;
            }
            else
            {
                CanHResize = true;
                Width = 23.0;
            }
            if (!double.IsNaN(content.Height))
            {
                CanVResize = false;
                Height = content.Height; ;
            }
            else
            {
                CanVResize = true;
                Height = 23.0;
            }
            //Check if min/max values are set for content
            minWidth = content.MinWidth < 10.0 ? 10.0 : content.MinWidth;
            minHeight = content.MinHeight < 10.0 ? 10.0 : content.MinHeight;
            maxWidth = content.MaxWidth;
            maxHeight = content.MaxHeight;

            //Check if canvas values (X and Y coordinates) are set.
            double top = (double)content.GetValue(Canvas.TopProperty);
            if (double.IsNaN(top))
                top = 0.0;
            double left = (double)content.GetValue(Canvas.LeftProperty);
            if (double.IsNaN(left))
                left = 0.0;
            SetValue(Canvas.TopProperty, top);
            SetValue(Canvas.LeftProperty, left);

            //Set the actual content. Note that "Content" property is a new property. See below
            Content = content;
        }

        public new object Content
        {
            get
            {
                return ContentComponent.Content;
            }
            protected set
            {
                ContentComponent.Content = value;
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
                double newHeight = Height - e.VerticalChange;
                if (newHeight >= minHeight && newHeight <= maxHeight)
                {
                    Height = newHeight;
                    SetValue(Canvas.TopProperty, (double)GetValue(Canvas.TopProperty) + e.VerticalChange);
                }
            }
            if (name.Contains("Right"))
            {
                double newWidth = Width + e.HorizontalChange;
                if (newWidth >= minWidth && newWidth <= maxWidth)
                    Width = newWidth;
            }
            if (name.Contains("Bottom"))
            {
                double newHeight = Height + e.VerticalChange;
                if (newHeight >= minHeight && newHeight <= maxHeight)
                    Height = newHeight;
            }
            if (name.Contains("Left"))
            {
                double newWidth = Width - e.HorizontalChange;
                if (newWidth >= minWidth && newWidth <= maxWidth)
                {
                    Width = newWidth;
                    SetValue(Canvas.LeftProperty, (double)GetValue(Canvas.LeftProperty) + e.HorizontalChange);
                }
            }
        }

        private void SelectionThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            SetValue(Canvas.LeftProperty, (double)GetValue(Canvas.LeftProperty) + e.HorizontalChange);
            SetValue(Canvas.TopProperty, (double)GetValue(Canvas.TopProperty) + e.VerticalChange);
        }
    }
}
