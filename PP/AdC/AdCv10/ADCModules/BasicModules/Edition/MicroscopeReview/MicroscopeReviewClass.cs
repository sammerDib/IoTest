using System.Xml.Serialization;

using ADCEngine;

using AdcTools;

namespace BasicModules.Edition.MicroscopeReview
{
    public enum StrategyType
    {
        All,
        Random,
        First,
        Last,
        Biggest,
        Smallest,
        SizeSampling
    }

    public class MicroscopeReviewClass : Serializable, IValueComparer
    {
        [XmlAttribute] public string DefectLabel { get; set; }
        [XmlAttribute] public bool UseReview { get; set; }
        [XmlAttribute] public int NbSamples { get; set; }
        [XmlAttribute] public StrategyType Strategy { get; set; }

        public bool HasSameValue(object obj)
        {
            var @class = obj as MicroscopeReviewClass;
            return @class != null &&
                   DefectLabel == @class.DefectLabel &&
                   UseReview == @class.UseReview &&
                   NbSamples == @class.NbSamples &&
                   Strategy == @class.Strategy;
        }
    }
}
