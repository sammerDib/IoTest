using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.DMT.Service.Interface.Flow
{
    public class ComputePhaseMapAndMaskForPeriodAndDirectionConfiguration : FlowConfigurationBase
    {
        public override FlowReportConfiguration WriteReportMode { get; set; }
        
        [XmlElement("EnhancedMaskEdgeFilterConfiguration")]
        public List<EnhancedMaskEdgeFilterConfiguration> EnhancedMaskEdgeFilterConfigurations { get; set; }

        private Dictionary<Side, Dictionary<Length, Length>> _edgeFilterByDiameterAndSide;

        [XmlIgnore]
        public Dictionary<Side, Dictionary<Length, Length>> EdgeFilterByDiameterAndSide
        {
            get
            {
                if (_edgeFilterByDiameterAndSide is null || _edgeFilterByDiameterAndSide.Count != EnhancedMaskEdgeFilterConfigurations.GroupBy(item => item.Side).Count())
                {
                    _edgeFilterByDiameterAndSide = EnhancedMaskEdgeFilterConfigurations
                        .GroupBy(item => item.Side)
                        .ToDictionary(
                            groupItem => groupItem.Key,
                            groupItem => groupItem.ToDictionary(
                                listItem => listItem.WaferDiameter,
                                listItem => listItem.FilterEdgeWidth));
                }
                
                return _edgeFilterByDiameterAndSide;
            }
        }

        [Serializable]
        public class EnhancedMaskEdgeFilterConfiguration 
        {
            [XmlAttribute]
            public Side Side { get; set; }
            
            public Length WaferDiameter { get; set; }
            
            public Length FilterEdgeWidth { get; set; }
        }
    }
}
