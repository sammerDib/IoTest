using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;

namespace UnitySC.Shared.Tools
{
    public static class WindowExt
    {
        /// <summary>
        /// Displays the window (and blocks user input on all others) during an async task.
        /// throws exceptions from the task.
        /// </summary>
        public static bool? ShowModalDuringTask(this Window window, Func<Task> createTask)
        {
            //>Exception
            var task = createTask();

            window.Closing += (object sender, CancelEventArgs e) =>
            {
                if (!task.IsCompleted)
                {
                    // Refuse to close the dialog before the task has ended.
                    e.Cancel = true;
                }
            };

            // Automatically close the window upon task completion.
            window.Loaded += async (object sender, RoutedEventArgs e) =>
             {
                 try
                 {
                     //.Exception
                     await task;
                 }
                 catch (Exception) { }// Exception will be forwarded later on.

                 window.Close();
             };

            bool? res = window.ShowDialog();

            // Task is completed, forward exception.
            Func<Task> exceptionForwarder = async () =>
              {
                  //>Exception
                  await task;
              };

            //>Exception
            exceptionForwarder();

            return res;
        }
    }
}