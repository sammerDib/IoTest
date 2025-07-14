using Agileo.EquipmentModeling;

using UnitySC.Equipment.Abstractions.Vendor.Devices.GenericController;

namespace UnitySC.Equipment.Abstractions.Devices.Controller
{
    // Implement interface IMaterialLocationContainer if the device has material locations
    [Device(IsAbstract = true)]
    [Interlock(Type = typeof(IsRobotInitialized))]
    [Interlock(Type = typeof(IsLocationReadyForTransfer))]
    [Interlock(Type = typeof(IsLocationServedByRobot))]
    [Interlock(Type = typeof(IsSlotValid))]
    [Interlock(Type = typeof(IsLocationAccessedByRobot))]
    public interface IController : IGenericController, IMaterialLocationPicker
    {
        
    }
}
