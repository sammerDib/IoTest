using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Xml;

using ADC.Ressources;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Tools;

namespace ADCEngine
{
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Classe de base des paramètres, utilisée pour la réflexion 
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    [System.Reflection.Obfuscation(Exclude = true)]
    public abstract class ParameterBase : ObservableRecipient, IValueComparer
    {

        public abstract void Load(XmlNodeList parameterNodes);
        public abstract XmlElement Save(XmlNode parametersNode);
        public abstract UserControl ParameterUI { get; }
        public virtual UserControl ParameterSimplifiedUI { get { return ParameterUI; } }
        // see below for public virtual UserControl ParameterExportView { get; }
        public ModuleBase Module { get; private set; }

        public string error = null;
        /// <summary>
        /// Teste si le module est correctement configuré
        /// </summary>
        /// <returns>le message d'erreur ou null</returns>
        public virtual string Validate() { return error; }

        /// <summary>
        /// Comparaison de la valeur du parametre
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public abstract bool HasSameValue(object obj);

        /// <summary>
        /// Liste des valeurs possible pour le paramétre pour l'affichage de l'aide
        /// </summary>
        public virtual List<string> ValueList => new List<string>();

        private string _selectedOption;

        /// <summary>
        /// Valeur du paramétre séléctionné pour l'affichage de l'aide
        /// </summary>
        public string SelectedOption
        {
            get { return _selectedOption; }
            set
            {
                if (_selectedOption == value)
                    return;
                _selectedOption = value;
                OnPropertyChanged(nameof(SelectedOption));
                OnPropertyChanged(nameof(HelpValueName));
                OnPropertyChanged(nameof(SelectedOptionLabel));
            }
        }


        public string SelectedOptionLabel
        {
            get
            {
                return OptionToLabel(_selectedOption);
            }
        }

        public virtual string OptionToLabel(string option)
        {
            UIResource resource = UIResources.Instance.GetParameterValueResource(Module.Factory.ModuleName, Name, option);
            return resource != null && !string.IsNullOrEmpty(resource.UIValue) ? resource.UIValue : option;
        }

        //=================================================================
        // Constructeur
        //=================================================================
        public ParameterBase(ModuleBase module, string name)
        {
            Module = module;
            Name = name;
        }

        //=================================================================
        //
        //=================================================================
        public override string ToString()
        {
            return Label;
        }

        //=================================================================
        //
        //=================================================================
        public void ReportChange()
        {
            Module.ReportChange();
        }

        //=================================================================
        // Export View
        //=================================================================
        protected UserControl _parameterExportView = null;
        public virtual UserControl ParameterExportView
        {
            get
            {
                if (_parameterExportView == null)
                {
                    _parameterExportView = new View.ParameterExportView();
                    _parameterExportView.DataContext = this;
                }

                return _parameterExportView;
            }
        }

