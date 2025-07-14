using System;
using System.Xml.Serialization;

using Agileo.EquipmentModeling;

namespace UnitySC.GUI.Common.Vendor.Recipes.Instructions.DeviceCommand.Parameters
{
    [Serializable]
    public class BoolDeviceCommandParameter : DeviceCommandParameter
    {
        public BoolDeviceCommandParameter()
        {
        }

        public BoolDeviceCommandParameter(Parameter parameter) : base(parameter)
        {
        }

        [XmlElement(nameof(Value))]
        public bool Value { get; set; }

        public override bool Equals(DeviceCommandParameter other)
        {
            var instruction = other as BoolDeviceCommandParameter;
            if (instruction == null)
            {
                return false;
            }

            return Value == instruction.Value;
        }

        public override object Clone()
        {
            return new BoolDeviceCommandParameter
            {
                Name = Name,
                Value = Value
            };
        }

        public override string ValueAsString()
        {
            return Value.ToString();
        }

        public override object GetTypedValue()
        {
            return Value;
        }
    }
}
