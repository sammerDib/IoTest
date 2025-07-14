using System;
using System.Collections.Generic;
using System.IO;

using UnitySC.Shared.Format._001;
using UnitySC.Shared.Format.Base;

namespace UnitySC.Shared.Display._001
{
    public class KlarfExportResult : ExportResultBase<DataKlarf>
    {
        #region Overrides of ResultExportBase<DataKlarf>

        protected override List<Tuple<string, string>> ExportThumbnails(DataKlarf dataObject)
        {
            var thumbnails = new List<Tuple<string, string>>();

            string directoryName = Path.GetDirectoryName(dataObject.ResFilePath);

            if (string.IsNullOrWhiteSpace(directoryName)) return thumbnails;

            string multitiffpath = Path.Combine(directoryName, dataObject.TiffFileName);
            if (File.Exists(multitiffpath))
            {
                thumbnails.Add(new Tuple<string, string>(dataObject.TiffFileName, multitiffpath));
            }

            return thumbnails;
        }

        #endregion
    }
}
