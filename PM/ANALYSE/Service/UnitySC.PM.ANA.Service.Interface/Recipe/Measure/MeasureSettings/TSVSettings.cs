using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.TSV;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings
{
    [DataContract]
    public class TSVSettings : MeasureSettingsBase, IAutoFocusMeasureSettings, ISummaryProvider
    {
        [XmlIgnore]
        public override MeasureType MeasureType => MeasureType.TSV;

        [XmlIgnore]
        public override bool MeasureStartAtMeasurePoint => true;

        [DataMember]
        public TSVMeasurePrecision Precision { get; set; }

        [DataMember]
        public TSVAcquisitionStrategy Strategy { get; set; }

        [DataMember]
        public Length DepthTarget { get; set; }

        [DataMember]
        public ResultCorrectionSettings DepthCorrection { get; set; } = new ResultCorrectionSettings();

        [DataMember]
        public LengthTolerance DepthTolerance { get; set; }

        [DataMember]
        public Length LengthTarget { get; set; }

        [DataMember]
        public ResultCorrectionSettings LengthCorrection { get; set; } = new ResultCorrectionSettings();

        [DataMember]
        public LengthTolerance LengthTolerance { get; set; }

        [DataMember]
        public Length WidthTarget { get; set; }

        [DataMember]
        public ResultCorrectionSettings WidthCorrection { get; set; } = new ResultCorrectionSettings();

        [DataMember]
        public LengthTolerance WidthTolerance { get; set; }

        [DataMember]
        public string CameraId { get; set; }

        [DataMember]
        public ProbeSettings Probe { get; set; }

        [DataMember]
        public Length EllipseDetectionTolerance { get; set; }

        [DataMember]
        public TSVShape Shape { get; set; }

        [DataMember]
        public CenteredRegionOfInterest ROI { get; set; }

        [DataMember]
        public TopImageAcquisitionContext MeasureContext { get; set; }

        [DataMember]
        public ShapeDetectionModes ShapeDetectionMode { get; set; }

        [DataMember]
        public AutoFocusSettings AutoFocusSettings { get; set; }

        [DataMember]
        public List<LayerSettings> PhysicalLayers { get; set; }

        [DataMember]
        public string DColTSVDepthLabel { get; set; } = "TSV Depth";

        [DataMember]
        public string DColTSVCDWidthLabel { get; set; } = "TSV CD Width";

        [DataMember]
        public string DColTSVCDLengthLabel { get; set; } = "TSV CD Height";

        public MeasureState GetLengthMeasureState(Length lengthToTest)
        {
            return MeasureStateComputer.GetMeasureState(lengthToTest, LengthTolerance, LengthTarget);
        }

        public MeasureState GetDepthMeasureState(Length depthToTest)
        {
            return MeasureStateComputer.GetMeasureState(depthToTest, DepthTolerance, DepthTarget);
        }

        public MeasureState GetWidthMeasureState(Length widthToTest)
        {
            return MeasureStateComputer.GetMeasureState(widthToTest, WidthTolerance, WidthTarget);
        }

        #region ISummaryProvider

        public List<string> GetProbesUsed()
        {
            var probesUsed = new List<string>
            {
                "Camera" // For the CD
            };
            if (!(Probe?.ProbeId is null))
                probesUsed.Add(Probe.ProbeId);
            return probesUsed;
        }

        public List<string> GetObjectivesUsed()
        {
            var objectivesUsed = new List<string>();
            var topObjective = MeasureContext?.TopObjectiveContext?.ObjectiveId;
            if (topObjective != null)
                objectivesUsed.Add(topObjective);
            var autofocusCameraObjective = (AutoFocusSettings?.ImageAutoFocusContext as TopImageAcquisitionContext)?.TopObjectiveContext?.ObjectiveId;
            if (autofocusCameraObjective != null)
                objectivesUsed.Add(autofocusCameraObjective);
            var autofocusLiseObjective = (AutoFocusSettings?.LiseAutoFocusContext as TopObjectiveContext)?.ObjectiveId;
            if (autofocusLiseObjective != null)
                objectivesUsed.Add(autofocusLiseObjective);

            return objectivesUsed.Distinct().ToList();
        }

        public List<string> GetLightsUsed()
        {
            var lightsUsed = new List<string>();
            var lights = MeasureContext?.Lights?.Lights;

            if (lights != null)
            {
                lightsUsed.AddRange(lights.Where(l => l.Intensity > 0).Select(l => l.DeviceID));
            }
            var autofocusCameraLights = (AutoFocusSettings?.ImageAutoFocusContext as TopImageAcquisitionContext)?.Lights.Lights;
            if (autofocusCameraLights != null)
            {
                lightsUsed.AddRange(autofocusCameraLights.Where(l => l.Intensity > 0).Select(l => l.DeviceID));
            }

            return lightsUsed.Distinct().ToList();
        }

        public AutoFocusType? GetAutoFocusTypeUsed()
        {
            if (AutoFocusSettings != null)
                return AutoFocusSettings.Type;

            return null;
        }

        #endregion ISummaryProvider
    }
}
