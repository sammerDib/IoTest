using CommunityToolkit.Mvvm.ComponentModel;
using UnitySC.Shared.Data.Enum;


namespace UnitySC.Result.CommonUI.ViewModel.Search
{
    public class PPViewModel : ObservableRecipient
    {
        #region Properties

        private ResultType _resType;
        public ResultType ResultType
        {
            get => _resType;
            set
            {
                if (_resType != value)
                {
                    _resType = value;
                    OnPropertyChanged();
                }
            }
        }
        
        private string _resLabelName;
        public string ResultLabelName
        {
            get => _resLabelName;
            set
            {
                if (_resLabelName != value)
                {
                    _resLabelName = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Constructors

        public PPViewModel()
        {
            ResultLabelName = "<No Label>";
            ResultType = ResultType.Empty;
        }

        public PPViewModel(ResultType resTyp)
        {
            ResultLabelName = resTyp.DefaultLabelName(0);
            ResultType = resTyp;
        }

        public PPViewModel(string labelName, ResultType resTyp)
        {
            ResultLabelName = labelName;
            ResultType = resTyp;
        }

        #endregion
    }
}
