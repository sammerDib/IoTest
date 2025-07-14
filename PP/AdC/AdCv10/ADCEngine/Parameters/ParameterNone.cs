using System.Windows.Controls;
using System.Xml;

using ADCEngine.View;

namespace ADCEngine
{
    internal class ParameterNone : ParameterBase
    {
        public ParameterNone(ModuleBase module)
            : base(module, name: "None")
        {
        }

        public override void Load(XmlNodeList parameterNodes) { }
        public override XmlElement Save(XmlNode parametersNode) { return null; }

        public override bool HasSameValue(object obj)
        {
            var none = obj as ParameterNone;
            return none != null;
        }

        private ParameterNoneView _parameterUI;
        public override UserControl ParameterUI
        {
            get
            {
                if (_parameterUI == null)
                {
                    _parameterUI = new ParameterNoneView();
                    _parameterUI.DataContext = this;
                }
                return _parameterUI;
            }
        }

    }
}
