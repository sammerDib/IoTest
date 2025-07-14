using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using BasicModules;

using LibProcessing;

namespace AdvancedModules.ShapeDetection
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class SharpenModule : ImageModuleBase
    {
        private static ProcessingClass _processClass = new ProcessingClassMil();

        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly EnumParameter<enTypeOfSharpen> paramTypeOfSharpen;


        //=================================================================
        // Constructeur
        //=================================================================
        public SharpenModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramTypeOfSharpen = new EnumParameter<enTypeOfSharpen>(this, "TypeOfSharpen");

            paramTypeOfSharpen.Value = enTypeOfSharpen.SharpenHigh;
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("process " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            IImage image = (IImage)obj;

            _processClass.Sharpen(image.CurrentProcessingImage, paramTypeOfSharpen);

            ProcessChildren(obj);
        }

    }
}
