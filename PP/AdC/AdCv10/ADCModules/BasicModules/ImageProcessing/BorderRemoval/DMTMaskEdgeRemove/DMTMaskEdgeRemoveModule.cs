using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using BasicModules.DataLoader;

using LibProcessing;

using Matrox.MatroxImagingLibrary;

using UnitySC.Shared.LibMIL;

namespace BasicModules.BorderRemoval.DMTMaskEdgeRemove
{
    public class DMTMaskEdgeRemoveModule : ImageModuleBase
    {
        
        private readonly ProcessingClassMil _processingClass = new ProcessingClassMil();
        
        public DMTMaskEdgeRemoveModule(IModuleFactory factory, int id, Recipe recipe) : base(factory, id, recipe)
        {
        }

        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("ApplyDMTMaskModule" + obj);
            Interlocked.Increment(ref nbObjectsIn);
            
            var image = (ImageBase)obj;
            
            var inputInfo = Recipe.InputInfoList[image.Layer.Index];
            if (inputInfo is FullImageWithMaskInputInfo inputInfoWithMask)
            {
                using (var mask = new ProcessingImage())
                {
                    DataLoaderBase.LoadImageFromFile(inputInfoWithMask.MaskFilePath, mask.GetMilImage());
                    _processingClass.Binarisation(mask, 1);
                    MilImage.Arith(image.CurrentProcessingImage.GetMilImage(), mask.GetMilImage(), image.CurrentProcessingImage.GetMilImage(), MIL.M_AND);        
                }
            }
            else
            {
                logError("Mask file path not provided, cannot apply mask");
            }
            
            ProcessChildren(obj);
        }
    }
}
