using System;

using Agileo.Common.Logging;

using UnitySC.GUI.Common.Vendor.Helpers;

namespace UnitySC.GUI.Common.UIComponents.Commands
{
    public class ContextualSafeDelegateCommand : ContextualDelegateCommand
    {
        #region Constructors

        public ContextualSafeDelegateCommand(Action executeMethod)
            : base(executeMethod)
        {
        }

        public ContextualSafeDelegateCommand(
            Action executeMethod,
            Func<Tuple<bool, string>> canExecuteMethod)
            : base(executeMethod, canExecuteMethod)
        {
        }

        #endregion

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
                Logger.GetLogger(nameof(ContextualSafeDelegateCommand))
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
                Logger.GetLogger(nameof(ContextualSafeDelegateCommand))
                    .Debug(
                        $"{nameof(ContextualSafeDelegateCommand)} execution failed. {e.Message}",
                        e.ToString());

                App.Instance.UserInterface.Navigation.SelectedBusinessPanel?.Popups.Show(
                    PopupHelper.Error(
                        "An exception occurred during command execution. Please see logs for more details"));
            }
        }

        #endregion Methods
    }
}
