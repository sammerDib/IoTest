using System.Runtime.Serialization;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Units;


namespace UnitySC.PM.ANA.Service.Interface.Profile1D
{
    [DataContract]
    public class Profile1DInput : IANAInputFlow
    {
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

            if (Speed.MillimetersPerSecond == 0.0)
            {
                validity.IsValid = false;
                validity.Message.Add($"The speed must not be 0.");
            }

            if (StartPosition.X == EndPosition.X && StartPosition.Y != EndPosition.Y)
            {
                validity.IsValid = false;
                validity.Message.Add($"The start and end position must not be equal.");
            }
            
            return validity;
        }

        [DataMember]
        public ANAContextBase InitialContext { get; set; }
        
        [DataMember]
        public LiseInput LiseData { get; set; }
        
        [DataMember]
        public Speed Speed { get; set; }
        
        [DataMember]
        public XYPosition StartPosition { get; set; }
        
        [DataMember]
        public XYPosition EndPosition { get; set; }
    }
}
