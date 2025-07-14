using System.Runtime.Serialization;

using UnitySC.PM.EME.Service.Interface.Context;
using UnitySC.PM.EME.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.EME.Service.Interface.Algo
{
    public class DistanceSensorCalibrationInput : IEMEInputFlow
    {
        [DataMember]
        public EMEContextBase InitialContext { get; set; }

        public InputValidity CheckInputValidity()
        {
            return new InputValidity(true);
        }

    }
}
