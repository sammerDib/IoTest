using UnitySC.PM.Shared.Hardware.Service.Interface;

namespace UnitySC.PM.Shared.Hardware.Controllers.FeatureInterfaces
{
    public interface IOpcMotion : IMotion
    {
        void TriggerUpdateEvent();

        void CustomCommand(string custom);
    }
}
