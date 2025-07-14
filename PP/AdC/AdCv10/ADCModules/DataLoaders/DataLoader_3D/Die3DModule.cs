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
    public class Die3DModule : DieDataLoaderBase
    {
        //=================================================================
        // Constructeur
        //=================================================================
        public Die3DModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
        }

        public override ActorType DataLoaderActorType => ActorType.BrightField3D;
        public override IEnumerable<ResultType> CompatibleResultTypes => new List<ResultType>();// { (int)eChannelID.BrightField3D_DieToDie // not implemented};

        //=================================================================
        //
        //=================================================================
        public override bool FilterImage(ResultType restyp,  int NoColumn = -1)
        {
            if (restyp.GetActorType() != ActorType.BrightField3D)
                return false;

            return false; // not implemented  // true  if (channelID == (int)eChannelID.BrightField3D_DieToDie)
        }

    }
}
