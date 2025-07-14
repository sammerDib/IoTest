using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using BasicModules;

using LibProcessing;

namespace AdvancedModules.ComplexTransformation
{
    public class ConvolveModule : ImageModuleBase
    {
        private static ProcessingClass _processClass = new ProcessingClassMil();

        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly EnumParameter<enTypeOfConvolve> paramTypeOfConvolve;


        //=================================================================
        // Constructeur
        //=================================================================
        public ConvolveModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramTypeOfConvolve = new EnumParameter<enTypeOfConvolve>(this, "TypeOfConvolve");
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("process " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            IImage image = (IImage)obj;

            _processClass.UserConvolve(image.CurrentProcessingImage, paramTypeOfConvolve);

            ProcessChildren(obj);
        }
    }
}
