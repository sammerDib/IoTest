using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.EME.Service.Interface.Algo.GetZFocus;
using UnitySC.PM.EME.Service.Interface.Context;
using UnitySC.PM.EME.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Interface.Algo
{
    [DataContract]
    [XmlInclude(typeof(EMEContextBase))]
    public class MultiSizeChuckInput : IEMEInputFlow
    {
        public MultiSizeChuckInput()
        {
        }

        public MultiSizeChuckInput(Length waferDiameter, GetZFocusInput getZFocusInput = null, EMEContextBase context = null)
        {
            WaferDiameter = waferDiameter;
            GetZFocusInput = getZFocusInput;
            InitialContext = context;
        }

        public MultiSizeChuckInput(EMEContextBase context)
        {
            InitialContext = context;
        }

        public InputValidity CheckInputValidity()
        {
            return new InputValidity(true);
        }

        [DataMember]
        public EMEContextBase InitialContext { get; set; }

        [DataMember]
        public GetZFocusInput GetZFocusInput { get; set; }

        [DataMember]
        public Length WaferDiameter { get; set; }
    }
}
