using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.Core.KeyGesture
{
    /// <summary>
    /// Non generic implementation of <see cref="IKeyGestureHandler"/> as a list of <see cref="IKeyGestureAction"/>.
    /// </summary>
    public class KeyGestureActionList : List<IKeyGestureAction>, IKeyGestureHandler
    {
        #region Implementation of IKeyGestureHandler

        public bool OnKeyDown(KeyEventArgs keyEventArgs)
        {
            foreach (var keyGestureAction in this)
            {
                var result = keyGestureAction.ExecuteIfMatched(keyEventArgs, null);
                if (!result) continue;

                keyEventArgs.Handled = true;
                return true;
            }

            return false;
        }

        #endregion
    }

    /// <summary>
    /// Generic implementation of <see cref="IKeyGestureHandler"/> as a list of <see cref="KeyGestureAction{T}"/>.
    /// </summary>
    public class KeyGestureActionList<T> : List<KeyGestureAction<T>>, IKeyGestureHandler where T : class
    {
        private readonly Func<T> _getParameter;

        public KeyGestureActionList(Func<T> getParameter)
        {
            _getParameter = getParameter;
        }

        #region Implementation of IKeyGestureHandler

        public bool OnKeyDown(KeyEventArgs keyEventArgs)
        {
            foreach (var keyGestureAction in this)
            {
                var result = keyGestureAction.ExecuteIfMatched(keyEventArgs, _getParameter());
                if (!result) continue;

                keyEventArgs.Handled = true;
                return true;
            }

            return false;
        }

        #endregion

        /// <summary>
        /// Creates and adds a <see cref="KeyGestureAction{T}"/> to the <see cref="KeyGestureActionList{T}"/>.
        /// </summary>
        public void Add(Action<T> execute, params System.Windows.Input.KeyGesture[] gestures)
        {
            Add(new KeyGestureAction<T>(execute, gestures));
        }

        /// <summary>
        /// Creates and adds a <see cref="KeyGestureAction{T}"/> to the <see cref="KeyGestureActionList{T}"/>.
        /// </summary>
        public void Add(Action<T> execute, Predicate<T> canExecute, params System.Windows.Input.KeyGesture[] gestures)
        {
            Add(new KeyGestureAction<T>(execute, canExecute, gestures));
        }
    }
}
