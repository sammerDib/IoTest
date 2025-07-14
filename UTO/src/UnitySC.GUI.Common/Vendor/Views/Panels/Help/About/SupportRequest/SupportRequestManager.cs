using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

using Agileo.Common.Tracing;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Configuration;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Help.About.SupportRequest
{
    /// <summary>
    /// Manages the creation and sending of support requests, including the archiving of necessary files and handling of progress and completion events.
    /// </summary>
    public class SupportRequestManager : Notifier
    {
        #region Fields

        private readonly BackgroundWorker _bwZipFile = new();

        private Archive _zipWorker;

        private string _archivePath = string.Empty;

        private readonly HashSet<string> _filesToExport = new();

        private readonly HashSet<string> _foldersToExport = new();

        public event EventHandler<ArchiveCompleteEventArgs> ArchiveComplete;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SupportRequestManager"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor sets up the background worker for handling zip file operations,
        /// including progress reporting and completion events.
        /// </remarks>
        public SupportRequestManager()
        {
            _bwZipFile.WorkerReportsProgress = true;
            _bwZipFile.ProgressChanged += OnBwZipFileProgressChanged;
            _bwZipFile.RunWorkerCompleted += OnBwZipFileRunWorkerCompleted;
            _bwZipFile.DoWork += OnBwZipFileDoWork;
        }

        #region Properties

        private bool _isExecuting;

        /// <summary>
        /// Gets a value indicating whether the support request manager is currently executing an operation.
        /// </summary>
        /// <value>
        /// <c>true</c> if the support request manager is executing an operation; otherwise, <c>false</c>.
        /// </value>
        public bool IsExecuting
        {
            get => _isExecuting;
            private set => SetAndRaiseIfChanged(ref _isExecuting, value);
        }

        private string _problemDescription;

        /// <summary>Gets or sets the problem description.</summary>
        /// <value>The problem description.</value>
        public string ProblemDescription
        {
            get => _problemDescription;
            set => SetAndRaiseIfChanged(ref _problemDescription, value);
        }

        private double _zipProgression;

        /// <summary>Gets or sets the zip progression.</summary>
        /// <value>The zip progression.</value>
        public double ZipProgression
        {
            get => _zipProgression;
            private set => SetAndRaiseIfChanged(ref _zipProgression, value);
        }

        #endregion

        #region Commands

        #region SendReportCommand

        private ICommand _sendReportCommand;

        public ICommand SendReportCommand
            => _sendReportCommand
               ??= new DelegateCommand(
                   SendReportCommandExecute,
                   SendReportCommandCanExecute);

        protected bool SendReportCommandCanExecute()
        {
            return !IsExecuting;
        }

        protected void SendReportCommandExecute()
        {
            _bwZipFile.RunWorkerAsync();
        }

        /// <summary>
        /// Method executed synchronously before performing the SendReport actions.
        /// </summary>
        public virtual void OnReportGenerating()
        {
        }

        /// <summary>
        /// Method executed synchronously after performing the SendReport actions.
        /// </summary>
        public virtual void OnReportGenerated()
        {
        }

        #endregion

        #endregion

        #region Private methods

        private void OnBwZipFileDoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                IsExecuting = true;

                OnReportGenerating();

                _zipWorker = new Archive(
                    _archivePath,
                    @"Issue_" + DateTime.Now.ToString("yyyy-MM-dd @ H-mm-ss"));
                foreach (var fileToExport in _filesToExport.Where(File.Exists))
                {
                    _zipWorker.AddFileToArchive(fileToExport);
                }

                foreach (var folderToExport in _foldersToExport.Where(Directory.Exists))
                {
                    _zipWorker.AddFolderToArchive(folderToExport);
                }

                _zipWorker.AddIssueDescription(ProblemDescription);
                _zipWorker.AddReadme();
                _zipWorker.AddSystemInformation();

                _zipWorker.ProgressChanged += ZipProgressChanged;
                _zipWorker.ArchiveCompleted += ZipWorkerArchiveCompleted;

                _zipWorker.Compress();
                
                OnReportGenerated();
            }
            catch (Exception ex)
            {
                var traceParam = new TraceParam { StringAttachment = ex.Message };
                App.Instance.Tracer.Trace(
                    nameof(SupportRequestManager),
                    TraceLevelType.Error,
                    "Failed to send support request",
                    traceParam);

                if (_zipWorker != null)
                {
                    _zipWorker.ProgressChanged -= ZipProgressChanged;
                    _zipWorker.ArchiveCompleted -= ZipWorkerArchiveCompleted;
                }

                ArchiveComplete?.Invoke(
                    this,
                    new ArchiveCompleteEventArgs(ex.Message, false, false, true));
                IsExecuting = false;
            }

            ProblemDescription = string.Empty;
        }

        private void ZipWorkerArchiveCompleted(bool doesArchiveExists, bool isArchiveComplete)
        {
            CompleteArchive(doesArchiveExists, isArchiveComplete);
        }

        private void CompleteArchive(bool doesArchiveExists, bool isArchiveComplete)
        {
            _zipWorker.ProgressChanged -= ZipProgressChanged;
            _zipWorker.ArchiveCompleted -= ZipWorkerArchiveCompleted;

            string message;
            LocalizableText localizedMessage;

            if (doesArchiveExists && isArchiveComplete)
            {
                message = $"Support request created at '{_zipWorker.FullPath}'.";
                localizedMessage = new LocalizableText(
                    nameof(AboutResources.SUPPORT_REQUEST_CREATED_USER_MESSAGE),
                    _zipWorker.FullPath);
                App.Instance.Tracer.Trace(
                    Constants.TracerName,
                    TraceLevelType.Info,
                    message);
            }
            else if (!doesArchiveExists)
            {
                message = $"Failed create support request at '{_zipWorker.FullPath}'.";
                localizedMessage = new LocalizableText(
                    nameof(AboutResources.SUPPORT_REQUEST_CREATION_FAILED_USER_MESSAGE),
                    _zipWorker.FullPath);
                App.Instance.Tracer.Trace(
                    Constants.TracerName,
                    TraceLevelType.Error,
                    message);
            }
            else
            {
                message = $"Incomplete support request created at '{_zipWorker.FullPath}'.";
                localizedMessage = new LocalizableText(
                    nameof(AboutResources.SUPPORT_REQUEST_INCOMPLETE_USER_MESSAGE),
                    _zipWorker.FullPath);
                App.Instance.Tracer.Trace(
                    Constants.TracerName,
                    TraceLevelType.Error,
                    message);
            }

            IsExecuting = false;
            ArchiveComplete?.Invoke(
                this,
                new ArchiveCompleteEventArgs(
                    localizedMessage,
                    isArchiveComplete,
                    doesArchiveExists));
        }

        private void ZipProgressChanged(double progress)
        {
            _bwZipFile.ReportProgress((int)Math.Ceiling(progress));
        }

        private void OnBwZipFileProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ZipProgression = e.ProgressPercentage;
        }

        private void OnBwZipFileRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Nothing
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Configures the support request manager with the specified files and folders to export,
        /// the archive path, and the traces path.
        /// </summary>
        /// <param name="fromConfig">A collection of file and folder paths to be included in the support request.</param>
        /// <param name="archivePath">The path where the archive will be stored.</param>
        /// <param name="tracesPath">The path where trace files are located.</param>
        public void Setup(ICollection<string> fromConfig, string archivePath, string tracesPath)
        {
            _archivePath = archivePath;

            if (fromConfig == null || fromConfig.Count == 0)
            {
                App.Instance.Tracer.Trace(
                    nameof(SupportRequestManager),
                    TraceLevelType.Warning,
                    "Files to export for support request are NOT configured");
                return;
            }

            _filesToExport.Clear();
            _foldersToExport.Clear();
            foreach (var path in fromConfig)
            {
                if (string.IsNullOrEmpty(path))
                {
                    App.Instance.Tracer.Trace(
                        nameof(SupportRequestManager),
                        TraceLevelType.Warning,
                        "File path to export for support request is empty",
                        path);
                    continue;
                }

                if (Directory.Exists(path))
                {
                    _foldersToExport.Add(path);
                }
                else if (File.Exists(path))
                {
                    _filesToExport.Add(path);
                }
                else
                {
                    App.Instance.Tracer.Trace(
                        nameof(SupportRequestManager),
                        TraceLevelType.Warning,
                        $"Config file or folder {path} does not exist");
                }
            }

            if ((_filesToExport == null || _filesToExport.Count == 0)
                && (_foldersToExport == null || _foldersToExport.Count == 0))
            {
                App.Instance.Tracer.Trace(
                    nameof(SupportRequestManager),
                    TraceLevelType.Warning,
                    "Files to export for support request are NOT found, check configuration");
            }
        }

        #endregion
    }
}
