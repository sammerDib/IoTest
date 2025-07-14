using System.Windows.Controls;

using ADCEngine.View;

namespace ADCEngine
{
    ///////////////////////////////////////////////////////////////////////
    // IntParameter
    ///////////////////////////////////////////////////////////////////////
    [System.Reflection.Obfuscation(Exclude = true)]
    public class IntParameter : ParameterTemplate<int>
    {
        public IntParameter(ModuleBase module, string name, int min = 0, int max = int.MaxValue) :
            base(module, name)
        {
            Min = min;
            Max = max;

            if (min > 0)
                _value = min;
            if (max < 0)
                _value = max;
        }

        public int Min { get; private set; }
        public int Max { get; private set; }

        protected override bool TryParse(string str)
        {
            int x;
            bool b = int.TryParse(str, out x);
            if (b)
                Value = x;
            return b;
        }

        private DoubleParameterExpertView _parameterUI;
        public override UserControl ParameterUI
        {
            get
            {
                if (_parameterUI == null)
                {
                    _parameterUI = new DoubleParameterExpertView();
                    _parameterUI.DataContext = this;
                }
                return _parameterUI;
            }
        }

        private DoubleParameterSimplifiedView _parameterSimplifiedUI;
        public override UserControl ParameterSimplifiedUI
        {
            get
            {
                if (_parameterSimplifiedUI == null)
                {
                    _parameterSimplifiedUI = new DoubleParameterSimplifiedView();
                    _parameterSimplifiedUI.DataContext = this;
                }
                return _parameterSimplifiedUI;
            }
        }

    }
}
