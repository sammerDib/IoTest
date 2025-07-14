using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;

using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Enums;

namespace UnitySC.Equipment.Abstractions.Devices.Efem
{
    [DataContract(Namespace = "")]
    public class PersistentLocationInformations
    {
        private static readonly object Lock = new();

        [IgnoreDataMember]
        public Dictionary<string, SerializableSubstrate> LocationInformations { get; set; }

        [DataMember(Name = "LocationInformations")]
        public LocationInformationsContainer[] LocationInformationsSerializableData
        {
            get
            {
                List<LocationInformationsContainer> list = new List<LocationInformationsContainer>();
                foreach (var elem in LocationInformations)
                    list.Add(new LocationInformationsContainer()
                    {
                        SubstrateLocationName = elem.Key,
                        SubstrateInformations = elem.Value
                    });

                return list
                    .ToArray();
            }
            set => LocationInformations =
                value.ToDictionary(elem => elem.SubstrateLocationName, elem => elem.SubstrateInformations);
        }

        public static string Serialize(PersistentLocationInformations informations, string filePath)
        {
            try
            {
                lock (Lock)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
                    var s = new DataContractSerializer(informations.GetType());
                    var settings = new XmlWriterSettings { Indent = true };

                    using var w = XmlWriter.Create(filePath, settings);
                    s.WriteObject(w, informations);
                    return string.Empty;

                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public static PersistentLocationInformations Deserialize(string filePath)
        {
            try
            {
                var s = new DataContractSerializer(typeof(PersistentLocationInformations));
                using FileStream fs = File.Open(filePath, FileMode.Open);
                return (PersistentLocationInformations)s.ReadObject(fs);
            }
            catch
            {
                return new PersistentLocationInformations();
            }
        }
    }

    [DataContract(Namespace = "")]
    public class LocationInformationsContainer
    {
        [DataMember]
        public string SubstrateLocationName { get; set; }

        [DataMember]
        public SerializableSubstrate SubstrateInformations { get; set; }
    }

    [DataContract(Namespace = "")]
    public class SerializableSubstrate
    {
        [DataMember]
        public SampleDimension MaterialDimension { get; set; }

        [DataMember]
        public MaterialType MaterialType { get; set; }

        [DataMember]
        public byte SourcePort { get; set; }

        [DataMember]
        public byte SourceSlot { get; set; }
    }
}
