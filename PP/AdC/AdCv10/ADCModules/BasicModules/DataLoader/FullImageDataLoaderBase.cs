using System.Drawing;

using AcquisitionAdcExchange;

using AdcBasicObjects;

using ADCEngine;

using AdcTools;

using UnitySC.Shared.LibMIL;

using UnitySC.Shared.Tools;

namespace BasicModules.DataLoader
{
    public abstract class FullImageDataLoaderBase : DataLoaderBase
    {
        //=================================================================
        // Constructeur
        //=================================================================
        public FullImageDataLoaderBase(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
        }

        //=================================================================
        // Init
        //=================================================================
        protected override void OnInit()
        {
            // Init de la queue de sortie
            //...........................
            base.OnInit();

            // Init de la layer sous-jacente
            //..............................
            FullImageLayer layer = new FullImageLayer();
            Layer = layer;

            Layer.name = Factory.ModuleName;
            layer.Index = Recipe.NbLayers++;
            layer.Wafer = Wafer;
            layer.MaxDataIndex = nbObjectsIn = 1;
            layer.KeepImageData = RecursiveNeedAllData();

            // Matrice
            //........
            FullImageInputInfo inputInfo = (FullImageInputInfo)InputInfo;
            layer.Matrix = CreateMatrix(inputInfo.MatrixInfo);

            // Contextes Machine
            //..................
            layer.SetContextMachineFromInput(inputInfo.ContextMachineList);

            SetLayerIds(inputInfo);
            layer.MetaData.AddRange(inputInfo.MetaData);
        }

        //=================================================================
        // 
        //=================================================================
        public override ObjectBase ConvertAcquisitionData(AcquisitionDataObject acqDataObject)
        {
            FullImageLayer layer = (FullImageLayer)Layer;

            using (FullImage adcImage = new FullImage(layer))
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
                AcquisitionFullImage acqImage = (AcquisitionFullImage)acqImageObject.AcqData;
                adcImage.Filename = acqImage.Filename;

                // Coordonnées
                adcImage.imageRect = new Rectangle(0, 0, adcImage.CurrentProcessingImage.Width, adcImage.CurrentProcessingImage.Height);
                adcImage.ImageIndex = 1;

                layer.ImageSize = new Size(adcImage.Width, adcImage.Height);

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
            AcquisitionFullImage acqImage = (AcquisitionFullImage)acqObject.AcqData;
            FullImageInputInfo inputInfo = (FullImageInputInfo)InputInfo;

            PathString path = new PathString(inputInfo.Folder) / acqImage.Filename;
            bool berr = path.OptimNetworkPath(out string sErrorMsg);
            if (!berr)
                logWarning("Optim Network path fail : " + sErrorMsg);
            LoadImageFromFile(path, acqImageObject.MilImage);


            return true;
        }

    }
}
