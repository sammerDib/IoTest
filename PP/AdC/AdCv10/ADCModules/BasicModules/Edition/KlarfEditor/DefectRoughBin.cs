using System;
using System.Xml;
using System.Xml.Serialization;

using ADCEngine;

using AdcTools;

namespace BasicModules.KlarfEditor
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public class DefectRoughBin : Serializable, IValueComparer
    {
        [XmlAttribute] public string DefectLabel { get; set; }
        [XmlAttribute]
        public int RoughBinNum
        {
            get;
            set;
        }
        [XmlAttribute] public int FineBinNum { get; set; }
        [XmlAttribute] public int ClassNumber { get; set; }

        //=================================================================
        // Constructeur
        //=================================================================
        public DefectRoughBin()
        {
            DefectLabel = String.Empty;
            RoughBinNum = 0;
        }

        public DefectRoughBin(String sLabel, int nRoughBinNumber)
        {
            DefectLabel = sLabel;
            RoughBinNum = nRoughBinNumber;
        }

        public bool HasSameValue(object obj)
        {
            var bin = obj as DefectRoughBin;
            return bin != null &&
                   DefectLabel == bin.DefectLabel &&
                   RoughBinNum == bin.RoughBinNum &&
                   FineBinNum == bin.FineBinNum;

        }

    }
}
