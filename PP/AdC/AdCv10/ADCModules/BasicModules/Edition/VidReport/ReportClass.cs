using System;
using System.Xml;
using System.Xml.Serialization;

using ADCEngine;

using AdcTools;


namespace BasicModules.VidReport
{
    ///////////////////////////////////////////////////////////////////////
    // Chaque classe de défauts est associée à un VID
    ///////////////////////////////////////////////////////////////////////
    [Serializable]
    public class ReportClass : Serializable, IValueComparer
    {
        [XmlAttribute] public string DefectLabel { get; set; }
        [XmlAttribute] public int VID { get; set; } = -1;
        [XmlAttribute] public string VidLabel { get; set; }
        public double[] BinD { get; set; }
        public int[] Bin { get; set; } = new int[3];

        /// <summary>
        /// C'est un peu bidouille car on gère 2 versions de paramètres:
        /// - V1 les Bins étaient en int (Bin)
        /// - V2 les Bins sont en double (BinD)
        /// </summary>
        [XmlIgnore]
        public double[] Bin2
        {
            get
            {
                if (BinD == null)
                {
                    BinD = new double[3] { Bin[0], Bin[1], Bin[2] };
                    Bin = null;
                }
                return BinD;
            }
        }

        public bool HasSameValue(object obj)
        {
            var @class = obj as ReportClass;
            return @class != null &&
                   DefectLabel == @class.DefectLabel &&
                   VID == @class.VID &&
                   VidLabel == @class.VidLabel &&
                   Bin2[0] == @class.Bin2[0] &&
                   Bin2[1] == @class.Bin2[1] &&
                   Bin2[2] == @class.Bin2[2];
        }

    }

}
