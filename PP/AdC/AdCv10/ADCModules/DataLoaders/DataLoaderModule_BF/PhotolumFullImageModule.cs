
using System;
using System.Collections.Generic;



using ADCEngine;

using BasicModules.DataLoader;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.Enum.Module;

using static DataLoaderModule_BF.LightAcquisition;


namespace DataLoaderModule_BF
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class PhotolumFullImageModule : FullImageDataLoaderBase
    {
        //=================================================================
        // Constructeur
        //=================================================================
        public PhotolumFullImageModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            ParamLightAcquisitionType = new EnumParameter<LightAcquisitionType>(this, "LightAcquisitionType");

        }

        /// <summary>
        /// Parameter that indicates whether the Dataloader is expecting for a 
        /// Photolum or Darkfield90 Image acquisition
        /// </summary>
        public EnumParameter<LightAcquisition.LightAcquisitionType> ParamLightAcquisitionType;

        public override ActorType DataLoaderActorType => ActorType.BrightField2D;

        public override IEnumerable<ResultType> CompatibleResultTypes => GetCompatibleResultType();

        protected override void OnInit()
        {
            // Init de la queue de sortie
            //...........................

            base.OnInit();
            Layer.name = ParamLightAcquisitionType.Value.ToString();

        }



        //=================================================================
        //  FilterImage
        //  Returns true is the Image is accepted, false if it isn't valid
        //  for this module
        //=================================================================
        public override bool FilterImage(ResultType restyp, int noColumn = -1)
        {
            string logMsg = "PhotolumFullImageModule: ";

            if (restyp.GetActorType() != ActorType.EMERA)
                return false;

            var lightSrc = restyp.GetEMELightSource();

            switch (ParamLightAcquisitionType.Value)
            {
                case LightAcquisitionType.PL:
                    switch (lightSrc)
                    {
                        case EMELightSource.UV:
                            var emeHelper = restyp.GetEMEAcquisitionTypeId();
                            switch (emeHelper)
                            {
                                case 3:
                                case 4:
                                case 5: return true;
                                default:
                                    logDebug(logMsg + "Expecting an Image made by Photolum acquisition " +
                                        "found one made with an unrecognized Acquisition type Id");
                                    return false;
                            }

                        case EMELightSource.Visible:
                            logDebug(logMsg + "Expecting an Image made by Photolum acquisition ," +
                                "found one made with Darkfield acquisition type");
                            return false;
                        default:
                            logDebug(logMsg + "Unexpected acquisition method type on received Image." +
                                "This module only receives images made by Darkfield90 and Photolum acquisition types");
                            return false;
                    }
                case LightAcquisitionType.DDF90:
                    switch (lightSrc)
                    {
                        case EMELightSource.Visible:
                            return true;
                        case EMELightSource.UV:
                            logDebug(logMsg + "Expecting an Image made by Darkfield90 acquisition ," +
                                "found one made with Photolum acquisition type");
                            return false;
                        default:
                            logDebug(logMsg + "Unexpected acquisition method type on received Image." +
                                "This module only receives images made by Darkfield90 and Photolum acquisition types");
                            return false;
                    }

                // Why this default is needed
                default: return false;
            }

        }
        private IEnumerable<ResultType> GetCompatibleResultType()
        {
            switch (ParamLightAcquisitionType.Value)
            {
                case LightAcquisitionType.PL:
                    yield return ResultType.EME_UV_NoFilter;
                    yield return ResultType.EME_UV_NoFilter_LowRes;
                    yield return ResultType.EME_UV_LinearPolarizingFilter;
                    yield return ResultType.EME_UV_LinearPolarizingFilter_LowRes;
                    yield return ResultType.EME_UV_BandPass450nm50;
                    yield return ResultType.EME_UV_BandPass450nm50_LowRes;
                    yield return ResultType.EME_UV_BandPass470nm50;
                    yield return ResultType.EME_UV_BandPass470nm50_LowRes;
                    yield return ResultType.EME_UV_BandPass550nm50;
                    yield return ResultType.EME_UV_BandPass550nm50_LowRes;
                    yield return ResultType.EME_UV_LowPass650nm;
                    yield return ResultType.EME_UV_LowPass650nm_LowRes;
                    yield return ResultType.EME_UV_LowPass750nm;
                    yield return ResultType.EME_UV_LowPass750nm_LowRes;
                    yield return ResultType.EME_UV2_NoFilter;
                    yield return ResultType.EME_UV2_NoFilter_LowRes;
                    yield return ResultType.EME_UV2_LinearPolarizingFilter;
                    yield return ResultType.EME_UV2_LinearPolarizingFilter_LowRes;
                    yield return ResultType.EME_UV2_BandPass450nm50;
                    yield return ResultType.EME_UV2_BandPass450nm50_LowRes;
                    yield return ResultType.EME_UV2_BandPass470nm50;
                    yield return ResultType.EME_UV2_BandPass470nm50_LowRes;
                    yield return ResultType.EME_UV2_BandPass550nm50;
                    yield return ResultType.EME_UV2_BandPass550nm50_LowRes;
                    yield return ResultType.EME_UV2_LowPass650nm;
                    yield return ResultType.EME_UV2_LowPass650nm_LowRes;
                    yield return ResultType.EME_UV2_LowPass750nm;
                    yield return ResultType.EME_UV2_LowPass750nm_LowRes;
                    yield return ResultType.EME_UV3_NoFilter;
                    yield return ResultType.EME_UV3_NoFilter_LowRes;
                    yield return ResultType.EME_UV3_LinearPolarizingFilter;
                    yield return ResultType.EME_UV3_LinearPolarizingFilter_LowRes;
                    yield return ResultType.EME_UV3_BandPass450nm50;
                    yield return ResultType.EME_UV3_BandPass450nm50_LowRes;
                    yield return ResultType.EME_UV3_BandPass470nm50;
                    yield return ResultType.EME_UV3_BandPass470nm50_LowRes;
                    yield return ResultType.EME_UV3_BandPass550nm50;
                    yield return ResultType.EME_UV3_BandPass550nm50_LowRes;
                    yield return ResultType.EME_UV3_LowPass650nm;
                    yield return ResultType.EME_UV3_LowPass650nm_LowRes;
                    yield return ResultType.EME_UV3_LowPass750nm;
                    yield return ResultType.EME_UV3_LowPass750nm_LowRes;
                    break;
                case LightAcquisitionType.DDF90:
                    yield return ResultType.EME_Visible0;
                    yield return ResultType.EME_Visible0_LowRes;
                    yield return ResultType.EME_Visible90;
                    yield return ResultType.EME_Visible90_LowRes;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }


    public class LightAcquisition
    {
        public enum LightAcquisitionType
        {
            PL,
            DDF90
        }

        public LightAcquisitionType Value { get; set; } = LightAcquisitionType.PL; // By default

        public LightAcquisition(int lightType)
        {
            if (Enum.IsDefined(typeof(LightAcquisitionType), lightType))
            {
                Value = (LightAcquisitionType)lightType;
            }
            else
            {
                throw new Exception("Light Acquisition : Unknown Light Acquisition Type <" + lightType + ">");
            }
        }

        public LightAcquisition(string lightType)
        {
            if (Enum.IsDefined(typeof(LightAcquisitionType), lightType))
            {
                Value = (LightAcquisitionType)Enum.Parse(typeof(LightAcquisitionType), lightType);
            }
            else
            {
                throw new Exception("Light Acquisition : Unknown Light Acquisition Type <" + lightType + ">");
            }
        }
        public override string ToString()
        {
            return Value.ToString("G");
        }
    }
}
