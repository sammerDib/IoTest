using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitySC.Shared.Format.Base.Stats
{
    public class FloatStatsContainer : IStatsContainer
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

        public static FloatStatsContainer Empty => new FloatStatsContainer(float.NaN, float.NaN, float.NaN, float.NaN, float.NaN);

        public FloatStatsContainer(float mean, float min, float max, float stdDev, float median)
        {
            Mean = mean;
            Min = min;
            Max = max;
            StdDev = stdDev;
            Median = median;
        }

        public float Mean { get; }

        public float Min { get; }

        public float Max { get; }

        public float Delta => Max - Min;

        public float StdDev { get; }

        public float Sigma3 => 3 * StdDev;

        public float Median { get; }

        public static FloatStatsContainer GenerateFromFloatsKeepSortedData(ICollection<float> data, out List<float> sortedPointList, bool excludePureZero = false)
        {
            sortedPointList = null;

            if (data.Count == 0) 
                throw new ArgumentException($"{nameof(FloatStatsContainer)} - {nameof(GenerateFromFloats)} : the provided data could not be empty for statistic calculation.");

            if (data.Count <= 1024 * 512)
            {
                if(excludePureZero)
                    sortedPointList = data.Where(f => !float.IsNaN(f) && f != 0.0f).OrderBy(d => d).ToList();
                else
                    sortedPointList = data.Where(f => !float.IsNaN(f)).OrderBy(d => d).ToList();
            }
            else
            {
                if (excludePureZero)
                    sortedPointList = data.AsParallel().Where(f => !float.IsNaN(f) && f != 0.0f).OrderBy(d => d).ToList();
                else
                    sortedPointList = data.AsParallel().Where(f => !float.IsNaN(f)).OrderBy(d => d).ToList();
            }

            if (sortedPointList.Count == 0)
            {
                return new FloatStatsContainer(0.0f, -0.000001f, 0.000001f, 0.000001f, 0.0f);
            }

            double mean = 0.0;
            double squaredSum = 0.0; // en double car on risque des overflow sur les grands amas de donnée et docn un perte en precision conséquente
            float min = float.PositiveInfinity;
            float max = float.NegativeInfinity;

            foreach (float value in sortedPointList)
            {
                double dVal = (double)value;
                mean += dVal;
                squaredSum += dVal * dVal;

                if (value < min)
                    min = value;

                if (value > max)
                    max = value;
            }

            mean /= sortedPointList.Count;
            squaredSum /= sortedPointList.Count;

            float stdDev = (float)Math.Sqrt(Math.Abs(squaredSum - mean * mean));

            float median;
            if (sortedPointList.Count % 2 == 0)
                median = (sortedPointList[sortedPointList.Count / 2 - 1] + sortedPointList[sortedPointList.Count / 2]) * 0.5f;
            else
                median = sortedPointList[sortedPointList.Count / 2];

            return new FloatStatsContainer((float)mean, min, max, stdDev, median);
        }


        public static FloatStatsContainer GenerateFromFloats(ICollection<float> data)
        {
            return GenerateFromFloatsKeepSortedData(data, out List<float> sortedDatalist);
        }

        public static FloatStatsContainer GenerateFromStats(ICollection<FloatStatsContainer> data)
        {
            if (data.Count == 0)
                throw new ArgumentException($"{nameof(FloatStatsContainer)} - {nameof(GenerateFromStats)} : the provided data could not be empty for statistic calculation.");

            var measuredData = data.ToList();

            var sortedMeans = measuredData.
                Select(stats => stats.Mean).
                OrderBy(length => length).
                ToList();

            if (sortedMeans.Count == 0)
            {
                return new FloatStatsContainer(0.0f, 0.0f, 0.0f, 0.0f, 0.0f);
            }

            double mean = 0.0;
            double squaredSum = 0.0;
            float min = float.PositiveInfinity;
            float max = float.NegativeInfinity;

            foreach (var statsContainer in measuredData)
            {
                float value = statsContainer.Mean;
                double dVal = (double)value;
                mean += dVal;
                squaredSum += dVal * dVal;

                if (value < min)
                    min = value;

                if (value > max)
                    max = value;
            }

            mean /= (double)measuredData.Count;
            squaredSum /= (double)measuredData.Count;

            float stdDev = (float)Math.Sqrt(Math.Abs(squaredSum - mean * mean));

            float median;
            if (measuredData.Count % 2 == 0)
                median = (sortedMeans[measuredData.Count / 2 - 1] + sortedMeans[measuredData.Count / 2]) * 0.5f;
            else
                median = sortedMeans[measuredData.Count / 2];

            return new FloatStatsContainer((float)mean, min, max, stdDev, median);
        }
    }
}
