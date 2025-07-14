using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitySC.Shared.Logger
{
    public class HardwareLoggerFactory : IHardwareLoggerFactory
    {
        private readonly Func<string, string, string, IHardwareLogger> _loggerFactory;

        public HardwareLoggerFactory(Func<string, string, string, IHardwareLogger> loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public IHardwareLogger CreateHardwareLogger(string logLevel, string hardwareFamily, string deviceName)
        {
            return _loggerFactory(logLevel, hardwareFamily, deviceName);
        }
    }
}
