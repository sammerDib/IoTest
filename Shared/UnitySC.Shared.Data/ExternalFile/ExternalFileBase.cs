using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace UnitySC.Shared.Data.ExternalFile
{
    [DataContract]
    [KnownType(typeof(ExternalImage))]
    [KnownType(typeof(ExternalMountainsTemplate))]
    /// Used to defined external file
    /// Data are save outside the XmlContent
    public abstract class ExternalFileBase
    {
        [DataMember]
        [XmlAttribute("Key")]
        public string FileNameKey { get; set; }

        [DataMember]
        [XmlIgnore]
        public byte[] Data { get; set; }

        [XmlIgnore]
        public abstract string FileExtension { get; set; }

        /// <summary>
        /// Update current instance with new content.
        /// </summary>
        /// <param name="externalFileBase"></param>
        public abstract void UpdateWith(ExternalFileBase externalFileBase);

        public abstract void LoadFromFile(string filePath);

        public abstract void SaveToFile(string filePath);
    }
}
