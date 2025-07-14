using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using LibProcessing;

namespace BasicModules.OpeningClosing
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class ClosingModule : ImageModuleBase
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
        public ClosingModule(IModuleFactory factory, int id, Recipe recipe)
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
            logDebug("Closing " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            IImage image = (IImage)obj;

            _processClass.Close(image.CurrentProcessingImage, paramKindOfPicture, paramNbIterations);

            ProcessChildren(obj);
        }

    }
}
