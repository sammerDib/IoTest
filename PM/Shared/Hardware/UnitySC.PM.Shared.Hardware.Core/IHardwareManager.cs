using UnitySC.PM.Shared.Status.Service.Interface;

namespace UnitySC.PM.Shared.Hardware.Core
{
    public interface IHardwareManager
    {
        void Init(IHardwareConfiguation hardwareConfiguation, IGlobalStatusServer globalStatusServer);

        void Reset();

        void Stop();
    }
}
