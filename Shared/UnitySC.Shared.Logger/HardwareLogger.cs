using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

using Serilog;
using Serilog.Core;
using Serilog.Core.Enrichers;
using Serilog.Events;

namespace UnitySC.Shared.Logger
{
    public class HardwareLogger : SerilogLogger<object>, IHardwareLogger
    {
        public bool IsLogEnabled { get; set; } = true;
        private readonly LoggingLevelSwitch _loggingLevelSwitch;
        private const string DeviceLogPathFormat = @"{0}\{1}";

        public HardwareLogger(string logLevel, string hardwareFamily, string deviceName) : base()
        {
            // Define log level
            if(!Enum.TryParse(logLevel, out LogEventLevel logEventLevel))
                IsLogEnabled = false;

            _loggingLevelSwitch = new LoggingLevelSwitch
            {
                MinimumLevel = logEventLevel
            };

            // Create hardware log path
            string generalPathFormat = SerilogInit.GetSerilogConfigValue("serilog:write-to:File.pathHardware");
            string generalDir = Path.GetDirectoryName(generalPathFormat);
            string generalFile = Path.GetFileName(generalPathFormat);
            string hardwarePathFormat = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format(DeviceLogPathFormat, generalDir, generalFile));


            // Rolling file option
            string outputTemplate = SerilogInit.GetSerilogConfigValue("serilog:write-to:File.outputHardwareTemplate") ?? "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} -HW- [{Level:u3}] {{{DeviceName}}} : {Message}{NewLine}{Exception}";
            string outputConsoleTemplate = SerilogInit.GetSerilogConfigValue("serilog:write-to:Console.outputHardwareTemplate") ?? "[{Timestamp:HH:mm:ss.fff -HW- {Level:u3}] {{DeviceName}} : {Message}{NewLine}{Exception}";

            long fileSizeLimitBytes = Convert.ToInt64(SerilogInit.GetSerilogConfigValue("serilog:write-to:File.fileSizeLimitBytes"));
            int retainedFileCountLimit = Convert.ToInt32(SerilogInit.GetSerilogConfigValue("serilog:write-to:File.retainedFileCountLimit"));
            bool rollOnFileSizeLimit = Convert.ToBoolean(SerilogInit.GetSerilogConfigValue("serilog:write-to:File.rollOnFileSizeLimit"));
            RollingInterval rollingInterval = (RollingInterval)Enum.Parse(typeof(RollingInterval), SerilogInit.GetSerilogConfigValue("serilog:write-to:File.rollingInterval"));
            Logger = new LoggerConfiguration()
                               .MinimumLevel.ControlledBy(_loggingLevelSwitch)
                               .Filter.ByExcluding(_ => !IsLogEnabled)
                               .WriteTo.File(path: hardwarePathFormat, outputTemplate: outputTemplate, fileSizeLimitBytes: fileSizeLimitBytes, retainedFileCountLimit: retainedFileCountLimit, rollOnFileSizeLimit: rollOnFileSizeLimit, rollingInterval: rollingInterval, shared: true)
                               .WriteTo.Console(outputTemplate:outputConsoleTemplate)
                               .Enrich.WithProperty("FamilyName", hardwareFamily)
                               .Enrich.WithProperty("DeviceName", deviceName)
                               .CreateLogger();

            LogDirectory = Path.GetDirectoryName(hardwarePathFormat);

        }
    }

}
