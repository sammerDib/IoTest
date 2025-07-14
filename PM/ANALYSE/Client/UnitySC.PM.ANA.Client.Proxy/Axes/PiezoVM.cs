using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.PM.ANA.Client.Proxy.Axes
{
    public class PiezoVM : ObservableObject
    {
        private string _name;

        public string Name
        {
            get => _name; set { if (_name != value) { _name = value; OnPropertyChanged(); } }
        }

        private LengthVM _position;

        public LengthVM Position
        {
            get => _position; set { if (_position != value) { _position = value; OnPropertyChanged(); } }
        }

        private LengthVM _min;

        public LengthVM Min
        {
            get => _min; set { if (_min != value) { _min = value; OnPropertyChanged(); } }
        }

        private LengthVM _max;

        public LengthVM Max
        {
            get => _max; set { if (_max != value) { _max = value; OnPropertyChanged(); } }
        }

        public override string ToString()
        {
            return $"{Name}: {Position.Value} {Position.UnitSymbol}";
        }
    }
}
