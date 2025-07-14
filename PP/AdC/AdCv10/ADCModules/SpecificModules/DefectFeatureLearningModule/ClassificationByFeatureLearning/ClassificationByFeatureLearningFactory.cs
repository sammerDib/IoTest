using ADCEngine;

namespace DefectFeatureLearning.ClassificationByFeatureLearning
{
    internal class ClassificationByFeatureLearningFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "ClassificationByFeatureLearning"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_ClusterOperation; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new ClassificationByFeatureLearningModule(this, id, recipe);
        }

        public override bool ModifiesData { get { return false; } }
    }
}
