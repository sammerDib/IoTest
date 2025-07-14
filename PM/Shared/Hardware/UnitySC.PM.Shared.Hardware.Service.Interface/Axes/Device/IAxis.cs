using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Axes
{
    public interface IAxis
    {
        #region Properties

        AxisConfig AxisConfiguration { get; }
        ILogger Logger { get; set; }
        string Name { get; }
        string AxisID { get; }
        bool Initialized { get; set; }
        Length CurrentPos { get; set; }
        bool Enabled { get; set; }
        bool EnabledPrev { get; set; }
        bool Moving { get; set; }
        bool MovingPrev { get; set; }
        Message DeviceError { get; set; }
        bool IsLandingUsed { get; }

        #endregion Properties

        #region Private methods

        bool ArePredifinedPositionsConfiguredValid();

        bool IsParkPosConfiguredValid();

        bool IsManualLoadPosConfiguredValid();

        bool IsHomePosConfiguredValid();

        #endregion Private methods
    }
}
