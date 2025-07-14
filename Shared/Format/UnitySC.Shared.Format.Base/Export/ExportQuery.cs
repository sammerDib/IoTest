using System.Collections.Generic;
using System.Drawing;

namespace UnitySC.Shared.Format.Base.Export
{
    public class ExportQuery
    {
        public string FilePath { get; set; }

        public bool SaveAsZip { get; set; }

        public bool SaveResultFile { get; set; }

        public bool SaveThumbnails { get; set; }

        public Bitmap Snapshot { get; set; }

        public List<string> AdditionalExports { get; set; }
    }
}
