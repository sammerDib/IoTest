using System;
using System.Threading.Tasks;

using Agileo.Common.Logging;

namespace UnitySC.GUI.Common.UIComponents.Commands
{
    public class ContextualSafeDelegateCommandAsync : ContextualDelegateCommand
    {
        public ContextualSafeDelegateCommandAsync(Func<Task> executeMethod)
            : base(async () => await Execute(executeMethod))
        {
        }

        public ContextualSafeDelegateCommandAsync(
            Func<Task> executeMethod,
            Func<Tuple<bool, string>> canExecuteMethod)
            : base(async () => await Execute(executeMethod), canExecuteMethod)
        {
        }

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
                Logger.GetLogger(nameof(ContextualSafeDelegateCommandAsync))
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
                Logger.GetLogger(nameof(ContextualSafeDelegateCommandAsync))
                    .Debug(
                        $"{nameof(ContextualSafeDelegateCommandAsync)} execution failed. {e.Message}",
                        e.ToString());
            }
        }

        #endregion Methods
    }
}
