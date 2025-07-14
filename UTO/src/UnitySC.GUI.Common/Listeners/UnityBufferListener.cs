using Agileo.Common.Tracing;
using Agileo.Common.Tracing.Listeners;

namespace UnitySC.GUI.Common.Listeners
{
    public class UnityBufferListener : IListener
    {
        #region Properties

        public BufferListener BufferListener { get; }

        #endregion

        #region Constructors

        public UnityBufferListener(BufferListener bufferListener)
        {
            BufferListener = bufferListener;
        }

        #endregion

        #region IListener

        public void Close()
        {
            BufferListener.Close();
        }

        public void DoLog(TraceLine traceLine)
        {
            if (traceLine.Text.Contains("GVER")
                || traceLine.Text.Contains("VersionAcquisitionCommand")
                || traceLine.Text.Contains("Macro command  was added to the queue"))
            {
                return;
            }

            BufferListener.DoLog(traceLine);
        }

        public string Name
        {
            get => BufferListener.Name;
            set => BufferListener.Name = value;
        }

        #endregion

        #region Operator

        public static explicit operator BufferListener(UnityBufferListener bufferListener)
        {
            return bufferListener.BufferListener;
        }

        #endregion
    }
}
