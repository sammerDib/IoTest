using System;
using System.IO;

using UnitySC.PM.Shared;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Service.Core.Shared
{
    [Serializable]
    public class EmePMConfiguration : PMConfiguration
    {
        public new static PMConfiguration Init(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"PMConfiguration file is missing <{path}>");
            
            var pmConfig = XML.Deserialize<PMConfiguration>(path);
            
            bool useLocalAddresses = ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>().UseLocalAddresses;
            if (!useLocalAddresses)
            {
                return pmConfig;
            }
            
            const string localRootFolder = "C:\\EMERA";
            if (pmConfig.DataAccessAddress != null)
                pmConfig.DataAccessAddress.Host = "localhost";
            if (pmConfig.DataFlowAddress != null)
                pmConfig.DataFlowAddress.Host = "localhost";
            if (pmConfig.UTOAddress != null)
                pmConfig.UTOAddress.Host = "localhost";

            pmConfig.OutputAcqServer = ConvertNetworkPathToLocalPath(pmConfig.OutputAcqServer, localRootFolder);
            pmConfig.OutputAdaFolder = ConvertNetworkPathToLocalPath(pmConfig.OutputAdaFolder, localRootFolder);
            return pmConfig;
        }

        private static string ConvertNetworkPathToLocalPath(string networkPath, string localDrive)
        {
            if (string.IsNullOrEmpty(networkPath))
            {
                return networkPath;
            }

            // Extract the share name from the network path
            string[] parts = networkPath.Split(new [] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
            {
                throw new ArgumentException("The path provided is not a valid network path.");
            }

            string shareName = parts[0];

            // Remove the share name from the network path
            string relativePath = networkPath.Substring(shareName.Length + 1);

            // Combine the local drive with the relative path
            return Path.Combine(localDrive, relativePath);
        }
    }
}
