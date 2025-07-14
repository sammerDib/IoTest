using System;
using System.Collections.Generic;
using System.Linq;

using UnitySC.Shared.Format.Base.Stats;

namespace UnitySC.Shared.Format.Metro
{
    public class MetroDoubleStatsContainer : IStatsContainer
    {
        #region Implementation of IStatsContainer

        object IStatsContainer.Mean => Mean;

        object IStatsContainer.Min => Min;

        object IStatsContainer.Max => Max;

        object IStatsContainer.StdDev => StdDev;

        object IStatsContainer.Median => Median;

        object IStatsContainer.Delta => Delta;

        object IStatsContainer.Sigma3 => Sigma3;

        #endregion Implementation of IStatsContainer

        private MetroDoubleStatsContainer(double mean, double min, double max, double stdDev, double median, MeasureState state, string unit)
        {
            Mean = mean;
            Min = min;
            Max = max;
            StdDev = stdDev;
            Median = median;
            State = state;
            Unit = unit;
        }

        public double Mean { get; }

        public double Min { get; }

        public double Max { get; }

        public double Delta => Max - Min;

        public double StdDev { get; }

        public double Sigma3 => 3 * StdDev;

        public double Median { get; }

        public string Unit { get; }
        public MeasureState State { get; }

        public static MetroDoubleStatsContainer GenerateFromDoubles(ICollection<Tuple<double, MeasureState>> data, string unit)
        {
            if (data.Count == 0) throw new ArgumentException($"{nameof(MetroDoubleStatsContainer)} - {nameof(GenerateFromDoubles)} : the provided data could not be empty for statistic calculation.");

            var sortedPointList = data.
                Where(tupe => tupe.Item2 != MeasureState.NotMeasured && !double.IsNaN(tupe.Item1)).
                OrderBy(tuple => tuple.Item1).
                ToList();

            if (sortedPointList.Count == 0)
            {
                return new MetroDoubleStatsContainer(double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, MeasureState.NotMeasured, unit);
            }

            double mean = 0.0;
            double min = double.PositiveInfinity;
            double max = double.NegativeInfinity;

            double squaredSum = 0d;

            bool successAtAllPoints = true;
            bool errorAtAllPoints = true;

            foreach ((double value, var state) in sortedPointList)
            {
                if (state != MeasureState.Success)
                    successAtAllPoints = false;
                if (state != MeasureState.Error)
                    errorAtAllPoints = false;

                mean += value;
                squaredSum += value * value;

                if (value < min)
                    min = value;

                if (value > max)
                    max = value;
            }

            mean /= (double)sortedPointList.Count;
            squaredSum /= (double)sortedPointList.Count;

            double stdDev = Math.Sqrt(Math.Abs(squaredSum - mean * mean));

            double median;
            if (sortedPointList.Count % 2 == 0)
                median = (sortedPointList[sortedPointList.Count / 2 - 1].Item1 + sortedPointList[sortedPointList.Count / 2].Item1) * 0.5;
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

            return new MetroDoubleStatsContainer(mean, min, max, stdDev, median, finalState, unit);
        }

        public static MetroDoubleStatsContainer GenerateFromStats(ICollection<MetroDoubleStatsContainer> data)
        {
            if (data.Count == 0) throw new ArgumentException($"{nameof(MetroDoubleStatsContainer)} - {nameof(GenerateFromStats)} : the provided data could not be empty for statistic calculation.");

            var measuredData = data.Where(stats => stats.State != MeasureState.NotMeasured).ToList();

            var sortedMeans = measuredData.
                Select(stats => stats.Mean).
                OrderBy(length => length).
                ToList();

            if (sortedMeans.Count == 0)
            {
                return new MetroDoubleStatsContainer(0.0, 0.0, 0.0, 0.0, 0.0, MeasureState.NotMeasured, data.Count > 0 ? data.First().Unit : string.Empty);
            }

            double mean = 0.0;
            double min = double.PositiveInfinity;
            double max = double.NegativeInfinity;

            double squaredSum = 0d;
            bool successAtAllPoints = true;
            bool errorAtAllPoints = true;

            foreach (var statsContainer in measuredData)
            {
                if (statsContainer.State != MeasureState.Success)
                    successAtAllPoints = false;
                if (statsContainer.State != MeasureState.Error)
                    errorAtAllPoints = false;

                double value = statsContainer.Mean;

                mean += value;
                squaredSum += value * value;

                if (value < min)
                    min = value;

                if (value > max)
                    max = value;
            }

            mean /= measuredData.Count;
            squaredSum /= measuredData.Count;

            double stdDev = Math.Sqrt(Math.Abs(squaredSum - mean * mean));

            double median;
            if (measuredData.Count % 2 == 0)
                median = (sortedMeans[measuredData.Count / 2 - 1] + sortedMeans[measuredData.Count / 2]) * 0.5;
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

            return new MetroDoubleStatsContainer(mean, min, max, stdDev, median, finalState, data.Count > 0 ? data.First().Unit : string.Empty);
        }
    }
}
