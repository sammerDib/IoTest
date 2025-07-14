using System;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Measure
{
    public enum MeasureType
    {
        TSV,
        NanoTopo,
        Thickness,
        XYCalibration,
        Topography,
        Bow,
        Warp,
        Step,
        EdgeTrim,
        Trench,
        //Roughness,
        //Pillar,
        //PeriodicStructure,
        //Overlay,
        //CD,
        //EBR,
    }

    public static class MeasureTypeExtensions
    {
        public static MeasureType GetMeasureType(this ResultType resType)
        {
            switch (resType)
            {
                case ResultType.ANALYSE_TSV:
                    return MeasureType.TSV;

                case ResultType.ANALYSE_NanoTopo:
                    return MeasureType.NanoTopo;

                case ResultType.ANALYSE_Thickness:
                    return MeasureType.Thickness;

                case ResultType.ANALYSE_XYCalibration:
                    return MeasureType.XYCalibration;

                case ResultType.ANALYSE_Topography:
                    return MeasureType.Topography;

                case ResultType.ANALYSE_Bow:
                    return MeasureType.Bow;

                case ResultType.ANALYSE_Warp:
                    return MeasureType.Warp;
                
                case ResultType.ANALYSE_Step:
                    return MeasureType.Step;

                case ResultType.ANALYSE_EdgeTrim:
                    return MeasureType.EdgeTrim;

                case ResultType.ANALYSE_Trench:
                    return MeasureType.Trench;

                //case ResultType.ANALYSE_Roughness:
                //    return MeasureType.Roughness;

                //case ResultType.ANALYSE_Pillar:
                //    return MeasureType.Pillar;
                //case ResultType.ANALYSE_PeriodicStructure:
                //    return MeasureType.PeriodicStructure;
                //case ResultType.ANALYSE_Overlay:
                //    return MeasureType.Overlay;
                //case ResultType.ANALYSE_CD:
                //    return MeasureType.CD;
                //case ResultType.ANALYSE_EBR:
                //    return MeasureType.EBR;

                default:
                    throw new InvalidOperationException($"Measure type is not defined for {resType}");
            }
        }

        public static ResultType GetResultType(this MeasureType mesType)
        {
            switch (mesType)
            {
                case MeasureType.TSV:
                    return ResultType.ANALYSE_TSV;

                case MeasureType.NanoTopo:
                    return ResultType.ANALYSE_NanoTopo;

                case MeasureType.Thickness:
                    return ResultType.ANALYSE_Thickness;

                case MeasureType.XYCalibration:
                    return ResultType.ANALYSE_XYCalibration;

                case MeasureType.Topography:
                    return ResultType.ANALYSE_Topography;

                case MeasureType.Bow:
                    return ResultType.ANALYSE_Bow;

                case MeasureType.Warp:
                    return ResultType.ANALYSE_Warp;
                
                case MeasureType.Step:
                    return ResultType.ANALYSE_Step;

                case MeasureType.EdgeTrim:
                    return ResultType.ANALYSE_EdgeTrim;

                case MeasureType.Trench:
                    return ResultType.ANALYSE_Trench;
                //case MeasureType.Roughness:
                //    return ResultType.ANALYSE_Roughness;
                //case MeasureType.Trench:
                //    return ResultType.ANALYSE_Trench;
                //case MeasureType.Pillar:
                //    return ResultType.ANALYSE_Pillar;
                //case MeasureType.PeriodicStructure:
                //    return ResultType.ANALYSE_PeriodicStructure;
                //case MeasureType.Overlay:
                //    return ResultType.ANALYSE_Overlay;
                //case MeasureType.CD:
                //    return ResultType.ANALYSE_CD;
                //case MeasureType.EBR:
                //    return ResultType.ANALYSE_EBR;

                default:
                    throw new InvalidOperationException($"Result type is not defined for {mesType}");
            }
        }
    }
}
