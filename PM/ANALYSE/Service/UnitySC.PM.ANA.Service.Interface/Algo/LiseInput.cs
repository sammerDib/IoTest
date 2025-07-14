using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    public class LiseInput
    {
        public LiseInput() 
        {
        }

        public LiseInput(string probeId, double gain = double.NaN, int nbAveraging = 1)
        {
            ProbeID = probeId;
            Gain = gain;
            NbAveraging = nbAveraging;
        }

        [DataMember]
        public string ProbeID { get; set; }

        [DataMember]
        public double Gain { get; set; }

        [DataMember]
        public int NbAveraging { get; set; }

        public virtual InputValidity CheckInputValidity()
        {
            var validity = new InputValidity(true);

            if (ProbeID is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The probe ID is missing.");
            }

            if (NbAveraging <= 0)
            {
                validity.IsValid = false;
                validity.Message.Add($"The number of averaging can't be 0 or negative.");
            }

            return validity;
        }
    }
}
