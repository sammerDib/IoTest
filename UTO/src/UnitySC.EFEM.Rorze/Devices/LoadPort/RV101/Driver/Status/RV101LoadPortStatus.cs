using System;
using System.Globalization;

using UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.Status;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.Enums;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.Status
{
    public class RV101LoadPortStatus : LoadPortStatus
    {
        #region Constructors

        public RV101LoadPortStatus(RV101LoadPortStatus other) : base(other)
        {
        }

        public RV101LoadPortStatus(string messageStatusData)
            : base(messageStatusData)
        {
            var statuses = messageStatusData.Replace(":", string.Empty).Split('/');

            // Errors
            var errorSrcId = int.Parse(statuses[1].Substring(0, 2), NumberStyles.AllowHexSpecifier);
            if (Enum.IsDefined(typeof(ErrorControllerId), errorSrcId))
            {
                ErrorControllerId = (ErrorControllerId)errorSrcId;
            }
            else
            {
                throw new ArgumentException(
                    $@"Contains invalid error controller id. Status={messageStatusData}, Id={errorSrcId}",
                    nameof(messageStatusData));
            }

            var errorCode = int.Parse(statuses[1].Substring(2, 2), NumberStyles.AllowHexSpecifier);
            if (Enum.IsDefined(typeof(ErrorCode), errorCode))
            {
                ErrorCode = (ErrorCode)errorCode;
            }
            else
            {
                throw new ArgumentException(
                    $@"Contains invalid error code. Status={messageStatusData}, Id={errorCode}",
                    nameof(messageStatusData));
            }
        }

        #endregion

        #region Properties

        public ErrorControllerId ErrorControllerId { get; private set; }

        public ErrorCode ErrorCode { get; private set; }

        #endregion

        #region Overrides

        internal void Set(RV101LoadPortStatus other = null)
        {
            base.Set(other);

            lock (this)
            {
                if (other == null)
                {
                    // Errors
                    ErrorControllerId = ErrorControllerId.Others;
                    ErrorCode = ErrorCode.None;
                }
                else
                {
                    // Errors
                    ErrorControllerId = other.ErrorControllerId;
                    ErrorCode = other.ErrorCode;
                }
            }
        }

        public override object Clone()
        {
            return new RV101LoadPortStatus(this);
        }

        #endregion
    }
}
