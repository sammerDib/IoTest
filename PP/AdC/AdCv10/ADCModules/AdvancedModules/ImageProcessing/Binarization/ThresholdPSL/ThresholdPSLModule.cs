using System.Collections.Generic;
using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using BasicModules;

using LibProcessing;

using MergeContext.Context;

namespace AdvancedModules.Binarisation
{
    internal class ThresholdPSLModule : ImageModuleBase
    {
        private static ProcessingClass _processClass = new ProcessingClassMil();

        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly IntParameter paramMinSignificantPSLLevel;
        public readonly IntParameter paramMaxSignificantPSLLevel;


        //=================================================================
        // Constructeur
        //=================================================================
        public ThresholdPSLModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramMinSignificantPSLLevel = new IntParameter(this, "MinSignificantPSLLevel");
            paramMinSignificantPSLLevel.Value = 0;

            paramMaxSignificantPSLLevel = new IntParameter(this, "MaxSignificantPSLLevel");
            paramMaxSignificantPSLLevel.Value = 255;
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("ThresholdPSL " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            IImage image = (IImage)obj;
            LookupTable lutpsl = image.Layer.GetContextMachine<LookupTable>(ConfigurationType.LutPsl.ToString());

            Process(obj, lutpsl);
        }

        //=================================================================
        // 
        //=================================================================
        protected void Process(ObjectBase obj, LookupTable lutpsl)
        {
            IImage image = (IImage)obj;

            List<double> rangeMin = new List<double>();
            List<double> rangeMax = new List<double>();
            bool bInRange = false;
            // on part du principe que les index de la lookuptale sont croissant positif et distant de 1 [0-255]
            // les values de la lut peuvent être comme elles veuleunt
            for (int i = 0; i < lutpsl.LookupValues.Count; i++)
            {
                if (paramMinSignificantPSLLevel.Value <= lutpsl.LookupValues[i].Value && lutpsl.LookupValues[i].Value <= paramMaxSignificantPSLLevel.Value)
                {
                    // is in range
                    if (!bInRange)
                    {
                        // on rentre dans un nouveau range
                        rangeMin.Add(lutpsl.LookupValues[i].Index);
                        rangeMax.Add(lutpsl.LookupValues[i].Index);
                        bInRange = true;
                    }
                    else
                    {
                        // on continue le range courant la valeur max doit être mis à jour
                        rangeMax[rangeMax.Count - 1] = lutpsl.LookupValues[i].Index;
                    }
                }
                else
                {
                    //out of range
                    if (bInRange)
                    {
                        // close current range
                        bInRange = false;
                    }
                }
            }

            _processClass.ThresholdMultiRange(image.CurrentProcessingImage, rangeMin.ToArray(), rangeMax.ToArray());

            ProcessChildren(obj);
        }

    }
}
