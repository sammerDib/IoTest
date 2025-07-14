using System.Collections.Generic;

using UnitySC.Shared.Image;
using UnitySC.Shared.Data.Enum;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Interface.Fringe
{
    public interface IFringeManager
    {
        void Init();

        void Shutdown();

        int GetNbImages(Measure.Fringe fringe);

        Dictionary<FringesDisplacement, Dictionary<int, List<USPImageMil>>> GetFringeImageDict(Side side,
            Measure.Fringe fringe);
    }
}
