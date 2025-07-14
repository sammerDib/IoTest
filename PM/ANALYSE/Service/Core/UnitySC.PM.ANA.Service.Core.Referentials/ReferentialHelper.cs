using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.ANA.Service.Core.Referentials
{
    public static class ReferentialHelper
    {
        public static ReferentialBase CreateDieOrWaferReferential(DieIndex dieIndex)
        {
            if (dieIndex is null)
                return new WaferReferential();

            return new DieReferential(dieIndex.Column, dieIndex.Row);
        }
    }
}
