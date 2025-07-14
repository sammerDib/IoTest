using System;

namespace UnitySC.Equipment.Abstractions.Vendor.Communication
{
    public class CommandStepChangedEventArgs : EventArgs
    {
        public string Name { get; }
        public ushort Value { get; }

        public CommandStepChangedEventArgs(string name, ushort value)
        {
            Name = name;
            Value = value;
        }

        public CommandStepChangedEventArgs(ProtocolizedDriver.CommandStep commandStep)
        {
            Name = commandStep.Name;
            Value = commandStep.Value;
        }
    }
}
