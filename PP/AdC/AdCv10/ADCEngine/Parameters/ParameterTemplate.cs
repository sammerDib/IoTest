using System;
using System.Collections.Generic;
using System.Xml;

using UnitySC.Shared.Tools;

namespace ADCEngine
{
    ///////////////////////////////////////////////////////////////////////
    //
    // ParameterTemplate
    //
    // Représentation générique d'un paramètre d'un module.
    // En plus de stocker la valeur, on gère la lecture écriture dans le XML,
    // le fait que le paramètres soit exporté ou non, son label pour l'export...
    ///////////////////////////////////////////////////////////////////////
    [System.Reflection.Obfuscation(Exclude = true)]
    public abstract class ParameterTemplate<T> : ParameterBase
    {
        //=================================================================
        // Méthodes abstraites
        //=================================================================
        protected abstract bool TryParse(string str);

        //=================================================================
        // Constructeur
        //=================================================================
        public ParameterTemplate(ModuleBase module, string name)
            : base(module, name)
        {
        }

        //=================================================================
        // Value et Type
        //=================================================================

        private Type _type = null;
        public Type Type
        {
            get
            {
                if (_type == null)
                    _type = _value.GetType();
                return _type;
            }
            protected set
            {
                _type = value;
            }
        }

        protected T _value;
        public virtual T Value
        {
            get
            {
                return _value;
            }
            set
            {
                T oldvalue = _value;
                if (oldvalue.Equals(value) == false)
                {
                    _value = value;

                    if (_value.Equals(oldvalue) == false)
                    {
                        if (ValueChanged != null)
                            ValueChanged(_value);
                        OnPropertyChanged(nameof(Value));
                        ReportChange();
                        SelectedOption = _value.ToString();
                    }
                }
            }
        }

        //=================================================================
        // Callbacks pour ValueChanged
        //=================================================================
        public delegate void ValueChangedEventHandler(T t);
        public event ValueChangedEventHandler ValueChanged;

        //=================================================================
        // Opérateur de conversion
        //=================================================================
        public static implicit operator T(ParameterTemplate<T> p)
        {
            return p.Value;
        }

        //=================================================================
        // XML Load
        //=================================================================
        public override void Load(XmlNodeList parameterNodes)
        {
            XmlNode node = ReadParameter(Name, parameterNodes);
            if (node != null)
            {
                Load(node);
            }
        }

        protected void Load(XmlNode parameterNode)
        {
            string str = parameterNode.Attributes["Label"].Value;
            if (str != Name)
                throw new ApplicationException("parsing an invalid parameter param.Label=" + Name + " xml.Label=" + str);

            str = parameterNode.GetAttributeValue("Value");
            bool b = TryParse(str);
            if (!b)
                throw new ApplicationException("field \"" + Name + "\"must be of type " + Type);

            IsExported = false;
            if (parameterNode.Attributes["Exported"] != null)
                IsExported = bool.Parse(parameterNode.Attributes["Exported"].Value);

            if (IsExported)
                ExportLabel = parameterNode.GetAttributeValue("ExportLabel");
        }

        //=================================================================
        // XML Save
        //=================================================================
        public override XmlElement Save(XmlNode parametersNode)
        {
            XmlDocument xmldoc = parametersNode.OwnerDocument;
            XmlElement element = xmldoc.CreateElement("Parameter");

            element.SetAttribute("Label", Name);
            element.SetAttribute("Type", Type.ToString());
            element.SetAttribute("Value", Value.ToString());
            if (IsExported)
            {
                element.SetAttribute("Exported", "True");
                element.SetAttribute("ExportLabel", ExportLabel);
            }
            parametersNode.AppendChild(element);

            return element;
        }

        //=================================================================
        // ToString
        //=================================================================
        public override string ToString()
        {
            return "Param " + Name + "=" + Value.ToString();
        }

        public override bool HasSameValue(object obj)
        {
            var template = obj as ParameterTemplate<T>;
            return template != null &&
                Name == template.Name &&
                EqualityComparer<Type>.Default.Equals(_type, template._type) &&
                EqualityComparer<T>.Default.Equals(_value, template._value);
        }
    }
}
