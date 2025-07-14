using System;
using System.Xml;
using System.Xml.Serialization;

using ADCEngine;

using AdcTools;

namespace BasicModules.YieldmapEditor
{
    [Serializable]
    public class PrmDefectKiller : Serializable, IValueComparer
    {
        [XmlAttribute] public string DefectLabel { get; set; }
        [XmlAttribute] public int KillerStatusNum { get; set; }

        //=================================================================
        // Constructeur
        //=================================================================
        public PrmDefectKiller()
        {
            DefectLabel = String.Empty;
            KillerStatusNum = 0;
        }

        public PrmDefectKiller(String sLabel, int nKillerStatusNum)
        {
            DefectLabel = sLabel;
            KillerStatusNum = nKillerStatusNum;
        }

        public bool HasSameValue(object obj)
        {
            var killer = obj as PrmDefectKiller;
            return killer != null &&
                   DefectLabel == killer.DefectLabel &&
                   KillerStatusNum == killer.KillerStatusNum;
        }
    }
}
