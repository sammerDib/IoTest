using System.Linq;

using AdcBasicObjects;

using ADCEngine;

using UnitySC.Shared.LibMIL;

using Matrox.MatroxImagingLibrary;

namespace BasicModules.Mathematic
{
    public class SubtractionModule : MathematicModuleBase
    {
        [ExportableParameter(false)]
        public readonly FirstOperandParameter paramFirstOperand;
        public readonly DoubleParameter paramConst;

        //=================================================================
        // Constructeur
        //=================================================================
        public SubtractionModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramConst = new DoubleParameter(this, "const");
            paramFirstOperand = new FirstOperandParameter(this, "FirstOperand");
        }

        //=================================================================
        // 
        //=================================================================
        protected override void ProcessMultiImage(MultiImage multiImage)
        {
            CheckImageSize(multiImage);
            PerformMathematic(multiImage.Images);
            ProcessChildren(multiImage.Images[paramFirstOperand]);
        }

        //=================================================================
        // 
        //=================================================================
        protected override void PerformMathematic(ImageBase[] images)
        {
            MilImage milImage1 = images[paramFirstOperand].CurrentProcessingImage.GetMilImage();

            for (int i = 0; i < images.Count(); i++)
            {
                MilImage milImage2 = images[i].CurrentProcessingImage.GetMilImage();
                if (milImage2 != milImage1)
                    MilImage.Arith(milImage1, milImage2, milImage1, MIL.M_SUB + MIL.M_SATURATION);
            }

            if (paramConst != 0)
                milImage1.Arith(paramConst, MIL.M_SUB_CONST + MIL.M_SATURATION);
        }

    }
}
