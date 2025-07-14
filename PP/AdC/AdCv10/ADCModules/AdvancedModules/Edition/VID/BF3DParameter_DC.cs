using System;
using System.Xml;
using System.Xml.Serialization;

using ADCEngine;

using AdcTools;


namespace AdvancedModules.Edition.VID
{
    public enum enDataType_3D
    {
        bumpsperdie,
        TotalWaferYield,
        DiesfailedError,
        diesInspected,
        diesPassed,
        diesRejected,
        BumpsInspected,
        BumpsPassed,
        BumpsFailed,
        InspectedYield,
        BumpMetrologyYield,
        diesfailedMissing,
        diesfailedHeight,
        diesfailedAverageHeight,
        diesfailedCoplanarity,
        diesfailedSubstrateCoplanarity,
        BumpsfailedMissing,
        BumpsfailedHeight,
        BumpHeightMin,
        BumpHeightMean,
        BumpHeightMax,
        BumpHeightStd,
        BumpAverageHeightMin,
        BumpAverageHeightMean,
        BumpAverageHeightMax,
        BumpAverageHeightStd,
        CoplanarityMin,
        CoplanarityMean,
        CoplanarityMax,
        CoplanarityStd,
        SubstrateCoplanarityMin,
        SubstrateCoplanarityMean,
        SubstrateCoplanarityMax,
        SubstrateCoplanarityStd,
        dieCollection
    }
    ///////////////////////////////////////////////////////////////////////
    // Chaque classe de défauts est associée à un VID
    ///////////////////////////////////////////////////////////////////////
    [Serializable]
    public class DataCollect_3D : Serializable, IValueComparer
    {
        [XmlAttribute] public string DataName { get; set; }
        [XmlAttribute] public int VID { get; set; } = -1;
        [XmlAttribute] public string VidLabel { get; set; } = null;
        [XmlAttribute] public enDataType_3D DataType { get; set; }

        public DataCollect_3D()
        {
            DataName = "";
        }
        public DataCollect_3D(String name)
        {
            DataName = name;
        }

        public bool HasSameValue(object obj)
        {
            var @class = obj as DataCollect_3D;
            return @class != null &&
                   DataName == @class.DataName &&
                   VID == @class.VID &&
                   VidLabel == @class.VidLabel;
        }
    }


}
