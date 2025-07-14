using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

using Agileo.AlarmModeling;
using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Vendor.Devices.Activities;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Alarms;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Alarms.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;

namespace UnitySC.Equipment.Abstractions.Vendor.Devices.GenericDevice
{
    public partial class GenericDevice
    {
        private ActivityManager _activityManager;

        #region Fields

        protected ErrorProvider ErrorProvider;
        private readonly object _lock = new object();

        #endregion

        /// <summary>Gets the activity currently executed by the device</summary>
        /// <returns>
        /// If an activity is in progress, the activity is returned; otherwise null.
        /// </returns>
        public Activity CurrentActivity => _activityManager.Activity;

        /// <summary>
        /// Gets the current command executed by the device If null or empty, no command in progress
        /// </summary>
        public string CurrentCommand { get; protected set; }

        #region GenericDevice command

        protected virtual void InternalInitialize(bool mustForceInit)
        {
            // Clearing all Set alarms
            ClearAllAlarms();
        }

        #endregion GenericDevice commands

        #region Interruptions

        protected override void InternalInterrupt(
            Interruption interruption,
            CommandExecution interruptedExecution)
        {
            switch (interruption.Kind)
            {
                case InterruptionKind.TimeOut:
                    SetState(OperatingModes.Maintenance);
                    break;
                case InterruptionKind.Abort:
                    if (State != OperatingModes.Idle)
                    {
                        SetState(OperatingModes.Maintenance);
                    }

                    break;
            }

            // Abort current activity if running due to command abort or timeout
            AbortActivity();

            base.InternalInterrupt(interruption, interruptedExecution);
        }

        #endregion Interruptions

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_activityManager != null)
                {
                    _activityManager.ActivityStarted -= ActivityManager_ActivityStarted;
                    _activityManager.ActivityDone -= ActivityManager_ActivityDone;
                    _activityManager.Dispose();
                }

