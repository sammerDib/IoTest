using System;

using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.Vendor.Configuration.DataCollection
{
    public class AxisMinMax : Notifier
    {

        public AxisMinMax(string unitName, string unitAbbreviation, double min, double max)
        {
            if (unitName == null) throw new ArgumentNullException(nameof(unitName));
            if (unitAbbreviation == null) throw new ArgumentNullException(nameof(unitAbbreviation));
            UnitName = unitName;
            UnitAbbreviation = unitAbbreviation;
            Min = min;
            Max = max;
            AxisName = unitName + " (" + unitAbbreviation + ")";
        }

        public string AxisName { get; set; }

        public string UnitName { get; set; }

        public string UnitAbbreviation { get; set; }

        private double _min;

        public double Min
        {
            get { return _min; }
            set
            {
                if (_min.Equals(value)) return;
                _min = value;
                CheckValidity();
                IsMinAuto = double.IsNaN(_min);
                OnPropertyChanged(nameof(Min));
            }
        }

        private bool _isMinAuto;

        public bool IsMinAuto
        {
            get { return _isMinAuto; }
            set
            {
                if (value == _isMinAuto) return;
                _isMinAuto = value;

                // Update Min property
                if (_isMinAuto)
                {
                    Min = double.NaN;
                }
                else
                {
                    Min = IsMaxAuto ? 0 : Max - 100;
                }

                OnPropertyChanged(nameof(IsMinAuto));
            }
        }

        private double _max;

        public double Max
        {
            get { return _max; }
            set
            {
                if (_max.Equals(value)) return;
                _max = value;
                CheckValidity();
                IsMaxAuto = double.IsNaN(_max);
                OnPropertyChanged(nameof(Max));
            }
        }

        private bool _isMaxAuto;

        public bool IsMaxAuto
        {
            get { return _isMaxAuto; }
            set
            {
                if (value == _isMaxAuto) return;
                _isMaxAuto = value;

                // Update Max property
                if (_isMaxAuto)
                {
                    Max = double.NaN;
                }
                else
                {
                    Max = IsMinAuto ? 100 : Min + 100;
                }

                OnPropertyChanged(nameof(IsMaxAuto));
            }
        }

        private bool _isRangeValid;

        public bool IsRangeValid
        {
            get { return _isRangeValid; }
            set
            {
                if (value == _isRangeValid) return;
                _isRangeValid = value;
                OnPropertyChanged(nameof(IsRangeValid));
            }
        }

        private void CheckValidity()
        {
            IsRangeValid = double.IsNaN(Min) || double.IsNaN(Max) || Min < Max;
        }

        public override string ToString()
        {
            return string.Concat(AxisName, ": [", Min, " ; ", Max, "]");
        }

        public void RefreshMinMaxValues()
        {
            OnPropertyChanged(nameof(Min));
            OnPropertyChanged(nameof(Max));
        }
    }
}
