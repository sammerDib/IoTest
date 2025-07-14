using System;

using Agileo.Drivers;

namespace UnitySC.Equipment.Abstractions.Devices.Robot
{
    /// <summary>
    /// Class responsible to contain information about the robot command which needs confirmation to run.
    /// </summary>
    public class CommandConfirmationRequestedEventArgs : CommandEventArgs<RobotAction>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandConfirmationRequestedEventArgs"/> class.
        /// Protected constructor for ICloneable.
        /// </summary>
        /// <param name="other">The object to be cloned.</param>
        protected CommandConfirmationRequestedEventArgs(CommandConfirmationRequestedEventArgs other)
            : base(other)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandConfirmationRequestedEventArgs"/> class specifying all parameters.
        /// </summary>
        /// <param name="uid">The unique identifier associated to the command.</param>
        /// <param name="action">The <see cref="RobotAction"/> instance containing the robot command parameters.</param>
        public CommandConfirmationRequestedEventArgs(Guid uid, RobotAction action)
            : base(uid, action.Command, action)
        {
        }
    }
}
