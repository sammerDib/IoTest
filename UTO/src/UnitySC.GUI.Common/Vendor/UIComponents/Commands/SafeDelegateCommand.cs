using System;
using System.Windows.Input;

using Agileo.Common.Logging;
using Agileo.GUI.Commands;

using UnitySC.GUI.Common.Vendor.Helpers;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Commands
{
    /// <summary>
    /// Provides an implementation of <see cref="ICommand"/> that is exception-safe.
    /// </summary>
    /// <seealso cref="DelegateCommand"/>
    public class SafeDelegateCommand : DelegateCommand
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeDelegateCommand" /> class.
        /// </summary>
        /// <param name="executeMethod">The <see cref="Action" /> to invoke when <see cref="ICommand.Execute" /> is called.</param>
        public SafeDelegateCommand(Action executeMethod)
            : this(executeMethod, () => true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeDelegateCommand" /> class.
        /// </summary>
        /// <param name="executeMethod">The <see cref="Action" /> to invoke when <see cref="ICommand.Execute" /> is called.</param>
        /// <param name="canExecuteMethod">The <see cref="Func{TResult}" /> to invoke when <see cref="ICommand.CanExecute" /> is called.</param>
        public SafeDelegateCommand(Action executeMethod, Func<bool> canExecuteMethod)
            : base(() => Execute(executeMethod), () => CanExecute(canExecuteMethod))
        {
            if (executeMethod == null)
            {
                throw new ArgumentNullException(nameof(executeMethod));
            }

            if (canExecuteMethod == null)
            {
                throw new ArgumentNullException(nameof(canExecuteMethod));
            }
        }

        #endregion Constructors

        #region Methods

        private static bool CanExecute(Func<bool> canExecuteMethod)
        {
            try
            {
                return canExecuteMethod();
            }
            catch (Exception e)
            {
                // Trace here because exception is non-nominal case
                // But user interactions are not recommended because CanExecute might be called periodically.
                Logger.GetLogger(nameof(SafeDelegateCommand))
                    .Debug($"CanExecute check failed. {e.Message}", e.ToString());
                return false;
            }
        }

        private static void Execute(Action executeMethod)
        {
            try
            {
                executeMethod();
            }
            catch (Exception e)
            {
                Logger.GetLogger(nameof(SafeDelegateCommand))
                    .Debug($"{nameof(SafeDelegateCommand)} execution failed. {e.Message}", e.ToString());

                App.Instance.UserInterface.Popups.Show(PopupHelper.Error("An exception occurred during command execution. Please see logs for more details"));
            }
        }

        #endregion Methods
    }
}
