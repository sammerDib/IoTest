using DeepLearningSoft48.Modules.Parameters;

using UnitySC.Shared.LibMIL;

using Matrox.MatroxImagingLibrary;

namespace DeepLearningSoft48.Modules.BasicModules.ImageProcessing.ShapeDetection
{
    public class SobelModule : ModuleBase
    {
        public enum EnTypeOfSobel
        {
            Average = 0,
            Gradiant,
            Vertical,
            Horizontal
        }

        public readonly EnumParameter<EnTypeOfSobel> paramTypeOfSobel;

        //=================================================================
        // Constructeur
        //=================================================================
        public SobelModule(IModuleFactory factory)
            : base(factory)
        {
            paramTypeOfSobel = new EnumParameter<EnTypeOfSobel>(this, "TypeOfSobel");

            paramTypeOfSobel.Value = EnTypeOfSobel.Gradiant;
        }

        //=================================================================
        // 
        //=================================================================
        public override MilImage Process(MilImage imgToProcess)
        {
            switch (paramTypeOfSobel.Value)
            {
                case EnTypeOfSobel.Average:
                    imgToProcess.EdgeDetect(MIL.M_SOBEL, MIL.M_DEFAULT, MIL.M_NULL);
                    break;
                case EnTypeOfSobel.Gradiant:
                    imgToProcess.EdgeDetect(MIL.M_SOBEL, MIL.M_REGULAR_EDGE_DETECT, MIL.M_NULL);
                    break;
                case EnTypeOfSobel.Horizontal:
                    imgToProcess.Convolve(MIL.M_HORIZ_EDGE);
                    break;
                case EnTypeOfSobel.Vertical:
                    imgToProcess.Convolve(MIL.M_VERT_EDGE);
                    break;
            }
            return imgToProcess;
        }

    }
}
