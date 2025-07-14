using System;
using System.Timers;

namespace UnitySC.Equipment.Abstractions.Vendor.Devices.Simulation
{
    public class SimulatedRampStatus : Status, IDisposable
    {
        #region Fields

        private readonly float _overshoot;
        protected int _refreshPeriod;
        protected readonly Timer RefreshTimer;
        private float _initialValue;
        protected float Increment;
        protected float InternalSetPoint;
        private readonly bool _canBeNegative;

        protected readonly object Locker = new object();

        #endregion Fields

        public event EventHandler ValueChanged;

        #region Properties

        private float _value;

        public float Value
        {
            get { return _value; }
            set
            {
                if (SetAndRaiseIfChanged(ref _value, value))
                {
                    OnValueChanged();
                }
            }
        }

        private float _setPoint;

        public float SetPoint
        {
            get { return _setPoint; }
            set
            {
                SetAndRaiseIfChanged(ref _setPoint, value);
            }
        }

        private float _speed;

        public float Speed
        {
            get { return _speed; }
            set
            {
                SetAndRaiseIfChanged(ref _speed, value);
            }
        }

        private bool _deactivateRamp;

        public bool DeactivateRamp
        {
            get { return _deactivateRamp; }
            set
            {
                SetAndRaiseIfChanged(ref _deactivateRamp, value);
            }
        }

        private bool _isStarted = true;

        public bool IsStarted
        {
            get { return _isStarted; }
            private set
            {
                SetAndRaiseIfChanged(ref _isStarted, value);
            }
        }

        public float InitialValue
        {
            get { return _initialValue; }
            set
            {
                SetAndRaiseIfChanged(ref _initialValue, value);
            }
        }

        public int RefreshPeriod
        {
            get { return _refreshPeriod; }
            protected set
            {
                SetAndRaiseIfChanged(ref _refreshPeriod, value);
            }
        }

        #endregion Properties

        #region Constructor

        public SimulatedRampStatus(string name, int refreshPeriod, ushort initialValue, float overshoot, bool canBeNegative = false) : base(name)
        {
            InitialValue = initialValue;
            Value = _initialValue;
            InternalSetPoint = _initialValue;
            _overshoot = overshoot;
            _canBeNegative = canBeNegative;

            RefreshPeriod = refreshPeriod;

            RefreshTimer = new Timer();
            RefreshTimer.Elapsed += RefreshTimer_Elapsed;
            RefreshTimer.Interval = _refreshPeriod;
        }

        protected SimulatedRampStatus(string name, ushort initialValue, float overshoot, bool canBeNegative = false) : base(name)
        {
            InitialValue = initialValue;
            Value = _initialValue;
            InternalSetPoint = _initialValue;
            _overshoot = overshoot;
            _canBeNegative = canBeNegative;
            RefreshTimer = new Timer();
        }

        #endregion Constructor

        #region Public Methods

        public void Start()
        {
            RefreshTimer.Start();
            lock (Locker)
            {
                IsStarted = true;
                InternalSetPoint = SetPoint;
                if (!DeactivateRamp)
                {
                    Increment = Speed / 60 * _refreshPeriod / 1000;
                }
                else
                {
                    Value = InternalSetPoint;
                }
            }
        }

        public void Stop()
        {
            lock (Locker)
            {
                IsStarted = false;
                InternalSetPoint = _initialValue;
                if (!DeactivateRamp)
                {
                    Increment = Speed / 60 * _refreshPeriod / 1000;
                }
                else
                {
                    Value = InternalSetPoint;
                }
            }
        }

        #endregion Public Methods

        #region Timer Elapsed

        protected void RefreshTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            RefreshTimer.Stop();

            lock (Locker)
            {
                float tempValue = Value;

                if (tempValue < InternalSetPoint)
                {
                    if (Math.Abs(tempValue - InternalSetPoint) > Increment)
                    {
                        tempValue += Increment;
                    }
                    else
                    {
                        tempValue += _overshoot;
                    }
                }
                else
                {
                    if (Math.Abs(tempValue - InternalSetPoint) > Increment)
                    {
                        tempValue -= Increment;
                    }
                    else
                    {
                        tempValue -= _overshoot;
                    }
                }

                if (!_canBeNegative && tempValue < 0)
                {
                    tempValue = 0;
                }

                Value = tempValue;
            }

            if (!_disposing)
            {
                RefreshTimer.Start();
            }
        }

        #endregion Timer Elapsed

        private void OnValueChanged()
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private bool _disposing;
        public void Dispose()
        {
            _disposing = true;
            if (RefreshTimer == null) return;
            if (IsStarted) Stop();

            RefreshTimer.Stop();
            RefreshTimer.Dispose();
        }
    }
}
