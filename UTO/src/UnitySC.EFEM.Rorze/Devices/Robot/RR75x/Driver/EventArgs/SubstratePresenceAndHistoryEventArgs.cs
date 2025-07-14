using Agileo.Drivers;
using Agileo.SemiDefinitions;

namespace UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.EventArgs
{
    /// <summary>
    /// Contains same information as <see cref="SubstratePresenceEventArgs"/>, with additional information about wafer previous location.
    /// </summary>
    public class SubstratePresenceAndHistoryEventArgs : SubstratePresenceEventArgs
    {
        public SubstratePresenceAndHistoryEventArgs(
            SlotState presence,
            TransferLocation location,
            byte port,
            byte slot,
            TransferLocation previousLocation,
            byte previousSlot)
            : base(presence, location, port, slot)
        {
            PreviousLocation = previousLocation;
            PreviousSlot     = previousSlot;
        }

        protected SubstratePresenceAndHistoryEventArgs(SubstratePresenceAndHistoryEventArgs other) : base(other)
        {
            PreviousLocation = other.PreviousLocation;
            PreviousSlot     = other.PreviousSlot;
        }

        public TransferLocation PreviousLocation { get; protected set; }

        public byte PreviousSlot { get; protected set; }

        public override object Clone() => new SubstratePresenceAndHistoryEventArgs(this);
    }
}
