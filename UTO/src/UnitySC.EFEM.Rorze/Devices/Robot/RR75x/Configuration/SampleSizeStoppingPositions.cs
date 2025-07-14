using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using Agileo.SemiDefinitions;

using Newtonsoft.Json;

namespace UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Configuration
{
    /// <summary>
    /// Represent possible <see cref="ModuleStoppingPositions"/> for each <see cref="TransferLocation"/> for the current <see cref="SampleDimension"/>.
    /// </summary>
    [Serializable]
    public class SampleSizeStoppingPositions
    {
        /// <summary>
        /// Used for serializing <see cref="StoppingPositionsPerModule"/>.
        /// </summary>
        [Serializable]
        public class ModuleStoppingPositionContainer
        {
            public TransferLocation Module { get; set; }

            public ModuleStoppingPositions ModuleStoppingPositions { get; set; }
        }

        /// <summary>
        /// Represent possible <see cref="ModuleStoppingPositions"/> for each <see cref="TransferLocation"/>.
        /// </summary>
        /// <remarks>
        /// Not directly serializable.
        /// Need to convert it into <see cref="StoppingPositionPerModuleSerializableData"/> for serialization.
        /// </remarks>
        [XmlIgnore]
        [JsonIgnore]
        public Dictionary<TransferLocation, ModuleStoppingPositions> StoppingPositionsPerModule { get; set; }

        [XmlArray(ElementName = "StoppingPositionsPerWaferSize")]
        [XmlArrayItem(ElementName = "ModuleStoppingPositions")]
        public ModuleStoppingPositionContainer[] StoppingPositionPerModuleSerializableData
        {
            get => StoppingPositionsPerModule.Select(elem => new ModuleStoppingPositionContainer
                {
                    Module                  = elem.Key,
                    ModuleStoppingPositions = elem.Value
                })
                .ToArray();
            set => StoppingPositionsPerModule =
                value.ToDictionary(elem => elem.Module, elem => elem.ModuleStoppingPositions);
        }

        public SampleSizeStoppingPositions()
        {
            StoppingPositionsPerModule = new Dictionary<TransferLocation, ModuleStoppingPositions>();
        }

        public override string ToString()
        {
            var builder = new StringBuilder(base.ToString());

            builder.AppendLine();

            foreach (var moduleStoppingPositions in StoppingPositionsPerModule)
            {
                builder.AppendLine($"{moduleStoppingPositions.Key}");
                builder.Append(moduleStoppingPositions.Value);
            }

            return builder.ToString();
        }
    }
}
