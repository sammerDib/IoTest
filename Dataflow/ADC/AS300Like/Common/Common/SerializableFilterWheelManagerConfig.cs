using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Common
{
    public class SerializableFilterWheelManagerConfig : XmlSerializableObject<SerializableFilterWheelManagerConfig>
    {
        public List<SerializableFilterWheelConfig> FilterWheels;
        [XmlIgnore]
        public Dictionary<int, SerializableFilterWheelConfig> Filter_Wheels
        {
            get;
            set;
        }

        public static new SerializableFilterWheelManagerConfig ReadFromFile(string XmlFilePath)
        {
            SerializableFilterWheelManagerConfig Conf = XmlSerializableObject<SerializableFilterWheelManagerConfig>.ReadFromFile(XmlFilePath);

            Conf.Filter_Wheels = new Dictionary<int, SerializableFilterWheelConfig>();
            foreach (SerializableFilterWheelConfig config in Conf.FilterWheels)
            {
                config.PositionLabels = new Dictionary<int, string>();
                foreach (FilterWheelLabel Label in config.Labels)
                {
                    config.PositionLabels.Add(Label.Position, Label.Label);
                }
                Conf.Filter_Wheels.Add(config.ID, config);
            }
            return Conf;
        }

        public override bool SaveToFile(string XmlFilePath)
        {
            this.FilterWheels.Clear();
            this.FilterWheels.AddRange(Filter_Wheels.Values);
            return base.SaveToFile(XmlFilePath);
        }
    }
}
