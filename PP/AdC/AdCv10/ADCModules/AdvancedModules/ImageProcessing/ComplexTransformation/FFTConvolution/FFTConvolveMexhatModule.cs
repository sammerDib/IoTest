using System;
using System.IO;
using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using BasicModules;

using LibProcessing;

namespace AdvancedModules.ComplexTransformation
{
    public class FFTConvolveMexhatModule : ImageModuleBase
    {
        private static ProcessingClass _processClass = new ProcessingClassMil();

        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly IntParameter paramWidthKernel;
        public readonly BoolParameter paramSaveFTTImg;
        public readonly BoolParameter paramActivateAdvancedDebug;

        private string _sDebugPath = String.Empty;
        //=================================================================
        // Constructeur
        //=================================================================
        public FFTConvolveMexhatModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramWidthKernel = new IntParameter(this, "Mexhat Kernel Width");

            paramSaveFTTImg = new BoolParameter(this, "Save the FFT image to disk");
            paramActivateAdvancedDebug = new BoolParameter(this, "ActivateAdvancedDebug");

            paramWidthKernel.Value = 25;

            paramSaveFTTImg.Value = true;
            paramActivateAdvancedDebug.Value = false;


        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();

            string sLotId = Wafer.GetWaferInfo(AcquisitionAdcExchange.eWaferInfo.LotID);
            string sRecipeId = Wafer.GetWaferInfo(AcquisitionAdcExchange.eWaferInfo.ToolRecipe);

            var DestinationDirectory = Recipe.WaferLogOutputDir;
            string sErr;
            if (!DestinationDirectory.OptimNetworkPath(out sErr))
            {
                logWarning("Unable To optimize network path : " + sErr);
            }
            Directory.CreateDirectory(DestinationDirectory);
            _sDebugPath = String.Format(@"{0}\{1}_Slot{2}", DestinationDirectory, DisplayName, Wafer.GetWaferInfo(AcquisitionAdcExchange.eWaferInfo.SlotID));

        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("process " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            IImage image = (IImage)obj;



            string sSavingPathName = _sDebugPath;

            _processClass.FFTConvolveMexhat(image.CurrentProcessingImage,
                paramWidthKernel.Value,
                paramSaveFTTImg.Value,
                paramActivateAdvancedDebug,
                sSavingPathName
                );

            ProcessChildren(obj);
        }
    }
}

