using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitySC.Shared.Logger
{
    public interface IHardwareLoggerFactory
    {
        IHardwareLogger CreateHardwareLogger(string logLevel, string hardwareFamily, string deviceName);
    }
}
