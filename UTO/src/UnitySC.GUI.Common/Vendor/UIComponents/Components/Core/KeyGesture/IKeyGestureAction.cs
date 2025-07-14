using System.Windows.Input;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.Core.KeyGesture
{
    public interface IKeyGestureAction
    {
        public bool ExecuteIfMatched(KeyEventArgs keyEventArgs, object parameter);
    }
}
