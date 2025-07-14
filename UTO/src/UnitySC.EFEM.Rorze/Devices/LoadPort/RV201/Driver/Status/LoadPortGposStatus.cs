using System;

using UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.Enums;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV201.Driver.Status
{
    public class LoadPortGposStatus : Equipment.Abstractions.Drivers.Common.Status
    {
        #region Fields

        private YAxisPositions _yAxis;
        private uint _zAxis;
        private uint _liftLower;
        private uint _carrierRetaining;

        #endregion

        #region Properties

        public YAxisPositions YAxis
        {
            get => _yAxis;
            internal set => _yAxis = value;
        }

        public uint ZAxis
        {
            get => _zAxis;
            internal set => _zAxis = value;
        }

        public uint LiftLower
        {
            get => _liftLower;
            internal set => _liftLower = value;
        }

        public uint CarrierRetaining
        {
            get => _carrierRetaining;
            internal set => _carrierRetaining = value;
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
                        Enum.TryParse(statuses[0], out _yAxis);
                        uint.TryParse(statuses[1], out _zAxis);
                        break;
                    case 3:
                        Enum.TryParse(statuses[0], out _yAxis);
                        uint.TryParse(statuses[1], out _zAxis);
                        uint.TryParse(statuses[2], out _liftLower);
                        break;
                    case 4:
                        Enum.TryParse(statuses[0], out _yAxis);
                        uint.TryParse(statuses[1], out _zAxis);
                        uint.TryParse(statuses[2], out _liftLower);
                        uint.TryParse(statuses[3], out _carrierRetaining);
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
                    _yAxis            = other._yAxis;
                    _zAxis            = other.ZAxis;
                    _liftLower        = other._liftLower;
                    _carrierRetaining = other._carrierRetaining;
                }
            }
        }

        private void SetDefaultsValue()
        {
            _yAxis            = 0;
            _zAxis            = 0;
            _liftLower        = 0;
            _carrierRetaining = 0;
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
