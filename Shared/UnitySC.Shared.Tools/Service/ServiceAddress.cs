using System.Xml.Serialization;

namespace UnitySC.Shared.Tools.Service
{
    public class ServiceAddress
    {
        [XmlAttribute("Port")]
        public int Port { get; set; }

        [XmlAttribute("Host")]
        public string Host { get; set; }

        public override string ToString()
        {
            return $"Host: {Host} Port: {Port}";
        }
    }
}
