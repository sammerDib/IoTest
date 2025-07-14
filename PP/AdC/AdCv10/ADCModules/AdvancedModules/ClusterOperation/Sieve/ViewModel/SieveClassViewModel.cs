using CommunityToolkit.Mvvm.ComponentModel;


namespace AdvancedModules.ClusterOperation.Sieve.ViewModel
{
    [System.Reflection.Obfuscation(Exclude = true)]
    internal class SieveClassViewModel : ObservableRecipient
    {
        private SieveClass SieveClass;
        private SieveViewModel ParentVM;

        public SieveClassViewModel(SieveClass pSieveClass, SieveViewModel pParentVM)
        {
            SieveClass = pSieveClass;
            ParentVM = pParentVM;
        }

        public string DefectLabel
        {
            get { return SieveClass.DefectLabel; }
            set
            {
                if (value == SieveClass.DefectLabel)
                    return;
                SieveClass.DefectLabel = value;
                OnPropertyChanged();
            }
        }

        public bool ApplyFilter
        {
            get { return SieveClass.ApplyFilter; }
            set
            {
                if (value == SieveClass.ApplyFilter)
                    return;
                SieveClass.ApplyFilter = value;
                ParentVM.NotifyPropertyChanged();
            }
        }

    }


}
