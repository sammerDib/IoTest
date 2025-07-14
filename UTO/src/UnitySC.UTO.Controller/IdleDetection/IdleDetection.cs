using System;
using System.Runtime.InteropServices;
using System.Timers;

using Agileo.GUI.Components;

namespace UnitySC.UTO.Controller.IdleDetection
{
    public class IdleDetection : Notifier, IDisposable
    {
        #region Fields

        private readonly object _lock = new();
        private readonly Timer _idleTimer;
        private double _idleMinutes;
        private uint _previousLastInput;
        private DateTime _idleStart;

        private const int TimerDuration = 60000;
        #endregion

        #region Properties

        public TimeSpan IdleTime
        {
            get
            {
                lock (_lock)
                {
                    return TimeSpan.FromMinutes(_idleMinutes);
                }
            }
        }

        public int TimeoutDuration { get; }

        private bool _inactivityDetected;

        public bool InactivityDetected
        {
            get => _inactivityDetected;
            private set
            {
                SetAndRaiseIfChanged(ref _inactivityDetected, value);
            }

        }

        #endregion

        #region Constructor

        public IdleDetection(int timeoutDuration)
        {
            TimeoutDuration = timeoutDuration;
            _idleTimer = new Timer(TimerDuration) {AutoReset = true};
            _idleTimer.Elapsed += IdleTimer_Elapsed;
            _idleTimer.Start();
            _idleStart = DateTime.Now;
        }
        
        #endregion

        #region Event Handler

        private void IdleTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (_lock)
            {
                var lastInput = GetLastInputInfoValue();

                if (lastInput == _previousLastInput)
                {
                    _idleMinutes = (DateTime.Now - _idleStart).TotalMinutes;
                }
                else
                {
                    _idleMinutes = 0;
                    _idleStart = DateTime.Now;
                }

                _previousLastInput = lastInput;
                InactivityDetected = _idleMinutes >= TimeoutDuration;
            }
        }

        #endregion

        #region user32

        private struct LastInputInfo
        {
            public uint cbSize;
            public uint dwTime;
        }

        [DllImport("user32.dll")]
        private static extern bool GetLastInputInfo(ref LastInputInfo lastInputInfo);
        private static uint GetLastInputInfoValue()
        {
            LastInputInfo lastInPut = new LastInputInfo();
            lastInPut.cbSize = (uint)Marshal.SizeOf(lastInPut);
            GetLastInputInfo(ref lastInPut);
            return lastInPut.dwTime;
        }

        #endregion

        #region Dispose

        private bool _disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue)
            {
                return;
            }

            if (disposing)
            {
                _idleTimer.Stop();
                _idleTimer.Elapsed -= IdleTimer_Elapsed;
                _idleTimer.Dispose();
            }

            _disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

    }
}
