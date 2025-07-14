using System.ComponentModel.Composition;

namespace UnitySC.PM.Shared.UC
{
    /// <summary>
    /// Initialisation of the process module
    /// </summary>
    [InheritedExport(typeof(IPmInit))] // Export all classes inherited this interface (MEF)
    public interface IPmInit : IPmUc
    {
        void BootStrap();
    }
}
