namespace UnitySC.Shared.Configuration
{
    public interface IServiceConfigurationManager    
	{
        string ConfigurationFolderPath { get; }
        string ConfigurationName { get; }
        string LogConfigurationFilePath { get; }
        bool MilIsSimulated { get; }
        string GetStatus();
    }
}
