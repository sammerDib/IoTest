using System;

using Agileo.GUI.Commands;

namespace UnitySC.GUI.Common.UIComponents.Commands
{
    //Only used to have a common class between regular and async implementation
    public abstract class ContextualDelegateCommand : DelegateCommand
    {
        #region Properties

        private readonly Func<Tuple<bool, string>> _canExecuteMethod;

        #endregion

        protected ContextualDelegateCommand(Action executeMethod)
            : base(executeMethod)
        {
        }

        protected ContextualDelegateCommand(Action executeMethod, Func<Tuple<bool, string>> canExecuteMethod)
            : base(executeMethod, () => CanExecuteFunc(canExecuteMethod))
        {
            _canExecuteMethod = canExecuteMethod;
        }

        #region Public Methods

        public string GetPreconditions()
        {
            var result = _canExecuteMethod?.Invoke();
            return result?.Item2;
        }

        #endregion

        #region Private Methods

        private static bool CanExecuteFunc(Func<Tuple<bool, string>> canExecuteFunc)
        {
            var result = canExecuteFunc?.Invoke();
            return result is { Item1: true };
        }

        #endregion
    }
}
