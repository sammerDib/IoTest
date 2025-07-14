using System;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;

using Agileo.EquipmentModeling;

using UnitsNet;

namespace UnitySC.GUI.Common.Vendor.Recipes.Instructions.DeviceCommand.Parameters
{
    [Serializable]
    public class QuantityDeviceCommandParameter : DeviceCommandParameter
    {
        public QuantityDeviceCommandParameter()
        {
        }

        public QuantityDeviceCommandParameter(Parameter parameter) : base(parameter)
        {
            Unit = parameter.Unit;
            UnitType = parameter.Type.Name;
        }

        [XmlElement(nameof(UnitType))]
        public string UnitType { get; set; }

        [XmlElement(nameof(Unit))]
        public object Unit { get; set; }

        [XmlElement(nameof(Value))]
        public string Value { get; set; }

        public override object GetTypedValue()
        {
            QuantityType qt;
            Enum.TryParse(UnitType, out qt);
            var qi = Quantity.GetInfo(qt);
            var unit = qi.ValueType;

            if (Value != null)
            {
                return Quantity.Parse(CultureInfo.InvariantCulture, unit, Value);
            }

            var quantityInfo = Quantity.Infos.Single(i => i.UnitType == Unit.GetType());
            var defaultUnitIndex =
                quantityInfo.UnitInfos.Select(x => x.Name).ToList().IndexOf(Unit.ToString()) + 1;
            var abbreviation =
                UnitAbbreviationsCache.Default.GetDefaultAbbreviation(Unit.GetType(), defaultUnitIndex,
                    CultureInfo.InvariantCulture);
            Value = "0.0" + abbreviation;
            return Quantity.Parse(CultureInfo.InvariantCulture, unit, Value);
        }

        public override bool Equals(DeviceCommandParameter other)
        {
            var commandParameter = other as QuantityDeviceCommandParameter;
            if (commandParameter == null)
            {
                return false;
            }

            return UnitType.Equals(commandParameter.UnitType) && Value == commandParameter.Value;
        }

        public override object Clone()
        {
            return new QuantityDeviceCommandParameter
            {
                Name = Name,
                UnitType = UnitType,
                Value = Value
            };
        }

        public override string ValueAsString()
        {
            return Value;
        }
    }
}
