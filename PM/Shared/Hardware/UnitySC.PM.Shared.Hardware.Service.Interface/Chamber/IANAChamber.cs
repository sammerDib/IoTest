namespace UnitySC.PM.Shared.Hardware.Service.Interface.Chamber
{
    public interface IANAChamber : IChamberFFUControl
    {
        void InitProcess();

        void SetChamberLightState(bool value);

        bool EMOState();

        bool RobotIsOutState();

        bool PrepareToTransferState();

    }
}
