using System;
using System.Windows.Controls;
using System.Windows.Shapes;

using LiveCharts;
using LiveCharts.Charts;
using LiveCharts.Dtos;

namespace UnitySC.UTO.Controller.Views.Panels.Gem.SubstrateTracking.CustomChart
{
    public class CustomPointView
    {
        public Shape HoverShape { get; set; }

        public ContentControl DataLabel { get; set; }

        public bool IsNew { get; set; }

        public CoreRectangle ValidArea { get; internal set; }

        public virtual void DrawOrMove(
            ChartPoint previousDrawn,
            ChartPoint current,
            int index,
            ChartCore chart)
        {
            throw new NotImplementedException();
        }

        public virtual void RemoveFromView(ChartCore chart)
        {
            throw new NotImplementedException();
        }

        public virtual void OnHover(ChartPoint point)
        {
            throw new NotImplementedException();
        }

        public virtual void OnHoverLeave(ChartPoint point)
        {
            throw new NotImplementedException();
        }
    }
}
