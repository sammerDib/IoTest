using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.PM.Shared.UI.ViewModels
{
    public class TabViewModelBase : ViewModelBaseExt
    {
        private string _title = "";

        /// <summary>
        /// Sets and gets the Title property.
        /// Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                if (_title == value)
                {
                    return;
                }

                _title = value;
                OnPropertyChanged();
            }
        }

        private bool _isEnables = true;

        public bool IsEnabled
        {
            get => _isEnables; set { if (_isEnables != value) { _isEnables = value; OnPropertyChanged(); } }
        }

        public virtual void PrepareDisplay()
        {
        }

        public virtual bool CanChangeTab()
        {
            return true;
        }

        public virtual void Update()
        {
        }
    }
}
