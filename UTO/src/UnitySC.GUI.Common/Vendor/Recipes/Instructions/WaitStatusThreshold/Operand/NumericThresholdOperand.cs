using System;
using System.Xml.Serialization;

using Agileo.EquipmentModeling;

namespace UnitySC.GUI.Common.Vendor.Recipes.Instructions.WaitStatusThreshold.Operand
{
    [Serializable]
    public class NumericThresholdOperand : ThresholdOperand
    {
        public NumericThresholdOperand() : base(null, string.Empty)
        {
        }

        public NumericThresholdOperand(DeviceStatus status, string deviceName) : base(status, deviceName)
        {
        }

        [XmlElement(nameof(Value))]
        public double Value { get; set; }

        public override bool Equals(Threshold other)
        {
            var thresholdValue = other as NumericThresholdOperand;

            if (ReferenceEquals(thresholdValue, this)) return true;
            if (thresholdValue == null) return false;

            return thresholdValue.StatusName == StatusName
                   && thresholdValue.WaitingOperator == WaitingOperator
                   && thresholdValue.DeviceName == DeviceName
                   && thresholdValue.Type == Type
                   && thresholdValue.Value.Equals(Value);
        }

        public override object Clone()
        {
            return new NumericThresholdOperand
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
            return Value.ToString();
        }
    }
}
