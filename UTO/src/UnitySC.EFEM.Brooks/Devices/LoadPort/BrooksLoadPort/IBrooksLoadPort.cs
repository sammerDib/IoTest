using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Devices.LoadPort;

namespace UnitySC.EFEM.Brooks.Devices.LoadPort.BrooksLoadPort
{
    // Implement interface IMaterialLocationContainer if the device has material locations
    [Device]
    public interface IBrooksLoadPort : ILoadPort
    {
        #region Statuses

        [Status]
        LightState PlacementLightState { get; }

        [Status]
        LightState PodLightState { get; }

        [Status]
        LightState PresenceLightState { get; }

        [Status]
        LightState ReadyLightState { get; }

        [Status]
        LightState ServiceLightState { get; }

        [Status]
        LightState Led1LightState { get; }

        [Status]
        LightState Led2LightState { get; }

        [Status]
        LightState Led3LightState { get; }

        [Status]
        LightState Led4LightState { get; }

        [Status]
        LightState Led5LightState { get; }

        [Status]
        LightState Led6LightState { get; }

        [Status]
        LightState Led7LightState { get; }

        [Status]
        LightState Led8LightState { get; }

        [Status]
        LightState Led9LightState { get; }

        [Status]
        LightState Led10LightState { get; }

        [Status]
        bool InfoPadA { get; }

        [Status]
        bool InfoPadB { get; }

        [Status]
        bool InfoPadC { get; }

        [Status]
        bool InfoPadD { get; }

        [Status]
        bool InfoPadE { get; }

        [Status]
        bool InfoPadF { get; }

        #endregion
    }
}
