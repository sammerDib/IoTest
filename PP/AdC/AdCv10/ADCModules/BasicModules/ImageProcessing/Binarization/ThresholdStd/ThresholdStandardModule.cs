using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using LibProcessing;

namespace BasicModules.Binarisation
{
    internal class ThresholdStandardModule : ImageModuleBase
    {
        private static ProcessingClass _processClass = new ProcessingClassMil();

        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly DoubleParameter paramLowThreshold;
        public readonly DoubleParameter paramHighThreshold;


        //=================================================================
        // Constructeur
        //=================================================================
        public ThresholdStandardModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramLowThreshold = new DoubleParameter(this, "LowThreshold", min: 0);
            paramHighThreshold = new DoubleParameter(this, "HighThreshold", min: 0);

            paramLowThreshold.Value = 5;
            paramHighThreshold.Value = 255;
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("process " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            IImage image = (IImage)obj;

            _processClass.Threshold(image.CurrentProcessingImage, paramLowThreshold, paramHighThreshold);

            ProcessChildren(obj);
        }

        //=================================================================
        // 
        //=================================================================
        public override string Validate()
        {
            if (paramLowThreshold > paramHighThreshold)
                return "inconsistant thresholds";

            return null;
        }

    }
}
