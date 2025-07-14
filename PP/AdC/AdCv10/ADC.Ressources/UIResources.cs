using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

using UnitySC.PP.Shared.Configuration;
using UnitySC.Shared.Tools;
using Serilog;

namespace ADC.Ressources
{
    /// <summary>
    /// Class de gestion des resources de l'IHM pour les modules, les paramétres et les valeurs des paramétres
    /// Affichage utilisateur + Aide
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

                                string pathUIResourcesXml = AppParameter.Instance.Get("PathUIResourcesXml");

                                string uiResourceFile = Path.Combine(pathUIResourcesXml, "UIResources.xml");
                                //string uiResourceFile = Path.Combine(PathString.GetExecutingAssemblyPath().Directory, "UIResources.xml");

                                _instance = XML.Deserialize<UIResources>(uiResourceFile);
                            }
                            catch (Exception ex)
                            {
                                Log.Error("Invalid UIResources.xml file" + ex.ToString());
                                _instance = new UIResources();
                            }

                        }
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// Liste des resources des modules
        /// </summary>
        public List<ModuleResource> Modules = new List<ModuleResource>();

        /// <summary>
        /// Liste de recources pour les types de modules
        /// </summary>
        public List<ModuleTypeResource> ModuleTypes = new List<ModuleTypeResource>();


        /// <summary>
        /// Récupére la resource associée à un module
        /// </summary>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        public ModuleResource GetModuleResource(string moduleName)
        {
            return Modules.FirstOrDefault(x => x.Key == moduleName);
        }

        /// <summary>
        /// Récupére la resource associé à un paramétre
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public ParameterResource GetParameterResource(string moduleName, string parameterName)
        {
            ModuleResource module = GetModuleResource(moduleName);
            return module != null ? module.Parameters.FirstOrDefault(x => x.Key == parameterName) : null;
        }

        /// <summary>
        /// Récupére la resource associé à la veleur d'un paramétre
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="parameterName"></param>
        /// <param name="parameterValue"></param>
        /// <returns></returns>
        public UIResource GetParameterValueResource(string moduleName, string parameterName, string parameterValue)
        {
            ParameterResource parameter = GetParameterResource(moduleName, parameterName);
            return parameter != null ? parameter.ParameterValues.FirstOrDefault(x => x.Key == parameterValue) : null;
        }

        public ModuleTypeResource GetModuleTypesResources(string typeName)
        {
            return ModuleTypes.FirstOrDefault(x => x.Key == typeName);
        }
    }

    /// <summary>
    /// Resoure de IHM
    /// </summary>
    public class UIResource
    {
        /// <summary>
        /// Nom défini dans l'application
        /// </summary>
        [XmlAttribute]
        public string Key;

        /// <summary>
        /// Nom de l'aide asscociée
        /// </summary>
        [XmlAttribute]
        public string HelpName = string.Empty;

        /// <summary>
        /// Nom affiché à l'utilisateur
        /// </summary>
        [XmlAttribute]
        public string UIValue = string.Empty;
    }

    /// <summary>
    /// Resource pour un module
    /// </summary>
    public class ModuleResource : UIResource
    {
        public List<ParameterResource> Parameters = new List<ParameterResource>();
    }

    /// <summary>
    /// Resource pour un paramétre
    /// </summary>
    public class ParameterResource : UIResource
    {
        public List<UIResource> ParameterValues = new List<UIResource>();
    }

    /// <summary>
    /// Resource pour un type de module
    /// </summary>
    public class ModuleTypeResource
    {
        [XmlAttribute]
        public string Key;

        [XmlAttribute]
        public string HelpName = string.Empty;
    }
}
