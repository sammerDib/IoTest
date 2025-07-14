using System;
using System.Xml.Serialization;

using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Data.Enum;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{
    [Serializable]
    public class ComputeLowAngleDarkFieldImageInput : IFlowInput
    {
        [XmlIgnore]
        public PSDResult XResult;

        [XmlIgnore]
        public PSDResult YResult;

        public int Period;

        public Measure.Fringe Fringe;

        public float PercentageOfLowSaturation = 0.03f;

        public float DarkDynamic = 1f;
        
        public Side Side { get; set; }

        public ComputeLowAngleDarkFieldImageInput(PSDResult xResult, PSDResult yResult, Measure.Fringe fringe, int period, Side side, float darkDynamic = 1.0f, float percentageOfLowSaturation = 0.03f)
        {
            XResult = xResult;
            YResult = yResult;
            Fringe = fringe;
            Period = period;
            DarkDynamic = darkDynamic;
            Side = side;
            PercentageOfLowSaturation = percentageOfLowSaturation;
        }

        public ComputeLowAngleDarkFieldImageInput(Measure.Fringe fringe, int period, Side side, float darkDynamic = 1.0f, float percentageOfLowSaturation = 0.03f)
        {
            Fringe = fringe;
            Period = period;
            DarkDynamic = darkDynamic;
            PercentageOfLowSaturation = percentageOfLowSaturation;
            Side = side;
        }

        public ComputeLowAngleDarkFieldImageInput(
            DeflectometryMeasure dfMeasure, float percentageOfLowSaturation = 0.03f)
        {
            Fringe = dfMeasure.Fringe;
            Period = dfMeasure.Fringe.Period;
            DarkDynamic = (float)dfMeasure.DarkDynamic;
            Side = dfMeasure.Side;
            PercentageOfLowSaturation = percentageOfLowSaturation;
        }

        public ComputeLowAngleDarkFieldImageInput()
        {
        }

        public InputValidity CheckInputValidity()
        {
            var result = new InputValidity(true);

            if (XResult is null)
            {
                result.IsValid = false;
                result.Message.Add($"Cannot compute a dark image without a X Phase map");
            }

            if (YResult is null)
            {
                result.IsValid = false;
                result.Message.Add($"Cannot compute a dark image without a Y Phase map");
            }

            if (Fringe is null)
            {
                result.IsValid = false;
                result.Message.Add($"Cannot compute a dark image without a fringe object");
            }
            else if (!Fringe.Periods.Contains(Period))
            {
                result.IsValid = false;
                result.Message.Add($"The provided period ({Period}) is not in the fringe object period list");
            }

            return result;
        }
    }
}
