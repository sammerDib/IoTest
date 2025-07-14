using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.Axes.ViewModel
{
    public class SettingType: INotifyPropertyChanged
    {
        public SettingType(string settingTypeDescription)
        {
            SettingTypeDescription = settingTypeDescription;
        }

        public string SettingTypeDescription { get; set; }
        public IList<SettingGroup> SettingGroups { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
