using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using UnitySC.PM.Shared.Hardware.ClientProxy.Global;
using UnitySC.PM.Shared.UI.Main;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.UI.Hardware.GlobalStatus
{
    public class GlobalStatusViewModel : ObservableObject, IMenuContentViewModel
    {
        public bool CanClose() => true;

        public bool IsEnabled => true;

        private GlobalDeviceVM _globalDeviceVM;
        public GlobalDeviceVM GlobalDeviceVM
        {
            get => _globalDeviceVM; set { if (_globalDeviceVM != value) { _globalDeviceVM = value; OnPropertyChanged(); } }
        }
        public void Refresh()
        {
            GlobalDeviceVM = ClassLocator.Default.GetInstance<GlobalDeviceSupervisor>().GlobalDeviceVM;
        }
    }
}
