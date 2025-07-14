using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Media;
using System.Xml.Serialization;

using UnitySC.Shared.Tools.Collection;

namespace UnitySC.PM.DMT.Service.Interface.Measure.Configuration
{
    [Serializable]
    [DataContract]
    public class BrightFieldMeasureConfiguration : MeasureConfigurationBase
    {
        private List<Color> _colorsList;

        [XmlElement("Color")] 
        public List<string> ColorNames;

        [XmlIgnore]
        public List<Color> Colors
        {
            get
            {
                if (_colorsList.IsNullOrEmpty())
                {
                    _colorsList = ColorNames.Select(name => (Color)ColorConverter.ConvertFromString(name)).ToList();
                }

                return _colorsList;
            }
        }
    }
}
