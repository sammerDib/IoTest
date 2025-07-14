using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Serilog;

namespace UnitySC.Shared.Logger
{
    public class LocalLogger : SerilogLogger<object>
    {     
        public LocalLogger(string loggerName) : base()
        {           
            String appLoggerPath = SerilogInit.GetSerilogConfigValue("serilog:write-to:File.path");
            string localLoggerFullPath = $"{appLoggerPath}\\..\\{loggerName}-.log";
            // Rolling file option
            string outputTemplate = SerilogInit.GetSerilogConfigValue("serilog:write-to:File.outputTemplate");
            long fileSizeLimitBytes = Convert.ToInt64(SerilogInit.GetSerilogConfigValue("serilog:write-to:File.fileSizeLimitBytes"));
            int retainedFileCountLimit = Convert.ToInt32(SerilogInit.GetSerilogConfigValue("serilog:write-to:File.retainedFileCountLimit"));
            bool rollOnFileSizeLimit = Convert.ToBoolean(SerilogInit.GetSerilogConfigValue("serilog:write-to:File.rollOnFileSizeLimit"));
            RollingInterval rollingInterval = (RollingInterval)Enum.Parse(typeof(RollingInterval), SerilogInit.GetSerilogConfigValue("serilog:write-to:File.rollingInterval"));
            Logger = new LoggerConfiguration()
                               .MinimumLevel.Debug()
                               .WriteTo.File(path: localLoggerFullPath, outputTemplate: outputTemplate, fileSizeLimitBytes: fileSizeLimitBytes, retainedFileCountLimit: retainedFileCountLimit, rollOnFileSizeLimit: rollOnFileSizeLimit, rollingInterval: rollingInterval)                              
                               .CreateLogger();
            LogDirectory = Path.GetDirectoryName(localLoggerFullPath);
        }
    }
}
