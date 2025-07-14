using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using LibProcessing;

namespace BasicModules.ShapeDetection
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class SobelModule : ImageModuleBase
    {
        private static ProcessingClass _processClass = new ProcessingClassMil();

        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly EnumParameter<enTypeOfSobel> paramTypeOfSobel;


        //=================================================================
        // Constructeur
        //=================================================================
        public SobelModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramTypeOfSobel = new EnumParameter<enTypeOfSobel>(this, "TypeOfSobel");

            paramTypeOfSobel.Value = enTypeOfSobel.Gradiant;
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("process " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            IImage image = (IImage)obj;

            _processClass.Sobel(image.CurrentProcessingImage, paramTypeOfSobel);

            ProcessChildren(obj);
        }

    }
}
