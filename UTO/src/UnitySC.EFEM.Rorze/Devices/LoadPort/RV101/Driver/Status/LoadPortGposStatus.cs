namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.Status
{
    public class LoadPortGposStatus : Equipment.Abstractions.Drivers.Common.Status
    {
        #region Fields
        private uint _zAxis;
        private uint _coverLock;

        #endregion

        #region Properties

        public uint ZAxis
        {
            get => _zAxis;
            internal set => _zAxis = value;
        }

        public uint CoverLock
        {
            get => _coverLock;
            internal set => _coverLock = value;
        }

        #endregion

        #region Constructors

        public LoadPortGposStatus(LoadPortGposStatus other)
        {
            Set(other);
        }

        public LoadPortGposStatus(string messageStatusData)
        {
            var statuses = messageStatusData.Replace(":", string.Empty).Split('/');

            if (statuses.Length < 2)
            {
                SetDefaultsValue();
                return;
            }

            try
            {
                switch (statuses.Length)
                {
                    case 2:
                        uint.TryParse(statuses[0], out _zAxis);
                        uint.TryParse(statuses[1], out _coverLock);
                        break;
                    default:
                        SetDefaultsValue();
                        break;
                }
            }
            catch
            {
                SetDefaultsValue();
            }
        }


        #endregion

        #region Private Methods

        /// <summary>
        /// Copy statuses from on received data.
        /// <param name="other">If null, Reset values, otherwise, set</param>
        /// </summary>
        private void Set(LoadPortGposStatus other = null)
        {
            lock (this)
            {
                if (other == null)
                {
                    SetDefaultsValue();
                }
                else
                {
                    _zAxis            = other.ZAxis;
                    _coverLock        = other._coverLock;
                }
            }
        }

        private void SetDefaultsValue()
        {
            _zAxis            = 0;
            _coverLock        = 0;
        }
        #endregion Private Methods

        #region Status Override

        public override object Clone()
        {
            return new LoadPortGposStatus(this);
        }

        #endregion Status Override
    }
}
