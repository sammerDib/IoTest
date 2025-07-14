using UnitySC.Shared.Data.FDC;

namespace UnitySC.Shared.FDC
{
    public interface IFDCProvider
    {
        FDCData GetFDC(string fdcName);

        void ResetFDC(string fdcName);

        void SetPersistentData(string fdcName, IPersistentFDCData persistentFDCData);

        void StartFDCMonitor();

        void Register();

        void SetInitialCountdownValue(string fdcName, double initvalue);

    }
}
