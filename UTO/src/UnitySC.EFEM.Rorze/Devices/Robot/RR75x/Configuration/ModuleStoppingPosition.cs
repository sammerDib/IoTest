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
    /// Represent possible stopping positions for current <see cref="TransferLocation"/>.
    /// </summary>
    [Serializable]
    public class ModuleStoppingPositions
    {
        /// <summary>
        /// Used for serializing <see cref="StoppingPositions"/>.
        /// </summary>
        [Serializable]
        public class StoppingPosition
        {
            public string InnerModulePosition { get; set; }

            public uint ArmStoppingPosition { get; set; }
        }

        /// <summary>
        /// Represent possible stopping positions for current <see cref="TransferLocation"/>.
        /// </summary>
        /// <remarks>
        /// Stopping positions must be provided as 1-based indexing.
        /// Validity range is from 1 to 400 and not from 0 to 399.
        /// Not directly serializable.
        /// Need to convert it into <see cref="StoppingPositionsSerializableData"/> for serialization.
        /// </remarks>
        [XmlIgnore]
        [JsonIgnore]
        public Dictionary<string, uint> StoppingPositions { get; set; }

        [XmlArray(ElementName = "StoppingPositionsPerModule")]
        [XmlArrayItem(ElementName = "StoppingPosition_Index1Based")]
        public StoppingPosition[] StoppingPositionsSerializableData
        {
            get => StoppingPositions.Select(elem => new StoppingPosition
                {
                    InnerModulePosition = elem.Key,
                    ArmStoppingPosition = elem.Value
                })
                .ToArray();
            set => StoppingPositions =
                value.ToDictionary(elem => elem.InnerModulePosition, elem => elem.ArmStoppingPosition);
        }

        public ModuleStoppingPositions()
        {
            StoppingPositions = new Dictionary<string, uint>();
        }

        public override string ToString()
        {
            var builder = new StringBuilder(base.ToString());

            builder.AppendLine();

            foreach (var stoppingPosition in StoppingPositions)
            {
                builder.AppendLine($"{stoppingPosition.Key} => {stoppingPosition.Value}");
            }

            return builder.ToString();
        }
    }
}
