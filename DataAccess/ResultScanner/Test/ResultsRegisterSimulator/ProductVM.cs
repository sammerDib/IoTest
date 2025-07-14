using System;

using CommunityToolkit.Mvvm.ComponentModel;

namespace ResultsRegisterSimulator
{
    public class ProductVM : ObservableObject
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

        public ProductVM()
        {
            ID = -1;
            Name = String.Empty;
        }

        public ProductVM(int iD, string name)
        {
            ID = iD;
            Name = name;
        }
    }
}
