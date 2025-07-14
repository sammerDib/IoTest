using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Flow;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    [XmlInclude(typeof(ANAContextBase))]
    public class AutofocusInput : IANAInputFlow
    {
        public AutofocusInput()
        { }

        public AutofocusInput(AutoFocusSettings settings, ANAContextBase initialContext = null)
        {
            Settings = settings;
            InitialContext = initialContext;
        }

        public InputValidity CheckInputValidity()
        {
            var validity = new InputValidity(true);
            validity.ComposeWith(Settings.CheckInputValidity());
            return validity;
        }

        [DataMember]
        public ANAContextBase InitialContext { get; set; }

        [DataMember]
        public AutoFocusSettings Settings { get; set; }
    }
}
