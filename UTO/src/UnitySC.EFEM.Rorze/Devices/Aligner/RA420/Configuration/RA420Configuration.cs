using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Configuration;
using UnitySC.Equipment.Abstractions.Devices.Aligner.Configuration;
using UnitySC.Equipment.Abstractions.Enums;

namespace UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Configuration
{
    [Serializable]
    [DataContract(Namespace = "")]

    // ReSharper disable once InconsistentNaming
    public class RA420Configuration : AlignerConfiguration
    {
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public CommunicationConfiguration CommunicationConfig { get; set; }

        [XmlIgnore]
        public Dictionary<uint, SubstrateInformations> SubstrateInformationsPerPositions
        {
            get;
            set;
        }

        [XmlArray(ElementName = "SubstrateInformationsPerPositions")]
        [XmlArrayItem(ElementName = "SubstrateInformationsPerPosition")]
        public SubstrateInformationsPerPositionsContainer[]
            SubstrateInformationsPerPositionsSerializableData
        {
            get
                => SubstrateInformationsPerPositions.Select(
                        elem => new SubstrateInformationsPerPositionsContainer
                        {
                            Position = elem.Key, SubstrateInformations = elem.Value
                        })
                    .ToArray();
            set
                => SubstrateInformationsPerPositions = value.ToDictionary(
                    elem => elem.Position,
                    elem => elem.SubstrateInformations);
        }

        protected override void SetDefaults()
        {
            base.SetDefaults();

            CommunicationConfig = new CommunicationConfiguration();
            SubstrateInformationsPerPositions = new Dictionary<uint, SubstrateInformations>
            {
                //Add default values
                {
                    1,
                    new SubstrateInformations()
                    {
                        SubstrateSize = SampleDimension.S300mm,
                        MaterialType = MaterialType.SiliconWithNotch
                    }
                },
                {
                    2,
                    new SubstrateInformations()
                    {
                        SubstrateSize = SampleDimension.S200mm,
                        MaterialType = MaterialType.SiliconWithNotch
                    }
                },
                {
                    3,
                    new SubstrateInformations()
                    {
                        SubstrateSize = SampleDimension.S150mm,
                        MaterialType = MaterialType.SiliconWithNotch
                    }
                }
            };
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine(base.ToString());
            builder.Append(CommunicationConfig);
            foreach (var keyPairValue in SubstrateInformationsPerPositions)
            {
                builder.AppendLine(
                    $"Dimension : {keyPairValue.Value.SubstrateSize}, Material type : {keyPairValue.Value.MaterialType}, Position : {keyPairValue.Key}");
            }

            return builder.ToString();
        }
    }

    [Serializable]
    [DataContract(Namespace = "")]
    public class SubstrateInformationsPerPositionsContainer
    {
        public uint Position { get; set; }
        public SubstrateInformations SubstrateInformations { get; set; }
    }

    [Serializable]
    [DataContract(Namespace = "")]
    public class SubstrateInformations
    {
        public SampleDimension SubstrateSize { get; set; }
        public MaterialType MaterialType { get; set; }
    }
}
