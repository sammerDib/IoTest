namespace UnitySC.Equipment.Abstractions.Vendor.Communication
{
    public interface IMessageDataBusDevice
    {
        void ConfigureMessageDataBus();

        void DisposeMessageDataBus();
    }
}
