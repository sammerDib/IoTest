using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;

using UnitySC.Shared.Configuration;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.TC.Shared.Operations.Interface;
using UnitySC.Shared.TC.Shared.Service.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.Shared.TC.Shared.Operations.Implementation
{
    public class AlarmOperations : IAlarmOperations
    {
        #region Members
        private const int ALARMID_GENERIC_MIN = 0;
        private const int ALARMID_GENERIC_MAX = 999;
        private const int ALARMID_DATAFLOW_MIN = 1000;
        private const int ALARMID_DATAFLOW_MAX = 1999;
        private const int ALARMID_ANALYSE_MIN = 2000;
        private const int ALARMID_ANALYSE_MAX = 2999;
        private const int ALARMID_DEMETER_MIN = 3000;
        private const int ALARMID_DEMETER_MAX = 3999;
        private const int ALARMID_EMERA_MIN = 4000;
        private const int ALARMID_EMERA_MAX = 4999;
        private const int ALARMID_HELIOS_MIN = 5000;
        private const int ALARMID_HELIOS_MAX = 5999;
        private const int ALARMID_ARGOS_MIN = 6000;
        private const int ALARMID_ARGOS_MAX = 6999;

        private List<Alarm> _alarmsList;
        private ILogger _logger;
        private IAlarmServiceCB _alarmService;
        private IAlarmOperationsCB _alarmOperationsCB;

        public AlarmOperations(ILogger logger)
        {
            _logger = logger;

            String configfile = ClassLocator.Default.GetInstance<IAutomationConfiguration>().AlarmConfigurationFilePath;
            _logger = ClassLocator.Default.GetInstance<ILogger<AlarmOperations>>();

            if (!ListExtension.IsNullOrEmpty<String>(configfile) && File.Exists(configfile))
            {
                try
                {
                    AlarmsList = XML.Deserialize<AlarmSettings>(configfile).AlarmsList;                    
                }
                catch (Exception ex)
                {
                    _logger.Error("AlarmSettings.xml is incorrect. exception =" + ex.Message + ex.StackTrace);
                    throw ex;
                }
            }
            else
            {
                AlarmSettings alarmsSettings = new AlarmSettings();
                alarmsSettings.AlarmsList = new List<Alarm>
                { 
                    new Alarm() { Active = false, ActorType= ActorType.ANALYSE, Description = "test", ID = 99, Level = AlarmCriticality.Major, Name = "TestsAlarm", FromErrors = new List<ErrorID>() { ErrorID.RecipeStartingError_PMRequestStart },
                                    LastTriggeringError = ErrorID.Undefined, Acknowledged =false, DoReactivation=false, ErrorSource = new Identity(0, UnitySC.Shared.Data.Enum.ActorType.DataflowManager,0) }
                };
                XML.Serialize(alarmsSettings, configfile);
                AlarmsList = XML.Deserialize<AlarmSettings>(configfile).AlarmsList;
            }
            if (!IsAlarmListValidated(AlarmsList))
                throw new Exception("AlarmSettings.xml file is not valid");
        }

        private bool IsAlarmListValidated(List<Alarm> alarms)
        {
            // No same ID in alarms list
            bool IsValidated = alarms.TrueForAll(alarmItem => alarms.FindAll(a => a.ID == alarmItem.ID).Count == 1);
            if (!IsValidated)
            {
                Alarm alarmFound = alarms.Find(a1 => alarms.Exists(a2 => (a1 != a2) && (a1.ID == a2.ID)));
                _logger.Error($"AlarmSettings.xml is incorrect. ID={alarmFound.ID} was found for both alarm at least");
            }

            IsValidated = alarms.TrueForAll(alarmItem => IsAlarmIDInRange(alarmItem));
            if (!IsValidated)
            {
                Alarm alarmFound = alarms.Find(a => !IsAlarmIDInRange(a));
                _logger.Error($"AlarmSettings.xml is incorrect. Alarm for {alarmFound.ActorType} with ID ={alarmFound.ID} is not in its correct range.");
            }
            // No same alarm name in alarms list
            IsValidated = alarms.TrueForAll(alarmItem => alarms.FindAll(a=> a.Name == alarmItem.Name).Count == 1);
            if (!IsValidated)
            {
               Alarm alarmFound = alarms.Find(a1 => alarms.Exists(a2 => (a1 != a2) && (a1.Name == a2.Name)));
                _logger.Error($"AlarmSettings.xml is incorrect. {alarmFound.Name} name was found for both alarm at least");
            }
            // For one ErrorID declared in AlarmSetting => One alarm
            List<ErrorID> ErrorIdAlreadyUsed = new List<ErrorID>();
            ErrorID erridInvalid = ErrorID.Undefined;
            foreach (ActorType actor in Enum.GetValues(typeof(ActorType)))
            {
                ErrorIdAlreadyUsed.Clear();
                foreach (Alarm alarm in alarms)
                {
                    if (alarm.ActorType == actor)
                    {
                        if (ErrorIdAlreadyUsed.Any(errid => alarm.FromErrors.Contains(errid) && (errid != ErrorID.Undefined)))
                        {
                            // Not valid !!!
                            IsValidated = false;
                            foreach (var errid in alarm.FromErrors)
                            {
                                if (ErrorIdAlreadyUsed.Contains(errid))
                                    erridInvalid = errid;
                            }
                            if (erridInvalid != ErrorID.Undefined)
                                _logger.Error($"AlarmSettings.xml is incorrect. {erridInvalid} is declared in AlarmSetting and is associated with several alarms in ActorType {actor.ToString()}");
                            else
                                _logger.Error($"AlarmSettings.xml is incorrect. An ErrorId is declared in AlarmSetting and is associated with several alarms in ActorType {actor.ToString()}");
                            break;
                        }
                        else
                            ErrorIdAlreadyUsed.AddRange(alarm.FromErrors.Where<ErrorID>(errid => (errid != ErrorID.Undefined)));
                    }
                }
            }
            // For one ErrorID in enum => One alarm
            foreach (ErrorID errid in Enum.GetValues(typeof(ErrorID)))
            {
                if (errid == ErrorID.Undefined) continue;
                IsValidated = false;
                foreach (ActorType actor in Enum.GetValues(typeof(ActorType)))
                {
                    if (GetAlarm(actor, errid) == null)
                    {
                        IsValidated = true;
                        break;
                    }
                }
                if (!IsValidated)
                {
                    _logger.Error($"AlarmSettings.xml is incorrect. {errid} is not declared with an alarm in AlarmSetting");
                    break;
                }
            }
            return IsValidated;
        }

        private bool IsAlarmIDInRange(Alarm alarm)
        {
            switch (alarm.ActorType)
            {
                case ActorType.Unknown: return (alarm.ID >= ALARMID_GENERIC_MIN) && (alarm.ID <= ALARMID_GENERIC_MAX);
                case ActorType.DataflowManager: return (alarm.ID >= ALARMID_DATAFLOW_MIN) && (alarm.ID <= ALARMID_DATAFLOW_MAX);
                case ActorType.ANALYSE: return (alarm.ID >= ALARMID_ANALYSE_MIN) && (alarm.ID <= ALARMID_ANALYSE_MAX);
                case ActorType.DEMETER: return (alarm.ID >= ALARMID_DEMETER_MIN) && (alarm.ID <= ALARMID_DEMETER_MAX);
                case ActorType.EMERA: return (alarm.ID >= ALARMID_EMERA_MIN) && (alarm.ID <= ALARMID_EMERA_MAX);
                case ActorType.HeLioS: return (alarm.ID >= ALARMID_HELIOS_MIN) && (alarm.ID <= ALARMID_HELIOS_MAX);
                case ActorType.Argos: return (alarm.ID >= ALARMID_ARGOS_MIN) && (alarm.ID <= ALARMID_ARGOS_MAX);
                case ActorType.BrightField2D:
                case ActorType.Darkfield:
                case ActorType.BrightFieldPattern:
                case ActorType.Edge:
                case ActorType.NanoTopography:
                case ActorType.LIGHTSPEED:
                case ActorType.BrightField3D:
                case ActorType.EdgeInspect:
                case ActorType.HardwareControl:
                case ActorType.ADC:
                default: return false;
            }
        }

        public void Init()
        {
            _alarmService = ClassLocator.Default.GetInstance<IAlarmServiceCB>();
            _alarmOperationsCB = ClassLocator.Default.GetInstance<IAlarmOperationsCB>();
        }

        public List<Alarm> AlarmsList
        {
            get => _alarmsList;
            set
            {
                _alarmsList = value;
            }
        }

        #endregion Members

        public List<Alarm> GetAllAlarms()
        {
            return AlarmsList;
        }

        private void ResetAlarm(Alarm alarm)
        {
            ResetAlarm(new List<Alarm>(){ alarm }) ;
        }

        public void ResetAlarm(List<Alarm> alarms)
        {
            foreach (var alarm in alarms)
            {
                _logger.Information("ResetSetAlarm - Name = " + alarm.Name);
                _alarmService.ResetAlarm(new List<Alarm> { alarm });
                
            }
        }
        public void SetAlarm(Identity identity, ErrorID errorId)
        {
            Alarm alarmsFound = GetAlarm(identity.ActorType, errorId);
            if (alarmsFound != null)
            {
                alarmsFound.ErrorSource = identity;
                alarmsFound.LastTriggeringError = errorId;
                switch (alarmsFound.Level)
                {
                    case AlarmCriticality.Information:
                    case AlarmCriticality.Warning:
                        SetAlarm(alarmsFound);     // If alarm is acknowledged, alarm is auto-cleared then thse alarms dont need to be reactivate to heighlight them  
                        break;
                    case AlarmCriticality.Minor:
                        if (alarmsFound.Active)
                            ReactivateAlarm(alarmsFound);
                        else
                            SetAlarm(alarmsFound);
                        break;
                    case AlarmCriticality.Major:    // If alarm already active and acknowledged, alarm is not triggered for user visibility => then clear it and activate it again  
                    case AlarmCriticality.Critical: // Critical => do Init
                        if (alarmsFound.Active)
                            ReactivateAlarm(alarmsFound);
                        else
                            SetAlarm(alarmsFound);
                        if(alarmsFound.Level == AlarmCriticality.Critical)
                            _alarmOperationsCB.SetCriticalErrorState(alarmsFound.ErrorSource, errorId);
                        _alarmService.StopCancelAllJobs(); // To abort all current jobs before Init
                        break;
                    default:
                        break;
                }
            }
                
        }

        private void ReactivateAlarm(Alarm alarm)
        {            
            Alarm alarmFound = AlarmsList.Find(a => a.Name == alarm.Name);
            alarmFound.DoReactivation = true;
            ResetAlarm(alarmFound);            
        }

        public void NotifyAlarmChanged(Alarm alarm)
        {
            Alarm alarmFound = AlarmsList.Find(a => a.Name == alarm.Name);

            // On alarm activation/desactivation 
            if (alarmFound.Active != alarm.Active)
            {
                if (alarm.Active)
                    _logger.Information($"Alarm {alarm.Name} activated");
                else
                {
                    _logger.Information($"Alarm {alarm.Name} released");
                }
            }
            // On alarm acknowkedged 
            if (alarmFound.Acknowledged != alarm.Acknowledged)
            {
                if (alarm.Acknowledged && alarm.Active)
                {
                    _logger.Information($"Alarm {alarm.Name} acknowledged");
                    // Action on Minor/Major alarm only
                    if ((alarmFound.Level == AlarmCriticality.Minor)|| (alarmFound.Level == AlarmCriticality.Major))                    
                    {
                        _alarmOperationsCB.OnErrorAcknowledged(alarmFound.ErrorSource, alarmFound.LastTriggeringError);
                        ResetAlarm(alarmFound);
                    }
                }
                else
                    _logger.Information($"Alarm {alarm.Name} not acknowledged");                
            }      

            // Update alarm state
            alarmFound.Active = alarm.Active;
            alarmFound.Acknowledged = alarm.Acknowledged && alarm.Active;

            // Reactivate alarm if needed
            if (alarmFound.DoReactivation)
                SetAlarm(alarmFound);
            else
                if(!alarmFound.Active)
                    alarmFound.LastTriggeringError = ErrorID.Undefined;
            
        }
        private void SetAlarm(Alarm alarm)
        {
            if (alarm != null)
            {
                alarm.DoReactivation = false;       
                _logger.Information("SetAlarm - Name = " + alarm.Name);
                _alarmService.SetAlarm(new List<Alarm>() { alarm });
            }
        }

        public Alarm GetAlarm(ActorType actor, ErrorID error)
        {
            return AlarmsList.Find(a => a.FromErrors.Contains(error) && (a.ActorType == actor));
        }

        public void ReInitialize()
        {
            foreach (var alarm in AlarmsList)
            {
                alarm.Acknowledged = false;
                alarm.DoReactivation = false;
                alarm.Active = false;
                alarm.LastTriggeringError= ErrorID.Undefined;
            }
        }
    }
}
