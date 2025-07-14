namespace UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740.Driver
{
    internal static class Constants
    {
        public struct Commands
        {
            public const string ReadSubstrateId = "SM\"READ\"0";
            public const string LoadJob         = "LF";
            public const string SwitchOnline    = "SO";
            public const string GetFileList     = "Get FileList";
            public const string GetImage        = "RB";
        }
    }
}
