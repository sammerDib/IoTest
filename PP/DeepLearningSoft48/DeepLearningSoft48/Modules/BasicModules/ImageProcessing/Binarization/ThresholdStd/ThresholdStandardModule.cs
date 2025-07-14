using System;

using DeepLearningSoft48.Modules.Parameters;

using UnitySC.Shared.LibMIL;

using Matrox.MatroxImagingLibrary;

namespace DeepLearningSoft48.Modules.BasicModules.ImageProcessing.Binarization.ThresholdStd
{
    internal class ThresholdStandardModule : ModuleBase
    {
        //=================================================================
        // XML Parameters
        //=================================================================
        public DoubleParameter paramLowThreshold;
        public DoubleParameter paramHighThreshold;

        //=================================================================
        // Constructor
        //=================================================================
        public ThresholdStandardModule(IModuleFactory factory)
            : base(factory)
        {
            paramLowThreshold = new DoubleParameter(this, "Low Threshold", min: 0);
            paramHighThreshold = new DoubleParameter(this, "High Threshold", min: 0);

            paramLowThreshold.Value = 5;
            paramHighThreshold.Value = 255;
        }

        //=================================================================
        // Process Method
        //=================================================================
        public override MilImage Process(MilImage imgToProcess)
        {
            if (Validate() == null)
            {
                if (imgToProcess.Type == 8 + MIL.M_UNSIGNED)
                {
                    imgToProcess.Binarize(MIL.M_IN_RANGE, paramLowThreshold, paramHighThreshold);
                }
                else
                {
                    using (MilImage milBinImage = new MilImage())
                    {
                        milBinImage.Alloc2d(imgToProcess.SizeX, imgToProcess.SizeY, 8 + MIL.M_UNSIGNED, imgToProcess.Attribute);

                        MilImage.Binarize(imgToProcess, milBinImage, MIL.M_IN_RANGE, paramLowThreshold, paramHighThreshold);

                        MilImage.Copy(milBinImage, imgToProcess);
                    }
                }
            }
            else
                throw new Exception(Validate());

            return imgToProcess;
        }

        //=================================================================
        // Validate Method
        //=================================================================
        public override string Validate()
        {
            if (paramLowThreshold > paramHighThreshold)
                return "Inconsistant threshold.";

            return null;
        }
    }
}
