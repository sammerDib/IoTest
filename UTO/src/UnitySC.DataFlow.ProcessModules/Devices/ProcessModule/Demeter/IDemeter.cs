using Agileo.EquipmentModeling;

using UnitsNet;

using UnitySC.DataFlow.ProcessModules.Devices.ProcessModule.UnityProcessModule;

namespace UnitySC.DataFlow.ProcessModules.Devices.ProcessModule.Demeter
{
    // Implement interface IMaterialLocationContainer if the device has material locations
    [Device]
    public interface IDemeter : IUnityProcessModule
    {
        #region Status

        [Status]
        string AcquisitionWaferId { get; }

        [Status]
        Ratio AcquisitionProgress { get; }

        [Status]
        string CalculationWaferId { get; }

        [Status]
        Ratio CalculationProgress { get; }

        #endregion
    }
}
