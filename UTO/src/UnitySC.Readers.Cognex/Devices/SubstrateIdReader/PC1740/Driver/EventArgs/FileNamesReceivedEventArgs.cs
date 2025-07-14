using System.Collections.Generic;

namespace UnitySC.Readers.Cognex.Devices.SubstrateIdReader.PC1740.Driver.EventArgs
{
    public class FileNamesReceivedEventArgs : System.EventArgs
    {
        public List<string> Recipes { get; }

        public FileNamesReceivedEventArgs(IEnumerable<string> fileNames)
        {
            Recipes = new List<string>(fileNames);
        }
    }
}
