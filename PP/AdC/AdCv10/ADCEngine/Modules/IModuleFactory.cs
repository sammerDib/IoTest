using System.ComponentModel;

using ADC.Ressources;

namespace ADCEngine
{
    #region DEFINTION DES ENUM GLOBAUX
    public enum eModuleType
    {
        [Description("Shape Detection")]
        en_ShapeDetection,
        [Description("Noise Reduction")]
        en_NoiseReduction,
        [Description("Binarisation")]
        en_Binarisation,
        [Description("Border Removal")]
        en_BorderRemoval,
        [Description("Opening Closing")]
        en_OpeningClosing,
        [Description("Complex Transformation")]
        en_ComplexTransformation,
        [Description("Other Operations")]
        en_OtherOperation,
        [Description("Pattern Inspection")]
        en_PatternInspection,
        [Description("Metrology")]
        en_Metrology,
        [Description("Reporting and Edition")]
        en_ReportingEdition,
        [Description("Cluster Operations")]
        en_ClusterOperation,
        [Description("Learning Process")]
        en_LearningProcess,
        [Description("Merge and Layer Selection")]
        en_Merge,
        [Description("Data Loader")]
        en_Loader,
        [Description("Trace and Rendering")]
        en_Trace,
        [Description("Utility function")]
        en_Utility,
        [Description("<Meta Block>")]
        en_MetaBlock,
        [Description("Mathematic")]
        en_Mathematic,
        [Description("Debug")] // Just for debug
        en_Debug
    }

    public enum DataProducerType
    {
        NoData,
        OptionnalData,
        Data
    }

    #endregion

    ///////////////////////////////////////////////////////////////////////
    // Interface de la factory
    ///////////////////////////////////////////////////////////////////////
    public abstract class IModuleFactory
    {
        public abstract ModuleBase FactoryMethod(int id, Recipe recipe);
        public abstract string ModuleName { get; }
        public abstract eModuleType ModuleType { get; }
        public override string ToString() { return ModuleName; }
        public virtual bool AcceptMultipleParents { get { return false; } }
        public virtual DataProducerType DataProducer { get { return DataProducerType.Data; } } // Le module produit des données pour ses fils
        public virtual bool NeedAllData { get { return false; } }   // Le module a besoin que la Layer converse toutes les données
        public virtual bool ModifiesData { get { return true; } }   // Le module modifie la donnée d'entrée


        //=================================================================
        // Texte pour l'IHM
        //=================================================================
        private string _label;
        [System.Reflection.Obfuscation(Exclude = true)]
        public string Label
        {
            get
            {
                if (_label == null)
                {
                    ModuleResource moduleResource = UIResources.Instance.GetModuleResource(ModuleName);
                    if (moduleResource != null && !string.IsNullOrEmpty(moduleResource.UIValue))
                    {
                        _label = moduleResource.UIValue;
                    }
                    else
                        _label = ModuleName;
                }
                return _label;
            }
        }

        /// <summary>
        /// Nom de l'aide pour retrouver le fichier html associé
        /// </summary>
        private string _helpName;
        [System.Reflection.Obfuscation(Exclude = true)]
        public string HelpName
        {
            get
            {
                if (_helpName == null)
                {
                    ModuleResource moduleResource = UIResources.Instance.GetModuleResource(ModuleName);
                    _helpName = moduleResource != null ? moduleResource.HelpName : string.Empty;
                }

                return _helpName;
            }
        }
    }
}
