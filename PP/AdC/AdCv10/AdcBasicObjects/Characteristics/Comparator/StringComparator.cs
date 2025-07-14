using System.Xml.Serialization;

namespace AdcBasicObjects
{
    ///////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////
    public class StringComparator : ComparatorBase
    {
        [XmlAttribute] public string Value;

        //=================================================================
        // 
        //=================================================================
        public override bool HasSameValue(object obj)
        {
            var comparator = obj as StringComparator;
            return comparator != null &&
                   base.HasSameValue(obj) &&
                   Value == comparator.Value;
        }

        //=================================================================
        // 
        //=================================================================
        public override bool Test(object o)
        {
            string str = (string)o;
            return str == Value;
        }

        //=================================================================
        // 
        //=================================================================
        public override string ToString()
        {
            return "==\"" + Value + "\"";
        }
    }

}
