using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using LibProcessing;

namespace BasicModules.NoiseReduction
{
    public class SmoothingModule : ImageModuleBase
    {
        private static ProcessingClass _processClass = new ProcessingClassCpp();

        //=================================================================
        // Paramètres du XML
        //=================================================================


        //=================================================================
        // Constructeur
        //=================================================================
        public SmoothingModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("Smoothing " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            IImage image = (IImage)obj;

            _processClass.Smoothing(image.CurrentProcessingImage);

            ProcessChildren(obj);
        }

    }
}
