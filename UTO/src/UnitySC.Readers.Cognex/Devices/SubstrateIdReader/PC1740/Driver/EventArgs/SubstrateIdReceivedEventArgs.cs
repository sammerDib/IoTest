namespace UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740.Driver.EventArgs
{
    public class SubstrateIdReceivedEventArgs : System.EventArgs
    {
        public string SubstrateId { get; }

        public bool IsSucceed { get; }

        public SubstrateIdReceivedEventArgs(string substrateId, bool isSucceed)
        {
            SubstrateId = substrateId;
            IsSucceed   = isSucceed;
        }
    }
}
