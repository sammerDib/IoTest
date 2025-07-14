using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using BasicModules;

using LibProcessing;

namespace AdvancedModules.NoiseReduction
{
    public class LocalDensityFilteringModule : ImageModuleBase
    {
        private static ProcessingClass _processClass = new ProcessingClassCpp();

        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly IntParameter paramMaskSize;
        public readonly IntParameter paramSignificantDensity;


        //=================================================================
        // Constructeur
        //=================================================================
        public LocalDensityFilteringModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramMaskSize = new IntParameter(this, "MaskSize");
            paramSignificantDensity = new IntParameter(this, "SignificantDensity");

            paramMaskSize.Value = 40;
            paramSignificantDensity.Value = 10;
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("process " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            IImage image = (IImage)obj;

            _processClass.LocalDensityFiltering(image.CurrentProcessingImage, paramMaskSize, paramSignificantDensity);

            ProcessChildren(obj);
        }

    }
}
