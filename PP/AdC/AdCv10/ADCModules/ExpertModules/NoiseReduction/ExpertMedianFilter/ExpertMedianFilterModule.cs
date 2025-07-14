using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using UnitySC.Shared.LibMIL;

using Matrox.MatroxImagingLibrary;

namespace ExpertModules.NoiseReduction.ExpertMedianFilter
{
    public class ExpertMedianFilterModule : ModuleBase
    {
        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly IntParameter paramKernelWidth;
        public readonly IntParameter paramKernelHeight;

        //=================================================================
        // Données internes
        //=================================================================
        private MilImage milKernel;

        //=================================================================
        // Constructeur
        //=================================================================
        public ExpertMedianFilterModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramKernelWidth = new IntParameter(this, "KernelWidth", 1, 100);
            paramKernelHeight = new IntParameter(this, "KernelHeight", 1, 100);

            paramKernelWidth.Value = paramKernelHeight.Value = 3;
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();

            int w = paramKernelWidth;
            int h = paramKernelHeight;
            uint[,] kernel = new uint[h, w];
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                    kernel[i, j] = 1;
            }

            milKernel = new MilImage();
            milKernel.Alloc2d(w, h, 32 + MIL.M_UNSIGNED, MIL.M_STRUCT_ELEMENT);
            milKernel.Put(kernel);
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnStopping(eModuleState oldState)
        {
            if (milKernel != null)
            {
                milKernel.Dispose();
                milKernel = null;
            }

            base.OnStopping(oldState);
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("process " + obj);
            Interlocked.Increment(ref nbObjectsIn);
            IImage image = (IImage)obj;

            MilImage milImage = image.CurrentProcessingImage.GetMilImage();
            milImage.Rank(milKernel, MIL.M_MEDIAN, MIL.M_GRAYSCALE);

            ProcessChildren(obj);
        }

    }
}
