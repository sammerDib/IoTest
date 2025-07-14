using System.Collections.Generic;
using System.Runtime.Serialization;

using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Calibration
{
    [DataContract]
    public class LiseHFIntegrationTimeCalibrationInput : IANAInputFlow
    { 
        // For serialization
        public LiseHFIntegrationTimeCalibrationInput()
        {

        }

        public LiseHFIntegrationTimeCalibrationInput(string probeId, List<string> objectiveIds) // voir ajout paramètre algo recherche light ref ?
        {
            ProbeId = probeId;
            ObjectiveIds = objectiveIds;
        }

        public InputValidity CheckInputValidity()
        {
            var validity = new InputValidity(true);

            if (ProbeId is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The probe ID is missing.");
            }

            if (ObjectiveIds is null || ObjectiveIds.Count == 0)
            {
                validity.IsValid = false;
                validity.Message.Add($"The objective ID is missing or empty.");
            }

            return validity;
        }

        [DataMember]
        public ANAContextBase InitialContext { get; set; }

        [DataMember]
        public List<string> ObjectiveIds { get; set; }

        [DataMember]
        public string ProbeId { get; set; }
    }

    [DataContract]
    public class LiseHFSpotCalibrationInput : IANAInputFlow
    {
        // For serialization
        public LiseHFSpotCalibrationInput()
        {

        }

        public LiseHFSpotCalibrationInput(string probeId, List<string> objectiveIds) // voir ajout paramètre algo recherche
        {
            ProbeId = probeId;
            ObjectiveIds = objectiveIds;
        }

        public InputValidity CheckInputValidity()
        {
            var validity = new InputValidity(true);

            if (ProbeId is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The LiseHF probe ID is missing.");
            }

            if (ObjectiveIds is null || ObjectiveIds.Count == 0)
            {
                validity.IsValid = false;
                validity.Message.Add($"objective ID list to calibrate is missing or is empty.");
            }

            return validity;
        }

        [DataMember]
        public ANAContextBase InitialContext { get; set; }

        [DataMember]
        public List<string> ObjectiveIds { get; set; }

        [DataMember]
        public string ProbeId { get; set; }

        [DataMember]
        public double StartExposureTime_ms { get; set; } = 95.0;

        [DataMember]
        public double StepExposureTime_ms { get; set; } = 5.0;
    }

    public class LiseHFSpotCheckInput : IANAInputFlow
    {
        // For serialization
        public LiseHFSpotCheckInput()
        {

        }

        public LiseHFSpotCheckInput(string probeId, string objectiveId, double camExposureTime_ms)
        {
            ProbeId = probeId;
            ObjectiveId = objectiveId;
            ExposureTime_ms = camExposureTime_ms;
        }

        public InputValidity CheckInputValidity()
        {
            var validity = new InputValidity(true);

            if (ProbeId is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The LiseHF probe ID is missing.");
            }

            if (ObjectiveId is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"objective ID to checkc spot position is missing or is empty.");
            }

            if(ExposureTime_ms <= 0.0)
            {
                validity.IsValid = false;
                validity.Message.Add($"Bad camera exposure time.");
            }
            return validity;
        }

        [DataMember]
        public ANAContextBase InitialContext { get; set; }

        [DataMember]
        public string ObjectiveId { get; set; }

        [DataMember]
        public string ProbeId { get; set; }

        [DataMember]
        public double ExposureTime_ms { get; set; } = 0.0;
    }
}
