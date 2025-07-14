using System;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Data.Enum;

namespace ResultsRegisterSimulator
{
    public class ResultVM : ObservableObject
    {
        private ResultType _resTyp = ResultType.Empty;
        public ResultType ResType
        {
            get => _resTyp;
            set
            {
                if (_resTyp != value) { _resTyp = value; OnPropertyChanged(); Label = ResType.GetLabelName(); }
            }
        }

        private bool _isUsed = false;
        public bool IsUsed
        {
            get => _isUsed;
            set
            {
                if (_isUsed != value) { _isUsed = value; OnPropertyChanged(); }
            }
        }

        private int _idxMax = 0;
        public int IdxMax
        {
            get => _idxMax; set { if (_idxMax != value) { _idxMax = value; OnPropertyChanged(); } }
        }

        private string _lbl = string.Empty;
        public string Label
        {
            get => _lbl; set { if (_lbl != value) { _lbl = value; OnPropertyChanged(); } }
        }
        
        private bool _isMultiResult = false;
        public bool IsMultiResult
        {
            get => _isMultiResult;
            set
            {
                if (_isMultiResult != value) { _isMultiResult = value; OnPropertyChanged(); }
            }
        }


        public ResultVM(ResultType type) : this(type, 0, false)
        {  }

        public ResultVM(ResultType type, int idxMax) : this(type, idxMax, false)
        {  }

        public ResultVM(ResultType type, int idxMax, bool isused)
        {
            IsUsed = isused;
            IdxMax = idxMax;
            ResType = type;

            IsMultiResult = ResType.GetResultFormat() == ResultFormat.Metrology;
        }
    }

}
