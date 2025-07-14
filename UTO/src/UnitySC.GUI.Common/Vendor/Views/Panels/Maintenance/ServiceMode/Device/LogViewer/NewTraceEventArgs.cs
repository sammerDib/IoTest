using System;

using Agileo.Common.Tracing;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.ServiceMode.Device.LogViewer
{
    public class NewTraceEventArgs : EventArgs
    {
        public string Trace { get; }

        public bool RemoveFirstTrace { get; }

        public TraceLine TraceLine { get; }

        public NewTraceEventArgs(string trace, bool removeFirstTrace, TraceLine traceLine)
        {
            Trace = trace;
            RemoveFirstTrace = removeFirstTrace;
            TraceLine = traceLine;
        }
    }
}
