using System;
using System.Globalization;

using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Enums;
using UnitySC.EFEM.Rorze.Drivers;
using UnitySC.EFEM.Rorze.Drivers.Enums;

using OperationMode = UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Enums.OperationMode;

namespace UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Status
{
    /// <summary>Aims to provides Robot Status</summary>
    public class RobotStatus : Equipment.Abstractions.Drivers.Common.Status
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RobotStatus" /> class.
        /// <param name="other">Create a deep copy of <see cref="RobotStatus" /> instance</param>
        /// </summary>
        public RobotStatus(RobotStatus other)
        {
            Set(other);
        }

        public RobotStatus(string messageStatusData)
        {
            var statuses = messageStatusData.Replace(":", string.Empty).Split('/');

            // Status
            OperationMode = (OperationMode)Enum.Parse(
                typeof(OperationMode),
                statuses[0][0].ToString());
            OriginReturnCompletion = (OriginReturnCompletion)Enum.Parse(
                typeof(OriginReturnCompletion),
                statuses[0][1].ToString());
            CommandProcessing = (CommandProcessing)Enum.Parse(
                typeof(CommandProcessing),
                statuses[0][2].ToString());
            OperationStatus = (OperationStatus)Enum.Parse(
                typeof(OperationStatus),
                statuses[0][3].ToString());
            MotionSpeed = (uint)RorzeConstants.CharToInt(statuses[0][4]);

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

        #region Internal Methods

        /// <summary>
        /// Copy statuses from on received data.
        /// <param name="other">If null, Reset values, otherwise, set</param>
        /// </summary>
        internal void Set(RobotStatus other = null)
        {
            lock (this)
            {
                if (other == null)
                {
                    // Status
                    OperationMode = OperationMode.Initializing;
                    OriginReturnCompletion = OriginReturnCompletion.NotCompleted;
                    CommandProcessing = CommandProcessing.Stop;
                    OperationStatus = OperationStatus.Stop;
                    MotionSpeed = 0;

                    // Errors
                    ErrorControllerId = ErrorControllerId.Others;
                    ErrorCode = ErrorCode.None;
                }
                else
                {
                    // Status
                    OperationMode = other.OperationMode;
                    OriginReturnCompletion = other.OriginReturnCompletion;
                    CommandProcessing = other.CommandProcessing;
                    OperationStatus = other.OperationStatus;
                    MotionSpeed = other.MotionSpeed;

                    // Errors
                    ErrorControllerId = other.ErrorControllerId;
                    ErrorCode = other.ErrorCode;
                }
            }
        }

        #endregion Internal Methods

        #region Properties

        public OperationMode OperationMode { get; private set; }

        public OriginReturnCompletion OriginReturnCompletion { get; private set; }

        public CommandProcessing CommandProcessing { get; private set; }

        public OperationStatus OperationStatus { get; private set; }

        /// <summary>Value = 0 for Normal speed, 1 to 20 for reduced speed.</summary>
        public uint MotionSpeed { get; private set; }

        public ErrorControllerId ErrorControllerId { get; private set; }

        public ErrorCode ErrorCode { get; private set; }

        #endregion Properties

        #region Overrides

        /// <summary>
        /// Create an object that it is a DEEP copy of current instance
        /// <returns>new object instance that it is a DEEP copy</returns>
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new RobotStatus(this);
        }

        #endregion Overrides
    }
}
