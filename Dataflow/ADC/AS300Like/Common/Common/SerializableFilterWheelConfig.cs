using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Serialization;

namespace Common
{
    public struct FilterWheelLabel
    {
        public int Position;
        public string Label;
    }

    public class SerializableFilterWheelConfig:XmlSerializableObject<SerializableFilterWheelConfig>
    {
        
        public int ID;
        public String ComPort;
        public int BaudRate;
        public int ComTimeoutMS;
        public int NbCOMRetries;
        public int PollingRateMS;
        public List<FilterWheelLabel> Labels;
        [XmlIgnore]
        public Dictionary<int, String> PositionLabels
        {
            get;
            set;
        }

        public static new SerializableFilterWheelConfig ReadFromFile(String XmlFilePath)
        {
            SerializableFilterWheelConfig Config = XmlSerializableObject<SerializableFilterWheelConfig>.ReadFromFile(XmlFilePath);
            Config.PositionLabels = new Dictionary<int, string>();
            foreach(FilterWheelLabel Label in Config.Labels)
            {
                Config.PositionLabels.Add(Label.Position, Label.Label);
            }
            return Config;
        }

    }
}
