using System.Threading;

using ADCEngine;

using LibProcessing;

using MergeContext.Context;

using UnitySC.Shared.Tools;

namespace AdvancedModules.Binarisation
{
    internal class ThresholdPSLFileModule : ThresholdPSLModule
    {
        private static ProcessingClass _processClass = new ProcessingClassMil();
        private LookupTable lutpsl;

        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly FileParameter paramPSLFile;


        //=================================================================
        // Constructeur
        //=================================================================
        public ThresholdPSLFileModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramPSLFile = new FileParameter(this, "PSLLUT", "PSL lut (*.xml)|*.xml");
        }

        protected override void OnInit()
        {
            base.OnInit();
            lutpsl = XML.Deserialize<LookupTable>(paramPSLFile.FullFilePath);
        }

        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("ThresholdPSLFile " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            Process(obj, lutpsl);
        }

    }
}
