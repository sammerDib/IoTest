using System;
using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using BasicModules;

using UnitySC.Shared.LibMIL;

using Matrox.MatroxImagingLibrary;

namespace ExpertModules.ComplexTransformation.CustomConvolve
{
    public class CustomConvolveModule : ImageModuleBase
    {
        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly KernelParameter paramKernel;
        public readonly BoolParameter paramNormalize;

        //=================================================================
        // Données internes
        //=================================================================
        private MilImage milKernel;

        //=================================================================
        // Constructeur
        //=================================================================
        public CustomConvolveModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramKernel = new KernelParameter(this, "Kernel");
            paramNormalize = new BoolParameter(this, "Normalize");
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();

            milKernel = new MilImage();
            milKernel.Alloc2d(paramKernel.Width, paramKernel.Height, 32 + MIL.M_FLOAT, MIL.M_KERNEL);

            double sum = 0;
            float[,] kernel = new float[paramKernel.Height, paramKernel.Width];
            for (int y = 0; y < paramKernel.Height; y++)
            {
                for (int x = 0; x < paramKernel.Width; x++)
                {
                    kernel[y, x] = (float)paramKernel.Kernel[y, x];
                    sum += Math.Abs(paramKernel.Kernel[y, x]);
                }
            }

            if (paramNormalize && sum != 0)
            {
                for (int y = 0; y < paramKernel.Height; y++)
                {
                    for (int x = 0; x < paramKernel.Width; x++)
                        kernel[y, x] /= (float)sum;
                }
            }

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
            milImage.Convolve(milKernel);

            ProcessChildren(obj);
        }
    }
}
