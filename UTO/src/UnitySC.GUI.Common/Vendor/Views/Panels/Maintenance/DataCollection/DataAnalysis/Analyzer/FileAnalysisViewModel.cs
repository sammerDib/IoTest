using System;
using System.IO;

using Agileo.DataMonitoring.DataWriter.File;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.DataCollection.DataAnalysis.Analyzer
{
    /// <summary>
    /// Regroup data about a file that might be analyzed.
    /// </summary>
    public class FileAnalysisViewModel
    {
        private readonly FileDataReader _fileDataReader;

        #region Constructors

        /// <summary>
        /// Stores data to be kept and generate some missing data.
        /// </summary>
        /// <param name="relatedDataCollectionPlanName">The name of the object that created the file.</param>
        /// <param name="filePath">The path to access the file.</param>
        /// <param name="fileDataReader"><see cref="FileDataReader"/> that must be used to read data in created file</param>
        public FileAnalysisViewModel(string relatedDataCollectionPlanName, string filePath, FileDataReader fileDataReader)
        {
            if (!File.Exists(filePath))
            {
                throw new InvalidOperationException($"{nameof(FileAnalysisViewModel)}: the given file ({filePath}) does not exist.");
            }

            FilePath = filePath;
            var fileInfo = new FileInfo(filePath);

            _fileDataReader = fileDataReader;

            //Initialize properties
            RelatedDataCollectionPlanName = relatedDataCollectionPlanName;
            FileName = fileInfo.Name;
            Extension = fileInfo.Extension.ToUpper();
            StartDate = fileInfo.CreationTime;
            StopDate = fileInfo.LastWriteTime;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Get the name of the object that created the file.
        /// </summary>
        public string RelatedDataCollectionPlanName { get; }

        /// <summary>
        /// Get the name of the file.
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Get the file extension.
        /// </summary>
        public string Extension { get; }

        /// <summary>
        /// Get the date of the collect beginning.
        /// </summary>
        public DateTime StartDate { get; }

        /// <summary>
        /// Get the date of the collect ending.
        /// </summary>
        public DateTime StopDate { get; }

        /// <summary>
        /// Get the path to access the file.
        /// </summary>
        public string FilePath { get; }

        #endregion Properties

        public FileDataReader GetDataReader()
        {
            return _fileDataReader;
        }
    }
}
