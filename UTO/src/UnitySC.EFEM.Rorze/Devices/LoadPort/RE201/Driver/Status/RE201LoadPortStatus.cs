using System;
using System.Globalization;

using UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.Status;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RE201.Driver.Status
{
    public class RE201LoadPortStatus : LoadPortStatus
    {
        #region Constructors

        public RE201LoadPortStatus(RE201LoadPortStatus other) : base(other)
        {
        }

        public RE201LoadPortStatus(string messageStatusData)
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

        internal void Set(RE201LoadPortStatus other = null)
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
            return new RE201LoadPortStatus(this);
        }

        #endregion
    }
}
