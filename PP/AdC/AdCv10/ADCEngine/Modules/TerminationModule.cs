using System;

using UnitySC.Shared.Tools;


namespace ADCEngine
{
    ///////////////////////////////////////////////////////////////////////
    // Factory
    ///////////////////////////////////////////////////////////////////////
    public class TerminationModuleFactory : IModuleFactory
    {
        public override string ModuleName
        {
            get { return "Termination"; }
        }

        public override eModuleType ModuleType
        {
            get { return eModuleType.en_Utility; }
        }

        public override ModuleBase FactoryMethod(int id, Recipe recipe)
        {
            return new TerminationModule(this, id, recipe);
        }
    }


    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class TerminationModule : ModuleBase
    {
        //=================================================================
        // 
        //=================================================================
        public TerminationModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            // Si cette méthode est appellée, c'est que la recette crée des objets mais ne les utilise pas.
            // Ce n'est pas un bonne recette, mais ça arrive souvent en debug.
            //throw new ApplicationException("TerminationModule.Process() must not be called");
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnStopping(eModuleState oldState)
        {
            logDebug("Recipe executed");
            SetState(eModuleState.Stopped);

            // Informe la recette qu'elle est terminée
            //........................................
            Recipe.RecipeExecuted();

            // Sanity check
            //.............
            if (ObjectBase.NbObjects != 0 && !DisposableObject.UseGC && !Recipe.IsRendering)
            {
                // Activer DEBUG_OBJECT_LEAK et regarder UnitySC.Shared.Tools.DisposableObject.ObjList
                //throw new ApplicationException("Some objects have not been freed");
            }
        }

    }
}
