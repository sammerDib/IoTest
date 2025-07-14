using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using LiveCharts;
using LiveCharts.Charts;
using LiveCharts.Definitions.Points;
using LiveCharts.Wpf;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.SubstrateTracking.CustomChart
{
    public class CustomStepLinePointView : CustomPointView, IStepPointView
    {
        public double DeltaX { get; set; }

        public double DeltaY { get; set; }

        public Line Line1 { get; set; }

        public Line Line2 { get; set; }

        public Path Shape { get; set; }

        public override void DrawOrMove(
            ChartPoint previousDrawn,
            ChartPoint current,
            int index,
            ChartCore chart)
        {
            var invertedMode = ((StepLineSeries)current.SeriesView).InvertedMode;
            if (IsNew)
            {
                if (invertedMode)
                {
                    Line1.X1 = current.ChartLocation.X;
                    Line1.X2 = current.ChartLocation.X - DeltaX;
                    Line1.Y1 = chart.DrawMargin.Height;
                    Line1.Y2 = chart.DrawMargin.Height;
                    Line2.X1 = current.ChartLocation.X - DeltaX;
                    Line2.X2 = current.ChartLocation.X - DeltaX;
                    Line2.Y1 = chart.DrawMargin.Height;
                    Line2.Y2 = chart.DrawMargin.Height;
                }
                else
                {
                    Line1.X1 = current.ChartLocation.X;
                    Line1.X2 = current.ChartLocation.X;
                    Line1.Y1 = chart.DrawMargin.Height;
                    Line1.Y2 = chart.DrawMargin.Height;
                    Line2.X1 = current.ChartLocation.X - DeltaX;
                    Line2.X2 = current.ChartLocation.X;
                    Line2.Y1 = chart.DrawMargin.Height;
                    Line2.Y2 = chart.DrawMargin.Height;
                }

                if (Shape != null)
                {
                    Canvas.SetLeft(Shape, current.ChartLocation.X - (Shape.Width / 2.0));
                    Canvas.SetTop(Shape, chart.DrawMargin.Height);
                }
            }

            if (DataLabel != null && double.IsNaN(Canvas.GetLeft(DataLabel)))
            {
                Canvas.SetTop(DataLabel, chart.DrawMargin.Height);
                Canvas.SetLeft(DataLabel, current.ChartLocation.X);
            }

            if (HoverShape != null)
            {
                if (Shape != null)
                {
                    if (Shape.Width > 5.0)
                    {
                        HoverShape.Width = Shape.Width;
                    }
                    else
                    {
                        HoverShape.Width = 5.0;
                    }
                }
                else
                {
                    HoverShape.Width = 5.0;
                }

                if (Shape != null)
                {
                    if (Shape.Height > 5.0)
                    {
                        HoverShape.Height = Shape.Height;
                    }
                    else
                    {
                        HoverShape.Height = 5.0;
                    }
                }
                else
                {
                    HoverShape.Height = 5.0;
                }

                Canvas.SetLeft(HoverShape, current.ChartLocation.X - (HoverShape.Width / 2.0));
                Canvas.SetTop(HoverShape, current.ChartLocation.Y - (HoverShape.Height / 2.0));
            }

            if (chart.View.DisableAnimations)
            {
                if (invertedMode)
                {
                    Line1.X1 = current.ChartLocation.X;
                    Line1.X2 = current.ChartLocation.X - DeltaX;
                    Line1.Y1 = current.ChartLocation.Y;
                    Line1.Y2 = current.ChartLocation.Y;
                    Line2.X1 = current.ChartLocation.X - DeltaX;
                    Line2.X2 = current.ChartLocation.X - DeltaX;
                    Line2.Y1 = current.ChartLocation.Y;
                    Line2.Y2 = current.ChartLocation.Y - DeltaY;
                }
                else
                {
                    Line1.X1 = current.ChartLocation.X;
                    Line1.X2 = current.ChartLocation.X;
                    Line1.Y1 = current.ChartLocation.Y;
                    Line1.Y2 = current.ChartLocation.Y - DeltaY;
                    Line2.X1 = current.ChartLocation.X - DeltaX;
                    Line2.X2 = current.ChartLocation.X;
                    Line2.Y1 = current.ChartLocation.Y - DeltaY;
                    Line2.Y2 = current.ChartLocation.Y - DeltaY;
                }

                if (Shape != null)
                {
                    Canvas.SetLeft(Shape, current.ChartLocation.X - (Shape.Width / 2.0));
                    Canvas.SetTop(Shape, current.ChartLocation.Y - (Shape.Height / 2.0));
                }

                if (DataLabel == null)
                {
                    return;
                }

                DataLabel.UpdateLayout();
                var length1 = CorrectXLabel(
                    current.ChartLocation.X - (DataLabel.ActualWidth * 0.5),
                    chart);
                var length2 = CorrectYLabel(
                    current.ChartLocation.Y - (DataLabel.ActualHeight * 0.5),
                    chart);
                Canvas.SetLeft(DataLabel, length1);
                Canvas.SetTop(DataLabel, length2);
            }
            else
            {
                var animationsSpeed = chart.View.AnimationsSpeed;
                if (invertedMode)
                {
                    Line1.BeginAnimation(
                        Line.X1Property,
                        new DoubleAnimation(current.ChartLocation.X, animationsSpeed));
                    Line1.BeginAnimation(
                        Line.X2Property,
                        new DoubleAnimation(current.ChartLocation.X - DeltaX, animationsSpeed));
                    Line1.BeginAnimation(
                        Line.Y1Property,
                        new DoubleAnimation(current.ChartLocation.Y, animationsSpeed));
                    Line1.BeginAnimation(
                        Line.Y2Property,
                        new DoubleAnimation(current.ChartLocation.Y, animationsSpeed));
                    Line2.BeginAnimation(
                        Line.X1Property,
                        new DoubleAnimation(current.ChartLocation.X - DeltaX, animationsSpeed));
                    Line2.BeginAnimation(
                        Line.X2Property,
                        new DoubleAnimation(current.ChartLocation.X - DeltaX, animationsSpeed));
                    Line2.BeginAnimation(
                        Line.Y1Property,
                        new DoubleAnimation(current.ChartLocation.Y, animationsSpeed));
                    Line2.BeginAnimation(
                        Line.Y2Property,
                        new DoubleAnimation(current.ChartLocation.Y - DeltaY, animationsSpeed));
                }
                else
                {
                    Line1.BeginAnimation(
                        Line.X1Property,
                        new DoubleAnimation(current.ChartLocation.X, animationsSpeed));
                    Line1.BeginAnimation(
                        Line.X2Property,
                        new DoubleAnimation(current.ChartLocation.X, animationsSpeed));
                    Line1.BeginAnimation(
                        Line.Y1Property,
                        new DoubleAnimation(current.ChartLocation.Y, animationsSpeed));
                    Line1.BeginAnimation(
                        Line.Y2Property,
                        new DoubleAnimation(current.ChartLocation.Y - DeltaY, animationsSpeed));
                    Line2.BeginAnimation(
                        Line.X1Property,
                        new DoubleAnimation(current.ChartLocation.X - DeltaX, animationsSpeed));
                    Line2.BeginAnimation(
                        Line.X2Property,
                        new DoubleAnimation(current.ChartLocation.X, animationsSpeed));
                    Line2.BeginAnimation(
                        Line.Y1Property,
                        new DoubleAnimation(current.ChartLocation.Y - DeltaY, animationsSpeed));
                    Line2.BeginAnimation(
                        Line.Y2Property,
                        new DoubleAnimation(current.ChartLocation.Y - DeltaY, animationsSpeed));
                }

                if (Shape != null)
                {
                    Shape.BeginAnimation(
                        Canvas.LeftProperty,
                        new DoubleAnimation(
                            current.ChartLocation.X - (Shape.Width / 2.0),
                            animationsSpeed));
                    Shape.BeginAnimation(
                        Canvas.TopProperty,
                        new DoubleAnimation(
                            current.ChartLocation.Y - (Shape.Height / 2.0),
                            animationsSpeed));
                }

                if (DataLabel == null)
                {
                    return;
                }

                DataLabel.UpdateLayout();
                var length3 = CorrectXLabel(
                    current.ChartLocation.X - (DataLabel.ActualWidth * 0.5),
                    chart);
                var length4 = CorrectYLabel(
                    current.ChartLocation.Y - (DataLabel.ActualHeight * 0.5),
                    chart);
                Canvas.SetLeft(DataLabel, length3);
                Canvas.SetTop(DataLabel, length4);
            }
        }

        public override void RemoveFromView(ChartCore chart)
        {
            chart.View.RemoveFromDrawMargin(HoverShape);
            chart.View.RemoveFromDrawMargin(Shape);
            chart.View.RemoveFromDrawMargin(DataLabel);
            chart.View.RemoveFromDrawMargin(Line1);
            chart.View.RemoveFromDrawMargin(Line2);
        }

        public override void OnHover(ChartPoint point)
        {
            var seriesView = (StepLineSeries)point.SeriesView;
            if (Shape != null)
            {
                Shape.Fill = Shape.Stroke;
            }

            ++seriesView.StrokeThickness;
        }

        public override void OnHoverLeave(ChartPoint point)
        {
            var seriesView = (StepLineSeries)point.SeriesView;
            if (Shape != null)
            {
                Shape.Fill = point.Fill == null
                    ? seriesView.PointForeground
                    : (Brush)point.Fill;
            }

            --seriesView.StrokeThickness;
        }

        protected double CorrectXLabel(double desiredPosition, ChartCore chart)
        {
            if (desiredPosition + (DataLabel.ActualWidth * 0.5) < -0.1)
            {
                return -DataLabel.ActualWidth;
            }

            if (desiredPosition + DataLabel.ActualWidth > chart.DrawMargin.Width)
            {
                desiredPosition -=
                    desiredPosition + DataLabel.ActualWidth - chart.DrawMargin.Width + 2.0;
            }

            if (desiredPosition < 0.0)
            {
                desiredPosition = 0.0;
            }

            return desiredPosition;
        }

        protected double CorrectYLabel(double desiredPosition, ChartCore chart)
        {
            desiredPosition -= (Shape == null
                                   ? 0.0
                                   : Shape.ActualHeight * 0.5)
                               + (DataLabel.ActualHeight * 0.5)
                               + 2.0;
            if (desiredPosition + DataLabel.ActualHeight > chart.DrawMargin.Height)
            {
                desiredPosition -= desiredPosition
                                   + DataLabel.ActualHeight
                                   - chart.DrawMargin.Height
                                   + 2.0;
            }

            if (desiredPosition < 0.0)
            {
                desiredPosition = 0.0;
            }

            return desiredPosition;
        }
    }
}
