using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using BasicModules;

using LibProcessing;

namespace AdvancedModules.Binarisation
{
    public class ThresholdMedianModule : ImageModuleBase
    {
        private static ProcessingClass _processClass = new ProcessingClassMil();

        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly IntParameter paramNbMorpho;
        public readonly IntParameter paramContrastThreashold;


        //=================================================================
        // Constructeur
        //=================================================================
        public ThresholdMedianModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramNbMorpho = new IntParameter(this, "NbMorpho");
            paramContrastThreashold = new IntParameter(this, "ContrastThreashold");

            paramContrastThreashold.Value = 15;
            paramNbMorpho.Value = 6;
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("ThresholdMedian " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            IImage image = (IImage)obj;

            _processClass.ThresholdMedian(image.CurrentProcessingImage, paramNbMorpho.Value, paramContrastThreashold.Value);

            ProcessChildren(obj);
        }

    }
}
