using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

using UnitySC.ToolControl.ProcessModules.Drivers.ToolControl.Interfaces.SecsGem;

namespace UnitySC.ToolControl.ProcessModules.Drivers.ToolControl.Interfaces
{
    [
       ComVisible(true),
       Guid(Constants.ITcUtoComInterfaceEventsString),
       InterfaceType(ComInterfaceType.InterfaceIsIDispatch),
   ]
    public interface ITcUtoComInterfaceEvents
    {
        [DispId(1)]
        void OnClose();

        [DispId(2)]
        void OnGetAllAvailableRecipeNames(IComStringList recipeNames);

        [DispId(3)]
        void OnGetAvailableModules(IProcessModuleCollection processModulecollection);

        [DispId(4)]
        void OnGetModuleState(string moduleId, ref ModuleState moduleState);

        [DispId(5)]
        void OnGetModuleAlignmentAngle(string moduleId, ref double angle);

        [DispId(6)]
        void OnCreateProcessJob(IProcessJob processJob);

        [DispId(7)]
        void OnUpdateProcessJobFlowRecipe(IProcessJob processJob, ref bool success);

        [DispId(8)]
        void OnDeleteProcessJob(IProcessJob processJob);

        [DispId(9)]
        void OnAbortProcessJob(IProcessJob processJob);

        [DispId(10)]
        void OnSelectModuleRecipe(IProcessJob processJob, string moduleId, string moduleRecipeId);

        [DispId(11)]
        void OnPrepareForSubstrateLoad(string moduleId, int eef);

        [DispId(12)]
        void OnExecuteModuleRecipe(IProcessJob processJob, string moduleId, string moduleRecipeId, ISubstrate substrate);

        [DispId(13)]
        void OnPrepareForSubstrateUnload(string moduleId, int eef);

        [DispId(14)]
        void OnSubstrateLoaded(string moduleId, int eef); // means eef/arm hast moved out too

        [DispId(15)]
        void OnSubstrateUnloaded(string moduleId, int eef); // means eef/arm hast moved out too

        [DispId(16)]
        void OnSendDataSetAck_S13F14(ITableDataResponse response);

        [DispId(17)]
        void OnDataSetRequest_S13F15(ITableDataRequest request);

        [DispId(18)]
        void OnS7F3Changed(string flowRecipeName, IStream stream, long streamLength, ref bool success, ref string errorMessage);

        [DispId(19)]
        void OnS7F5RequestPPIDChanged(string flowRecipeName, IStream stream, ref bool success, ref string errorMessage);

        [DispId(20)]
        void OnGetEquipmentState(ref EquipmentState equipmentState);

        [DispId(21)]
        void OnChangeOperationMode(OperationMode operationMode, ref bool success, ref string errorMessage);

        [DispId(22)]
        void OnModuleSubstratePresent(string moduleId, ISubstrate substrate);

        [DispId(23)]
        void OnModuleSubstrateRemoved(string moduleId, ISubstrate substrate);

        [DispId(24)]
        void OnSetTransportModuleState(ModuleState moduleState);

        [DispId(25)]
        void OnUTOEquipmentStateChanged(EquipmentState equipmentState);

        [DispId(26)]
        void OnS7F17DeletePPID(IComStringList flowRecipeNames, ref bool success, ref string errorMessage);
    }
}
