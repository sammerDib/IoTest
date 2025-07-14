using System;
using System.Collections.Generic;

using AcquisitionAdcExchange;

using ADCEngine;

using BasicModules.DataLoader;

using UnitySC.Shared.Data.Enum;

namespace DataLoaderModule_LightSpeed
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class LightSpeedDefectivityModule : FullImageDataLoaderBase
    {
        public enum eImageType { Likelihood, Visibility, Saturation };
        public enum eDirection { Forward, Backward }; // aka wide , narrow  to do change direction label
        public override string DisplayName { get { return Factory.ModuleName + "-" + Id + "\n" + LayerName; } }
        public override string DisplayNameInParamView { get { return Factory.Label + "\n" + paramImageType.Value.ToString(); } }
        public override string LayerName { get { return paramImageType.Value.ToString() + "-" + paramDirection.Value.ToString(); } }

        public override ActorType DataLoaderActorType => ActorType.HeLioS;
        public override IEnumerable<ResultType> CompatibleResultTypes => GetExpectedResultTypes();


        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly EnumParameter<eDirection> paramDirection;
        public readonly EnumParameter<eImageType> paramImageType;

        //=================================================================
        // Constructeur
        //=================================================================
        public LightSpeedDefectivityModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramDirection = new EnumParameter<eDirection>(this, "Direction");
            paramImageType = new EnumParameter<eImageType>(this, "SourceImage");
        }

        //=================================================================
        //
        //=================================================================
        public override bool FilterImage(ResultType resultType, int NoCumn = -1)
        {
            if (resultType.GetActorType() != ActorType.HeLioS)
                return false;

            return GetExpectedResultTypes().Contains(resultType);
        }

        private List<ResultType> GetExpectedResultTypes()
        {
            List<ResultType> expectedResTypes = new List<ResultType>();
            if (paramDirection.Value == eDirection.Forward) //aka WIDE
            {
                switch (paramImageType.Value)
                {
                    case eImageType.Likelihood:
                        expectedResTypes.Add(ResultType.HLS_Likelyhood_WideFW); // (int)eChannelID.Vraissemblance_Forward;
                        break;
                    case eImageType.Visibility:
                        expectedResTypes.Add(ResultType.HLS_Visibilility_WideFW); // (int)eChannelID.Visibility_Forward;
                        break;
                    case eImageType.Saturation:
                        expectedResTypes.Add(ResultType.HLS_Saturation_WideFW); // (int)eChannelID.Saturation_Forward;
                        break;
                    default:
                        throw new ApplicationException("unkown image type: " + paramImageType.Value);
                }
            }
            else if (paramDirection.Value == eDirection.Backward)
            {
                switch (paramImageType.Value)
                {
                    case eImageType.Likelihood:
                        expectedResTypes.Add(ResultType.HLS_Likelyhood_NarrowBW); // (int)eChannelID.Vraissemblance_Backward;
                        break;
                    case eImageType.Visibility:
                        expectedResTypes.Add(ResultType.HLS_Visibilility_NarrowBW); // (int)eChannelID.Visibility_Backward;
                        break;
                    case eImageType.Saturation:
                        expectedResTypes.Add(ResultType.HLS_Saturation_NarrowBW); // (int)eChannelID.Saturation_Backward;
                        break;
                    default:
                        throw new ApplicationException("unkown image type: " + paramImageType.Value);
                }
            }
            else
            {
                throw new ApplicationException("unkown image type: " + paramDirection.Value);
            }

            return expectedResTypes;
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();
            Layer.name = LayerName;
        }

    }
}
