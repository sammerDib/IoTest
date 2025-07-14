using System;
using System.Reflection;
using System.Text;

namespace UnitySC.Equipment.Abstractions
{
    /// <summary>
    /// Class responsible to contain information to be published by an event related to a command.
    /// </summary>
    /// <typeparam name="T">Type of class containing command parameters</typeparam>
    public class CommandEventArgs<T> : ICloneable where T : ICloneable
    {
        #region Properties

        /// <summary>
        /// Gets the command parameters.
        /// </summary>
        public T CommandParameters { get; }

        /// <summary>
        /// Gets the command unique identifier
        /// </summary>
        public Guid Uuid { get; }

        /// <summary>
        /// Gets the command identifier.
        /// </summary>
        public string CommandId { get; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandEventArgs{T}"/> class.
        /// </summary>
        public CommandEventArgs() :
            this(Guid.NewGuid(), string.Empty, default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandEventArgs{T}" /> class.
        /// Protected constructor for ICloneable.
        /// </summary>
        /// <param name="other">The object to be cloned.</param>
        protected CommandEventArgs(CommandEventArgs<T> other)
            : this(other.Uuid, other.CommandId, (T)other.CommandParameters.Clone())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandEventArgs{T}" /> class specifying all parameters.
        /// </summary>
        /// <param name="uid">The unique identifier associated to the command.</param>
        /// <param name="commandId">The command identifier.</param>
        /// <param name="commandParameters">The instance containing the command event data.</param>
        public CommandEventArgs(Guid uid, string commandId, T commandParameters)
        {
            Uuid              = uid;
            CommandId         = commandId;
            CommandParameters = commandParameters;
        }

        #endregion Constructors

        #region ICloneable

        /// <summary>
        /// Creates an object which is a copy of actual instance.
        /// </summary>
        /// <returns>
        /// New object which is a copy of this instance.
        /// </returns>
        public virtual object Clone()
        {
            return new CommandEventArgs<T>(this);
        }

        #endregion ICloneable

        #region Overrides

        /// <summary>
        /// Returns a human-readable text describing this object.
        /// </summary>
        /// <returns>A string, as human-readable text.</returns>
        /// <remarks>Overridden from object.</remarks>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"[{GetType().Name}]");

            foreach (PropertyInfo prop in GetType().GetProperties())
                sb.AppendLine($"\t{prop.Name} = {prop.GetValue(this, null) ?? "null"}");

            return sb.ToString();
        }

        #endregion Overrides
    }
}
