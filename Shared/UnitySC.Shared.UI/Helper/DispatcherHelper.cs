using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace UnitySC.Shared.UI.Helper
{
    public static class DispatcherHelper
    {
        /// <summary>
        /// Starts the asynchronous task when the system is idle. Once the action is complete, go back to the UI thread and invoke the synchronous method.
        /// </summary>
        public static void DoAsyncOnSystemIdle(Action asyncAction, Action syncAction)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, (Action)(() =>
            {
                Task.Factory.StartNew(() =>
                {
                    asyncAction();

                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, syncAction);
                });
            }));
        }

        /// <summary>
        /// Executes an action on the UI thread. If this method is called
        /// from the UI thread, the action is executed immediately. If the
        /// method is called from another thread, the action will be queued
        /// on the UI thread's dispatcher and executed asynchronously.
        /// </summary>
        /// <param name="action">The action that will be executed on the UI
        /// thread.</param>
        public static void CheckBeginInvokeOnUI(Action action)
        {
            if (action == null) return;

            var dispatcher = Application.Current?.Dispatcher;
            if (dispatcher == null)
            {
                throw new InvalidOperationException("Application dispatcher is null");
            }

            if (dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                dispatcher.BeginInvoke(action);
            }
        }
    }
}
