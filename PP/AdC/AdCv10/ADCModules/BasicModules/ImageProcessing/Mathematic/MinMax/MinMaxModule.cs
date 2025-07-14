using System.Linq;

using AdcBasicObjects;

using ADCEngine;

using UnitySC.Shared.LibMIL;

using Matrox.MatroxImagingLibrary;

namespace BasicModules.Mathematic
{
    public class MinMaxModule : MathematicModuleBase
    {
        [System.Reflection.Obfuscation(Exclude = true)]
        public enum MinMaxOp : long { MIN = MIL.M_MIN, MAX = MIL.M_MAX };
        public readonly EnumParameter<MinMaxOp> paramOp;
        public readonly ConditionalDoubleParameter paramConst;

        //=================================================================
        // Constructeur
        //=================================================================
        public MinMaxModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramConst = new ConditionalDoubleParameter(this, "const");
            paramOp = new EnumParameter<MinMaxOp>(this, "Operation");
            paramOp.Value = MinMaxOp.MIN;
        }

        //=================================================================
        // 
        //=================================================================
        protected override void PerformMathematic(ImageBase[] images)
        {
            long op = (long)paramOp.Value;
            MilImage milImage1 = images[0].CurrentProcessingImage.GetMilImage();

            for (int i = 1; i < images.Count(); i++)
            {
                MilImage milImage2 = images[i].CurrentProcessingImage.GetMilImage();
                MilImage.Arith(milImage1, milImage2, milImage1, op);
            }

            if (paramConst.IsUsed)
            {
                op = (paramOp == MinMaxOp.MIN) ? MIL.M_MIN_CONST : MIL.M_MAX_CONST;
                milImage1.Arith(paramConst, op);
            }
        }

    }
}
