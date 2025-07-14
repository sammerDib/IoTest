using System;
using System.Collections.Generic;
using System.IO;

using UnitySC.Shared.Format.ASO;
using UnitySC.Shared.Format.Base;

namespace UnitySC.Shared.Display.ASO
{
    public class AsoExportResult : ExportResultBase<DataAso>
    {
        #region Overrides of ResultExportBase<DataAso>

        protected override List<Tuple<string, string>> ExportThumbnails(DataAso dataObject)
        {
            var thumbnails = new List<Tuple<string, string>>();

            if (dataObject.ClusterList == null) return thumbnails;

            foreach (var clu in dataObject.ClusterList)
            {
                string grayLevelPath = Path.Combine(dataObject.LocalPath, clu.ThumbnailGreyLevelFilePath);
                if (File.Exists(grayLevelPath))
                    thumbnails.Add(new Tuple<string, string>(clu.ThumbnailGreyLevelFilePath, grayLevelPath));

                string binaryPath = Path.Combine(dataObject.LocalPath, clu.ThumbnailBinaryFilePath);
                if (File.Exists(binaryPath))
                    thumbnails.Add(new Tuple<string, string>(clu.ThumbnailBinaryFilePath, binaryPath));
            }

            return thumbnails;
        }

        #endregion
    }
}
