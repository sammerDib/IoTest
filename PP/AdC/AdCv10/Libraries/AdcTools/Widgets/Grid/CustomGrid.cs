using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace AdcTools
{
    ///////////////////////////////////////////////////////////////////////
    // Une Grid qui peut tracer la grille
    ///////////////////////////////////////////////////////////////////////
    public class CustomGrid : System.Windows.Controls.Grid
    {
        //=================================================================
        // DependencyProperty
        //=================================================================

        // DrawHorizontalLines
        //....................
        public static DependencyProperty DrawHorizontalLinesProperty = DependencyProperty.Register("DrawHorizontalLines", typeof(Boolean), typeof(Grid));
        public Boolean DrawHorizontalLines { get { return (Boolean)GetValue(DrawHorizontalLinesProperty); } set { SetValue(DrawHorizontalLinesProperty, value); } }

        // DrawVerticalLines
        //..................
        public static DependencyProperty DrawVerticalLinesProperty = DependencyProperty.Register("DrawVerticalLines", typeof(Boolean), typeof(Grid));
        public Boolean DrawVerticalLines { get { return (Boolean)GetValue(DrawVerticalLinesProperty); } set { SetValue(DrawVerticalLinesProperty, value); } }


        //=================================================================
        // OnRender
        //=================================================================
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            List<System.Windows.Controls.RowDefinition> rowDefinitions = RowDefinitions.ToList();
            List<System.Windows.Controls.ColumnDefinition> columnDefinitions = ColumnDefinitions.ToList();

            Pen pen = new Pen(Brushes.Black, 1.0);

            if (DrawVerticalLines)
            {
                foreach (ColumnDefinition columnDefinition in columnDefinitions)
                    dc.DrawLine(pen, new Point(columnDefinition.Offset, 0), new Point(columnDefinition.Offset, ActualHeight));

                var lastcol = columnDefinitions.Last();
                double lastline = lastcol.Offset + lastcol.ActualWidth;
                dc.DrawLine(pen, new Point(lastline, 0), new Point(lastline, ActualHeight));
            }

            if (DrawHorizontalLines)
            {
                foreach (RowDefinition rowDefinition in rowDefinitions)
                    dc.DrawLine(pen, new Point(0, rowDefinition.Offset), new Point(ActualWidth, rowDefinition.Offset));

                var lastrow = rowDefinitions.Last();
                double lastline = lastrow.Offset + lastrow.ActualHeight;
                dc.DrawLine(pen, new Point(0, lastline), new Point(ActualWidth, lastline));
            }
        }

        //=================================================================
        // 
        //=================================================================
        public IList<AdcTools.RowDefinition> AdcRowDefinitions
        {
            get
            {
                List<AdcTools.RowDefinition> list = new List<RowDefinition>();

                foreach (System.Windows.Controls.RowDefinition row in RowDefinitions)
                    list.Add(row as AdcTools.RowDefinition);

                return list;
            }
        }

        public IList<AdcTools.ColumnDefinition> AdcColumnDefinitions
        {
            get
            {
                List<AdcTools.ColumnDefinition> list = new List<ColumnDefinition>();
                foreach (System.Windows.Controls.ColumnDefinition col in ColumnDefinitions)
                    list.Add(col as AdcTools.ColumnDefinition);
                return list;
            }
        }

    }
}
