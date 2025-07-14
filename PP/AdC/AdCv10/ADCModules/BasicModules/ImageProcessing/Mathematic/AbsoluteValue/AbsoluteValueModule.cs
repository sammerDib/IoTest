using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using UnitySC.Shared.LibMIL;

using LibProcessing;

using Matrox.MatroxImagingLibrary;

namespace BasicModules.Mathematic
{
    public class AbsoluteValueModule : ImageModuleBase
    {
        private static ProcessingClass _processClass = new ProcessingClassMil();

        //=================================================================
        // Paramètres du XML
        //=================================================================


        //=================================================================
        // Constructeur
        //=================================================================
        public AbsoluteValueModule(IModuleFactory factory, int id, Recipe recipe)
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

            MilImage milImage = image.CurrentProcessingImage.GetMilImage();
            milImage.Arith(MIL.M_ABS);

            ProcessChildren(obj);
        }

    }
}
