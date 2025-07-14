using Agileo.SemiDefinitions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System;

namespace UnitySC.EFEM.Rorze.Devices.Robot.MapperRR75x.Configuration
{
    /// <summary>
    /// Represent possible <see cref="SampleSizeMappingPositions"/> for each <see cref="TransferLocation"/> for the current <see cref="SampleDimension"/>.
    /// </summary>
    [Serializable]
    public class SampleSizeMappingPositions
    {
        /// <summary>
        /// Used for serializing <see cref="SampleSizeMappingPositions.MappingPositionsPerModule"/>.
        /// </summary>
        [Serializable]
        public class ModuleMappingPositionContainer
        {
            public TransferLocation Module { get; set; }

            public ModuleMappingPosition ModuleMappingPositions { get; set; }
        }

        /// <summary>
        /// Represent possible <see cref="ModuleMappingPosition"/> for each <see cref="TransferLocation"/>.
        /// </summary>
        /// <remarks>
        /// Not directly serializable.
        /// Need to convert it into <see cref="MappingPositionPerModuleSerializableData"/> for serialization.
        /// </remarks>
        [XmlIgnore]
        [JsonIgnore]
        public Dictionary<TransferLocation, ModuleMappingPosition> MappingPositionsPerModule { get; set; }

        [XmlArray(ElementName = "MappingPositionsPerWaferSize")]
        [XmlArrayItem(ElementName = "ModuleMappingPositions")]
        public ModuleMappingPositionContainer[] MappingPositionPerModuleSerializableData
        {
            get => MappingPositionsPerModule.Select(elem => new ModuleMappingPositionContainer
            {
                Module = elem.Key,
                ModuleMappingPositions = elem.Value
            })
                .ToArray();
            set => MappingPositionsPerModule =
                value.ToDictionary(elem => elem.Module, elem => elem.ModuleMappingPositions);
        }

        public SampleSizeMappingPositions()
        {
            MappingPositionsPerModule = new Dictionary<TransferLocation, ModuleMappingPosition>();
        }

        public override string ToString()
        {
            var builder = new StringBuilder(base.ToString());

            builder.AppendLine();

            foreach (var moduleStoppingPositions in MappingPositionsPerModule)
            {
                builder.AppendLine($"{moduleStoppingPositions.Key}");
                builder.Append(moduleStoppingPositions.Value);
            }

            return builder.ToString();
        }
    }
}
