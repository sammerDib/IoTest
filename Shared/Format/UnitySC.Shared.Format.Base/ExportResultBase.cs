using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Format.Base.Export;

namespace UnitySC.Shared.Format.Base
{
    public class ExportResultBase<T> : IExportResult where T : IResultDataObject
    {
        #region Implementation of IResultExport

        public ExportResult Export(ExportQuery exportQuery, IResultDataObject dataObject)
        {
            if (dataObject is T typedDataObject)
            {
                return Export(exportQuery, typedDataObject);
            }

            throw new InvalidOperationException($"Parameter {nameof(dataObject)} is not a {typeof(T)} instance.");
        }

        #endregion

        public virtual ExportResult Export(ExportQuery exportQuery, T dataObject)
        {
            if (exportQuery == null) throw new ArgumentNullException(nameof(exportQuery));
            if (dataObject == null) throw new ArgumentNullException(nameof(dataObject));

            bool dataFileExist = File.Exists(dataObject.ResFilePath);

            string resFileName = Path.GetFileName(dataObject.ResFilePath);
            string exportFilePath = exportQuery.FilePath;

            var exportResult = new ExportResult();

            if (exportQuery.SaveAsZip)
            {
                if (!exportFilePath.EndsWith(".zip")) exportFilePath += ".zip";

                string directoryName = Path.GetDirectoryName(exportFilePath);
                if (string.IsNullOrEmpty(directoryName)) return exportResult;

                if (!dataFileExist)
                    resFileName = $"{Path.GetFileNameWithoutExtension(exportFilePath)}.{dataObject.ResType.GetExt()}";

                Directory.CreateDirectory(directoryName);
                using (var zipToOpen = new FileStream(exportFilePath, FileMode.OpenOrCreate))
                {
                    using (var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                    {
                        // Result File
                        if (exportQuery.SaveResultFile)
                        {
                            if (string.IsNullOrWhiteSpace(dataObject.ResFilePath))
                                throw new InvalidOperationException($"Can not export result file because {nameof(IResultDataObject.ResFilePath)} is null or white space.");

                            if (dataFileExist)
                                archive.CreateEntryFromFile(dataObject.ResFilePath, resFileName);
                            else
                            {
                                // cas in memory run recipe ANALYSE for exemple

                                string tmpPath = @"C:\temp\";
                                Directory.CreateDirectory(tmpPath);
                                tmpPath += DateTime.Now.ToString("exp_yyyyMMdd_HHmmss_fff.") + dataObject.ResType.GetExt();
                                if (!dataObject.WriteInFile(tmpPath, out string error))
                                    throw new Exception($"Can not Serialize Export temporary result file <{tmpPath}>.\n{error}");

                                archive.CreateEntryFromFile(tmpPath, resFileName);

                                File.Delete(tmpPath);
                            }
                        }

                        // Thumbnails
                        if (exportQuery.SaveThumbnails)
                        {
                            var thumbnailsPath = ExportThumbnails(dataObject);
                            foreach ((string fileName, string filePath) in thumbnailsPath)
                            {
                                if (File.Exists(filePath))
                                {
                                    byte[] bytes = File.ReadAllBytes(filePath);
                                    InsertInZip(archive, bytes, $"{fileName}");
                                }
                            }
                        }

                        // Snapshot
                        if (exportQuery.Snapshot != null)
                        {
                            using (var msSnapStream = new MemoryStream())
                            {
                                exportQuery.Snapshot.Save(msSnapStream, ImageFormat.Png);
                                InsertInZip(archive, msSnapStream.ToArray(), $"{resFileName}_Snapshot.png");
                            }
                        }

                        // AdditionalExports
                        if (exportQuery.AdditionalExports != null)
                        {
                            foreach (string additionalExport in exportQuery.AdditionalExports)
                            {
                                var exportCustomDataList = ExportCustomFile(resFileName, additionalExport, dataObject);
                                foreach (var customData in exportCustomDataList)
                                {
                                    if (customData.FileContent != null)
                                    {
                                        // export via buffer
                                        InsertInZip(archive, customData.FileContent, $"{customData.Path}{customData.Name}");

                                    }
                                    else if (!string.IsNullOrEmpty(customData.FilePath))
                                    {
                                        // export via existing file
                                        if (File.Exists(customData.FilePath))
                                        {
                                            byte[] bytes = File.ReadAllBytes(customData.FilePath);
                                            InsertInZip(archive, bytes, $"{customData.Path}{customData.Name}");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (exportFilePath.EndsWith("\\") == false) exportFilePath += "\\";

                string directoryName = Path.GetDirectoryName(exportFilePath);
                if (string.IsNullOrEmpty(directoryName)) return exportResult;

                if (!dataFileExist)
                    resFileName = $"{Path.GetFileNameWithoutExtension(directoryName)}.{dataObject.ResType.GetExt()}";

                Directory.CreateDirectory(directoryName);

                // Result File
                if (exportQuery.SaveResultFile)
                {
                    if (string.IsNullOrWhiteSpace(dataObject.ResFilePath))
                        throw new InvalidOperationException($"Can not export result file because {nameof(IResultDataObject.ResFilePath)} is null or white space.");

                    if (dataFileExist)
                    { 
                        File.Copy(dataObject.ResFilePath, exportFilePath + resFileName);
                    }
                    else
                    {
                        // cas in memory run recipe ANALYSE for exemple
                        if( ! dataObject.WriteInFile(exportFilePath + resFileName, out string error))
                            throw new Exception($"Can not Serialize export result file <{resFileName}> in <{exportFilePath}>.\n{error}");

                    }
                }

                // Thumbnails
                if (exportQuery.SaveThumbnails)
                {
                    var thumbnailsPath = ExportThumbnails(dataObject);

                    if (thumbnailsPath.Count > 0)
                    {
                        string thumnailsDirectory = $"{exportFilePath}";
                        foreach ((string fileName, string filePath) in thumbnailsPath)
                        {
                            if (File.Exists(filePath))
                            {
                                string thumbsubdir = Path.GetDirectoryName($"{thumnailsDirectory}{fileName}") + @"\";
                                Directory.CreateDirectory(thumbsubdir);
                                // attention fileName peut contenir un chemin relatif d'où l'interet de créer un repertoire à chaque fois
                                File.Copy(filePath, $"{thumnailsDirectory}{fileName}");
                            }
                        }
                    }
                }

                // Snapshot
                exportQuery.Snapshot?.Save($"{exportFilePath}{resFileName}_Snapshot.png", ImageFormat.Png);

                // AdditionalExports
                if (exportQuery.AdditionalExports != null)
                {
                    foreach (string additionalExport in exportQuery.AdditionalExports)
                    {
                        var exportCustomDataList = ExportCustomFile(resFileName, additionalExport, dataObject);
                        foreach (var customData in exportCustomDataList)
                        {
                            if (customData.FileContent != null)
                            {
                                // export via buffer
                                File.WriteAllBytes($"{exportFilePath}{customData.Path}{customData.Name}", customData.FileContent);
                            }
                            else if (!string.IsNullOrEmpty(customData.FilePath))
                            {
                                // export via existing file
                                if (File.Exists(customData.FilePath))
                                {
                                    string subdir = Path.GetDirectoryName($"{exportFilePath}{customData.Path}{customData.Name}") + @"\";
                                    Directory.CreateDirectory(subdir);
                                    // attention peut contenir un chemin relatif d'où l'interet de créer un repertoire à chaque fois
                                    File.Copy(customData.FilePath, $"{exportFilePath}{customData.Path}{customData.Name}");
                                }
                            }
                        }
                    }
                }
            }

            return new ExportResult { CorrectlyDone = true };
        }

        protected virtual List<ExportCustomData> ExportCustomFile(string resultName, string exportName, T dataObject)
        {
            return new List<ExportCustomData>();
        }

        protected virtual List<Tuple<string, string>> ExportThumbnails(T dataObject)
        {
            return new List<Tuple<string, string>>();
        }

        private static void InsertInZip(ZipArchive archive, byte[] fileContent, string filePath)
        {
            // any relative path will be correctly extracted when use "extract all but will not be displayed which can be very confusing
            // => modify relative path by removing.\ from path
            if (filePath.StartsWith(@".\") || filePath.StartsWith(@"./"))
                filePath = filePath.Remove(0, 2);

            var existingentry = archive.GetEntry(filePath);
            if (existingentry != null)
            {
                existingentry.Delete();
            }

            var entry = archive.CreateEntry(filePath);
            using (var entryStream = entry.Open())
            {
                // to keep it as image better to have it as bytes
                entryStream.Write(fileContent, 0, fileContent.Length);
            }
        }
    }
}
