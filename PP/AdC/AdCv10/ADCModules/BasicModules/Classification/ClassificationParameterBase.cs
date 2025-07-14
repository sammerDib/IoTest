using System.Collections.Generic;
using System.Linq;

using AdcBasicObjects;

using ADCEngine;

using AdcTools;

using BasicModules.DataLoader;
using BasicModules.Edition.DummyDefect;


namespace BasicModules
{
    public abstract class ClassificationParameterBase : ParameterBase
    {
        //=================================================================
        // Constructeur
        //=================================================================
        public ClassificationParameterBase(ModuleBase module, string name) :
            base(module, name)
        {
        }

        //=================================================================
        //
        //=================================================================
        public List<string> FindAvailableLayers()
        {
            List<string> layers = new List<string>();

            List<ModuleBase> loaders = Module.FindAncestors(mod => mod is DataLoaderBase);
            foreach (DataLoaderBase loader in loaders)
                layers.TryAdd(loader.LayerName);    //évite les dupliqués

            return layers;
        }

        //=================================================================
        //
        //=================================================================
        virtual public HashSet<string> FindAvailableDefectLabels()
        {
            HashSet<string> defectLabels = new HashSet<string>();

            List<ModuleBase> classifiers = Module.FindAncestors(mod => mod is IClassifierModule);
            foreach (IClassifierModule classifier in classifiers)
                defectLabels.UnionWith(classifier.DefectClassLabelList);

            return defectLabels;
        }

        public HashSet<string> FindAvailableDefectLabelsWithDummyDefect()
        {
            HashSet<string> defectLabels = FindAvailableDefectLabels();
            if (Module.FindAncestors(mod => mod is DummyDefectModule).Any())
                defectLabels.Add(DummyDefectModule.DefectClassName);
            return defectLabels;
        }

        //=================================================================
        //
        //=================================================================
        public HashSet<Characteristic> FindAvailableCharacteristics()
        {
            HashSet<Characteristic> caracList = new HashSet<Characteristic>();

            List<ModuleBase> characterizers = Module.FindAncestors(mod => mod is ICharacterizationModule);
            foreach (ICharacterizationModule characterizer in characterizers)
                caracList.UnionWith(characterizer.AvailableCharacteristics);

            return caracList;
        }


    }
}
