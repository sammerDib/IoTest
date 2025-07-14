using System;
using System.Windows.Controls;
using System.Xml;

using ADCEngine.View;

using UnitySC.Shared.Tools;

namespace ADCEngine
{
    ///////////////////////////////////////////////////////////////////////
    // Un int plus un booléen qui indique si on utilise le double
    ///////////////////////////////////////////////////////////////////////
    [System.Reflection.Obfuscation(Exclude = true)]
    public class ConditionalIntParameter : IntParameter
    {
        public delegate void UseChangedEventHandler(bool b);
        public event UseChangedEventHandler UseChanged;

        //=================================================================
        // IsUsed
        //=================================================================
        private bool _isUsed;
        public bool IsUsed
        {
            get { return _isUsed; }
            set
            {
                if (_isUsed != value)
                {
                    _isUsed = value;
                    OnPropertyChanged(nameof(IsUsed));

                    UseChanged?.Invoke(_isUsed);
                }
            }
        }

        //=================================================================
        // Constructeur
        //=================================================================
        public ConditionalIntParameter(ModuleBase module, string name, int min = int.MinValue, int max = int.MaxValue) :
            base(module, name, min, max)
        {
        }

        //=================================================================
        // XML Load
        //=================================================================
        public override void Load(XmlNodeList parameterNodes)
        {
            // Parse parameterBase
            //....................
            XmlNode node = ReadParameter(Name, parameterNodes);
            base.Load(node);

            // Parse IsUsed
            //.............
            bool x;
            string str = node.GetAttributeValue("IsUsed");
            bool b = Boolean.TryParse(str, out x);
            if (!b)
                throw new ApplicationException("invalid attribute \"IsUsed\" on field \"" + Name + "\" : " + str);
            IsUsed = x;
        }

        //=================================================================
        // XML Save
        //=================================================================
        public override XmlElement Save(XmlNode parametersNode)
        {
            XmlDocument xmldoc = parametersNode.OwnerDocument;
            XmlElement element = base.Save(parametersNode);

            element.SetAttribute("IsUsed", IsUsed.ToString());

            return element;
        }


        //=================================================================
        // IHM
        //=================================================================
        private ConditionalDoubleParameterExpertView _parameterUI;
        public override UserControl ParameterUI
        {
            get
            {
                if (_parameterUI == null)
                {
                    _parameterUI = new ConditionalDoubleParameterExpertView();
                    _parameterUI.DataContext = this;
                }
                return _parameterUI;
            }
        }

        private ConditionalDoubleParameterSimplifiedView _parameterSimplifiedUI;
        public override UserControl ParameterSimplifiedUI
        {
            get
            {
                if (_parameterSimplifiedUI == null)
                {
                    _parameterSimplifiedUI = new ConditionalDoubleParameterSimplifiedView();
                    _parameterSimplifiedUI.DataContext = this;
                }
                return _parameterSimplifiedUI;
            }
        }

    }
}
