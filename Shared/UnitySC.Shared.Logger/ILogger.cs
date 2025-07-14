using System;
using System.Collections.Generic;

namespace UnitySC.Shared.Logger
{
    public interface ILogger<T> : ILogger
    {
    }

    public interface ILogger
    {
        bool IsDebugEnabled();

        bool IsErrorEnabled();

        bool IsFatalEnabled();

        bool IsInformationEnabled();

        bool IsVerboseEnabled();

        bool IsWarningEnabled();

        void Debug(string messageTemplate, params object[] propertyValues);

        void Error(string messageTemplate, params object[] propertyValues);

        void Error(Exception exception, string messageTemplate, params object[] propertyValues);

        void Fatal(Exception exception, string messageTemplate, params object[] propertyValues);

        void Information(string messageTemplate, params object[] propertyValues);

        void Verbose(string messageTemplate, params object[] propertyValues);

        void Warning(string messageTemplate, params object[] propertyValues);

        string LogDirectory { get; }
    }
}
