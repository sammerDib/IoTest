namespace UnitySC.PM.Shared
{
    public interface IClientConfigurationManager
    {
        string ConfigurationFolderPath { get; }
        string ClientConfigurationFilePath { get; }
        string LogConfigurationFilePath { get; }

        string GetStatus();

        bool UseLocalAddresses { get; }

        bool IsWaferLessMode { get; }
    }
}
