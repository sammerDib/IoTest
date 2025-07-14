using System;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Sensor3D512
{
    public interface IHardwareSensor3D512
    {
        int Initialization(bool pbAfterPowerOFF);

        void StartAcquisition();

        void StopAcquisition();

        void SetupScanningParameters(Object pParameters);

        bool IsRunning { get; }
    }
}