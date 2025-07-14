using System.Linq;

using AdcBasicObjects;

using ADCEngine;

using BasicModules.Mathematic;

using UnitySC.Shared.LibMIL;

using Matrox.MatroxImagingLibrary;

namespace BasicModules.ImageProcessing.Mathematic.Average
{
    public class AverageModule : MathematicModuleBase
    {
        //=================================================================
        // Constructeur
        //=================================================================
        public AverageModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
        }

        //=================================================================
        // 
        //=================================================================
        protected override void PerformMathematic(ImageBase[] images)
        {
            MilImage milImage1 = images[0].CurrentProcessingImage.GetMilImage();
            int nbImages = images.Count();

            for (int i = 1; i < nbImages; i++)
            {
                milImage1.Arith(2, MIL.M_DIV_CONST);
                MilImage milImage2 = images[i].CurrentProcessingImage.GetMilImage();
                milImage2.Arith(2, MIL.M_DIV_CONST);
                MilImage.Arith(milImage1, milImage2, milImage1, MIL.M_ADD + MIL.M_SATURATION);
            }
        }
    }
}
