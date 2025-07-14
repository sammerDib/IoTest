using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Hardware.Probe.LiseHF;
using UnitySC.PM.ANA.Service.Core.CalibFlow;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Tools;

using static UnitySC.PM.ANA.Service.Core.Calibration.LiseGainCalibration;

namespace UnitySC.PM.ANA.Service.Core.MeasureCalibration
{
    public class ProbeCalibrationManagerLiseHF : IProbeCalibrationManager
    {
        private readonly AnaHardwareManager _hardwareManager;
        private readonly List<CalibrationLiseHFResult> _liseHFCalibrations;
        private readonly CancellationToken _cancellationToken;
        private ProbeLiseHFConfig _probeLiseHFConfig;
        
        private bool IsExpired(CalibrationLiseHFResult calibration)
        {
            if (_probeLiseHFConfig.CalibrationValidityPeriodMinutes < 0)
                return false;

            return (DateTime.UtcNow - calibration.Timestamp).TotalMinutes > _probeLiseHFConfig.CalibrationValidityPeriodMinutes;
        }

        public CancellationToken CancellationToken { get; set; }

        public ProbeCalibrationManagerLiseHF(ProbeLiseHFConfig probeLiseHFConfig, CancellationToken cancellationToken)
        {
            _liseHFCalibrations = new List<CalibrationLiseHFResult>();
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            _cancellationToken = cancellationToken;
            _probeLiseHFConfig= probeLiseHFConfig;
        }

        public IProbeCalibResult GetCalibration(bool createIfNeeded, string probeId, IProbeInputParams probeInputParams, PointPosition point, DieIndex die = null)
        {
            var liseHFInputParams = probeInputParams as LiseHFInputParams;
            CalibrationLiseHFResult resultat = null;

            // We look for a valid calibration
            resultat = _liseHFCalibrations
                .FirstOrDefault(c => c.CalibResult.AttenuationId == ProbeLiseHF.IlluminationToSliderID(liseHFInputParams.IsLowIlluminationPower)
                && c.CalibResult.ObjectiveId == liseHFInputParams.ObjectiveId);

            if (resultat != null && IsExpired(resultat))
            {
                _liseHFCalibrations.Remove(resultat);
                resultat = null;
            }

            if(resultat == null && createIfNeeded)
            {
                resultat = DoCalibration(probeInputParams);
                
                if (resultat != null && resultat.CalibResult != null && resultat.CalibResult.Success)
                {
                    // only if our calibration is good 

                    // we add the most recent calibration at the beginning.
                    // 1) it simplify recent research since it is at teh beginning of the list
                    // 2) in case calibration has not been well filtered or removed after a previous calibration then GetCalibration should now get the most recent one.
                    _liseHFCalibrations.Insert(0, resultat);
                }
                else
                {
                    // reset result even if our calibration is invalid
                    resultat = null; 
                }
            }

            return resultat?.CalibResult;
        }
        private CalibrationLiseHFResult DoCalibration(IProbeInputParams probeInputParams)
        {
            var position = _hardwareManager.Axes.GetPos();
            var newCalib = ExecuteLiseHFCalibration(probeInputParams as LiseHFInputParams);
            HardwareUtils.MoveAxesTo(_hardwareManager.Axes, position);
            return newCalib;
        }
        private CalibrationLiseHFResult ExecuteLiseHFCalibration(LiseHFInputParams probeInputParams)
        {
            var liseHFCalibrationInput = new CalibrationLiseHFInput(_probeLiseHFConfig.DeviceID);
            liseHFCalibrationInput.InputParams = probeInputParams;
            liseHFCalibrationInput.InitialContext = new ObjectiveContext(probeInputParams.ObjectiveId);
            var calibrationLiseHFFlow = new LiseHFCalibrationFlow(liseHFCalibrationInput)
            {
                CancellationToken = _cancellationToken,
            };

            var calibrationResult = calibrationLiseHFFlow.Execute();
            if (calibrationResult.Status.State != FlowState.Success)
                throw new Exception("LiseHF calibration failed.");

            return calibrationResult;
        }

        public void ResetCalibrations()
        {
            _liseHFCalibrations.Clear();
        }

        public void SetCalibration(string probeId, IProbeCalibResult probeCalibResult, IProbeInputParams probeInputParams, PointPosition point, DieIndex die = null)
        {
            var liseHFInputParams = probeInputParams as LiseHFInputParams;

            var calibIndex = GetCalibrationIndex(liseHFInputParams);
            if (calibIndex >= 0)
                _liseHFCalibrations.RemoveAt(calibIndex);
            var calibrationLiseHFFlowResult = new CalibrationLiseHFResult()
            {
                CalibResult = (ProbeLiseHFCalibResult)probeCalibResult,

                Timestamp = DateTime.Now,
                Status = new FlowStatus(FlowState.Success),
                Quality = 1
            };

            // we add the most recent calibration at the beginning.
            // 1) it simplify recent research since it is at teh beginning of the list
            // 2) in case calibration has not been well filtered or removed after a previous calibration then GetCalibration should now get the most recent one.
            _liseHFCalibrations.Insert(0, calibrationLiseHFFlowResult);
        }

        private int GetCalibrationIndex(LiseHFInputParams liseHFInputParams)
        {
            var attenuationId = ProbeLiseHF.IlluminationToSliderID(liseHFInputParams.IsLowIlluminationPower);
            return _liseHFCalibrations.FindIndex(c => c.CalibResult.AttenuationId == attenuationId 
                                                   && c.CalibResult.ObjectiveId == liseHFInputParams.ObjectiveId);
        }

        public void RecipeExecutionStarted()
        {
         }

        public void MeasureExecutionTerminated()
        {
        }

        public void CorrectMeasurePoint(MeasurePointDataResultBase measurePointDataResult)
        {
            // The Lise HF doesn't correct the measures at the end of the recipe execution
        }
    }
}
