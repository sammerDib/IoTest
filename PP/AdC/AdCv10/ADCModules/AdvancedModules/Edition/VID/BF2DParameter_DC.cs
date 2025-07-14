using System;
using System.Xml;
using System.Xml.Serialization;

using ADCEngine;

using AdcTools;


namespace AdvancedModules.Edition.VID
{
    public enum enDataType
    {
        bumpperdie,
        totalWaferYield,
        nbDieFailed,
        nbDieInspected,
        nbDiePassed,
        nbDieRejected,
        nbBumpInspected,
        nbBumpPassed,
        nbBumpFailed,
        inspectedYield,
        bumpMetrologyYield,
        nbDieFailedMissing,
        nbDieFailedDiameter,
        nbDieFailedOffset,
        nbDieFailedAverage,
        nbBumpFailedMissing,
        nbBumpFailedDiameter,
        nbBumpFailedOffset,
        statsBumpDiameterMin,
        statsBumpDiameterMean,
        statsBumpDiameterMax,
        statsBumpDiameterStdDev,
        statsBumpOffsetMin,
        statsBumpOffsetMean,
        statsBumpOffsetMax,
        statsBumpOffsetStdDev,
        statsBumpAverageDiameterMin,
        statsBumpAverageDiameterMean,
        statsBumpAverageDiameterMax,
        statsBumpAverageDiameterStdDev,
        dieCollection

    }
    ///////////////////////////////////////////////////////////////////////
    // Chaque classe de défauts est associée à un VID
    ///////////////////////////////////////////////////////////////////////
    [Serializable]
    public class DataCollect : Serializable, IValueComparer
    {
        [XmlAttribute] public string DataName { get; set; }
        [XmlAttribute] public int VID { get; set; } = -1;
        [XmlAttribute] public string VidLabel { get; set; } = null;
        [XmlAttribute] public enDataType DataType { get; set; }

        public DataCollect()
        {
            DataName = "";
        }
        public DataCollect(String name)
        {
            DataName = name;
        }

        public bool HasSameValue(object obj)
        {
            var @class = obj as DataCollect;
            return @class != null &&
                   DataName == @class.DataName &&
                   VID == @class.VID &&
                   VidLabel == @class.VidLabel;
        }
    }


}
