using System;

namespace UnitySC.Equipment.Abstractions.Drivers.Common.Exceptions
{
    /// <summary>
    /// CommandExecutionException
    /// </summary>
    public class CommandExecutionException : Exception
    {
        /// <summary>
        /// Exception that was thrown when device command execution failed
        /// </summary>
        /// <param name="command">Driver command</param>
        /// <param name="errors">Errors</param>
        public CommandExecutionException(DriverCommand command, string errors)
            : base($"Command '{command.Name}' with id '{command.Id}' failed. Error(s):{Environment.NewLine}{errors}")
        {
        }
    }
}
