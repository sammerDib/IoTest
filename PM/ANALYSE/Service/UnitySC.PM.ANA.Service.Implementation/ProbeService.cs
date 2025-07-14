using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Windows.Controls;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Hardware.ObjectiveSelector;
using UnitySC.PM.ANA.Hardware.Probe.Lise;
using UnitySC.PM.ANA.Hardware.Probe.LiseHF;
using UnitySC.PM.ANA.Service.Core.CalibFlow;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Implementation.ProbeMeasures;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.ReformulationMessage;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ProbeService : DuplexServiceBase<IProbeServiceCallback>, IProbeService, IProbeServiceCallback, IProbeServiceCallbackProxy
    {
        #region Fields

        private const int WaitStartMotion = 500;
        private const int WaitEndMotion = 10000;
        private readonly AnaHardwareManager _hardwareManager;
        private readonly object _lock = new object();
        private const string DeviceName = "Probes";
        private readonly XYZTopZBottomPosition _positionBeforeCalib; // note de rti : Check probeService calibration callback is really use, _positionBeforeCalib is not initialized (generate a warning)
        private readonly CalibrationManager _calibrationManager;
        private readonly IReferentialManager _referentialManager;

        private Dictionary<string, IProbeCalibration> _probeCalibrations;

        private Dictionary<string, IProbeMeasure> _probeMeasures;

        #endregion Fields

        #region Methods

        public ProbeService(ILogger<ProbeService> logger) : base(logger, ExceptionType.HardwareException)
        {
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            _calibrationManager = ClassLocator.Default.GetInstance<CalibrationManager>();
            _referentialManager = ClassLocator.Default.GetInstance<IReferentialManager>();

            _probeCalibrations = new Dictionary<string, IProbeCalibration>();
            _probeMeasures = new Dictionary<string, IProbeMeasure>();
        }

        public Response<List<IProbeConfig>> GetProbesConfig()
        {
            return InvokeDataResponse(messagesContainer =>
            {
                if (_hardwareManager.Probes.Count <= 0)
                    return null;

                var probesList = new List<IProbeConfig>();
                foreach (var probe in _hardwareManager.Probes)
                    probesList.Add(probe.Value.Configuration);

                Console.WriteLine("GetProbesData Called");

                return probesList;
            });
        }

        public Response<List<ObjectivesSelectorConfigBase>> GetObjectivesSelectorData()
        {
            return InvokeDataResponse(messagesContainer =>
            {
                if (_hardwareManager.ObjectivesSelectors.Count <= 0)
                    return null;

                var objectivesSelectorsList = new List<ObjectivesSelectorConfigBase>();
                foreach (var objectiveSelector in _hardwareManager.ObjectivesSelectors)
                    objectivesSelectorsList.Add(objectiveSelector.Value.Config);

                Console.WriteLine("GetObjectivesSelectorData Called");

                return objectivesSelectorsList;
            });
        }

        public Response<bool> SingleAcquisition(string probeID, IProbeInputParams inputParameters)
        {
            _logger?.Information(FormatMessage(probeID, "Received SingleShotAcquisition request"));
            return InvokeDataResponse<bool>(messagesContainer =>
            {
                try
                {
                    var probeToStartAcquisition = GetProbeFromID(probeID);
                    if (probeToStartAcquisition == null)
                        throw (new Exception(FormatMessage(probeID, $"Probe not found")));

                    probeToStartAcquisition.DoSingleAcquisition(inputParameters);

                    return true;
                }
                catch (Exception ex)
                {
                    ReformulationMessage(messagesContainer, ex.Message, MessageLevel.Error);
                    return false;
                }
            });
        }

        public Response<bool> StartContinuousAcquisition(string probeID, IProbeInputParams inputParameters)
        {
            _logger?.Information(FormatMessage(probeID, "Received StartAcquisition request"));
            return InvokeDataResponse<bool>(messagesContainer =>
            {
                try
                {
                    var probe = GetProbeFromID(probeID);
                    if (probe == null)
                    {
                        throw (new Exception(FormatMessage(probeID, $"Probe not found")));
                    }

                    //TODO move into probe.StartContinuousAcquisition, no specific here
                    if (probe is ProbeLiseHF)
                    {
                        var inputacq = inputParameters as LiseHFInputParams;
                        //This is the persistent integration time calibration 
                        var integrationTimeCalibration = _calibrationManager.GetLiseHFObjectiveIntegrationTime(inputacq.ObjectiveId);
                        if (integrationTimeCalibration == null)
                            throw new Exception($"No integration time calibration has been set in HW Config {inputacq.ObjectiveId}");

                        var integrationTime = inputacq.IsLowIlluminationPower
                            ? integrationTimeCalibration.LowIllumFilterIntegrationTime_ms
                            : integrationTimeCalibration.StandardFilterIntegrationTime_ms;

                        inputacq.IntegrationTimems = integrationTime;
                    }

                    probe.StartContinuousAcquisition(inputParameters);

                    return true;
                }
                catch (Exception ex)
                {
                    ReformulationMessage(messagesContainer, ex.Message, MessageLevel.Error);
                    return false;
                }
            });
        }

        public Response<bool> StopContinuousAcquisition(string probeID)
        {
            _logger?.Information(FormatMessage(probeID, "Received StopAcquisition request"));
            return InvokeDataResponse<bool>(messagesContainer =>
            {
                try
                {
                    var probe = GetProbeFromID(probeID);
                    if (probe == null)
                    {
                        throw (new Exception(FormatMessage(probeID, "StopAcquisition failed: Failed to get probe id")));
                    }

                    probe.StopContinuousAcquisition();

                    return true;
                }
                catch (Exception)
                {
                    ReformulationMessage(messagesContainer, "Probe Stop acquisition failed", MessageLevel.Error);
                    return false;
                }
            });
        }

        public Response<IProbeResult> DoMeasure(string probeID, IProbeInputParams inputParameters)
        {
            _logger?.Information(FormatMessage(probeID, "Received DoMeasure request"));
            return InvokeDataResponse(messagesContainer =>
            {
                IProbeResult probeResult = null;
                try
                {
                    var probeMeasure = GetProbeMeasure(probeID);

                    probeResult = probeMeasure.DoMeasure(inputParameters);

                    if (probeResult == null)
                        throw (new Exception(FormatMessage(probeID, "DoMeasure failed")));

                    return probeResult;
                }
                catch (Exception)
                {
                    ReformulationMessage(messagesContainer, "Probe DoMeasure failed", MessageLevel.Error);
                    return null;
                }
            });
        }

        public Response<List<IProbeResult>> DoMultipleMeasures(string probeID, IProbeInputParams inputParameters, int nbMeasuresWanted)
        {
            _logger?.Information(FormatMessage(probeID, "Received DoMultipleMeasures request"));
            return InvokeDataResponse(messagesContainer =>
            {
                var probeResults = new List<IProbeResult>();

                try
                {
                    while (probeResults.Count < nbMeasuresWanted)
                    {
                        var measure = DoMeasure(probeID, inputParameters);
                        probeResults.Add(measure.Result);
                    }

                    return probeResults;
                }
                catch (Exception)
                {
                    ReformulationMessage(messagesContainer, "Probe DoMultipleMeasures failed", MessageLevel.Error);
                    return new List<IProbeResult>();
                }
            });
        }

        public Response<bool> StartCalibration(string probeID, IProbeCalibParams probeCalibParams, IProbeInputParams inputParameters)
        {
            _logger?.Information(FormatMessage(probeID, "Received Calibrate request"));
            return InvokeDataResponse(messagesContainer =>
            {
                try
                {
                    var probeCalibration = GetProbeCalibration(probeID);
                    //TODO maybe we could use flow for calibration
                    //return probeCalibration.Probe.ProbeCalibrationManager.GetCalibration(true, probeID, inputParameters, null) != null;
                    return probeCalibration.StartCalibration(probeCalibParams, inputParameters);
                }
                catch (Exception)
                {
                    ReformulationMessage(messagesContainer, "Probe Calibrate failed", MessageLevel.Error);
                    return false;
                }
            });
        }

        public Response<bool> IsCalibrated(string probeID, IProbeCalibParams probeCalibParams, IProbeInputParams inputParameters)
        {
            _logger?.Information(FormatMessage(probeID, "Received IsCalibrated request"));
            return InvokeDataResponse(messagesContainer =>
            {
                try
                {
                    var probeCalibration = GetProbeCalibration(probeID);
                    return probeCalibration.Probe.CalibrationManager.GetCalibration(false, probeID, inputParameters, null) != null;

                }
                catch (Exception)
                {
                    ReformulationMessage(messagesContainer, "Probe is not calibrate", MessageLevel.Error);
                    return false;
                }
            });
        }
        public Response<VoidResult> CancelCalibration(string probeID)
        {
            _logger?.Information(FormatMessage(probeID, "Received Cancel Calibrate request"));
            return InvokeVoidResponse(messagesContainer =>
            {
                try
                {
                    var probeCalibration = GetProbeCalibration(probeID);
                    probeCalibration.CancelCalibration();
                }
                catch (Exception)
                {
                    ReformulationMessage(messagesContainer, "Cancel Probe Calibrate failed", MessageLevel.Error);
                }
            });
        }

        private IProbeCalibration GetProbeCalibration(string probeID)
        {
            if (_probeCalibrations.ContainsKey(probeID))
                return _probeCalibrations[probeID];
            else
            {
                // we must create it
                var probe = GetProbeFromID(probeID);
                if (probe == null)
                    throw (new Exception(FormatMessage(probeID, "GetProbeCalibration failed: no probe id found")));

                switch (probe)
                {
                    case ProbeDualLise probeDualLise:
                        _probeCalibrations.Add(probeID, new DualLiseCalibration(probeDualLise, _logger));
                        break;

                    case ProbeLiseHF probeLiseHF:
                        _probeCalibrations.Add(probeID, new LiseHFCalibration(probeLiseHF, _logger));
                        break;

                    default:
                        break;
                }

                return _probeCalibrations[probeID];
            }
        }

        private IProbeMeasure GetProbeMeasure(string probeID)
        {
            if (_probeMeasures.ContainsKey(probeID))
                return _probeMeasures[probeID];
            else
            {
                // we must create it
                var probe = GetProbeFromID(probeID);
                if (probe == null)
                    throw (new Exception(FormatMessage(probeID, "GetProbeMeasure failed: no probe id found")));

                switch (probe)
                {
                    case ProbeDualLise probeDualLise:
                        _probeMeasures.Add(probeID, new DualLiseMeasure(probeDualLise, (DualLiseCalibration)GetProbeCalibration(probeID), _logger));
                        break;

                    case ProbeLise probeLise:
                        _probeMeasures.Add(probeID, new SingleLiseMeasure(probeLise, _logger));
                        break;

                    case ProbeLiseHF probeLiseHF:
                        _probeMeasures.Add(probeID, new LiseHFMeasure(probeLiseHF, (LiseHFCalibration)GetProbeCalibration(probeID), _logger));
                        break;

                    default:
                        break;
                }

                return _probeMeasures[probeID];
            }
        }

        public Response<bool> SetNewObjectiveToUse(string objectiveSelectorID, string newObjectiveToUseID, bool applyObjectiveOffset)
        {
            _logger?.Information(FormatMessage("All probes", $"Received change objective request for objective {newObjectiveToUseID} using objective selector {objectiveSelectorID}"));
            return InvokeDataResponse(messagesContainer =>
            {
                lock (_lock)
                {
                    try
                    {
                        return HardwareUtils.SetNewObjective(objectiveSelectorID, newObjectiveToUseID, applyObjectiveOffset, _logger, _referentialManager, _hardwareManager, _calibrationManager);
                    }
                    catch (Exception ex)
                    {
                        _logger?.Error(ex, "Probe change objective failed");
                        ReformulationMessage(messagesContainer, "Probe change objective failed", MessageLevel.Error);
                        return false;
                    }
                }
            });
        }

       

        public Response<ObjectiveConfig> GetObjectiveInUse(string objectiveSelectorID)
        {
            _logger?.Information(FormatMessage("All probes", "Received get objective position request"));
            return InvokeDataResponse<ObjectiveConfig>(messagesContainer =>
            {
                try
                {
                    var objectivesSelector = _hardwareManager.ObjectivesSelectors[objectiveSelectorID];
                    if (objectivesSelector == null)
                        throw (new Exception(FormatMessage("All probes", "ObjectiveSelectorGetPos failed: no objectivesSelector id found")));

                    return objectivesSelector.GetObjectiveInUse();
                }
                catch (Exception ex)
                {
                    _logger?.Error(ex, "Probe get objective position failed");
                    ReformulationMessage(messagesContainer, "Probe get objective position failed", MessageLevel.Error);
                    return null;
                }
            });
        }

        public Response<VoidResult> SubscribeToProbeChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                lock (_lock)
                {
                    _logger.Information(FormatMessage(null, "Subscribed to probe change"));
                    Subscribe();
                }
            });
        }

        public Response<VoidResult> UnsubscribeToProbeChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                lock (_lock)
                {
                    _logger.Information(FormatMessage(null, "Unsubscribed to probe change"));
                    Unsubscribe();
                }
            });
        }

        public void ProbeStateUpdatedCallback(DeviceState probeState, string deviceID)
        {
            InvokeCallback(probeServiceCallback => probeServiceCallback.ProbeStateUpdatedCallback(probeState, deviceID));
        }

        public void ProbeRawMeasuresCallback(ProbeSignalBase probeRawSignal)
        {
            InvokeCallback(probeServiceCallback => probeServiceCallback.ProbeRawMeasuresCallback(probeRawSignal));
        }

        public void ProbeCalibrationResultsCallback(ProbeCalibResultsBase probeCalibrationResult)
        {
            var axesService = ClassLocator.Default.GetInstance<IAxesService>();
            if (_positionBeforeCalib != null)
            {
                var destination = new XYZTopZBottomPosition(
                    new StageReferential(),
                    _positionBeforeCalib.X,
                    _positionBeforeCalib.Y,
                    _positionBeforeCalib.ZTop,
                    _positionBeforeCalib.ZBottom);
                axesService.GotoPosition(destination, AxisSpeed.Normal);
                Thread.Sleep(WaitStartMotion);
                axesService.WaitMotionEnd(WaitEndMotion);
            }
            InvokeCallback(probeServiceCallback => probeServiceCallback.ProbeCalibrationResultsCallback(probeCalibrationResult));
        }

        public void ProbeNewObjectiveInUseCallback(ObjectiveResult currentObjective)
        {
            InvokeCallback(probeServiceCallback => probeServiceCallback.ProbeNewObjectiveInUseCallback(currentObjective));
        }

        private static void ReformulationMessage(List<Message> messageContainer, string message, MessageLevel defaultLevel = MessageLevel.Error)
        {
            var content = "Subscribed to probe change";
            var userContent = ReformulationMessageManager.GetUserContent(DeviceName, content, message);
            var level = ReformulationMessageManager.GetLevel(DeviceName, content, defaultLevel);
            //if (!string.IsNullOrEmpty(userContent))
            messageContainer.Add(new Message(level, userContent, message, DeviceName));
        }

        private IProbe GetProbeFromID(string probeID)
        {
            IProbe probe = null;
            if (_hardwareManager.Probes.ContainsKey(probeID))
                probe = _hardwareManager.Probes[probeID] as IProbe;

            return probe;
        }

        private string FormatMessage(string deviceID, string message)
        {
            return $"[{deviceID}]{message}";
        }

        private XYZTopZBottomPosition GetPos()
        {
            var axesService = ClassLocator.Default.GetInstance<IAxesService>();
            var result = axesService.GetCurrentPosition()?.Result;
            if (result is XYZTopZBottomPosition position)
            {
                return position;
            }
            else
            {
                _logger.Error($"Position from axes service is not a XYZTopZBottomPosition");
                return new XYZTopZBottomPosition(new MotorReferential(), 0, 0, 0, 0);
            }
        }

        private Length PreviousZOffsetWithMainObjective(string objectiveSelectorID, string previousObjectiveID)
        {
            var objectives = _hardwareManager.ObjectivesSelectors[objectiveSelectorID].Config.Objectives;
            var mainObjective = objectives.Find(x => x.IsMainObjective);
            var previousObjectiveConfig = objectives.First(x => x.DeviceID == previousObjectiveID);
            var previousCalibration = _calibrationManager.GetObjectiveCalibration(previousObjectiveConfig.DeviceID);
            if (mainObjective.DeviceID == previousObjectiveConfig.DeviceID || previousCalibration is null)
                return 0.Millimeters();
            return previousCalibration.ZOffsetWithMainObjective;
        }

        public Response<bool> ResetObjectivesSelectors()
        {
            return InvokeDataResponse(messageContainer =>
            {
                try
                {
                    _logger.Information("Reset objectives selectors");

                    foreach (var objective in _hardwareManager.ObjectivesSelectors)
                    {
                        if (objective.Value is LinMotUdp)
                        {
                            objective.Value.Disconnect();
                            objective.Value.Init();
                            _logger.Information("Objectives selector selector are succesffuly reset");
                        }
                    }
                    return true;
                }
                catch(Exception ex)
                {
                    _logger.Error("ResetObjectivesSelectors error : ", ex);

                    return false;
                }
            });
        }

        #endregion Methods
    }


   
}
