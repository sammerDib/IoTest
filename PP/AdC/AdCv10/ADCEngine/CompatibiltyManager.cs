using System;
using System.Collections.Generic;
using System.Windows;

using AdcTools;

using UnitySC.Shared.Tools;

namespace ADCEngine
{
    ///////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////
    internal class FactoryCompatibility
    {
        public string ModuleName { get; set; }
        public List<string> RequiredCompatibilityList { get; set; }
        public List<string> IncompatibilityList { get; set; }
        public List<string> ProvidedCompatibilityList { get; set; }
        public List<string> ClearedCompatibilityList { get; set; }
        public List<string> SuggestionList { get; set; }

        public FactoryCompatibility()
        {
            RequiredCompatibilityList = new List<string>();
            IncompatibilityList = new List<string>();
            ProvidedCompatibilityList = new List<string>();
            ClearedCompatibilityList = new List<string>();
            SuggestionList = new List<string>();
        }
    }

    ///////////////////////////////////////////////////////////////////////
    // 
    ///////////////////////////////////////////////////////////////////////
    internal class ModuleCompatibility
    {
        public string ModuleName { get; set; }
        public List<string> ProvidedCompatibilityList { get; set; }

        public ModuleCompatibility()
        {
            ProvidedCompatibilityList = new List<string>();
        }
    }

    ///////////////////////////////////////////////////////////////////////
    // 
    ///////////////////////////////////////////////////////////////////////
    public class CompatibilityManager
    {
        private List<string> _familyList = new List<string>();
        private Dictionary<string, FactoryCompatibility> _factoryCompatibilityMap = new Dictionary<string, FactoryCompatibility>();
        private Dictionary<ModuleBase, ModuleCompatibility> _modulesCompatibilityMap = new Dictionary<ModuleBase, ModuleCompatibility>();
        private static String _compatibilityXMLFile = PathString.GetExecutingAssemblyPath().Directory / "ModuleCompatibility.xml";

        //=================================================================
        // 
        //=================================================================        
        public void LoadSettings()
        {
            CompatibilityObjSerialiser.CompatibilityRules treeObj = CompatibilityObjSerialiser.DeSerialize(_compatibilityXMLFile);
            LoadSettings(treeObj);
        }

        public void LoadSettings(CompatibilityObjSerialiser.CompatibilityRules treeObj)
        {
            LoadFlags(treeObj);
            LoadModuleCompatibility(treeObj);
        }

        //=================================================================
        // 
        //=================================================================
        private void LoadFlags(CompatibilityObjSerialiser.CompatibilityRules treeObj)
        {
            _familyList.Clear();
            foreach (CompatibilityObjSerialiser.Family _family in treeObj.Families)
            {
                _familyList.Add(_family.Name);
            }
        }

        //=================================================================
        // 
        //=================================================================
        private void LoadModuleCompatibility(CompatibilityObjSerialiser.CompatibilityRules treeObj)
        {
            foreach (CompatibilityObjSerialiser.Module _module in treeObj.Modules)
            {
                FactoryCompatibility compatility = new FactoryCompatibility();
                compatility.ModuleName = _module.Name;

                foreach (CompatibilityObjSerialiser.Family _familyRequiere in _module.Requires)
                {
                    compatility.RequiredCompatibilityList.Add(_familyRequiere.Name);
                }

                foreach (CompatibilityObjSerialiser.Family _familyIncompatible in _module.IncompatibleWith)
                {
                    compatility.IncompatibilityList.Add(_familyIncompatible.Name);
                }

                foreach (CompatibilityObjSerialiser.Family _familyProvide in _module.Provides)
                {
                    compatility.ProvidedCompatibilityList.Add(_familyProvide.Name);
                }

                foreach (CompatibilityObjSerialiser.Family _familyClear in _module.Cleared)
                {
                    compatility.ClearedCompatibilityList.Add(_familyClear.Name);
                }

                foreach (CompatibilityObjSerialiser.ModuleName _moduleSuggested in _module.Suggestion)
                {
                    compatility.SuggestionList.Add(_moduleSuggested.Name);
                }

                _factoryCompatibilityMap.Add(compatility.ModuleName, compatility);
            }
        }

        //=================================================================
        // 
        //=================================================================
        public bool IsFactoryCompatibleWithParent(IModuleFactory factory, ModuleBase parent)
        {
            try
            {
                FactoryCompatibility factoryComptability = _factoryCompatibilityMap[factory.ModuleName];
                ModuleCompatibility parentComptability = _modulesCompatibilityMap[parent];

                foreach (var compatibility in factoryComptability.RequiredCompatibilityList)
                {
                    if (!parentComptability.ProvidedCompatibilityList.Contains(compatibility))
                        return false;
                }

                foreach (var compatibility in factoryComptability.IncompatibilityList)
                {
                    if (parentComptability.ProvidedCompatibilityList.Contains(compatibility))
                        return false;
                }

                return true;
            }
            catch (KeyNotFoundException)
            {
                MessageBox.Show("Module factory \"" + factory + "\" not found in module compatibility list", "Compatibility Error from file ModuleCompaibility. Contact support", MessageBoxButton.OK, MessageBoxImage.Stop);
                return false;
                //throw new ApplicationException("Module factory \"" + factory + "\" not found in module compatibility list");
            }
        }

