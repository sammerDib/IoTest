using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;

using Agileo.Common.Logging;
using Agileo.Common.Tracing;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Help.About.SupportRequest
{
    internal class Archive
    {
        private int _currentFileCount;
        private readonly HashSet<string> _files;

        // This dictionary contains all the file/folder correspondence to write inside the ReadMe file.
        private readonly Dictionary<string, string> _correspondenceFileData = new();

        // This dictionary contains all the files to include in the zip file.
        // Key being the original file path and value the destination path inside the zip.
        private readonly Dictionary<string, string> _originalFilePathToZipFilePath = new();

        private readonly string _archiveFolderPath;
        private readonly ILogger _logger;

        public Archive(string archivePath, string archiveName)
        {
            _files = new HashSet<string>();
            _archiveFolderPath = archivePath;
            _logger = Logger.GetLogger(nameof(Archive));

            if (!Directory.Exists(_archiveFolderPath))
            {
                Directory.CreateDirectory(_archiveFolderPath);
            }

            FullPath = Path.Combine(archivePath, archiveName + ".zip");
        }

        /// <summary>
        /// Represents the method that will handle the ProgressChanged event of an Archive.
        /// </summary>
        /// <param name="progress">The progress of the archive operation, represented as a double.</param>
        public delegate void ProgressChangedEventHandler(double progress);

        /// <summary>
        /// Represents the method that will handle the ArchiveCompleted event of the Archive class.
        /// </summary>
        /// <param name="doesArchiveExists">Indicates whether the archive file exists on disk.</param>
        /// <param name="isArchiveComplete">Indicates whether all expected files are contained in the archive.</param>
        public delegate void ArchiveCompletedEventHandler(
            bool doesArchiveExists,
            bool isArchiveComplete);

        /// <summary>Occurs when [progress changed].</summary>
        public event ProgressChangedEventHandler ProgressChanged;

        /// <summary>Occurs when [archive completed].</summary>
        public event ArchiveCompletedEventHandler ArchiveCompleted;

        public string FullPath { get; }

        /// <summary>Gets the progression.</summary>
        /// <value>The progression.</value>
        public double Progress
        {
            get
            {
                if (_files == null)
                {
                    return 0;
                }

                var filesSize = _files.Count;
                if (filesSize > 0)
                {
                    // ReSharper disable once PossibleLossOfFraction
                    return 100d * _currentFileCount / filesSize;
                }

                return 0;
            }
        }

        /// <summary>Adds the folder to archive.</summary>
        /// <param name="folderPath">The folder path.</param>
        public void AddFolderToArchive(string folderPath)
        {
            // Trimming separator char because Path.GetDirectory does not include them at the end of the string
            folderPath = folderPath.TrimEnd(Path.DirectorySeparatorChar);

            //Compute the folder inside the zip in which we'll save all the content
            var folderZipPath = new DirectoryInfo(folderPath).Name;

            int i = 0;
            //Check if any files in the zip already use that folder. If it's the case, add a (X) at the end of the folder name to handle same name folders.
            while (_originalFilePathToZipFilePath.Values.Any(x => x.StartsWith(folderZipPath)))
            {
                i++;
                folderZipPath = new DirectoryInfo(folderPath).Name + $"({i})";
            }

            _correspondenceFileData.Add(folderPath, folderZipPath);

            foreach (var filePath in Directory.EnumerateFiles(
                         folderPath,
                         "*.*",
                         SearchOption.AllDirectories))
            {
                //Get the relative path of the file from folderPath. That same relative path wil be used in the zip.
                var filePathUri = new Uri(Path.GetFullPath(filePath));
                var folderPathUri = new Uri(Path.GetFullPath(folderPath));
                var relativeFilePathFromFolderPath = Uri.UnescapeDataString(
                    folderPathUri.MakeRelativeUri(filePathUri)
                        .ToString()
                        .Replace('/', Path.DirectorySeparatorChar));

                //Uri does not output the relative path in a ".//path/to/file/" way. It outputs it as "folderName//path/to/file".
                //Thus, we need to extract only the relative path to then insert the folderZipPath at the beginning.
                var relativeFilePathFromFolderPathSegments = relativeFilePathFromFolderPath.Split(
                    Path.DirectorySeparatorChar,
                    Path.AltDirectorySeparatorChar);
                string relativePathInZip;
                if (relativeFilePathFromFolderPathSegments.Length > 1)
                {
                    relativeFilePathFromFolderPathSegments[0] = folderZipPath;
                    relativePathInZip =
                        Path.Combine(relativeFilePathFromFolderPathSegments.ToArray());
                }
                else
                {
                    relativePathInZip = Path.Combine(folderZipPath, relativeFilePathFromFolderPathSegments[0]);
                }

                relativePathInZip = Path.GetDirectoryName(relativePathInZip);

                AddFileToArchive(filePath, relativePathInZip, false);
            }
        }
        
        /// <summary>Adds the file to archive.</summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="relativePathInZip">The relative path inside the zip in which this file will be saved in.</param>
        /// <param name="saveInCorrespondenceFile">If true, this action will be written in the correspondence file.</param>
        public void AddFileToArchive(string filePath, string relativePathInZip = "", bool saveInCorrespondenceFile = true)
        {
            //Get the file name
            var fileName = new FileInfo(filePath).Name;
            var fileNameWithoutExtenstion = Path.GetFileNameWithoutExtension(fileName);
            var extension = Path.GetExtension(fileName);

            var zipFilePath = Path.Combine(relativePathInZip, fileName);

            int i = 0;
            //Check if any files with that path already exist. If it's the case, add a (X) at the end of the filename to handle same name files.
            while (_originalFilePathToZipFilePath.ContainsValue(zipFilePath))
            {
                i++;
                zipFilePath = Path.Combine(
                    relativePathInZip,
                    fileNameWithoutExtenstion + $"({i})" + extension);
            }

            //Once the final path in zip is computed, add it into the dictionaries
            if (saveInCorrespondenceFile)
            {
                _correspondenceFileData[filePath] = zipFilePath;
            }
            _originalFilePathToZipFilePath[filePath] = zipFilePath;

            _files.Add(filePath);
        }
        
        /// <summary>Adds the issue description.</summary>
        /// <param name="msg">The MSG.</param>
        public void AddIssueDescription(string msg)
        {
            using var zip = ZipFile.Open(FullPath, ZipArchiveMode.Update);
            var entry =
                zip.CreateEntry(Path.Combine(_archiveFolderPath, "IssueDescription.txt"));
            using var writer = new StreamWriter(entry.Open());
            writer.WriteLine(
                string.IsNullOrWhiteSpace(msg)
                    ? "No description"
                    : msg);
        }

        /// <summary>Adds the Readme.txt file.</summary>
        public void AddReadme()
        {
            using var zip = ZipFile.Open(FullPath, ZipArchiveMode.Update);
            var entry =
                zip.CreateEntry("Readme.txt");
            using var writer = new StreamWriter(entry.Open());
            writer.WriteLine("This file describe how the content of the Support request are generated." + Environment.NewLine + Environment.NewLine +
                             " - Individual files configured to be added are added at the root of the Zip file." + Environment.NewLine +
                             " - For configured folders to be added, a folder with the same name is added at the root of the Zip file, and all files inside the folder are added, event if they are in subfolders." + Environment.NewLine +
                             " - A \"SupportRequest\" folder is added at the root of the Zip file and contain the issue description entered at the time of the Support request creation." + Environment.NewLine +
                             " - A \"SystemInformation\" folder is added at the root of the Zip file and contain information files about the system and the running Equipment controller software (software and dll versions etc...)");

            writer.WriteLine();
            writer.WriteLine();

            writer.WriteLine(
                "Find below the list of files and folders added to the Zip file, with their original paths and where in the Zip they are stored :");

            writer.WriteLine();

            foreach (var data in _correspondenceFileData)
            {
                writer.WriteLine($"{data.Key} => {data.Value}");

                // Add new line
                writer.WriteLine();
            }
        }

        /// <summary>
        /// Add information about the local machine on which the soft is executed.
        /// </summary>
        public void AddSystemInformation()
        {
            var systemInfo = new SystemInformation.SystemInformation();

            using var zip = ZipFile.Open(FullPath, ZipArchiveMode.Update);
            var systemInfoTxt = zip.CreateEntry(
                Path.Combine("SystemInformation", "SystemInformation.txt"));
            using (var writer = new StreamWriter(systemInfoTxt.Open()))
                systemInfo.WriteAsText(writer);

            var systemInfoXml = zip.CreateEntry(
                Path.Combine("SystemInformation", "SystemInformation.xml"));
            using (var writer = new XmlTextWriter(systemInfoXml.Open(), Encoding.UTF8))
                systemInfo.WriteAsXml(writer);
        }

        /// <summary>Compresses this instance.</summary>
        public void Compress()
        {
            var errors = new List<string>();

            foreach (var file in _files)
            {
                try
                {
                    var savedFolder = _originalFilePathToZipFilePath[file];
                    AddFileToZip(FullPath, file, savedFolder);
                }
                catch (Exception ex)
                {
                    errors.Add(
                        $"Failed to add '{file}'.{Environment.NewLine}{ex}{Environment.NewLine}");
                }

                _currentFileCount++;
                ProgressChanged?.Invoke(Progress);
            }

            _currentFileCount = 0;

            if (errors.Any())
            {
                _logger.Error(
                    string.Join(Environment.NewLine, errors).AsAttachment(),
                    "Failed to add one or more file in zip at '{ZipFilePath}'. See attachment for details.",
                    FullPath);
            }

            ArchiveCompleted?.Invoke(File.Exists(FullPath), !errors.Any());
        }

        private void AddFileToZip(string zipFilename, string fileToAdd, string fileFolderOrigin)
        {
            using var zip = ZipFile.Open(zipFilename, ZipArchiveMode.Update);

            try
            {
                zip.CreateEntryFromFile(fileToAdd, fileFolderOrigin, CompressionLevel.Optimal);
            }
            catch
            {
                Thread.Sleep(200);
                File.Copy(fileToAdd, Path.GetTempPath() + Path.GetFileName(fileToAdd));
                zip.CreateEntryFromFile(
                    Path.GetTempPath() + Path.GetFileName(fileToAdd),
                    fileFolderOrigin,
                    CompressionLevel.Optimal);
                File.Delete(Path.GetTempPath() + Path.GetFileName(fileToAdd));
            }
        }
    }
}
