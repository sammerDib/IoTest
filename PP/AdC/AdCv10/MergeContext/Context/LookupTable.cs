using System.Collections.Generic;
using System.Xml.Serialization;

namespace MergeContext.Context
{
    public class LookupTable : AdcTools.Serializable
    {
        private List<LookupValue> _lookupValues = new List<LookupValue>();
        public List<LookupValue> LookupValues
        {
            get { return _lookupValues; }
            set { _lookupValues = value; }
        }
    }

    public class LookupValue
    {
        [XmlAttribute]
        public int Index { get; set; }
        [XmlAttribute]
        public int Value { get; set; }

        public override string ToString()
        {
            return Index.ToString() + "=>" + Value.ToString();
        }
    }
}
