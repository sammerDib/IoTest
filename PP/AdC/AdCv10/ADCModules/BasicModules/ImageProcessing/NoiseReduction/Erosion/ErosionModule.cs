using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using LibProcessing;

namespace BasicModules.NoiseReduction
{
    public class ErosionModule : ImageModuleBase
    {
        private static ProcessingClass _processClass = new ProcessingClassMil();

        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly IntParameter paramNbIterations;
        public readonly EnumParameter<enKindOfPicture> paramKindOfPicture;


        //=================================================================
        // Constructeur
        //=================================================================
        public ErosionModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramNbIterations = new IntParameter(this, "NbIterations");
            paramKindOfPicture = new EnumParameter<enKindOfPicture>(this, "KindOfPicture");

            paramNbIterations.Value = 1;
            paramKindOfPicture.Value = enKindOfPicture.Binary;
        }


        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("Erosion " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            IImage image = (IImage)obj;

            _processClass.Erosion(image.CurrentProcessingImage, paramKindOfPicture, paramNbIterations);

            ProcessChildren(obj);
        }

    }
}
