using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.EME.Service.Interface.Context;
using UnitySC.PM.EME.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.EME.Service.Interface.Algo
{
    [DataContract]

    [XmlInclude(typeof(EMEContextBase))]
    public class DistortionInput : IEMEInputFlow
    {
        public DistortionInput()
        {
        }

        public DistortionInput(double gaussianSigma)
        {
            GaussianSigma = gaussianSigma;
        }

        public InputValidity CheckInputValidity()
        {
            return new InputValidity(true);
        }

        [DataMember]
        public EMEContextBase InitialContext { get; set; }

        [DataMember]
        public double GaussianSigma { get; set; }
    }
}
