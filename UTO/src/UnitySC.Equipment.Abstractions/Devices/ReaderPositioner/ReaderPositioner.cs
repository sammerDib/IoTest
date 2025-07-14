using Agileo.SemiDefinitions;

namespace UnitySC.Equipment.Abstractions.Devices.ReaderPositioner
{
    public partial class ReaderPositioner
    {
        #region Setup

        private void InstanceInitialization()
        {
            // Default configure the instance.
            // Call made from the constructor.
        }

        #endregion

        #region Commands

        protected abstract void InternalSetPosition(SampleDimension dimension);

        #endregion
    }
}
