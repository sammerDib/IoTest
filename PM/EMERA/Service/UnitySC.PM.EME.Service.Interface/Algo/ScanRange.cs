
using System;
using System.Runtime.Serialization;

using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.EME.Service.Interface.Algo
{
    [DataContract]
    public enum ScanRangeType
    {
        [EnumMember]
        Small,

        [EnumMember]
        Medium,

        [EnumMember]
        Large,

        [EnumMember]
        AllAxisRange,

        [EnumMember]
        Configured,
    }

    [DataContract]
    public class ScanRange
    {
        public ScanRange() : this(0, 0)
        { }

        public ScanRange(double min, double max)
        {
            Min = min;
            Max = max;
        }

        public InputValidity CheckInputValidity()
        {
            var validity = new InputValidity(true);

            if (double.IsNaN(Min) || double.IsNaN(Max))
            {
                validity.IsValid = false;
                validity.Message.Add($"The scan range configured (min: {Min}, max: {Max}) must not contain NaN values.");
            }

            if (Min > Max)
            {
                validity.IsValid = false;
                validity.Message.Add($"The min value ({Min}) must be less than the max value ({Max}).");
            }

            return validity;
        }

        [DataMember]
        public double Min { get; set; }

        [DataMember]
        public double Max { get; set; }

        public double Median(int precision = 2)
        {
            return Math.Round((Min + Max) / 2, precision);
        }
    }

    [DataContract]
    public class ScanRangeWithStep
    {
        public ScanRangeWithStep()
        {
            Min = 0;
            Max = 0;
            Step = 0;
        }

        public ScanRangeWithStep(double min, double max, double step)
        {
            Min = min;
            Max = max;
            Step = step;
        }

        public InputValidity CheckInputValidity()
        {
            var validity = new InputValidity(true);

            if (double.IsNaN(Min) || double.IsNaN(Max) || double.IsNaN(Step))
            {
                validity.IsValid = false;
                validity.Message.Add($"The scan range configured (min: {Min}, max: {Max}, step: {Step}) must not contain NaN values.");
            }

            if (Min > Max)
            {
                validity.IsValid = false;
                validity.Message.Add($"The min value ({Min}) must be less than the max value ({Max}).");
            }

            if (Step <= 0)
            {
                validity.IsValid = false;
                validity.Message.Add($"The scan range step ({Step}) must be positive.");
            }

            return validity;
        }

        [DataMember]
        public double Min { get; set; }

        [DataMember]
        public double Max { get; set; }

        [DataMember]
        public double Step { get; set; }

        public double Median(int precision = 2)
        {
            return Math.Round((Min + Max) / 2, precision);
        }
    }
}
