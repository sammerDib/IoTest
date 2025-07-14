using System.Xml.Serialization;

using AdcTools;


namespace AdvancedModules.ClusterOperation.Sieve
{
    public class SieveClass : Serializable
    {
        [XmlAttribute] public string DefectLabel { get; set; }

        [XmlAttribute] public bool ApplyFilter { get; set; } = true;
    }
}
