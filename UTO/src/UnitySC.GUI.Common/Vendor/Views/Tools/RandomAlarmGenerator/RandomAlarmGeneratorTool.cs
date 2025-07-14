using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

using Agileo.AlarmModeling;
using Agileo.Common.Localization;
using Agileo.EquipmentModeling;
using Agileo.GUI.Commands;
using Agileo.GUI.Components.Tools;
using Agileo.GUI.Services.Icons;

using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;

namespace UnitySC.GUI.Common.Vendor.Views.Tools.RandomAlarmGenerator
{
    public class RandomAlarmGeneratorTool : Tool
    {
        #region Constructors

        static RandomAlarmGeneratorTool()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(RandomAlarmGeneratorToolResource)));
        }
        public RandomAlarmGeneratorTool(string id, IIcon icon = null) : base(id, icon)
        {
        }

        public RandomAlarmGeneratorTool() : base(nameof(RandomAlarmGeneratorToolResource.RANDOM_ALARM_GENERATOR), PathIcon.Alarm)
        {
        }

        #endregion

        #region Commands

        #region GenerateRandomAlarmsCommand

        private ICommand _generateRandomAlarmsCommand;
        
        public ICommand GenerateRandomAlarmsCommand
        {
            get
            {
                if (_generateRandomAlarmsCommand != null)
                {
                    return _generateRandomAlarmsCommand;
                }

                _generateRandomAlarmsCommand = new DelegateCommand(GenerateRandomAlarmsCommandExecuteMethod, GenerateRandomAlarmsCommandCanExecuteMethod);

                return _generateRandomAlarmsCommand;
            }
        }

        private void GenerateRandomAlarmsCommandExecuteMethod()
        {
            var rnd = new Random();

            foreach (var device in App.Instance.EquipmentManager.Equipment.AllDevices<Device>())
            {
                if (!device.Alarms.Any())
                {
                    continue;
                }

                var indicesOfAlarmsToRaise = new List<int>();
                for (var i = 0; i < device.Alarms.Count; i++)
                {
                    if (device.Alarms[i].RelativeId != 0)
                    {
                        indicesOfAlarmsToRaise.Add(i);
                    }
                }

                var nbAlarmsAvailable = indicesOfAlarmsToRaise.Count;
                var nbAlarmsToRaise = rnd.Next(0, nbAlarmsAvailable);

                for (var i = 0; i < nbAlarmsToRaise; ++i)
                {
                    //Pick alarm to raise
                    var indexAlarmToRaise = rnd.Next(0, indicesOfAlarmsToRaise.Count);
                    var alarmToRaise = device.Alarms[indicesOfAlarmsToRaise[indexAlarmToRaise]];
                    indicesOfAlarmsToRaise.RemoveAt(indexAlarmToRaise);

                    //Pick how many times the alarm will be raised
                    var nbAlarmOccurences = rnd.Next(0, 11);

                    //Raise the alarm
                    for (var j = 0; j < nbAlarmOccurences; ++j)
                    {
                        //If alarm already raised, clear it to raise it again
                        if (alarmToRaise.State != AlarmState.Cleared)
                        {
                            App.Instance.AlarmCenter.Services.ClearAlarm(device, alarmToRaise.Name);
                        }

                        App.Instance.AlarmCenter.Services.SetAlarm(device, alarmToRaise.Name);
                    }
                }
            }
        }

        private bool GenerateRandomAlarmsCommandCanExecuteMethod() => true;

        #endregion

        #endregion
    }
}
