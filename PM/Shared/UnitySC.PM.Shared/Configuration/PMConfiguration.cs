using System;
using System.IO;

using UnitySC.Shared.Data;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared
{
    /// <summary>
    ///  Définition du fichier XML de configuration des process modules
    /// </summary>
    [Serializable]
    public class PMConfiguration : ModuleConfiguration
    {
        static public new PMConfiguration Init(string path)
        {
            

            if (!File.Exists(path))
                throw new FileNotFoundException($"PMConfiguration file is missing <{path}>");
            var PMConfig = XML.Deserialize<PMConfiguration>(path);
            var UseLocalAddresses = ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>().UseLocalAddresses;
            if (UseLocalAddresses)
            {
                var localRootFolder = "C:\\UnitySC";
                if (PMConfig.DataAccessAddress != null)
                    PMConfig.DataAccessAddress.Host = "localhost";
                if (PMConfig.DataFlowAddress != null)
                    PMConfig.DataFlowAddress.Host = "localhost";
                if (PMConfig.UTOAddress != null)
                    PMConfig.UTOAddress.Host = "localhost";

                PMConfig.OutputAcqServer = ConvertNetworkPathToLocalPath(PMConfig.OutputAcqServer, localRootFolder);
                PMConfig.OutputAdaFolder = ConvertNetworkPathToLocalPath(PMConfig.OutputAdaFolder, localRootFolder);
            }
            return PMConfig;
        }

        private static string ConvertNetworkPathToLocalPath(string networkPath, string localDrive)
        {
            if (string.IsNullOrEmpty(networkPath))
            {
                return networkPath;
            }
            // Ensure the network path starts with the expected UNC prefix
            if (!networkPath.StartsWith(@"\\"))
            {
                return networkPath;
            }

            // Extract the share name from the network path
            string[] parts = networkPath.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
            {
                throw new ArgumentException("The path provided is not a valid network path.");
            }

            string shareName = parts[0] ;

            // Remove the share name from the network path
            string relativePath = networkPath.Substring(shareName.Length + 3);

            // Combine the local drive with the relative path
            string localPath = Path.Combine(localDrive, relativePath);

            return localPath;
        }
    }


}
