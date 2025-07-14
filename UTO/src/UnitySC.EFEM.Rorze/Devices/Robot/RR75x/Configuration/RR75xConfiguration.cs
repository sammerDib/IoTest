using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

using Agileo.SemiDefinitions;

using Newtonsoft.Json;

using UnitySC.Equipment.Abstractions.Configuration;
using UnitySC.Equipment.Abstractions.Devices.Robot.Configuration;

namespace UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Configuration
{
    public class RR75xConfiguration : RobotConfiguration
    {
        #region Inner Classes

        /// <summary>
        /// Used for serializing <see cref="StoppingPositionPerSampleSize"/>.
        /// </summary>
        [Serializable]
        public class SampleSizeStoppingPositionsContainer
        {
            public SampleDimension WaferSize { get; set; }

            public SampleSizeStoppingPositions SampleSizeStoppingPositions { get; set; }
        }

        #endregion Inner Classes

        /// <summary>
        /// Represent possible <see cref="SampleSizeStoppingPositions"/> for each <see cref="SampleDimension"/>.
        /// </summary>
        /// <remarks>
        /// Not directly serializable.
        /// Need to convert it into <see cref="StoppingPositionPerSampleSizeSerializableData"/> for serialization.
        /// </remarks>
        [XmlIgnore]
        [JsonIgnore]
        public Dictionary<SampleDimension, SampleSizeStoppingPositions> StoppingPositionPerSampleSize { get; set; }

        [XmlArray(ElementName = "RobotArmStoppingPositions")]
        [XmlArrayItem(ElementName = "SampleSizeStoppingPositions")]
        public SampleSizeStoppingPositionsContainer[] StoppingPositionPerSampleSizeSerializableData
        {
            get => StoppingPositionPerSampleSize.Select(elem => new SampleSizeStoppingPositionsContainer
                {
                    WaferSize                   = elem.Key,
                    SampleSizeStoppingPositions = elem.Value
                })
                .ToArray();
            set => StoppingPositionPerSampleSize =
                value.ToDictionary(elem => elem.WaferSize, elem => elem.SampleSizeStoppingPositions);
        }

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public CommunicationConfiguration CommunicationConfig { get; set; }

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public double LastMemorizedSpeed { get; set; }

        protected override void SetDefaults()
        {
            base.SetDefaults();

            // Default value are extracted from a file called "StationConfig.xml"
            // Current one drive file path is "Agileo Automation\Projets - 154 - UnitySC\P002_Controller_Rorze\Customer Docs\Documents Rorze"
            // The provenance of this document might be Rorze or UnitySC.
            // It's values must be incremented by one to be used as values are 0-based while those of robot stopping positions are 1-based.
            // There validity is not guarantee. If something let you think that they are wrong,
            //    - If you think it is just for the current machine, change them in the current configuration file of the executable
            //    - If you think that it may be for all new machines, change the default configuration file (in source code, "Agileo.EFEMRorze.RC550.EFEM.Robot.Resources.Config1.xml")
            //    - If you find a more accurate document, change values in code, delete old robot configuration file and regenerate one (don't forget to include it in source folder).
            StoppingPositionPerSampleSize = new Dictionary<SampleDimension, SampleSizeStoppingPositions>();
            CommunicationConfig           = new CommunicationConfiguration();
            LastMemorizedSpeed = 50;
        }

        public override string ToString()
        {
            var builder = new StringBuilder(base.ToString());

            foreach (var sampleSizeStoppingPositions in StoppingPositionPerSampleSize)
            {
                builder.AppendLine();
                builder.AppendLine($"Stopping positions for {sampleSizeStoppingPositions.Key} wafer size.");
                builder.Append(sampleSizeStoppingPositions.Value);
            }

            builder.Append(CommunicationConfig);
            builder.Append($"{nameof(LastMemorizedSpeed)}:{LastMemorizedSpeed}");

            return builder.ToString();
        }
    }
}
