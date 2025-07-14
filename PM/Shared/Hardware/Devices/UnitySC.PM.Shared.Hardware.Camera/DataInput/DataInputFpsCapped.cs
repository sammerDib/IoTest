using System;
using System.Threading.Tasks;

namespace UnitySC.PM.Shared.Hardware.Camera.DataInput
{
    /// <summary>
    /// Fps limiter.
    /// Usefull for displaying images at a lower rate as the acquisition.
    /// </summary>
    public class DataInputFpsCapped<T> :
        DataInputStream<T>
    {
        /// <summary>
        /// Target will never be called faster than Fps.
        /// </summary>
        public DataInputStream<T> Target;

        public DataInputFpsCapped()
        {
            Fps = 39.93d;
        }

        /// <summary>
        /// Will be called from a background thread to give the current data counter, but not ofters than Fps.
        /// </summary>
        public Action<UInt64> BackgroundCounter;

        /// <summary>
        /// Max fps.
        /// </summary>
        public double Fps
        {
            get
            {
                return 1d / new TimeSpan(_minimalDelay_ticks).TotalSeconds;
            }

            set
            {
                _minimalDelay_ticks = TimeSpan.FromSeconds(1 / value).Ticks;
            }
        }

        private Int64 _minimalDelay_ticks;

        public Task DisposeAsync()
        {
            _counter = 0;

            //>DataException
            return Target.DisposeAsync();
        }

        /// <summary>
        /// Instant of the last write sent to the target.
        /// </summary>
        private Int64 _lastWrite_ticks = 0L;

        /// <summary>
        /// Data counter (without fps limit).
        /// </summary>
        private UInt64 _counter;

        public void Write(System.Drawing.Point point, T data)
        {
            _counter++;

            Int64 ticks = DateTime.UtcNow.Ticks;
            if ((ticks - _lastWrite_ticks) >= _minimalDelay_ticks)
            {
                _lastWrite_ticks = ticks;
                BackgroundCounter?.Invoke(_counter);

                //>DataException
                Target.Write(point, data);
            }
        }
    }
}