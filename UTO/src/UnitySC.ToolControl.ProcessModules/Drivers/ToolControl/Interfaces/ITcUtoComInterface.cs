using System.Runtime.InteropServices;

using UnitySC.ToolControl.ProcessModules.Drivers.ToolControl.Interfaces.SecsGem;

namespace UnitySC.ToolControl.ProcessModules.Drivers.ToolControl.Interfaces
{
    [
        ComVisible(true),
        Guid(Constants.ITcUtoComInterfaceString),
        InterfaceType(ComInterfaceType.InterfaceIsDual)
    ]
    public interface ITcUtoComInterface
    {
        void AreJobsActive(ref bool value);
        void Advise();
        void ReadyForSubstrateLoad(string moduleId, bool success);
        void ReadyForSubstrateUnload(string moduleId, bool success);
        void SetModuleState(string processModuleId, ModuleState moduleState);
        void TriggerSubstrateLoaded(string moduleId);
        void TriggerSubstrateUnloaded(string moduleId);
        void TriggerSubstratePresent(string moduleId, bool value);
        void ProcessProgramModificationNotify(string processProgramID, PPChangeState state);
        void SendCollectionEvent(string collectionEventName, ISecsVariableList dataVariables);
        void SendDataSet_S13F13(ITableData_S13F13 tableData);
        void SendDataSet_S13F16(ITableData_S13F16 tableData);
        void SetEquipmentState(EquipmentState equipmentState);
        void RequestChangeOperationMode(OperationMode operationMode, ref bool success, ref string errorMessage);
        void GetUTOEquipmentState(ref EquipmentState equipmentState);
        void Unadvise();

        ISecsDataFactory DataFactory { get; }
    }
}
