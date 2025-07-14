using System.Windows.Controls;

using ADCEngine.View;

namespace ADCEngine
{
    ///////////////////////////////////////////////////////////////////////
    // DoubleParameter
    ///////////////////////////////////////////////////////////////////////
    [System.Reflection.Obfuscation(Exclude = true)]
    public class DoubleParameter : ParameterTemplate<double>
    {
        public DoubleParameter(ModuleBase module, string name, double min = double.NegativeInfinity, double max = double.PositiveInfinity) :
            base(module, name)
        {
            Min = min;
            Max = max;

            if (min > 0)
                _value = min;
            if (max < 0)
                _value = max;
        }

        public double Min { get; private set; }
        public double Max { get; private set; }

        protected override bool TryParse(string str)
        {
            double x;
            bool b = double.TryParse(str, out x);
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
