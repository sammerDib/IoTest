using Agileo.EquipmentModeling;

namespace UnitySC.EFEM.Rorze.Devices.ReaderPositioner
{
    public partial class ReaderPositioner
    {
        #region Commands

        protected override void InternalSimulateInitialize(bool mustForceInit, Tempomat tempomat)
        {
            base.InternalSimulateInitialize(mustForceInit, tempomat);
            InternalInitialize(mustForceInit);
        }

        #endregion
    }
}
