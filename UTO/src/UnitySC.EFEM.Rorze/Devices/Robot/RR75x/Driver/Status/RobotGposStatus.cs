namespace UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Status
{
    public class RobotGposStatus : Equipment.Abstractions.Drivers.Common.Status
    {
        #region Fields

        private uint _xAxis;
        private uint _zAxis;
        private uint _rotationAxis;
        private uint _upperArm;
        private uint _lowerArm;
        private uint _upperFinger;
        private uint _lowerFinger;

        #endregion Fields

        #region Properties

        public uint XAxis
        {
            get => _xAxis;
            internal set => _xAxis = value;
        }

        public uint ZAxis
        {
            get => _zAxis;
            internal set => _zAxis = value;
        }

        public uint RotationAxis
        {
            get => _rotationAxis;
            internal set => _rotationAxis = value;
        }

        public uint UpperArm
        {
            get => _upperArm;
            internal set => _upperArm = value;
        }

        public uint LowerArm
        {
            get => _lowerArm;
            internal set => _lowerArm = value;
        }

        public uint UpperFinger
        {
            get => _upperFinger;
            internal set => _upperFinger = value;
        }

        public uint LowerFinger
        {
            get => _lowerFinger;
            internal set => _lowerFinger = value;
        }

        #endregion Properties

        #region Constructors

        public RobotGposStatus(RobotGposStatus other)
        {
            Set(other);
        }

        public RobotGposStatus(string messageStatusData)
        {
            var statuses = messageStatusData.Replace(":", string.Empty).Split('/');

            if (statuses.Length < 5)
            {
                SetDefaultsValue();
                return;
            }

            try
            {
                uint.TryParse(statuses[0], out _xAxis);
                uint.TryParse(statuses[1], out _zAxis);
                uint.TryParse(statuses[2], out _rotationAxis);
                uint.TryParse(statuses[3], out _upperArm);
                uint.TryParse(statuses[4], out _lowerArm);

                if (statuses.Length == 7)
                {
                    uint.TryParse(statuses[5], out _upperFinger);
                    uint.TryParse(statuses[6], out _lowerFinger);
                }
            }
            catch
            {
                SetDefaultsValue();
            }
        }

        #endregion Constructors

        #region Private Methods

        /// <summary>
        /// Copy statuses from on received data.
        /// <param name="other">If null, Reset values, otherwise, set</param>
        /// </summary>
        private void Set(RobotGposStatus other = null)
        {
            lock (this)
            {
                if (other == null)
                {
                    SetDefaultsValue();
                }
                else
                {
                    _xAxis        = other.XAxis;
                    _zAxis        = other.ZAxis;
                    _rotationAxis = other.RotationAxis;
                    _upperArm     = other.UpperArm;
                    _lowerArm     = other.LowerArm;
                    _upperFinger  = other.UpperFinger;
                    _lowerFinger  = other.LowerFinger;
                }
            }
        }

        private void SetDefaultsValue()
        {
            _xAxis        = 999;
            _zAxis        = 999;
            _rotationAxis = 999;
            _upperArm     = 999;
            _lowerArm     = 999;
            _upperFinger  = 999;
            _lowerFinger  = 999;
        }

        #endregion Private Methods

        #region Status Override

        public override object Clone()
        {
            return new RobotGposStatus(this);
        }

        #endregion Status Override
    }
}
