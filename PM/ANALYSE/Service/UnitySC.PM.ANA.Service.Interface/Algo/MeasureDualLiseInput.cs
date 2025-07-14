using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Flow;
using UnitySC.PM.ANA.Service.Interface.ProbeLise;
using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    [XmlInclude(typeof(ANAContextBase))]
    public class MeasureDualLiseInput : IANAInputFlow
    {
        public MeasureDualLiseInput()
        { }

        public MeasureDualLiseInput(string probeID, MeasureLiseInput measureLiseUpInput, MeasureLiseInput measureLiseDownInput, ProbeSampleLayer unknownLayer, ProbeDualLiseCalibResult dualLiseCalibration)
        {
            ProbeID = probeID;
            MeasureLiseUp = measureLiseUpInput;
            MeasureLiseDown = measureLiseDownInput;
            UnknownLayer = unknownLayer;
            DualLiseCalibration = dualLiseCalibration;
        }

        public InputValidity CheckInputValidity()
        {
            var validity = new InputValidity(true);

            if (ProbeID is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The probe ID is missing.");
            }

            if (MeasureLiseUp is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The measure data for Lise up is missing.");
            }
            else
            {
                validity.ComposeWith(MeasureLiseUp.CheckInputValidity());
            }

            if (MeasureLiseDown is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The measure data for Lise down is missing.");
            }
            else
            {
                validity.ComposeWith(MeasureLiseDown.CheckInputValidity());
            }

            if (UnknownLayer is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The probe sample layer of unknownLayer is missing.");
            }

            if (DualLiseCalibration is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The global thickness calibration is missing.");
            }

            return validity;
        }

        [DataMember]
        public ANAContextBase InitialContext { get; set; }

        [DataMember]
        public string ProbeID { get; set; }

        [DataMember]
        public MeasureLiseInput MeasureLiseUp { get; set; }

        [DataMember]
        public MeasureLiseInput MeasureLiseDown { get; set; }

        [DataMember]
        public ProbeSampleLayer UnknownLayer { get; set; }

        [DataMember]
        public ProbeDualLiseCalibResult DualLiseCalibration { get; set; }
    }
}
