using System;

namespace UnitySC.Equipment.Abstractions.Drivers.Common
{
    public class DriverCommand : IEquatable<DriverCommand>
    {
        public ushort Id { get; }
        public string Name { get; }

        protected DriverCommand(ushort id, string name)
        {
            Id = id;
            Name = name;
        }

        public bool Equals(DriverCommand other)
        {
            return other.Id.Equals(Id) && other.Name.Equals(Name);
        }

        public override string ToString() => $"{Name}[{Id}]";

        public static DriverCommand Empty => new(default, default);
    }
}
