using System;

using AutoMapper;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.UI.ViewModel
{
    public class LengthVM : ObservableObject
    {
        public LengthVM(Length length, LengthUnit displayUnit)
        {
            _value = length.ToUnit(displayUnit).Value;
            _unit = displayUnit;
        }

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

        public override string ToString()
        {
            return Value.ToString();
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return Value.ToString(format, formatProvider);
        }

        public string ToString(string format)
        {
            return Value.ToString(format);
        }
    }

    public static class LengthExtensions
    {
        public static LengthVM ToVM(this Length length)
        {
            return new LengthVM(length);
        }

        public static LengthVM ToVM(this Length length, LengthUnit unit)
        {
            return new LengthVM(length, unit);
        }
    }

    public class LengthToVMConverter : ITypeConverter<Length, LengthVM>
    {
        public LengthVM Convert(Length source, LengthVM destination, ResolutionContext context)
        {
            return source?.ToVM();
        }
    }

    public class VMToLengthConverter : ITypeConverter<LengthVM, Length>
    {
        public Length Convert(LengthVM source, Length destination, ResolutionContext context)
        {
            return source?.Length;
        }
    }
}
