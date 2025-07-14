using CommunityToolkit.Mvvm.ComponentModel;

namespace UnitySC.PM.ANA.Client.Proxy.Probe.Models
{
    public class ExportConfiguration : ObservableObject
    {

        private string _folderName;
        private string _fileName;
        private int? _numberOfAcquisition;
        private bool _exportRawData;
        private bool _exportSelectedPeaks;
        private bool _isRunning;

        public ExportConfiguration()
        {
            _isRunning = false;
        }

        public string FolderName
        {
            get
            {
                return _folderName;
            }
            set
            {
                _folderName = value;
                OnPropertyChanged();
            }
        }

        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
                OnPropertyChanged();
            }
        }

        public int? NumberOfAcquisition
        {
            get
            {
                return _numberOfAcquisition;
            }
            set
            {
                _numberOfAcquisition = value;
                OnPropertyChanged();
            }
        }

        public bool ExportRawData
        {
            get
            {
                return _exportRawData;
            }
            set
            {
                _exportRawData = value;
                OnPropertyChanged();
            }
        }

        public bool ExportSelectedPeaks
        {
            get
            {
                return _exportSelectedPeaks;
            }
            set
            {
                _exportSelectedPeaks = value;
                OnPropertyChanged();
            }
        }

        public bool IsRunning
        {
            get
            {
                return _isRunning;
            }
            set
            {
                _isRunning = value;
                OnPropertyChanged();
            }
        }
    }

}
