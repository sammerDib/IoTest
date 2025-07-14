using System;
using System.Configuration;

using Serilog;

using UnitySC.Shared.Tools;

namespace ADCConfiguration.Services
{
    public class LogService
    {
        public LogService()
        {
            PathString logfolder = ConfigurationManager.AppSettings["LogFolder"];
            PathString logfile = PathString.GetExeFullPath().ChangeExtension(".log").Filename;
            Log.Logger = new LoggerConfiguration()
                               .ReadFrom.AppSettings()
                               .WriteTo.File(path: logfolder / logfile,
                                            rollOnFileSizeLimit: true,
                                            fileSizeLimitBytes: 20971520,
                                            retainedFileCountLimit: 100)
                               .WriteTo.Console()
                               .CreateLogger();
        }

        public void LogInfo(string msg, bool logUser = true)
        {
            Log.Information(FormatMessage(msg, logUser));
        }

        public void LogError(string msg, bool logUser = true)
        {
            Log.Error(FormatMessage(msg, logUser));
        }

        public void LogError(string msg, Exception ex, bool logUser = true)
        {
            Log.Error(FormatMessage(msg + ": " + ex.ToString(), logUser));
        }

        public void LogWarning(string msg, bool logUser = true)
        {
            Log.Warning(FormatMessage(msg, logUser));
        }

        public void LogDebug(string msg, bool logUser = false)
        {
            Log.Debug(FormatMessage(msg, logUser));
        }

        private string FormatMessage(string msg, bool logUser)
        {
            var user = Services.Instance.AuthentificationService.CurrentUser;
            if (user != null && logUser)
                return string.Format("({0}) {1}", user.Login, msg);
            else
                return msg;
        }

    }
}
