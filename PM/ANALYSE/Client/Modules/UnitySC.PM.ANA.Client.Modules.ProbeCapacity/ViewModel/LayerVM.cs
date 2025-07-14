using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.PM.ANA.Client.Modules.ProbeCapacity.ViewModel
{
    public class LayerVM : ObservableObject
    {
        private bool _inEdition;
        public bool InEdition
        {
            get => _inEdition; set { if (_inEdition != value) { _inEdition = value; OnPropertyChanged(); } }
        }

        private string _name;
        public string Name
        {
            get => _name; set { if (_name != value) { _name = value; OnPropertyChanged(); } }
        }

        private double _thickness;
        public double Thickness
        {
            get => _thickness; 
            set 
            { 
                if (_thickness != value) 
                { 
                    _thickness = value; 
                    OnPropertyChanged(); 
                }
            }
        }

        private double _refractiveIndex;
        public double RefractiveIndex
        {
            get => _refractiveIndex; set { if (_refractiveIndex != value) { _refractiveIndex = value; OnPropertyChanged(); } }
        }

        private bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked; set { if (_isChecked != value) { _isChecked = value; OnPropertyChanged(); } }
        }

        private AutoRelayCommand _changeInEdition;
        public AutoRelayCommand ChangeInEdition
        {
            get
            {
                return _changeInEdition ?? (_changeInEdition = new AutoRelayCommand(
              () =>
              {
                  InEdition = !InEdition;
              },
              () => { return true; }));
            }
        }
    }
}
