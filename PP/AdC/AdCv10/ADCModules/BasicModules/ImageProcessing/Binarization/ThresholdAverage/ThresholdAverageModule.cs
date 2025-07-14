using System;
using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using LibProcessing;

namespace BasicModules.Binarisation
{
    public class ThresholdAverageModule : ImageModuleBase
    {
        private static ProcessingClass _processClass = new ProcessingClassMil();

        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly DoubleParameter paramDeviation;
        public readonly IntParameter paramMinSignificantGreyLevel;


        //=================================================================
        // Constructeur
        //=================================================================
        public ThresholdAverageModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramDeviation = new DoubleParameter(this, "Deviation");
            paramDeviation.Value = 10;
            paramMinSignificantGreyLevel = new IntParameter(this, "MinSignificantGreyLevel");
            paramMinSignificantGreyLevel.Value = 5;
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("ThresholdAverage " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            IImage image = (IImage)obj;
            //MilImage milImage = image.GetMilImage();

            double lowParam = 0;
            double HighParam = 0;

            double Average = _processClass.GetGreyLevelAverage(image.CurrentProcessingImage, (int)paramMinSignificantGreyLevel);
            double Deviation = Average * paramDeviation / 100;
            lowParam = Math.Max(0, Average - Deviation);
            HighParam = Math.Min(Constants.constWhiteGreyLevel, Average + Deviation);

            _processClass.Threshold(image.CurrentProcessingImage, lowParam, HighParam);

            ProcessChildren(obj);
        }

    }
}
