using System;
using System.Linq;
using System.Xml.Serialization;

using Agileo.EquipmentModeling;

using UnitySC.GUI.Common.Vendor.Helpers;

namespace UnitySC.GUI.Common.Vendor.Recipes.Instructions.DeviceCommand.Parameters
{
    [Serializable]
    public class EnumerableDeviceCommandParameter : DeviceCommandParameter
    {
        public EnumerableDeviceCommandParameter()
        {
        }

        public EnumerableDeviceCommandParameter(Parameter parameter)
            : base(parameter)
        {
            if (!(parameter.Type is CSharpType csharpType))
            {
                throw new ArgumentException($"Parameter {parameter} is not an enum type.");
            }

            EnumType = csharpType.FullName;
            AssemblyName = csharpType.PlatformType.Assembly.GetName().Name;
        }

        [XmlElement("AssemblyName")]
        public string AssemblyName { get; set; }

        [XmlElement("EnumType")]
        public string EnumType { get; set; }

        [XmlElement("Value")]
        public string Value { get; set; }

        public override object GetTypedValue()
        {
            return EnumLoader.GetEnumValues(EnumType, AssemblyName)
                       .OfType<IComparable>()
                       .FirstOrDefault(e => e.ToString().Equals(Value, StringComparison.Ordinal));
        }

        public override bool Equals(DeviceCommandParameter other)
        {
            var instruction = other as EnumerableDeviceCommandParameter;
            if (instruction == null)
            {
                return false;
            }

            return Value == instruction.Value && EnumType == instruction.EnumType;
        }

        public override object Clone()
        {
            return new EnumerableDeviceCommandParameter
            {
                Name = Name,
                AssemblyName = AssemblyName,
                EnumType = EnumType,
                Value = Value
            };
        }

        public override string ValueAsString()
        {
            return Value;
        }
    }
}
