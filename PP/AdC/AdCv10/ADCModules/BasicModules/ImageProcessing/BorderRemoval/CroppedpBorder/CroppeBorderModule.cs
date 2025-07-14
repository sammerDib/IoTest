using System.Drawing;
using System.IO;
using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using AdcTools;

using LibProcessing;

using UnitySC.Shared.LibMIL;


using Matrox.MatroxImagingLibrary;

namespace BasicModules.CroppepBorder
{
    ///////////////////////////////////////////////////////////////////////
    // Module : CroppeBorderModule
    //
    //        Decoupe l'image  suivant deux marges  :
    //                              paramMargin_X et paramMargin_Y
    // 
    ///////////////////////////////////////////////////////////////////////
    public class CroppeBorderModule : ImageModuleBase
    {
        private static ProcessingClassMil _processingClassMil = new ProcessingClassMil();
        
        //=================================================================
        // Paramètres du XML
        //=================================================================
        public IntParameter paramMargin_X;
        public IntParameter paramMargin_Y;
        public BoolParameter paramDebug;

        //=================================================================
        // Constructeur
        //=================================================================
        public CroppeBorderModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {

            paramMargin_X = new IntParameter(this, "Margin X");
            paramMargin_Y = new IntParameter(this, "Margin Y");
            paramDebug = new BoolParameter(this, "Debug");

            paramMargin_X.Value = 1000;  // Pixel
            paramMargin_Y.Value = 1000;  // Pixel
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("CroppeBorder " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            ImageBase image = (ImageBase)obj;
            
            // calcul de la nouvelle taille de l'image
            int widthCroppeImage = image.Rectangle.Width - 2 * paramMargin_X;
            int heightCroppeImage = image.Rectangle.Height - 2 * paramMargin_Y;
            
            image.ResultProcessingImage = _processingClassMil.Crop(image.CurrentProcessingImage, (int)paramMargin_X, (int)paramMargin_Y, widthCroppeImage, heightCroppeImage);
            image.OriginalProcessingImage = _processingClassMil.Crop(image.OriginalProcessingImage, (int)paramMargin_X, (int)paramMargin_Y, widthCroppeImage, heightCroppeImage);
            
            // on ajuste la taille de l'image ImageBase
            image.imageRect.Width = image.OriginalProcessingImage.Width;
            image.imageRect.Height = image.OriginalProcessingImage.Height;
            image.imageRect.X += paramMargin_X;
            image.imageRect.Y += paramMargin_Y;
            ProcessChildren(obj);
        }

        //=================================================================
        // 
        //=================================================================
#if DRAW_MAP
        public override void Stop(ModuleBase parent)
        {
            EdgeRemoveAlgorithm.map.Save(PathString.GetTempPath() / "EdgeRemove.tif");
            EdgeRemoveAlgorithm.map.DelRef();

            base.Stop(parent);
        }
#endif

    }
}

    

