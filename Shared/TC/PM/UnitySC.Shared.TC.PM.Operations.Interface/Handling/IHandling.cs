using System.Collections.Generic;

using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.TC.PM.Operations.Interface
{
    public interface IHandling
    {
        MaterialPresence CheckWaferPresence(Length size);
        void Init();
        bool IsChuckInLoadingPosition { get; }
        bool IsChuckInProcessPosition { get; }
        NotificationTemplate<IPMHandlingStatesChangedCB> HandlingManagerCB { get; }
        void MoveChuck(ChuckPosition positionRequested, MaterialTypeInfo materialTypeInfo);
        void PMInitialization();
        void RefreshChuckPositionState();
        void ResetHardware();
        void ResetSmokeDetectorError();
        void Shutdown();
        SlitDoorValidationState GetSlitDoorValidationState();

        SlitDoorPosition GetSlitDoorState();
        void MoveSlitDoor(SlitDoorPosition state);
        void CheckWaferPresenceAndClampOrRelease();
        bool IsSensorWaferPresenceOnChuckAvailable(Length size);
        List<Length> GetMaterialDiametersSupported();
        void UpdateBackupFileAndApplyWaferPresenceChanged(Length size, MaterialPresence presence);
    }
}
