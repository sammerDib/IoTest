using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ConfigurationManager.Configuration
{
    public class GlobalConfiguration
    {
        private const string _configExtension = ".exe.config";
        private const string WcfHostKey = "Wcf.Host";
        private const string WcfClientKey = "Wcf.Client";
        private const string BaseAddressKey = "BaseAddress";

        public List<Setting> Settings { get; set; }

        public GlobalConfiguration()
        {
            List<ApplicationType> alls = Enum.GetValues(typeof(ApplicationType)).Cast<ApplicationType>().ToList();
            List<ApplicationType> allExceptADCConfiguration = alls.Where(x => x != ApplicationType.ADCConfiguration).ToList();
            Settings = new List<Setting>();
            Settings.Add(new Setting() { Key = "KlarfExtension", ConfigurationType = ConfigurationType.String, UsedIn = allExceptADCConfiguration, Help = "Klarf file extension" });
            Settings.Add(new Setting() { Key = "LogFolder", ConfigurationType = ConfigurationType.Folder, UsedIn = alls, Help = "Directory for logs" });
            Settings.Add(new Setting() { Key = "serilog:minimum-level", ConfigurationType = ConfigurationType.LogLevel, UsedIn = alls, Help = "Log level" });
            Settings.Add(new Setting() { Key = "DatabaseConfig.AdditionnalRecipeFiles.ServerDirectory", ConfigurationType = ConfigurationType.Folder, UsedIn = alls, Help = "Directory shared on database computer with the additionnal recipe files" });
            Settings.Add(new Setting() { Key = "Editor.RecipeFolder", ConfigurationType = ConfigurationType.Folder, UsedIn = allExceptADCConfiguration, Help = "Directory for recipes" });
            Settings.Add(new Setting() { Key = "Editor.MetablockFolder", ConfigurationType = ConfigurationType.Folder, UsedIn = allExceptADCConfiguration, Help = "Directory for Metablock" });
            Settings.Add(new Setting() { Key = "Editor.StartupMode", ConfigurationType = ConfigurationType.StartupMode, UsedIn = allExceptADCConfiguration, Help = "Startup mode. Expert or Simplified. If AdcEditor start as simplified mode the expert mode is not available" });
            Settings.Add(new Setting() { Key = "Editor.HelpFolder", ConfigurationType = ConfigurationType.Folder, UsedIn = allExceptADCConfiguration, Help = "Directory for Adc Help" });
            Settings.Add(new Setting() { Key = "AdaFolder", ConfigurationType = ConfigurationType.Folder, UsedIn = allExceptADCConfiguration, Help = "Directory for Ada" });
            Settings.Add(new Setting() { Key = "DatabaseConfig.ServerName", ConfigurationType = ConfigurationType.SQLConnectionString, UsedIn = alls, Custom = true, Help = @"Sql server connection for database configuration ([ServerName]\[Instance])" });
            Settings.Add(new Setting() { Key = "DatabaseConfig.UseExportedDatabase", ConfigurationType = ConfigurationType.Bool, UsedIn = allExceptADCConfiguration, Help = "True => Use a local exported configuration file (adcdb), False => Direct connection to database config (DatabaseConfig.ServerName)" });
            Settings.Add(new Setting() { Key = "DatabaseConfig.ExportedDatabaseFile", ConfigurationType = ConfigurationType.String, UsedIn = allExceptADCConfiguration, Expert = true, Help = "Path for the exported configuration file (adcdb)" });
            Settings.Add(new Setting() { Key = "DatabaseConfig.UseDatabaseToGetRecipes", ConfigurationType = ConfigurationType.Bool, UsedIn = allExceptADCConfiguration, Expert = true, Help = "True => Use the database in AdaToAdc to get recipe, False => Use path to get recipe" });
            Settings.Add(new Setting() { Key = "DatabaseConfig.RecipeCache", ConfigurationType = ConfigurationType.Folder, UsedIn = allExceptADCConfiguration, Expert = true, Help = "Local directory for recipe cache between database config and Adc" });
            Settings.Add(new Setting() { Key = "DatabaseResults.ServerName", ConfigurationType = ConfigurationType.ResultDb, UsedIn = allExceptADCConfiguration, Help = @"Sql server connection for database results([ServerName]\[Instance])" });
            Settings.Add(new Setting() { Key = "DatabaseResults.Use", ConfigurationType = ConfigurationType.Bool, UsedIn = allExceptADCConfiguration, Help = "True => Add result in database results" });
            Settings.Add(new Setting() { Key = "DeepControl.AddressClient", ConfigurationType = ConfigurationType.String, UsedIn = allExceptADCConfiguration, Help = "Address Client for DeepControl Module" });
            //Settings.Add(new Setting() { Key = "Grading.Path", ConfigurationType = ConfigurationType.Folder, UsedIn = allExceptADCConfiguration, Help = "Directory for Sorting results" });            
            Settings.Add(new Setting() { Key = "AdcEngine.ProductionMode", ConfigurationType = ConfigurationType.ProductionMode, UsedIn = allExceptADCConfiguration, Help = "InADC => Adc engine is executed in AdcEditor, InAcquisition => Adc engine is executed in acquisition or AdaToV10" });
            Settings.Add(new Setting() { Key = "AdcEngine.NbTasksPerPool", ConfigurationType = ConfigurationType.Int, UsedIn = allExceptADCConfiguration, Help = "Number of tasks per pool" });
            Settings.Add(new Setting() { Key = "Debug.ImageViewer", ConfigurationType = ConfigurationType.File, UsedIn = allExceptADCConfiguration, Expert = true, Help = "Image viewer in debug" });
            Settings.Add(new Setting() { Key = "AdaToAdc.TestMode.PreloadImages", ConfigurationType = ConfigurationType.Bool, UsedIn = allExceptADCConfiguration, Expert = true, Help = "True => Preload images before recipe execution for test" });
            Settings.Add(new Setting() { Key = "AdaToAdc.TestMode.AlwaysSendTheSameImage", ConfigurationType = ConfigurationType.Bool, UsedIn = allExceptADCConfiguration, Expert = true, Help = "True => Always send the same image for test" });
            Settings.Add(new Setting() { Key = "AdaToAdc.TransferToRobot.Enable", ConfigurationType = ConfigurationType.Bool, UsedIn = allExceptADCConfiguration, Expert = true, Help = "True => Send information to robot" });
            Settings.Add(new Setting() { Key = "AdaToAdc.TransferToRobot.Embedded", ConfigurationType = ConfigurationType.Bool, UsedIn = allExceptADCConfiguration, Expert = true, Help = "True => TransferToRobot is embedded in engine" });

            Settings.Add(new Setting() { Key = "BaseAddress.DataAccess", ConfigurationType = ConfigurationType.BaseAddress, UsedIn = alls, Custom = true, Help = "Base address for DataAccess services (ex: localhost:2221 or 10.100.20.155:2345)" });
            Settings.Add(new Setting() { Key = "BaseAddress.DataFlow", ConfigurationType = ConfigurationType.BaseAddress, UsedIn = alls, Custom = true, Help = "Base address for DataFlow services (ex: localhost:2221 or 10.100.20.155:2345)" });

            Settings.Add(new Setting() { Key = "Wcf.Host.ADCEngine.AdcExecutor", ConfigurationType = ConfigurationType.WcfAddress, UsedIn = allExceptADCConfiguration, Custom = true, Expert = true, Help = "Wcf host address for exection and aquisition" });
            Settings.Add(new Setting() { Key = "Wcf.Host.AdcToRobot.TransferToRobot", ConfigurationType = ConfigurationType.WcfAddress, UsedIn = allExceptADCConfiguration, Custom = true, Expert = true, Help = "Wcf host address to send information to robot" });
            Settings.Add(new Setting() { Key = "Wcf.Client.IAdcExecutor", ConfigurationType = ConfigurationType.WcfAddress, UsedIn = allExceptADCConfiguration, Custom = true, Expert = true, Help = "Wcf client address for execution" });
            Settings.Add(new Setting() { Key = "Wcf.Client.IAdcAcquisition", ConfigurationType = ConfigurationType.WcfAddress, UsedIn = allExceptADCConfiguration, Custom = true, Expert = true, Help = "Wcf client address for acquistion" });
            Settings.Add(new Setting() { Key = "Wcf.Client.ITransferToRobot", ConfigurationType = ConfigurationType.WcfAddress, UsedIn = allExceptADCConfiguration, Custom = true, Expert = true, Help = "Wcf client address to send information to robot" });

        }

        public void LoadValues()
        {
            StringBuilder error = new StringBuilder();
            foreach (ApplicationType appType in Enum.GetValues(typeof(ApplicationType)))
            {
                XmlDocument doc = new XmlDocument();
                string fileName = appType + _configExtension;
                Setting currentSetting = null;
                try
                {
                    doc.Load(fileName);
                    foreach (Setting setting in Settings.Where(x => x.UsedIn.Contains(appType)))
                    {
                        currentSetting = setting;
                        if (setting.Custom)
                        {
                            setting.Values.Add(appType, LoadCustomConfig(doc, setting.Key));
                        }
                        else
                        {
                            var node = doc.DocumentElement.SelectSingleNode("/configuration/appSettings//add[@key='" + setting.Key + "']/@value");
                            if (node != null)
                                setting.Values.Add(appType, node.Value);
                            else
                                setting.Values.Add(appType, "");
                        }
                    }
                }
                catch
                {
                    error.AppendLine("Error in load file " + fileName + " Key: " + currentSetting?.Key + " is missing");
                }
            }

            if (!string.IsNullOrEmpty(error.ToString()))
                System.Windows.MessageBox.Show(error.ToString());
        }

        public void SaveValues()
        {
            StringBuilder error = new StringBuilder();
            foreach (ApplicationType appType in Enum.GetValues(typeof(ApplicationType)))
            {
                XmlDocument doc = new XmlDocument();
                string fileName = appType + _configExtension;
                Setting currentSetting = null;
                try
                {
                    doc.Load(fileName);
                    foreach (Setting setting in Settings.Where(x => x.UsedIn.Contains(appType)))
                    {
                        currentSetting = setting;
                        if (setting.Custom)
                            UpdateCustomConfig(doc, setting.Key, setting.Values[appType]);
                        else
                        {
                            var node = doc.DocumentElement.SelectSingleNode("/configuration/appSettings//add[@key='" + setting.Key + "']/@value");
                            if (node != null)
                            {
                                doc.DocumentElement.SelectSingleNode("/configuration/appSettings//add[@key='" + setting.Key + "']/@value").Value = setting.Values[appType];
                            }
                            else
                            {   // cette ligne n'existe pas . Il faut la créer
                                XmlNode xNode = doc.CreateNode(XmlNodeType.Element, "add", "");
                                XmlAttribute xKey = doc.CreateAttribute("key");
                                XmlAttribute xValue = doc.CreateAttribute("value");
                                xKey.Value = setting.Key;
                                xValue.Value = setting.Values[appType];
                                xNode.Attributes.Append(xKey);
                                xNode.Attributes.Append(xValue);
                                doc.GetElementsByTagName("appSettings")[0].InsertAfter(xNode,
                                doc.GetElementsByTagName("appSettings")[0].LastChild);
                            }
                        }
                    }

                    doc.Save(fileName);
                }
                catch (Exception ex)
                {
                    error.AppendLine($"Error in save file <{fileName}>, Key: <{currentSetting?.Key}> is missing [msg={ex.Message}]");
                }
            }

            if (!string.IsNullOrEmpty(error.ToString()))
                throw new Exception(error.ToString());
        }

        public string LoadCustomConfig(XmlDocument doc, string key)
        {

            XmlNode res = null;
            if (key == "DatabaseConfig.ServerName")
            {
                res = doc.DocumentElement.SelectSingleNode("/configuration/connectionStrings//add[@name='InspectionEntities']/@connectionString");
            }
            else if (key.StartsWith(BaseAddressKey))
            {
                var SubIdkey = key.Substring(BaseAddressKey.Length + 1);
                string searchContractPattern = SubIdkey.ToLower();

                var clientendpoints = doc.DocumentElement.SelectSingleNode(string.Format("/configuration/system.serviceModel/client"));
                if (clientendpoints != null)
                {
                    foreach (XmlNode endpoint in clientendpoints.ChildNodes)
                    {
                        if (endpoint.NodeType == XmlNodeType.Comment)
                            continue;

                        var contract = endpoint.SelectSingleNode($"./@contract");
                        if (contract != null && contract.Value.ToLower().Contains(searchContractPattern))
                        {
                            var address = endpoint.SelectSingleNode($"./@address");
                            var nameattr = endpoint.SelectSingleNode($"./@name");
                            var fulladress = address.Value;
                            string startaddress = "net.tcp://";
                            string endaddress = $"/{nameattr.Value}";

                            // return the first occurence of relative service baseaddress
                            return fulladress.Substring(startaddress.Length, fulladress.Length - (startaddress.Length + endaddress.Length));
                        }
                    }
                }
            }
            else if (key.StartsWith(WcfHostKey))
            {
                res = doc.DocumentElement.SelectSingleNode(string.Format("/configuration/system.serviceModel/services//service[@name='{0}']/host/baseAddresses/add/@baseAddress", key.Substring(WcfHostKey.Length + 1)));
            }
            else if (key.StartsWith(WcfClientKey))
            {
                res = doc.DocumentElement.SelectSingleNode(string.Format("/configuration/system.serviceModel/client//endpoint[@name='{0}']/@address", key.Substring(WcfClientKey.Length + 1)));
            }
            return res?.Value;
        }



        public void UpdateCustomConfig(XmlDocument doc, string key, string newValue)
        {
            XmlNode res = null;
            if (key == "DatabaseConfig.ServerName")
            {
                res = doc.DocumentElement.SelectSingleNode("/configuration/connectionStrings//add[@name='InspectionEntities']/@connectionString");
                res.Value = newValue;
            }
            else if (key.StartsWith(BaseAddressKey))
            {
                var SubIdkey = key.Substring(BaseAddressKey.Length + 1);
                string searchContractPattern = SubIdkey.ToLower();

                var clientendpoints = doc.DocumentElement.SelectSingleNode(string.Format("/configuration/system.serviceModel/client"));
                if (clientendpoints != null)
                {
                    foreach (XmlNode endpoint in clientendpoints.ChildNodes)
                    {
                        if (endpoint.NodeType == XmlNodeType.Comment)
                            continue;

                        var contract = endpoint.SelectSingleNode($"./@contract");
                        if (contract.Value.ToLower().Contains(searchContractPattern))
                        {
                            var address = endpoint.SelectSingleNode($"./@address");
                            var nameattr = endpoint.SelectSingleNode($"./@name");
                            address.Value = $"net.tcp://{newValue}/{nameattr.Value}";
                        }
                    }
                }
            }
            else if (key.StartsWith(WcfHostKey))
            {
                var test = key.Substring(WcfHostKey.Length + 1);
                res = doc.DocumentElement.SelectSingleNode(string.Format("/configuration/system.serviceModel/services//service[@name='{0}']/host/baseAddresses/add/@baseAddress", key.Substring(WcfHostKey.Length + 1)));
                res.Value = newValue;
            }
            else if (key.StartsWith(WcfClientKey))
            {
                res = doc.DocumentElement.SelectSingleNode(string.Format("/configuration/system.serviceModel/client//endpoint[@name='{0}']/@address", key.Substring(WcfClientKey.Length + 1)));
                res.Value = newValue;
            }
        }
    }
}
