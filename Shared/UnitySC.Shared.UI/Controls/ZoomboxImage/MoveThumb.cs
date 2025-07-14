using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace UnitySC.Shared.UI.Controls.ZoomboxImage
{
    /// <summary>
    /// Une poignée qui sert à déplacer un control qui lui est passée comme DataContext.
    /// </summary>
    public class MoveThumb : Thumb
    {
        public MoveThumb()
        {
            DragDelta += new DragDeltaEventHandler(MoveThumb_DragDelta);
        }

        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var designerItem = DataContext as Control;

            if (designerItem != null)
            {
                var canvas = (Canvas)designerItem.Parent;

                double MaxW = canvas.ActualWidth;
                double MaxH = canvas.ActualHeight;
                double left = Canvas.GetLeft(designerItem);
                double top = Canvas.GetTop(designerItem);
                double w = designerItem.ActualWidth;
                double h = designerItem.ActualHeight;

                left = Math.Max(0, left + e.HorizontalChange);
                top = Math.Max(0, top + e.VerticalChange);
                if (left + w >= MaxW)
                    left = MaxW - w;
                if (top + h >= MaxH)
                    top = MaxH - h;

                Canvas.SetLeft(designerItem, left);
                Canvas.SetTop(designerItem, top);
            }
        }
    }
}