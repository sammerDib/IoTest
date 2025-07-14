using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using LibProcessing;

namespace AdvancedModules.ImageProcessing.NoiseReduction.MedianFilter
{
    public class MedianFilterModule : ModuleBase
    {
        private static ProcessingClass _processClass = new ProcessingClassCpp();

        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly IntParameter paramRadius;
        // public IntParameter readonly paramL2CacheMem;

        //=================================================================
        // Constructeur
        //=================================================================
        public MedianFilterModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramRadius = new IntParameter(this, "KernelRadius", 0, 2000);
            //paramL2CacheMem = new IntParameter(this, "L2CacheMemSize");

            paramRadius.Value = 3;
            //paramL2CacheMem.Value = 512 * 1024;
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("process " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            IImage image = (IImage)obj;

            long l2cachesizeMemtopotim = 512L * 1024L;
            //long l2cachesizeMemtopotim = paramL2CacheMem.Value;

            //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            // log("######### Start ALGO Median");
            // sw.Restart();
            _processClass.MedianFilter(image.CurrentProcessingImage, paramRadius.Value, l2cachesizeMemtopotim);
            // sw.Stop();
            // log(String.Format("######### END ALGO Median - Done in {0} ms [ Rad = {1} | L2mem = {2} ]", sw.ElapsedMilliseconds, paramRadius.Value, l2cachesizeMemtopotim));

            ProcessChildren(obj);
        }

    }
}
