using System;

using Agileo.Common.Logging;
using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;
using Agileo.StateMachine;

namespace UnitySC.Equipment.Abstractions.Vendor.Devices.Activities
{
    /// <summary>
    /// Activity model used to define State Machine activities
    /// </summary>
    public abstract class Activity : IDisposable
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Activity"/> class.
        /// Default constructor is required by auto-generated codes from State Machines, even if the default State Machine constructor is never used in the project.
        /// </summary>
        protected Activity()
        {
            // For State Machine compliance. Not used
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Activity"/> class.
        /// </summary>
        /// <param name="id">The activity identifier.</param>
        protected Activity(string id)
        {
            Id = id;
            Logger = Agileo.Common.Logging.Logger.GetLogger(Id + "_Activity");
            IsStopping = false;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the Activity identifier
        /// </summary>
        public string Id { get; protected set; }

        /// <summary>
        /// Gets or sets the current State of the State Machine
        /// </summary>
        public virtual string State { get; protected set; }

        /// <summary>
        /// Gets or sets the Start time of the execution.
        /// </summary>
        /// <value>value is updated when the Start method is called</value>
        public DateTime StartTime { get; protected set; }

        /// <summary>
        /// Gets or sets the End time of the execution.
        /// </summary>
        /// <value>value is updated when the Activity Done event is sent</value>
        public DateTime EndTime { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether <see cref="Activity"/> is started.
        /// </summary>
        /// <returns><c>true</c> if it is started, otherwise <c>false</c>.</returns>
        public bool IsStarted { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="Activity"/> is stopping.
        /// </summary>
        /// <returns><c>true</c> if it is stopping, otherwise <c>false</c>.</returns>
        public bool IsStopping { get; protected set; }
        
        /// <summary>
        /// Gets or sets the State Machine to be used to manage the Activity (if needed)
        /// </summary>
        protected StateMachine StateMachine { get; set; }

        /// <summary>
        /// Gets or sets the Logger to be used to log events, states and activities on instance.
        /// </summary>
        protected ILogger Logger { get; }

        #endregion Properties
        
        #region Events

        /// <summary>
        /// Sends the current state of the activity when changed.
        /// </summary>
        public event EventHandler<ActivityEventArgs> StateChanged;

        /// <summary>
        /// Sent when Activity starts.
        /// </summary>
        public event EventHandler<ActivityEventArgs> ActivityStarted;

        /// <summary>
        /// Sent when Activity ends.
        /// </summary>
        public event EventHandler<ActivityEventArgs> ActivityDone;

        #endregion Events

        #region Commands

        /// <summary>
        /// Starts the Activity.
        /// Default behavior is to set IsStarted to <see langword="true"/> and sets the StartTime.
        /// If a StateMachine has been created, call the Start Method on it and register events for logs
        /// </summary>
        /// <returns>OK if succeeded, ErrorCode otherwise</returns>
        internal ErrorDescription Start()
        {
            if (IsStarted)
            {
                return new ErrorDescription(ErrorCode.CommandNotValidForCurrentState, "Already in desired state.");
            }

            IsStarted = true;
            StartTime = DateTime.Now;

            if (StateMachine != null)
            {
                StateMachine.OnStateChanged += StateMachine_OnStateChanged;
                StateMachine.OnTransitionChanged += StateMachine_OnTransitionChanged;
                StateMachine.Start();
            }

            return StartFinalizer();
        }

        /// <summary>
        /// Check if the activity can be started. It validates the context and parameters
        /// </summary>
        /// <param name="context">Activity Manager is provided to provide the context of the required execution</param>
        /// <returns>OK if succeeded, ErrorCode otherwise</returns>
        public abstract ErrorDescription Check(ActivityManager context);

        /// <summary>
        /// In order to connect the State Machine to equipment events, it is required to finalize the Start Command for each sub-activities.
        /// This is the aim of the abstract method
        /// </summary>
        /// <returns>OK if succeeded, ErrorCode otherwise</returns>
        protected abstract ErrorDescription StartFinalizer();

        /// <summary>
        /// Abort the Activity
        /// </summary>
        /// <returns>OK if succeeded, ErrorCode otherwise</returns>
        public virtual ErrorDescription Abort()
        {
            if (!IsStarted)
            {
                return new ErrorDescription(ErrorCode.CommandNotValidForCurrentState, "Activity not started.");
            }

            StateMachine.PostEvent(ActivityDoneEvent.OnError("Activity is aborted"));
            return new ErrorDescription();
        }

        /// <summary>
        /// Stops execution of the Activity.
        /// </summary>
        /// <returns>ErrorCode.UnsupportedOptionRequested because command not supported by default</returns>
        public virtual ErrorDescription StopExecution()
        {
            if (!IsStarted)
            {
                return new ErrorDescription(ErrorCode.CommandNotValidForCurrentState, "Activity not started.");
            }

            StateMachine.PostEvent(new StopActivity());
            return new ErrorDescription();
        }

        #endregion Commands

        #region State Machine logs

        private void StateMachine_OnStateChanged(object sender, StateChangeArgs e)
        {
            if (!e.Activated)
            {
                return;
            }

            Logger.Info("[{ActivityId}] State entered: {State}", Id, e.Name);
            State = e.Name;
            OnStateChanged(ActivityEventArgs.OnSuccess(this));
        }

        private void StateMachine_OnTransitionChanged(object sender, TransitionChangeArgs e)
        {
            if (!e.Fired)
            {
                return;
            }

            Logger.Debug(
                "[{ActivityId}] {SourceState} -> {DestinationState} | {Event} {Condition} {Action}",
                Id,
                e.Source,
                e.Destination,
                e.Event,
                string.IsNullOrEmpty(e.Condition) ? string.Empty : $"[{e.Condition}]",
                string.IsNullOrEmpty(e.Action) ? string.Empty : $"/ {e.Action}");
        }

        #endregion

        #region Event Senders Methods

        /// <summary>
        /// Raises the <see cref="StateChanged" /> event.
        /// </summary>
        /// <param name="args">The <see cref="ActivityEventArgs"/> instance containing the event data.</param>
        protected void OnStateChanged(ActivityEventArgs args)
        {
            try
            {
                StateChanged?.Invoke(this, args);
            }
            catch (Exception ex)
            {
                Logger.Error(
                    ex,
                    "[{ActivityId}] {Method} - Exception occurred during event sending.",
                    Id,
                    nameof(OnStateChanged));
            }
        }

        /// <summary>
        /// Raises the <see cref="ActivityStarted" /> event.
        /// </summary>
        /// <param name="args">The <see cref="ActivityEventArgs"/> instance containing the event data.</param>
        protected void OnActivityStarted(ActivityEventArgs args)
        {
            try
            {
                ActivityStarted?.Invoke(this, args);
            }
            catch (Exception ex)
            {
                Logger.Error(
                    ex,
                    "[{ActivityId}] {Method} - Exception occurred during event sending.",
                    Id,
                    nameof(OnActivityStarted));
            }
        }

        /// <summary>
        /// Raises the <see cref="ActivityDone" /> event.
        /// </summary>
        /// <param name="args">The <see cref="ActivityEventArgs"/> instance containing the event data.</param>
        protected void OnActivityDone(ActivityEventArgs args)
        {
            try
            {
                ActivityDone?.Invoke(this, args);
            }
            catch (Exception ex)
            {
                Logger.Error(
                    ex,
                    "[{ActivityId}] {Method} - Exception occurred during event sending.",
                    Id,
                    nameof(OnActivityDone));
            }
        }

        #endregion Event Senders Methods

        #region IDisposable Support

        private bool _disposedValue;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">indicates whether the method call comes from a Dispose method (its value is true) or from a destructor (its value is false).</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing && StateMachine != null)
                {
                    // Remove callbacks for logs if the State Machine has been started.
                    if (IsStarted)
                    {
                        StateMachine.OnStateChanged -= StateMachine_OnStateChanged;
                        StateMachine.OnTransitionChanged -= StateMachine_OnTransitionChanged;
                        StateMachine.Dispose();
                    }

                    StateMachine = null;
                }

                _disposedValue = true;
            }

            IsStarted = false;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support

        #region Common State Machine Actions

        /// <summary>
        /// First Action to attach on Entry Action of State Machine to send <see cref="ActivityStarted"/> Event
        /// </summary>
        /// <param name="ev">event that starts the SM</param>
        protected virtual void ActivityEntry(Event ev) => OnActivityStarted(ActivityEventArgs.OnSuccess(this));

        /// <summary>
        /// Latest Action to attach on exit of State Machine to send <see cref="ActivityDone"/> Event.
        /// Almost cases, the <see cref="ActivityDone"/> event with Status = OK will be sent.
        /// In case of an error has been detected, the SM has to use the <see cref="ActivityDoneEvent"/> that goes out of State Machine and sends the Error Status.
        /// </summary>
        /// <param name="ev">event that contains the result of the execution. It must be <see cref="ActivityDoneEvent"/> instance type.</param>
        /// <remarks>If the Event is not a <see cref="ActivityDoneEvent"/> instance type, 'ActivityDone' Event is sent with Status 'Error'</remarks>
        protected virtual void ActivityExit(Event ev)
        {
            EndTime = DateTime.Now;

            var activityEventArgs = ev switch
            {
                ActivityDoneEvent { Status: not CommandStatusCode.Ok } doneEvent => ActivityEventArgs.OnError(
                    this,
                    doneEvent.Status,
                    doneEvent.ErrText,
                    doneEvent.Exception),
                _ => ActivityEventArgs.OnSuccess(this)
            };

            OnActivityDone(activityEventArgs);
        }

        /// <summary>
        /// Action to attach on stop of State Machine to send <see cref="ActivityDone"/> Event.
        /// The <see cref="ActivityDone"/> event with Status = warning will be sent.
        /// </summary>
        /// <param name="ev">event that leads to the execution stop. It must be <see cref="StopActivity"/> instance type.</param>
        protected virtual void ActivityStop(Event ev)
            => StateMachine.PostEvent(ActivityDoneEvent.OnWarning("Activity is stopped"));

        #endregion Common State Machine Actions

        #region Helpers

        /// <summary>
        /// Tries to perform the provided action.
        /// </summary>
        /// <param name="action">The action to be performed safely.</param>
        /// <remarks>In case action to perform fails, ActivityDone event is raised.</remarks>
        protected void TryCatch(Action action)
        {
            try
            {
                action();
            }
            catch (CommandExecutionFailedException commandException)
            {
                Logger.Debug(
                    "[{ActivityId}] Error occurred during command execution ({CommandExecutionFailedReason}), ActivityDone will be posted. {@Exception}",
                    Id,
                    commandException.Reason,
                    commandException);

                StateMachine?.PostEvent(
                    ActivityDoneEvent.OnException(commandException, "Error occurred during command execution"));
            }
            catch (Exception ex)
            {
                Logger.Debug(
                    "[{ActivityId}] Error occurred during command execution, ActivityDone will be posted. {@Exception}",
                    Id,
                    ex);

                StateMachine?.PostEvent(ActivityDoneEvent.OnException(ex, "Error occurred during command execution"));
            }
        }

        /// <summary>
        /// Tries to perform the provided action.
        /// </summary>
        /// <param name="action">The action to be performed safely.</param>
        /// <param name="nextEventToSend">Event to send when command is successfully executed; null if no event to send.</param>
        /// <remarks>In case action to perform fails, ActivityDone event is raised.</remarks>
        protected void TryCatch(Action action, Event nextEventToSend)
        {
            try
            {
                action(); // Synchronous command execution on a device

                // Added to protect in case of Abort when Pausing a WaferTransfer.
                if (StateMachine == null)
                {
                    return;
                }

                // Some times there is no need to send event
                if (nextEventToSend != null)
                {
                    StateMachine.PostEvent(nextEventToSend); // Post event to exit the current state
                }
            }
            catch (CommandExecutionFailedException commandException)
            {
                Logger.Debug(
                    "[{ActivityId}] Error occurred during command execution ({CommandExecutionFailedReason}), ActivityDone will be posted. {@Exception}",
                    Id,
                    commandException.Reason,
                    commandException);

                StateMachine?.PostEvent(
                    ActivityDoneEvent.OnException(commandException, "Error occurred during command execution"));
            }
            catch (Exception ex)
            {
                Logger.Debug(
                    "[{ActivityId}] Error occurred during command execution, ActivityDone will be posted. {@Exception}",
                    Id,
                    ex);

                StateMachine?.PostEvent(ActivityDoneEvent.OnException(ex, "Error occurred during command execution"));
            }
        }

        #endregion Helpers

        #region Overrides

        public override string ToString() => $"Activity '{Id}'";

        #endregion
    }
}
