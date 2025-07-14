using System;
using System.Collections.Generic;

using Agileo.Common.Communication;

using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver.PostmanCommands;
using UnitySC.EFEM.Rorze.Devices.IoModule.GenericRC5xx.Driver.Status;
using UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Driver.Statuses;
using UnitySC.EFEM.Rorze.Drivers;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.RC550.Dio0.Driver.PostmanCommands
{
    public class GetDio0SignalCommand : GetIoSignalCommand
    {
        public static GetIoSignalCommand NewOrder(
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            IoModuleIds moduleId,
            int unitNumber)
        {
            var parameters = new List<string>
            {
                ((int)moduleId).ToString("000"),
                unitNumber.ToString()
            };

            return new GetDio0SignalCommand(RorzeConstants.CommandTypeAbb.Order, deviceId, sender, eqFacade, parameters.ToArray());
        }

        public static GetIoSignalCommand NewEvent(
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
        {
            return new GetDio0SignalCommand(RorzeConstants.CommandTypeAbb.Event, deviceId, sender, eqFacade);
        }

        private GetDio0SignalCommand(
            char commandType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            params string[] commandParameters)
            : base(commandType, deviceId, sender, eqFacade, commandParameters)
        {
        }

        protected override List<SignalData> GetIoSignals(string messageData)
        {
            var parsedData = messageData.Split('/');

            // There must be at least the I/O module No. and one I/O status in the received data.
            if (parsedData.Length < 2)
            {
                throw new ArgumentException(
                    "Invalid format of argument.\n"
                    + "Format is \"nnn/********/********/...\" where '*' is a one-digit hexadecimal number and 'n' a one-digit decimal number.\n"
                    + $"Received data is {messageData}.");
            }

            // The I/O module No. must be an integer
            if (!uint.TryParse(parsedData[0], out var ioModuleId))
            {
                throw new ArgumentException(
                    "The I/O module No. must be an integer.\n"
                    + "Format is \"nnn/********/********/...\" where '*' is a one-digit hexadecimal number and 'n' a one-digit decimal number.\n"
                    + $"Received data is {messageData}.");
            }

            var signalList = new List<SignalData>(parsedData.Length - 1);

            for (var i = 1; i < parsedData.Length; ++i)
            {
                switch ((IoModuleIds)ioModuleId)
                {
                    case IoModuleIds.RC550_HCL3_ID0:
                        signalList.Add(new FanDetectionSignalData(ioModuleId, parsedData[i]));
                        break;

                    // LPs
                    case IoModuleIds.RC550_HCL1_ID0:
                    case IoModuleIds.RC550_HCL1_ID1:
                    case IoModuleIds.RC550_HCL1_ID2:
                    case IoModuleIds.RC550_HCL1_ID3:
                        signalList.Add(
                            new LayingPlanLoadPortSignalData(
                                ioModuleId,
                                ioModuleId,
                                parsedData[i]));
                        break;

                    // LP1 E84
                    case IoModuleIds.SB078_Port4_HCL0_ID0:
                    case IoModuleIds.SB078_Port4_HCL0_ID1:
                        signalList.Add(new E84SignalData((uint)IoModuleIds.SB078_Port4_HCL0_ID0, ioModuleId, parsedData[i]));
                        break;

                    // LP2 E84
                    case IoModuleIds.SB078_Port4_HCL0_ID2:
                    case IoModuleIds.SB078_Port4_HCL0_ID3:
                        signalList.Add(new E84SignalData((uint)IoModuleIds.SB078_Port4_HCL0_ID2, ioModuleId, parsedData[i]));
                        break;

                    // LP3 E84
                    case IoModuleIds.SB078_Port4_HCL1_ID0:
                    case IoModuleIds.SB078_Port4_HCL1_ID1:
                        signalList.Add(new E84SignalData((uint)IoModuleIds.SB078_Port4_HCL1_ID0, ioModuleId, parsedData[i]));
                        break;

                    // LP4 E84
                    case IoModuleIds.SB078_Port4_HCL1_ID2:
                    case IoModuleIds.SB078_Port4_HCL1_ID3:
                        signalList.Add(new E84SignalData((uint)IoModuleIds.SB078_Port4_HCL1_ID2, ioModuleId, parsedData[i]));
                        break;

                    // LP5 E84
                    case IoModuleIds.SB078_Port4_HCL2_ID0:
                    case IoModuleIds.SB078_Port4_HCL2_ID1:
                        signalList.Add(new E84SignalData((uint)IoModuleIds.SB078_Port4_HCL2_ID0, ioModuleId, parsedData[i]));
                        break;

                    // LP6 E84
                    case IoModuleIds.SB078_Port4_HCL2_ID2:
                    case IoModuleIds.SB078_Port4_HCL2_ID3:
                        signalList.Add(new E84SignalData((uint)IoModuleIds.SB078_Port4_HCL2_ID2, ioModuleId, parsedData[i]));
                        break;

                    // LP7 E84
                    case IoModuleIds.SB078_Port4_HCL3_ID0:
                    case IoModuleIds.SB078_Port4_HCL3_ID1:
                        signalList.Add(new E84SignalData((uint)IoModuleIds.SB078_Port4_HCL3_ID0, ioModuleId, parsedData[i]));
                        break;

                    // LP8 E84
                    case IoModuleIds.SB078_Port4_HCL3_ID2:
                    case IoModuleIds.SB078_Port4_HCL3_ID3:
                        signalList.Add(new E84SignalData((uint)IoModuleIds.SB078_Port4_HCL3_ID2, ioModuleId, parsedData[i]));
                        break;

                    default:
                        throw new NotSupportedException(
                            $"{nameof(GetDio0SignalCommand)} - {nameof(GetIoSignals)} does not manage IO controller ID {ioModuleId}");
                }

                // Initial ioModuleId is the start index. If there are more than 1 signal, it should be increased by one each time.
                ++ioModuleId;
            }

            return signalList;
        }
    }
}
