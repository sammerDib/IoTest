using System;
using System.Collections.Generic;
using System.Linq;

using Serilog.Core;
using Serilog.Events;


namespace AdcTools.Serilog
{
    public class StringSink : ILogEventSink
    {
        private IFormatProvider _formatProvider;

        private static List<string> strings = new List<string>();
        private static object mutex = new object();
        public static bool IsInitialized { get; private set; }

        public StringSink(IFormatProvider formatProvider)
        {
            IsInitialized = true;
            _formatProvider = formatProvider;
        }

        public void Emit(LogEvent logEvent)
        {
            if (logEvent.Level == LogEventLevel.Debug)
                return;
            string str = logEvent.RenderMessage(_formatProvider);
            lock (mutex)
            {
                strings.Add(str);
            }
        }

        public static List<string> EatString()
        {
            List<string> res;
            lock (mutex)
            {
                res = strings.ToList();
                strings.Clear();
            }
            return res;
        }

    }
}
