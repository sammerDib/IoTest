using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using UnitySC.Shared.LibMIL;

using Matrox.MatroxImagingLibrary;

namespace BasicModules.Mathematic
{
    public class LogarithmModule : ImageModuleBase
    {
        //=================================================================
        // Paramètres du XML
        //=================================================================
        [System.Reflection.Obfuscation(Exclude = true)]
        public enum LogarithmOp : long { LN = MIL.M_LN, LOG2 = MIL.M_LOG2, LOG10 = MIL.M_LOG10 };
        public readonly EnumParameter<LogarithmOp> paramLogarithmOp;

        //=================================================================
        // Constructeur
        //=================================================================
        public LogarithmModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramLogarithmOp = new EnumParameter<LogarithmOp>(this, "Operation");
            paramLogarithmOp.Value = LogarithmOp.LN;
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("process " + obj);
            Interlocked.Increment(ref nbObjectsIn);
            IImage image = (IImage)obj;

            long op = (long)paramLogarithmOp.Value;
            MilImage milImage = image.CurrentProcessingImage.GetMilImage();
            milImage.Arith(op);

            ProcessChildren(obj);
        }

    }
}
