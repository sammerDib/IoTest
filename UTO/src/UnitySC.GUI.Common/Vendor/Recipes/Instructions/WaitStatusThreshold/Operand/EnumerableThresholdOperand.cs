using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

using Agileo.EquipmentModeling;

using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.Recipes.Instructions.WaitStatusThreshold.Operators;

namespace UnitySC.GUI.Common.Vendor.Recipes.Instructions.WaitStatusThreshold.Operand
{
    [Serializable]
    public class EnumerableThresholdOperand : ThresholdOperand
    {
        public EnumerableThresholdOperand() : base(null, string.Empty)
        { }

        public EnumerableThresholdOperand(DeviceStatus status, string deviceName) : base(status, deviceName)
        {
            AssemblyName = (((CSharpType)status.Type).PlatformType.Assembly.ManifestModule.Name).Replace(".dll", "");
            Value = EnumLoader.GetEnumValues(Type, AssemblyName)?.Cast<object>().FirstOrDefault()?.ToString();
        }

        [XmlElement(nameof(Value))]
        public string Value { get; set; }

        [XmlElement("AssemblyName")]
        public string AssemblyName { get; set; }

        protected override List<WaitingOperator> DefineAvailableWaitingOperator()
        {
            return new List<WaitingOperator>
            {
                WaitingOperator.Equals,
                WaitingOperator.NotEquals
            };
        }

        public override bool Equals(Threshold other)
        {
            var thresholdValue = other as EnumerableThresholdOperand;

            if (ReferenceEquals(thresholdValue, this)) return true;
            if (thresholdValue == null) return false;

            if (thresholdValue.StatusName != StatusName
                || thresholdValue.WaitingOperator != WaitingOperator
                || thresholdValue.DeviceName != DeviceName
                || thresholdValue.Type != Type
                || thresholdValue.Value != Value) return false;

            return true;
        }

        public override object Clone()
        {
            return new EnumerableThresholdOperand
            {
                StatusName = StatusName,
                WaitingOperator = WaitingOperator,
                DeviceName = DeviceName,
                Type = Type,
                Value = Value,
                AssemblyName = AssemblyName
            };
        }

        public override string ValueAsString()
        {
            return Value;
        }
    }
}
