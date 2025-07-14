using System;
using System.Xml.Serialization;

namespace UnitySC.Shared.Data
{
    //Temporary class while waitiong for UTO integration and replace AIS_TC
    [Obsolete("To be deleted - Temporary class while waitiong for UTO integration and replace AIS_TC")]
    [Serializable]
    [XmlRoot("Recipe")]
    public class ModuleRecipe
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string Author { get; set; }

        [XmlAttribute]
        public string Description { get; set; }

        [XmlAttribute]
        public long Duration { get; set; }

        [XmlIgnore]
        public bool Enabled { get; set; }

        [XmlAttribute(AttributeName = "Enabled")]
        public byte EnabledAsByte
        {
            get
            {
                return Convert.ToByte(Enabled);
            }
            set
            {
                Enabled = Convert.ToBoolean(value);
            }
        }
    }
}
