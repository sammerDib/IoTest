using System.ComponentModel.Composition;

namespace UnitySC.PM.Shared.UC
{
    [InheritedExport(typeof(IConfigurationUc))] // Export all classes inherited this interface (MEF)
    public interface IConfigurationUc : IPmUc
    {
    }
}
