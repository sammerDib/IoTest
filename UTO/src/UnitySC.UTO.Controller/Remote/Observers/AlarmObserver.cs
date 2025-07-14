using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Agileo.AlarmModeling;
using Agileo.Common.Logging;
using Agileo.Semi.Gem.Abstractions.E30;
using Agileo.Semi.Gem300.Abstractions.E87;

using UnitySC.EFEM.Rorze.Devices.IoModule.EK9000;
using UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio1;
using UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio2;
using UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0;
using UnitySC.Equipment.Abstractions.Devices.Aligner;
using UnitySC.Equipment.Abstractions.Devices.Ffu;
using UnitySC.Equipment.Abstractions.Devices.LightTower;
using UnitySC.Equipment.Abstractions.Devices.LoadPort;
using UnitySC.Equipment.Abstractions.Devices.Robot;
using UnitySC.Equipment.Abstractions.Devices.SubstrateIdReader;
using UnitySC.UTO.Controller.Remote.Constants;
using UnitySC.UTO.Controller.Remote.Services;

namespace UnitySC.UTO.Controller.Remote.Observers
{
    internal class AlarmObserver : E30StandardSupport
    {
        #region Fields

        #region Equipment Alarms (Relative ID)

        //Robot
        private const int RobotCommandFailed = 10;
        private const int RobotCommandError = 1004;
        private const int RobotWaferFall = 1008;

        //LoadPorts
        private const int FosbDoorDetection = 1157;

        //FFU
        private const int FfuSpeedAlarm = 11;
        private const int FfuDifferentielPressureAlarm = 12;

        //GEM controller
        private const int GemTcCommTimeout = 140001;
        private const int GemDoubleSlotDetected = 140002;
        private const int GemCarrierIdReadFail = 140003;
        private const int StartIndexGem300Alarms = 1000000;

        #endregion

        #endregion

        #region Constructors

        public AlarmObserver(IE30Standard e30Standard, ILogger logger)
            : base(e30Standard, logger)
        {
        }

        #endregion Constructors

        #region IInstanciableDevice Support

        public override void OnSetup(Agileo.EquipmentModeling.Equipment equipment)
        {
            base.OnSetup(equipment);

            /*
             System manager alarms handled by Agileo's Alarms component
             For now, only the SystemManager is responsible for manage modules alarms.
             So, we subscribe AlarmStateChanged on this device only.
            */
            GUI.Common.App.Instance.AlarmCenter.Services.AlarmOccurrenceStateChanged += Services_AlarmOccurrenceStateChanged;
            App.UtoInstance.GemController.E30Std.AlarmServices.AlarmChanged += AlarmServices_AlarmChanged;
        }

        #endregion IInstanciableDevice Support

        #region Event Handlers

        private void AlarmServices_AlarmChanged(object sender, AlarmEventArgs e)
        {
            if (e.Alarm.ID <= 870000)
            {
                //When alarm is raised for E84 and E87, we will clear them after load port initialization
                return;
            }

            if (GUI.Common.App.Instance.AlarmCenter.Repository.GetAlarms()
                .FirstOrDefault(al => al.Name == e.Alarm.Name) is not {} alarm)
            {
                return;
            }

            switch (alarm.State)
            {
                case AlarmState.Set:
                    if(!e.Alarm.IsSet)
                    {
                        GUI.Common.App.Instance.AlarmCenter.Services.ClearAlarm(App.UtoInstance.GemController, e.Alarm.Name);
                    }
                    break;
                case AlarmState.Cleared:
                    if (e.Alarm.IsSet)
                    {
                        GUI.Common.App.Instance.AlarmCenter.Services.SetAlarm(App.UtoInstance.GemController, e.Alarm.Name);

                        Task.Run(
                            () =>
                            {
                                //Check alarms that needs to be reset automatically
                                if (e.Alarm.Name.Contains(
                                        E87WellknownNames.Alarms.AttemptToUseOutOfServiceLP)
                                    || e.Alarm.Name.Contains(E87WellknownNames.Alarms.CarrierPresenceError)
                                    || e.Alarm.Name.Contains(E87WellknownNames.Alarms.CarrierPlacementError)
                                    || e.Alarm.Name.Contains(E87WellknownNames.Alarms.CarrierDockFailure)
                                    || e.Alarm.Name.Contains(E87WellknownNames.Alarms.CarrierOpenFailure)
                                    || e.Alarm.Name.Contains(E87WellknownNames.Alarms.CarrierRemovalError))
                                {
                                    E30Standard.AlarmServices.ClearAlarm(e.Alarm.Name);
                                }
                            });
                    }
                    break;
            }
        }

