using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using Agileo.EquipmentModeling;

using UnitySC.GUI.Common.Vendor.Recipes.Instructions.WaitStatusThreshold.Operators;

namespace UnitySC.GUI.Common.Vendor.Recipes.Instructions.WaitStatusThreshold.Operand
{
    [Serializable]
    public class StringThresholdOperand : ThresholdOperand
    {
        public StringThresholdOperand() : base(null, string.Empty)
        {
        }

        public StringThresholdOperand(DeviceStatus status, string deviceName) : base(status, deviceName)
        {
        }

        [XmlElement(nameof(Value))]
        public string Value { get; set; }

        public override bool Equals(Threshold other)
        {
            var thresholdValue = other as StringThresholdOperand;

            if (ReferenceEquals(thresholdValue, this)) return true;
            if (thresholdValue == null) return false;

            return thresholdValue.StatusName == StatusName
                   && thresholdValue.WaitingOperator == WaitingOperator
                   && thresholdValue.DeviceName == DeviceName
                   && thresholdValue.Type == Type
                   && thresholdValue.Value.Equals(Value);
        }

        protected override List<WaitingOperator> DefineAvailableWaitingOperator()
        {
            return new List<WaitingOperator>
            {
                WaitingOperator.Equals,
                WaitingOperator.NotEquals
            };
        }

        public override object Clone()
        {
            return new StringThresholdOperand
            {
                StatusName = StatusName,
                WaitingOperator = WaitingOperator,
                DeviceName = DeviceName,
                Type = Type,
                Value = Value
            };
        }

        public override string ValueAsString()
        {
            return Value;
        }
    }
}
