using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings
{
    public enum TopographyAcquisitionResolution
    {
        [XmlEnum(Name = "LargeFOV")]
        LargeFOV,

        [XmlEnum(Name = "HighResolution")]
        HighResolution
    }

    public enum SurfacesInFocus
    {
        [XmlEnum(Name = "Top")]
        Top,

        [XmlEnum(Name = "Bottom")]
        Bottom,

        [XmlEnum(Name = "Unknown")]
        Unknown,
    }

    public class TopographySettings : MeasureSettingsBase, IAutoFocusMeasureSettings, ISummaryProvider
    {
        [XmlIgnore]
        public override MeasureType MeasureType => MeasureType.Topography;

        [XmlIgnore]
        public override bool MeasureStartAtMeasurePoint => true;

        [DataMember]
        public TopImageAcquisitionContext MeasureContext { get; set; }

        [DataMember]
        public AutoFocusSettings AutoFocusSettings { get; set; }

        [DataMember]
        public string LightId { get; set; }

        [DataMember]
        public string CameraId { get; set; }

        [DataMember]
        public string ObjectiveId { get; set; }

        [DataMember]
        public SurfacesInFocus SurfacesInFocus { get; set; }

        [DataMember]
        public Length HeightVariation { get; set; }

        [DataMember]
        public Length ScanMargin { get; set; }

        [DataMember]
        public PostProcessingSettings PostProcessingSettings { get; set; }

        [XmlIgnore]
        public override bool PostProcessingSettingsIsEnabled { get { return PostProcessingSettings?.IsEnabled ?? false; } }

        [DataMember]
        public CenteredRegionOfInterest ROI { get; set; }

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
            if ((AutoFocusSettings?.Type==AutoFocusType.Lise)|| (AutoFocusSettings?.Type == AutoFocusType.LiseAndCamera))
            {
                var autoFocusProbe = AutoFocusSettings?.ProbeId;
                if (autoFocusProbe != null)
                    probesUsed.Add(autoFocusProbe);
            }

            if ((AutoFocusSettings?.Type == AutoFocusType.Camera) || (AutoFocusSettings?.Type == AutoFocusType.LiseAndCamera))
            {
                    probesUsed.Add("Camera");
            }

            return probesUsed;
        }

        #endregion ISummaryProvider
    }
}
