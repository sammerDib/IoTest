namespace ConfigurationManager.Configuration
{
    public enum ConfigurationType
    {
        Bool,
        Folder,
        String,
        File,
        SQLConnectionString,
        ResultDb,
        WcfAddress,
        ProductionMode,
        LogLevel,
        Int,
        StartupMode,
        BaseAddress,
    }

    public enum LogLevel
    {
        Debug,
        Warning,
        Information,
        Error
    }

    public enum ProductionMode
    {
        InADC,
        InAcquisition
    }

    public enum ApplicationType
    {
        ADCEditor,
        ADCProd,
        AdaToAdc,
        ADCConfiguration
    }

    public enum SettingState
    {
        Error,
        Valid,
        InProgress
    }

    public enum StartupMode
    {
        ExpertRecipeEdition,
        SimplifiedRecipeEdition
    }
}
