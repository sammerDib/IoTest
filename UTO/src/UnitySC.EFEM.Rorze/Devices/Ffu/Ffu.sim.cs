using Agileo.EquipmentModeling;

using UnitySC.EFEM.Rorze.Devices.Ffu.Simulator;

namespace UnitySC.EFEM.Rorze.Devices.Ffu
{
    public partial class Ffu : ISimDevice
    {
        #region Properties

        public ISimDeviceView SimDeviceView
            => new FfuSimulatorUserControl { DataContext = SimulationData };

        protected internal FfuSimulationData SimulationData { get; private set; }

        #endregion

        #region IGenericDevice Commands

        protected override void InternalSimulateInitialize(bool mustForceInit, Tempomat tempomat)
        {
            base.InternalSimulateInitialize(mustForceInit, tempomat);
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

        #endregion

        #region Simulation methods

        private void SetUpSimulatedMode()
        {
            SimulationData = new FfuSimulationData(this);
        }

        #endregion
    }
}
