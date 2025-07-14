namespace DeepLearningSoft48.Modules.BasicModules.ImageProcessing.Binarization.ThresholdStd
{
    /// <summary>
    /// Class used by all modules to set their name.
    /// ModuleBase class needs a factory when passing through a module's construtor.
    /// </summary>
    public class ThresholdStandardFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "ThresholdStandard"; }
        }

        public override ModuleBase FactoryMethod()
        {
            return new ThresholdStandardModule(this);
        }
    }
}
