using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Agileo.Common.Logging;
using Agileo.Drivers;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Drivers.Common.Exceptions;

namespace UnitySC.Equipment.Abstractions.Drivers.Common
{
    /// <summary>
    /// Provides services to manage the execution of a task (program, command) by a <see cref="DriverWrapper"/>.
    /// </summary>
    public class DriverWrapper : IDisposable
    {
        #region Fields

        private readonly IExtendedDeviceDriver _driver;
        private TaskCompletionSource<CommandEventArgs> _driverTaskCompletion;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DriverWrapper"/> class.
        /// </summary>
        /// <param name="driver">Driver instance used to perform tasks.</param>
        /// <param name="logger">Optional logger instance used to trace what is happening.</param>
        /// <exception cref="ArgumentNullException">When <paramref name="driver"/> is null.</exception>
        public DriverWrapper(IExtendedDeviceDriver driver, ILogger logger)
        {
            _driver = driver ?? throw new ArgumentNullException(nameof(driver));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion Constructors

        #region Properties

        protected ILogger Logger { get; }

        public DriverCommand RunningCommand { get; private set; }

        #endregion

        #region Task Execution

        /// <summary>
        /// Interrupt a blocking call on <see cref="RunCommand"/>.
        /// </summary>
        /// <remarks>
        /// This allows to end execution of <see cref="RunCommand"/> method and release its caller.
        /// It should be used in abnormal situations, like when a time out occurred.
        /// </remarks>
        public void InterruptTask()
        {
            _driver.ClearCommandsQueue();
            _driverTaskCompletion?.TrySetCanceled();
        }

        /// <summary>
        /// Execute the task on hardware. It is a BLOCKING call.
        /// </summary>
        /// <param name="action">The driver command to run.</param>
        /// <param name="command">The command to run.</param>
        /// <exception cref="Exception">The exception that is thrown when the task is finished abnormally.</exception>
        public void RunCommand(Action action, DriverCommand command)
        {
            _driverTaskCompletion = new TaskCompletionSource<CommandEventArgs>(TaskCreationOptions.RunContinuationsAsynchronously);

            // Perform some basic argument checks here (that can not always be checked with preconditions on device's command)
            // Calling "_driver.IsReadyToAcceptProgram" should be done with preconditions on device's command
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            try
            {
                RunningCommand = command;

                // Register to driver's events that will notify end of task
                _driver.CommandDone += Driver_CommandDone;
                _driver.ErrorOccurred += Driver_ErrorOccurred;
                _driver.CommandInterrupted += Driver_CommandInterrupted;

                // Start the task on hardware
                action();

                // Wait for task completion.
                // Here we block the calling thread, it will be released if one of these occurs:
                // - _driver rejected the task
                // - _driver completed the task
                // - Command is interrupted (timed out or aborted)
                Logger.Debug(
                    "Driver's command {DriverCommand} -> Waiting for completion...",
                    command);

                _driverTaskCompletion.Task.Wait(Timeout.Infinite);
            }
            catch (AggregateException e)
            {
                // Handle cancellation or exception in task.
                // InnerExceptions contains the details, so we throw it to EquipmentModeling framework
                var first = e.InnerExceptions.FirstOrDefault();
                if (first != null)
                {
                    throw first;
                }

                throw new CommandExecutionException(
                    command,
                    "An exception occurred while waiting from driver command completion.");
            }
            finally
            {
                Logger.Debug("Driver's command {DriverCommand} -> Finally", command);
                RunningCommand = DriverCommand.Empty;

                _driver.CommandDone -= Driver_CommandDone;
                _driver.ErrorOccurred -= Driver_ErrorOccurred;
                _driver.CommandInterrupted -= Driver_CommandInterrupted;
                _driverTaskCompletion?.TrySetResult(null);
                _driverTaskCompletion = null;
            }
        }

        #endregion Task Execution

        #region Event Handlers

        private void Driver_CommandDone(object sender, CommandEventArgs e)
        {
            // Command done is not the running one (maybe interrupt)
            if (e.Name != RunningCommand.Name)
            {
                return;
            }

            Logger.Debug(
                "Driver's CommandDone event received. Command={DriverCommand}, Status={CommandStatus}.",
                e.Name,
                e.Status);

            if (e.Status != CommandStatusCode.Ok && ConfirmError(e))
            {
                // Go out KO (throw an exception)
                _driverTaskCompletion?.TrySetException(
                    new CommandExecutionException(RunningCommand, e.Error?.ToString()));
            }
            else
            {
                // Go out OK
                _driverTaskCompletion?.TrySetResult(e);
            }
        }

        private void Driver_ErrorOccurred(object sender, ErrorOccurredEventArgs e)
        {
            // Command done is not the running one (maybe interrupt)
            Logger.Debug("ErrorOccurred");
            if (e.CommandInError != RunningCommand.Name)
            {
                return;
            }

            Logger.Warning(
                "Driver's ErrorOccurred event received. Command={DriverCommand}, Error={CommandError}.",
                e.CommandInError,
                e.Error);

            _driverTaskCompletion?.TrySetException(new InvalidOperationException(e.Error.ToString()));
        }

        private void Driver_CommandInterrupted(object sender, System.EventArgs e)
        {
            Logger.Warning(
                "Driver's CommandInterrupted event received. Error will be set before releasing synchronization wrapper.");

            _driverTaskCompletion?.TrySetException(new InvalidOperationException("Transactions have been discarded."));
        }

        #endregion Event Handlers

        #region Protected

        /// <summary>
        /// Determines if the command really ended with error.
        /// Useful when driver reports an error that should be ignored and handled another way.
        /// </summary>
        /// <param name="commandEventArgs"></param>
        /// <returns>
        /// <see langword="true"/> when error is confirmed and wrapper should throw an exception. Otherwise <see langword="true"/> .
        /// </returns>
        protected virtual bool ConfirmError(CommandEventArgs commandEventArgs)
        {
            return true;
        }

        #endregion

        #region IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///  Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> when called from <see cref="Dispose(bool)"/>;<c>false</c> when called from destructor.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                InterruptTask();
            }
        }

        #endregion IDisposable
    }
}
