using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdcBaseComposant;
using System.Xml;


namespace BasicModules
{
    class ThresholdParameters : ModuleParametersBase
    {
        //=================================================================
        // Properties
        //=================================================================
        public double _lowCond = 0;
        public double LowCond
        {
            get { return _lowCond; }
            set
            {
                _lowCond = value;
                NotifyPropertyChanged(() => LowCond);
            }
        }

        public double _highCond = 255;
        public double HighCond
        {
            get { return _highCond; }
            set
            {
                _highCond = value;
                NotifyPropertyChanged(() => HighCond);
            }
        }

        //=================================================================
        // 
        //=================================================================
        public override void Init(XmlNodeList parameters)
        {
            _lowCond = double.Parse(ReadParameterString("LowThreshold", parameters));
            _highCond = double.Parse(ReadParameterString("HighThreshold", parameters));
        }

        //=================================================================
        //
        //=================================================================
        public override void SaveParameters(XmlNode parametersNode)
        {
            SaveParameterString(parametersNode, "LowThreshold", _lowCond);
            SaveParameterString(parametersNode, "HighThreshold", _highCond);
        }

    }
}