                CommandExecutionStateChanged -= GenericDevice_CommandExecutionStateChanged;
                AlarmStateChanged -= GenericDevice_AlarmStateChanged;
            }

            base.Dispose(disposing);
        }

        protected void SetState(OperatingModes state)
        {
            lock (_lock)
            {
                Logger.Debug($"SetState with current state = {State}, previous state = {PreviousState} and new state = {state}");
                if (state != State)
                {
                    PreviousState = State;
                    State = state;
                }
            }
        }

        #region Events

        /// <summary>Indicates that a device activity has been started.</summary>
        public event EventHandler<ActivityEventArgs> ActivityStarted;

        /// <summary>Indicates that a device activity has been finished.</summary>
        public event EventHandler<ActivityEventArgs> ActivityDone;

        #endregion Events

        #region Raising Events

        protected void OnActivityStarted(ActivityEventArgs args)
        {
            try
            {
                ActivityStarted?.Invoke(this, args);
            }
            catch (Exception ex)
            {
                Logger.Error(
                    $"{Name} {nameof(OnActivityStarted)} - Exception occurred during event sending.",
                    ex);
            }
        }

        protected void OnActivityDone(ActivityEventArgs args)
        {
            try
            {
                ActivityDone?.Invoke(this, args);
            }
            catch (Exception ex)
            {
                Logger.Error(
                    $"{Name} {nameof(OnActivityDone)} - Exception occurred during event sending.",
                    ex);
            }
        }

        #endregion Raising Events

        #region Setup

        private void InstanceInitialization()
        {
            // Default configure the instance.
            // Call made from the constructor.
        }

        public override void SetUp(SetupPhase phase)
        {
            switch (phase)
            {
                case SetupPhase.AboutToSetup:
                    SetState(OperatingModes.Maintenance);
                    _activityManager = new ActivityManager(Logger);
                    _activityManager.ActivityStarted += ActivityManager_ActivityStarted;
                    _activityManager.ActivityDone += ActivityManager_ActivityDone;
                    CommandExecutionStateChanged += GenericDevice_CommandExecutionStateChanged;
                    AlarmStateChanged += GenericDevice_AlarmStateChanged;
                    break;
                case SetupPhase.SettingUp:
                    break;
                case SetupPhase.SetupDone:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
            }
        }

        #endregion Setup

        #region Event handlers

        protected virtual void ActivityManager_ActivityStarted(object sender, ActivityEventArgs e)
        {
            // Redirect event from ActivityManager to device event
            OnActivityStarted(e);
        }

        protected virtual void ActivityManager_ActivityDone(object sender, ActivityEventArgs e)
        {
            // Redirect event from ActivityManager to device event
            OnActivityDone(e);
        }

        private void GenericDevice_CommandExecutionStateChanged(
            object sender,
            CommandExecutionEventArgs e)
        {
            HandleCommandExecutionStateChanged(e);
        }

        /// <summary>
        /// Handle the alarmStateChanged event
        /// </summary>
        /// <param name="sender">The Sender of the event</param>
        /// <param name="e">The <see cref="AlarmStateChangedEventArgs"/> arguments of the event</param>
        private void GenericDevice_AlarmStateChanged(object sender, AlarmStateChangedEventArgs e)
        {
            HandleAlarmStateChanged(e);
        }

        protected virtual void HandleCommandExecutionStateChanged(CommandExecutionEventArgs e)
        {
            switch (e.NewState)
            {
                case ExecutionState.Running:
                    CurrentCommand = e.Execution.Context.Command.Name;
                    SetState(
                        e.Execution.Context.Command.Name == nameof(IGenericDevice.Initialize)
                            ? OperatingModes.Initialization
                            : OperatingModes.Executing);
                    break;
                case ExecutionState.Success:
                case ExecutionState.Failed:
                    // The State may already be engineering due to command behavior implementation.
                    // That would be the case where the device received an error from the driver that needs a new init.
                    // Otherwise, the device is fit to execute a new command and therefore go back to Idle or Maintenance state.
                    CurrentCommand = string.Empty;
                    if (e.NewState == ExecutionState.Failed)
                    {
                        SetAlarmById(
                            AlarmsConstants.CommandFailedAlarmId,
                            e.Execution.Context.Command.Name);
                    }

                    switch (State)
                    {
                        case OperatingModes.Maintenance:
                        case OperatingModes.Idle:
                            //Do nothing
                            break;
                        case OperatingModes.Initialization:
                            SetState(
                                e.NewState == ExecutionState.Success
                                    ? OperatingModes.Idle
                                    : OperatingModes.Maintenance);
                            break;
                        case OperatingModes.Executing:
                            SetState(
                                PreviousState == OperatingModes.Maintenance
                                    ? OperatingModes.Maintenance
                                    : OperatingModes.Idle);
                            break;
                    }

                    break;
            }
        }

        /// <summary>
        /// Handle the AlarmStateChanged event
        /// </summary>
        /// <param name="e">The <see cref="AlarmStateChangedEventArgs"/> arguments that contains
        /// information about the alarm that changed state.</param>
        protected virtual void HandleAlarmStateChanged(AlarmStateChangedEventArgs e)
        {
            // If a critical or undefined criticity alarm is set ...
            if (e.State == AlarmState.Set && ErrorProvider?.GetError(e.Occurrence.Alarm.RelativeId).Criticity != AlarmCriticity.NonCritical)
            {
                //Aborts the current activity
                Interrupt(InterruptionKind.Abort);

                //Set the device in maintenance mode
                if (State != OperatingModes.Maintenance)
                {
                    SetState(OperatingModes.Maintenance);
                }
            }
        }

        #endregion Event handlers

        #region Commands on Activity

        /// <summary>Starts the provided activity.</summary>
        /// <param name="activity">The activity.</param>
        /// <returns>
        /// Error code and Error description. If No error, ErrorCode == <see cref="ErrorCode.Ok" />.
        /// </returns>
        protected virtual void StartActivity(Activity activity)
        {
            var errorDesc = _activityManager.Start(activity);
            if (errorDesc.ErrorCode != ErrorCode.Ok)
            {
                throw new InvalidOperationException($"{activity.Id} failed to start: " + errorDesc);
            }
        }

        /// <summary>Starts and waits the provided activity.</summary>
        /// <param name="activity">The activity to run.</param>
        /// <param name="timeout">
        /// The number of milliseconds to wait, or Infinite (-1) to wait indefinitely.
        /// </param>
        /// <returns>
        /// Error code and Error description. If No error, ErrorCode == <see cref="ErrorCode.Ok" />.
        /// </returns>
        protected virtual void RunActivity(Activity activity, int timeout = Timeout.Infinite)
        {
            var errorDesc = _activityManager.Run(activity, timeout);
            if (errorDesc.ErrorCode != ErrorCode.Ok)
            {
                throw new InvalidOperationException($"{activity.Id} failed: " + errorDesc);
            }
        }

        /// <summary>
        /// If an activity is stored but didn't start or isn't removed for any reasons, Free the Activity
        /// </summary>
        public void CancelActivity()
        {
            if (CurrentActivity == null)
            {
                return;
            }

            _activityManager.Remove();
        }

        /// <summary>Aborts the current activity.</summary>
        public void AbortActivity()
        {
            if (CurrentActivity == null)
            {
                return;
            }

            _activityManager.Abort();
        }

        #endregion Commands on Activity

        #region Alarms

        /// <summary>Loads the device alarms from a CSV file.</summary>
        /// <param name="alarmCenter">The alarm center instance.</param>
        /// <param name="relativeConfigurationDirectory">The device relative configuration directory.</param>
        /// <param name="deviceConfigRootPath">The path to root folder containing Devices configuration.</param>
        public virtual void LoadAlarms(
            IAlarmCenter alarmCenter,
            string relativeConfigurationDirectory,
            string deviceConfigRootPath = "")
        {
            if (ErrorProvider != null)
            {
                return;
            }

            if (string.IsNullOrEmpty(deviceConfigRootPath))
            {
                deviceConfigRootPath =
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            }

            var deviceResourcesDir = Path.GetFullPath(
                Path.Combine(deviceConfigRootPath, relativeConfigurationDirectory));
            var csvErrorFilePattern = Path.Combine(
                deviceResourcesDir,
                $"{{0}}_{DeviceType.Name}.csv");
            ErrorProvider = this.LoadAlarms(
                alarmCenter,
                csvErrorFilePattern,
                $"{Name}_ErrorProvider",
                Logger);
        }

        #region Ack Alarm

        public bool AckAllAlarms()
        {
            if (!this.TryGetAlarmCenter(out var alarmCenter))
            {
                return false;
            }

            try
            {
                foreach (var al in Alarms)
                {
                    if (al.State == AlarmState.Set && !al.Acknowledged)
                    {
                        alarmCenter.Services.AcknowledgeAlarm(
                            this,
                            al.Name,
                            AlarmsConstants.ExternalAcknowledge);
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error while acknowledging all alarms");
            }

            return false;
        }

        public bool AckByAlarmId(int alarmId)
        {
            if (!this.TryGetAlarmCenter(out var alarmCenter))
            {
                return false;
            }

            try
            {
                if (alarmId == 0)
                {
                    throw new InvalidDataException("Value for AlarmId can't be = 0");
                }

                var alarmUniqueKey = this.FormatAlarmUniqueKey(alarmId.ToString());
                foreach (var al in Alarms)
                {
                    if (al.Name == alarmUniqueKey && al.State == AlarmState.Set && !al.Acknowledged)
                    {
                        alarmCenter.Services.AcknowledgeAlarm(
                            this,
                            al.Name,
                            AlarmsConstants.ExternalAcknowledge);
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Logger.Error(
                    e,
                    "Error while acknowledging all alarms with ID '{AlarmId}'",
                    alarmId);
            }

            return false;
        }

        #endregion Ack Alarms

        #region Clear Alarms

        public bool ClearAllAlarms()
        {
            var ret = true;
            foreach (var al in Alarms)
            {
                try
                {
                    if (al.State == AlarmState.Set)
                    {
                        ClearAlarm(al.Name);
                    }
                }
                catch (Exception e)
                {
                    ret = false;
                    Logger.Error(
                        e,
                        "Error while clearing all alarms. Current alarm: {AlarmId}",
                        al.Id);
                }
            }

            return ret;
        }

        public bool ClearAllAlarmsById(int alarmId)
        {
            try
            {
                if (alarmId == 0)
                {
                    throw new InvalidDataException("Value for AlarmId can't be = 0");
                }

                var alarmUniqueKey = this.FormatAlarmUniqueKey(alarmId.ToString());
                foreach (var al in Alarms)
                {
                    if (al.State == AlarmState.Set && al.Name == alarmUniqueKey)
                    {
                        ClearAlarm(al.Name);
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error while clearing all alarms with ID '{AlarmId}'", alarmId);
            }

            return false;
        }

        #endregion Clear Alarms

        public void SetAlarmById(string relativeId, params object[] substitutionParams)
        {
            var alarmUniqueKey = this.FormatAlarmUniqueKey(relativeId);
            if (Alarms.Any(a => a.Name == alarmUniqueKey && a.State != AlarmState.Set))
            {
                SetAlarm(alarmUniqueKey, substitutionParams);
            }
        }

        public void ClearAlarmById(string relativeId)
        {
            var alarmUniqueKey = this.FormatAlarmUniqueKey(relativeId);
            if (Alarms.Any(al => al.Name == alarmUniqueKey && al.State == AlarmState.Set))
            {
                ClearAlarm(alarmUniqueKey);
            }
        }

        public void ClearAlarmByKey(string alarmKey)
        {
            if (!this.TryGetAlarmCenter(out var alarmCenter))
            {
                return;
            }

            if (alarmCenter.Repository.GetAlarmOccurrences()
                    .Any(a => a.Alarm.Name.Equals(alarmKey)
                                            && a.State == AlarmState.Set))
            {
                ClearAlarm(alarmKey);
            }
        }
        #endregion

        /// <summary>Set device state to engineering and throw the exception.</summary>
        /// <param name="e">The exception to throw</param>
        protected void MarkExecutionAsFailed(Exception e)
        {
            SetState(OperatingModes.Maintenance);
            OnUserErrorRaised($"Error occurred with following message : {e.Message}. Please check logs or make support request.");
            throw e;
        }

        /// <summary>
        /// Set device state to engineering and throw an <see cref="InvalidOperationException" /> with the
        /// specified message.
        /// </summary>
        /// <param name="message"></param>
        protected void MarkExecutionAsFailed(string message)
        {
            SetState(OperatingModes.Maintenance);
            OnUserErrorRaised($"Error occurred with following message : {message}. Please check logs or make support request.");
            throw new InvalidOperationException(message);
        }

        #region IUserInformationProvider

        public event EventHandler<UserInformationEventArgs> UserInformationRaised;
        protected void OnUserInformationRaised(string message)
        {
            UserInformationRaised?.Invoke(this, new UserInformationEventArgs(message));
        }

        public event EventHandler<UserInformationEventArgs> UserWarningRaised;
        protected void OnUserWarningRaised(string message)
        {
            UserWarningRaised?.Invoke(this, new UserInformationEventArgs(message));
        }

        public event EventHandler<UserInformationEventArgs> UserErrorRaised;
        protected void OnUserErrorRaised(string message)
        {
            UserErrorRaised?.Invoke(this, new UserInformationEventArgs(message));
        }

        #endregion
    }
}
