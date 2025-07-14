using CommunityToolkit.Mvvm.ComponentModel;

namespace UnitySC.Shared.ResultUI.Common.ViewModel.Export
{
    public class ExportEntry : ObservableObject
    {
        public string EntryName { get; }

        private bool _isChecked = true;

        public bool IsChecked
        {
            get { return _isChecked; }
            set { SetProperty(ref _isChecked, value); }
        }

        public ExportEntry(string entryName)
        {
            EntryName = entryName;
        }
    }
}
