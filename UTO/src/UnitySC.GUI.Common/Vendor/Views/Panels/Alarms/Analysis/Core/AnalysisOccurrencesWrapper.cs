using System;
using System.Collections.Generic;
using System.Linq;

using Agileo.AlarmModeling;
using Agileo.GUI.Components;

using OxyPlot;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Alarms.Analysis.Core
{
    public class AnalysisOccurrencesWrapper : Notifier
    {
        public Alarm Alarm { get; set; }

        public int OccurrenceCount => Occurrences.Count;

        public double OccurrenceCountRatio { get; set; }

        public TimeSpan OccurrenceDuration { get; set; } = TimeSpan.Zero;

        public double OccurrenceDurationRatio { get; set; }

        public List<AlarmOccurrence> Occurrences { get; } = new();
        public OxyColor Color { get; set; }
        public int SortedIndex { get; set; }

        public AnalysisOccurrencesWrapper(Alarm alarm)
        {
            Alarm = alarm;
        }

        public int GetOccurrenceCount(DateTime startTime, DateTime endTime)
        {
            return Occurrences.Count(occurrence => occurrence.SetTimeStamp >= startTime && occurrence.SetTimeStamp < endTime);
        }

        public TimeSpan GetOccurrenceDuration(DateTime startTime, DateTime endTime)
        {
            TimeSpan duration = TimeSpan.Zero;

            foreach (var occurrence in Occurrences)
            {
                DateTime maxStartTime = startTime > occurrence.SetTimeStamp ? startTime : occurrence.SetTimeStamp;
                DateTime maxEndTime;

                if (occurrence.ClearedTimeStamp.HasValue)
                {
                    maxEndTime = endTime > occurrence.ClearedTimeStamp ? occurrence.ClearedTimeStamp.Value : endTime;
                }
                else
                {
                    maxEndTime = endTime > DateTime.Now ? DateTime.Now : endTime;
                }

                duration += maxEndTime - maxStartTime;
            }

            return duration;
        }

        private bool _isPartOf80PercentOfImpact;

        public bool IsPartOf80PercentOfImpact
        {
            get => _isPartOf80PercentOfImpact;
            set => SetAndRaiseIfChanged(ref _isPartOf80PercentOfImpact, value);
        }

        public int ImpactingPosition { get; set; }
    }
}
