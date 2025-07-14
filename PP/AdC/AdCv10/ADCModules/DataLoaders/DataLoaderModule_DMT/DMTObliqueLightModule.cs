using System;
using System.Collections.Generic;

using ADCEngine;

using BasicModules.DataLoader;

using UnitySC.Shared.Data.Enum;

namespace DataLoaderModule_DMT
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class DMTObliqueLightModule : FullImageDataLoaderBase
    {
        [System.Reflection.Obfuscation(Exclude = true)]
        public enum eSide { Front, Back };
        public override string DisplayName { get { return $"{Factory.Label}-{Id}\n{paramSide.Value.ToString()}"; } }
        public override string DisplayNameInParamView { get { return $"{Factory.Label}\n{paramSide.Value.ToString()}"; } }
        public override string LayerName { get { return $"OL_{paramSide.Value.ToString()}"; } }

        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly EnumParameter<eSide> paramSide;

        public override ActorType DataLoaderActorType => ActorType.DEMETER;
        public override IEnumerable<ResultType> CompatibleResultTypes => GetExpectedResultTypes();

        //=================================================================
        // Constructeur
        //=================================================================
        public DMTObliqueLightModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
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
            switch (paramSide.Value)
            {
                case eSide.Front:
                    expectedrestyp = new List<ResultType>()
                            {
                                ResultType.DMT_ObliqueLight_Front
                            };
                    break;
                case eSide.Back:
                expectedrestyp = new List<ResultType>()
                    {
                        ResultType.DMT_ObliqueLight_Back
                    };
                break;
                default:
                    throw new ApplicationException("unkown Side for ObliqueLight : " + paramSide?.Value.ToString());
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
}
