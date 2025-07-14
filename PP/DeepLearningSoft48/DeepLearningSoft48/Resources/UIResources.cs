using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Serialization;

using Serilog;

using UnitySC.Shared.Tools;

namespace DeepLearningSoft48.Resources
{
    /// <summary>
    /// HMI resource management class for modules, parameters and parameter values.
    /// </summary>
    public class UIResources
    {
        private static volatile UIResources _instance;
        private static object _syncRoot = new Object();

        /// <summary>
        ///  Singleton instance
        /// </summary>
        [XmlIgnore]
        public static UIResources Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                        {
                            try
                            {

                                string uiResourceFile = ".\\Resources\\UIResources.xml";

                                _instance = XML.Deserialize<UIResources>(uiResourceFile);
                            }
                            catch (Exception ex)
                            {
                                Log.Error("Invalid UIResources.xml file" + ex.ToString());
                                Debug.WriteLine("Invalid UIResources.xml file" + ex.ToString());
                                _instance = new UIResources();
                            }

                        }
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// List of module resources.
        /// </summary>
        public List<ModuleResource> Modules = new List<ModuleResource>();

        /// <summary>
        /// Resource list for module types.
        /// </summary>
        public List<ModuleTypeResource> ModuleTypes = new List<ModuleTypeResource>();

        /// <summary>
        /// Retrieves the resource associated with a module.
        /// </summary>
        /// <param name="moduleName"></param>
        /// <returns> ModuleResource </returns>
        public ModuleResource GetModuleResource(string moduleName)
        {
            return Modules.FirstOrDefault(x => x.Key == moduleName);
        }

        /// <summary>
        /// Retrieves the resource associated with a parameter.
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="parameterName"></param>
        /// <returns> ParameterResource </returns>
        public ParameterResource GetParameterResource(string moduleName, string parameterName)
        {
            ModuleResource module = GetModuleResource(moduleName);
            return module != null ? module.Parameters.FirstOrDefault(x => x.Key == parameterName) : null;
        }

        /// <summary>
        /// Retrieves the resource associated with the value of a parameter.
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <returns> UIResource </returns>
        public UIResource GetParameterValueResource(string moduleName, string parameterName, string parameterValue)
        {
            ParameterResource parameter = GetParameterResource(moduleName, parameterName);
            return parameter?.ParameterValues.FirstOrDefault(x => x.Key == parameterValue);
        }

        public ModuleTypeResource GetModuleTypesResources(string typeName)
        {
            return ModuleTypes.FirstOrDefault(x => x.Key == typeName);
        }
    }

    /// <summary>
    /// HMI Resource
    /// </summary>
    public class UIResource
    {
        /// <summary>
        /// Name defined in the application.
        /// </summary>
        [XmlAttribute]
        public string Key;

        /// <summary>
        /// Name of the associated help.
        /// </summary>
        [XmlAttribute]
        public string HelpName = string.Empty;

        /// <summary>
        /// Name displayed to the user.
        /// </summary>
        [XmlAttribute]
        public string UIValue = string.Empty;
    }

    /// <summary>
    /// Resource for a module.
    /// </summary>
    public class ModuleResource : UIResource
    {
        public List<ParameterResource> Parameters = new List<ParameterResource>();
    }

    /// <summary>
    /// Resource for a parameter.
    /// </summary>
    public class ParameterResource : UIResource
    {
        public List<UIResource> ParameterValues = new List<UIResource>();
    }

    /// <summary>
    /// Resource for a module type.
    /// </summary>
    public class ModuleTypeResource
    {
        [XmlAttribute]
        public string Key;

        [XmlAttribute]
        public string HelpName = string.Empty;
    }
}
