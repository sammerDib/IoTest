using System;

namespace ADCEngine
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    public class RootModuleFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "Root"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_Utility; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new RootModule(this, id, recipe);
        }
    }


    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class RootModule : ModuleBase
    {
        //=================================================================
        // 
        //=================================================================
        public RootModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            ModuleProperty = eModuleProperty.Stage;
        }

        //=================================================================
        // Process
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            if (State == eModuleState.Aborting)
                return;

            try
            {
                ProcessChildren(obj);
            }
            catch (Exception e)
            {
                HandleException(e);
            }
        }

    }
}
