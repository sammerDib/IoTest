using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.DMT.Service.Interface.Measure.Outputs;
using UnitySC.Shared.Tools.Collection;

namespace UnitySC.PM.DMT.Service.Interface.Measure.Configuration
{
    [Serializable]
    public class DeflectometryMeasureConfiguration : MeasureConfigurationBase
    {
        private List<int> _periodsList;

        private List<int> _numberOfImagesPerDirectionList;
        
        public string Periods;

        public string NumberOfImagesPerDirection;

        public double FringesMaxValue;

        public int GlobalTopoPointSize = 16;

        public DeflectometryOutput AvailableOutputs;
        
		[XmlIgnore]
        public List<int> PeriodList {
            get
            {
                if (_periodsList.IsNullOrEmpty())
                {
                    _periodsList = Periods.Split(',').Select(period => int.Parse(period.Trim())).ToList();
                }
                
                return _periodsList;
            }
        }

        [XmlIgnore]
        public List<int> NumberOfImagesPerDirectionList
        {
            get
            {
                if (_numberOfImagesPerDirectionList.IsNullOrEmpty())
                {
                    _numberOfImagesPerDirectionList = NumberOfImagesPerDirection.Split(',').Select(number => int.Parse(number.Trim())).ToList();
                }
                
                return _numberOfImagesPerDirectionList;
            }
        }
    }
}
