using System.Windows.Controls;
using System.Xml;

using ADCEngine.Parameters.View.ExpertView;

namespace ADCEngine.Parameters
{
    public class SeparatorParameter : ParameterBase
    {
        //=================================================================
        // Constructeur
        //=================================================================
        public SeparatorParameter(ModuleBase module, string label) :
            base(module, label)
        {
        }

        //=================================================================
        // XML Load
        //=================================================================
        public override void Load(XmlNodeList parameterNodes)
        {
        }

        //=================================================================
        // XML Save
        //=================================================================
        public override XmlElement Save(XmlNode parametersNode)
        {
            return null;
        }

        public override bool HasSameValue(object obj)
        {
            var none = obj as SeparatorParameter;
            return none != null;
        }

        //=================================================================
        // IHM
        //=================================================================
        private SeparatorParameterView _parameterUI;
        public override UserControl ParameterUI
        {
            get
            {
                if (_parameterUI == null)
                {
                    _parameterUI = new SeparatorParameterView();
                    _parameterUI.DataContext = this;
                }
                return _parameterUI;
            }
        }

        private SeparatorParameterView _parameterSimplifiedUI;
        public override UserControl ParameterSimplifiedUI
        {
            get
            {
                if (_parameterSimplifiedUI == null)
                {
                    _parameterSimplifiedUI = new SeparatorParameterView();
                    _parameterSimplifiedUI.DataContext = this;
                }
                return _parameterSimplifiedUI;
            }
        }
    }
}
