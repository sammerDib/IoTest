using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.DataFlow.ProcessModules.Drivers.WCF;
using UnitySC.Equipment.Abstractions.Devices.ProcessModule.Configuration;

namespace UnitySC.DataFlow.ProcessModules.Devices.ProcessModule.UnityProcessModule.Configuration
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class UnityProcessModuleConfiguration : ProcessModuleConfiguration
    {
        #region Properties

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public WcfConfiguration WcfConfiguration { get; set; }

        #endregion

        #region Overrides

        protected override void SetDefaults()
        {
            base.SetDefaults();

            WcfConfiguration = new WcfConfiguration()
            {
                WcfHostIpAddressAsString = "127.0.0.1",
                WcfHostPort = 2030,
                WcfServiceUriSegment = "UTOPMService",
                WcfCommunicationCheckDelay = 5,
                WcfRetryNumber = 5
            };
        }

        #endregion
    }
}
