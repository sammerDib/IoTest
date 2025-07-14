using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitySC.Shared.Format.Base
{
    public class ExportCustomData
    {
        // Export Data Name identity
        public string Name { get; set; }
        // Export Data Destination Path
        public string Path { get; set; }
        // Existing File to Copy
        public string FilePath { get; set; }
        // File content buffer in memory
        public byte[] FileContent { get; set; }
    }
}
