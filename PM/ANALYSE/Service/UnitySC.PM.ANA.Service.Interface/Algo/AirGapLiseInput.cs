using System.Runtime.Serialization;

using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    public class AirGapLiseInput : IANAInputFlow
    {
        // For serialization
        public AirGapLiseInput()
        {
        }

        public AirGapLiseInput(string probeId, double gain = double.NaN, int nbAveraging = 1)
        {
            LiseData = new LiseInput(probeId, gain, nbAveraging);
        }

        public InputValidity CheckInputValidity()
        {
            var validity = new InputValidity(true);

            if (LiseData is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The lise data is missing.");
            }
            else
            {
                validity.ComposeWith(LiseData.CheckInputValidity());
            }

            return validity;
        }

        [DataMember]
        public ANAContextBase InitialContext { get; set; }

        [DataMember]
        public LiseInput LiseData { get; set; }
    }
}
