using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

using DeepLearningSoft48.Models.DefectAnnotations;
using DeepLearningSoft48.Services;
using DeepLearningSoft48.ViewModels.DefectAnnotations;

namespace DeepLearningSoft48.Views.Components.DesignerCanvasComponents
{
    /// <summary>
    /// Interaction logic for RectangleDesignerComponent.xaml
    /// </summary>
    public partial class RectangleDesignerComponent : UserControl
    {
        private static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(RectangleDesignerComponent), new FrameworkPropertyMetadata(false));

        private static readonly DependencyProperty IsCtrlKeyPressedProperty = DependencyProperty.Register("IsCtrlKeyPressed", typeof(bool), typeof(RectangleDesignerComponent), new FrameworkPropertyMetadata(false));

        private readonly double minHeight;
        private readonly double minWidth;

        private readonly double maxHeight;
        private readonly double maxWidth;
        public BoundingBoxVM BoundingBoxVM { get; private set; }

        public event EventHandler DragCompleted;

        public bool CanVResize { get; private set; }
        public bool CanHResize { get; private set; }

        public RectangleDesignerComponent(FrameworkElement content, BoundingBoxVM boundingBoxVM)
        {
            InitializeComponent();

            BoundingBoxVM = boundingBoxVM;

            //Check is the content element can resize
            //If cannot resize H or V than the corresponding resize handles will not be displayed.
            //Minimum height/width must be 30.

            CanHResize = true;
            Width = boundingBoxVM.Width;

            CanVResize = true;
            Height = boundingBoxVM.Height;

            //Check if min/max values are set for content
            minWidth = content.MinWidth < 10.0 ? 10.0 : content.MinWidth;
            minHeight = content.MinHeight < 10.0 ? 10.0 : content.MinHeight;
            maxWidth = content.MaxWidth;
            maxHeight = content.MaxHeight;

            //Check if canvas values (X and Y coordinates) are set.
            double top = (double)content.GetValue(Canvas.TopProperty);
            if (double.IsNaN(top))
                top = boundingBoxVM.OriginY;
            double left = (double)content.GetValue(Canvas.LeftProperty);
            if (double.IsNaN(left))
                left = boundingBoxVM.OriginX;
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

                BoundingBoxVM.IsSelected = value;

                if (value)
                    Canvas.SetZIndex(this, 100);
                else
                    Canvas.SetZIndex(this, 0);
                Focus();
            }
        }

        public bool IsCtrlKeyPressed
        {
            get { return (bool)GetValue(IsCtrlKeyPressedProperty); }
            set
            {
                SetValue(IsCtrlKeyPressedProperty, value);
            }
        }

        private void DesignerComponent_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            IsSelected = true;
        }

        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {

            var currentTop = (double)GetValue(Canvas.TopProperty);
            var currentLeft = (double)GetValue(Canvas.LeftProperty);

            string name = ((Thumb)sender).Name;
            if (name.Contains("Top"))
            {
                double newHeight = Height - e.VerticalChange;
                if (newHeight >= minHeight && newHeight <= maxHeight)
                {
                    var newTop = currentTop + e.VerticalChange;
                    if (newTop < 0)
                    {
                        newHeight = Height + currentTop;
                        newTop = 0;
                    }
                    Height = newHeight;
                    SetValue(Canvas.TopProperty, newTop);
                }
            }
            if (name.Contains("Right"))
            {
                double newWidth = Width + e.HorizontalChange;
                if (newWidth >= minWidth && newWidth <= maxWidth)
                {
                    if (currentLeft + newWidth > (Parent as Canvas).ActualWidth)
                        newWidth = (Parent as Canvas).ActualWidth - currentLeft;
                    Width = newWidth;
                }

            }
            if (name.Contains("Bottom"))
            {
                double newHeight = Height + e.VerticalChange;
                if (newHeight >= minHeight && newHeight <= maxHeight)
                {
                    if (currentTop + newHeight > (Parent as Canvas).ActualHeight)
                        newHeight = (Parent as Canvas).ActualHeight - currentTop;
                    Height = newHeight;
                }

            }
            if (name.Contains("Left"))
            {
                double newWidth = Width - e.HorizontalChange;
                if (newWidth >= minWidth && newWidth <= maxWidth)
                {
                    var newLeft = currentLeft + e.HorizontalChange;
                    if (newLeft < 0)
                    {
                        newWidth = Width + currentLeft;
                        newLeft = 0;
                    }

                    Width = newWidth;
                    SetValue(Canvas.LeftProperty, newLeft);
                }
            }

            UpdateDrawingItem();
        }

        private void UpdateDrawingItem()
        {
            BoundingBoxVM.OriginX = (int)((double)GetValue(Canvas.LeftProperty));
            BoundingBoxVM.OriginY = (int)((double)GetValue(Canvas.TopProperty));
            BoundingBoxVM.Width = (int)Width;
            BoundingBoxVM.Height = (int)Height;
        }

        private void SelectionThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            SetValue(Canvas.LeftProperty, (double)GetValue(Canvas.LeftProperty) + e.HorizontalChange);
            SetValue(Canvas.TopProperty, (double)GetValue(Canvas.TopProperty) + e.VerticalChange);

            //UpdateDrawingItem();
        }

        private void ThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            // Create a copy of the BoundingBox we want to replace, pass it to the WaferService where it'll be stored
            BoundingBox boundingBox = new BoundingBox(BoundingBoxVM.OriginX, BoundingBoxVM.OriginY, BoundingBoxVM.Width, BoundingBoxVM.Height, BoundingBoxVM.Category, BoundingBoxVM.Source);
            WaferService.SetPreviousDefectAnnotation(boundingBox);
        }

        private void ThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            UpdateDrawingItem();

            OnDragCompleted();
        }

        public void OnDragCompleted()
        {
            EventHandler eh = DragCompleted;
            if (eh != null)
            {
                eh(this, new EventArgs());
            }

            // Create a copy of the newly updated BoundingBox then pass it to the UpdateAnnotation where it replaces the old BoundingBox
            BoundingBox boundingBox = new BoundingBox(BoundingBoxVM.OriginX, BoundingBoxVM.OriginY, BoundingBoxVM.Width, BoundingBoxVM.Height, BoundingBoxVM.Category, BoundingBoxVM.Source);
            WaferService.UpdateAnnotation(boundingBox, WaferService.SelectedWafer);
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl)
                IsCtrlKeyPressed = true;
        }

        private void UserControl_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl)
                IsCtrlKeyPressed = false;
        }
    }
}
