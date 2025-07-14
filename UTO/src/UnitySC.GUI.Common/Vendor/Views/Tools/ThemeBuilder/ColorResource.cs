using System.Windows.Media;

using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.Vendor.Views.Tools.ThemeBuilder
{
    public class ColorResource : Notifier
    {
        private string _resourceKey;

        public string ResourceKey
        {
            get => _resourceKey;
            set => SetAndRaiseIfChanged(ref _resourceKey, value);
        }

        private Color _color;

        public Color Color
        {
            get => _color;
            set => SetAndRaiseIfChanged(ref _color, value);
        }

        public ColorResource(string resourceKey, Color color)
        {
            _resourceKey = resourceKey;
            _color = color;
        }
    }
}
