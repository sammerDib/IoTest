using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.TC.Shared.Data.PMProcessingState;

namespace UnitySC.Shared.TC.Shared
{
    public delegate void OnPMStateChanged(TC_PMState state);

    //public delegate void OnPMProcessingStateChanged(BasePMProcessingState state);
    public delegate void OnMaterialPresenceChanged(MaterialPresence state);

    public delegate void OnTransferStateChanged(TransferStateBase state);

    public delegate void OnDoorSlitStateChanged(SlitDoorPosition state);

    public delegate void OnPMModeChanged();
}
