using System.Windows.Input;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.Core.KeyGesture
{
    public interface IKeyGestureHandler
    {
        bool OnKeyDown(KeyEventArgs keyEventArgs);
    }
}
