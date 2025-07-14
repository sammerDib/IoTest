using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

using Agileo.Recipes.Management.StorageFormats;

namespace UnitySC.GUI.Common.Vendor.Configuration
{
    /// <summary>
    /// Define the configuration of Scenario
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "")]
    public class ScenarioConfiguration : FilesBaseConfiguration
    {
        #region Private Methods

        #region Overrides

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine("<Scenario Configuration>");
            builder.AppendLine(base.ToString());
            return builder.ToString();
        }

        #endregion

        /// <summary>
        /// Sets the default values (called on deserializing and from constructor)
        /// </summary>
        protected override void SetDefaults()
        {
            var currentExecutingPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Path = System.IO.Path.Combine(currentExecutingPath, "Configuration", "Scenarios");

            StorageFormat = StorageFormat.XML;
            FileExtension = "scenario";
        }

        #endregion
    }
}
