using System;

using Agileo.Common.Tracing;

namespace UnitySC.GUI.Common.Vendor.UIComponents.DesignTime
{
    public class DesignTimeTracer : ITracer
    {
        public void Trace(string sourceAlias, TraceLevelType level, string formattedText)
        {
            throw new NotImplementedException();
        }

        public void Trace(string sourceAlias, TraceLevelType level, Exception exception)
        {
            throw new NotImplementedException();
        }

        public void Trace(string sourceAlias, TraceLevelType level, string formattedText, TraceParam traceParam)
        {
            throw new NotImplementedException();
        }

        public void Trace(string sourceAlias, TraceLevelType level, string formattedText, params object[] substitutionParam)
        {
            throw new NotImplementedException();
        }

        public void Trace(
            string sourceAlias,
            TraceLevelType level,
            Exception exception,
            string formattedText,
            params object[] substitutionParam)
        {
            throw new NotImplementedException();
        }

        public void Trace(string sourceAlias, TraceLevelType level, string formattedText, TraceParam traceParam, params object[] substitutionParam)
        {
            throw new NotImplementedException();
        }

        public void Trace(
            string sourceAlias,
            TraceLevelType level,
            Exception exception,
            string formattedText,
            TraceParam traceParam,
            params object[] substitutionParam)
        {
            throw new NotImplementedException();
        }

        public void Trace(TraceLevelType level, string formattedText)
        {
            throw new NotImplementedException();
        }

        public void Trace(TraceLevelType level, Exception exception)
        {
            throw new NotImplementedException();
        }

        public void Trace(TraceLevelType level, Exception exception, string formattedText, params object[] substitutionParam)
        {
            throw new NotImplementedException();
        }

        public void Trace(TraceLevelType level, string formattedText, TraceParam traceParam)
        {
            throw new NotImplementedException();
        }

        public void Trace(TraceLevelType level, string formattedText, params object[] substitutionParam)
        {
            throw new NotImplementedException();
        }

        public void Trace(TraceLevelType level, string formattedText, TraceParam traceParam, params object[] substitutionParam)
        {
            throw new NotImplementedException();
        }

        public void Trace(
            TraceLevelType level,
            Exception exception,
            string formattedText,
            TraceParam traceParam,
            params object[] substitutionParam)
        {
            throw new NotImplementedException();
        }

        public string TracerName { get; } = "DesignTime Tracer";
    }
}
