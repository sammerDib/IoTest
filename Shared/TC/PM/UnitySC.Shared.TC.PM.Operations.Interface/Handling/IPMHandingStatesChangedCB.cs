using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.TC.PM.Operations.Interface
{
    public interface IPMHandlingStatesChangedCB
    {
        void OnMaterialPresenceStateChanged(MaterialPresence materialState);

        void OnSlitDoorStateChanged(SlitDoorPosition slitDoorState);

        void OnLoadingUnloadingPositionChanged(bool isInLoadingUnloadingPosition);

        void OnInitializationChanged(bool isInitialized);
        void OnHandlingError(ErrorID errorId, string msgError);

    }
}
