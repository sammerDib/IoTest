using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

using UnitySC.Equipment.Abstractions.Configuration;

namespace UnitySC.Equipment.Abstractions.Devices.LightTower.Configuration
{
    [Serializable]
    public class LightTowerConfiguration : DeviceConfiguration
    {
        #region Properties

        /// <summary>
        /// Get or Set the collection of <see cref="LightTowerStatus"/>
        /// </summary>
        [XmlArray("LightTowerStatuses", IsNullable = true)]
        [XmlArrayItem("LightTowerStatus")]
        public List<LightTowerStatus> LightTowerStatuses { get; set; }

        #endregion Properties

        #region Constructor

        public LightTowerConfiguration()
        {
            SetDefaultsInternal();
        }

        #endregion

        #region Override methods

        /// <inheritdoc />
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine(base.ToString());

            if (LightTowerStatuses != null)
            {
                foreach (var ltStates in LightTowerStatuses)
                {
                    builder.Append(ltStates).AppendLine();
                }
            }

            return builder.ToString();
        }

        protected override void SetDefaults()
        {
            LightTowerStatuses = new List<LightTowerStatus>();
        }

        #endregion Override methods

        #region Private methods

        private void SetDefaultsInternal()
        {
            SetDefaults();
        }

        #endregion

    }
}
