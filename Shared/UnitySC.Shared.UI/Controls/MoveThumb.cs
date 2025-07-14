using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace UnitySC.Shared.UI.Controls
{
    public class MoveThumb : Thumb
    {
        private DiePoint diePoint;
        private Canvas designerCanvas;

        public MoveThumb()
        {
            DragStarted += new DragStartedEventHandler(this.MoveThumb_DragStarted);
            DragDelta += new DragDeltaEventHandler(this.MoveThumb_DragDelta);
        }

        private void MoveThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            this.diePoint = DataContext as DiePoint;

            if (this.diePoint != null)
            {
                this.designerCanvas = VisualTreeHelper.GetParent(this.diePoint) as Canvas;
            }
        }

        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (this.diePoint != null && this.designerCanvas != null)
            {
                double minLeft = double.MaxValue;
                double minTop = double.MaxValue;

                minLeft = Math.Min(Canvas.GetLeft(this.diePoint), minLeft);
                minTop = Math.Min(Canvas.GetTop(this.diePoint), minTop);

                //foreach (DiePoint item in this.designerCanvas.SelectedItems)
                //{
                //    minLeft = Math.Min(Canvas.GetLeft(item), minLeft);
                //    minTop = Math.Min(Canvas.GetTop(item), minTop);
                //}

                double deltaHorizontal = Math.Max(-minLeft, e.HorizontalChange);
                double deltaVertical = Math.Max(-minTop, e.VerticalChange);

                var newLeft = Math.Min(Canvas.GetLeft(this.diePoint) + deltaHorizontal, designerCanvas.ActualWidth - this.ActualWidth);
                var newRight = Math.Min(Canvas.GetTop(this.diePoint) + deltaVertical, designerCanvas.ActualHeight - this.ActualHeight);

                Canvas.SetLeft(this.diePoint, newLeft);
                Canvas.SetTop(this.diePoint, newRight);

                this.designerCanvas.InvalidateMeasure();
                e.Handled = true;
            }
        }
    }
}
