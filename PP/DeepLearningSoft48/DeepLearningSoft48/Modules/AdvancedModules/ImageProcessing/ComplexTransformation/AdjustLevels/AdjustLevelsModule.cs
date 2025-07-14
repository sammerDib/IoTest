using System;

using DeepLearningSoft48.Modules.Parameters;

using UnitySC.Shared.LibMIL;

using Matrox.MatroxImagingLibrary;

namespace DeepLearningSoft48.Modules.AdvancedModules.ImageProcessing.ComplexTransformation.AdjustLevels
{
    public class AdjustLevelsModule : ModuleBase
    {
        //=================================================================
        // XML Parameters
        //=================================================================
        public readonly IntParameter paramMin;
        public readonly IntParameter paramMax;

        //=================================================================
        // Constructor
        //=================================================================
        public AdjustLevelsModule(IModuleFactory factory)
            : base(factory)
        {
            paramMin = new IntParameter(this, "Min Level", 0, 255);
            paramMax = new IntParameter(this, "Max Level", 0, 255);
            paramMin.Value = 0;
            paramMax.Value = 255;
        }

        //=================================================================
        // Process Method
        //=================================================================
        public override MilImage Process(MilImage imgToProcess)
        {
            if (Validate() == null)
            {
                imgToProcess.Equalization(MIL.M_UNIFORM, MIL.M_NULL, paramMin, paramMax);
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
            if (paramMin.Value >= paramMax.Value)
            {
                return "Invalid Parameters Min >= Max";
            }
            return null;
        }
    }
}
