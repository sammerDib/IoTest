using Agileo.EquipmentModeling;

namespace UnitySC.EfemController.ProcessModules.Devices.ProcessModule.EfemControllerProcessModule
{
    public partial class EfemControllerProcessModule
    {
        #region Commands

        protected override void InternalSimulateInitialize(bool mustForceInit, Tempomat tempomat)
        {
            base.InternalSimulateInitialize(mustForceInit, tempomat);
            InternalInitialize(mustForceInit);
        }

        #endregion

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
    }
}
