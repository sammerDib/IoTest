using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

using Agileo.DataMonitoring.DataWriter.File;
using Agileo.DataMonitoring.DataWriter.File.Csv;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;

using UnitySC.GUI.Common.Vendor.Helpers;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.DataCollection.Library.Popups
{
    /// <summary>
    /// Popup containing data relative to the file that just have been written.
    /// </summary>
    public class FolderPathDetailsPopup : Notifier
    {
        /// <summary>
        /// Build a new popup.
        /// </summary>

        public FolderPathDetailsPopup() : this(new List<FileDataWriter>())
        {


            if (!IsInDesignMode)
            {
                throw new InvalidOperationException("Parameter-less constructor must be used at design time only");
            }

            FileDataWriters.Add(new CsvFileDataWriter("Dcp1", "StoragePath\\SubFolderPath"));
            FileDataWriters.Add(new CsvFileDataWriter("Dcp2", "StoragePath\\SubFolderPath"));
            FileDataWriters.Add(new CsvFileDataWriter("Dcp3", "StoragePath\\SubFolderPath"));
            FileDataWriters.Add(new CsvFileDataWriter("Dcp4", "StoragePath\\SubFolderPath"));
        }

        /// <summary>
        /// Build a new popup with the given parameter.
        /// </summary>
        /// <param name="fileDataWriters">The <see cref="IEnumerable{T}"/> of <see cref="FileDataWriters"/> just recorded.</param>
        public FolderPathDetailsPopup(IEnumerable<FileDataWriter> fileDataWriters)
        {
            FileDataWriters = new ObservableCollection<FileDataWriter>(fileDataWriters);
        }


        private ObservableCollection<FileDataWriter> _fileDataWriters;

        /// <summary>
        /// The <see cref="IEnumerable{T}"/> of <see cref="FileDataWriters"/> just recorded.
        /// </summary>
        public ObservableCollection<FileDataWriter> FileDataWriters
        {
            get { return _fileDataWriters; }
            set
            {
                if (_fileDataWriters == value) return;
                _fileDataWriters = value;
                OnPropertyChanged(nameof(FileDataWriters));
            }
        }

        #region OpenFolderCommand

        private ICommand _openFolderCommand;

        public ICommand OpenFolderCommand =>
            _openFolderCommand ?? (_openFolderCommand = new DelegateCommand<FileDataWriter>(OpenFolderCommandExecute));

        private static void OpenFolderCommandExecute(FileDataWriter fileDataWriter)
        {
            OpenFileDirectory.ProcessStart(fileDataWriter.OutputFile);
        }

        #endregion OpenFolderCommand
    }
}