        private void Services_AlarmOccurrenceStateChanged(object sender, AlarmOccurrenceEventArgs args)
        {
            if (args.AlarmOccurrence.Acknowledged && args.AlarmOccurrence.State == AlarmState.Set)
            {
                //no need to notify automation for alarm acknowledged
                return;
            }

            if (args.AlarmOccurrence.Alarm.Id > StartIndexGem300Alarms)
            {
                //no need to notify automation for GEM300 alarm
                return;
            }

            int alarmId = GetGemAlarmID(args.AlarmOccurrence); // get alarm id here

            try
            {
                var alarm = E30Standard.AlarmServices.GetAlarm(alarmId);

                var variableUpdates = new List<VariableUpdate>()
                {
                    new(DVs.AlarmID, alarm.ID),
                    new(DVs.AlarmDescription, alarm.Text),
                    new(DVs.AlarmExtendedText, args.AlarmOccurrence.Text ?? string.Empty)
                };

                if (args.AlarmOccurrence.State == AlarmState.Set)
                {
                    E30Standard.AlarmServices.SetAlarm(alarmId, variableUpdates);
                }
                else
                {
                    if (alarm.ID >= 840000)
                    {
                        //When alarm is raised for E84 and E87, we will clear them after load port initialization
                        return;
                    }
                    E30Standard.AlarmServices.ClearAlarm(alarmId);
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception);
            }
        }

        #endregion Event Handlers

        #region Private

        private int GetGemAlarmID(AlarmOccurrence occurrence)
        {
            try
            {
                return E30Standard.AlarmServices.GetAlarm(occurrence.Alarm.Name).ID;
            }
            catch
            {
                //do nothing
            }

            //10010 - 11146 => RA4200 (Aligner)
            if (occurrence.Alarm.Container is IAligner)
            {
                return ALIDs.AlignerAlarm;
            }

            //30010 - 31147 => RR75x0 (Robot)
            if (occurrence.Alarm.Container is IRobot)
            {
                if (occurrence.Alarm.RelativeId is RobotCommandFailed or RobotCommandError)
                {
                    return ALIDs.RobotMovingFail;
                }

                if (occurrence.Alarm.RelativeId is RobotWaferFall)
                {
                    return ALIDs.WaferBrokenInterlock;
                }

                return ALIDs.EfemAlarm;
            }

            //41010 - 42191 => RV2011 (LoadPort1)
            //42010 - 43191 => RV2012 (LoadPort2)
            if (occurrence.Alarm.Container is ILoadPort)
            {
                return ALIDs.EfemAlarm;
            }

            //21010 => PC17401 (SubstrateIdReader)
            //60010 - 61085 => DIO0
            //71010 - 72035 => DIO11
            //82010 - 83035 => DIO22
            //100010 => RC5300 (LightTower)
            if (occurrence.Alarm.Container is IDio0
                or IDio1
                or IDio2
                or IEK9000
                or ILightTower
                or ISubstrateIdReader)
            {
                return ALIDs.EfemAlarm;
            }

            //110010 - 110011 => RC5500 (FFU)
            if (occurrence.Alarm.Container is IFfu)
            {
                if (occurrence.Alarm.RelativeId is FfuSpeedAlarm or FfuDifferentielPressureAlarm)
                {
                    return ALIDs.SpeedPressureDifferentialFailure;
                }

                return ALIDs.EfemAlarm;
            }

            //GEMController
            if (occurrence.Alarm.RelativeId is GemTcCommTimeout)
            {
                return ALIDs.TcCommunicationTimeout;
            }

            if (occurrence.Alarm.RelativeId is GemDoubleSlotDetected)
            {
                return ALIDs.DoubleSlotDetected;
            }

            if (occurrence.Alarm.RelativeId is GemCarrierIdReadFail)
            {
                return ALIDs.CarrierIdReadFail;
            }

            return ALIDs.ControllerAlarm;
        }

        #endregion

        #region dispose

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                /* System manager alarms handled by Agileo's Alarms component */
                GUI.Common.App.Instance.AlarmCenter.Services.AlarmOccurrenceStateChanged -=
                    Services_AlarmOccurrenceStateChanged;
                App.UtoInstance.GemController.E30Std.AlarmServices.AlarmChanged -= AlarmServices_AlarmChanged;
            }

            base.Dispose(disposing);
        }

        #endregion dispose
    }
}
