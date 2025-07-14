using System;
using System.Windows.Controls;

using ADCEngine.View;

using UnitySC.Shared.Tools;

namespace ADCEngine
{
    ///////////////////////////////////////////////////////////////////////
    // PathStringParameter
    ///////////////////////////////////////////////////////////////////////
    [System.Reflection.Obfuscation(Exclude = true)]
    public class StringParameter : ParameterTemplate<string>
    {
        public StringParameter(ModuleBase module, string name) :
            base(module, name)
        {
            _value = new PathString();
        }

        protected override bool TryParse(string str)
        {
            Value = str;
            return true;
        }

        public String String
        {
            get { return Value; }
            set
            {
                Value = value;
                OnPropertyChanged();
                ReportChange(); // voir si necessaire ?
            }
        }

        private StringParameterExpertView _parameterUI;
        public override UserControl ParameterUI
        {
            get
            {
                if (_parameterUI == null)
                {
                    _parameterUI = new StringParameterExpertView();
                    _parameterUI.DataContext = this;
                }
                return _parameterUI;
            }
        }

        private StringParameterSimplifiedView _parameterSimplifiedUI;
        public override UserControl ParameterSimplifiedUI
        {
            get
            {
                if (_parameterSimplifiedUI == null)
                {
                    _parameterSimplifiedUI = new StringParameterSimplifiedView();
                    _parameterSimplifiedUI.DataContext = this;
                }
                return _parameterSimplifiedUI;
            }
        }

    }
}
