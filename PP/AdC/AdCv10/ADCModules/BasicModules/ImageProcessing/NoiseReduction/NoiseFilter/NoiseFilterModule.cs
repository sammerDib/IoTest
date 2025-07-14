using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using LibProcessing;

namespace BasicModules.NoiseReduction
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class NoiseFilterModule : ImageModuleBase
    {
        private static ProcessingClass _processClass = new ProcessingClassMil();

        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly DoubleParameter paramNoisePercent;


        //=================================================================
        // Constructeur
        //=================================================================
        public NoiseFilterModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramNoisePercent = new DoubleParameter(this, "NoisePercent");

            paramNoisePercent.Value = 10;
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("NoiseFilter " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            IImage image = (IImage)obj;

            _processClass.RemoveLittleBlob(image.CurrentProcessingImage, paramNoisePercent);

            ProcessChildren(obj);
        }

    }
}
