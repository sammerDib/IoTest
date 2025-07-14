using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using BasicModules;

using LibProcessing;

namespace ExpertModules.ComplexTransformation
{
    public class ExtractHolesModule : ImageModuleBase
    {
        private static ProcessingClass _processClass = new ProcessingClassMil();

        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly EnumParameter<enKindOfPicture> paramKindOfPicture;


        //=================================================================
        // Constructeur
        //=================================================================
        public ExtractHolesModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramKindOfPicture = new EnumParameter<enKindOfPicture>(this, "KindOfPicture");

            paramKindOfPicture.Value = enKindOfPicture.Binary;
        }


        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("ExtractHoles " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            IImage image = (IImage)obj;

            _processClass.MilBlobExtractHoles(image.CurrentProcessingImage, paramKindOfPicture);

            ProcessChildren(obj);
        }

    }
}
