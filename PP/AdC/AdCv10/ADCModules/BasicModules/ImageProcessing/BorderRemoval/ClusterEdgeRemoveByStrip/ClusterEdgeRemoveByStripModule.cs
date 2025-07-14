using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using LibProcessing;

namespace BasicModules.BorderRemoval
{
    public class ClusterEdgeRemoveByStripModule : ImageModuleBase
    {
        private ProcessingClass _processClass = null;

        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly IntParameter paramRemoveSize;


        //=================================================================
        // Constructeur
        //=================================================================
        public ClusterEdgeRemoveByStripModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramRemoveSize = new IntParameter(this, "RemoveSize");

            paramRemoveSize.Value = 2;

            _processClass = new ProcessingClassMil();
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("ClusterEdgeRemoveByStrip " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            IImage image = (IImage)obj;

            _processClass.RemoveBorderBand(image.CurrentProcessingImage, paramRemoveSize);

            ProcessChildren(obj);
        }

    }
}
