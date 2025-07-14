using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml.XPath;

using Serilog;
using Serilog.Core.Enrichers;
using Serilog.Events;

namespace UnitySC.Shared.Logger
{
    public class SerilogLogger<T> : ILogger<T>
    {
        internal Serilog.ILogger Logger;
        internal string CallerName;

        public virtual string LogDirectory { get; protected set; }

        public SerilogLogger()
        {
            if (typeof(T) == typeof(object))
                CallerName = string.Empty;
            else
            {
                // Take last 3 namespace parts
                var namespaceParts = typeof(T).FullName.Split('.').Reverse().Take(3).Reverse();
                CallerName = "[" + string.Join(".", namespaceParts) + "] ";
            }

            Logger = Log.Logger;
            LogDirectory = SerilogInit.LogDirectory;
        }

        private void Write(LogEventLevel level, Exception exception, string messageTemplate, params object[] propertyValues)
        {
            if (string.IsNullOrWhiteSpace(messageTemplate))
            {
                throw new ArgumentException("the template message for Serilog cannot be empty or null", nameof(messageTemplate));
            }
            Logger
                .ForContext("Caller", CallerName)
                .Write(level, exception, messageTemplate, propertyValues);
        }
        public void Verbose(string messageTemplate, params object[] propertyValues)
        {
            Write(LogEventLevel.Verbose, null, messageTemplate, propertyValues);
        }

        public void Debug(string messageTemplate, params object[] propertyValues)
        {
            Write(LogEventLevel.Debug, null, messageTemplate, propertyValues);
        }

        public void Information(string messageTemplate, params object[] propertyValues)
        {
            Write(LogEventLevel.Information, null, messageTemplate, propertyValues);
        }

        public void Warning(string messageTemplate, params object[] propertyValues)
        {
            Write(LogEventLevel.Warning, null, messageTemplate, propertyValues);
        }

        public void Error(string messageTemplate, params object[] propertyValues)
        {
            Write(LogEventLevel.Error, null, messageTemplate, propertyValues);
        }

        public void Error(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            Write(LogEventLevel.Error, exception, messageTemplate, propertyValues);
            if (exception?.InnerException != null)
                Write(LogEventLevel.Error, exception, messageTemplate, propertyValues);
        }

        public void Fatal(Exception exception, string messageTemplate, params object[] propertyValues)
        {
            Write(LogEventLevel.Fatal, exception, messageTemplate, propertyValues);
        }

        public bool IsDebugEnabled()
        {
            return Logger.IsEnabled(LogEventLevel.Debug);
        }

        public bool IsErrorEnabled()
        {
            return Logger.IsEnabled(LogEventLevel.Error);
        }

        public bool IsFatalEnabled()
        {
            return Logger.IsEnabled(LogEventLevel.Fatal);
        }

        public bool IsInformationEnabled()
        {
            return Logger.IsEnabled(LogEventLevel.Information);
        }

        public bool IsVerboseEnabled()
        {
            return Logger.IsEnabled(LogEventLevel.Verbose);
        }

        public bool IsWarningEnabled()
        {
            return Logger.IsEnabled(LogEventLevel.Warning);
        }
    }

    public class SerilogInit
    {
        public static string LogDirectory { get; set; }
        public static string ConfigurationFile { get; set; }

        private static Func<string, string> s_getSerilogConfig;

        /// <summary>
        /// Init serilog with current app.config settings
        /// </summary>
        public static void InitWithCurrentAppConfig()
        {
            Log.Logger = new LoggerConfiguration()
                           .ReadFrom.AppSettings()
                           .CreateLogger();

            s_getSerilogConfig = (key) =>
            {
                return ConfigurationManager.AppSettings[key];
            };

            string serilogFilePathConfig = GetSerilogConfigValue("serilog:write-to:File.path");
            LogDirectory = SerilogFilePathToLogDirectory(serilogFilePathConfig);
        }

        /// <summary>
        /// Init serilog with config file settings
        /// </summary>
        public static void Init(string configurationFilePath)
        {
            ConfigurationFile = configurationFilePath;
            if (!File.Exists(configurationFilePath))
                throw new FileNotFoundException($"Configuration file not found in serilog init: {configurationFilePath}");
            s_getSerilogConfig = (key) =>
            {
                XPathNavigator nav;
                XPathDocument docNav;
                docNav = new XPathDocument(ConfigurationFile);
                nav = docNav.CreateNavigator();
                return nav.SelectSingleNode($"/configuration/appSettings/add[@key='{key}']/@value")?.Value;
            };

            string serilogFilePathConfig = GetSerilogConfigValue("serilog:write-to:File.path");
            LogDirectory = SerilogFilePathToLogDirectory(serilogFilePathConfig);

            Log.Logger = new LoggerConfiguration()
                           .ReadFrom.AppSettings(filePath: ConfigurationFile)
                           .CreateLogger();
        }

        private static string SerilogFilePathToLogDirectory(string serilogFilePathConfig)
        {
            string logDirectory;
            string currentDir = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            string logPath = Path.GetDirectoryName(serilogFilePathConfig);
            if (logPath != null)
                logDirectory = Path.Combine(currentDir, Path.GetDirectoryName(serilogFilePathConfig));
            else
                logDirectory = Path.Combine(currentDir, "Logs");
            return logDirectory;
        }

        public static string GetSerilogConfigValue(string key)
        {
            return s_getSerilogConfig.Invoke(key);
        }
    }
}
