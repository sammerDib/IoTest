using System.Collections.ObjectModel;
using System.Text;

using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Enums;

namespace UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.EventArgs
{
    /// <summary>
    /// Aims to provide formatted mapping patterns specific for the current device (RORZE RR75x).
    /// </summary>
    public class MappingEventArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MappingEventArgs" /> class.
        /// </summary>
        public MappingEventArgs(Collection<RR75xSlotState> mapping)
        {
            Mapping = mapping;
        }

        /// <summary>
        /// Get the list of slot states from the bottom to the top of the carrier.
        /// </summary>
        public Collection<RR75xSlotState> Mapping { get; protected set; }

        public override string ToString()
        {
            var build = new StringBuilder();
            build.AppendLine(base.ToString());
            build.AppendLine();
            build.AppendLine("Number of slots: " + Mapping.Count);

            build.AppendLine("--- Carrier Top ---");
            for (var i = Mapping.Count - 1; i >= 0; --i)
            {
                // The last list index represents the upper slot.
                build.AppendLine($"{i}: {Mapping[i]}");
            }

            build.AppendLine("--- Carrier Bottom ---");

            return build.ToString();
        }
    }
}
