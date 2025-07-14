using System.Runtime.Serialization;

using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    public class ThicknessLiseInput : LiseInput
    {
        public ThicknessLiseInput()
        {
        }

        public ThicknessLiseInput(string probeId, ProbeSample sample, double gain = double.NaN, int nbAveraging = 1)
            : base(probeId, gain, nbAveraging)
        {
            Sample = sample;
        }

        [DataMember]
        public ProbeSample Sample { get; set; }

        public override InputValidity CheckInputValidity()
        {
            var validity = base.CheckInputValidity();

            if (Sample is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The sample is missing.");
            }

            return validity;
        }
    }
}
