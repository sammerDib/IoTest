using Agileo.GUI.Components;
using Agileo.GUI.Services.Icons;

namespace UnitySC.GUI.Common.UIComponents.GuiExtended
{
    public class BadgedIcon : Notifier, IIcon
    {
        private IIcon _icon;

        public IIcon Icon
        {
            get => _icon;
            set => SetAndRaiseIfChanged(ref _icon, value);
        }

        private int _count;

        public int Count
        {
            get => _count;
            set => SetAndRaiseIfChanged(ref _count, value);
        }
    }
}
