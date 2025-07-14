using System;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps
{
    public class LengthVM : ObservableObject
    {
        public LengthVM(double value, LengthUnit unit)
        {
            _value = value;
            _unit = unit;
        }

        public LengthVM(Length length)
        {
            _value = length.Value;
            _unit = length.Unit;
        }

        public Length Length
        {
            get => new Length(_value, _unit);
            set
            {
                Value = value.Value;
                Unit = value.Unit;
                OnPropertyChanged();
            }
        }

        private double _value = 0;

        public double Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Length));
                }
            }
        }

        private LengthUnit _unit = LengthUnit.Meter;

        public LengthUnit Unit
        {
            get => _unit;
            set
            {
                if (_unit != value)
                {
                    Value = Length.ToUnit(value).Value;

                    _unit = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(UnitSymbol));
                    OnPropertyChanged(nameof(Length));
                }
            }
        }

        public string UnitSymbol => Length.UnitSymbol;
    }
}
