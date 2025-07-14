using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Timers;

using UnitsNet;

namespace UnitySC.Equipment.Abstractions.Vendor.ProcessExecution
{
    public class RecipeElapsedTimeInfo : INotifyPropertyChanged, IDisposable
    {
        #region Constructor

        public RecipeElapsedTimeInfo()
        {
            _timerElapsedTime = new Timer(TimerElapseIntervalMs);
            _timerElapsedTime.Elapsed += OnTimedEvent;
            _timerElapsedTime.AutoReset = true;
            _timerElapsedTime.Enabled = true;
            ClearTimerInfo();
        }

        #endregion

        #region Public Methods

        public void ClearTimerInfo()
        {
            IsTimerInstructionRequired = false;
            IsTimerJobRequired = false;
            IsTimerStepRequired = false;

            _isRecipeStarted = false;
            DateTimeRecipeStarted = DateTime.MinValue;

            RefreshTimers();

            //reset the job total duration
            TimeElapsedInJob = Duration.Zero;
        }

        #endregion

        #region Fields

        private readonly Timer _timerElapsedTime;
        private const double TimerElapseIntervalMs = 1000;

        private readonly object _locker = new();

        private bool _isRecipeStarted;

        #endregion

        #region Properties

        public bool IsTimerStepRequired { get; set; }

        public bool IsTimerInstructionRequired { get; set; }

        public bool IsTimerJobRequired { get; set; }

        private DateTime _dateTimeRecipeStarted;

        public DateTime DateTimeRecipeStarted
        {
            get => _dateTimeRecipeStarted;
            set
            {
                _dateTimeRecipeStarted = value;
                OnPropertyChanged(nameof(DateTimeRecipeStarted));
            }
        }

        private Duration _timeElapsedInStep;

        public Duration TimeElapsedInStep
        {
            get => _timeElapsedInStep;
            set
            {
                lock (_locker)
                {
                    _timeElapsedInStep = value;
                }

                OnPropertyChanged(nameof(TimeElapsedInStep));
            }
        }

        private Duration _timeElapsedInInstruction;

        public Duration TimeElapsedInInstruction
        {
            get => _timeElapsedInInstruction;
            set
            {
                lock (_locker)
                {
                    _timeElapsedInInstruction = value;
                }

                OnPropertyChanged(nameof(TimeElapsedInInstruction));
            }
        }

        private Duration _timeElapsedInJob;

        public Duration TimeElapsedInJob
        {
            get => _timeElapsedInJob;
            set
            {
                lock (_locker)
                {
                    _timeElapsedInJob = value;
                }

                OnPropertyChanged(nameof(TimeElapsedInJob));
            }
        }

        #endregion

        #region TimerElapsedTime

        private void RefreshTimers()
        {
            TimeElapsedInInstruction = IsTimerInstructionRequired
                ? TimeElapsedInInstruction + Duration.FromMilliseconds(TimerElapseIntervalMs)
                : Duration.Zero;

            TimeElapsedInStep = IsTimerStepRequired
                ? TimeElapsedInStep + Duration.FromMilliseconds(TimerElapseIntervalMs)
                : Duration.Zero;

            //maintain the job total duration after the end of the job
            if (IsTimerJobRequired)
            {
                TimeElapsedInJob += Duration.FromMilliseconds(TimerElapseIntervalMs);
            }

            if (!_isRecipeStarted)
            {
                DateTimeRecipeStarted = DateTime.Now;
                _isRecipeStarted = true;
            }
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e) => RefreshTimers();

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #endregion

        #region IDisposable

        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue)
            {
                return;
            }

            if (disposing && _timerElapsedTime != null)
            {
                _timerElapsedTime.Stop();
                _timerElapsedTime.Elapsed -= OnTimedEvent;
                _timerElapsedTime.Dispose();
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
