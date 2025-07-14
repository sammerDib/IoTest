using OxyPlot.Series;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Alarms.Analysis.OxyPlotExtended
{
    /// <summary>
    /// OxyPlot PieSlice with custom Tag object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TaggedPieSlice<T> : PieSlice
    {
        public T Tag { get; set; }

        public TaggedPieSlice(string label, double value) : base(label, value)
        {
        }
    }
}
