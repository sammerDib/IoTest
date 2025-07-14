using System.ComponentModel;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.DMT.Modules.Settings.ViewModel
{
    public interface ISettingVM : INotifyPropertyChanged, INotifyPropertyChanging
    {
        string Header { get; }
        
        Side WaferSide { get; }
        
        bool IsBusy { get; set; }
        
        string BusyMessage { get; set; }
    }
}
