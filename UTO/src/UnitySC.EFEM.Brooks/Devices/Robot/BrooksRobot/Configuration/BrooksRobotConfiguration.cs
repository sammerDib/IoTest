using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

using Agileo.SemiDefinitions;

using Newtonsoft.Json;

using UnitySC.Equipment.Abstractions.Devices.Robot.Configuration;

namespace UnitySC.EFEM.Brooks.Devices.Robot.BrooksRobot.Configuration
{
    public class BrooksRobotConfiguration : RobotConfiguration
    {

        #region Properties

        /// <summary>
        /// Gets or sets the name of robot in Brooks efem.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string BrooksRobotName { get; set; }

        /// <summary>
        /// Gets or sets the name of upper arm.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string UpperArmName { get; set; }


        /// <summary>
        /// Gets or sets the name of lower arm.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string LowerArmName { get; set; }


        /// <summary>
        /// Gets or sets the name of upper end effector.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string UpperEndEffectorName { get; set; }

        /// <summary>
        /// Gets or sets the name of lower end effector.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string LowerEndEffectorName { get; set; }


        /// <summary>
        /// Gets or sets the name of motion profile for homming.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string RobotHomeMotionProfile { get; set; }

        #endregion Properties


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
                    WaferSize = elem.Key,
                    SampleSizeStoppingPositions = elem.Value
                })
                .ToArray();
            set => StoppingPositionPerSampleSize =
                value.ToDictionary(elem => elem.WaferSize, elem => elem.SampleSizeStoppingPositions);
        }


        protected override void SetDefaults()
        {
            base.SetDefaults();
            StoppingPositionPerSampleSize = new Dictionary<SampleDimension, SampleSizeStoppingPositions>();
            BrooksRobotName = "WaferEngine";
            UpperArmName = "EFEM.WaferEngine.EE1";
            LowerArmName = "EFEM.WaferEngine.EE2";
            UpperEndEffectorName = "EE1";
            LowerEndEffectorName = "EE2";
            RobotHomeMotionProfile = "Home";
        }

        public override string ToString()
        {
            var builder = new StringBuilder(base.ToString());

            foreach (var sampleSizeStoppingPositions in StoppingPositionPerSampleSize)
            {
                builder.AppendLine();
                builder.AppendLine($"Stopping positions for {sampleSizeStoppingPositions.Key} wafer size.");
                builder.Append(sampleSizeStoppingPositions.Value);
                builder.AppendLine($"{nameof(BrooksRobotName)}: {BrooksRobotName}");
                builder.AppendLine($"{nameof(UpperArmName)}: {UpperArmName}");
                builder.AppendLine($"{nameof(LowerArmName)}: {LowerArmName}");
                builder.AppendLine($"{nameof(UpperEndEffectorName)}: {UpperEndEffectorName}");
                builder.AppendLine($"{nameof(LowerEndEffectorName)}: {LowerEndEffectorName}");
                builder.AppendLine($"{nameof(RobotHomeMotionProfile)}: {RobotHomeMotionProfile}");
            }

            return builder.ToString();
        }
    }

    #region Inner Classes

    /// <summary>
    /// Used for serializing <see cref="SampleSizeStoppingPositions"/>.
    /// </summary>
    [Serializable]
    public class SampleSizeStoppingPositionsContainer
    {
        public SampleDimension WaferSize { get; set; }

        public SampleSizeStoppingPositions SampleSizeStoppingPositions { get; set; }
    }

    /// <summary>
    /// Represent possible <see cref="ModuleStoppingPosition"/> for each <see cref="TransferLocation"/> for the current <see cref="SampleDimension"/>.
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

            public ModuleStoppingPosition ModuleStoppingPosition { get; set; }
        }

        /// <summary>
        /// Represent possible <see cref="ModuleStoppingPosition"/> for each <see cref="TransferLocation"/>.
        /// </summary>
        /// <remarks>
        /// Not directly serializable.
        /// Need to convert it into <see cref="StoppingPositionPerModuleSerializableData"/> for serialization.
        /// </remarks>
        [XmlIgnore]
        [JsonIgnore]
        public Dictionary<TransferLocation, ModuleStoppingPosition> StoppingPositionsPerModule { get; set; }

        [XmlArray(ElementName = "StoppingPositionsPerWaferSize")]
        [XmlArrayItem(ElementName = "ModuleStoppingPositions")]
        public ModuleStoppingPositionContainer[] StoppingPositionPerModuleSerializableData
        {
            get => StoppingPositionsPerModule.Select(elem => new ModuleStoppingPositionContainer
            {
                Module = elem.Key,
                ModuleStoppingPosition = elem.Value
            })
                .ToArray();
            set => StoppingPositionsPerModule =
                value.ToDictionary(elem => elem.Module, elem => elem.ModuleStoppingPosition);
        }

        public SampleSizeStoppingPositions()
        {
            StoppingPositionsPerModule = new Dictionary<TransferLocation, ModuleStoppingPosition>();
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


    /// <summary>
    /// Represent possible stopping positions for current <see cref="TransferLocation"/>.
    /// </summary>
    [Serializable]
    [DataContract]
    public class ModuleStoppingPosition
    {
        [DataMember]
        public double XPosition { get; set; }

        [DataMember]
        public double ThetaPosition { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder(base.ToString());

            builder.AppendLine($"XPosition : {XPosition}");
            builder.AppendLine($"ThetaPosition : {ThetaPosition}");

            return builder.ToString();
        }
    }

    #endregion Inner Classes
}
