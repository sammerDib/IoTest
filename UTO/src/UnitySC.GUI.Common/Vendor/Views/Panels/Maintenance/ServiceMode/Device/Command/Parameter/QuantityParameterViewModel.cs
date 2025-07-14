using System;
using System.Globalization;

using Agileo.EquipmentModeling;

using UnitsNet;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.ServiceMode.Device.Command.Parameter
{
    public class QuantityParameterViewModel : ParameterViewModel
    {
        private readonly object _initialValue;

        public QuantityParameterViewModel(Agileo.EquipmentModeling.Parameter p, DeviceCommandViewModel deviceCommandViewModel)
            : base(p, deviceCommandViewModel, ((CSharpType)p.Type).PlatformType)
        {
            if (Value == null) return;

            if (Parameter.Unit == null)
            {
                var unitProperty = Value.GetType().GetProperty("Unit");

                if (unitProperty != null)
                {
                    _initialValue = unitProperty.GetValue(Value);

                    Unit = UnitAbbreviationsCache.Default.GetDefaultAbbreviation(unitProperty.PropertyType,
                        Convert.ToInt32(_initialValue), CultureInfo.CurrentUICulture);
                }
            }
            else
            {
                _initialValue = Parameter.Unit;

                Unit = UnitAbbreviationsCache.Default.GetDefaultAbbreviation(Parameter.Unit.GetType(),
                    Convert.ToInt32(Parameter.Unit), CultureInfo.CurrentUICulture);
            }
        }

        public string Unit { get; }

        public double DoubleValue
        {
            get
            {
                var valueProperty = Value.GetType().GetProperty("Value");
                return  valueProperty != null ? (double) valueProperty.GetValue(Value) : default;
            }
            set
            {
                Value = Activator.CreateInstance(Type, value, _initialValue);
                OnPropertyChanged();
            }
        }
    }
}
