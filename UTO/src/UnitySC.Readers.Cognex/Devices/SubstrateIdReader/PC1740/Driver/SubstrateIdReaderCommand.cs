using UnitySC.Equipment.Abstractions.Drivers.Common;
using UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740.Driver.PostmanCommands;

namespace UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740.Driver
{
    public class SubstrateIdReaderCommand : DriverCommand
    {
        public static SubstrateIdReaderCommand GetFileList { get; } = new(1, nameof(GetFileListCommand));
        public static SubstrateIdReaderCommand LoadJob { get; } = new(2, nameof(LoadJobCommand));
        public static SubstrateIdReaderCommand Read { get; } = new(3, nameof(ReadCommand));
        public static SubstrateIdReaderCommand GetImage { get; } = new(4, nameof(GetImageCommand));


        public SubstrateIdReaderCommand(ushort id, string name) : base(id, name)
        {
        }
    }
}
