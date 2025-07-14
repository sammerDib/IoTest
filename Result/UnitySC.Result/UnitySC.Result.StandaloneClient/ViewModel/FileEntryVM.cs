using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Result.StandaloneClient.Models;
using UnitySC.Shared.Format.Base;

namespace UnitySC.Result.StandaloneClient.ViewModel
{
    public class FileEntryVM : ObservableRecipient
    {
        #region Properties

        public FileEntry FileEntry { get; }

        private int _fileIndex;

        public int FileIndex
        {
            get => _fileIndex;
            set => SetProperty(ref _fileIndex, value);
        }
        
        #endregion Properties

        #region Constructor
        
        public FileEntryVM(FileEntry fileEntry, int fileIndex)
        {
            FileEntry = fileEntry;
            FileIndex = fileIndex;
        }

        #endregion Constructor
    }
}
