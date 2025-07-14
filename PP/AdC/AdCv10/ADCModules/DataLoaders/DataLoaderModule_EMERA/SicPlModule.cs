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
using UnitySC.Shared.Tools;

namespace DataLoaderModule_SICPL
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class SicPlModule : MosaicDataLoaderBase
    {
        public enum LightType { Visible, Visible90, UV, UV2, UV3 }
        
        public enum FilterType { NoFilter, LinearPolarizing, BandPass450nm50, BandPass550nm50, LowPass650nm, LowPass750nm }
        
        public override string DisplayName { get { return $"{Factory.ModuleName}-{Id}\n{ParamLightType.Value.ToString()}-{ParamFilterType.Value.ToString()}"; } }
        
        public override string DisplayNameInParamView { get { return $"{Factory.Label}\n{ParamLightType.Value.ToString()} {ParamFilterType.Value.ToString()}"; } }
        
        public override string LayerName { get { return $"PL_{ParamLightType.Value.ToString()}_{ParamFilterType.Value.ToString()}"; } }

        public override ActorType DataLoaderActorType => ActorType.EMERA;
        
        public override IEnumerable<ResultType> CompatibleResultTypes => GetCompatibleResultType();

        //public enum SicPlChannelId { PlChannelID_0, PlChannelID_1, PlChannelID_2, PlChannelID_3 };
        private readonly int[] _coef = {1,3,5,7,9,11,13,15,17,19,21,23,25 };
        private int _offsetAllImagesX = 0;
        private int _offsetAllImagesY = 0;

        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly EnumParameter<LightType> ParamLightType;
        public readonly EnumParameter<FilterType> ParamFilterType;
        public readonly BoolParameter ParamUseLowRes;
        
        public IntParameter ParamOffset_X;
        public IntParameter ParamOffset_Y;


        //=================================================================
        // Constructeur
        //=================================================================
        public SicPlModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            ParamLightType = new EnumParameter<LightType>(this, "Light type");
            ParamFilterType = new EnumParameter<FilterType>(this, "Filter type");
            ParamUseLowRes = new BoolParameter(this, "Use low resolution images");
            ParamOffset_X = new IntParameter(this, "Offset X pixel",int.MinValue);
            ParamOffset_Y = new IntParameter(this, "Offset Y pixel",int.MinValue);
            ParamLightType.ValueChanged += ParamLightTypeOnValueChanged;

            ParamOffset_X.Value = 1000;  // Pixel
            ParamOffset_Y.Value = 1000;  // Pixel
        }

        private void ParamLightTypeOnValueChanged(LightType lightType)
        {
            if (lightType == LightType.Visible)
            {
                ParamFilterType.Value = FilterType.NoFilter;
                ParamFilterType.IsEnabled = false;
            }
            else if (!ParamFilterType.IsEnabled)
            {
                ParamFilterType.IsEnabled = true;
            }
        }

        //=================================================================
        //
        //=================================================================
        public override bool FilterImage(ResultType resultType, int NoCumn = -1)
        {
            
            if (resultType.GetActorType() != ActorType.EMERA)
                    return false;
            return GetCompatibleResultType().Contains(resultType);
		}

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();
            _offsetAllImagesX = (((((MosaicInputInfo)InputInfo).NbColumns / 2) - 1) * 2 + 2) * ParamOffset_X;
            _offsetAllImagesY = (((((MosaicInputInfo)InputInfo).NbLines / 2) - 1) * 2 + 2) * ParamOffset_Y;

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

            int x = column * layer.MosaicImageSize.Width - _coef[column] * ParamOffset_X + _offsetAllImagesX;
            int y = line * layer.MosaicImageSize.Height - _coef[line] * ParamOffset_Y + _offsetAllImagesY;


            var rect = new Rectangle(x, y, layer.MosaicImageSize.Width, layer.MosaicImageSize.Height);
            return rect;
        }

        private static ResultType GetNoFilterResultType(LightType lightType, bool useLowRes)
        {
            switch (lightType)
            {
                case LightType.Visible:
                    return useLowRes ? ResultType.EME_Visible0_LowRes : ResultType.EME_Visible0;
                case LightType.Visible90:
                    return useLowRes ? ResultType.EME_Visible90_LowRes : ResultType.EME_Visible90;
                case LightType.UV:
                    return useLowRes ? ResultType.EME_UV_NoFilter_LowRes : ResultType.EME_UV_NoFilter;
                case LightType.UV2:
                    return useLowRes ? ResultType.EME_UV2_NoFilter_LowRes : ResultType.EME_UV2_NoFilter;
                case LightType.UV3:
                    return useLowRes ? ResultType.EME_UV3_NoFilter_LowRes : ResultType.EME_UV3_NoFilter;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lightType));
            }
        }

        private static ResultType GetLinearPolarizingFilterResultType(LightType lightType, bool useLowRes)
        {
            switch (lightType)
            {
                case LightType.Visible:
                case LightType.Visible90:
                    throw new ArgumentOutOfRangeException();
                case LightType.UV:
                    return useLowRes ? ResultType.EME_UV_LinearPolarizingFilter_LowRes : ResultType.EME_UV_LinearPolarizingFilter;
                case LightType.UV2:
                    return useLowRes ? ResultType.EME_UV2_LinearPolarizingFilter_LowRes : ResultType.EME_UV2_LinearPolarizingFilter;
                case LightType.UV3:
                    return useLowRes ? ResultType.EME_UV3_LinearPolarizingFilter_LowRes : ResultType.EME_UV3_LinearPolarizingFilter;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lightType));
            }
        }
        
        private static ResultType GetBandPass450Nm50FilterResultType(LightType lightType, bool useLowRes)
        {
            switch (lightType)
            {
                case LightType.Visible:
                case LightType.Visible90:
                    throw new ArgumentOutOfRangeException();
                case LightType.UV:
                    return useLowRes ? ResultType.EME_UV_BandPass450nm50_LowRes : ResultType.EME_UV_BandPass450nm50;
                case LightType.UV2:
                    return useLowRes ? ResultType.EME_UV2_BandPass450nm50_LowRes : ResultType.EME_UV2_BandPass450nm50;
                case LightType.UV3:
                    return useLowRes ? ResultType.EME_UV3_BandPass450nm50_LowRes : ResultType.EME_UV3_BandPass450nm50;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lightType));
            }
        }
        
        private static ResultType GetBandPass550Nm50FilterResultType(LightType lightType, bool useLowRes)
        {
            switch (lightType)
            {
                case LightType.Visible:
                case LightType.Visible90:
                    throw new ArgumentOutOfRangeException();
                case LightType.UV:
                    return useLowRes ? ResultType.EME_UV_BandPass550nm50_LowRes : ResultType.EME_UV_BandPass550nm50;
                case LightType.UV2:
                    return useLowRes ? ResultType.EME_UV2_BandPass550nm50_LowRes : ResultType.EME_UV2_BandPass550nm50;
                case LightType.UV3:
                    return useLowRes ? ResultType.EME_UV3_BandPass550nm50_LowRes : ResultType.EME_UV3_BandPass550nm50;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lightType));
            }
        }

        private static ResultType GetLowPass650NmFilterResultType(LightType lightType, bool useLowRes)
        {
            switch (lightType)
            {
                case LightType.Visible:
                case LightType.Visible90:
                    throw new ArgumentOutOfRangeException();
                case LightType.UV:
                    return useLowRes ? ResultType.EME_UV_LowPass650nm_LowRes : ResultType.EME_UV_LowPass650nm;
                case LightType.UV2:
                    return useLowRes ? ResultType.EME_UV2_LowPass650nm_LowRes : ResultType.EME_UV2_LowPass650nm;
                case LightType.UV3:
                    return useLowRes ? ResultType.EME_UV3_LowPass650nm_LowRes : ResultType.EME_UV3_LowPass650nm;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lightType));
            }
        }
        
        private static ResultType GetLowPass750NmFilterResultType(LightType lightType, bool useLowRes)
        {
            switch (lightType)
            {
                case LightType.Visible:
                case LightType.Visible90:
                    throw new ArgumentOutOfRangeException();
                case LightType.UV:
                    return useLowRes ? ResultType.EME_UV_LowPass750nm_LowRes : ResultType.EME_UV_LowPass750nm;
                case LightType.UV2:
                    return useLowRes ? ResultType.EME_UV2_LowPass750nm_LowRes : ResultType.EME_UV2_LowPass750nm;
                case LightType.UV3:
                    return useLowRes ? ResultType.EME_UV3_LowPass750nm_LowRes : ResultType.EME_UV3_LowPass750nm;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lightType));
            }
        }
        
        private IEnumerable<ResultType> GetCompatibleResultType()
        {
            switch (ParamFilterType.Value)
            {
                case FilterType.NoFilter:
                    yield return GetNoFilterResultType(ParamLightType, ParamUseLowRes);
                    break;
                case FilterType.LinearPolarizing:
                    yield return GetLinearPolarizingFilterResultType(ParamLightType, ParamUseLowRes);
                    break;
                case FilterType.BandPass450nm50:
                    yield return GetBandPass450Nm50FilterResultType(ParamLightType, ParamUseLowRes);
                    break;
                case FilterType.BandPass550nm50:
                    yield return GetBandPass550Nm50FilterResultType(ParamLightType, ParamUseLowRes);
                    break;
                case FilterType.LowPass650nm:
                    yield return GetLowPass650NmFilterResultType(ParamLightType, ParamUseLowRes);
                    break;
                case FilterType.LowPass750nm:
                    yield return GetLowPass750NmFilterResultType(ParamLightType, ParamUseLowRes);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
      
    }
}
