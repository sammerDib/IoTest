using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace UnitySC.PM.ANA.Client.CommonUI.View
{

    public class PointBubbleShape : Shape
    {
        protected override Geometry DefiningGeometry
        {
            get { return GetGeometry(); }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size desiredSize = new Size();

            desiredSize.Width=double.IsPositiveInfinity(availableSize.Width) ? 0 : availableSize.Width;
            desiredSize.Height = double.IsPositiveInfinity(availableSize.Height) ? 0 : availableSize.Width;

   
            // we will size ourselves to fit the available space
            return desiredSize;
        }

        public Point TargetPoint { get; set; }

        public int ArrowHeight { get; set; } = 20;

        public int ArrowWidth { get; set; } = 20;


        //private Geometry GetGeometry()
        //{
        //    double cornerRadius = 10;
        //    double speechOffset = 30;
        //    double speechDepth = 20;
        //    double speechWidth = 20;
        //    double width = ActualWidth - StrokeThickness;
        //    double height = ActualHeight - StrokeThickness;
        //    var g = new StreamGeometry();
        //    using (var context = g.Open())
        //    {
        //        double x0 = StrokeThickness / 2;
        //        double x1 = x0 + cornerRadius;
        //        double x2 = width - cornerRadius - x0;
        //        double x3 = x2 + cornerRadius;

        //        double x4 = x0 + speechOffset;
        //        double x5 = x4 + speechWidth;

        //        double y0 = StrokeThickness / 2;
        //        double y1 = y0 + cornerRadius;
        //        double y2 = height - speechDepth - (cornerRadius * 2);
        //        double y3 = y2 + cornerRadius;
        //        double y4 = y3 + speechDepth;
        //        context.BeginFigure(new Point(x1, y0), true, true);
        //        context.LineTo(new Point(x2, y0), true, true);
        //        context.ArcTo(new Point(x3, y1), new Size(cornerRadius, cornerRadius), 90, false, SweepDirection.Clockwise, true, true);
        //        context.LineTo(new Point(x3, y2), true, true);
        //        context.ArcTo(new Point(x2, y3), new Size(cornerRadius, cornerRadius), 90, false, SweepDirection.Clockwise, true, true);
        //        context.LineTo(new Point(x5, y3), true, true);
        //        context.LineTo(new Point(x4, y4), true, true);
        //        context.LineTo(new Point(x4, y3), true, true);
        //        context.LineTo(new Point(x1, y3), true, true);
        //        context.ArcTo(new Point(x0, y2), new Size(cornerRadius, cornerRadius), 90, false, SweepDirection.Clockwise, true, true);
        //        context.LineTo(new Point(x0, y1), true, true);
        //        context.ArcTo(new Point(x1, y0), new Size(cornerRadius, cornerRadius), 90, false, SweepDirection.Clockwise, true, true);
        //    }
        //    g.Freeze();
        //    return g;
        //}

        private Geometry GetGeometry()
        {
            double width = ActualWidth - StrokeThickness;
            double height = ActualHeight - StrokeThickness;
            var g = new StreamGeometry();
            using (var context = g.Open())
            {
               double x0 = StrokeThickness / 2;
                 double x1 = width - x0;
                double y0 = StrokeThickness / 2;
                double y1 = height;
                context.BeginFigure(new Point(x0,y0+ ArrowHeight/2), true, true);
                context.LineTo(new Point(x0+ TargetPoint.X-ArrowWidth/2, y0+ArrowHeight / 2), true, true);
                context.LineTo(new Point(TargetPoint.X, TargetPoint.Y), true, true);
                context.LineTo(new Point(TargetPoint.X + ArrowWidth / 2, y0 + ArrowHeight / 2), true, true);
                context.LineTo(new Point(x1, y0 + ArrowHeight / 2), true, true);
                context.LineTo(new Point(x1, y1), true, true);
                 context.LineTo(new Point(x0, y1), true, true);
            }
            g.Freeze();
            return g;
        }



    }
}
