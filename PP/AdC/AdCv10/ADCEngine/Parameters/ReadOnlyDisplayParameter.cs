using System;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using ADCEngine.View;
using ADCEngine.Parameters.View.ExpertView;
using System.Runtime.InteropServices;

namespace ADCEngine.Parameters
{

    public class ReadOnlyDisplayParameter : ParameterBase
    {
        //=================================================================
        // Constructeur
        //=================================================================
        public ReadOnlyDisplayParameter(ModuleBase module, string label) :
            base(module, label)
        {
        }

        //=================================================================
        // XML Load
        //=================================================================
        public override void Load(XmlNodeList parameterNodes)
        {
        }

        protected string _value;
        public string DisplayString
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged();
                ReportChange(); // voir si necessaire ?
                SelectedOption = _value;
            }
        }

        //=================================================================
        // XML Save
        //=================================================================
        public override XmlElement Save(XmlNode parametersNode)
        {
            return null;
        }

        public override bool HasSameValue(object obj)
        {
            var displayParam = obj as ReadOnlyDisplayParameter;
            return displayParam != null &&
                Name == displayParam.Name &&
                DisplayString == displayParam.DisplayString;
        }

        //=================================================================
        // IHM
        //=================================================================
        private ReadOnlyDisplayParameterView _parameterUI;
        public override UserControl ParameterUI
        {
            get
            {
                if (_parameterUI == null)
                {
                    _parameterUI = new ReadOnlyDisplayParameterView();
                    _parameterUI.DataContext = this;
                }
                return _parameterUI;
            }
        }

        private ReadOnlyDisplayParameterView _parameterSimplifiedUI;
        public override UserControl ParameterSimplifiedUI
        {
            get
            {
                if (_parameterSimplifiedUI == null)
                {
                    _parameterSimplifiedUI = new ReadOnlyDisplayParameterView();
                    _parameterSimplifiedUI.DataContext = this;
                }
                return _parameterSimplifiedUI;
            }
        }
    }
}
