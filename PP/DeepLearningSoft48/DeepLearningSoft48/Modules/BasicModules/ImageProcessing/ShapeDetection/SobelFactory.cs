namespace DeepLearningSoft48.Modules.BasicModules.ImageProcessing.ShapeDetection
{
    public class SobelFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "Sobel"; }
        }

        public override ModuleBase FactoryMethod()
        {
            return new SobelModule(this);
        }

    }
}
