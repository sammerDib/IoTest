using System;
using System.Globalization;
using System.Xml.Serialization;

using Agileo.EquipmentModeling;

namespace UnitySC.GUI.Common.Vendor.Recipes.Instructions.DeviceCommand.Parameters
{
    [Serializable]
    public class NumericDeviceCommandParameter : DeviceCommandParameter
    {
        public NumericDeviceCommandParameter()
        {
        }

        public NumericDeviceCommandParameter(Parameter parameter) : base(parameter)
        {
            PlatformType = ((CSharpType)parameter.Type).PlatformType.FullName;
        }

        [XmlElement(nameof(PlatformType))]
        public string PlatformType { get; set; }

        [XmlElement(nameof(Value))]
        public double Value { get; set; }

        public override object GetTypedValue()
        {
            var platformType = Type.GetType(PlatformType);
            if (platformType == null)
            {
                return null;
            }

            return Convert.ChangeType(Value, platformType, CultureInfo.InvariantCulture);
        }

        public override bool Equals(DeviceCommandParameter other)
        {
            var commandParameter = other as NumericDeviceCommandParameter;
            return commandParameter != null
                   && string.Equals(PlatformType, commandParameter.PlatformType, StringComparison.Ordinal)
                   && Value.Equals(commandParameter.Value);
        }

        public override object Clone()
        {
            return new NumericDeviceCommandParameter
            {
                Name = Name,
                PlatformType = PlatformType,
                Value = Value
            };
        }

        public override string ValueAsString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
