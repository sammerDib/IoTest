using System.Collections.Generic;
using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using LibProcessing;

namespace BasicModules.NoiseReduction
{
    public class MilBlobFilteringModule : ImageModuleBase
    {
        private ProcessingClassMil _processClass = new ProcessingClassMil();

        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly ConditionalDoubleParameter paramBreath;
        public readonly ConditionalDoubleParameter paramCompactness;
        public readonly ConditionalDoubleParameter paramConvexPerimeter;
        public readonly ConditionalDoubleParameter paramElongation;
        public readonly ConditionalDoubleParameter paramEulerNumber;
        public readonly ConditionalDoubleParameter paramLength;
        public readonly ConditionalDoubleParameter paramPerimeter;
        public readonly ConditionalDoubleParameter paramRoughness;
        public readonly ConditionalDoubleParameter paramArea;
        public readonly ConditionalDoubleParameter paramAxisPrincipal;

        public readonly EnumParameter<enTypeOfCondition> paramTypeOfCondition;


        //=================================================================
        // Constructeur
        //=================================================================
        public MilBlobFilteringModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramBreath = new ConditionalDoubleParameter(this, "Breath");
            paramCompactness = new ConditionalDoubleParameter(this, "Compactness");
            paramConvexPerimeter = new ConditionalDoubleParameter(this, "ConvexPerimeter");
            paramElongation = new ConditionalDoubleParameter(this, "Elongation");
            paramEulerNumber = new ConditionalDoubleParameter(this, "EulerNumber");
            paramLength = new ConditionalDoubleParameter(this, "Lenght");
            paramPerimeter = new ConditionalDoubleParameter(this, "Perimeter");
            paramRoughness = new ConditionalDoubleParameter(this, "Roughtness");
            paramArea = new ConditionalDoubleParameter(this, "Area");
            paramAxisPrincipal = new ConditionalDoubleParameter(this, "AxisPrincipal");

            paramBreath.IsUsed = false;
            paramCompactness.IsUsed = false;
            paramElongation.IsUsed = false;
            paramEulerNumber.IsUsed = false;
            paramLength.IsUsed = false;
            paramPerimeter.IsUsed = false;
            paramRoughness.IsUsed = false;
            paramArea.IsUsed = true;
            paramAxisPrincipal.IsUsed = false;

            paramArea.Value = 1;

            paramTypeOfCondition = new EnumParameter<enTypeOfCondition>(this, "TypeOfCondition");

            paramTypeOfCondition.Value = enTypeOfCondition.en_Greater;
        }

        //=================================================================
        //
        //=================================================================
        private List<BlobFilteringParameters> _listCharacteristicUsed = new List<BlobFilteringParameters>();

        protected override void OnInit()
        {
            base.OnInit();

            _listCharacteristicUsed.Add(new BlobFilteringParameters(paramBreath, paramBreath.IsUsed, enCharacteristicMILBlobFiltering.en_Breath));
            _listCharacteristicUsed.Add(new BlobFilteringParameters(paramCompactness, paramCompactness.IsUsed, enCharacteristicMILBlobFiltering.en_Compactness));
            _listCharacteristicUsed.Add(new BlobFilteringParameters(paramConvexPerimeter, paramConvexPerimeter.IsUsed, enCharacteristicMILBlobFiltering.en_ConvexPerimeter));
            _listCharacteristicUsed.Add(new BlobFilteringParameters(paramElongation, paramElongation.IsUsed, enCharacteristicMILBlobFiltering.en_Elongation));
            _listCharacteristicUsed.Add(new BlobFilteringParameters(paramEulerNumber, paramEulerNumber.IsUsed, enCharacteristicMILBlobFiltering.en_EulerNumber));
            _listCharacteristicUsed.Add(new BlobFilteringParameters(paramLength, paramLength.IsUsed, enCharacteristicMILBlobFiltering.en_Length));
            _listCharacteristicUsed.Add(new BlobFilteringParameters(paramPerimeter, paramPerimeter.IsUsed, enCharacteristicMILBlobFiltering.en_Perimeter));
            _listCharacteristicUsed.Add(new BlobFilteringParameters(paramRoughness, paramRoughness.IsUsed, enCharacteristicMILBlobFiltering.en_Roughness));
            _listCharacteristicUsed.Add(new BlobFilteringParameters(paramArea, paramArea.IsUsed, enCharacteristicMILBlobFiltering.en_Area));
            _listCharacteristicUsed.Add(new BlobFilteringParameters(paramAxisPrincipal, paramAxisPrincipal.IsUsed, enCharacteristicMILBlobFiltering.en_AxisPrincipal));
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("MilBlobFiltering " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            IImage image = (IImage)obj;

            _processClass.MilBlobFiltering(image.CurrentProcessingImage, _listCharacteristicUsed, paramTypeOfCondition);

            ProcessChildren(obj);
        }

    }
}
