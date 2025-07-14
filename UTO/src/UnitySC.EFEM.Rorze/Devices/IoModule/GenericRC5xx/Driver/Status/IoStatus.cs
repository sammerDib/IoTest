using System;
using System.Globalization;

using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver.Enums;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver.Status
{
    /// <summary>
    /// Aims to provide IO card status.
    /// </summary>
    public class IoStatus : Equipment.Abstractions.Drivers.Common.Status
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IoStatus"/> class.
        /// <param name="other">Create a deep copy of <see cref="IoStatus"/> instance</param>
        /// </summary>
        public IoStatus(IoStatus other)
        {
            Set(other);
        }

        public IoStatus(string messageStatusData)
        {
            var statuses = messageStatusData.Replace(":", string.Empty).Split('/');

            // Status
            OperationMode     = (OperationMode)Enum.Parse(typeof(OperationMode), statuses[0][0].ToString());
            CommandProcessing = (CommandProcessing)Enum.Parse(typeof(CommandProcessing), statuses[0][2].ToString());

            // Errors
            IoModuleInError = int.Parse(statuses[1].Substring(0, 2), NumberStyles.AllowHexSpecifier);
            ErrorCode       = int.Parse(statuses[1].Substring(2), NumberStyles.AllowHexSpecifier);
        }

        #endregion Constructors

        #region Properties

        public OperationMode OperationMode { get; private set; }

        public CommandProcessing CommandProcessing { get; private set; }

        public int IoModuleInError { get; private set; }

        public int ErrorCode { get; private set; }

        #endregion Properties

        #region Overrides

        /// <summary>
        /// Create an object that it is a DEEP copy of current instance
        /// <returns>new object instance that it is a DEEP copy</returns>
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new IoStatus(this);
        }

        #endregion Overrides

        #region Internal Methods

        /// <summary>
        /// Copy statuses from on received data.
        /// <param name="other">If null, Reset values, otherwise, set</param>
        /// </summary>
        internal void Set(IoStatus other = null)
        {
            lock (this)
            {
                if (other == null)
                {
                    // Status
                    OperationMode     = OperationMode.Initializing;
                    CommandProcessing = CommandProcessing.Stop;

                    // Errors
                    IoModuleInError = 0;
                    ErrorCode       = 0;
                }
                else
                {
                    // Status
                    OperationMode     = other.OperationMode;
                    CommandProcessing = other.CommandProcessing;

                    // Errors
                    IoModuleInError = other.IoModuleInError;
                    ErrorCode       = other.ErrorCode;
                }
            }
        }

        #endregion Internal Methods
    }
}
