using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using LibProcessing;

namespace BasicModules.OpeningClosing
{
    public class ExpansionModule : ImageModuleBase
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
        public ExpansionModule(IModuleFactory factory, int id, Recipe recipe)
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
            logDebug("Expansion " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            IImage image = (IImage)obj;

            _processClass.Expansion(image.CurrentProcessingImage, paramKindOfPicture, paramNbIterations);

            ProcessChildren(obj);
        }

    }
}
