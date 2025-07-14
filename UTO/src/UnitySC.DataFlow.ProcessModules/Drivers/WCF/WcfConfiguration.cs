using System;
using System.Net;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using Newtonsoft.Json;

namespace UnitySC.DataFlow.ProcessModules.Drivers.WCF
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class WcfConfiguration
    {
        #region Properties

        /// <summary>
        /// <see cref="IPAddress"/> is not serializable as typed.
        /// <see cref="WcfHostIpAddressAsString"/> is used only for serializing it.
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public IPAddress WcfHostIpAddress { get; set; }

        /// <summary>
        /// <see cref="IPAddress"/> is not a serializable type.
        /// <see cref="WcfHostIpAddress"/> is used only for serializing it.
        /// </summary>
        [XmlElement(nameof(WcfHostIpAddress))]
        public string WcfHostIpAddressAsString
        {
            get => WcfHostIpAddress.ToString();
            set
            {
                if (IPAddress.TryParse(value, out IPAddress address))
                {
                    WcfHostIpAddress = address;
                }
            }
        }

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public uint WcfHostPort { get; set; }

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public string WcfServiceUriSegment { get; set; }

        /// <summary>
        /// Delay between 2 AreYouThere request (in seconds)
        /// </summary>
        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public uint WcfCommunicationCheckDelay { get; set; }

        [XmlElement(IsNullable = false)]
        [DataMember(EmitDefaultValue = false)]
        public uint WcfRetryNumber { get; set; }
        #endregion
    }
}
