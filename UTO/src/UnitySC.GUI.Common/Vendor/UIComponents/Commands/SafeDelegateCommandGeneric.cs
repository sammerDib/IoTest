using System;
using System.Windows.Input;

using Agileo.Common.Logging;
using Agileo.GUI.Commands;

using UnitySC.GUI.Common.Vendor.Helpers;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Commands
{
    /// <summary>
    /// Provides a generic implementation of <see cref="ICommand"/> that is exception-safe.
    /// </summary>
    /// <seealso cref="DelegateCommand{T}"/>
    public class SafeDelegateCommand<T> : DelegateCommand<T>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeDelegateCommand" /> class.
        /// </summary>
        /// <param name="executeMethod">The <see cref="Action" /> to invoke when <see cref="ICommand.Execute" /> is called.</param>
        public SafeDelegateCommand(Action<T> executeMethod)
            : this(executeMethod, o => true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeDelegateCommand" /> class.
        /// </summary>
        /// <param name="executeMethod">The <see cref="Action" /> to invoke when <see cref="ICommand.Execute" /> is called.</param>
        /// <param name="canExecuteMethod">The <see cref="Func{TResult}" /> to invoke when <see cref="ICommand.CanExecute" /> is called.</param>
        public SafeDelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
            : base(o => Execute(executeMethod, o), o => CanExecute(canExecuteMethod, o))
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

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeDelegateCommand" /> class.
        /// </summary>
        /// <param name="executeMethod">The <see cref="Action" /> to invoke when <see cref="ICommand.Execute" /> is called.</param>
        /// <param name="arg">Argument to pass on every calls of <see cref="ICommand.Execute" />.</param>
        public SafeDelegateCommand(Action<T> executeMethod, T arg)
            : this(executeMethod, o => true, arg)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeDelegateCommand" /> class.
        /// </summary>
        /// <param name="executeMethod">The <see cref="Action" /> to invoke when <see cref="ICommand.Execute" /> is called.</param>
        /// <param name="canExecuteMethod">The <see cref="Func{TResult}" /> to invoke when <see cref="ICommand.CanExecute" /> is called.</param>
        /// <param name="arg">Argument to pass on every calls of <see cref="ICommand.CanExecute" /> and <see cref="ICommand.Execute" />.</param>
        public SafeDelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod, T arg)
            : base(o => Execute(executeMethod, arg), o => CanExecute(canExecuteMethod, arg))
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

        private static bool CanExecute<TArg>(Func<TArg, bool> canExecuteMethod, TArg arg)
        {
            try
            {
                return canExecuteMethod(arg);
            }
            catch (Exception e)
            {
                // Trace here because exception is non-nominal case
                // But user interactions are not recommended because CanExecute might be called periodically.
                Logger.GetLogger(nameof(SafeDelegateCommand<T>))
                    .Debug($"CanExecute check failed. {e.Message}", e.ToString());
                return false;
            }
        }

        private static void Execute<TArg>(Action<TArg> executeMethod, TArg arg)
        {
            try
            {
                executeMethod(arg);
            }
            catch (Exception e)
            {
                Logger.GetLogger(nameof(SafeDelegateCommand<T>))
                    .Debug($"{nameof(SafeDelegateCommand<T>)} execution failed. {e.Message}", e.ToString());

                App.Instance.UserInterface.Popups.Show(PopupHelper.Error("An exception occurred during command execution. Please see logs for more details"));
            }
        }

        #endregion Methods
    }
}
