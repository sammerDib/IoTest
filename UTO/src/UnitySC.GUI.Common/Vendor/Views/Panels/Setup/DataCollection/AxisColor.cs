using System.Windows.Media;

using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Setup.DataCollection
{
    public class AxisColor : Notifier
    {
        public AxisColor(string axisName, string quantityType, string quantityAbbreviation, Color color)
        {
            AxisName = axisName;
            QuantityType = quantityType;
            QuantityAbbreviation = quantityAbbreviation;
            Color = color;
        }

        private string _axisName;

        public string AxisName
        {
            get { return _axisName; }
            set
            {
                if (_axisName == value) return;
                _axisName = value;
                OnPropertyChanged(nameof(AxisName));
            }
        }

        private Color _color;

        public Color Color
        {
            get { return _color; }
            set
            {
                if (_color == value) return;
                _color = value;
                OnPropertyChanged(nameof(Color));
            }
        }

        private string _quantityType;

        public string QuantityType
        {
            get { return _quantityType; }
            set
            {
                if (_quantityType == value) return;
                _quantityType = value;
                OnPropertyChanged(nameof(QuantityType));
            }
        }

        private string _quantityAbbreviation;

        public string QuantityAbbreviation
        {
            get { return _quantityAbbreviation; }
            set
            {
                if (_quantityAbbreviation == value) return;
                _quantityAbbreviation = value;
                OnPropertyChanged(nameof(QuantityAbbreviation));
            }
        }
    }
}
