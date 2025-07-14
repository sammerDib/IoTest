using System;
using System.Text;

using Agileo.SemiDefinitions;

namespace UnitySC.Equipment.Abstractions.Vendor.Devices.Activities
{
    /// <summary>
    /// Hold information about command started/finished by an activity.
    /// </summary>
    public class ActivityCommandEventArgs : ActivityEventArgs
    {
        /// <summary>
        /// Gets the Identification of the command
        /// </summary>
        public string CommandId { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityCommandEventArgs"/> class.
        /// </summary>
        /// <param name="activity">Activity on which command is applied</param>
        /// <param name="commandId">Command started/done on the specified activity</param>
        /// <param name="status">Status of the Command</param>
        /// <param name="errorText">Error text in case of <paramref name="status"/> is different to <see cref="CommandStatusCode.Ok"/></param>
        /// <param name="exception">Exception in case of <paramref name="status"/> is different to <see cref="CommandStatusCode.Ok"/></param>
        protected ActivityCommandEventArgs(
            Activity activity,
            string commandId,
            CommandStatusCode status,
            string errorText,
            Exception exception)
            : base(activity, status, errorText, exception)
        {
            CommandId = commandId;
        }

        public static ActivityCommandEventArgs OnSuccess(Activity activity, string commandId)
            => new ActivityCommandEventArgs(activity, commandId, CommandStatusCode.Ok, string.Empty, null);

        public static ActivityCommandEventArgs OnError(
            Activity activity,
            string commandId,
            CommandStatusCode status,
            string errorMessage,
            Exception exception = null)
            => new ActivityCommandEventArgs(activity, commandId, status, errorMessage, exception);

        /// <inheritdoc />
        public override string ToString()
        {
            StringBuilder stringBuilder = new();
            stringBuilder.Append('[').Append(nameof(ActivityCommandEventArgs)).AppendLine("]");
            AppendPropertiesToString(stringBuilder);
            stringBuilder.Append(nameof(CommandId)).Append(" = ").AppendLine(CommandId);
            return stringBuilder.ToString();
        }
    }
}
