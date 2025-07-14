using System;
using System.Collections.Generic;

using AcquisitionAdcExchange;

using ADCEngine;

using BasicModules.DataLoader;

using UnitySC.Shared.Data.Enum;

namespace DataLoaderModule_DMT
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class DMTDeflectometryModule : FullImageDataLoaderBase
    {
        [System.Reflection.Obfuscation(Exclude = true)]
        public enum eImageType { CurvatureX, CurvatureY,  DarkPicture, /*Amplitude*/ };
        [System.Reflection.Obfuscation(Exclude = true)]
        public enum eSide { Front, Back };
        public override string DisplayName { get { return $"{Factory.Label}-{Id}\n{paramImageType.Value.ToString()} {paramSide.Value.ToString()}"; } }
        public override string DisplayNameInParamView { get { return $"{Factory.Label}\n{paramImageType.Value.ToString()} {paramSide.Value.ToString()}"; } }
        public override string LayerName { get { return paramImageType.Value.ToString(); } }

        public override ActorType DataLoaderActorType => ActorType.DEMETER;
        public override IEnumerable<ResultType> CompatibleResultTypes => GetExpectedResultTypes();

        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly EnumParameter<eImageType> paramImageType;
        public readonly EnumParameter<eSide> paramSide;


        //=================================================================
        // Constructeur
        //=================================================================
        public DMTDeflectometryModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramImageType = new EnumParameter<eImageType>(this, "SourceImage");
            paramSide = new EnumParameter<eSide>(this, "Side");
        }

        //=================================================================
        //
        //=================================================================
        public override bool FilterImage(ResultType resultType, int NoCumn = -1)
        {
            if (resultType.GetActorType() != ActorType.DEMETER)
                return false;

            return GetExpectedResultTypes().Contains(resultType);      
        }

        private List<ResultType> GetExpectedResultTypes()
        {
            List<ResultType> expectedrestyp;
            switch (paramImageType.Value)
            {
                case eImageType.CurvatureX:
                    expectedrestyp = new List<ResultType>()
                    {
                        (paramSide.Value == eSide.Front) ?  ResultType.DMT_CurvatureX_Front : ResultType.DMT_CurvatureX_Back
                    };
                    break;
                case eImageType.CurvatureY:
                    expectedrestyp = new List<ResultType>()
                    {
                        (paramSide.Value == eSide.Front) ?  ResultType.DMT_CurvatureY_Front : ResultType.DMT_CurvatureY_Back
                    };
                    break;
                case eImageType.DarkPicture:
                    expectedrestyp = new List<ResultType>()
                    {
                        (paramSide.Value == eSide.Front) ?  ResultType.DMT_Dark_Front :  ResultType.DMT_Dark_Back
                    };
                    break;
               /* case eImageType.Amplitude: // disabled at teh moment
                    expectedrestyp = new List<ResultType>()
                    {
                        (paramSide.Value == eSide.Front) ?  ResultType.DMT_AmplitudeX_Front : ResultType.DMT_AmplitudeX_Back
                    };
                    break;*/
                default:
                    throw new ApplicationException("unkown image type: " + paramImageType);
            }

            return expectedrestyp;
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();
            Layer.name = paramImageType.Value.ToString();
        }

    }
}
