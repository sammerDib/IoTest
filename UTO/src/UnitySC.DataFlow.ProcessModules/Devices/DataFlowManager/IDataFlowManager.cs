using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager;

namespace UnitySC.DataFlow.ProcessModules.Devices.DataFlowManager
{
    // Implement interface IMaterialLocationContainer if the device has material locations
    [Device]
    public interface IDataFlowManager : IAbstractDataFlowManager
    {
    }
}
