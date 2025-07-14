using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using BasicModules;

using LibProcessing;

using MergeContext.Context;

namespace SpecificModules.Threshold3DModule
{
    public class Threshold3DModule : ImageModuleBase
    {
        private static ProcessingClass _processClass = new ProcessingClassMil();

        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly BoolParameter paramUseAbsoluteHeight;
        public readonly DoubleParameter paramLowHeight;
        public readonly DoubleParameter paramHightHeight;
        public readonly DoubleParameter paramLowSigma;
        public readonly DoubleParameter paramHightSigma;


        //=================================================================
        // Constructeur
        //=================================================================
        public Threshold3DModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramUseAbsoluteHeight = new BoolParameter(this, "UseAbsoluteHeight");
            paramLowHeight = new DoubleParameter(this, "LowHeight");
            paramHightHeight = new DoubleParameter(this, "HightHeight");
            paramLowSigma = new DoubleParameter(this, "LowSigma");
            paramHightSigma = new DoubleParameter(this, "HightSigma");
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("process " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            IImage image = (IImage)obj;

            double[] arMinRange = new double[2];
            double[] arMaxRange = new double[2];

            arMinRange[0] = paramLowHeight.Value - paramLowSigma.Value;
            arMaxRange[0] = paramLowHeight.Value + paramLowSigma.Value;

            arMinRange[1] = paramHightHeight.Value - paramHightSigma.Value;
            arMaxRange[1] = paramHightHeight.Value + paramHightSigma.Value;

            if (!paramUseAbsoluteHeight.Value)
            {
                WaferPosition waferPosition = image.Layer.GetContextMachine<WaferPosition>(ConfigurationType.WaferPosition.ToString());
                if (waferPosition != null)
                {
                    arMinRange[0] += (double)waferPosition.Background3DValue;
                    arMaxRange[0] += (double)waferPosition.Background3DValue;
                    arMinRange[1] += (double)waferPosition.Background3DValue;
                    arMaxRange[1] += (double)waferPosition.Background3DValue;
                }
                else
                {
                    logDebug("Background3DValue is not defined in the current context machine");
                }
            }

            _processClass.ThresholdMultiRange(image.CurrentProcessingImage, arMinRange, arMaxRange);

            ProcessChildren(obj);
        }
    }
}
