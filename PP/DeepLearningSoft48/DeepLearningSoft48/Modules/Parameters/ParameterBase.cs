using System.Collections.Generic;
using System.Windows.Controls;

using CommunityToolkit.Mvvm.ComponentModel;

using DeepLearningSoft48.Resources;

namespace DeepLearningSoft48.Modules.Parameters
{
    /// <summary>
    /// Parameter base class.
    /// </summary>
    public abstract class ParameterBase : ObservableRecipient
    {
        //====================================================================
        // Properties
        //====================================================================

        /// <summary>
        /// HMI View used to display the parameter.
        /// </summary>
        public abstract UserControl ParameterUI { get; }

        /// <summary>
        /// Module Involved.
        /// </summary>
        public ModuleBase Module { get; private set; }

        /// <summary>
        /// Module's Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Tests if the module is correctly configured.
        /// </summary>
        /// <returns> Null or error message. </returns>
        public virtual string Validate() { return null; }

        /// <summary>
        /// List of possible values for the parameter for displaying the help.
        /// </summary>
        public virtual List<string> ValueList => new List<string>();


        //====================================================================
        // Constructor
        //====================================================================
        public ParameterBase(ModuleBase module, string name)
        {
            Module = module;
            Name = name;
        }

        //====================================================================
        // Active: indicates whether the parameter can be modified by the HMI.
        //====================================================================
        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled == value)
                    return;
                _isEnabled = value;
                OnPropertyChanged();
            }
        }

        //====================================================================
        // HMI's Text to display
        //====================================================================
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

        //====================================================================
        // ToString Method
        //====================================================================
        public override string ToString()
        {
            return Label;
        }
    }
}
