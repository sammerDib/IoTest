using System;
using System.Linq;
using System.Windows.Input;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTree.Events
{
    public interface IKeyGestureAction
    {

    }

    public class KeyGestureAction<T> : ICommand, IKeyGestureAction where T : class
    {
        private readonly Action<TreeNode<T>> _executeAction;
        private readonly Predicate<TreeNode<T>> _canExecuteAction;
        
        private readonly KeyGesture[] _gestures;

        public KeyGestureAction(Action<TreeNode<T>> execute, params KeyGesture[] gestures) : this(execute, _ => true, gestures)
        {
        }

        public KeyGestureAction(Action<TreeNode<T>> execute, Predicate<TreeNode<T>> canExecute, params KeyGesture[] gestures)
        {
            _executeAction = execute;
            _canExecuteAction = canExecute;
            _gestures = gestures;
        }

        public bool ExecuteIfMatched(KeyEventArgs keyEventArgs, TreeNode<T> treeElement)
        {
            if (!_gestures.All(gesture => gesture.Matches(keyEventArgs.Source, keyEventArgs))) return false;

            if (CanExecute(treeElement))
            {
                Execute(treeElement);
                return true;
            }

            return false;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecuteAction((TreeNode<T>) parameter);
        }

        public void Execute(object parameter) => _executeAction((TreeNode<T>) parameter);

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
