using System;

using Agileo.SemiDefinitions;
using Agileo.StateMachine;

namespace UnitySC.Equipment.Abstractions.Vendor.Devices.Activities
{
    /// <summary>
    /// Event associated to the latest transition of Activities SM.
    /// It allows sending the <see cref="ActivityDoneEvent"/> with status of the execution
    /// </summary>
    public class ActivityDoneEvent : Event
    {
        private ActivityDoneEvent(CommandStatusCode status, string errText, Exception exception)
        {
            Status = status;
            ErrText = errText;
            Exception = exception;
        }

        public CommandStatusCode Status { get; }

        public string ErrText { get; }

        public Exception Exception { get; }

        public static ActivityDoneEvent OnSuccess() => new(CommandStatusCode.Ok, string.Empty, null);

        public static ActivityDoneEvent OnWarning(string errorMessage)
            => new(CommandStatusCode.Warning, errorMessage, null);

        public static ActivityDoneEvent OnError(string errorMessage)
            => new(CommandStatusCode.Error, errorMessage, null);

        public static ActivityDoneEvent OnRejected(string errorMessage)
            => new(CommandStatusCode.Rejected, errorMessage, null);

        public static ActivityDoneEvent OnValidationError(string errorMessage)
            => new(CommandStatusCode.ValidationError, errorMessage, null);

        public static ActivityDoneEvent OnException(Exception exception, string errorMessage)
            => new(CommandStatusCode.ExceptionThrown, errorMessage, exception);
    }

    /// <summary>
    /// Event associated to the Stop command on Activity SM.
    /// </summary>
    public class StopActivity : Event
    {
    }
}
