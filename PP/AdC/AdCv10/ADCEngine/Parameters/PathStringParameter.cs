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
    public class PathStringParameter : ParameterTemplate<PathString>
    {
        public enum Operation { Open, Save, Folder };
        public Operation operation;

        public PathStringParameter(ModuleBase module, string name, Operation operation, string filter = null) :
            base(module, name)
        {
            _value = new PathString();
            Filter = filter;
            this.operation = operation;
        }

        protected override bool TryParse(string str)
        {
            Value = str;
            return true;
        }

        public static implicit operator String(PathStringParameter p)
        {
            return p.Value;
        }

        public String String
        {
            get { return Value; }
            set
            {
                Value = value;
                OnPropertyChanged();
                ReportChange(); // à voir si necessaire
            }
        }

        public string Filter { get; set; }

        private PathStringParameterExpertView _parameterUI;
        public override UserControl ParameterUI
        {
            get
            {
                if (_parameterUI == null)
                {
                    _parameterUI = new PathStringParameterExpertView();
                    _parameterUI.DataContext = this;
                }
                return _parameterUI;
            }
        }

        private PathStringParameterSimplifiedView _parameterSimplifiedUI;
        public override UserControl ParameterSimplifiedUI
        {
            get
            {
                if (_parameterSimplifiedUI == null)
                {
                    _parameterSimplifiedUI = new PathStringParameterSimplifiedView();
                    _parameterSimplifiedUI.DataContext = this;
                }
                return _parameterSimplifiedUI;
            }
        }

    }
}
