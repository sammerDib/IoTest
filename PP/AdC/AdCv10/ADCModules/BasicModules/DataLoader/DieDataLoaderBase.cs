using System;
using System.Drawing;

using AcquisitionAdcExchange;

using AdcBasicObjects;

using ADCEngine;

using AdcTools;

using UnitySC.Shared.LibMIL;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;

namespace BasicModules.DataLoader
{
    ///////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////
    public abstract class DieDataLoaderBase : DataLoaderBase
    {
        //=================================================================
        // Constructeur
        //=================================================================
        public DieDataLoaderBase(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            // Init de la queue de sortie
            //...........................
            base.OnInit();

            // Init de la layer sous-jacente
            //..............................
            DieInputInfo inputInfo = (DieInputInfo)InputInfo;

            DieLayer layer = new DieLayer();
            Layer = layer;

            Layer.name = Factory.ModuleName;
            layer.Index = Recipe.NbLayers++;
            layer.Wafer = Wafer;
            layer.NbDies = inputInfo.NbData;
            layer.KeepImageData = RecursiveNeedAllData();

            nbObjectsIn = layer.NbDies;
            if (nbObjectsIn == 0)
                throw new ApplicationException("no Die image in folder: \"" + inputInfo.Folder + "\" basename: " + inputInfo.Basename);

            SetLayerIds(inputInfo);
            layer.MetaData.AddRange(inputInfo.MetaData);

            layer.MinIndexX = inputInfo.MinIndexX;
            layer.MaxIndexX = inputInfo.MaxIndexX;
            layer.MinIndexY = inputInfo.MinIndexY;
            layer.MaxIndexY = inputInfo.MaxIndexY;

            layer.DiePitchX_µm = inputInfo.DiePitchX_µm;
            layer.DiePitchY_µm = inputInfo.DiePicthY_µm;
            layer.DieOriginX_µm = inputInfo.DieOriginX_µm;
            layer.DieOriginY_µm = inputInfo.DieOriginY_µm;
            layer.DieSizeX_µm = inputInfo.DieSizeX_µm;
            layer.DieSizeY_µm = inputInfo.DieSizeY_µm;


            long maxDataIndex = layer.DieIndexes.Area();
            if (maxDataIndex <= 0 || maxDataIndex >= int.MaxValue)  //Sanity check
                throw new ApplicationException("invalid MaxDataIndex: " + maxDataIndex);
            layer.MaxDataIndex = (int)maxDataIndex;

            // Matrice
            //........
            layer.Matrix = CreateMatrix(inputInfo.MatrixInfo);
            layer.WaferCenterX = inputInfo.MatrixInfo.WaferCenterX;
            layer.WaferCenterY = inputInfo.MatrixInfo.WaferCenterY;
            // Contextes Machine
            //..................
            layer.SetContextMachineFromInput(inputInfo.ContextMachineList);
        }

        //=================================================================
        // 
        //=================================================================
        public override ObjectBase ConvertAcquisitionData(AcquisitionDataObject acqDataObject)
        {
            DieLayer layer = (DieLayer)Layer;

            using (DieImage adcImage = new DieImage(layer))
            {
                //-------------------------------------------------------------
                // ProcessingImage
                //-------------------------------------------------------------
                AcquisitionImageObject acqImageObject = (AcquisitionImageObject)acqDataObject;

                MilImage milImage = acqImageObject.MilImage;
                adcImage.OriginalProcessingImage.SetMilImage(milImage);
                adcImage.CurrentIsOriginal = true;

                //-------------------------------------------------------------
                // Métadata
                //-------------------------------------------------------------
                AcquisitionDieImage acqImage = (AcquisitionDieImage)acqImageObject.AcqData;
                adcImage.Filename = acqImage.Filename;

                // Coordonnées
                adcImage.DieIndexX = acqImage.IndexX;
                adcImage.DieIndexY = acqImage.IndexY;
                adcImage.ImageIndex = (acqImage.IndexY - layer.DieIndexes.Top) * layer.DieIndexes.Width + (acqImage.IndexX - layer.DieIndexes.Left);
                adcImage.imageRect = new Rectangle(acqImage.X, acqImage.Y, milImage.SizeX, milImage.SizeY);

                //-------------------------------------------------------------
                // Vérifie que le die est dans le wafer
                //-------------------------------------------------------------
                QuadF micronQuad = layer.Matrix.pixelToMicron(adcImage.imageRect);
                var res = Wafer.IsQuadInside(micronQuad);
                if (res != eCompare.QuadIsInside)
                    throw new ApplicationException("die is outside of wafer, IndexX: " + adcImage.DieIndexX + " IndexY: " + adcImage.DieIndexY);

                adcImage.AddRef();
                return adcImage;
            }
        }

        //=================================================================
        // 
        //=================================================================
        public override bool LoadAcquisitionImageFromFile(AcquisitionDataObject acqObject)
        {
            AcquisitionImageObject acqImageObject = (AcquisitionImageObject)acqObject;
            AcquisitionDieImage acqImage = (AcquisitionDieImage)acqObject.AcqData;
            DieInputInfo inputInfo = (DieInputInfo)InputInfo;

            PathString path = new PathString(inputInfo.Folder) / acqImage.Filename;
            bool berr = path.OptimNetworkPath(out string sErrorMsg);
            if (!berr)
                logWarning("Optim Network path fail : " + sErrorMsg);
            LoadImageFromFile(path, acqImageObject.MilImage);

            return true;
        }
        //=================================================================
        //
        //=================================================================
        public override bool FilterImage(ResultType restyp, int noColumn = -1)
        {
            return (restyp.GetActorType() == ActorType.BrightFieldPattern);
        }

    }
}
