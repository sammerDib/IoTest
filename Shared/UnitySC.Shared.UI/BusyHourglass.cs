using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace UnitySC.Shared.UI
{
    public static class BusyHourglass
    {
        /// <summary>
        /// A value indicating whether the UI is currently busy
        /// </summary>
        private static bool s_isBusy;

        /// <summary>
        /// Sets the busystate as busy.
        /// </summary>
        public static void SetBusyState()
        {
            if (s_isBusy)
                return;

            s_isBusy = true;
            Mouse.OverrideCursor = Cursors.Wait;
            new DispatcherTimer(TimeSpan.FromSeconds(0), DispatcherPriority.ApplicationIdle, Timer_Tick, Application.Current.Dispatcher);
        }

        /// <summary>
        /// Sets the busystate to not busy
        /// </summary>
        private static void SetIdleState()
        {
            if (!s_isBusy)
                throw new ApplicationException("Not busy");

            s_isBusy = false;
            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Handles the Tick event of the dispatcherTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void Timer_Tick(object sender, EventArgs e)
        {
            var dispatcherTimer = sender as DispatcherTimer;
            if (dispatcherTimer != null)
            {
                SetIdleState();
                dispatcherTimer.Stop();
            }
        }
    }
}
