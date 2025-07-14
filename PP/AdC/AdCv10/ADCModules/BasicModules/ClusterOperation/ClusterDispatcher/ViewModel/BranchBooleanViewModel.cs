using CommunityToolkit.Mvvm.ComponentModel;

namespace BasicModules.ClusterDispatcher
{
    ///////////////////////////////////////////////////////////////////////
    // 
    ///////////////////////////////////////////////////////////////////////
    [System.Reflection.Obfuscation(Exclude = true)]
    internal class BranchBooleanViewModel : ObservableRecipient
    {
        private bool? _bool = false;
        public bool? Bool
        {
            get
            {
                return _bool;
            }
            set
            {
                if (_bool == value)
                    return;
                _bool = value;

                OnBoolChanged();
                OnPropertyChanged();
            }
        }

        protected void OnBoolChanged()
        {
            if (_bool == true)
            {
                foreach (var branch in dispatcherDefectClassViewModel.branchTable)
                {
                    if (branch != null && branch != this)
                        branch.Bool = false;
                }
                dispatcherDefectClassViewModel.BranchIndex = branchIndex;
            }
            else
            {
                dispatcherDefectClassViewModel.BranchIndex = -1;
            }
        }

        public DispatcherDefectClassViewModel dispatcherDefectClassViewModel;
        private int branchIndex;

        public BranchBooleanViewModel(DispatcherDefectClassViewModel dispatcherDefectClassViewModel, int branchIndex)
        {
            this.dispatcherDefectClassViewModel = dispatcherDefectClassViewModel;
            this.branchIndex = branchIndex;
        }

        public override string ToString()
        {
            return "bool?-" + _bool.ToString();
        }

    }
}
