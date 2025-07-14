using AdcBasicObjects;
using ADCEngine;
using AdcTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace GlobaltopoModule
{

    public class GTVidBowWarpPrmClass: Serializable, IValueComparer
    {
        [XmlAttribute] public string Label { get; set; }
        [XmlAttribute] public int VID { get; set; } = -1;
        public bool HasSameValue(object obj)
        {
            var @class = obj as GTVidBowWarpPrmClass;
            return @class != null &&
                   Label == @class.Label &&
                   VID == @class.VID;
        }

    }
}
