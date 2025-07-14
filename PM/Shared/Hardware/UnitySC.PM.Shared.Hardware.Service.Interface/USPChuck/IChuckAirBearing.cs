using System.Collections.Generic;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Chuck
{
    public interface IChuckAirBearing
    {
        void InitAirbearing();

        Dictionary<string, float> GetAirBearingPressuresValues();
    }
}
