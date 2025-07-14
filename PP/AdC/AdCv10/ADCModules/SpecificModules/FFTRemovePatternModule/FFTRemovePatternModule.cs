using System;
using System.Drawing;
using System.IO;
using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using BasicModules;

using LibProcessing;

using UnitySC.Shared.Tools;

namespace SpecificModules
{
    public class FFTRemovePatternModule : ImageModuleBase
    {
        private static ProcessingClass _processClass = new ProcessingClassMil();

        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly DoubleParameter paramEdgeExclusionum;
        public readonly DoubleParameter paramFFTPeaksThresholdRatio;
        public readonly IntParameter paramFrequencyTolerance;
        public readonly BoolParameter paramComputeDarkImage;
        public readonly BoolParameter paramSaveFFTimgNmsk;
        public readonly BoolParameter paramActivateAdvancedDebug;

        private string _sDebugPath = String.Empty;

        //=================================================================
        // Constructeur
        //=================================================================
        public FFTRemovePatternModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramEdgeExclusionum = new DoubleParameter(this, "EdgeExclusionMicrons");
            paramFFTPeaksThresholdRatio = new DoubleParameter(this, "FFTPeaksThresholdRatio");
            paramFrequencyTolerance = new IntParameter(this, "FrequencyTolerance");

            paramComputeDarkImage = new BoolParameter(this, "ComputeDarkImage");
            paramSaveFFTimgNmsk = new BoolParameter(this, "SaveFFTimgNmsk");
            paramActivateAdvancedDebug = new BoolParameter(this, "ActivateAdvancedDebug");

            paramEdgeExclusionum.Value = 500;
            paramFFTPeaksThresholdRatio.Value = 7.5;
            paramFrequencyTolerance.Value = 1;

            paramComputeDarkImage.Value = false;
            paramSaveFFTimgNmsk.Value = false;
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

            PathString DestinationDirectory = Recipe.WaferLogOutputDir;
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


            double WaferDiameterMicrons = 0.0;
            WaferBase wafer = Recipe.Wafer;
            if (Wafer is NotchWafer)
            {
                WaferDiameterMicrons = (int)((Wafer as NotchWafer).Diameter);
            }
            else if (Wafer is FlatWafer)
            {
                WaferDiameterMicrons = (int)((Wafer as FlatWafer).Diameter);

            }
            else if (Wafer is RectangularWafer)
            {
                float width_um = (Wafer as RectangularWafer).Width;
                float Height_um = (Wafer as RectangularWafer).Height;
                WaferDiameterMicrons = (int)(Math.Sqrt(width_um * width_um + Height_um * Height_um));
            }

            RectangleF rect_microns = wafer.SurroundingRectangleWithFlats;
            Rectangle rect_pixel = Rectangle.Empty;
            PointF WaferCenter = PointF.Empty;

            MatrixBase layermatrix = image.Layer.Matrix;
            double dPixelSizeX = 1.0;
            double dPixelSizeY = 1.0;
            if (layermatrix is RectangularMatrix)
            {
                RectangularMatrix rcm = layermatrix as RectangularMatrix;
                dPixelSizeX = rcm.PixelWidth;
                dPixelSizeY = rcm.PixelHeight; // X et Y devrait être quasiment pareil
                WaferCenter = rcm.WaferCenter;
                rect_pixel = rcm.micronToPixel(rect_microns);
            }
            else
                throw new ApplicationException(" Bad Matrix Type only Rectangular matrix is acceptable");

            double dEdgeremoveRadius_px = (WaferDiameterMicrons * 0.5 - paramEdgeExclusionum.Value) / ((dPixelSizeX + dPixelSizeY) * 0.5);

            string sSavingPathName = _sDebugPath;
            bool DisableSaves = false;

            _processClass.FFTRemovePattern(image.CurrentProcessingImage,
                rect_pixel,
                WaferCenter,
                dEdgeremoveRadius_px,
                paramFFTPeaksThresholdRatio.Value,
                paramFrequencyTolerance.Value,
                paramComputeDarkImage.Value,
                paramSaveFFTimgNmsk.Value,
                paramActivateAdvancedDebug,
                sSavingPathName,
                DisableSaves
                );

            ProcessChildren(obj);
        }
    }
}

