using System.Collections.Generic;
using System.Drawing;

namespace UnitySC.Shared.Format.Base
{
    public interface IResultDisplay
    {
        Bitmap DrawImage(IResultDataObject dataobj, params object[] inprm);

        bool GenerateThumbnailFile(IResultDataObject dataobj, params object[] inprm);

        List<ResultDataStats> GenerateStatisticsValues(IResultDataObject dataobj, params object[] inprm);

        void UpdateInternalDisplaySettingsPrm(params object[] inprm);

        Color GetColorCategory(IResultDataObject dataobj, string sCategoryName);

        IExportResult ExportResult { get; }
    }
}
