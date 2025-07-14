using System;
using System.Xml.Serialization;

namespace UnitySC.Shared.Format.Metro
{
    public enum MeasureState
    {
        [XmlEnum(Name = "Success")]
        Success, // All Measures are in tolerance scope (100% in tolerance)

        [XmlEnum(Name = "Partial")]
        Partial, //  A least one (or more) measures are outside tolerance scope and a least one (or more) measures are inside the scope (x% measure in tolerance 0<x<100)

        [XmlEnum(Name = "Error")]
        Error,  // All measure are outside tolerance

        [XmlEnum(Name = "NotMeasured")]
        NotMeasured // At least one measure has not be done, for HW issue reason or Sensor out of range issue or bad quality
    }

    public static class EnumExtensions
    {
        public static string ToHumanizedString(this MeasureState state)
        {
            return ToHumanizedString(state as MeasureState?);
        }

        public static string ToHumanizedString(this MeasureState? state)
        {
            if (state == null) return "-";
            switch (state.Value)
            {
                case MeasureState.Success:
                    return "Good";
                case MeasureState.Partial:
                    return "Warning";
                case MeasureState.Error:
                    return "Bad";
                case MeasureState.NotMeasured:
                    return "Not Measured";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
