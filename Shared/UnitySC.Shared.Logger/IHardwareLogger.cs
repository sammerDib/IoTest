using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace UnitySC.Shared.Logger
{
    public interface IHardwareLogger : ILogger
    {
        bool IsLogEnabled { get; set; }
    }
}
