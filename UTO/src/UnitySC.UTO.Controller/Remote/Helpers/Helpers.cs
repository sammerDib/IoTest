using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Agileo.Semi.Communication.Abstractions.E5;
using Agileo.Semi.Gem300.Abstractions.E40;
using Agileo.Semi.Gem300.Abstractions.E87;
using Agileo.Semi.Gem300.Abstractions.E90;

using UnitySC.Equipment.Abstractions.Material;
using UnitySC.GUI.Common.Equipment.LoadPort;
using UnitySC.Shared.TC.Shared.Data;

using Carrier = UnitySC.Equipment.Abstractions.Material.Carrier;
using IProcessJob = Agileo.Semi.Gem300.Abstractions.E40.IProcessJob;
using MaterialType = Agileo.Semi.Gem300.Abstractions.E40.MaterialType;
using Substrate = UnitySC.Equipment.Abstractions.Vendor.Material.Substrate;
using SubstrateLocation = Agileo.Semi.Gem300.Abstractions.E90.SubstrateLocation;

namespace UnitySC.UTO.Controller.Remote.Helpers
{
    public static class Helpers
    {
        public static IE90Standard E90Std => App.ControllerInstance.GemController.E90Std;
        public static IE87Standard E87Std => App.ControllerInstance.GemController.E87Std;

        public static bool IsCarrierUsedByCurrentPj(string carrierId)
        {
            var executingPj = App.ControllerInstance.GemController.E40Std.ProcessJobs.Where(
                pj => pj.JobState
                    is not JobState.QUEUED_POOLED
                    and not JobState.PROCESSCOMPLETE
                    and not JobState.ABORTED
                    and not JobState.STOPPED).ToList();

            if (executingPj.Count == 0)
            {
                return false;
            }

            return executingPj.Any(pj => pj.CarrierIDSlotsAssociation.Any(c => c.CarrierID.Equals(carrierId)));
        }

        public static bool IsCarrierUsedByQueuedPj(string carrierId)
        {
            //Assumption: only one CJ/PJ running at a time
            var pjs = App.ControllerInstance.GemController.E40Std.ProcessJobs.Where(pj => pj.JobState.Equals(JobState.QUEUED_POOLED)).ToList();

            foreach (var processJob in pjs)
            {
                switch (processJob.MaterialType)
                {
                    case MaterialType.Carriers:
                        return CheckCarrierUsedByJob(carrierId, processJob.CarrierIDSlotsAssociation);
                    case MaterialType.Substrates:
                        return CheckCarrierUsedByJob(carrierId, processJob.SubstrateNames);
                }
            }

            return false;
        }

        private static bool CheckCarrierUsedByJob(string carrierId, IEnumerable<MaterialNameListElement> carrierIdSlotAssociation)
        {
            //MF is Carriers
            if (carrierIdSlotAssociation != null)
            {
                return carrierIdSlotAssociation.Any(element => element.CarrierID == carrierId);
            }

            return false;
        }

