using System;
using System.Linq;

using Agileo.EquipmentModeling;
using Agileo.GUI.Services.LightTower;
using UnitsNet;

namespace UnitySC.Equipment.Abstractions.Devices.LightTower
{
    public partial class LightTower
    {
        protected virtual void InternalSimulateDefineGreenLightMode(LightState state, Tempomat tempomat)
        {
            tempomat.Sleep(Duration.FromSeconds(2));
            GreenLight = state;
        }

        protected virtual void InternalSimulateDefineOrangeLightMode(LightState state, Tempomat tempomat)
        {
            tempomat.Sleep(Duration.FromSeconds(2));
            OrangeLight = state;
        }

        protected virtual void InternalSimulateDefineBlueLightMode(LightState state, Tempomat tempomat)
        {
            tempomat.Sleep(Duration.FromSeconds(2));
            BlueLight = state;
        }

        protected virtual void InternalSimulateDefineRedLightMode(LightState state, Tempomat tempomat)
        {
            tempomat.Sleep(Duration.FromSeconds(2));
            RedLight = state;
        }

        protected virtual void InternalSimulateDefineBuzzerMode(Agileo.SemiDefinitions.BuzzerState state, Tempomat tempomat)
        {
            tempomat.Sleep(Duration.FromSeconds(2));
            BuzzerState = state;
        }

        protected virtual void InternalSimulateSetDateAndTime(Tempomat tempomat)
        {
            tempomat.Sleep(Duration.FromSeconds(2));
        }

        protected virtual void InternalSimulateDefineState(
            Enums.LightTowerState state,
            Tempomat tempomat)
        {
            if (!Configuration.LightTowerStatuses.Any())
            {
                //If no configuration is available it means that we are using EFEM Ctrl and we do not pilot the light tower
                return;
            }

            // Get each light state in configuration
            var stateConfig =
                Configuration.LightTowerStatuses.Find(s => s.LightTowerState == state);

            if (stateConfig == null)
            {
                throw new ArgumentNullException(
                    $"No light configuration exists for SignalTower state '{state}'.");
            }

            // Update tower values
            GreenLight = UpdateLightState(stateConfig.Green, GreenLight);
            OrangeLight = UpdateLightState(stateConfig.Orange, OrangeLight);
            BlueLight = UpdateLightState(stateConfig.Blue, BlueLight);
            RedLight = UpdateLightState(stateConfig.Red, RedLight);
            BuzzerState = stateConfig.BuzzerState;
            SignalTowerState = state;
        }
    }
}
