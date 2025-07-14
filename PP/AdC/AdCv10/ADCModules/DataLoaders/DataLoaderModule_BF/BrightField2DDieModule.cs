using System.Collections.Generic;

using AcquisitionAdcExchange;

using ADCEngine;

using BasicModules.DataLoader;

using UnitySC.Shared.Data.Enum;

namespace DataLoaderModule_BF
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class BrightField2DDieModule : DieDataLoaderBase
    {
        //=================================================================
        // Constructeur
        //=================================================================
        public BrightField2DDieModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {

        }

        public override ActorType DataLoaderActorType => ActorType.BrightFieldPattern;
        public override IEnumerable<ResultType> CompatibleResultTypes => GetExpectedResultTypes();

        //=================================================================
        //
        //=================================================================


        public override bool FilterImage(ResultType resultType, int NoCumn = -1)
        {
            if (resultType.GetActorType() != ActorType.BrightFieldPattern)
                return false;
            // Brightfield is not implementted yet in USP. @true if channelID == (int)eChannelID.BrightFieldPattern)
            return GetExpectedResultTypes().Contains(resultType);
        }

        private List<ResultType> GetExpectedResultTypes()
        {
            List<ResultType> expectedrestyp =  new List<ResultType>();
           /* switch (paramImageType.Value)
            {
                case eImageType.Brightfield:
                    expectedrestyp = new List<ResultType>()
                    {
                        ResultType.NotDefined
                    };
                    break;
                default:
                    throw new ApplicationException("unkown image type: " + paramImageType);
            }*/

            return expectedrestyp;
        }
    }
}
