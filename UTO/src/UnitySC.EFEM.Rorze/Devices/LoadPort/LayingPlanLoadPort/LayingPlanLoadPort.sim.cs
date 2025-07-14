using System;
using System.Collections.ObjectModel;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Driver.Enums;
using UnitySC.Equipment.Abstractions.Devices.LoadPort.Simulation;

using SlotState = UnitySC.Equipment.Abstractions.Material.SlotState;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.LayingPlanLoadPort
{
    public partial class LayingPlanLoadPort
    {
        protected override void InternalSimulateMap(Tempomat tempomat)
        {
            InternalMap();
        }

        protected override void UpdateLoadPortStatus(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(LoadPortSimulationViewModel.LoadPortControlViewModel
                    .IsCarrierPlacementOk):
                case nameof(LoadPortSimulationViewModel.LoadPortControlViewModel.IsCarrierPresent):
                    switch (SimulationData.CarrierConfiguration.Type)
                    {
                        case SampleDimension.S200mm:
                            PlacementSensorC = LoadPortViewModel.LoadPortControlViewModel
                                .IsCarrierPresent;
                            PlacementSensorD = LoadPortViewModel.LoadPortControlViewModel
                                .IsCarrierPlacementOk;
                            break;

                        case SampleDimension.S150mm:
                            PlacementSensorA = LoadPortViewModel.LoadPortControlViewModel
                                .IsCarrierPlacementOk;
                            PlacementSensorC = LoadPortViewModel.LoadPortControlViewModel
                                .IsCarrierPresent;
                            break;

                        case SampleDimension.S100mm:
                            PlacementSensorB = LoadPortViewModel.LoadPortControlViewModel
                                .IsCarrierPlacementOk;
                            PlacementSensorC = LoadPortViewModel.LoadPortControlViewModel
                                .IsCarrierPresent;
                            break;

                        default:
                            PlacementSensorA = false;
                            PlacementSensorB = false;
                            PlacementSensorC = false;
                            PlacementSensorD = false;
                            break;
                    }

                    break;

                default:
                    base.UpdateLoadPortStatus(propertyName);
                    break;
            }

            UpdateStatuses();
        }

        public Collection<RR75xSlotState> GetSimulatedMapping()
        {
            if (ExecutionMode == ExecutionMode.Real)
            {
                throw new InvalidOperationException(
                    "Trying to get simulated mapping while load port is in real mode.");
            }

            var slotsState = new Collection<RR75xSlotState>();
            foreach (var slotState in SimulationData.Mapping)
            {
                switch (slotState)
                {
                    case SlotState.NoWafer:
                        slotsState.Add(RR75xSlotState.WaferDoesNotExist);
                        break;
                    case SlotState.HasWafer:
                        slotsState.Add(RR75xSlotState.WaferExists);
                        break;
                    case SlotState.DoubleWafer:
                        slotsState.Add(RR75xSlotState.SeveralWafersInSameSlot);
                        break;
                    case SlotState.CrossWafer:
                        slotsState.Add(RR75xSlotState.CrossedWafer);
                        break;
                    case SlotState.FrontBow:
                        slotsState.Add(RR75xSlotState.FrontBow);
                        break;
                    case SlotState.Thick:
                        slotsState.Add(RR75xSlotState.ThicknessAbnormal_ThickWafer);
                        break;
                    case SlotState.Thin:
                        slotsState.Add(RR75xSlotState.ThicknessAbnormal_ThinWafer);
                        break;
                }
            }

            return slotsState;
        }
    }
}
