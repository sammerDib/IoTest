using System.Collections.Generic;
using System.Runtime.Serialization;

using UnitySC.PM.ANA.Service.Interface.Context;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings
{
    [DataContract]
    public class DualLiseSettings : ProbeSettings
    {
        [DataMember]
        public SingleLiseSettings LiseUp { get; set; }

        [DataMember]
        public SingleLiseSettings LiseDown { get; set; }

        public override bool Equals(object obj)
        {
            return obj is DualLiseSettings settings &&
                   ProbeId == settings.ProbeId &&
                   LiseUp.ProbeId == settings.LiseUp.ProbeId &&
                   LiseUp.LiseGain == settings.LiseUp.LiseGain &&
                   LiseDown.ProbeId == settings.LiseDown.ProbeId &&
                   LiseDown.LiseGain == settings.LiseDown.LiseGain &&
                   EqualityComparer<ObjectiveContext>.Default.Equals(LiseUp.ProbeObjectiveContext, settings.LiseUp.ProbeObjectiveContext) &&
                   EqualityComparer<ObjectiveContext>.Default.Equals(LiseDown.ProbeObjectiveContext, settings.LiseDown.ProbeObjectiveContext);
        }

        public override int GetHashCode()
        {
            return (ProbeId, LiseUp.ProbeId, LiseUp.LiseGain, LiseUp.ProbeObjectiveContext, LiseDown.ProbeId, LiseDown.LiseGain, LiseDown.ProbeObjectiveContext).GetHashCode();
        }

        public override ProbeSettings Clone()
        {
            var settings = new DualLiseSettings();
            settings.ProbeId = ProbeId;
            settings.LiseUp = LiseUp.Clone() as SingleLiseSettings;
            settings.LiseDown = LiseDown.Clone() as SingleLiseSettings;

            return settings;
        }
    }
}
