using System;
using System.Xml.Serialization;

namespace DeepLearningSoft48.Models
{
    [Serializable]
    public class Module
    {
        [XmlAttribute("name")]
        public string Name { get; set; }
    }
}
