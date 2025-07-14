using UnitySC.Shared.Configuration;

namespace UnitySC.PP.Shared.Configuration
{
    public interface IPPServiceConfigurationManager : IServiceConfigurationManager
    {
        string PPConfigurationFilePath { get; }        
    }
}
