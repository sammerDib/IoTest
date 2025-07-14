using System.Linq;

using AdcBasicObjects;

using ADCEngine;

using UnitySC.Shared.LibMIL;

using Matrox.MatroxImagingLibrary;

namespace BasicModules.Mathematic
{
    public class AdditionModule : MathematicModuleBase
    {
        public readonly DoubleParameter paramConst;

        //=================================================================
        // Constructeur
        //=================================================================
        public AdditionModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramConst = new DoubleParameter(this, "const");
        }

        //=================================================================
        // 
        //=================================================================
        protected override void PerformMathematic(ImageBase[] images)
        {
            MilImage milImage1 = images[0].CurrentProcessingImage.GetMilImage();

            for (int i = 1; i < images.Count(); i++)
            {
                MilImage milImage2 = images[i].CurrentProcessingImage.GetMilImage();
                MilImage.Arith(milImage1, milImage2, milImage1, MIL.M_ADD + MIL.M_SATURATION);
            }

            if (paramConst != 0)
                milImage1.Arith(paramConst, MIL.M_ADD_CONST + MIL.M_SATURATION);
        }

    }
}
