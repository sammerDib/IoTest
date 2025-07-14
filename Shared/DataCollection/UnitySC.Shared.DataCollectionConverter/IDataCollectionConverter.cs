using System.ComponentModel.Composition;

using UnitySC.Shared.Data.DVID;
using UnitySC.Shared.Data.SecsGem;

namespace UnitySC.Shared.DataCollectionConverter
{

    [InheritedExport(typeof(IDataCollectionConverter))] // Export all classes inherited this interface (MEF)
    public interface IDataCollectionConverter
    {
        SecsVariableList ConvertToSecsVariableList(ModuleDataCollection moduleDataCollection);

    }
}