        private static bool CheckCarrierUsedByJob(string carrierId, IEnumerable<string> substrateNames)
        {
            //MF is Substrates
            if (substrateNames != null)
            {
                foreach (var substrateName in substrateNames)
                {
                    if (E90Std.GetSubstrate(substrateName) is not { } substrate)
                    {
                        continue;
                    }

                    if (substrate.SubstSource.Equals(carrierId))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool IsAllWaferAtDestination(IProcessJob pj, Agileo.Semi.Gem300.Abstractions.E87.Carrier carrier)
        {
            if (pj.MaterialType is MaterialType.Carriers)
            {
                return CheckSubstStatesInJob(carrier.ObjID, pj.CarrierIDSlotsAssociation);
            }

            if (pj.MaterialType is MaterialType.Substrates)
            {
                return CheckSubstStatesInJob(pj.SubstrateNames);
            }
            
            return false;
        }

        private static bool CheckSubstStatesInJob(string carrierId, IEnumerable<MaterialNameListElement> carrierIdSlotAssociation)
        {
            //MF is Carriers
            foreach (var materialNameListElement in carrierIdSlotAssociation)
            {
                if (materialNameListElement.CarrierID.Equals(carrierId) && materialNameListElement.SlotIds != null)
                {
                    foreach (var slotId in materialNameListElement.SlotIds)
                    {
                        SubstrateLocation substLoc = E90Std.GetCarrierSubstrateLocation(carrierId, slotId);
                        if (substLoc.SubstLocState.Equals(LocationState.Unoccupied))
                        {
                            return false;
                        }

                        if (!E90Std.GetSubstrate(substLoc.SubstID).SubstState.Equals(SubstState.AtDestination))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private static bool CheckSubstStatesInJob(IEnumerable<string> substrateNames)
        {
            //MF is Substrates
            if (substrateNames != null)
            {
                return E90Std.Substrates
                    .Where(s => substrateNames.Contains(s.ObjID))
                    .Any(s => s.SubstState != SubstState.AtDestination);
            }

            return true;
        }

        public static VidDataType ConvertFormatToDataType(DataItemFormat format)
        {
            switch (format)
            {
                case DataItemFormat.ASC:
                    return VidDataType.String;

                case DataItemFormat.BIN:
                case DataItemFormat.BOO:
                    return VidDataType.Boolean;

                case DataItemFormat.SI1:
                    return VidDataType.I1;

                case DataItemFormat.SI2:
                    return VidDataType.I2;

                case DataItemFormat.SI4:
                    return VidDataType.I4;

                case DataItemFormat.SI8:
                    return VidDataType.I8;

                case DataItemFormat.UI1:
                    return VidDataType.U1;

                case DataItemFormat.UI2:
                    return VidDataType.U2;

                case DataItemFormat.UI4:
                    return VidDataType.U4;

                case DataItemFormat.UI8:
                    return VidDataType.U8;

                case DataItemFormat.FP4:
                    return VidDataType.F4;

                case DataItemFormat.FP8:
                    return VidDataType.F8;

                case DataItemFormat.LST:
                    return VidDataType.LIST;
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }
        }

        public static void BuildInputSpecByCarrier(
            Carrier carrier,
            List<IndexedSlotState> selectedSlots,
            Collection<string> carrierInputSpec,
            List<MaterialNameListElement> materialNameList)
        {
            if (carrier != null
                && selectedSlots.Any())
            {
                carrierInputSpec.Add(carrier.Id);
                materialNameList.Add(new MaterialNameListElement(
                    carrier.Id,
                    selectedSlots.Select(s => s.Substrate.SourceSlot)));
            }
        }

        public static void BuildSubstratesListByCarrier(Carrier carrier, List<IndexedSlotState> selectedSlots, List<Wafer> wafers)
        {
            if (carrier != null)
            {
                wafers.AddRange(selectedSlots.Select(s => s.Substrate as Wafer).ToList());
            }
        }

        public static List<Wafer> BuildWaferList(IProcessJob processJob, IEnumerable<Substrate> substrates)
        {
            var wafers = new List<Wafer>();

            foreach (var item in processJob.CarrierIDSlotsAssociation)
            {
                for (var slotIndex = 0; slotIndex < item.SlotIds.Count; slotIndex++)
                {
                    //Retrieve load port where the carrier is placed
                    var loadPort =
                        App.ControllerInstance.ControllerEquipmentManager.LoadPorts.Values
                            .FirstOrDefault(
                                lp => lp.Carrier != null
                                      && lp.Carrier.Id == item.CarrierID);

                    if (loadPort != null
                        && loadPort.Carrier.MaterialLocations.Any())
                    {
                        //Retrieve substrate from selected slot
                        var sourceSlot = item.SlotIds[slotIndex];
                        var selectedSubstrate =
                            loadPort.Carrier.MaterialLocations[sourceSlot - 1]
                                .Material as Wafer
                            ?? (Wafer)substrates
                                .FirstOrDefault(
                                    w => w.SourcePort == loadPort.InstanceId
                                         && w.SourceSlot == sourceSlot);

                        wafers.Add(selectedSubstrate);
                    }
                }
            }

            return wafers;
        }

        public static List<IProcessJob> CheckSettingUpJobDone()
        {
            var resultJobs = new List<IProcessJob>();
            var processJobs = App.ControllerInstance.GemController.E40Std.ProcessJobs.Where(pj => pj.JobState.Equals(JobState.SETTINGUP)).ToList();

            foreach (var processJob in processJobs)
            {
                if (IsAllJobWafersInstantiated(processJob))
                {
                    resultJobs.Add(processJob);
                }
            }

            return resultJobs;
        }

        public static bool IsAllJobWafersInstantiated(IProcessJob processJob)
        {
            switch (processJob.MaterialType)
            {
                case MaterialType.Carriers:
                    return IsAllWafersInstantiated(processJob.CarrierIDSlotsAssociation);

                case MaterialType.Substrates:
                    return IsAllWafersInstantiated(processJob.SubstrateNames);

                default:
                    return false;
            }
        }

        public static bool IsAllWafersInstantiated(IEnumerable<MaterialNameListElement> carrierIdSlotAssociation)
        {
            foreach (var materialNameListElement in carrierIdSlotAssociation)
            {
                foreach (var slotId in materialNameListElement.SlotIds)
                {
                    var substrateLocation =  E90Std.GetCarrierSubstrateLocation(materialNameListElement.CarrierID, slotId);
                    if (substrateLocation == null)
                    {
                        return false;
                    }

                    if (substrateLocation.SubstLocState == LocationState.Unoccupied
                        && E90Std.Substrates.FirstOrDefault(s => s.SubstSource == substrateLocation.ObjID) == null)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool IsAllWafersInstantiated(IEnumerable<string> substrateNames)
        {
            foreach (var substrateName in substrateNames)
            {
                if (E90Std.Substrates.FirstOrDefault(s => s.ObjID == substrateName) == null)
                {
                    return false;
                }
            }

            return true;
        }

        public static HCACK ConvertBoolToHcack(bool isSuccess)
        {
            return isSuccess
                ? HCACK.AcknowledgeCommandHasBeenPerformed
                : HCACK.CannotPerformedNow;
        }

        public static HCACK ConvertAcknowledgeToHcack(Acknowledge acknowledge)
        {
            switch (acknowledge)
            {
                case Acknowledge.PerformedWithErrors:
                case Acknowledge.Performed:
                    return HCACK.AcknowledgeCommandHasBeenPerformed;
                case Acknowledge.InvalidCommand:
                    return HCACK.CommandDoesNotExist;
                case Acknowledge.InvalidState:
                case Acknowledge.CannotPerformNow:
                    return HCACK.CannotPerformedNow;
                case Acknowledge.InvalidDataOrArgument:
                    return HCACK.AtLeastOneParameterIsInvalid;
                case Acknowledge.WillPerformLater:
                    return HCACK.AcknowledgeCommandWillBePerformedWithCompletionSignaledLaterByAnEvent;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static HCACK ConvertAcknowledgeToHcack(ServiceAcknowledge acknowledge)
        {
            switch (acknowledge)
            {
                case ServiceAcknowledge.PerformedSuccessful:
                    return HCACK.AcknowledgeCommandHasBeenPerformed;
                case ServiceAcknowledge.ServiceDoesNotExist:
                    return HCACK.CommandDoesNotExist;
                case ServiceAcknowledge.CannotPerformNow:
                    return HCACK.CannotPerformedNow;
                case ServiceAcknowledge.AtLeastOneParameterIsInvalid:
                    return HCACK.AtLeastOneParameterIsInvalid;
                case ServiceAcknowledge.ServiceWillBePerformedLaterWithNotification:
                    return HCACK.AcknowledgeCommandWillBePerformedWithCompletionSignaledLaterByAnEvent;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
