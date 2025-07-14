using System;

using Serilog;
using Serilog.Configuration;

namespace AdcTools.Serilog
{
    public static class StringSinkConfiguration
    {
        public static LoggerConfiguration StringSink(
                  this LoggerSinkConfiguration loggerConfiguration,
                  IFormatProvider fmtProvider = null)
        {
            return loggerConfiguration.Sink(new StringSink(fmtProvider));
        }
    }
}
