using System;
using System.Xml.Serialization;

using Agileo.EquipmentModeling;

using Humanizer;

using Newtonsoft.Json;

namespace UnitySC.GUI.Common.Vendor.Recipes.Instructions.DeviceCommand.Parameters
{
    [Serializable]
    [XmlInclude(typeof(BoolDeviceCommandParameter))]
    [XmlInclude(typeof(EnumerableDeviceCommandParameter))]
    [XmlInclude(typeof(MaterialLocationDeviceCommandParameter))]
    [XmlInclude(typeof(MaterialLocationContainerDeviceCommandParameter))]
    [XmlInclude(typeof(NumericDeviceCommandParameter))]
    [XmlInclude(typeof(QuantityDeviceCommandParameter))]
    [XmlInclude(typeof(StringDeviceCommandParameter))]
    public abstract class DeviceCommandParameter : IEquatable<DeviceCommandParameter>, ICloneable
    {
        protected DeviceCommandParameter()
        {
        }

        protected DeviceCommandParameter(Parameter parameter)
        {
            Name = parameter.Name;
        }

        [XmlElement(nameof(Name))]
        public string Name { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public string PrettyLabel => string.Concat(Name.Humanize(), ": ", ValueAsString());

        public abstract bool Equals(DeviceCommandParameter other);

        public abstract object Clone();

        public abstract string ValueAsString();

        public abstract object GetTypedValue();
    }
}
