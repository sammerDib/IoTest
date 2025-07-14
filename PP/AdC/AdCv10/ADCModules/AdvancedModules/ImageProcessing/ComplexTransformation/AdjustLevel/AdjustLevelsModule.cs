using System;
using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using BasicModules;

using LibProcessing;

namespace AdvancedModules.ImageProcessing.ComplexTransformation.AdjustLevel
{
    public class AdjustLevelsModule : ImageModuleBase
    {
        private static ProcessingClass _processClass = new ProcessingClassCpp();

        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly IntParameter paramMin;
        public readonly IntParameter paramMax;


        //=================================================================
        // Constructeur
        //=================================================================
        public AdjustLevelsModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramMin = new IntParameter(this, "MinLevel", 0, 255);
            paramMax = new IntParameter(this, "MaxLevel", 0, 255);
            paramMin.Value = 0;
            paramMax.Value = 255;
        }

        protected override void OnInit()
        {
            base.OnInit();

            if (paramMin.Value >= paramMax.Value)
            {
                throw new ApplicationException(Name + " Invalid Parameters Min >= Max");
            }
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug(obj.ToString());
            Interlocked.Increment(ref nbObjectsIn);

            IImage image = (IImage)obj;

            _processClass.LinearDynamicScale(image.CurrentProcessingImage, paramMin.Value, paramMax.Value);

            ProcessChildren(obj);
        }
    }
}
