using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using ADCEngine;
using AdcTools;
using AcquisitionAdcExchange;
using BasicModules.DataLoader;
using AdcBasicObjects;
using UnitySC.Shared.LibMIL;
using UnitySC.Shared.Data.Enum;
using Format001;
using UnitySC.Shared.Tools;

namespace DataLoaderModule_SICPL
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class SicPlModule : MosaicDataLoaderBase
    {
        [System.Reflection.Obfuscation(Exclude = true)]
        //public enum eImageType { Image_X0_Y0, Image_X0_Y1, Image_X0_Y2, Image_X1_Y0, Image_X1_Y1, Image_X1_Y2, Image_X2_Y0, Image_X2_Y1, Image_X2_Y2, Image_X3_Y0, Image_X3_Y1, Image_X3_Y2};
		public override string DisplayName { get { return Factory.ModuleName + "-" + Id /*+ "\n" + paramImageType.Value.ToString()*/; } }
        public override string DisplayNameInParamView { get { return Factory.Label /*+ "\n" + paramImageType.Value.ToString()*/; } }
        //public override string LayerName { get { return paramImageType.Value.ToString(); } }

        //public override eModuleID DataLoaderType => eModuleID.PL;
        // ? not sure what this is for, maybe it defines the format of image expected ?

        //public override IEnumerable<int> Channels => GetExpectedChannelIds();

        public override ActorType DataLoaderActorType { get; }
        public override IEnumerable<ResultType> CompatibleResultTypes { get; }

        //public enum SicPlChannelId { PlChannelID_0, PlChannelID_1, PlChannelID_2, PlChannelID_3 };
        private int[] coef = {1,3,5,7,9,11,13,15,17 };
        private int offsetAllImagesX = 0;
        private int offsetAllImagesY = 0;

        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly EnumParameter<PrmSicPlChannelId.SicPlChannelId> paramChannelId; 
        
        public IntParameter paramOffset_X;
        public IntParameter paramOffset_Y;


        //=================================================================
        // Constructeur
        //=================================================================
        public SicPlModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramChannelId = new EnumParameter<PrmSicPlChannelId.SicPlChannelId>(this, "SourceImage");
            paramOffset_X = new IntParameter(this, "Offset X pixel",int.MinValue);
            paramOffset_Y = new IntParameter(this, "Offset Y pixel",int.MinValue);

            paramOffset_X.Value = 1000;  // Pixel
            paramOffset_Y.Value = 1000;  // Pixel
        }

        //=================================================================
        //
        //=================================================================
        public override bool FilterImage(ResultType resultType, int NoCumn = -1)
        {
            //-------------------------------------------------------------
            // Module ID
            //-------------------------------------------------------------
            if (resultType.GetActorType() != ActorType.EMERA)
                    return false;

            //-------------------------------------------------------------
            // Channel ID
            //-------------------------------------------------------------
            //bool match = GetExpectedChannelIds().Contains(channelID);
            //if (!match)
            //    return false;

            //-------------------------------------------------------------
            // Image Index
            //-------------------------------------------------------------
            int expectedChannelID;
            switch (paramChannelId.Value)
            {
                case PrmSicPlChannelId.SicPlChannelId.PlChannelID_0:
                    expectedChannelID = (int)eChannelID.PhotoluminescenceChannel_0;
                    break;
                case PrmSicPlChannelId.SicPlChannelId.PlChannelID_1:
                    expectedChannelID = (int)eChannelID.PhotoluminescenceChannel_1;
                    break;
                case PrmSicPlChannelId.SicPlChannelId.PlChannelID_2:
                    expectedChannelID = (int)eChannelID.PhotoluminescenceChannel_2;
                    break;
                case PrmSicPlChannelId.SicPlChannelId.PlChannelID_3:
                    expectedChannelID = (int)eChannelID.PhotoluminescenceChannel_3;
                    break;
                default:
                    throw new ApplicationException("unkown image type: " + paramChannelId.Value);
            }

            //match = (channelID == expectedChannelID);
            return true;// match;
		}
        public List<int> GetExpectedChannelIds()
        {
            return new List<int>()
            {
                (int)eChannelID.PhotoluminescenceChannel_0,
                (int)eChannelID.PhotoluminescenceChannel_1,
                (int)eChannelID.PhotoluminescenceChannel_2,
                (int)eChannelID.PhotoluminescenceChannel_3,
            };
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();
            offsetAllImagesX = (((((MosaicInputInfo)this.InputInfo).NbColumns / 2)-1) * 2 + 2) * paramOffset_X;
            offsetAllImagesY = (((((MosaicInputInfo)this.InputInfo).NbLines / 2) - 1) * 2 + 2) * paramOffset_Y;

        }
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            //base.Process(parent, obj);
            CheckMemoryLimit();

            //-------------------------------------------------------------
            // Est-ce une image pour ce DataLoader ?
            //-------------------------------------------------------------
            AcquisitionImageObject acqImageObject = obj as AcquisitionImageObject;
            if (acqImageObject == null)
                return;

            AcquisitionMilImage acqImage = (AcquisitionMilImage)acqImageObject.AcqData;
            //bool b = FilterImage(acqImage.ModuleID, acqImage.ChannelID, acqImage.ImageID);
            //if (!b)
            //    return;  to verif

            // On enregistre l'image dans la recette mergée pour être capable de la rejouer
            //.............................................................................
            if (!IsReprocessing)
            {
                if (InputInfo != null && InputInfo.DataLoaderIdList.Count() > 0 && InputInfo.DataLoaderIdList[0] == this.Id)
                    ((InspectionInputInfoBase)InputInfo).InputDataList.Add(acqImage);
            }

            //-------------------------------------------------------------
            // Convertit en ImageBase
            //-------------------------------------------------------------
            using (ImageBase adcImage = (ImageBase)ConvertAcquisitionData(acqImageObject))
            {
                // Ajoute l'image à la Layer
                //..........................
                ((ImageLayerBase)Layer).AddImage(adcImage);

                // Queue
                //......
                logDebug("queueing image: " + adcImage);
                outputQueue.Enqueue(adcImage);
                // Ne pas tester le résultat car la queue peut être fermée lors d'un abort
            }
        }

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
                adcImage.imageRect = SicPixelRect(adcImage.Line, adcImage.Column);

                adcImage.AddRef();
                return adcImage;
            }
        }



        public override bool LoadAcquisitionImageFromFile(AcquisitionDataObject acqObject)
        {
            AcquisitionImageObject acqImageObject = (AcquisitionImageObject)acqObject;
            MosaicInputInfo inputInfo = (MosaicInputInfo)InputInfo;
            AcquisitionMosaicImage acqImage = (AcquisitionMosaicImage)acqObject.AcqData;

            //-------------------------------------------------------------
            // Est-ce qu'il faut charger cette image ?
            //-------------------------------------------------------------
            Rectangle pixelRect = SicPixelRect(acqImage.Line, acqImage.Column);
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

        protected Rectangle SicPixelRect(int line, int column)
        {
            MosaicLayer layer = (MosaicLayer)Layer;

            int x = column * layer.MosaicImageSize.Width - coef[column] * paramOffset_X + offsetAllImagesX;
            int y = line * layer.MosaicImageSize.Height - coef[line] * paramOffset_Y + offsetAllImagesY;


            var rect = new Rectangle(x, y, layer.MosaicImageSize.Width, layer.MosaicImageSize.Height);
            return rect;
        }

      
    }
}
