using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace UnitySC.Shared.UI.Controls.ZoomboxImage
{
    /// <summary>
    /// Une poignée qui sert à redimensionner un control qui lui est passée comme DataContext.
    /// </summary>
    public class ResizeThumb : Thumb
    {
        public ResizeThumb()
        {
            DragDelta += new DragDeltaEventHandler(ResizeThumb_DragDelta);
        }

        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var designerItem = DataContext as Control;
            var canvas = (Canvas)designerItem.Parent;

            double MaxW = canvas.ActualWidth;
            double MaxH = canvas.ActualHeight;
            double left = Canvas.GetLeft(designerItem);
            double top = Canvas.GetTop(designerItem);
            double w = designerItem.ActualWidth;
            double h = designerItem.ActualHeight;
            double right = left + w;
            double bottom = top + h;

            if (designerItem != null)
            {
                double deltaVertical, deltaHorizontal;

                switch (VerticalAlignment)
                {
                    case VerticalAlignment.Bottom:
                        deltaVertical = Math.Min(-e.VerticalChange, designerItem.ActualHeight - designerItem.MinHeight);
                        if (bottom - deltaVertical >= MaxH)
                            deltaVertical = bottom - MaxH;
                        designerItem.Height -= deltaVertical;
                        break;

                    case VerticalAlignment.Top:
                        deltaVertical = Math.Min(e.VerticalChange, designerItem.ActualHeight - designerItem.MinHeight);
                        if (top + deltaVertical < 0)
                            deltaVertical = -top;
                        Canvas.SetTop(designerItem, top + deltaVertical);
                        designerItem.Height -= deltaVertical;
                        break;

                    default:
                        break;
                }

                switch (HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        deltaHorizontal = Math.Min(e.HorizontalChange, designerItem.ActualWidth - designerItem.MinWidth);
                        if (left + deltaHorizontal < 0)
                            deltaHorizontal = -left;
                        Canvas.SetLeft(designerItem, left + deltaHorizontal);
                        designerItem.Width -= deltaHorizontal;
                        break;

                    case HorizontalAlignment.Right:
                        deltaHorizontal = Math.Min(-e.HorizontalChange, designerItem.ActualWidth - designerItem.MinWidth);
                        if (right - deltaHorizontal >= MaxW)
                            deltaHorizontal = right - MaxW;
                        designerItem.Width -= deltaHorizontal;
                        break;

                    default:
                        break;
                }
            }

            e.Handled = true;
        }
    }
}