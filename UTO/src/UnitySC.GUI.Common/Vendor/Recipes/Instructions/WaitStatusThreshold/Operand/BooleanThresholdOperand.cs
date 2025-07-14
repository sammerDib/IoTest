using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using Agileo.EquipmentModeling;

using UnitySC.GUI.Common.Vendor.Recipes.Instructions.WaitStatusThreshold.Operators;

namespace UnitySC.GUI.Common.Vendor.Recipes.Instructions.WaitStatusThreshold.Operand
{
    [Serializable]
    public class BooleanThresholdOperand : ThresholdOperand
    {
        public BooleanThresholdOperand() : base(null, string.Empty) { }

        public BooleanThresholdOperand(DeviceStatus status, string deviceName) : base(status, deviceName)
        {
        }

        [XmlElement(nameof(Value))]
        public bool Value { get; set; }

        public override bool Equals(Threshold other)
        {
            var thresholdValue = other as BooleanThresholdOperand;

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
            return new BooleanThresholdOperand
            {
                StatusName = StatusName,
                WaitingOperator = WaitingOperator,
                DeviceName = DeviceName,
                Type = Type,
                Value = Value
            };
        }

        protected override List<WaitingOperator> DefineAvailableWaitingOperator()
        {
            return new List<WaitingOperator>
            {
                WaitingOperator.Equals
            };
        }

        public override string ValueAsString()
        {
            return Value.ToString();
        }
    }
}
