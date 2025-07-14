using System;

using UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Enums;

namespace UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.Status
{
    public class AlignerGposStatus : Equipment.Abstractions.Drivers.Common.Status
    {
        #region Fields

        private XAxisPosition _xAxisPosition;
        private YAxisPosition _yAxisPosition;
        private ZAxisPosition _zAxisPosition;

        #endregion

        #region Properties

        public XAxisPosition XAxisPosition
        {
            get => _xAxisPosition;
            internal set => _xAxisPosition = value;
        }

        public YAxisPosition YAxisPosition
        {
            get => _yAxisPosition;
            internal set => _yAxisPosition = value;
        }

        public ZAxisPosition ZAxisPosition
        {
            get => _zAxisPosition;
            internal set => _zAxisPosition = value;
        }

        #endregion

        #region Constructors

        public AlignerGposStatus(AlignerGposStatus other)
        {
            Set(other);
        }

        public AlignerGposStatus(string messageStatusData)
        {
            var statuses = messageStatusData.Replace(":", string.Empty).Split('/');

            if (statuses.Length != 4)
            {
                SetDefaultsValue();
                return;
            }

            try
            {
                Enum.TryParse(statuses[0], out _xAxisPosition);
                Enum.TryParse(statuses[1], out _yAxisPosition);
                Enum.TryParse(statuses[2], out _zAxisPosition);
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
        private void Set(AlignerGposStatus other = null)
        {
            lock (this)
            {
                if (other == null)
                {
                    SetDefaultsValue();
                }
                else
                {
                    _xAxisPosition = other._xAxisPosition;
                    _yAxisPosition = other._yAxisPosition;
                    _zAxisPosition = other._zAxisPosition;
                }
            }
        }

        private void SetDefaultsValue()
        {
            _xAxisPosition = XAxisPosition.XAxisAtOrigin;
            _yAxisPosition = YAxisPosition.YAxisAtOrigin;
            _zAxisPosition = ZAxisPosition.ZAxisAtOrigin;
        }

        #endregion Private Methods

        #region Status Override

        public override object Clone()
        {
            return new AlignerGposStatus(this);
        }

        #endregion Status Override
    }
}
