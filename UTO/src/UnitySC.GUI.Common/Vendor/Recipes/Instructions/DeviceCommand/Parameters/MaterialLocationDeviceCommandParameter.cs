using System;
using System.Linq;
using System.Xml.Serialization;

using Agileo.EquipmentModeling;

namespace UnitySC.GUI.Common.Vendor.Recipes.Instructions.DeviceCommand.Parameters
{
    [Serializable]
    public class MaterialLocationDeviceCommandParameter : DeviceCommandParameter
    {
        public MaterialLocationDeviceCommandParameter()
        {
        }

        public MaterialLocationDeviceCommandParameter(Parameter parameter) : base(parameter)
        {
        }

        [XmlElement(nameof(MaterialLocationContainerName))]
        public string MaterialLocationContainerName { get; set; }

        [XmlElement(nameof(DeviceInstanceId))]
        public int DeviceInstanceId { get; set; }

        [XmlElement(nameof(Value))]
        public string Value { get; set; }

        public override bool Equals(DeviceCommandParameter other)
        {
            var commandParameter = other as MaterialLocationDeviceCommandParameter;

            return commandParameter != null && Value.Equals(commandParameter.Value, StringComparison.Ordinal);
        }

        public override object Clone()
        {
            return new MaterialLocationDeviceCommandParameter
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
            var materialLocationContainer = App.Instance.EquipmentManager.Equipment
                .AllOfType<IMaterialLocationContainer>()
                .FirstOrDefault(d => d.Name.Equals(MaterialLocationContainerName, StringComparison.Ordinal));

            return materialLocationContainer?.MaterialLocations
                .FirstOrDefault(ml => ml.Name.Equals(Value, StringComparison.Ordinal));
        }
    }
}
