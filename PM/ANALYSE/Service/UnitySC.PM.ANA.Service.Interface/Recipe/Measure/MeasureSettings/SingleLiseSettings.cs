using System.Collections.Generic;
using System.Runtime.Serialization;

using UnitySC.PM.ANA.Service.Interface.Context;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings
{
    public class SingleLiseSettings : ProbeWithObjectiveContextSettings
    {
        [DataMember]
        public double LiseGain { get; set; }

        public override bool Equals(object obj)
        {
            return obj is SingleLiseSettings settings &&
                   ProbeId == settings.ProbeId &&
                   LiseGain == settings.LiseGain &&
                   EqualityComparer<ObjectiveContext>.Default.Equals(ProbeObjectiveContext, settings.ProbeObjectiveContext);
        }

        public override int GetHashCode()
        {
            return (ProbeId, LiseGain, ProbeObjectiveContext).GetHashCode();
        }

        public override ProbeSettings Clone()
        {
            var settings = new SingleLiseSettings();
            settings.ProbeId = ProbeId;
            settings.LiseGain = LiseGain;
            settings.ProbeObjectiveContext = ProbeObjectiveContext;

            return settings;
        }
    }
}
