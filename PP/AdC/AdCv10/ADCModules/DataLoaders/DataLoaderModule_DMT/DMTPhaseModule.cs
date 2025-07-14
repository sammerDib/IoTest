using System;
using System.Collections.Generic;

using AcquisitionAdcExchange;

using ADCEngine;

using BasicModules.DataLoader;

using UnitySC.Shared.Data.Enum;

namespace DataLoaderModule_DMT
{
    // This module is currently unused in ADC
    // Uncomment this when those acquisition map wil be available by Demeter
    // WAITING FOR 2025 specifications (cf Isabelle)
    /*
        ///////////////////////////////////////////////////////////////////////
        // Module
        ///////////////////////////////////////////////////////////////////////
        public class DMTPhaseModule : FullImageDataLoaderBase
        {
            [System.Reflection.Obfuscation(Exclude = true)]
            public enum eImageType { PhaseX, PhaseY };
            public enum eSide { Front, Back };


            public override string DisplayName { get { return $"{Factory.Label}-{Id}\n{LayerLabelName}"; } }
            public override string DisplayNameInParamView { get { return $"{Factory.Label}\n{LayerLabelName}"; } }
            public override string LayerName { get { return $"{paramImageType.Value.ToString()}_{paramSide.Value.ToString()}"; } }
            private string LayerLabelName { 
                get 
                {
                    int imagelabellen = eImageType.PhaseX.ToString().Length - 1;
                    string lbl = paramImageType.Value.ToString().Substring(imagelabellen);
                    return $"{lbl} {paramSide.Value.ToString()}"; 
                }
            }


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
            public DMTPhaseModule(IModuleFactory factory, int id, Recipe recipe)
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
                if (paramSide.Value == eSide.Front)
                {
                    switch (paramImageType.Value)
                    {
                        case eImageType.PhaseX:
                            expectedrestyp = new List<ResultType>()
                        {
                            ResultType.DMT_TopoPhaseX_Front
                        };
                            break;
                        case eImageType.PhaseY:
                            expectedrestyp = new List<ResultType>()
                        {
                            ResultType.DMT_TopoPhaseY_Front
                        };
                            break;
                        default:
                            throw new ApplicationException("unkown image type FS : " + paramImageType);
                    }
                }
                else
                {
                    switch (paramImageType.Value)
                    {
                        case eImageType.PhaseX:
                            expectedrestyp = new List<ResultType>()
                            {
                                ResultType.DMT_TopoPhaseX_Back
                            };
                            break;
                        case eImageType.PhaseY:
                            expectedrestyp = new List<ResultType>()
                            {
                                ResultType.DMT_TopoPhaseY_Back
                            };
                            break;
                        default:
                            throw new ApplicationException("unkown image type BS : " + paramImageType);
                    }
                }

                return expectedrestyp;
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
    */
}
