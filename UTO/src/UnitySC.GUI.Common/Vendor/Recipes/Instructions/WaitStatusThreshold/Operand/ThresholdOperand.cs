using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

using Agileo.EquipmentModeling;

using Humanizer;

using UnitySC.GUI.Common.Vendor.Recipes.Instructions.WaitStatusThreshold.Operators;

namespace UnitySC.GUI.Common.Vendor.Recipes.Instructions.WaitStatusThreshold.Operand
{
    public abstract class ThresholdOperand : Threshold
    {
        protected ThresholdOperand(DeviceStatus status, string deviceName)
        {
            if (status != null)
            {
                //[KQu] cannot set the DeviceName here because 'Container.Name' is incorrect when several instance of device exists.
                //For example, the 'Container.Name' of LoadPort1/LoadPort2 statuses is 'LoadPort'.
                DeviceName = deviceName;
                StatusName = status.Name;
                Type = ((CSharpType)status.Type).FullName;
            }

            AvailableWaitingOperators = InitDefineAvailableWaitingOperator();
            WaitingOperator = AvailableWaitingOperators.FirstOrDefault();
        }

        [XmlIgnore]
        public override string PrettyLabel => string.Concat(HumanizedStatusName, " ", WaitingOperator.ToHumanizedString(), " ", ValueAsString());

        private WaitingOperator _waitingOperator;

        [XmlElement(nameof(WaitingOperator))]
        public WaitingOperator WaitingOperator
        {
            get
            {
                return _waitingOperator;

            }
            set
            {
                if (!AvailableWaitingOperators.Contains(value))
                {
                    throw new InvalidOperationException($"The {nameof(WaitingOperator).Humanize()} is not allowed for a threshold of type {GetType().Name}.");
                }
                _waitingOperator = value;
            }
        }

        [XmlIgnore]
        public List<WaitingOperator> AvailableWaitingOperators { get; }

        protected virtual List<WaitingOperator> DefineAvailableWaitingOperator()
        {
            return Enum.GetValues(typeof(WaitingOperator)).Cast<WaitingOperator>().ToList();
        }

        [XmlElement(nameof(DeviceName))]
        public string DeviceName { get; set; }

        [XmlElement(nameof(Type))]
        public string Type { get; set; }

        [XmlElement(nameof(StatusName))]
        public string StatusName { get; set; }

        [XmlIgnore]
        public string HumanizedStatusName => $"[{DeviceName}] {StatusName?.Humanize()}";

        public abstract string ValueAsString();

        private List<WaitingOperator> InitDefineAvailableWaitingOperator()
        {
            return DefineAvailableWaitingOperator();
        }

    }
}
