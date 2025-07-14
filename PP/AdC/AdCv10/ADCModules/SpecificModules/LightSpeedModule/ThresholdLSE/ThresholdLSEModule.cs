using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using UnitySC.Shared.LibMIL;

using Matrox.MatroxImagingLibrary;

namespace SpecificModules.LightSpeedModule
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class ThresholdLSEModule : ModuleBase
    {
        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly ConditionalDoubleParameter paramFactor;
        public readonly ConditionalDoubleParameter paramOffset;

        //=================================================================
        // Constructeur
        //=================================================================
        public ThresholdLSEModule(IModuleFactory facotory, int id, Recipe recipe)
            : base(facotory, id, recipe)
        {
            paramFactor = new ConditionalDoubleParameter(this, "Factor");
            paramFactor.Value = 1;
            paramOffset = new ConditionalDoubleParameter(this, "Offset");
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("process " + obj);
            Interlocked.Increment(ref nbObjectsIn);
            IImage image = (IImage)obj;

            double threshold = double.Parse(image.Layer.MetaData[AcquisitionAdcExchange.LayerMetaData.lseMin_nm]);
            if (paramFactor.IsUsed)
                threshold *= paramFactor.Value;
            if (paramOffset.IsUsed)
                threshold += paramOffset.Value;

            MilImage milImage = image.CurrentProcessingImage.GetMilImage();
            using (MilImage milBinaryImage = new MilImage())
            {
                milBinaryImage.Alloc2d(milImage.OwnerSystem, milImage.SizeX, milImage.SizeY, 8 + MIL.M_UNSIGNED, milImage.Attribute);
                MilImage.Binarize(milImage, milBinaryImage, MIL.M_GREATER_OR_EQUAL, threshold, MIL.M_NULL);
                image.CurrentProcessingImage.SetMilImage(milBinaryImage);
            }

            ProcessChildren(obj);
        }

    }
}
