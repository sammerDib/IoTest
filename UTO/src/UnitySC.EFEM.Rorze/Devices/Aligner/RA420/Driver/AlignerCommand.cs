using Agileo.Drivers;

using UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver.PostmanCommands;
using UnitySC.EFEM.Rorze.Drivers.PostmanCommands;
using UnitySC.Equipment.Abstractions.Drivers.Common;

namespace UnitySC.EFEM.Rorze.Devices.Aligner.RA420.Driver
{
    internal class AlignerCommand : DriverCommand
    {
        public static AlignerCommand Initialization { get; } = new(1, nameof(AlignerCommands.Initialization));
        public static AlignerCommand ResetError { get; } = new(2, nameof(ResetErrorCommand));
        public static AlignerCommand GetStatuses { get; } = new(3, nameof(GetStatuses));
        public static AlignerCommand SetSize { get; } = new(4, nameof(SetSubstrateSizeCommand));
        public static AlignerCommand GoHome { get; } = new(5, nameof(HomeCommand));
        public static AlignerCommand Align { get; } = new(6, nameof(AlignCommand));
        public static AlignerCommand CancelSubstrateChuck { get; } = new(7, nameof(CancelSubstrateChuckCommand));
        public static AlignerCommand ChuckSubstrate { get; } = new(8, nameof(ChuckSubstrateCommand));
        public static AlignerCommand SetDateAndTime { get; } = new(9, nameof(SetDateAndTimeCommand));
        public static AlignerCommand InitializeCommunication { get; } = new(10, nameof(InitializeCommunication));

        public AlignerCommand(ushort id, string name) : base(id, name)
        {
        }
    }
}
