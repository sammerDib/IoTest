namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Light
{
    public interface IPowerLightController
    {
        void InitLightSources();
        void RefreshLightSource(string lightID);
        void SetPower(string lightID, double powerInPercent);
        double GetPower(string lightID);
        void RefreshPower(string lightID);
        void SwitchOn(string lightID, bool powerOn);
        void RefreshSwitchOn(string lightID);
    }
}
