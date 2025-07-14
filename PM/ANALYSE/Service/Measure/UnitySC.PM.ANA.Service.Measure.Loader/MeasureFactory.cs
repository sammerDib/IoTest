using System;

using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Measure.Bow;
using UnitySC.PM.ANA.Service.Measure.NanoTopo;
using UnitySC.PM.ANA.Service.Measure.Thickness;
using UnitySC.PM.ANA.Service.Measure.Topography;
using UnitySC.PM.ANA.Service.Measure.TSV;
using UnitySC.PM.ANA.Service.Measure.Warp;
using UnitySC.PM.ANA.Service.Measure.XYCalibration;
using UnitySC.PM.ANA.Service.Measure.Step;
using UnitySC.PM.ANA.Service.Measure.EdgeTrim;
using UnitySC.PM.ANA.Service.Measure.Trench;

namespace UnitySC.PM.ANA.Service.Measure.Shared
{
    internal static class MeasureFactory
    {
        /// <exception cref="ArgumentException">When given measure type is not handled.</exception>
        public static IMeasure CreateMeasure(MeasureType type)
        {
            switch (type)
            {
                case MeasureType.XYCalibration: return new MeasureXYCalibration();
                case MeasureType.TSV: return new MeasureTSV();
                case MeasureType.NanoTopo: return new MeasureNanoTopo();
                case MeasureType.Bow: return new MeasureBow();
                case MeasureType.Thickness: return new MeasureThickness();
                case MeasureType.Topography: return new MeasureTopography();
                case MeasureType.Warp: return new MeasureWarp();
                case MeasureType.Step: return new MeasureStep();
                case MeasureType.EdgeTrim: return new MeasureEdgeTrim();
                case MeasureType.Trench: return new MeasureTrench();
                default:
                    throw new ArgumentException($"Unknown measure type \"{type}\"");
            }
        }
    }
}
