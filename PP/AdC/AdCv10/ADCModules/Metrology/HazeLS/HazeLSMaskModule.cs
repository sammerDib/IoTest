using System;
using System.IO;
using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using BasicModules;

using UnitySC.Shared.LibMIL;

using Matrox.MatroxImagingLibrary;

namespace HazeLSModule
{
    internal class HazeLSMaskModule : ImageModuleBase
    {
        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly FileParameter paramMaskFile; //png : white = keep; other = masked

        //=================================================================
        // Constructeur
        //=================================================================
        public HazeLSMaskModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            string sFilters = "Haze Mask File (*.png)|*.png";
            paramMaskFile = new FileParameter(this, "Haze Mask File", sFilters);
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();
        }

        //=================================================================
        // 
        //=================================================================
        public override string Validate()
        {
            string error = base.Validate();
            if (error != null)
                return error;

            if (String.IsNullOrEmpty(paramMaskFile.FullFilePath) || String.IsNullOrWhiteSpace(paramMaskFile.FullFilePath))
                return "Empty  mask file path";

            if (File.Exists(paramMaskFile.FullFilePath) == false)
                return $"Mask file is not accessible : <{paramMaskFile.FullFilePath}>";

            return null;
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            if (State == eModuleState.Aborting)
                return;

            logDebug("Process Haze masking " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            ImageBase image = (ImageBase)obj;
            try
            {
                MilImage milImage = image.CurrentProcessingImage.GetMilImage();
                int imgWidth = milImage.SizeX;
                int imgHeight = milImage.SizeY;

                int mskWidth = MilImage.DiskInquire(paramMaskFile.FullFilePath, MIL.M_SIZE_X);
                int mskheight = MilImage.DiskInquire(paramMaskFile.FullFilePath, MIL.M_SIZE_Y);
                int mskDepth = MilImage.DiskInquire(paramMaskFile.FullFilePath, MIL.M_SIZE_BIT);


                if (imgWidth != mskWidth || imgHeight != mskheight)
                    throw new ApplicationException($"Mask image has a different size from the Haze image : expected size < {imgWidth} x {imgHeight} >");

                if (mskDepth != 8)
                    throw new ApplicationException($"Mask image is not a 8 bit image");

                using (MilImage milMask = new MilImage())
                {
                    milMask.Restore(paramMaskFile.FullFilePath);
                    milMask.Binarize(MIL.M_NOT_EQUAL, 0, MIL.M_NULL); // anything stricly différent from white color (255) will masked
                    milMask.Arith(255.0, MIL.M_DIV_CONST);

                    MilImage.Arith(milImage, milMask, milImage, MIL.M_MULT);
                }
                ProcessChildren(obj);
            }
            catch (Exception ex)
            {
                string sMessage = $"Haze Masking unexpected exception raised : {ex.Message}";
                throw;
            }
            finally
            {

            }

        }
    }
}
