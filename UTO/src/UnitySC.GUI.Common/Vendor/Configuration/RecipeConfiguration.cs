using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

using Agileo.Recipes.Management.StorageFormats;

namespace UnitySC.GUI.Common.Vendor.Configuration
{
    /// <summary>
    /// Define the configuration of Recipe
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "")]
    public class RecipeConfiguration : FilesBaseConfiguration
    {
        #region Private Methods

        #region Overrides

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine("<Recipe Configuration>");
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
            Path = System.IO.Path.Combine(currentExecutingPath, "Configuration", "Recipes");

            StorageFormat = StorageFormat.XML;
            FileExtension = "xml";
        }

        #endregion
    }
}
