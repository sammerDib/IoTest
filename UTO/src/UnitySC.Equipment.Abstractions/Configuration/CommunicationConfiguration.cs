using System.Net;
using System.Text;
using System.Xml.Serialization;

using Newtonsoft.Json;

namespace UnitySC.Equipment.Abstractions.Configuration
{
    /// <summary>
    /// Class containing TCP communication parameters.
    /// </summary>
    public class CommunicationConfiguration
    {
        /// <summary>
        /// Define the communication mode: client or server.
        /// </summary>
        public ConnectionMode ConnectionMode { get; set; }

        /// <summary>
        /// <see cref="IPAddress"/> is not serializable as typed.
        /// <see cref="IpAddressAsString"/> is used only for serializing it.
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public IPAddress IpAddress { get; set; }

        /// <summary>
        /// <see cref="IPAddress"/> is not a serializable type.
        /// <see cref="IpAddressAsString"/> is used only for serializing it.
        /// </summary>
        [XmlElement(nameof(IpAddress))]
        public string IpAddressAsString
        {
            get => IpAddress.ToString();
            set
            {
                if (IPAddress.TryParse(value, out IPAddress address))
                {
                    IpAddress = address;
                }
            }
        }

        public uint TcpPort { get; set; }

        /// <summary>
        /// Gets or sets the value indicating the answer timeout in ms.
        /// </summary>
        public uint AnswerTimeout { get; set; }

        /// <summary>
        /// Gets or sets the value indicating the confirmation timeout in ms.
        /// </summary>
        public uint ConfirmationTimeout { get; set; }

        /// <summary>
        /// Gets or sets the value indicating the initialization timeout in ms.
        /// </summary>
        public uint InitTimeout { get; set; }

        public int CommunicatorId { get; set; }

        public int MaxNbRetry { get; set; }

        public int ConnectionRetryDelay { get; set; }

        public uint AliveBitPeriod { get; set; }

        public CommunicationConfiguration()
        {
            ConnectionMode       = ConnectionMode.Server;
            IpAddress            = IPAddress.Parse("0.0.0.0");
            TcpPort              = 12000;
            AnswerTimeout        = 180000;
            InitTimeout          = 180000;
            ConfirmationTimeout  = 500;
            CommunicatorId       = 0;
            MaxNbRetry           = 0;
            ConnectionRetryDelay = 5000;
            AliveBitPeriod = 1000;
        }

        public override string ToString()
        {
            var builder = new StringBuilder(base.ToString()).AppendLine();

            builder.AppendLine($"Connection Mode     : {ConnectionMode}");
            builder.AppendLine($"Ip Address          : {IpAddress}");
            builder.AppendLine($"Tcp Port            : {TcpPort}");
            builder.AppendLine($"Answer Timeout      : {AnswerTimeout}");
            builder.AppendLine($"Confirmation Timeout: {ConfirmationTimeout}");
            builder.AppendLine($"Init Timeout        : {InitTimeout}");
            builder.AppendLine($"Communicator Id     : {CommunicatorId}");
            builder.AppendLine($"Max retry number    : {MaxNbRetry}");
            builder.AppendLine($"Delay between retry : {ConnectionRetryDelay}");
            builder.AppendLine($"Alive bit period    : {AliveBitPeriod}");

            return builder.ToString();
        }
    }
}
