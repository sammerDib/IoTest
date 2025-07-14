using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using UnitySC.Shared.Logger;
using UnitySC.Shared.UI.Dialog.ExceptionDialogs;

namespace UnitySC.Shared.UI.ExceptionDialogs
{
    public class ExceptionManager
    {
        private readonly ILogger _logger;

        public ExceptionManager(ILogger logger)
        {
            _logger = logger;
        }

        public void Init()
        {
            // From the main UI dispatcher thread.
            Application.Current.DispatcherUnhandledException += DispatcherOnUnhandledException;
            /// From all threads in the AppDomain.
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
        }

        #region Private methods

        private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
        {
            Application.Current.Dispatcher.Invoke(() =>
                SendReport(unobservedTaskExceptionEventArgs.Exception)
            );
            Environment.Exit(0);
        }

        private void DispatcherOnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs dispatcherUnhandledExceptionEventArgs)
        {
            SendReport(dispatcherUnhandledExceptionEventArgs.Exception);
            Environment.Exit(0);
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            Application.Current.Dispatcher.Invoke(() =>
                SendReport((Exception)unhandledExceptionEventArgs.ExceptionObject)
            );
            Environment.Exit(0);
        }

        public void SendReport(Exception exception, string developerMessage = "")
        {
            _logger.Error(exception, "An unexpected error occurred in the application.");
            var ev = new ExceptionViewer("An unexpected error occurred in the application.", exception);
            ev.ShowDialog();
        }

        #endregion Private methods
    }
}