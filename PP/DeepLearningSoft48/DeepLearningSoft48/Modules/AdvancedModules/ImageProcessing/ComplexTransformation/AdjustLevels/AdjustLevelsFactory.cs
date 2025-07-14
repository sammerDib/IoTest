namespace DeepLearningSoft48.Modules.AdvancedModules.ImageProcessing.ComplexTransformation.AdjustLevels
{
    /// <summary>
    /// Class used by all modules to set their name.
    /// ModuleBase class needs a factory when passing through a module's construtor.
    /// </summary>
    public class AdjustLevelsFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "AdjustLevels"; }
        }

        public override ModuleBase FactoryMethod()
        {
            return new AdjustLevelsModule(this);
        }
    }
}
