using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    [XmlInclude(typeof(ANAContextBase))]
    public class CalibrationDualLiseInput : IANAInputFlow
    {
        public CalibrationDualLiseInput()
        { }

        public CalibrationDualLiseInput(string probeID, MeasureLiseInput measureLiseUpInput, MeasureLiseInput measureLiseDownInput, ProbeSample calibrationSample, XYPosition calibrationPosition)
        {
            ProbeID = probeID;
            MeasureLiseUp = measureLiseUpInput;
            MeasureLiseDown = measureLiseDownInput;
            CalibrationSample = calibrationSample;
            CalibrationPosition = calibrationPosition;
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

            if (CalibrationSample is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The calibration sample is missing.");
            }

            if (CalibrationPosition is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The calibration position is missing.");
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
        public ProbeSample CalibrationSample { get; set; }

        [DataMember]
        public XYPosition CalibrationPosition { get; set; }
    }
}
