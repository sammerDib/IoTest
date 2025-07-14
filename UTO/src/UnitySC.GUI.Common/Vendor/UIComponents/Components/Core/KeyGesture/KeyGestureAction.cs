using System;
using System.Linq;
using System.Windows.Input;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.Core.KeyGesture
{
    /// <summary>
    /// Non generic implementation of <see cref="KeyGestureAction"/>.
    /// </summary>
    public class KeyGestureAction : ICommand, IKeyGestureAction
    {
        private readonly Action _executeAction;
        private readonly Func<bool> _canExecuteAction;

        private readonly System.Windows.Input.KeyGesture[] _gestures;

        public KeyGestureAction(Action execute, params System.Windows.Input.KeyGesture[] gestures) : this(execute, () => true, gestures)
        {
        }

        public KeyGestureAction(Action execute, Func<bool> canExecute, params System.Windows.Input.KeyGesture[] gestures)
        {
            _executeAction = execute;
            _canExecuteAction = canExecute;
            _gestures = gestures;
        }

        #region Implementation of ICommand

        public virtual bool CanExecute(object parameter) => _canExecuteAction();

        public virtual void Execute(object parameter) => _executeAction();

        public event EventHandler CanExecuteChanged;

        #endregion

        #region Implementation of IKeyGestureAction

        public virtual bool ExecuteIfMatched(KeyEventArgs keyEventArgs, object parameter)
        {
            if (!Matches(keyEventArgs)) return false;

            if (CanExecute(parameter))
            {
                Execute(parameter);
                return true;
            }

            return false;
        }

        #endregion

        /// <summary>Determines whether all <see cref="System.Windows.Input.KeyGesture" /> matches the input associated with the specified <see cref="KeyEventArgs" /> object.</summary>
        /// <param name="keyEventArgs"></param>
        /// <returns> <see langword="true" /> if the event data matches this <see cref="System.Windows.Input.KeyGesture" />; otherwise, <see langword="false" />.</returns>
        public bool Matches(KeyEventArgs keyEventArgs) => _gestures.All(gesture => gesture.Matches(keyEventArgs.Source, keyEventArgs));
    }

    /// <summary>
    /// Generic implementation of <see cref="KeyGestureAction"/>.
    /// </summary>
    /// <typeparam name="T">Type of parameter</typeparam>
    public class KeyGestureAction<T> : KeyGestureAction where T : class
    {
        private readonly Action<T> _executeAction;
        private readonly Predicate<T> _canExecuteAction;

        public KeyGestureAction(Action<T> execute, params System.Windows.Input.KeyGesture[] gestures) : this(execute, _ => true, gestures)
        {
        }

        public KeyGestureAction(Action<T> execute, Predicate<T> canExecute, params System.Windows.Input.KeyGesture[] gestures) : base(null, null, gestures)
        {
            _executeAction = execute;
            _canExecuteAction = canExecute;
        }

        #region Public Methods

        public bool ExecuteIfMatched(KeyEventArgs keyEventArgs, T parameter) => base.ExecuteIfMatched(keyEventArgs, parameter);

        public override bool CanExecute(object parameter) => _canExecuteAction(parameter as T);

        public override void Execute(object parameter) => _executeAction(parameter as T);

        #endregion
    }
}
