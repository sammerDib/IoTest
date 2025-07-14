using System;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace UnitySC.GUI.Common.Vendor.Configuration
{
    /// <summary>
    /// Defines all Application Paths
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "")]
    public class ApplicationPath
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationPath"/> class.
        /// </summary>
        public ApplicationPath()
        {
            SetDefaults();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the On-Line User Manual path.
        /// </summary>
        /// <value>\Manual\UM.pdf</value>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string UserManualPath { get; set; }

        /// <summary>
        /// Gets or sets the Automation Config path.
        /// </summary>
        /// <value>\Manual\UM.pdf</value>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string AutomationConfigPath { get; set; }

        /// <summary>
        /// Gets or sets the Automation Log path.
        /// </summary>
        /// <value>\Manual\UM.pdf</value>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string AutomationLogPath { get; set; }

        /// <summary>
        /// Gets or sets the Automation Variables path.
        /// </summary>
        /// <value>\OutputFiles\Variables</value>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string AutomationVariablesPath { get; set; }

        /// <summary>
        /// Gets or sets the Dcp Storage path.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string DcpStoragePath { get; set; }

        /// <summary>
        /// Gets or sets the AlarmAnalysis capture storage path.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string AlarmAnalysisCaptureStoragePath { get; set; }

        /// <summary>
        /// Gets or sets the data monitoring configuration file path.
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string DataMonitoringPath { get; set; }

        /// <summary>
        /// Gets or sets the DataFlow client configuration folder path
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string DfClientConfigurationFolderPath { get; set; }
        #endregion

        #region Overrides

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine("<Path Application>");
            builder.AppendLine($"User Manual Path: {UserManualPath}");
            builder.AppendLine($"Automation Config Path: {AutomationConfigPath}");
            builder.AppendLine($"Automation Log Path: {AutomationLogPath}");
            builder.AppendLine($"Automation Variables Path: {AutomationVariablesPath}");
            builder.AppendLine($"Data monitoring Path: {DataMonitoringPath}");
            builder.AppendLine($"DataFlow Client configuration folder Path: {DfClientConfigurationFolderPath}");
            return builder.ToString();
        }

        #endregion

        #region Private Methods

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context) => SetDefaults();

        /// <summary>
        /// Sets the default values (called on deserializing and from constructor)
        /// </summary>
        private void SetDefaults()
        {
            UserManualPath = @".\Documentation\User Manual.xps";
            AutomationConfigPath = @".\Configuration\XML";
            AutomationLogPath = @".\Data\Logs\Automation";
            AutomationVariablesPath = @".\Data\Variables";
            DcpStoragePath = @".\Data\DataCollectionPlan\CollectedData";
            AlarmAnalysisCaptureStoragePath = @".\Data\Alarms\Captures";
            DataMonitoringPath = @"Configuration\\XML\\DataMonitoring.xml";
            DfClientConfigurationFolderPath = @".\Configuration\XML";
        }

        #endregion
    }
}
