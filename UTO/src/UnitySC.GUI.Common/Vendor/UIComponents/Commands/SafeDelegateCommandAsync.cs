using System;
using System.Threading.Tasks;
using System.Windows.Input;

using Agileo.Common.Logging;
using Agileo.GUI.Commands;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Commands
{
    /// <summary>
    /// Provides an implementation of <see cref="ICommand"/> that is exception-safe.
    /// Works with asynchronous commands.
    /// </summary>
    /// <seealso cref="DelegateCommand"/>
    public class SafeDelegateCommandAsync : DelegateCommand
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeDelegateCommandAsync" /> class.
        /// </summary>
        /// <param name="executeMethod">The asynchronous method to invoke when <see cref="ICommand.Execute" /> is called.</param>
        public SafeDelegateCommandAsync(Func<Task> executeMethod)
            : this(executeMethod, () => true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeDelegateCommandAsync" /> class.
        /// </summary>
        /// <param name="executeMethod">The asynchronous method to invoke when <see cref="ICommand.Execute" /> is called.</param>
        /// <param name="canExecuteMethod">The <see cref="Func{TResult}" /> to invoke when <see cref="ICommand.CanExecute" /> is called.</param>
        public SafeDelegateCommandAsync(Func<Task> executeMethod, Func<bool> canExecuteMethod)
            : base(async () => await Execute(executeMethod), () => CanExecute(canExecuteMethod))
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
                Logger.GetLogger(nameof(SafeDelegateCommandAsync))
                    .Debug($"CanExecute check failed. {e.Message}", e.ToString());
                return false;
            }
        }

        private static async Task Execute(Func<Task> executeMethod)
        {
            try
            {
                await executeMethod();
            }
            catch (Exception e)
            {
                Logger.GetLogger(nameof(SafeDelegateCommandAsync))
                    .Debug($"{nameof(SafeDelegateCommandAsync)} execution failed. {e.Message}", e.ToString());
            }
        }

        #endregion Methods
    }
}
