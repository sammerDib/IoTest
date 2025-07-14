using Agileo.EquipmentModeling;

using UnitySC.EFEM.Rorze.Devices.LightTower.Simulator;

namespace UnitySC.EFEM.Rorze.Devices.LightTower
{
    public partial class LightTower : ISimDevice
    {
        #region Properties

        public ISimDeviceView SimDeviceView
            => new LightTowerSimulatorUserControl() { DataContext = SimulationData };

        protected internal LightTowerSimulationData SimulationData { get; private set; }

        #endregion

        #region IGenericDevice Commands

        protected override void InternalSimulateInitialize(bool mustForceInit, Tempomat tempomat)
        {
            InternalInitialize(mustForceInit);
        }

        #endregion IGenericDevice Commands

        #region ICommunicatingDevice Commands

        protected override void InternalSimulateStartCommunication(Tempomat tempomat)
        {
            InternalStartCommunication();
        }

        protected override void InternalSimulateStopCommunication(Tempomat tempomat)
        {
            InternalStopCommunication();
        }

        #endregion ICommunicatingDevice Commands

        #region ILightTower Commands

        protected override void InternalSimulateSetDateAndTime(Tempomat tempomat)
        {
            // Command not applicable to this hardware
        }

        #endregion ILightTower Commands

        #region Simulation methods

        private void SetUpSimulatedMode()
        {
            SimulationData = new LightTowerSimulationData(this);
        }

        #endregion
    }
}
