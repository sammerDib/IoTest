namespace UnitySC.Equipment.Abstractions.Devices.SmifLoadPort
{
    public partial class SmifLoadPort
    {
        private void InstanceInitialization()
        {
        // Default configure the instance.
        // Call made from the constructor.
        }

        protected abstract void InternalGoToSlot(byte slot);
    }
}