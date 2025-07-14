using System;
using System.Threading;
using System.Threading.Tasks;

using Agileo.Common.Logging;
using Agileo.SemiDefinitions;

namespace UnitySC.Equipment.Abstractions.Vendor.Devices.Activities
{
    /// <summary>
    /// Manager that keep the activity in progress and manages all activities
    /// </summary>
    public class ActivityManager : IDisposable
    {
        private TaskCompletionSource<ErrorDescription> _tcs;

        #region Constructors

        public ActivityManager(ILogger logger)
        {
            Logger = logger;
        }

        #endregion

        #region Properties

        protected ILogger Logger { get; }

        /// <summary>
        /// Gets the Activity in progress
        /// </summary>
        public Activity Activity { get; private set; }

        #endregion

        #region Events

        /// <summary>
        /// Sent when Activity starts.
        /// </summary>
        public event EventHandler<ActivityEventArgs> ActivityStarted;

        /// <summary>
        /// Sent when Activity ends.
        /// </summary>
        public event EventHandler<ActivityEventArgs> ActivityDone;

        #endregion Events

        /// <summary>
        /// Creates and start the provided Activity
        /// </summary>
        /// <param name="activity">Activity to start. Only 1 activity can be active at the same time.
        /// If an activity is in progress, or if the selected activity detects that it can't start,
        /// an error code is returned. The only exception is the STOP activity that requires the
        /// Production activity in progress.</param>
        /// <returns>Error code and Error description. If No error, ErrorCode == <see cref="ErrorCode.Ok"/>.</returns>
        public ErrorDescription Start(Activity activity)
        {
            if (Activity != null)
            {
                var error = new ErrorDescription(
                    ErrorCode.Busy,
                    $"{Activity.Id} activity is already in use. Not allowed to start.");
                Logger.Warning("Activity '{ActivityId}' rejected: {@Error}", activity.Id, error);
                return error;
            }

            Logger.Info("Check activity '{ActivityId}'", activity.Id);
            var err = activity.Check(this);
            if (err.ErrorCode != ErrorCode.Ok)
            {
                Logger.Warning("Activity '{ActivityId}' rejected: {@Error}", activity.Id, err);
                return err;
            }

            Activity = activity;
            Activity.ActivityDone += Activity_ActivityDone;
            Activity.ActivityStarted += Activity_ActivityStarted;
            err = Activity.Start();
            if (err.ErrorCode != ErrorCode.Ok)
            {
                Logger.Warning("Activity '{ActivityId}' cannot start: {@Error}", activity.Id, err);
            }
            else
            {
                Logger.Info("Activity '{ActivityId}' started", activity.Id);
            }

            return err;
        }

        /// <summary>
        /// Creates, starts and wait the provided Activity
        /// </summary>
        /// <param name="activity">Activity to start and wait. Only 1 activity can be active at the same time.
        /// If an activity is in progress, or if the selected activity detects that it can't start,
        /// an error code is returned. The only exception is the STOP activity that requires the
        /// Production activity in progress.</param>
        /// <param name="timeout">The number of milliseconds to wait, or Infinite (-1) to wait indefinitely.</param>
        /// <returns>Error code and Error description. If No error, ErrorCode == <see cref="ErrorCode.Ok"/>.</returns>
        public ErrorDescription Run(Activity activity, int timeout = Timeout.Infinite)
        {
            _tcs = new TaskCompletionSource<ErrorDescription>();
            var err = Start(activity);
            if (err.ErrorCode != ErrorCode.Ok)
            {
                return err;
            }

            _tcs.Task.Wait(timeout);
            return _tcs.Task.Result;
        }

        /// <summary>
        /// If an activity is stored but didn't start or isn't removed for any reasons, Free the Activity
        /// </summary>
        /// <returns>IF errorCode equals OK, canceled. Otherwise, it is not possible to cancel the Current activity</returns>
        public ErrorDescription Remove()
        {
            if (Activity == null)
            {
                Logger.Debug("There is no activity to Remove. Command is ignored");
                return new ErrorDescription(ErrorCode.CommandNotValidForCurrentState, "There is no activity to cancel");
            }

            Logger.Info("Remove current activity '{ActivityId}'", Activity.Id);
            DisposeActivity(Activity);
            Activity = null;
            return new ErrorDescription();
        }

        public ErrorDescription Abort()
        {
            if (Activity == null)
            {
                Logger.Debug("There is no activity to Abort. Command is ignored");
                return new ErrorDescription(ErrorCode.CommandNotValidForCurrentState, "There is no activity to abort");
            }

            Logger.Info("Abort current activity '{ActivityId}'", Activity.Id);
            return Activity.Abort();
        }

        /// <summary>
        /// Asynchronous Method disposing the current Activity
        /// Deletion of Activity must be done outside of its own event Handler.
        /// </summary>
        /// <param name="activity">Activity to be disposed</param>
        private void DisposeActivity(Activity activity)
            => Task.Run(
                () =>
                {
                    if (activity == null)
                    {
                        return;
                    }

                    activity.ActivityDone -= Activity_ActivityDone;
                    activity.ActivityStarted -= Activity_ActivityStarted;
                    activity.Dispose();
                    activity = null;
                });

        #region Event Handlers

        private void Activity_ActivityDone(object sender, ActivityEventArgs e)
        {
            if (e.Status == CommandStatusCode.Ok)
            {
                Logger.Info("Activity '{ActivityId}' done with success.", e.Activity.Id);
            }
            else
            {
                Logger.Warning(
                    e.Exception,
                    "Activity '{ActivityId}' done with error: {ErrorDescription} ({ErrorCode}).",
                    e.Activity.Id,
                    e.ErrorText,
                    e.Status);
            }

            // Copy activity into local variable for event handler and free property to allow new Activity to start
            var lastActivity = Activity;
            Activity = null;

            _tcs?.TrySetResult(
                e.Status == CommandStatusCode.Ok
                    ? new ErrorDescription()
                    : new ErrorDescription(ErrorCode.Error, e.ErrorText));

            var activityEventArgs = e.Status == CommandStatusCode.Ok
                ? ActivityEventArgs.OnSuccess(lastActivity)
                : ActivityEventArgs.OnError(lastActivity, e.Status, e.ErrorText, e.Exception);

            ActivityDone?.Invoke(this, activityEventArgs);

            // Deletion of Activity must be done outside of its own event Handler.
            DisposeActivity(lastActivity);
        }

        private void Activity_ActivityStarted(object sender, ActivityEventArgs e)
            => ActivityStarted?.Invoke(this, ActivityEventArgs.OnSuccess(Activity));

        #endregion Event Handlers

        #region IDisposable Support

        private bool _disposedValue;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">indicates whether the method call comes from a Dispose method (its value is true) or from a destructor (its value is false).</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue)
            {
                return;
            }

            if (disposing)
            {
                DisposeActivity(Activity);
                Activity = null;
            }

            _disposedValue = true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
