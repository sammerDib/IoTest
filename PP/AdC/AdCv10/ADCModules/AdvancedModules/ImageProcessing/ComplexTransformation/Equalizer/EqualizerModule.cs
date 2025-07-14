using System;
using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using BasicModules;

using LibProcessing;

namespace AdvancedModules.ComplexTransformation
{
    public class EqualizerModule : ImageModuleBase
    {
        private static ProcessingClass _processClass = new ProcessingClassMil();

        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly EnumParameter<enEqualizationOperation> paramTypeOfEqualizer;
        public readonly DoubleParameter paramAlpha;
        public readonly DoubleParameter paramLowest;
        public readonly DoubleParameter paramHighest;



        //=================================================================
        // Constructeur
        //=================================================================
        public EqualizerModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramAlpha = new DoubleParameter(this, "AlphaValue", min: 0);
            paramAlpha.Value = 1;
            paramLowest = new DoubleParameter(this, "LowestValue");
            paramLowest.Value = 0;
            paramHighest = new DoubleParameter(this, "HighestValue");
            paramHighest.Value = 255;

            paramTypeOfEqualizer = new EnumParameter<enEqualizationOperation>(this, "TypeOfEqualizer");
            paramTypeOfEqualizer.ValueChanged += (t) =>
            {
                switch (t)
                {
                    case enEqualizationOperation.Exponential:
                    case enEqualizationOperation.RayLeigh:
                        paramAlpha.IsEnabled = true;
                        break;
                    case enEqualizationOperation.HyperCubeRoot:
                    case enEqualizationOperation.HyperLogarithm:
                    case enEqualizationOperation.Uniform:
                        paramAlpha.IsEnabled = false;
                        break;
                    default:
                        throw new ApplicationException($"unknown enEqualizationOperation value: {t}");
                }
            };
            paramTypeOfEqualizer.Value = enEqualizationOperation.Uniform;
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("process " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            IImage image = (IImage)obj;

            _processClass.Equalization(image.CurrentProcessingImage, paramTypeOfEqualizer, paramAlpha, paramLowest, paramHighest);

            ProcessChildren(obj);
        }

        //=================================================================
        // 
        //=================================================================
        public override string Validate()
        {
            string err = base.Validate();
            if (err != null)
                return err;

            if (paramTypeOfEqualizer.Value == enEqualizationOperation.Exponential ||
                paramTypeOfEqualizer.Value == enEqualizationOperation.RayLeigh)
            {
                if (paramAlpha.Value <= 0)
                    return $"{paramAlpha.Label} must be strictly positive";
            }

            if (paramHighest.Value <= paramLowest.Value)
                return $"{paramLowest.Label} must be strictly less than {paramLowest.Label}";

            return null;
        }

    }
}