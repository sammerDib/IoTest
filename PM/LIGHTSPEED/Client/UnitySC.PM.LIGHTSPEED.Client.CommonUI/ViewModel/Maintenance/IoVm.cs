using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace UnitySC.PM.LIGHTSPEED.Client.CommonUI.ViewModel.Maintenance
{
    public class IoVm : ViewModelBase
    {
        private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled; set { if (_isEnabled != value) { _isEnabled = value; RaisePropertyChanged(); } }
        }

        private string _name;
        public string Name
        {
            get => _name; set { if (_name != value) { _name = value; RaisePropertyChanged(); } }
        }
        public IoVm()
        {

        }
    }
}
