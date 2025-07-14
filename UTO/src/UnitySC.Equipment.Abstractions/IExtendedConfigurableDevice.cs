using Agileo.Common.Configuration;

using UnitySC.Equipment.Abstractions.Vendor.Devices;

namespace UnitySC.Equipment.Abstractions
{
    public interface IExtendedConfigurableDevice : IConfigurableDevice

    {
        public IConfigManager ConfigManager { get; }

        public interface IExtendedConfigurableDevice<out T> : IConfigurableDevice
            where T : IConfiguration
        {
            
        }
    }
}