        //=================================================================
        // Active: indique si le paramètre peut-être modifié par l'IHM
        //=================================================================
        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled == value)
                    return;
                _isEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
            }
        }

        //=================================================================
        // Name
        //=================================================================
        public string Name { get; set; }

        protected bool _isExported;
        public bool IsExported
        {
            get { return _isExported; }
            set
            {
                if (_isExported != value)
                {
                    _isExported = value;
                    OnPropertyChanged(nameof(IsExported));
                }
            }
        }

        //=================================================================
        // Texte pour l'IHM
        // =================================================================
        private string _label;
        public string Label
        {
            get
            {
                if (_label == null)
                {
                    ParameterResource parameterResource = UIResources.Instance.GetParameterResource(Module.Factory.ModuleName, Name);
                    if (parameterResource != null && !string.IsNullOrEmpty(parameterResource.UIValue))
                    {
                        _label = parameterResource.UIValue;
                    }
                    else
                        _label = Name;
                }
                return _label;
            }
        }

        /// <summary>
        /// Nom de l'aide pour retrouver le fichier html associé
        /// </summary>
        private string _helpName;
        public string HelpName
        {
            get
            {
                if (_helpName == null)
                {
                    ParameterResource parameterResource = UIResources.Instance.GetParameterResource(Module.Factory.ModuleName, Name);
                    _helpName = parameterResource != null ? parameterResource.HelpName : string.Empty;
                }

                return _helpName;
            }
        }

        public string HelpValueName
        {
            get
            {
                UIResource parameterValueResource = UIResources.Instance.GetParameterValueResource(Module.Factory.ModuleName, Name, SelectedOption);
                return parameterValueResource != null ? parameterValueResource.HelpName : string.Empty;
            }
        }


        protected string _exportLabel;
        public string ExportLabel
        {
            get
            {
                if (_exportLabel != null && _exportLabel != "")
                    return _exportLabel;
                else
                    return Label;
            }
            set
            {
                if (_exportLabel != value)
                {
                    _exportLabel = value;
                    OnPropertyChanged(nameof(ExportLabel));
                }
            }
        }

        //=================================================================
        // Helpers pour lire / sauver les paramètres XML
        //=================================================================
        private string Info(XmlNodeList parameters)
        {
            string recipe = null;
            if (parameters.Count > 0)
            {
                XmlDocument xmldoc = parameters[0].OwnerDocument;
                recipe = xmldoc.BaseURI;
            }

            string info = "Module: \"" + Module;
            if (recipe != null && recipe != "")
                info += "\"  Recipe: \"" + recipe + "\"";
            return info;
        }

        public XmlNode ReadParameter(string label, XmlNodeList parameters)
        {
            foreach (XmlNode node in parameters)
            {
                if (node.GetAttributeValue("Label") == label)
                    return node;
            }

            string recipe = "";
            if (parameters.Count > 0)
            {
                XmlDocument xmldoc = parameters[0].OwnerDocument;
                recipe = xmldoc.BaseURI;
            }

            throw new ApplicationException("missing field: \"" + label + "\" in " + Info(parameters));
        }

        public string ReadParameterString(string label, XmlNodeList parameters)
        {
            XmlNode parameter = ReadParameter(label, parameters);

            return parameter.GetAttributeValue("Value");
        }

        public bool ReadParameterBool(string label, XmlNodeList parameters)
        {
            string str = ReadParameterString(label, parameters);

            bool x;
            bool ok = bool.TryParse(str, out x);
            if (!ok)
                throw new ApplicationException("field \"" + label + "\"must be a boolean in " + Info(parameters));

            return x;
        }

        public int ReadParameterInt(string label, XmlNodeList parameters)
        {
            string str = ReadParameterString(label, parameters);

            int x;
            bool b = int.TryParse(str, out x);
            if (!b)
                throw new ApplicationException("field \"" + label + "\"must be an integer in " + Info(parameters));

            return x;
        }

        public float ReadParameterFloat(string label, XmlNodeList parameters)
        {
            string str = ReadParameterString(label, parameters);

            float x;
            bool b = float.TryParse(str, out x);
            if (!b)
                throw new ApplicationException("field \"" + label + "\"must be a floating point number in" + Info(parameters));

            return x;
        }

        public double ReadParameterDouble(string label, XmlNodeList parameters)
        {
            string str = ReadParameterString(label, parameters);

            double x;
            bool b = double.TryParse(str, out x);
            if (!b)
                throw new ApplicationException("field \"" + label + "\"must be a floating point number in" + Info(parameters));

            return x;
        }

        public T ReadParameterEnum<T>(string label, XmlNodeList parameters) where T : struct
        {
            string str = ReadParameterString(label, parameters);

            T t;
            bool b = Enum.TryParse(str, out t);
            if (!b)
                throw new ApplicationException("field \"" + label + "\"must be an enum of type " + typeof(T) + " in" + Info(parameters));

            return (T)t;
        }

        public static XmlElement SaveParameter(XmlNode node, string label, object value)
        {
            if (value == null)
                throw new ApplicationException("value must not be null ");

            XmlDocument xmldoc = node.OwnerDocument;
            XmlElement element = xmldoc.CreateElement("Parameter");

            element.SetAttribute("Label", label);
            element.SetAttribute("Type", value.GetType().ToString());
            element.SetAttribute("Value", value.ToString());
            node.AppendChild(element);

            return element;
        }
    }
}
