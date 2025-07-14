using System;
using System.Linq;
using System.Windows.Controls;

using ADCEngine;

namespace BasicModules.Mathematic
{
    public class FirstOperandParameter : IntParameter
    {
        public int FirstOperandIndex { get { return Value; } set { Value = value; } }

        //=================================================================
        // Constructeur
        //=================================================================
        public FirstOperandParameter(ModuleBase module, string name)
            : base(module, name)
        {
        }

        //=================================================================
        // 
        //=================================================================
        public override string Validate()
        {
            if (FirstOperandIndex < 0 || Module.Parents.Count() <= FirstOperandIndex)
                return "First operand is an invalid branch";

            return null;
        }

        //=================================================================
        // UI
        //=================================================================
        private FirstOperandParameterView _parameterView;
        private FirstOperandParameterViewModel _parameterViewModel;
        public override UserControl ParameterUI
        {
            get
            {
                if (_parameterView == null)
                {
                    _parameterView = new FirstOperandParameterView();
                    _parameterViewModel = new FirstOperandParameterViewModel(this);
                    _parameterView.DataContext = _parameterViewModel;
                }
                return _parameterView;
            }
        }

        public override UserControl ParameterSimplifiedUI
        {
            get
            {
                throw new ApplicationException("no simplfied view for parameter: " + Label);
            }
        }

    }
}
