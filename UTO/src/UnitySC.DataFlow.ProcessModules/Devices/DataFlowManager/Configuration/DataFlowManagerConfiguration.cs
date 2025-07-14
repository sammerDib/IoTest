using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.DataFlow.ProcessModules.Drivers.WCF;
using UnitySC.Equipment.Abstractions.Configuration;

namespace UnitySC.DataFlow.ProcessModules.Devices.DataFlowManager.Configuration
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class DataFlowManagerConfiguration : DeviceConfiguration
    {
        #region Properties

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public WcfConfiguration WcfConfiguration { get; set; }

        public bool UseOnlyRecipeNameAsId { get; set; }

        #endregion

        #region Overrides

        protected override void SetDefaults()
        {
            base.SetDefaults();

            WcfConfiguration = new WcfConfiguration()
            {
                WcfHostIpAddressAsString = "127.0.0.1",
                WcfHostPort = 2020,
                WcfServiceUriSegment = "DataFlowManager",
                WcfCommunicationCheckDelay = 5,
                WcfRetryNumber = 5
            };

            UseOnlyRecipeNameAsId = false;
        }

        #endregion
    }
}
