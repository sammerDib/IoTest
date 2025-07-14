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
    public class LaplacianModule : ImageModuleBase
    {
        private static ProcessingClass _processClass = new ProcessingClassMil();

        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly EnumParameter<enTypeOfLaplacian> paramTypeOfLaplacian;


        //=================================================================
        // Constructeur
        //=================================================================
        public LaplacianModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramTypeOfLaplacian = new EnumParameter<enTypeOfLaplacian>(this, "TypeOfLaplacian");

            paramTypeOfLaplacian.Value = enTypeOfLaplacian.LaplacianHigh;
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("process " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            IImage image = (IImage)obj;

            _processClass.Laplacian(image.CurrentProcessingImage, paramTypeOfLaplacian);

            ProcessChildren(obj);
        }

    }
}
