using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

using Agileo.AlarmModeling;
using Agileo.GUI.Commands;

namespace UnitySC.Equipment.Abstractions.Vendor.Devices.Simulation.Alarms
{
    public class AlarmSimulationViewModel
    {
        #region Fields

        private readonly GenericDevice.GenericDevice _device;

        #endregion

        #region Constructor

        public AlarmSimulationViewModel(GenericDevice.GenericDevice device)
        {
            _device = device;
            Alarms = _device.Alarms.ToList();
            SelectedAlarm = Alarms[0];
        }

        #endregion

        #region Properties

        public List<Alarm> Alarms { get; }

        public Alarm SelectedAlarm { get; set; }

        #endregion

        #region Commands

        private DelegateCommand _setAlarmCommand;

        public ICommand SetAlarmCommand => _setAlarmCommand ??= new DelegateCommand(SetAlarmCommandExecute);

        private void SetAlarmCommandExecute()
        {
            if (SelectedAlarm != null)
            {
                _device.SetAlarmById(SelectedAlarm.RelativeId.ToString());
            }
        }

        private DelegateCommand _clearAlarmCommand;

        public ICommand ClearAlarmCommand => _clearAlarmCommand ??= new DelegateCommand(ClearAlarmCommandExecute);

        private void ClearAlarmCommandExecute()
        {
            if (SelectedAlarm != null)
            {
                _device.ClearAlarmById(SelectedAlarm.RelativeId.ToString());
            }
        }

        #endregion
    }
}
