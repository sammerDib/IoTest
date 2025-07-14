namespace UnitySC.PM.Shared.Hardware.Service.Interface.Controller
{
    public interface ICdaPneumaticValve
    {
        bool ValveIsOpened { get; }

        void OpenCdaPneumaticValve();

        void CloseCdaPneumaticValve();
    }
}
