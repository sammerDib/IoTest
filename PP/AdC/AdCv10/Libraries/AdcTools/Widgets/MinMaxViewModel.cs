using CommunityToolkit.Mvvm.ComponentModel;

namespace AdcTools
{
    ///////////////////////////////////////////////////////////////////////
    // un ViewModel gérénique pour gérer un min et un max
    ///////////////////////////////////////////////////////////////////////
    [System.Reflection.Obfuscation(Exclude = true)]
    public class MinMaxViewModel : ObservableRecipient
    {
        private double _min, _max;
        private bool _hasMin, _hasMax;

        public bool IsNull { get { return !HasMin && !HasMax; } }

        //=================================================================
        // 
        //=================================================================
        public MinMaxViewModel(double min, double max)
        {
            _min = min;
            _max = max;

            _hasMin = !double.IsNegativeInfinity(_min);
            _hasMax = !double.IsPositiveInfinity(_max);
        }

        //=================================================================
        // 
        //=================================================================
        public double Min
        {
            get
            {
                if (_hasMin)
                    return _min;
                else
                    return double.NegativeInfinity;
            }
            set
            {
                _hasMin = !double.IsNegativeInfinity(value);
                if (_hasMin)
                    _min = value;
                CheckValidity();
                OnPropertyChanged(nameof(Min));
            }
        }

        //=================================================================
        // 
        //=================================================================
        public double Max
        {
            get
            {
                if (_hasMax)
                    return _max;
                else
                    return double.PositiveInfinity;
            }
            set
            {
                _hasMax = !double.IsPositiveInfinity(value);
                if (_hasMax)
                    _max = value;
                CheckValidity();
                OnPropertyChanged(nameof(Max));
            }
        }

        //=================================================================
        // 
        //=================================================================
        public bool HasMin
        {
            get
            {
                return _hasMin;
            }
            set
            {
                if (_hasMin == value)
                    return;
                _hasMin = value;

                CheckValidity();
                OnPropertyChanged(nameof(Min));
                OnPropertyChanged(nameof(HasMin));
            }
        }

        //=================================================================
        // 
        //=================================================================
        public bool HasMax
        {
            get
            {
                return _hasMax;
            }
            set
            {
                if (_hasMax == value)
                    return;
                _hasMax = value;

                CheckValidity();
                OnPropertyChanged(nameof(Max));
                OnPropertyChanged(nameof(HasMax));
            }
        }

        //=================================================================
        // 
        //=================================================================
        private bool _isValid = true;
        public bool IsValid
        {
            get
            {
                return _isValid;
            }
            set
            {
                if (_isValid == value)
                    return;
                _isValid = value;
                OnPropertyChanged(nameof(IsValid));
            }
        }

        private void CheckValidity()
        {
            if (HasMin && HasMax)
                IsValid = (Min <= Max);
            else
                IsValid = true;
        }

        //=================================================================
        //
        //=================================================================
        public override string ToString()
        {
            return "[" + Min.ToMathString() + ".." + Max.ToMathString() + "]";
        }

    }
}
