using System;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Devices.LightTower.Enums;

namespace UnitySC.Equipment.Abstractions.Devices.LightTower.Configuration
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class LightTowerStatus
    {
        #region Properties

        /// <summary>
        /// Get or Set the <see cref="LightTowerStatus"/> description
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string Description { get; set; }

        /// <summary>
        /// Get or Set the BuzzerState
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public BuzzerState BuzzerState { get; set; }

        /// <summary>
        /// Get or Set the Blue light state
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public LightState Blue { get; set; }

        /// <summary>
        /// Get or Set the Green light state
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public LightState Green { get; set; }

        /// <summary>
        /// Get or Set the Orange light state
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public LightState Orange { get; set; }

        /// <summary>
        /// Get or Set the Red light state
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public LightState Red { get; set; }

        /// <summary>
        /// Get or Set the LightTowerState
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public LightTowerState LightTowerState { get; set; }

        #endregion Properties

        #region Constructor

        public LightTowerStatus()
        {
            SetDefaults();
        }

        #endregion

        #region Override methods

        /// <inheritdoc />
        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append('[').Append(LightTowerState).AppendLine("]");
            builder.Append("Description : ").AppendLine(Description);
            builder.Append("Buzzer : ").Append(BuzzerState).AppendLine();
            builder.Append("Blue : ").Append(Blue).AppendLine();
            builder.Append("Green : ").Append(Green).AppendLine();
            builder.Append("Orange : ").Append(Orange).AppendLine();
            builder.Append("Red : ").Append(Red).AppendLine();
            builder.Append("State : ").Append(LightTowerState).AppendLine();

            return builder.ToString();
        }

        #endregion Override methods

        #region Private Methods

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            SetDefaults();
        }

        private void SetDefaults()
        {
            Description = string.Empty;
            BuzzerState = BuzzerState.Undetermined;
            Blue = LightState.Undetermined;
            Green = LightState.Undetermined;
            Orange = LightState.Undetermined;
            Red = LightState.Undetermined;
            LightTowerState = LightTowerState.AllTheLightsOff;
        }

        #endregion
    }
}
