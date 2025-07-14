using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.DMT.Service.Interface.Measure.Outputs;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.Enum.Module;

namespace UnitySC.PM.DMT.Service.Interface.Measure
{
    /// <summary>
    ///     Data for deflectometry measure
    /// </summary>
    [DataContract]
    public class DeflectometryMeasure : MeasureBase
    {
        public override string MeasureName => "Deflectometry";

        public override MeasureType MeasureType => MeasureType.DeflectometryMeasure;

        /// <summary>
        ///     Fringe to display
        /// </summary>
        [DataMember]
        public Fringe Fringe { get; set; }

        /// <summary>
        ///     Measure results
        /// </summary>
        [DataMember]
        public DeflectometryOutput Outputs { get; set; }

        /// <summary>
        ///     Measure results
        /// </summary>
        [DataMember]
        [XmlIgnore]
        public DeflectometryOutput AvailableOutputs { get; set; }

        /// <summary>
        ///     Curvature Dynamic, i.e. sensibilité de l'algo
        /// </summary>
        [DataMember]
        public double CurvatureDynamic { get; set; } = 1;

        [DataMember] public double DarkDynamic { get; set; } = 1;
        [DataMember] public double UnWrappedDynamic { get; set; } = 1;

        [DataMember]
        public bool UseEnhancedMask { get; set; } = true;

        public override List<ResultType> GetOutputTypes()
        {
            var outputResultTypes = new List<ResultType>();

            if (Outputs.HasFlag(DeflectometryOutput.Curvature))
            {
                SafeAddOutputTo(outputResultTypes, (Side == Side.Front ? DMTResult.CurvatureX_Front : DMTResult.CurvatureX_Back));
                SafeAddOutputTo(outputResultTypes, (Side == Side.Front ? DMTResult.CurvatureY_Front : DMTResult.CurvatureY_Back));
            }

            if (Outputs.HasFlag(DeflectometryOutput.Amplitude))
            {
                SafeAddOutputTo(outputResultTypes, (Side == Side.Front ? DMTResult.AmplitudeX_Front : DMTResult.AmplitudeX_Back));
                SafeAddOutputTo(outputResultTypes, (Side == Side.Front ? DMTResult.AmplitudeY_Front : DMTResult.AmplitudeY_Back));
            }

            if (Outputs.HasFlag(DeflectometryOutput.NanoTopo))
            {
                // TODO SDE add nanotopo results.
            }

            if (Outputs.HasFlag(DeflectometryOutput.UnwrappedPhase))
            {
                // Result are not Prodcution results - they are skipped - waiting Isabelle specification
                SafeAddOutputTo(outputResultTypes, (Side == Side.Front ? DMTResult.UnwrappedPhaseX_Front : DMTResult.UnwrappedPhaseX_Back));
                SafeAddOutputTo(outputResultTypes, (Side == Side.Front ? DMTResult.UnwrappedPhaseY_Front : DMTResult.UnwrappedPhaseY_Back));
            }

            if (Outputs.HasFlag(DeflectometryOutput.GlobalTopo))
            {
                // Result are not Prodcution results - they are skipped - waiting Isabelle specification
                //SafeAddOutputTo(outputResultTypes,(Side == Enum.Side.Front ? DMTResult.TopoPhaseX_Front : DMTResult.TopoPhaseX_Back));
                //SafeAddOutputTo(outputResultTypes, (Side == Enum.Side.Front ? DMTResult.TopoPhaseY_Front : DMTResult.TopoPhaseY_Back));
                SafeAddOutputTo(outputResultTypes, (Side == Side.Front ? DMTResult.TopoPhaseZ_Front : DMTResult.TopoPhaseZ_Back));
            }

            if (Outputs.HasFlag(DeflectometryOutput.LowAngleDarkField))
            {
                SafeAddOutputTo(outputResultTypes, (Side == Side.Front ? DMTResult.LowAngleDarkField_Front : DMTResult.LowAngleDarkField_Back));
            }

            return outputResultTypes;
        }
    }
}
