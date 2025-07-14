using System.Xml;
using System.Xml.Serialization;

using ADCEngine;

using AdcTools;

using UnitySC.Shared.Tools;

namespace AdcBasicObjects
{
    ///////////////////////////////////////////////////////////////////////
    // Base class
    ///////////////////////////////////////////////////////////////////////
    public abstract class ComparatorBase : Serializable, IValueComparer
    {
        [XmlIgnore] public Characteristic characteristic;

        public abstract bool Test(object o);

        //=================================================================
        // Traitements spéciaux pour la Characteristic
        //=================================================================
        protected override Serializable OnLoaded(XmlNode node)
        {
            string name = node.GetAttributeValue("Characteristic");
            characteristic = Characteristic.Parse(name);

            return this;
        }

        public override XmlNode SerializeAsChildOf(XmlNode parent)
        {
            XmlNode node = base.SerializeAsChildOf(parent);
            node.AddAttribute("Characteristic", characteristic.ToString());
            return node;
        }

        public virtual bool HasSameValue(object obj)
        {
            var @base = obj as ComparatorBase;
            return @base != null && characteristic == @base.characteristic;
        }

    }

}
