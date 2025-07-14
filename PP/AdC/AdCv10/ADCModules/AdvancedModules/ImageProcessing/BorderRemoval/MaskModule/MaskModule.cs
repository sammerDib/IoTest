using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using BasicModules;

using LibProcessing;

namespace AdvancedModules.BorderRemoval
{
    public class MaskModule : ImageModuleBase
    {
        private static ProcessingClass _processClass = new ProcessingClassMil();

        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly FileParameter paramMaskFile;


        //=================================================================
        // Constructeur
        //=================================================================
        public MaskModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramMaskFile = new FileParameter(this, "Mask Image", "BMP file (*.bmp)|*.bmp|Tiff file (*.tiff)|*.tiff;*.tif|Png file (*.png)|*.png|Jpeg file (*.jpg)|*.jpg|All files (*.*)|*.*");
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("process " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            IImage image = (IImage)obj;

            _processClass.MaskRemove(image.CurrentProcessingImage, paramMaskFile.FullFilePath);

            ProcessChildren(obj);
        }

    }
}
