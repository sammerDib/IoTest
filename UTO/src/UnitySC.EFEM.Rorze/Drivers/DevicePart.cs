using System.Collections.Generic;

namespace UnitySC.EFEM.Rorze.Drivers
{
    /// <summary>
    /// Contain all data about a device part.
    /// A device part could be a data table or a motion axis.
    /// </summary>
    public class DevicePart
    {
        public DevicePart(string abbreviation, IEnumerable<string> applicableSubCommands)
        {
            Abbreviation          = abbreviation;
            ApplicableSubCommands = new List<string>(applicableSubCommands);
        }

        public string Abbreviation { get; }

        public IReadOnlyList<string> ApplicableSubCommands { get; }
    }
}
