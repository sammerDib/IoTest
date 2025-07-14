using System.Collections.Generic;

namespace UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740.Driver.EventArgs
{
    public class ImageReceivedEventArgs : System.EventArgs
    {
        public string FilePath { get; }

        public List<string> BitMapDataList { get; }

        public ImageReceivedEventArgs(string filePath, List<string> bitMapDataList)
        {
            FilePath       = filePath;
            BitMapDataList = bitMapDataList;
        }
    }
}
