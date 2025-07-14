using System.Text;

namespace UnitySC.Equipment.Abstractions.Configuration
{
    public class ModbusConfiguration
    {
        public string IpAddress { get; set; }
        public int TcpPort { get; set; }
        public bool IsSimulated { get; set; }
        public double PollingPeriodInterval { get; set; }
        public ushort MaxSpaceBetweenWordsInRange { get; set; }
        public int ConnectionTimeout { get; set; }
        public int ConnectionRetryDelay { get; set; }
        public int ConnectionRetryNumber { get; set; }
        public string TagsConfigurationPath { get; set; }

        public ModbusConfiguration()
        {
            IpAddress                   = "127.0.0.1";
            TcpPort                     = 502;
            PollingPeriodInterval       = 100;
            IsSimulated                 = false;
            MaxSpaceBetweenWordsInRange = 1;
            ConnectionTimeout           = 50000;
            ConnectionRetryDelay        = 5000;
            ConnectionRetryNumber       = 0;
            TagsConfigurationPath       = "Configuration/XML/TagsConfiguration.xml";
        }

        public override string ToString()
        {
            var builder = new StringBuilder(base.ToString()).AppendLine();

            builder.AppendLine($"Ip Address                 : {IpAddress}");
            builder.AppendLine($"Tcp Port                   : {TcpPort}");
            builder.AppendLine($"Is simulated               : {IsSimulated}");
            builder.AppendLine($"Polling period interval    : {PollingPeriodInterval}");
            builder.AppendLine($"Max space between words    : {MaxSpaceBetweenWordsInRange}");
            builder.AppendLine($"Communicator Timeout       : {ConnectionTimeout}");
            builder.AppendLine($"Delay between retry        : {ConnectionRetryDelay}");
            builder.AppendLine($"Max retry number           : {ConnectionRetryNumber}");
            builder.AppendLine($"TagsConfiguration file path: {TagsConfigurationPath}");

            return builder.ToString();
        }
    }
}
