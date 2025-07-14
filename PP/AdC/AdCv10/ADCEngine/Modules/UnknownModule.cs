using System;

namespace ADCEngine
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    public class UnknownModuleFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "Unknown"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_Utility; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new UnknownModule(this, id, recipe);
        }
    }


    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class UnknownModule : ModuleBase
    {
        /// <summary> La raison pour laquelle le module n'a pas été chargé correctement </summary>
        public string Error;

        //=================================================================
        // 
        //=================================================================
        public UnknownModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            throw new NotImplementedException();
        }

        //=================================================================
        // 
        //=================================================================
        public override string Validate()
        {
            return Error;
        }
    }
}
