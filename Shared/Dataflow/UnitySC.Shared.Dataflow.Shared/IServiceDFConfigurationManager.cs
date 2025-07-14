namespace UnitySC.PM.Shared
{
    public interface IServiceDFConfigurationManager
    {
        string ConfigurationFolderPath { get; }
        string InputConfigurationName { get; }
        string DFServerConfigurationFilePath { get; }
        string LogConfigurationFilePath { get; }
        bool UseLocalAddresses { get; }
        string GetStatus();
    }
}
