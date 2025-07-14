using System.Windows.Controls;

using DeepLearningSoft48.Modules.Parameters.Views;

namespace DeepLearningSoft48.Modules.Parameters
{
    public class DoubleParameter : ParameterTemplate<double>
    {
        //====================================================================
        // Properties
        //====================================================================
        public double Min { get; private set; }
        public double Max { get; private set; }

        //====================================================================
        // Constructor
        //====================================================================
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

        //====================================================================
        // HMI View
        //====================================================================
        private DoubleParameterView _parameterUI;
        public override UserControl ParameterUI
        {
            get
            {
                if (_parameterUI == null)
                {
                    _parameterUI = new DoubleParameterView();
                    _parameterUI.DataContext = this;
                }
                return _parameterUI;
            }
        }
    }
}
