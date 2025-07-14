using System;
using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using LibProcessing;

namespace AdvancedModules.ImageProcessing.NoiseReduction.MedianFloatFilter
{
    public class MedianFloatFilterModule : ModuleBase
    {
        private static ProcessingClass _processClass = new ProcessingClassCpp();

        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly IntParameter paramRadiusX;
        public readonly IntParameter paramRadiusY;
        public readonly IntParameter paramNbCores;


        //=================================================================
        // Constructeur
        //=================================================================
        public MedianFloatFilterModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            int coreCount = Environment.ProcessorCount;
            paramRadiusX = new IntParameter(this, "KernelRadiusX", 1, 2000);
            paramRadiusY = new IntParameter(this, "KernelRadiusY", 1, 2000);

            paramNbCores = new IntParameter(this, "NbCores", 1, coreCount);

            paramRadiusX.Value = 5;
            paramRadiusY.Value = 5;

            paramNbCores.Value = coreCount - 1;
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("process " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            IImage image = (IImage)obj;
            int NbCoreUsed = paramNbCores.Value;
            int coreCount = Environment.ProcessorCount;
            if (NbCoreUsed > coreCount)
                NbCoreUsed = coreCount;
            else if (NbCoreUsed < 1)
                NbCoreUsed = 1;

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

            // log("######### Start ALGO Median Float on " + NbCoreUsed + " Cores");
            // sw.Restart();
            _processClass.MedianFloatFilter(image.CurrentProcessingImage, paramRadiusX.Value, paramRadiusY.Value, NbCoreUsed);
            // sw.Stop();
            // log(String.Format("######### END ALGO Median Float - Done in {0} ms [ Rad = [{1},{2}] | NBCore = {3} ]", sw.ElapsedMilliseconds, paramRadiusX.Value,  paramRadiusY.Value, NbCoreUsed));

            ProcessChildren(obj);
        }

    }
}
