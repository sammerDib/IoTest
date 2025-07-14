using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

using LiveCharts;
using LiveCharts.Definitions.Points;
using LiveCharts.SeriesAlgorithms;
using LiveCharts.Wpf;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.SubstrateTracking.CustomChart
{
    public class CustomStepLineSeries : StepLineSeries
    {
        public CustomStepLineSeries(object configuration):base()
        {
            Configuration = configuration;
        }

        public CustomStepLineSeries()
        {
            
        }

        public static readonly DependencyProperty AlternativeStrokeThicknessProperty =
            DependencyProperty.Register(
                nameof(AlternativeStrokeThickness),
                typeof(double),
                typeof(CustomStepLineSeries),
                new PropertyMetadata(0.0, CallChartUpdater()));
        public static readonly DependencyProperty IsAlternativeStrokeThicknessProperty =
            DependencyProperty.Register(
                nameof(IsAlternativeStrokeThickness),
                typeof(bool),
                typeof(CustomStepLineSeries),
                new PropertyMetadata(false, CallChartUpdater()));

        public double AlternativeStrokeThickness
        {
            get => (double)GetValue(AlternativeStrokeThicknessProperty);
            set => SetValue(AlternativeStrokeThicknessProperty, value);
        }

        public bool IsAlternativeStrokeThickness
        {
            get => (bool)GetValue(IsAlternativeStrokeThicknessProperty);
            set => SetValue(IsAlternativeStrokeThicknessProperty, value);
        }

        /// <summary>
        /// Gets the view of a given point
        /// </summary>
        /// <param name="point"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public override IChartPointView GetPointView(ChartPoint point, string label)
        {
            var pbv = (CustomStepLinePointView)point.View;

            if (pbv == null)
            {
                pbv = new CustomStepLinePointView
                {
                    IsNew = true, Line2 = new Line(), Line1 = new Line()
                };

                Model.Chart.View.AddToDrawMargin(pbv.Line2);
                Model.Chart.View.AddToDrawMargin(pbv.Line1);
                Model.Chart.View.AddToDrawMargin(pbv.Shape);
            }
            else
            {
                pbv.IsNew = false;
                point.SeriesView.Model.Chart.View
                    .EnsureElementBelongsToCurrentDrawMargin(pbv.Shape);
                point.SeriesView.Model.Chart.View.EnsureElementBelongsToCurrentDrawMargin(
                    pbv.HoverShape);
                point.SeriesView.Model.Chart.View.EnsureElementBelongsToCurrentDrawMargin(
                    pbv.DataLabel);
                point.SeriesView.Model.Chart.View
                    .EnsureElementBelongsToCurrentDrawMargin(pbv.Line2);
                point.SeriesView.Model.Chart.View
                    .EnsureElementBelongsToCurrentDrawMargin(pbv.Line1);
            }

            pbv.Line1.StrokeThickness = StrokeThickness;
            pbv.Line1.Stroke = AlternativeStroke;
            pbv.Line1.StrokeDashArray = StrokeDashArray;
            pbv.Line1.Visibility = Visibility;
            Panel.SetZIndex(pbv.Line1, Panel.GetZIndex(this));

            if (IsAlternativeStrokeThickness)
            {
                pbv.Line2.StrokeThickness = AlternativeStrokeThickness;
            }
            else
            {
                pbv.Line2.StrokeThickness = StrokeThickness;
            }
            
            pbv.Line2.Stroke = Stroke;
            pbv.Line2.StrokeDashArray = StrokeDashArray;
            pbv.Line2.Visibility = Visibility;
            if (point.Stroke != null)
            {
                pbv.Line2.Stroke = (Brush)point.Stroke;
                pbv.Line2.Fill = (Brush)point.Stroke;
            }
            Panel.SetZIndex(pbv.Line2, Panel.GetZIndex(this));

            if (PointGeometry != null && Math.Abs(PointGeometrySize) > 0.1 && pbv.Shape == null)
            {
                if (PointGeometry != null)
                {
                    pbv.Shape = new Path
                    {
                        Stretch = Stretch.Fill, StrokeThickness = StrokeThickness
                    };
                }

                Model.Chart.View.AddToDrawMargin(pbv.Shape);
            }

            if (pbv.Shape != null)
            {
                pbv.Shape.Fill = PointForeground;
                pbv.Shape.StrokeThickness = StrokeThickness;
                pbv.Shape.Stroke = Stroke;
                pbv.Shape.StrokeDashArray = StrokeDashArray;
                pbv.Shape.Visibility = Visibility;
                pbv.Shape.Width = PointGeometrySize;
                pbv.Shape.Height = PointGeometrySize;
                pbv.Shape.Data = PointGeometry;
                Panel.SetZIndex(pbv.Shape, Panel.GetZIndex(this) + 1);

                if (point.Stroke != null)
                {
                    pbv.Shape.Stroke = (Brush)point.Stroke;
                }

                if (point.Fill != null)
                {
                    pbv.Shape.Fill = (Brush)point.Fill;
                }
            }

            if (Model.Chart.RequiresHoverShape && pbv.HoverShape == null)
            {
                pbv.HoverShape = new Rectangle {Fill = Brushes.Transparent, StrokeThickness = 0};

                Panel.SetZIndex(pbv.HoverShape, int.MaxValue);
                
                Model.Chart.View.AddToDrawMargin(pbv.HoverShape);
            }

            if (pbv.HoverShape != null)
            {
                pbv.HoverShape.Visibility = Visibility;
            }

            if (DataLabels)
            {
                pbv.DataLabel = UpdateLabelContent(
                    new DataLabelViewModel
                    {
                        FormattedText = label

                        //Point = point
                    },
                    pbv.DataLabel);
            }

            if (!DataLabels && pbv.DataLabel != null)
            {
                Model.Chart.View.RemoveFromDrawMargin(pbv.DataLabel);
                pbv.DataLabel = null;
            }

            return pbv;
        }

        internal ContentControl UpdateLabelContent(
            DataLabelViewModel content,
            ContentControl currentControl)
        {
            ContentControl control;

            if (currentControl == null)
            {
                control = new ContentControl();
                control.SetBinding(
                    VisibilityProperty,
                    new Binding {Path = new PropertyPath(VisibilityProperty), Source = this});
                Panel.SetZIndex(control, int.MaxValue - 1);

                Model.Chart.View.AddToDrawMargin(control);
            }
            else
            {
                control = currentControl;
            }

            control.Content = content;
            control.ContentTemplate = DataLabelsTemplate;
            control.FontFamily = FontFamily;
            control.FontSize = FontSize;
            control.FontStretch = FontStretch;
            control.FontStyle = FontStyle;
            control.FontWeight = FontWeight;
            control.Foreground = Foreground;

            return control;
        }
    }
}
