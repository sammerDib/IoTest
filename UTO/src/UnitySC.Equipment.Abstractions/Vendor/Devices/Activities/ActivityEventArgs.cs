using System;
using System.Text;

using Agileo.SemiDefinitions;

namespace UnitySC.Equipment.Abstractions.Vendor.Devices.Activities
{
    /// <summary>
    /// Event arg associated to an activity
    /// </summary>
    public class ActivityEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityEventArgs"/> class.
        /// </summary>
        /// <param name="activity">Activity that finished the command</param>
        /// <param name="status">Status of the Command done</param>
        /// <param name="errorText">Error text in case of <paramref name="status"/> is different to <see cref="CommandStatusCode.Ok"/></param>
        /// <param name="exception">Exception in case of <paramref name="status"/> is different to <see cref="CommandStatusCode.Ok"/></param>
        protected ActivityEventArgs(Activity activity, CommandStatusCode status, string errorText, Exception exception)
        {
            Activity = activity;
            Status = status;
            ErrorText = errorText;
            Exception = exception;
        }

        public static ActivityEventArgs OnSuccess(Activity activity)
        {
            return new ActivityEventArgs(activity, CommandStatusCode.Ok, string.Empty, null);
        }

        public static ActivityEventArgs OnError(
            Activity activity,
            CommandStatusCode status,
            string errorMessage,
            Exception exception = null)
        {
            return new ActivityEventArgs(activity, status, errorMessage, exception);
        }

        /// <summary>
        /// Gets the Status of the command
        /// </summary>
        public CommandStatusCode Status { get; }

        /// <summary>
        /// Gets the Associated error text if an error occurred
        /// </summary>
        public string ErrorText { get; }

        /// <summary>
        /// Gets the associated <see cref="Exception"/> if an error occurred.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Gets the Associated Activity
        /// </summary>
        public Activity Activity { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            StringBuilder stringBuilder = new();
            stringBuilder.Append('[').Append(nameof(ActivityEventArgs)).AppendLine("]");
            AppendPropertiesToString(stringBuilder);
            return stringBuilder.ToString();
        }

        protected void AppendPropertiesToString(StringBuilder stringBuilder)
        {
            stringBuilder.Append(nameof(Activity)).Append(" = ").Append(Activity).AppendLine();
            stringBuilder.Append(nameof(Status)).Append(" = ").Append(Status).AppendLine();
            stringBuilder.Append(nameof(ErrorText)).Append(" = ").AppendLine(ErrorText);
            stringBuilder.Append(nameof(Exception)).Append(" = ").Append(Exception?.ToString() ?? string.Empty);
        }
    }
}
