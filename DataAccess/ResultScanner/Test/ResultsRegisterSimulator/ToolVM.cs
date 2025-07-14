using System;
using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

namespace ResultsRegisterSimulator
{
    public class ToolVM : ObservableObject
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
                if (_name != value) { _name = value; OnPropertyChanged(); }
            }
        }

        private ObservableCollection<ChamberVM> _chambers;

        public ObservableCollection<ChamberVM> Chambers
        {
            get => _chambers; set { if (_chambers != value) { _chambers = value; OnPropertyChanged(); } }
        }

        public ToolVM()
        {
            ID = -1;
            Name = String.Empty;
        }

        public ToolVM(int iD, string name)
        {
            ID = iD;
            Name = name;
        }
    }
}
