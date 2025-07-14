using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using BasicModules;

using LibProcessing;

using UnitySC.Shared.LibMIL;

namespace AdvancedModules.NoiseReduction
{
    public class TrimingModule : ImageModuleBase
    {
        private static ProcessingClass _processClass = new ProcessingClassCpp();

        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly IntParameter paramRange;
        public readonly IntParameter paramNbNeighbours;


        //=================================================================
        // Constructeur
        //=================================================================
        public TrimingModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramRange = new IntParameter(this, "Range");
            paramNbNeighbours = new IntParameter(this, "NbNeighbours");

            paramNbNeighbours.Value = 4;
            paramRange.Value = 1;
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
            _processClass.Rognage(image.CurrentProcessingImage, paramNbNeighbours);

            ProcessChildren(obj);
        }

    }
}
