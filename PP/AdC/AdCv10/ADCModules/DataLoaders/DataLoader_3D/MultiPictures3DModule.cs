using System.Collections.Generic;

using AcquisitionAdcExchange;

using ADCEngine;

using BasicModules.DataLoader;

using UnitySC.Shared.Data.Enum;

namespace DataLoaderModule_3D
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class MultiPictures3DModule : MosaicDataLoaderBase
    {
        //=================================================================
        // Constructeur
        //=================================================================
        public MultiPictures3DModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
        }

        public override ActorType DataLoaderActorType => ActorType.BrightField3D;

        public override IEnumerable<ResultType> CompatibleResultTypes => new List<ResultType>() { }; //eChannelID.BrightField3D_HighRes // not yet implented
        //=================================================================
        //
        //=================================================================
        public override bool FilterImage(ResultType restyp, int NoColumn = -1)
        {
            if (restyp.GetActorType() != ActorType.BrightField3D)
                return false;

            // Brightfield 3d is not implementted yet in USP. @true if channelID == (int)eChannelID.BrightField3D_HighRes)
            return false;

        }
    }
}