        //=================================================================
        // 
        //=================================================================
        public bool IsFactoryCompatibleWithChild(ModuleBase parent, IModuleFactory factory, ModuleBase child)
        {
            try
            {
                if (factory.DataProducer == DataProducerType.NoData)
                    return false;

                List<string> providedCompatibilityList = ComputeProvidedCompatibilityList(factory, parent);

                FactoryCompatibility childFactoryComptability = _factoryCompatibilityMap[child.Factory.ModuleName];

                foreach (var compatibility in childFactoryComptability.RequiredCompatibilityList)
                {
                    if (!providedCompatibilityList.Contains(compatibility))
                        return false;
                }

                foreach (var compatibility in childFactoryComptability.IncompatibilityList)
                {
                    if (providedCompatibilityList.Contains(compatibility))
                        return false;
                }

                return true;
            }
            catch (KeyNotFoundException)
            {
                throw new ApplicationException("Factory " + factory + " not found in module compatibility list");
            }
        }

        //=================================================================
        // 
        //=================================================================
        public List<string> ComputeProvidedCompatibilityList(IModuleFactory factory, ModuleBase parent)
        {
            List<string> compatibilityList = new List<string>();

            ModuleCompatibility parentComptability = _modulesCompatibilityMap[parent];
            compatibilityList.AddRange(parentComptability.ProvidedCompatibilityList);

            FactoryCompatibility factoryComptability;
            bool b = _factoryCompatibilityMap.TryGetValue(factory.ModuleName, out factoryComptability);
            if (!b)
                throw new ApplicationException("Factory " + factory + " not found in module compatibility list");
            compatibilityList.AddRange(factoryComptability.ProvidedCompatibilityList);
            compatibilityList.RemoveRange(factoryComptability.ClearedCompatibilityList);

            return compatibilityList;
        }

        //=================================================================
        // 
        //=================================================================
        public void AddModule(ModuleBase module)
        {
            //-------------------------------------------------------------
            // Construction de la liste de compatibility
            //-------------------------------------------------------------
            List<string> compatibilityList = new List<string>();

            foreach (ModuleBase parent in module.Parents)
            {
                ModuleCompatibility parentComptability = _modulesCompatibilityMap[parent];
                compatibilityList.AddRange(parentComptability.ProvidedCompatibilityList);
            }

            FactoryCompatibility factoryComptability;
            bool b = _factoryCompatibilityMap.TryGetValue(module.Factory.ModuleName, out factoryComptability);
            if (!b)
                throw new ApplicationException("Factory " + module.Factory.ModuleName + " not found in module compatibility list");
            compatibilityList.AddRange(factoryComptability.ProvidedCompatibilityList);

            if (!(module is RootModule))
                compatibilityList.Remove("Root");

            //-------------------------------------------------------------
            // Creation du ModuleCompatibility
            //-------------------------------------------------------------
            ModuleCompatibility compatibility = new ModuleCompatibility();
            compatibility.ModuleName = module.ToString();
            compatibility.ProvidedCompatibilityList.UnionWith(compatibilityList);
            compatibility.ProvidedCompatibilityList.RemoveRange(factoryComptability.ClearedCompatibilityList);

            //-------------------------------------------------------------
            // Ajout au dictionnaire
            //-------------------------------------------------------------
            _modulesCompatibilityMap[module] = compatibility;
        }

        //=================================================================
        // 
        //=================================================================
        public void AddTree(ModuleBase module)
        {
            AddModule(module);
            foreach (ModuleBase child in module.Children)
                AddTree(child);
        }

        //=================================================================
        // 
        //=================================================================
        public void AddRecipe(Recipe recipe)
        {
            _modulesCompatibilityMap.Clear();
            AddRecursive(recipe.Termination);
        }

        private void AddRecursive(ModuleBase module)
        {
            foreach (ModuleBase parent in module.Parents)
            {
                if (!(_modulesCompatibilityMap.ContainsKey(parent)))
                    AddRecursive(parent);
            }

            AddModule(module);
        }

        //=================================================================
        // 
        //=================================================================
        public void RemoveModule(ModuleBase module)
        {
            _modulesCompatibilityMap.Remove(module);
        }

        //=================================================================
        // 
        //=================================================================
        public List<string> GetModuleCompatibility(ModuleBase module)
        {
            var campatility = _modulesCompatibilityMap[module];
            return campatility.ProvidedCompatibilityList;
        }

        //=================================================================
        // 
        //=================================================================
        public List<string> GetModuleSuggestions(string factoryName)
        {
            var campatility = _factoryCompatibilityMap[factoryName];
            return campatility.SuggestionList;
        }

    }
}
