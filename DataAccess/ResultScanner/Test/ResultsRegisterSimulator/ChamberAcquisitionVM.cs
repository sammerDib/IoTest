using System;
using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Data.Enum;

namespace ResultsRegisterSimulator
{
    public class ChamberAcquisitionVM : ObservableObject
    {
    
        private ObservableCollection<ChamberAcquisitionItemVM> _listResTypes;
        public ObservableCollection<ChamberAcquisitionItemVM> ListResTypes
        {
            get => _listResTypes; set { if (_listResTypes != value) { _listResTypes = value; OnPropertyChanged(); } }
        }

        public ChamberAcquisitionVM(ActorType actor)
        {
            var list = new ObservableCollection<ChamberAcquisitionItemVM>();
            switch (actor)
            {
                case ActorType.DEMETER:
                    list.Add(new ChamberAcquisitionItemVM(ResultType.DMT_AmplitudeX_Front));
                    list.Add(new ChamberAcquisitionItemVM(ResultType.DMT_AmplitudeY_Front));
                    list.Add(new ChamberAcquisitionItemVM(ResultType.DMT_CurvatureX_Front));
                    list.Add(new ChamberAcquisitionItemVM(ResultType.DMT_CurvatureY_Front));
                    list.Add(new ChamberAcquisitionItemVM(ResultType.DMT_Brightfield_Front));

                    list.Add(new ChamberAcquisitionItemVM(ResultType.DMT_AmplitudeX_Back));
                    list.Add(new ChamberAcquisitionItemVM(ResultType.DMT_AmplitudeY_Back));
                    list.Add(new ChamberAcquisitionItemVM(ResultType.DMT_CurvatureX_Back));
                    list.Add(new ChamberAcquisitionItemVM(ResultType.DMT_CurvatureY_Back));
                    list.Add(new ChamberAcquisitionItemVM(ResultType.DMT_Brightfield_Back));

                    list[0].IsUsed = true;
                    list[1].IsUsed = true;
                    list[4].IsUsed = true;

                    break;

                case ActorType.HeLioS:
                    list.Add(new ChamberAcquisitionItemVM(ResultType.HLS_Amplitude_WideFW));
                    list.Add(new ChamberAcquisitionItemVM(ResultType.HLS_Saturation_WideFW));
                    list.Add(new ChamberAcquisitionItemVM(ResultType.HLS_Haze_WideFW));

                    list.Add(new ChamberAcquisitionItemVM(ResultType.HLS_Amplitude_NarrowBW));
                    list.Add(new ChamberAcquisitionItemVM(ResultType.HLS_Saturation_NarrowBW));
                    list.Add(new ChamberAcquisitionItemVM(ResultType.HLS_Haze_NarrowBW));

                    list[0].IsUsed = true;
                    list[1].IsUsed = true;

                    list[3].IsUsed = true;
                    list[4].IsUsed = true;
                    break;

                default:
                    break;
            }
            ListResTypes = list;
        }
    }

    public class ChamberAcquisitionItemVM : ObservableObject
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


        public ChamberAcquisitionItemVM(ResultType type)
        {
            IsUsed = false;
            IdxMax = 0;
            ResType = type;
        }
    }

}
