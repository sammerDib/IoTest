using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Alarms.Analysis.Core
{
    public class AnalysisDeviceOccurrencesWrapper
    {
        public AnalysisDeviceOccurrencesWrapper(string deviceName)
        {
            DeviceName = deviceName;
        }

        public string DeviceName { get; }

        public List<AnalysisOccurrencesWrapper> Alarms { get; set; }

        public Color Color { get; set; }

        public int TotalOccurrencesCount => Alarms.Sum(wrapper => wrapper.OccurrenceCount);

        public double TotalOccurrencesCountRatio => Math.Round(Alarms.Sum(wrapper => wrapper.OccurrenceCountRatio), MidpointRounding.AwayFromZero);

        public TimeSpan TotalOccurrenceDuration
        {
            get
            {
                var totalOccurrenceDuration = TimeSpan.Zero;
                Alarms.ForEach(wrapper => totalOccurrenceDuration += wrapper.OccurrenceDuration);
                return totalOccurrenceDuration;
            }
        }

        public double TotalOccurrenceDurationRatio => Math.Round(Alarms.Sum(wrapper => wrapper.OccurrenceDurationRatio), MidpointRounding.AwayFromZero);

        public int GetOccurrenceCount(DateTime startTime, DateTime endTime) => Alarms.Sum(wrapper => wrapper.GetOccurrenceCount(startTime, endTime));

        public TimeSpan GetOccurrenceDuration(DateTime startTime, DateTime endTime)
        {
            var duration = TimeSpan.Zero;
            Alarms.ForEach(wrapper => duration += wrapper.GetOccurrenceDuration(startTime, endTime));
            return duration;
        }
    }
}
