using System;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace UnitySC.Shared.UI.Helper
{
    public static class TaskHelper
    {
        /// <summary>
        /// Starts the asynchronous task when the system is idle. Once the action is complete, go back to the UI thread and invoke the synchronous method.
        /// </summary>
        public static void DoAsyncOnSystemIdle(Action asyncAction, Action syncAction)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, (Action)(() =>
            {
                Task.Factory.StartNew(() =>
                {
                    asyncAction();

                    System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, syncAction);
                });
            }));
        }
    }
}
