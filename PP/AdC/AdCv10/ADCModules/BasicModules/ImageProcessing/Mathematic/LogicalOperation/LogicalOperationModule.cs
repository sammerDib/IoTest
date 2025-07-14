using System.Linq;

using AdcBasicObjects;

using ADCEngine;

using UnitySC.Shared.LibMIL;

using Matrox.MatroxImagingLibrary;

namespace BasicModules.Mathematic
{
    public class LogicalOperationModule : MathematicModuleBase
    {
        [System.Reflection.Obfuscation(Exclude = true)]
        public enum LogicalOp : long { OR = MIL.M_OR, AND = MIL.M_AND, XOR = MIL.M_XOR };
        public readonly EnumParameter<LogicalOp> paramLogicalOp;

        //=================================================================
        // Constructeur
        //=================================================================
        public LogicalOperationModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramLogicalOp = new EnumParameter<LogicalOp>(this, "Operation");
            paramLogicalOp.Value = LogicalOp.OR;
        }

        //=================================================================
        // 
        //=================================================================
        protected override void PerformMathematic(ImageBase[] images)
        {
            long op = (long)paramLogicalOp.Value;
            MilImage milImage1 = images[0].CurrentProcessingImage.GetMilImage();

            for (int i = 1; i < images.Count(); i++)
            {
                MilImage milImage2 = images[i].CurrentProcessingImage.GetMilImage();
                MilImage.Arith(milImage1, milImage2, milImage1, op);
            }
        }

    }
}
