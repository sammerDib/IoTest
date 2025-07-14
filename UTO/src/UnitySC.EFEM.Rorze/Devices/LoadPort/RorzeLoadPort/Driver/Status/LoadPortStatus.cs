using System;

using UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.Enums;
using UnitySC.EFEM.Rorze.Drivers;
using UnitySC.EFEM.Rorze.Drivers.Enums;

using OperationMode = UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.Enums.OperationMode;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.Status
{
    /// <summary>Aims to provides Load Port Status</summary>
    public class LoadPortStatus : Equipment.Abstractions.Drivers.Common.Status
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoadPortStatus" /> class.
        /// <param name="other">Create a deep copy of <see cref="LoadPortStatus" /> instance</param>
        /// </summary>
        public LoadPortStatus(LoadPortStatus other)
        {
            Set(other);
        }

        public LoadPortStatus(string messageStatusData)
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
        }

        #region Internal Methods

        /// <summary>
        /// Copy statuses from on received data.
        /// <param name="other">If null, Reset values, otherwise, set</param>
        /// </summary>
        internal void Set(LoadPortStatus other = null)
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
                }
                else
                {
                    // Status
                    OperationMode = other.OperationMode;
                    OriginReturnCompletion = other.OriginReturnCompletion;
                    CommandProcessing = other.CommandProcessing;
                    OperationStatus = other.OperationStatus;
                    MotionSpeed = other.MotionSpeed;
                }
            }
        }

        #endregion Internal Methods

        #region Properties

        public OperationMode OperationMode { get; private set; }

        public OriginReturnCompletion OriginReturnCompletion { get; private set; }

        public CommandProcessing CommandProcessing { get; private set; }

        public OperationStatus OperationStatus { get; private set; }

        /// <summary>Value = 0 for Normal speed, 1 to 10 for reduced speed.</summary>
        public uint MotionSpeed { get; private set; }

        #endregion Properties

        #region Overrides

        /// <summary>
        /// Create an object that it is a DEEP copy of current instance
        /// <returns>new object instance that it is a DEEP copy</returns>
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new LoadPortStatus(this);
        }

        #endregion Overrides
    }
}
