using System.IO;
using System.Xml.Serialization;

namespace UnitySC.DataAccess.Service.Implementation
{
    public class DataAccessConfiguration
    {
        private static volatile DataAccessConfiguration s_instance;
        private static readonly object s_syncRoot = new object();
        public static string SettingsFilePath;
        

        public static DataAccessConfiguration Instance
        {
            get
            {
                if (s_instance == null)
                {
                    lock (s_syncRoot)
                    {
                        if (s_instance == null)
                        {
                            s_instance = new DataAccessConfiguration();

                            if (string.IsNullOrEmpty(SettingsFilePath))
                                throw new System.Exception("SettingsFilePath must be set before getting an instance");
                            Instance.Load(SettingsFilePath);
                        }
                    }
                }

                return s_instance;
            }
        }

        #region Methods

        /// <summary>
        ///     Load settings from xml file
        /// </summary>
        /// <param name="filePath">Xml file path</param>
        private void Load(string filePath)
        {
            if (File.Exists(filePath))
            {
                var xs = new XmlSerializer(typeof(DataAccessConfiguration));

                using (var rd = new StreamReader(filePath))
                {
                    s_instance = xs.Deserialize(rd) as DataAccessConfiguration;
                }
            }
            else
                throw new System.ArgumentException($"The configuration file {filePath} does not exist");
        }

        public string TemplateResultFolderPath { get; set; }
        public string TemplateResultFileName { get; set; }
        public string TemplateTCPMRecipeName { get; set; }
        public string RootExternalFilePath { get; set; }
        public string TemplateExternalFilePath { get; set; }
        public string DbConnectionString { get; set; }
        public string DBBackupFolder { get; set; }

        #endregion Methods
    }
}
