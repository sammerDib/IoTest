using System.Windows.Controls;

using ADCEngine.View;

namespace ADCEngine
{
    ///////////////////////////////////////////////////////////////////////
    // BoolParameter
    ///////////////////////////////////////////////////////////////////////
    [System.Reflection.Obfuscation(Exclude = true)]
    public class BoolParameter : ParameterTemplate<bool>
    {
        public BoolParameter(ModuleBase module, string name) : base(module, name) { }

        protected override bool TryParse(string str)
        {
            bool x;
            bool b = bool.TryParse(str, out x);
            if (b)
                Value = x;
            return b;
        }

        private BoolParameterExpertView _parameterUI;
        public override UserControl ParameterUI
        {
            get
            {
                if (_parameterUI == null)
                {
                    _parameterUI = new BoolParameterExpertView();
                    _parameterUI.DataContext = this;
                }
                return _parameterUI;
            }
        }

        private BoolParameterSimplifiedView _parameterSimplifiedUI;
        public override UserControl ParameterSimplifiedUI
        {
            get
            {
                if (_parameterSimplifiedUI == null)
                {
                    _parameterSimplifiedUI = new BoolParameterSimplifiedView();
                    _parameterSimplifiedUI.DataContext = this;
                }
                return _parameterSimplifiedUI;
            }
        }

    }
}
