using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

using ADC.Ressources;

using ADCEngine.View;

using UnitySC.Shared.Tools;

namespace ADCEngine
{
    ///////////////////////////////////////////////////////////////////////
    // EnumParameter
    ///////////////////////////////////////////////////////////////////////
    [System.Reflection.Obfuscation(Exclude = true)]
    public class EnumParameter<T> : ParameterTemplate<T> where T : struct
    {
        //=================================================================
        // Constructeur
        //=================================================================
        public EnumParameter(ModuleBase module, string name) :
            base(module, name)
        {
            Type = typeof(T);
            _value = (T)Enum.Parse(Type, "0"); // TODO Est-ce qu'il y a plus simple pour initialiser un enum ?
        }

        //=================================================================
        // 
        //=================================================================
        protected override bool TryParse(string str)
        {
            T t;
            bool b = Enum.TryParse(str, out t);
            if (b)
                Value = t;
            return b;
        }

        //=================================================================
        // Liste des valeurs possibles
        // à utiliser avec DisplayMemberPath="Value" SelectedValuePath="Key" dans le XAML
        //=================================================================
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

        public override string OptionToLabel(string option)
        {
            UIResource resource = UIResources.Instance.GetParameterValueResource(Module.Factory.ModuleName, Name, option);
            if (resource != null && !string.IsNullOrEmpty(resource.UIValue))
            {
                return resource.UIValue;
            }
            else
            {
                foreach (var enumValue in Enum.GetValues(Type).Cast<Enum>())
                {
                    if (enumValue.ToString() == option)
                        return enumValue.GetDescription();
                }

                return option;
            }
        }

        //=================================================================
        // IHM
        //=================================================================
        private EnumParameterExpertView _parameterUI;
        public override UserControl ParameterUI
        {
            get
            {
                if (_parameterUI == null)
                {
                    _parameterUI = new EnumParameterExpertView();
                    _parameterUI.DataContext = this;
                }
                return _parameterUI;
            }
        }

        private EnumParameterSimplifiedView _parameterSimplifiedUI;
        public override UserControl ParameterSimplifiedUI
        {
            get
            {
                if (_parameterSimplifiedUI == null)
                {
                    _parameterSimplifiedUI = new EnumParameterSimplifiedView();
                    _parameterSimplifiedUI.DataContext = this;
                }
                return _parameterSimplifiedUI;
            }
        }

        /// <summary>
        /// Liste des valeurs disponible pour l'affichage de l'aide
        /// </summary>
        public override List<string> ValueList => Enum.GetNames(typeof(T)).ToList();

    }
}
