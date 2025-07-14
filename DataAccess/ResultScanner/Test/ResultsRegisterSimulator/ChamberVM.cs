using System;
using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Data.Enum;

namespace ResultsRegisterSimulator
{
    public class ChamberVM : ObservableObject
    {
        private int _dbID;

        public int ID
        {
            get => _dbID;
            set
            {
                if (_dbID != value) { _dbID = value; OnPropertyChanged(); }
            }
        }

        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value) { _name = value; OnPropertyChanged(); OnPropertyChanged(nameof(ResName)); }
            }
        }

        private ActorType _actortype = ActorType.Unknown;

        public ActorType ActorType
        {
            get => _actortype;
            set
            {
                if (_actortype != value) { _actortype = value; OnPropertyChanged(); }
            }
        }

        public string ResName
        {
            get => $"{_name} Result Item Types";
        }

        public string ResNameAcq
        {
            get => $"{_name} Acq Item Types";
        }

        private bool _isUsed = true;

        public bool IsUsed
        {
            get => _isUsed;
            set
            {
                if (_isUsed != value) { _isUsed = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsAcqUsed)); }
            }
        }

        public bool IsAcqUsed
        {
            get => _isUsed && _isRegAcqVisible;
        }


        private ObservableCollection<ResultVM> _listResults;
        public ObservableCollection<ResultVM> ListResults 
        {
            get => _listResults; set { if (_listResults != value) { _listResults = value; OnPropertyChanged(); } }
        }

        #region Acquisition 
        private bool _isRegAcqVisible = false;
        public bool IsRegAcqVisible
        {
            get => _isRegAcqVisible; set { if (_isRegAcqVisible != value) { _isRegAcqVisible = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsAcqUsed)); } }
        }
        #endregion

        private ChamberAcquisitionVM _chAcqVM = null;
        public ChamberAcquisitionVM ChamberAcqVM
        {
            get => _chAcqVM; set { if (_chAcqVM != value) { _chAcqVM = value; OnPropertyChanged(); } }
        }
        

        public ChamberVM()
        {
            ID = -1;
            Name = String.Empty;
            _actortype = ActorType.Unknown;

            IsRegAcqVisible = false;

            ListResults = new ObservableCollection<ResultVM>();
        }

        public ChamberVM(int iD, string name, ActorType actorType)
        {
            ID = iD;
            Name = name;
            ActorType = actorType;

            bool StdUse = (actorType != ActorType.Unknown && actorType != ActorType.HardwareControl && actorType != ActorType.DataflowManager);

            var list = new ObservableCollection<ResultVM>();

            if (StdUse)
            {
                list.Add(new ResultVM(ResultType.ADC_Klarf));
                list.Add(new ResultVM(ResultType.ADC_ASO));
            }

            if(actorType == ActorType.LIGHTSPEED || actorType == ActorType.HeLioS)
            {
                list.Add(new ResultVM(ResultType.ADC_Haze));
            }

            if (actorType == ActorType.ANALYSE)
            {
                list.Add(new ResultVM(ResultType.ANALYSE_TSV));
                list.Add(new ResultVM(ResultType.ANALYSE_NanoTopo));
                list.Add(new ResultVM(ResultType.ANALYSE_Thickness));
                list.Add(new ResultVM(ResultType.ANALYSE_Topography));
                list.Add(new ResultVM(ResultType.ANALYSE_Step));
                list.Add(new ResultVM(ResultType.ANALYSE_Trench));
                list.Add(new ResultVM(ResultType.ANALYSE_Bow));
                list.Add(new ResultVM(ResultType.ANALYSE_Pillar));
                list.Add(new ResultVM(ResultType.ANALYSE_PeriodicStructure));
            }

            IsRegAcqVisible =  actorType == ActorType.HeLioS || actorType == ActorType.DEMETER;
            
            ChamberAcqVM = new ChamberAcquisitionVM(actorType);

            ListResults = list;
        }
    }
}
