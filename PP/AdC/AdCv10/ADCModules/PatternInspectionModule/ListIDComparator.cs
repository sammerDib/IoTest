using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

using AdcBasicObjects;

namespace PatternInspectionModule
{
    ///////////////////////////////////////////////////////////////////////
    // 
    ///////////////////////////////////////////////////////////////////////
    public class ListIDComparator : ComparatorBase
    {
        [XmlArray("ListId")]
        [XmlArrayItem("Id")]
        public List<int> ListId { get; set; }

        public override bool HasSameValue(object obj)
        {
            var comparator = obj as ListIDComparator;
            return comparator != null &&
                   base.HasSameValue(obj) &&
                   ListId.SequenceEqual(comparator.ListId);
        }

        //=================================================================
        // 
        //=================================================================
        public override bool Test(object o)
        {
            CharacListID ListObj = (CharacListID)o;
            bool bRes = false;
            foreach (int id in ListId)
            {
                bRes |= ListObj.Contains(id);
            }
            return bRes;
        }

        //=================================================================
        // 
        //=================================================================
        public override string ToString()
        {
            string sRes = "";
            if (ListId.Count > 0)
            {
                for (int nIdx = 0; nIdx < (ListId.Count - 1); nIdx++)
                {
                    sRes += ListId[nIdx].ToString();
                    sRes += "|";
                }
                sRes += ListId[ListId.Count - 1].ToString();
            }
            return sRes;
        }
    }
}
