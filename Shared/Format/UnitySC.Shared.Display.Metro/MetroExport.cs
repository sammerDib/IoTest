using System;
using System.Collections.Generic;
using System.IO;

using UnitySC.Shared.Format.Base;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.NanoTopo;
using UnitySC.Shared.Format.Metro.Topography;
using UnitySC.Shared.Format.Metro.TSV;

namespace UnitySC.Shared.Display.Metro
{
    public class MetroExportResult : ExportResultBase<MetroResult>
    {
        public const string CsvExport = "CSV";
        public const string PdfExport = "Report PDF";

        #region Overrides of ResultExportBase<MetroResult>

        protected override List<Tuple<string, string>> ExportThumbnails(MetroResult dataObject)
        {
            var thumbnails = new List<Tuple<string, string>>();

            if (dataObject.MeasureResult != null)
            {
                switch (dataObject.ResType)
                {
                    case Data.Enum.ResultType.ANALYSE_TSV:
                    case Data.Enum.ResultType.ANALYSE_NanoTopo:
                    case Data.Enum.ResultType.ANALYSE_Topography:
                        break;
                    default:
                        return thumbnails;
                }

                var points = dataObject.MeasureResult.GetAllPoints();
                string rootPath = Path.GetDirectoryName(dataObject.ResFilePath);

                if (string.IsNullOrWhiteSpace(rootPath)) return thumbnails;

                foreach (var point in points)
                {
                    switch (point)
                    {
                        case TSVPointResult tsvPointResult:
                            {
                                foreach (var data in tsvPointResult.Datas)
                                {
                                    if (data is TSVPointData tsvData)
                                    {
                                        if (!string.IsNullOrWhiteSpace(tsvData.ResultImageFileName))
                                        {
                                            string fileName = tsvData.ResultImageFileName;
                                            thumbnails.Add(new Tuple<string, string>(fileName, Path.Combine(rootPath, tsvData.ResultImageFileName)));
                                        }
                                    }
                                }

                                break;
                            }
                        case NanoTopoPointResult nanoTopoPointResult:
                            {
                                foreach (var data in nanoTopoPointResult.Datas)
                                {
                                    if (data is NanoTopoPointData nanotopoData)
                                    {
                                        if (!string.IsNullOrWhiteSpace(nanotopoData.ResultImageFileName))
                                        {
                                            string fileName = nanotopoData.ResultImageFileName;
                                            thumbnails.Add(new Tuple<string, string>(fileName, Path.Combine(rootPath, nanotopoData.ResultImageFileName)));
                                        }
                                    }
                                }

                                break;
                            }
                        case TopographyPointResult topographyPointResult:
                            {
                                foreach (var data in topographyPointResult.Datas)
                                {
                                    if (data is TopographyPointData topographyData)
                                    {
                                        if (!string.IsNullOrWhiteSpace(topographyData.ResultImageFileName))
                                        {
                                            string fileName = topographyData.ResultImageFileName;
                                            thumbnails.Add(new Tuple<string, string>(fileName, Path.Combine(rootPath, topographyData.ResultImageFileName)));
                                        }
                                    }
                                }

                                break;
                            }
                    }
                }
            }

            return thumbnails;
        }

        protected override List<ExportCustomData> ExportCustomFile(string resultName, string exportName, MetroResult dataObject)
        {
            var customData = new List<ExportCustomData>();

            switch (exportName)
            {
                case CsvExport:
                    byte[] csv = dataObject.MeasureResult.ToCsv();
                    customData.Add(new ExportCustomData() { Name = $"{resultName}_Data.csv", FileContent = csv });
                    break;
                case PdfExport:
                    customData.AddRange(ExportPDFReports(dataObject));
                    break;
                default:
                    throw new NotImplementedException($"Export {exportName} not supported by {nameof(MetroExportResult)}");
            }
            return customData;
        }
        #endregion

        protected List<ExportCustomData> ExportPDFReports(MetroResult dataObject)
        {
            var reports = new List<ExportCustomData>();

            if (dataObject.MeasureResult != null)
            {
                switch (dataObject.ResType)
                {
                    case Data.Enum.ResultType.ANALYSE_NanoTopo:
                    case Data.Enum.ResultType.ANALYSE_Topography:
                        break;
                    default:
                        return reports;
                }

                var points = dataObject.MeasureResult.GetAllPoints();
                string rootPath = Path.GetDirectoryName(dataObject.ResFilePath);

                if (string.IsNullOrWhiteSpace(rootPath)) return reports;

                foreach (var point in points)
                {
                    switch (point)
                    {
                        case NanoTopoPointResult nanoTopoPointResult:
                            {
                                foreach (var data in nanoTopoPointResult.Datas)
                                {
                                    if (data is NanoTopoPointData nanotopoData)
                                    {
                                        if (!string.IsNullOrWhiteSpace(nanotopoData.ReportFileName))
                                        {
                                            string fileName = Path.GetFileName(nanotopoData.ReportFileName);
                                            string pathfile = Path.GetDirectoryName(nanotopoData.ReportFileName) + @"\";
                                            reports.Add(new ExportCustomData() { Name = fileName, Path = pathfile, FilePath = Path.Combine(rootPath, nanotopoData.ReportFileName) });
                                        }
                                    }
                                }

                                break;
                            }
                        case TopographyPointResult topographyPointResult:
                            {
                                foreach (var data in topographyPointResult.Datas)
                                {
                                    if (data is TopographyPointData topographyData)
                                    {
                                        if (!string.IsNullOrWhiteSpace(topographyData.ReportFileName))
                                        {
                                            string fileName = Path.GetFileName(topographyData.ReportFileName);
                                            string Pathfile = Path.GetDirectoryName(topographyData.ReportFileName) + @"\";
                                            reports.Add(new ExportCustomData() { Name = fileName, Path = Pathfile, FilePath = Path.Combine(rootPath, topographyData.ReportFileName) });
                                        }
                                    }
                                }
                                break;
                            }
                    }
                }
            }

            return reports;
        }
    }
}
