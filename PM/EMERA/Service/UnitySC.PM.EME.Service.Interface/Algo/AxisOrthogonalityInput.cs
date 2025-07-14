using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.EME.Service.Interface.Algo.GetZFocus;
using UnitySC.PM.EME.Service.Interface.Context;
using UnitySC.PM.EME.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.EME.Service.Interface.Algo
{
    [DataContract]
    [XmlInclude(typeof(EMEContextBase))]
    public class AxisOrthogonalityInput : IEMEInputFlow
    {

        public AxisOrthogonalityInput(GetZFocusInput getZFocusInput = null, EMEContextBase context = null)
        {
            GetZFocusInput = getZFocusInput;
            InitialContext = context;
        }

        public InputValidity CheckInputValidity()
        {
            var validity = new InputValidity(true);

            if (!(GetZFocusInput is null))
            {
                validity.ComposeWith(GetZFocusInput.CheckInputValidity());
            }

            return validity;
        }

        [DataMember]
        public EMEContextBase InitialContext { get; set; }

        [DataMember]
        public GetZFocusInput GetZFocusInput { get; set; }
    }
}
