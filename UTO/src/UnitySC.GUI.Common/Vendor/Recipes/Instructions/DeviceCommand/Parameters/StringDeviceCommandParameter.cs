using System.Xml.Serialization;

using Agileo.EquipmentModeling;

namespace UnitySC.GUI.Common.Vendor.Recipes.Instructions.DeviceCommand.Parameters
{
    public class StringDeviceCommandParameter : DeviceCommandParameter
    {
        public StringDeviceCommandParameter()
        {
        }

        public StringDeviceCommandParameter(Parameter parameter) : base(parameter)
        {
        }

        [XmlElement(nameof(Value))]
        public string Value { get; set; }

        public override bool Equals(DeviceCommandParameter other)
        {
            var parameterModel = other as StringDeviceCommandParameter;
            if (parameterModel == null)
            {
                return false;
            }

            return Value?.Equals(parameterModel.Value) ?? false;
        }

        public override object Clone()
        {
            return new StringDeviceCommandParameter
            {
                Name = Name,
                Value = Value
            };
        }

        public override string ValueAsString()
        {
            return Value;
        }

        public override object GetTypedValue()
        {
            return Value;
        }
    }
}
