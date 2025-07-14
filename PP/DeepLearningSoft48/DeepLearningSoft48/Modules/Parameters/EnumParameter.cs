using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

using DeepLearningSoft48.Modules.Parameters.Views;
using DeepLearningSoft48.Resources;

using UnitySC.Shared.Tools;

namespace DeepLearningSoft48.Modules.Parameters
{
    public class EnumParameter<T> : ParameterTemplate<T> where T : struct
    {
        //====================================================================
        // Constructor
        //====================================================================
        public EnumParameter(ModuleBase module, string name) :
            base(module, name)
        {
            Type = typeof(T);
            _value = (T)Enum.Parse(Type, "0");
        }

        //====================================================================
        // List of possible values
        // to be used with DisplayMemberPath="Value" SelectedValuePath="Key"
        // in XAML.
        //====================================================================
        public IEnumerable<KeyValuePair<Enum, string>> EnumList
        {
            get
            {
                List<KeyValuePair<Enum, string>> values = new List<KeyValuePair<Enum, string>>();
                foreach (var enumValue in Enum.GetValues(Type).Cast<Enum>())
                {
                    UIResource resource = UIResources.Instance.GetParameterValueResource(Module.Factory.ModuleName, Name, enumValue.ToString());
                    string displayValue = resource != null && !string.IsNullOrEmpty(resource.UIValue) ? resource.UIValue : enumValue.GetDescription();
                    values.Add(new KeyValuePair<Enum, string>(enumValue, displayValue));
                }

                return values;
            }
        }

        //====================================================================
        // HMI View
        //====================================================================
        private EnumParameterView _parameterUI;
        public override UserControl ParameterUI
        {
            get
            {
                if (_parameterUI == null)
                {
                    _parameterUI = new EnumParameterView();
                    _parameterUI.DataContext = this;
                }
                return _parameterUI;
            }
        }
        public override List<string> ValueList => Enum.GetNames(typeof(T)).ToList();
    }
}
