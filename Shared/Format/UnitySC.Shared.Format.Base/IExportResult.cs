using UnitySC.Shared.Format.Base.Export;

namespace UnitySC.Shared.Format.Base
{
    public interface IExportResult
    {
        ExportResult Export(ExportQuery exportQuery, IResultDataObject dataObject);
    }
}
