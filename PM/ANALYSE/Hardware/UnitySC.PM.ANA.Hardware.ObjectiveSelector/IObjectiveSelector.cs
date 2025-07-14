using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Tools.Service;

public delegate void ErrorDelegate(Message error);

namespace UnitySC.PM.ANA.Hardware.ObjectiveSelector
{
    public interface IObjectiveSelector
    {
        ObjectivesSelectorConfigBase Config { get; set; }
        string DeviceID { get; set; }

        void Init();

        void Disconnect();

        void SetObjective(ObjectiveConfig newObjectiveToUse);

        ObjectiveConfig GetObjectiveInUse();

        ModulePositions Position { get; set; }

        void WaitMotionEnd();
    }
}
