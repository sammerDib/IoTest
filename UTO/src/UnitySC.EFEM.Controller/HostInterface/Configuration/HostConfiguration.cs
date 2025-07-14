using System;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace UnitySC.EFEM.Controller.HostInterface.Configuration
{
    /// <summary>
    /// Class responsible to hold the configuration parameters of the communication with host controller.
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "")]
    public class HostConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HostConfiguration"/> class.
        /// </summary>
        public HostConfiguration()
        {
            SetDefaults();
        }

        /// <summary>
        /// Gets or sets the IP address to listen on.
        /// </summary>
        [XmlElement]
        [DataMember]
        public string IpAddress { get; set; }

        /// <summary>
        /// Gets or sets the TCP port to open.
        /// </summary>
        [XmlElement]
        [DataMember]
        public uint TcpPort { get; set; }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            SetDefaults();
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine($"<{nameof(HostConfiguration)}>");
            builder.AppendLine($"{nameof(IpAddress)}: {IpAddress}");
            builder.AppendLine($"{nameof(TcpPort)}: {TcpPort}");
            return builder.ToString();
        }

        /// <summary>
        /// Sets the default values (called on deserializing and from constructor)
        /// </summary>
        private void SetDefaults()
        {
            IpAddress = IPAddress.Any.ToString();
            TcpPort   = 5001;
        }
    }
}
