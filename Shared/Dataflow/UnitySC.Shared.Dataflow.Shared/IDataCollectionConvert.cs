using UnitySC.Shared.Data.DVID;
using UnitySC.Shared.Data.SecsGem;

namespace UnitySC.Shared.Dataflow.Shared
{
    public interface IDataCollectionConvert
    {
        SecsVariableList ConvertToSecsVariableList(ModuleDataCollection moduleDataCollection);
    }
}
