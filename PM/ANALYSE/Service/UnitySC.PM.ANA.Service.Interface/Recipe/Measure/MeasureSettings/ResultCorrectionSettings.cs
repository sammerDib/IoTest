using System.Runtime.Serialization;

using UnitySC.Shared.Logger;

using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings
{
    [DataContract]
    public class ResultCorrectionSettings
    {
        [DataMember]
        public Length Offset { get; set; } = 0.Micrometers();

        [DataMember]
        public double Coef { get; set; } = 1;

        public Length ApplyCorrection(Length originalResult)
        {
            return originalResult * Coef + Offset;
        }

        public Length ApplyCorrectionAndLog(Length valueToCorrect, string nameOfValueToLog, ILogger logger)
        {
            var correctedValue = ApplyCorrection(valueToCorrect);
            if (valueToCorrect != null && correctedValue != null)
            {
                logger.Debug($"{nameOfValueToLog} result correction Before=>After : {valueToCorrect} => {correctedValue} ");
            }
            return correctedValue;
        }
    }
}
