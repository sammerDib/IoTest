using Agileo.Common.Tracing;
using Agileo.Common.Tracing.Listeners;

namespace UnitySC.GUI.Common.Listeners
{
    public class UnityTraceStorage : IListener
    {
        #region Properties

        public TraceStorage TraceStorage { get; }

        #endregion

        #region Constructor

        public UnityTraceStorage(TraceStorage traceStorage)
        {
            TraceStorage = traceStorage;
        }

        #endregion

        #region IListener

        public void Close()
        {
            TraceStorage.Close();
        }

        public void DoLog(TraceLine traceLine)
        {
            if (traceLine.Text.Contains("GVER")
                || traceLine.Text.Contains("VersionAcquisitionCommand")
                || traceLine.Text.Contains("Macro command  was added to the queue"))
            {
                return;
            }

            TraceStorage.DoLog(traceLine);
        }

        public string Name
        {
            get => TraceStorage.Name;
            set => TraceStorage.Name = value;
        }

        #endregion

        public static explicit operator TraceStorage(UnityTraceStorage unityTraceStorage)
        {
            return unityTraceStorage.TraceStorage;
        }
    }
}
