using System;

using UnitySC.PM.Shared.Hardware.Service.Interface.Sensor3D512;

namespace UnitySC.PM.Shared.Hardware.Sensor3D512
{
    public class Sensor3D512 : IHardwareSensor3D512
    {
        public bool IsRunning => throw new NotImplementedException();

        public int Initialization(bool pbAfterPowerOFF)
        {
            throw new NotImplementedException();
        }

        public void SetupScanningParameters(Object pParameters)
        {
            throw new NotImplementedException();
        }

        public void StartAcquisition()
        {
            throw new NotImplementedException();
        }

        public void StopAcquisition()
        {
            throw new NotImplementedException();
        }
    }
}