using System;
using System.Collections.Generic;
using System.IO;

namespace UnitySC.Shared.Logger
{
    /// <summary>
    /// Empty logger.
    /// </summary>
    public class NullLogger :
         ILogger
    {
        public string LogDirectory => Directory.GetCurrentDirectory();

        public void Debug(string messageTemplate, params object[] propertyValues)
        {
        }

        public void Error(string messageTemplate, params object[] propertyValues)
        {
        }

        public void Error(Exception exception, string messageTemplate, params object[] propertyValues)
        {
        }

        public void Fatal(Exception exception, string messageTemplate, params object[] propertyValues)
        {
        }

        public void Information(string messageTemplate, params object[] propertyValues)
        {
        }

        public bool IsDebugEnabled()
        {
            return true;
        }

        public bool IsErrorEnabled()
        {
            return true;
        }

        public bool IsFatalEnabled()
        {
            return true;
        }

        public bool IsInformationEnabled()
        {
            return true;
        }

        public bool IsVerboseEnabled()
        {
            return true;
        }

        public bool IsWarningEnabled()
        {
            return true;
        }

        public void Verbose(string messageTemplate, params object[] propertyValues)
        {
        }

        public void Warning(string messageTemplate, params object[] propertyValues)
        {
        }
    }
}
