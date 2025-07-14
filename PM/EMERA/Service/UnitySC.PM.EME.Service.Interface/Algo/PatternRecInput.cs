using System.Runtime.Serialization;

using UnitySC.PM.EME.Service.Interface.Algo.GetZFocus;
using UnitySC.PM.EME.Service.Interface.Context;
using UnitySC.PM.EME.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.EME.Service.Interface.Algo
{
    public class PatternRecInput : IEMEInputFlow
    {
        public PatternRecInput()
        {
        }

        public PatternRecInput(PatternRecognitionData data, bool runAutofocus = false, GetZFocusInput getZFocusInput = null)
        {
            Data = data;
            RunAutofocus = runAutofocus;
            GetZFocusInput = getZFocusInput;
        }

        public InputValidity CheckInputValidity()
        {
            var validity = new InputValidity(true);

            if (RunAutofocus)
            {
                if (GetZFocusInput == null)
                {
                    validity.IsValid = false;
                    validity.Message.Add($"Input for camera autofocus must be provided if use autofocus is set to 'true'.");
                }
                else
                {
                    validity.ComposeWith(GetZFocusInput.CheckInputValidity());
                }
            }

            if (Data is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The pattern recognition data is missing.");
            }
            else
            {
                validity.ComposeWith(Data.CheckInputValidity());
            }

            return validity;
        }

        [DataMember]
        public EMEContextBase InitialContext { get; set; }

        [DataMember]
        public PatternRecognitionData Data { get; set; }

        [DataMember]
        public bool RunAutofocus { get; set; }

        public bool RunPreprocessing
        {
            get { return !double.IsNaN(Data.Gamma); }
        }

        [DataMember]
        public GetZFocusInput GetZFocusInput { get; set; }
    }
}
