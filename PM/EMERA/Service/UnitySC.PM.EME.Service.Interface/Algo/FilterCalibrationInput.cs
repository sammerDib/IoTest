using System.Collections.Generic;
using System.Runtime.Serialization;

using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.PM.EME.Service.Interface.Context;
using UnitySC.PM.EME.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools.Collection;

namespace UnitySC.PM.EME.Service.Interface.Algo
{
    public class FilterCalibrationInput : IEMEInputFlow
    {
        [DataMember]
        public EMEContextBase InitialContext { get; set; }

        public List<Filter> Filters { get; set; }

        public InputValidity CheckInputValidity()
        {
            var validity = new InputValidity(true);

            if (Filters.IsNullOrEmpty())
            {
                validity.IsValid = false;
                validity.Message.Add("No filters found.");
            }

            return validity;
        }
    }
}
