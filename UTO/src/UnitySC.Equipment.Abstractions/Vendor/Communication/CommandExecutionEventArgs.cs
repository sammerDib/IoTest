using System;

using Agileo.SemiDefinitions;

namespace UnitySC.Equipment.Abstractions.Vendor.Communication
{
    /// <summary>
    /// Notifies that a command is finished.
    /// </summary>
    public class CommandExecutionEventArgs
    {
        private Exception _exception;

        /// <summary>
        /// Initializes a new instance of <see cref="CommandExecutionEventArgs"/>.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cmdName">The command name.</param>
        /// <param name="status">The status of the command.</param>
        /// <param name="errors">Errors occurred.</param>
        private CommandExecutionEventArgs(uint id, string cmdName, CommandStatusCode status, string[] errors)
        {
            Name = cmdName;
            Status = status;
            Id = id;
            Errors = errors;
        }

        /// <summary>
        /// Indicates whether the command is successfully done or not.
        /// </summary>
        public CommandStatusCode Status { get; }

        /// <summary>
        /// Name of the command.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        public uint Id { get; }

        /// <summary>
        /// Gets the list of errors.
        /// </summary>
        public string[] Errors { get; }

        /// <summary>
        /// Gets the exception thrown.
        /// </summary>
        public Exception Exception
        {
            get
            {
                if (_exception != null || Errors == null)
                    return _exception;

                _exception = new InvalidOperationException(string.Join(";", Errors));
                return _exception;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandExecutionEventArgs"/> class with <see cref="CommandStatusCode"/> set to <see cref="CommandStatusCode.Ok"/>.
        /// </summary>
        /// <param name="id">Identifier.</param>
        /// <param name="cmdName">Command name.</param>
        /// <returns>Provides a <see cref="CommandExecutionEventArgs"/> instance with <see cref="CommandStatusCode"/> set to <see cref="CommandStatusCode.Ok"/>.</returns>
        public static CommandExecutionEventArgs Ok(uint id, string commandName)
        {
            return new CommandExecutionEventArgs(id, commandName, CommandStatusCode.Ok, null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandExecutionEventArgs"/> class with <see cref="CommandStatusCode"/> set to <see cref="CommandStatusCode.Error"/>.
        /// </summary>
        /// <param name="id">Identifier.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="errors">Errors occurred.</param>
        /// <returns>Provides a <see cref="CommandExecutionEventArgs"/> instance with <see cref="CommandStatusCode"/> set to <see cref="CommandStatusCode.Error"/>.</returns>
        public static CommandExecutionEventArgs Failed(uint id, string cmdName, string errors)
        {
            return new CommandExecutionEventArgs(id, cmdName, CommandStatusCode.Error, new[] { errors });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandExecutionEventArgs"/> class with <see cref="CommandStatusCode"/> set to <see cref="CommandStatusCode.Rejected"/>.
        /// </summary>
        /// <param name="id">Identifier.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="errors">Errors occurred.</param>
        /// <returns>Provides a <see cref="CommandExecutionEventArgs"/> instance with <see cref="CommandStatusCode"/> set to <see cref="CommandStatusCode.Rejected"/>.</returns>
        public static CommandExecutionEventArgs Rejected(uint id, string cmdName, string errors)
        {
            return new CommandExecutionEventArgs(id, cmdName, CommandStatusCode.Rejected, new[] { errors });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandExecutionEventArgs"/> class with <see cref="CommandStatusCode"/> set to <see cref="CommandStatusCode.Warning"/>.
        /// </summary>
        /// <param name="id">Identifier.</param>
        /// <param name="cmdName">Command name.</param>
        /// <param name="warnings">Warnings occurred</param>
        /// <returns>Provides a <see cref="CommandExecutionEventArgs"/> instance with <see cref="CommandStatusCode"/> set to <see cref="CommandStatusCode.Warning"/>.</returns>
        public static CommandExecutionEventArgs Warning(uint id, string cmdName, string warnings)
        {
            return new CommandExecutionEventArgs(id, cmdName, CommandStatusCode.Warning, new[] { warnings });
        }
    }
}
