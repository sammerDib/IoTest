using System.Threading;

using UnitySC.Shared.Configuration;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.TC.PM.Operations.Interface;
using UnitySC.Shared.TC.PM.Service.Interface;
using UnitySC.Shared.TC.Shared.Operations.Implementation;
using UnitySC.Shared.TC.Shared.Operations.Interface;
using UnitySC.Shared.Tools;

namespace UnitySC.Shared.TC.PM.Operations.Implementation
{
    public class UTOPMOperations : UTOBaseOperations, IUTOPMOperations
    {
        private IMaterialOperations _materialOperations;
        private IPMStatusVariableOperations _svOperations;
        private IUTOPMServiceCB _utoService;
        public IMaterialOperations MaterialOperations { get => _materialOperations; }
        public IUTOPMServiceCB UTOService { get => _utoService; }
        public IPMStatusVariableOperations SVOperations { get => _svOperations; }

        public UTOPMOperations()
        {
            _materialOperations = ClassLocator.Default.GetInstance<IMaterialOperations>();
            _svOperations = ClassLocator.Default.GetInstance<IPMStatusVariableOperations>();
            _svOperations.Init(ClassLocator.Default.GetInstance<IAutomationConfiguration>().SVConfigurationFilePath);
            _utoService = ClassLocator.Default.GetInstance<IUTOPMServiceCB>();
        }

        #region IPMStateManagerCB implementation

        public void OnPMStateChanged(TC_PMState state)
        {
            SVOperations.Update_AllTCPMState(state);
        }

        public void OnTransferStateChanged(EnumPMTransferState state)
        {
            SVOperations.Update_TransferState(state.ToString());
        }

        public void OnMaterialChanged_pmsm(Material material)
        {
        }

        public void OnCommunicationEnableRequested()
        {
            CommunicationOperations.SwitchState = EnableState.Enabled;
        }

        public void FireEvent(int eventName)
        {
        }


        public void CommunicationEstablished()
        {
        }

        public void CommunicationInterrupted()
        {
            MaterialOperations.ResetTransferInProgress();
        }

        public void CommunicationCheck()
        {
        }

        public void OnTransferValidationStateChanged(bool validated)
        {
            SVOperations.Update_TransferValidationState(validated);
            if (validated)
                NotifyMaterialNeedTransfer(); // Signal to UTO that a material is ready to be transfered
            
        }

        // Signal to UTO that a material is ready to be transfered
        public void NotifyMaterialNeedTransfer()
        {
            Thread.Sleep(200); // TODO: this sleep let the time for UTO to catch values of previous state changed before next events - need to improve with UTO
            _utoService.PMReadyToTransfer(); // PM is ReadyToTransfer => TC can execute PrepareForTransfer command
        }

        #endregion IPMStateManagerCB implementation
    }
}
