using System.Runtime.Serialization;

using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Calibration
{
    [DataContract]
    public class ObjectiveCalibrationInput : IANAInputFlow
    {
        // For serialization
        public ObjectiveCalibrationInput()
        {
        }

        public ObjectiveCalibrationInput(string probeId, string objectiveId, ObjectiveCalibration previousCalibration, double gain = double.NaN)
        {
            ProbeId = probeId;
            ObjectiveId = objectiveId;
            PreviousCalibration = previousCalibration;
            Gain = gain;
        }

        public InputValidity CheckInputValidity()
        {
            var validity = new InputValidity(true);

            if (ProbeId is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The probe ID is missing.");
            }

            if (ObjectiveId is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The objective ID is missing.");
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
        public double Gain { get; set; }

        [DataMember]
        public Length OpticalReferenceElevationFromStandardWafer { get; set; }

        [DataMember]
        public ObjectiveCalibration PreviousCalibration { get; set; }
    }
}
