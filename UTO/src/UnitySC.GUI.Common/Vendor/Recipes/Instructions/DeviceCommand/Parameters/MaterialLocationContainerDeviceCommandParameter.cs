using System;
using System.Linq;
using System.Xml.Serialization;

using Agileo.EquipmentModeling;

namespace UnitySC.GUI.Common.Vendor.Recipes.Instructions.DeviceCommand.Parameters
{
    [Serializable]
    public class MaterialLocationContainerDeviceCommandParameter : DeviceCommandParameter
    {
        public MaterialLocationContainerDeviceCommandParameter()
        {
        }

        public MaterialLocationContainerDeviceCommandParameter(Parameter parameter) : base(parameter)
        {
        }

        [XmlElement("Value")]
        public string Value { get; set; }

        public override object GetTypedValue()
        {
            return App.Instance.EquipmentManager.Equipment
                .AllOfType<IMaterialLocationContainer>()
                .FirstOrDefault(d => d.Name.Equals(Value, StringComparison.Ordinal));
        }

        public override bool Equals(DeviceCommandParameter other)
        {
            var commandParameter = other as MaterialLocationContainerDeviceCommandParameter;
            return commandParameter != null && string.Equals(Value, commandParameter.Value, StringComparison.Ordinal);
        }

        public override object Clone()
        {
            return new MaterialLocationContainerDeviceCommandParameter
            {
                Name = Name,
                Value = Value
            };
        }

        public override string ValueAsString()
        {
            return Value;
        }
    }
}
