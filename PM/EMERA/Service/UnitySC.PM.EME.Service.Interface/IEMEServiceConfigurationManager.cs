using UnitySC.PM.Shared;

namespace UnitySC.PM.EME.Service.Interface
{
    public interface IEMEServiceConfigurationManager : IPMServiceConfigurationManager
    {
        string RecipeConfigurationFilePath { get; }
    }
}
