using ADCEngine;

namespace BasicModules.Trace
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    internal class TraceImageFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "TraceImage"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_Trace; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new TraceImageModule(this, id, recipe);
        }

        public override bool AcceptMultipleParents { get { return true; } }
        public override DataProducerType DataProducer { get { return DataProducerType.NoData; } }
        //#warning vérifier si on modifie les images
        //        public override bool ModifiesData { get { return false; } }
    }

}
