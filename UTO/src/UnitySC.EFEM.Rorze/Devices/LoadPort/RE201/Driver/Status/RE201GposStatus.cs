namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.Status
{
    public class RE201GposStatus : Equipment.Abstractions.Drivers.Common.Status
    {
        #region Status Override

        public override object Clone()
        {
            return new RE201GposStatus(this);
        }

        #endregion Status Override

        #region Fields

        private uint _zAxis;
        private uint _rotationAxis;

        #endregion

        #region Properties

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

        #endregion

        #region Constructors

        public RE201GposStatus(RE201GposStatus other)
        {
            Set(other);
        }

        public RE201GposStatus(string messageStatusData)
        {
            var statuses = messageStatusData.Replace(":", string.Empty).Split('/');

            if (statuses.Length != 2)
            {
                SetDefaultsValue();
                return;
            }

            try
            {
                //If the "HOME" command is executed, the data of the Z-Axis is totally five digits consisting of two digits (carrier type)
                //and three digits (slot).
                if (statuses[0].Length == 5)
                {
                    var zAxis = statuses[0].Remove(0, 2);
                    uint.TryParse(zAxis, out _zAxis);
                }
                else
                {
                    uint.TryParse(statuses[0], out _zAxis);
                }

                uint.TryParse(statuses[1], out _rotationAxis);
            }
            catch
            {
                SetDefaultsValue();
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Copy statuses from on received data.
        ///     <param name="other">If null, Reset values, otherwise, set</param>
        /// </summary>
        private void Set(RE201GposStatus other = null)
        {
            lock (this)
            {
                if (other == null)
                {
                    SetDefaultsValue();
                }
                else
                {
                    _zAxis = other.ZAxis;
                    _rotationAxis = other.RotationAxis;
                }
            }
        }

        private void SetDefaultsValue()
        {
            _zAxis = 0;
            _rotationAxis = 0;
        }

        #endregion Private Methods
    }
}
