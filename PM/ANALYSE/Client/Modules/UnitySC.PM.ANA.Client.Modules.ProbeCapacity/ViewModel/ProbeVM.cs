using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.ANA.Service.Interface.Compatibility.Capability;

namespace UnitySC.PM.ANA.Client.Modules.ProbeCapacity.ViewModel
{
    public class ProbeVM : ObservableObject
    {
        public ProbeVM(string name)
        {
            Name = name;
            Capabilities = new ObservableCollection<CapabilityBase>();
        }
        public string Name { get; set; }
        public ObservableCollection<CapabilityBase> Capabilities { get; set;}

        private bool _isTop;
        public bool IsTop
        {
            get => _isTop; set { if (_isTop != value) { _isTop = value; OnPropertyChanged(); } }
        }

        private bool _isBottom;
        public bool IsBottom
        {
            get => _isBottom; set { if (_isBottom != value) { _isBottom = value; OnPropertyChanged(); } }
        }

        private bool _isDual;
        public bool IsDual
        {
            get => _isDual; set { if (_isDual != value) { _isDual = value; OnPropertyChanged(); } }
        }
    }
}
