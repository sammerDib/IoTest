using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using LibProcessing;

namespace BasicModules.Mathematic
{
    public class InversionModule : ImageModuleBase
    {
        private static ProcessingClass _processClass = new ProcessingClassMil();

        //=================================================================
        // Constructeur
        //=================================================================
        public InversionModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {

        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("process " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            IImage image = (IImage)obj;

            _processClass.Inversion(image.CurrentProcessingImage);

            ProcessChildren(obj);
        }

    }
}
