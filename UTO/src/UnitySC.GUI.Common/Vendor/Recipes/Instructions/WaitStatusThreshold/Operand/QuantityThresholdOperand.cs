using System;
using System.Globalization;
using System.Xml.Serialization;

using Agileo.EquipmentModeling;

using UnitsNet;

namespace UnitySC.GUI.Common.Vendor.Recipes.Instructions.WaitStatusThreshold.Operand
{
    [Serializable]
    public class QuantityThresholdOperand : ThresholdOperand
    {
        public QuantityThresholdOperand() : base(null, string.Empty) { }

        public QuantityThresholdOperand(DeviceStatus status, string deviceName) : base(status, deviceName)
        {
        }

        [XmlElement(nameof(Value))]
        public string Value { get; set; }

        [XmlIgnore]
        public IQuantity Quantity
        {
            get
            {
                QuantityType qt;
                Enum.TryParse(Type, out qt);
                QuantityInfo qi = UnitsNet.Quantity.GetInfo(qt);
                Type unit = qi.ValueType;
                return UnitsNet.Quantity.Parse(CultureInfo.InvariantCulture, unit, Value);
            }
            set
            {
                Value = value.ToString(CultureInfo.InvariantCulture);
                Type = value.Type.ToString();
            }
        }

        public override string ValueAsString()
        {
            return Quantity.ToString();
        }

        public override bool Equals(Threshold other)
        {
            var thresholdValue = other as QuantityThresholdOperand;

            if (ReferenceEquals(thresholdValue, this)) return true;
            if (thresholdValue == null) return false;

            if (thresholdValue.StatusName != StatusName
               || thresholdValue.WaitingOperator != WaitingOperator
               || thresholdValue.DeviceName != DeviceName
               || !thresholdValue.Quantity.Equals(Quantity)
               || thresholdValue.Type != Type
               || thresholdValue.Value != Value) return false;

            return true;
        }

        public override object Clone()
        {
            return new QuantityThresholdOperand
            {
                StatusName = StatusName,
                WaitingOperator = WaitingOperator,
                DeviceName = DeviceName,
                Quantity = Quantity,
                Type = Type,
                Value = Value
            };
        }
    }
}
