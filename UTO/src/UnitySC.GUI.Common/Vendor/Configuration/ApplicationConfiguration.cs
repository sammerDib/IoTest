using System;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

using Agileo.AlarmModeling.Configuration;
using Agileo.GUI.Configuration;

namespace UnitySC.GUI.Common.Vendor.Configuration
{
    /// <summary>
    /// Defines the Configuration of the Application.
    /// Aims to centralize all options associated to the System.
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "")]
    public class ApplicationConfiguration : AgileoGuiConfiguration
    {
        #region Properties

        /// <summary>
        /// Gets or sets Application File Paths.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public ApplicationPath ApplicationPath { get; set; }

        ///// <summary>
        ///// Gets or sets Configuration of the Alarm Center System.
        ///// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public AlarmCenterConfiguration Alarms { get; set; }
        

        /// <summary>
        /// Gets or Sets configuration of recipe manager
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public RecipeConfiguration RecipeConfiguration { get; set; }
        

        /// <summary>
        /// Gets or Sets configuration of scenario manager
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public ScenarioConfiguration ScenarioConfiguration { get; set; }

        /// <summary>
        /// Gets or sets Configuration of the user interface
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public UserInterfaceConfiguration UserInterfaceConfiguration { get; set; }

        /// <summary>
        /// Gets or sets Configuration of the equipment identity
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public EquipmentIdentityConfig EquipmentIdentityConfig { get; set; }

        #endregion Properties

        #region Override methods

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine("<ApplicationConfiguration>");
            builder.AppendLine(base.ToString());
            builder.AppendLine(ApplicationPath.ToString());
            builder.AppendLine(Alarms.ToString());
            builder.AppendLine(UserInterfaceConfiguration.ToString());
            builder.AppendLine(EquipmentIdentityConfig.ToString());

            return builder.ToString();
        }

        protected override void SetDefaults()
        {
            ApplicationPath = new ApplicationPath();
            Alarms = new AlarmCenterConfiguration();
            RecipeConfiguration = new RecipeConfiguration();
            ScenarioConfiguration = new ScenarioConfiguration();
            UserInterfaceConfiguration = new UserInterfaceConfiguration();
            EquipmentIdentityConfig = new EquipmentIdentityConfig();
        }

        #endregion Override methods
    }
}
