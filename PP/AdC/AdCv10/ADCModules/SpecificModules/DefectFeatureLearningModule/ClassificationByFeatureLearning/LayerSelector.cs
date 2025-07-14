using System.Xml.Serialization;

using AdcBasicObjects;

using ADCEngine;

using AdcTools;

namespace DefectFeatureLearning.ClassificationByFeatureLearning
{
    public class LayerSelector : Serializable, IValueComparer
    {
        public string DefectLabel { get; set; }
        [XmlIgnore]
        public Characteristic Characteristic { get; set; }

        /// <summary> Pour que la sérialization se passe bien </summary>
        public string CharacteristicName
        {
            get { return Characteristic.ToString(); }
            set { Characteristic = Characteristic.Parse(value); }
        }

        bool IValueComparer.HasSameValue(object obj)
        {
            LayerSelector selector = obj as LayerSelector;
            return selector != null &&
                   selector.DefectLabel == DefectLabel &&
                   selector.Characteristic == Characteristic;
        }
    }
}
