using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;

using ADC.Model;

using ADCEngine;

using AdcTools;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Tools;

namespace ADC
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public class SelectModuleViewModel : ObservableRecipient
    {
        //=================================================================
        // Sous-classe pour les catégories de modules
        //=================================================================
        public class ModuleCategory : IComparable
        {
            public string Description { get; set; }
            public eModuleType? moduleType;
            public List<Object> FactoryList { get; set; }

            public ModuleCategory()
            {
                FactoryList = new List<Object>();
            }

            public int CompareTo(object obj)
            {
                if (obj == null) return 1;

                ModuleCategory otherModuleCategory = obj as ModuleCategory;
                if (otherModuleCategory != null)
                    return Description.CompareTo(otherModuleCategory.Description);
                else
                    throw new ArgumentException("Object is not a ModuleCategory");
            }

            public override string ToString()
            {
                return Description;
            }

            private string _helpName;
            public string HelpName
            {
                get
                {
                    if (_helpName == null)
                    {
                        ADC.Ressources.ModuleTypeResource moduleTypeResource = ADC.Ressources.UIResources.Instance.GetModuleTypesResources(moduleType.ToString());
                        _helpName = moduleTypeResource != null ? moduleTypeResource.HelpName : string.Empty;
                    }

                    return _helpName;
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public class MetablocInfo : IComparable
        {
            public string Label { get; set; }
            public List<Object> metablockList { get; set; }

            public MetablocInfo()
            {
                metablockList = new List<Object>();
            }

            public int CompareTo(object obj)
            {
                if (obj == null)
                    return 1;

                MetablocInfo otherMetablocInfo = obj as MetablocInfo;
                if (otherMetablocInfo != null)
                    throw new ArgumentException("Object is not a MetablocInfo");
                return Label.CompareTo(otherMetablocInfo.Label);
            }

            public override string ToString()
            {
                return Label;
            }
        }

        //=================================================================
        // Propriétés et membres
        //=================================================================
        public ObservableCollection<ModuleCategory> CategoryList { get; set; }
        public ModuleCategory PreferredCategory { get; set; }
        public string ModuleFlags { get; private set; }

        private bool _checkChildren;
        private List<IModuleFactory> _compatibilityList;
        private ModuleBase _parentModule;


        //=================================================================
        // Constructor
        //=================================================================
        public SelectModuleViewModel() { }   //Just for design
        public SelectModuleViewModel(ModuleBase module, bool checkChildren, bool allowMetaBlock = false)
        {
            //-------------------------------------------------------------
            // Init
            //-------------------------------------------------------------
            _parentModule = module;
            _checkChildren = checkChildren;

            //-------------------------------------------------------------
            // Création de la liste des factory compatibles
            //-------------------------------------------------------------
            _compatibilityList = BuildCompatibilityList(checkChildren);

            //-------------------------------------------------------------
            // Création de la liste des catégories
            //-------------------------------------------------------------
            CategoryList = new ObservableCollection<ModuleCategory>();

            //-------------------------------------------------------------
            // Création des "<All Modules>" et "<Suggestions>"
            //-------------------------------------------------------------
            ModuleCategory category = BuildAllModulesCategory();
            CategoryList.Add(category);
            PreferredCategory = category;

            category = BuildSuggestionsCategory();
            if (category != null)
            {
                CategoryList.Add(category);
                PreferredCategory = category;
            }

            //-------------------------------------------------------------
            // distribution des modules compatibles dans leur catégorie 
            //-------------------------------------------------------------
            List<eModuleType> moduleTypes = Enum.GetValues(typeof(eModuleType)).Cast<eModuleType>().ToList();

            foreach (eModuleType type in moduleTypes)
            {
                if (type != eModuleType.en_Utility)
                {
                    category = BuildModuleCategory(type);
                    if (category != null)
                        CategoryList.Add(category);
                }
            }

            //-------------------------------------------------------------
            // Création de la catégorie "<en_MetaBlock>" si au moins un metablock est compatible
            //-------------------------------------------------------------
            if (allowMetaBlock)
            {
                category = BuildMetaBlockCategory();
                if (category != null)
                    CategoryList.Add(category);
            }

            //-------------------------------------------------------------
            // Pour le debug, les flags
            //-------------------------------------------------------------
            CompatibilityManager compman = ServiceRecipe.Instance().RecipeCurrent.CompatibilityManager;
            List<string> compatilityList = compman.GetModuleCompatibility(_parentModule);
            foreach (string family in compatilityList)
                ModuleFlags += family + " ";
            if (ModuleFlags == null)
                ModuleFlags = "∅";
            ModuleFlags = "For Debug: " + ModuleFlags;
        }

        //=================================================================
        // List des IModuleFactory compatibles avec parent 
        //=================================================================
        private List<IModuleFactory> BuildCompatibilityList(bool checkChildren)
        {
            ADCEngine.ADC adc = ADCEngine.ADC.Instance;
            CompatibilityManager compman = ServiceRecipe.Instance().RecipeCurrent.CompatibilityManager;

            List<IModuleFactory> list = new List<IModuleFactory>((IEnumerable<IModuleFactory>)adc.Factories.Values.ToList());

            list.Remove(adc.Factories["Root"]);
            list.Remove(adc.Factories["Termination"]);
#if !DEBUG
            list.RemoveAll(x => x.ModuleType == eModuleType.en_Debug);
#endif
            if (_parentModule != null)
            {
                list = list.Where(factory => compman.IsFactoryCompatibleWithParent(factory, _parentModule)).ToList();

                if (checkChildren)
                {
                    foreach (var child in _parentModule.Children)
                        list = list.Where(factory => compman.IsFactoryCompatibleWithChild(_parentModule, factory, child)).ToList();
                }
            }

            list.Sort((a, b) => a.ToString().CompareTo(b.ToString()));
            return list;
        }


        //=================================================================
        // Construit une catégorie avec sa liste de modules compatibles
        //=================================================================
        private ModuleCategory BuildModuleCategory(eModuleType moduleType)
        {
            //-------------------------------------------------------------
            // Construction de la liste des modules
            //-------------------------------------------------------------
            List<IModuleFactory> list = _compatibilityList;

            list = list.Where(factory => ((IModuleFactory)factory).ModuleType == moduleType).ToList();

            if (list.Count == 0)
                return null;

            //-------------------------------------------------------------
            // Retourne la category
            //-------------------------------------------------------------
            ModuleCategory category = new ModuleCategory();
            category.Description = moduleType.GetDescription();
            category.moduleType = moduleType;
            category.FactoryList = list.OfType<object>().ToList();

            return category;
        }

        //=================================================================
        // 
        //=================================================================
        private ModuleCategory BuildAllModulesCategory()
        {
            ModuleCategory category = new ModuleCategory();
            category.Description = "<All Modules>";
            category.FactoryList = _compatibilityList.OfType<object>().ToList();

            return category;
        }

        //=================================================================
        // 
        //=================================================================
        private ModuleCategory BuildSuggestionsCategory()
        {
            CompatibilityManager compman = ServiceRecipe.Instance().RecipeCurrent.CompatibilityManager;

            ModuleCategory category = new ModuleCategory();
            category.Description = "<Suggested Modules>";
            category.FactoryList = new List<object>();

            var suggestions = compman.GetModuleSuggestions(_parentModule.Factory.ModuleName);
            foreach (IModuleFactory factory in _compatibilityList)
            {
                string name = factory.ModuleName;
                if (suggestions.Contains(name))
                    category.FactoryList.Add(factory);
            }

            if (category.FactoryList.IsEmpty())
                return null;
            else
                return category;
        }

        /// <summary>
        /// Construit la liste des metablocs disponibles
        /// </summary>
        /// <returns></returns>
        private ModuleCategory BuildMetaBlockCategory()
        {
            CompatibilityManager compman = ServiceRecipe.Instance().RecipeCurrent.CompatibilityManager;

            // Lister les metablocs
            string[] listfiles = null;
            String pathMetaBlocks = null;
            try
            {
                pathMetaBlocks = ConfigurationManager.AppSettings["Editor.MetablockFolder"];
                listfiles = Directory.GetFiles(pathMetaBlocks, "*.adcmtb", SearchOption.AllDirectories);
            }
            catch (Exception ex)
            {
                ExceptionMessageBox.Show("Failed to read: " + pathMetaBlocks, ex);
                return null;
            }

            List<Object> listMetablocks = new List<object>();

            foreach (string filename in listfiles)
            {
                List<ModuleBase> _metablock = new List<ModuleBase>();
                MetablocInfo metablock = new MetablocInfo();

                try
                {
                    ServiceRecipe.Instance().LoadMetaBlock(filename, ref _metablock);
                }
                catch (Exception ex)
                {
                    string msg = "Failed to read \"" + filename + "\" " + ex.Message;
                    ExceptionMessageBox.Show(msg, ex);
                    continue;
                }

                // type du 1er module du metabloc compatible ?
                if (compman.IsFactoryCompatibleWithParent(_metablock[0].Factory, _parentModule))
                {
                    metablock.metablockList = new List<Object>((IEnumerable<Object>)_metablock);
                    metablock.Label = Path.GetFileNameWithoutExtension(filename);
                    listMetablocks.Add(metablock);
                }

            }

            if (listMetablocks.Count == 0)
                return null;

            // Sélectionner les metablocs compatibles
            ModuleCategory category = new ModuleCategory();

            category.Description = eModuleType.en_MetaBlock.GetDescription();
            category.FactoryList = listMetablocks;

            return category;
        }

    }
}
