using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace UnitySC.GUI.Common.Vendor.Helpers
{
    public static class DispatcherHelper
    {
        private static Dispatcher UiDispatcher => App.Instance?.Dispatcher;
        
        /// <summary>
        /// Executes an action on the UI thread. If this method is called
        /// from the UI thread, the action is executed immediately. If the
        /// method is called from another thread, the action will be enqueued
        /// on the UI thread's dispatcher and executed asynchronously.
        /// </summary>
        /// <param name="action">The action that will be executed on the UI
        /// thread.</param>
        public static void DoInUiThreadAsynchronously(Action action)
        {
            if (IsInDesignMode)
            {
                action();
                return;
            }

            var dispatcher = UiDispatcher;

            if (dispatcher == null || !dispatcher.Thread.IsAlive)
            {
                return;
            }

            dispatcher.BeginInvoke(action);
        }

        /// <summary>
        /// Executes an action on the UI thread. If this method is called
        /// from the UI thread, the action is executed immediately. If the
        /// method is called from another thread, the action will be executed
        /// on the UI thread's dispatcher synchronously.
        /// </summary>
        /// <param name="action">The action that will be executed on the UI
        /// thread.</param>
        public static void DoInUiThread(Action action)
        {
            if (IsInDesignMode)
            {
                action();
                return;
            }

            var dispatcher = UiDispatcher;

            if (dispatcher == null || !dispatcher.Thread.IsAlive)
            {
                return;
            }

            if (dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                dispatcher.Invoke(action);
            }
        }

        public static T DoInUiThread<T>(Func<T> func)
        {
            if (IsInDesignMode)
            {
                return func();
            }

            var dispatcher = UiDispatcher;

            if (dispatcher == null || !dispatcher.Thread.IsAlive)
            {
                return default;
            }

            return dispatcher.CheckAccess() ? func() : dispatcher.Invoke(func);
        }

        public static void DoAfter(int seconds, Action action)
        {
            Task.Delay(new TimeSpan(0, 0, seconds)).ContinueWith(o =>
            {
                action();
            });
        }

        /// <summary>
        /// Indicates if the instance is running in WPF designer.
        /// </summary>
        public static bool IsInDesignMode => App.IsInDesignMode;

        public static void ThrowExceptionIfIsNotInDesignMode([CallerMemberName] string callerName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            if (!IsInDesignMode)
            {
                throw new InvalidOperationException($"The member {callerName} is only usable on design mode. In file {filePath} at line {lineNumber}");
            }
        }
    }
}
