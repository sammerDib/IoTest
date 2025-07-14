using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings
{
    [DataContract]
    public class ThicknessSettings : MeasureSettingsBase, ISummaryProvider
    {
        [XmlIgnore]
        public override MeasureType MeasureType => MeasureType.Thickness;

        [XmlIgnore]
        public override bool MeasureStartAtMeasurePoint => true;

        /// <summary>
        /// All physical layers filled in by the user before the group notion
        /// </summary>
        [DataMember]
        public List<LayerSettings> PhysicalLayers { get; set; }

        /// <summary>
        /// The layers that the user wants to measure.
        /// </summary>
        [DataMember]
        public List<Layer> LayersToMeasure { get; set; }

        /// <summary>
        /// Acquisition startegy used for each point defined in measure.
        /// </summary>
        [DataMember]
        public AcquisitionStrategy Strategy { get; set; }

        [DataMember]
        public bool HasWarpMeasure { get; set; }

        [DataMember]
        public Length WarpTargetMax { get; set; }

        [DataMember]
        public AutoFocusSettings AutoFocusSettings { get; set; }

        public MeasureState GetWarpMeasureState(Length warpToTest)
        {
            return MeasureStateComputer.GetMeasureState(warpToTest, new Length(0d, LengthUnit.Micrometer), WarpTargetMax);
        }

        #region ISummaryProvider
        public AutoFocusType? GetAutoFocusTypeUsed()
        {
            if (AutoFocusSettings != null)
            {
                return AutoFocusSettings.Type;
            }

            return null;
        }

        public List<string> GetLightsUsed()
        {
            var lightsUsed = new List<string>();
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

            foreach (var layer in LayersToMeasure)
            {
                switch(layer.ProbeSettings)
                {
                    case DualLiseSettings dualLiseSettings:
                        objectivesUsed.Add(dualLiseSettings.LiseUp.ProbeObjectiveContext.ObjectiveId);
                        objectivesUsed.Add(dualLiseSettings.LiseDown.ProbeObjectiveContext.ObjectiveId);
                        break;
                    case SingleLiseSettings singleLiseSettings:
                        objectivesUsed.Add(singleLiseSettings.ProbeObjectiveContext.ObjectiveId);
                        break;
                }
            }

            string autofocusCameraObjective = (AutoFocusSettings?.ImageAutoFocusContext as TopImageAcquisitionContext)?.TopObjectiveContext?.ObjectiveId;
            if (autofocusCameraObjective != null)
            {
                objectivesUsed.Add(autofocusCameraObjective);
            }
            string autofocusLiseObjective = (AutoFocusSettings?.LiseAutoFocusContext as TopObjectiveContext)?.ObjectiveId;
            if (autofocusLiseObjective != null)
            {
                objectivesUsed.Add(autofocusLiseObjective);
            }

            return objectivesUsed.Distinct().ToList();
        }

        public List<string> GetProbesUsed()
        {
            var probesUsed = new List<string>();

            foreach (var layer in LayersToMeasure)
            {
                if (!probesUsed.Contains(layer.ProbeSettings.ProbeId))
                {
                    probesUsed.Add(layer.ProbeSettings.ProbeId);
                }
            }

            return probesUsed.Distinct().ToList();
        }

        #endregion ISummaryProvider
    }
}
