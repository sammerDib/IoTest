using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using Agileo.SemiDefinitions;

using Newtonsoft.Json;

using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Configuration;

namespace UnitySC.EFEM.Rorze.Devices.Robot.MapperRR75x.Configuration
{
    public class MapperRR75xConfiguration : RR75xConfiguration
    {
        #region Inner Classes

        /// <summary>
        /// Used for serializing <see cref="MappingPositionPerSampleSize"/>.
        /// </summary>
        [Serializable]
        public class SampleSizeMappingPositionsContainer
        {
            public SampleDimension WaferSize { get; set; }

            public SampleSizeMappingPositions SampleSizeMappingPositions { get; set; }
        }

        #endregion Inner Classes

        /// <summary>
        /// Represent possible <see cref="SampleSizeMappingPositions" /> for each
        /// <see cref="SampleDimension" />.
        /// </summary>
        /// <remarks>
        /// Not directly serializable. Need to convert it into
        /// <see cref="MappingPositionPerSampleSizeSerializableData" /> for serialization.
        /// </remarks>
        [XmlIgnore]
        [JsonIgnore]
        public Dictionary<SampleDimension, SampleSizeMappingPositions> MappingPositionPerSampleSize
        {
            get;
            set;
        }

        [XmlArray(ElementName = "RobotArmMappingPositions")]
        [XmlArrayItem(ElementName = "SampleSizeMappingPositions")]
        public SampleSizeMappingPositionsContainer[] MappingPositionPerSampleSizeSerializableData
        {
            get
                => MappingPositionPerSampleSize.Select(
                        elem => new SampleSizeMappingPositionsContainer
                        {
                            WaferSize = elem.Key,
                            SampleSizeMappingPositions = elem.Value
                        })
                    .ToArray();
            set
                => MappingPositionPerSampleSize = value.ToDictionary(
                    elem => elem.WaferSize,
                    elem => elem.SampleSizeMappingPositions);
        }

        protected override void SetDefaults()
        {
            base.SetDefaults();

            MappingPositionPerSampleSize = new Dictionary<SampleDimension, SampleSizeMappingPositions>();
        }

        public override string ToString()
        {
            var builder = new StringBuilder(base.ToString());

            foreach (var sampleSizeMappingPositions in MappingPositionPerSampleSize)
            {
                builder.AppendLine();
                builder.AppendLine($"Mapping positions for {sampleSizeMappingPositions.Key} wafer size.");
                builder.Append(sampleSizeMappingPositions.Value);
            }

            return builder.ToString();
        }
    }
}
