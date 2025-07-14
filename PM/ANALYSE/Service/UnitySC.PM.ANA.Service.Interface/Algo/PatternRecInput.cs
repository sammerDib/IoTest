using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Flow;
using UnitySC.PM.ANA.Service.Interface.Recipe.Context;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    [XmlInclude (typeof(PatternRecognitionDataWithContext))]
    public class PatternRecInput : IANAInputFlow
    {
        public PatternRecInput()
        {
        }

        public PatternRecInput(PatternRecognitionData data, bool runAutofocus = false, AutoFocusSettings autofocusSettings = null)
        {
            Data = data;
            RunAutofocus = runAutofocus;
            AutoFocusSettings = autofocusSettings;
        }

        public InputValidity CheckInputValidity()
        {
            var validity = new InputValidity(true);

            if (RunAutofocus == true)
            {
                if (AutoFocusSettings == null)
                {
                    validity.IsValid = false;
                    validity.Message.Add($"Settings for camera or Lise autofocus must be provided if use autofocus is set to 'true'.");
                }
                else
                {
                    validity.ComposeWith(AutoFocusSettings.CheckInputValidity());
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
        public ANAContextBase InitialContext { get; set; }

        [DataMember]
        public PatternRecognitionData Data { get; set; }

        [DataMember]
        public bool RunAutofocus { get; set; }

        public bool RunPreprocessing
        {
            get { return !double.IsNaN(Data.Gamma); }
        }

        [DataMember]
        public AutoFocusSettings AutoFocusSettings { get; set; }
    }
}
