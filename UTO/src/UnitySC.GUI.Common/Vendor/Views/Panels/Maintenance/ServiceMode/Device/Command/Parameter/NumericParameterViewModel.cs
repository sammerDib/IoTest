using System;
using System.Globalization;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.ServiceMode.Device.Command.Parameter
{
    public class NumericParameterViewModel : ParameterViewModel
    {
        public NumericParameterViewModel(Agileo.EquipmentModeling.Parameter parameter, DeviceCommandViewModel deviceCommandViewModel, Type type) : base(parameter, deviceCommandViewModel, type)
        {
        }

        public double DoubleValue
        {
            get
            {
                if (Value is IConvertible convertible)
                {
                    return (double)convertible.ToDecimal(CultureInfo.CurrentCulture);
                }
                return double.NaN;
            }
            set
            {
                var convertible = value as IConvertible;
                try
                {
                    Value = convertible.ToType(Type, CultureInfo.CurrentCulture);
                }
                catch
                {
                    if (value > DoubleValue)
                    {
                        var field = Type.GetField("MaxValue");
                        if (field != null && field.IsLiteral && !field.IsInitOnly)
                        {
                            Value = field.GetRawConstantValue();
                        }
                    }
                    if (value < DoubleValue)
                    {
                        var field = Type.GetField("MinValue");
                        if (field != null && field.IsLiteral && !field.IsInitOnly)
                        {
                            Value = field.GetRawConstantValue();
                        }
                    }
                }

                OnPropertyChanged();
            }
        }
    }
}
