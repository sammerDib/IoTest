using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings
{
    
    [DataContract]
    public class TrenchSettings : MeasureSettingsBase, IAutoFocusMeasureSettings, ISummaryProvider
    {
        [XmlIgnore]
        public override MeasureType MeasureType => MeasureType.Trench;

        [XmlIgnore]
        public override bool MeasureStartAtMeasurePoint => true;

        [DataMember]
        public TopImageAcquisitionContext MeasureContext { get; set; }

        [DataMember]
        public AutoFocusSettings AutoFocusSettings { get; set; }

        [DataMember]
        public ProbeSettings ProbeSettings { get; set; }

        [DataMember]
        public Length DepthTarget { get; set; }

        [DataMember]
        public ResultCorrectionSettings DepthCorrection { get; set; } = new ResultCorrectionSettings();

        [DataMember]
        public LengthTolerance DepthTolerance { get; set; }

        [DataMember]
        public bool IsWidthMeasured { get; set; }


        [DataMember]
        public Length WidthTarget { get; set; }

        [DataMember]
        public ResultCorrectionSettings WidthCorrection { get; set; } = new ResultCorrectionSettings();

        [DataMember]
        public LengthTolerance WidthTolerance { get; set; }


        [DataMember]
        public Length ScanSize { get; set; }

        [DataMember]
        public Angle ScanAngle { get; set; }

        [DataMember]
        public Length TopEdgeExclusionSize { get; set; }

        [DataMember]
        public Length BottomEdgeExclusionSize { get; set; }

        [DataMember]
        public Length StepSize { get; set; }


        // Exclusions to be defined


        #region ISummaryProvider

        public AutoFocusType? GetAutoFocusTypeUsed()
        {
            if (AutoFocusSettings != null)
                return AutoFocusSettings.Type;

            return null;
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

        public List<string> GetProbesUsed()
        {
            var probesUsed = new List<string>();

            // Probes Used by AutoFocus
            if ((AutoFocusSettings?.Type == AutoFocusType.Lise) || (AutoFocusSettings?.Type == AutoFocusType.LiseAndCamera))
            {
                var autoFocusProbe = AutoFocusSettings?.ProbeId;
                if (autoFocusProbe != null)
                    probesUsed.Add(autoFocusProbe);
            }

            if ((AutoFocusSettings?.Type == AutoFocusType.Camera) || (AutoFocusSettings?.Type == AutoFocusType.LiseAndCamera))
            {
                probesUsed.Add("Camera");
            }

            if (ProbeSettings !=null)
                probesUsed.Add(ProbeSettings.ProbeId);

            return probesUsed;
        }
        #endregion ISummaryProvider
    }
}
