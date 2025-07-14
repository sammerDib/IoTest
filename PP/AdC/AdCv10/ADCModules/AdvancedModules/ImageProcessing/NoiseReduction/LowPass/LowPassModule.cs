using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using BasicModules;

using LibProcessing;

namespace AdvancedModules.NoiseReduction
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class LowPassModule : ImageModuleBase
    {
        private static ProcessingClass _processClass = new ProcessingClassMil();

        //=================================================================
        // Paramètres du XML
        //=================================================================


        //=================================================================
        // Constructeur
        //=================================================================
        public LowPassModule(IModuleFactory factory, int id, Recipe recipe)
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

            _processClass.LowPass(image.CurrentProcessingImage);

            ProcessChildren(obj);
        }

    }
}
