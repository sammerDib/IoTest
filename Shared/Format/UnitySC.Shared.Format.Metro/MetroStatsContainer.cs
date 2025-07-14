using System;
using System.Collections.Generic;
using System.Linq;

using UnitySC.Shared.Format.Base.Stats;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Format.Metro
{
    public class MetroStatsContainer : IStatsContainer
    {
        #region Implementation of IStatsContainer

        object IStatsContainer.Mean => Mean;

        object IStatsContainer.Min => Min;

        object IStatsContainer.Max => Max;

        object IStatsContainer.StdDev => StdDev;

        object IStatsContainer.Median => Median;

        object IStatsContainer.Delta => Delta;

        object IStatsContainer.Sigma3 => Sigma3;

        #endregion

        public static MetroStatsContainer Empty => new MetroStatsContainer(null, null, null, null, null, MeasureState.NotMeasured);

        private MetroStatsContainer(Length mean, Length min, Length max, Length stdDev, Length median, MeasureState state)
        {
            Mean   = mean;
            Min    = min;
            Max    = max;
            StdDev = stdDev;
            Median = median;
            State  = state;
        }

        public Length Mean { get; }

        public Length Min { get; }

        public Length Max { get; }

        public Length Delta => Max == null || Min == null ? null : Max - Min;

        public Length StdDev { get; }

        public Length Sigma3 => StdDev == null ? null : 3 * StdDev;

        public Length Median { get; }

        public MeasureState State { get; }
        
        public static MetroStatsContainer GenerateFromLength(ICollection<Tuple<Length, MeasureState>> data)
        {
            if (data.Count == 0)
                throw new ArgumentException($"{nameof(MetroStatsContainer)} - {nameof(GenerateFromLength)} : the provided data could not be empty for statistic calculation.");

            var sortedPointList = data.
                Where(tupe => tupe.Item2 != MeasureState.NotMeasured && (!(tupe.Item1 is null)) && !double.IsNaN(tupe.Item1.Value)).
                OrderBy(tuple => tuple.Item1).
                ToList();

            if (sortedPointList.Count == 0)
            {
                return new MetroStatsContainer(null, null, null, null, null, MeasureState.NotMeasured);
            }

            var mean = new Length(0, LengthUnit.Micrometer);
            Length min = null;
            Length max = null;

            double squaredSum = 0d;

            bool successAtAllPoints = true;
            bool errorAtAllPoints = true;

            foreach (var (length, state) in sortedPointList)
            {
                if (state != MeasureState.Success)
                    successAtAllPoints = false;
                if (state != MeasureState.Error)
                    errorAtAllPoints = false;

                if (length != null)
                {
                    mean += length;
                    squaredSum += length.Micrometers * length.Micrometers;

                    if (min == null || length < min)
                        min = length;

                    if (max == null || length > max)
                        max = length;
                }
            }

            mean /= sortedPointList.Count;
            squaredSum /= sortedPointList.Count;

            double sum = Math.Abs(squaredSum - mean.Micrometers * mean.Micrometers);
            double sqrt = Math.Sqrt(sum);
            var stdDev = new Length(sqrt, LengthUnit.Micrometer);

            Length median;
            if (sortedPointList.Count % 2 == 0)
                median = (sortedPointList[sortedPointList.Count / 2 - 1].Item1 + sortedPointList[sortedPointList.Count / 2].Item1) / 2;
            else
                median = sortedPointList[sortedPointList.Count / 2].Item1;

            MeasureState finalState;

            // If any not measured
            if (sortedPointList.Count != data.Count)
                finalState = MeasureState.NotMeasured;
            else if (successAtAllPoints)
                finalState = MeasureState.Success;
            else if (errorAtAllPoints)
                finalState = MeasureState.Error;
            else
                finalState = MeasureState.Partial;

            return new MetroStatsContainer(mean, min, max, stdDev, median, finalState);
        }

        public static MetroStatsContainer GenerateFromStats(ICollection<MetroStatsContainer> data)
        {
            if (data.Count == 0)
                throw new ArgumentException($"{nameof(MetroStatsContainer)} - {nameof(GenerateFromStats)} : the provided data could not be empty for statistic calculation.");

            var measuredData = data.Where(stats => stats != null && stats.State != MeasureState.NotMeasured).ToList();

            var sortedMeans = measuredData.
                Select(stats => stats.Mean).
                OrderBy(length => length).
                ToList();

            if (sortedMeans.Count == 0)
            {
                return new MetroStatsContainer(null, null, null, null, null, MeasureState.NotMeasured);
            }

            var mean = new Length(0, LengthUnit.Micrometer);
            var min = new Length(double.PositiveInfinity, LengthUnit.Micrometer);
            var max = new Length(double.NegativeInfinity, LengthUnit.Micrometer);

            double squaredSum = 0d;
            bool successAtAllPoints = true;
            bool errorAtAllPoints = true;

            foreach (var statsContainer in measuredData)
            {
                if (statsContainer.State != MeasureState.Success)
                    successAtAllPoints = false;
                if (statsContainer.State != MeasureState.Error)
                    errorAtAllPoints = false;

                var value = statsContainer.Mean;

                mean += value;
                squaredSum += value.Micrometers * value.Micrometers;

                if (value < min)
                    min = value;

                if (value > max)
                    max = value;
            }

            mean /= measuredData.Count;
            squaredSum /= measuredData.Count;

            double sum = Math.Abs(squaredSum - mean.Micrometers * mean.Micrometers);
            double sqrt = Math.Sqrt(sum);
            var stdDev = new Length(sqrt, LengthUnit.Micrometer);

            Length median;
            if (measuredData.Count % 2 == 0)
                median = (sortedMeans[measuredData.Count / 2 - 1] + sortedMeans[measuredData.Count / 2]) / 2;
            else
                median = sortedMeans[measuredData.Count / 2];

            MeasureState finalState;

            // If any not measured
            if (measuredData.Count != data.Count)
                finalState = MeasureState.NotMeasured;
            else if (successAtAllPoints)
                finalState = MeasureState.Success;
            else if (errorAtAllPoints)
                finalState = MeasureState.Error;
            else
                finalState = MeasureState.Partial;

            return new MetroStatsContainer(mean, min, max, stdDev, median, finalState);
        }
    }
}
