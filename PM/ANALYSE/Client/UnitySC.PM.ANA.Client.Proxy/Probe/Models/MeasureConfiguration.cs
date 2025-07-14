using CommunityToolkit.Mvvm.ComponentModel;

namespace UnitySC.PM.ANA.Client.Proxy.Probe.Models
{
    public class MeasureConfiguration : ObservableObject
    {
        private string _folderName;
        private string _fileName;
        private int? _numberOfMeasure;

        public MeasureConfiguration()
        {
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

        public int? NumberOfMeasure
        {
            get
            {
                return _numberOfMeasure;
            }
            set
            {
                _numberOfMeasure = value;
                OnPropertyChanged();
            }
        }
    }
}
