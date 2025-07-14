using System.Windows.Controls;

using DeepLearningSoft48.Modules.Parameters.Views;

namespace DeepLearningSoft48.Modules.Parameters
{
    public class IntParameter : ParameterTemplate<int>
    {
        //====================================================================
        // Properties
        //====================================================================
        public double Min { get; private set; }
        public double Max { get; private set; }

        //====================================================================
        // Constructor
        //====================================================================
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
