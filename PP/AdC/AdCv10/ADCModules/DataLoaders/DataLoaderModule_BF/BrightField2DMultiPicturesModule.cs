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
    public class BrightField2DMultiPicturesModule : MosaicDataLoaderBase
    {
        //=================================================================
        // Constructeur
        //=================================================================
        public BrightField2DMultiPicturesModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
        }

        public override ActorType DataLoaderActorType => ActorType.BrightField2D;
        public override IEnumerable<ResultType> CompatibleResultTypes => new List<ResultType>() { 
            ResultType.NotDefined 
        };

        //=================================================================
        //
        //=================================================================
        public override bool FilterImage(ResultType restyp, int NoColumn = -1)
        {
            if (restyp.GetActorType() != ActorType.BrightField2D)
                return false;

            // Brightfield is not implementted yet in USP. @true if channelID == (int)eChannelID.BrightField2D)
            return false;

        }

    }
}
