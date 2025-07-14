using System;
using System.Drawing;

using AcquisitionAdcExchange;

using AdcBasicObjects;

using ADCEngine;

using AdcTools;

using UnitySC.Shared.LibMIL;

using UnitySC.Shared.Tools;

namespace BasicModules.DataLoader
{
    public abstract class MosaicDataLoaderBase : DataLoaderBase
    {
        //=================================================================
        // Constructeur
        //=================================================================
        public MosaicDataLoaderBase(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
        }

        //=================================================================
        // Init
        //=================================================================
        protected override void OnInit()
        {
            //-------------------------------------------------------------
            // Init de la queue de sortie
            //-------------------------------------------------------------
            base.OnInit();

            //-------------------------------------------------------------
            // Init de la layer sous-jacente
            //-------------------------------------------------------------
            MosaicLayer layer = new MosaicLayer();
            Layer = layer;

            layer.name = Factory.ModuleName;
            layer.Index = Recipe.NbLayers++;
            layer.Wafer = Wafer;

            MosaicInputInfo inputInfo = (MosaicInputInfo)InputInfo;
            layer.NbLines = inputInfo.NbLines;
            layer.NbColumns = inputInfo.NbColumns;
            layer.MosaicImageSize = new Size(inputInfo.MosaicImageWidth, inputInfo.MosaicImageHeight);
            layer.MaxDataIndex = layer.NbColumns * layer.NbLines;
            layer.KeepImageData = RecursiveNeedAllData();

            SetLayerIds(inputInfo);
            layer.MetaData.AddRange(inputInfo.MetaData);

            nbObjectsIn = inputInfo.NbData;
            if (nbObjectsIn == 0)
                nbObjectsIn = Layer.MaxDataIndex;
            if (nbObjectsIn == 0)
                throw new ApplicationException("no mosaic images in folder \"" + inputInfo.Folder + "\", basename=\"" + inputInfo.Basename + "\"");

            // Matrice
            //........
            layer.Matrix = CreateMatrix(inputInfo.MatrixInfo);

            // Contextes Machine
            //..................
            layer.SetContextMachineFromInput(inputInfo.ContextMachineList);
        }

        //=================================================================
        // 
        //=================================================================
        public override ObjectBase ConvertAcquisitionData(AcquisitionDataObject acqDataObject)
        {
            MosaicLayer layer = (MosaicLayer)Layer;

            using (MosaicImage adcImage = new MosaicImage(layer))
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
                AcquisitionMosaicImage acqImage = (AcquisitionMosaicImage)acqImageObject.AcqData;
                adcImage.Filename = acqImage.Filename;

                // Coordonnées
                adcImage.Line = acqImage.Line;
                adcImage.Column = acqImage.Column;
                adcImage.ImageIndex = adcImage.Column * layer.NbLines + adcImage.Line;
                adcImage.imageRect = PixelRect(adcImage.Line, adcImage.Column);

                adcImage.AddRef();
                return adcImage;
            }
        }

        //=================================================================
        // 
        //=================================================================
        protected Rectangle PixelRect(int line, int column)
        {
            MosaicLayer layer = (MosaicLayer)Layer;

            int x = column * layer.MosaicImageSize.Width;
            int y = line * layer.MosaicImageSize.Height;

            Rectangle rect = new Rectangle(x, y, layer.MosaicImageSize.Width, layer.MosaicImageSize.Height);
            return rect;
        }

        //=================================================================
        // 
        //=================================================================
        public override bool LoadAcquisitionImageFromFile(AcquisitionDataObject acqObject)
        {
            AcquisitionImageObject acqImageObject = (AcquisitionImageObject)acqObject;
            MosaicInputInfo inputInfo = (MosaicInputInfo)InputInfo;
            AcquisitionMosaicImage acqImage = (AcquisitionMosaicImage)acqObject.AcqData;

            //-------------------------------------------------------------
            // Est-ce qu'il faut charger cette image ?
            //-------------------------------------------------------------
            Rectangle pixelRect = PixelRect(acqImage.Line, acqImage.Column);
            QuadF micronQuad = Layer.Matrix.pixelToMicron(pixelRect);
            if (Wafer.IsQuadInside(micronQuad) == eCompare.QuadIsOutside)
            {
                logWarning("out of wafer, skipping image: " + acqImage);
                return false;
            }
            if (acqImage.Column >= inputInfo.NbColumns || acqImage.Line >= inputInfo.NbLines)
                throw new ApplicationException($"invalid image C{acqImage.Column}_L{acqImage.Line} in mosaic of size {inputInfo.NbColumns}x{inputInfo.NbLines}");

            //-------------------------------------------------------------
            // Chargement de l'image
            //-------------------------------------------------------------
            PathString path = new PathString(inputInfo.Folder) / acqImage.Filename;
            bool berr = path.OptimNetworkPath(out string sErrorMsg);
            if (!berr)
                logWarning("Optim Network path fail : " + sErrorMsg);
            LoadImageFromFile(path, acqImageObject.MilImage);

            return true;
        }

    }
}
