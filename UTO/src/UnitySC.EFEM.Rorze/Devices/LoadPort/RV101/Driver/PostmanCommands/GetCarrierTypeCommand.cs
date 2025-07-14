using System;

using Agileo.Common.Communication;

using UnitySC.EFEM.Rorze.Devices.LoadPort.RorzeLoadPort.Driver.Status;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.Enums;
using UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.Status;
using UnitySC.EFEM.Rorze.Drivers;
using UnitySC.Equipment.Abstractions.Drivers.Common.EventArgs;

namespace UnitySC.EFEM.Rorze.Devices.LoadPort.RV101.Driver.PostmanCommands
{
    public class GetCarrierTypeCommand : RV101Command
    {
        private readonly GetCarrierTypeParameter _orderParameter;

        #region Constructors

        public static GetCarrierTypeCommand NewOrder(
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            GetCarrierTypeParameter commandParameter = GetCarrierTypeParameter.AcquireIdentificationMethodAndCarrierType)
        {
            if (commandParameter == GetCarrierTypeParameter.AcquireIdentificationMethodAndCarrierType)
            {
                // Standard use case, parameter should be omitted.
                return new GetCarrierTypeCommand(RorzeConstants.CommandTypeAbb.Order, deviceId, sender, eqFacade);
            }

            return new GetCarrierTypeCommand(RorzeConstants.CommandTypeAbb.Order, deviceId, sender, eqFacade, ((int)commandParameter).ToString());
        }

        public static GetCarrierTypeCommand NewEvent(
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade)
        {
            return new GetCarrierTypeCommand(RorzeConstants.CommandTypeAbb.Event, deviceId, sender, eqFacade);
        }

        private GetCarrierTypeCommand(
            char commandType,
            byte deviceId,
            IMacroCommandSender sender,
            IEquipmentFacade eqFacade,
            params string[] commandParameters)
            : base(commandType, RorzeConstants.DeviceTypeAbb.LoadPort, deviceId,
                RorzeConstants.Commands.CarrierTypeAcquisition,
                sender, eqFacade, commandParameters)
        {
            if (!commandType.Equals(RorzeConstants.CommandTypeAbb.Order))
            {
                return;
            }

            if (commandParameters.Length == 0)
            {
                _orderParameter = GetCarrierTypeParameter.AcquireIdentificationMethodAndCarrierType;
            }
            else if (Enum.TryParse(commandParameters[0], out GetCarrierTypeParameter getCarrierTypeParameter))
            {
                _orderParameter = getCarrierTypeParameter;
            }
            else
            {
                _orderParameter = GetCarrierTypeParameter.AcquireIdentificationMethodAndCarrierType;
            }
        }

        #endregion Constructors

        #region RorzeCommand

        protected override bool TreatAck(RorzeMessage message)
        {
            var deviceName = message.DeviceType + message.DeviceId;
            switch (_orderParameter)
            {
                case GetCarrierTypeParameter.AcquireIdentificationMethodAndCarrierType:
                    var splitMessageData = message.Data.Split('/');

                    facade.SendEquipmentEvent(
                        (int)EFEMEvents.CarrierIdentificationMethodReceived,
                        new StatusEventArgs<CarrierTypeStatus>(deviceName, new CarrierTypeStatus(splitMessageData[0])));
                    facade.SendEquipmentEvent(
                        (int)EFEMEvents.CarrierTypeReceived,
                        new StatusEventArgs<CarrierTypeStatus>(deviceName, new CarrierTypeStatus(splitMessageData[1])));
                    break;

                case GetCarrierTypeParameter.GetIdentificationMethodOnly:
                    facade.SendEquipmentEvent(
                        (int)EFEMEvents.CarrierIdentificationMethodReceived,
                        new StatusEventArgs<CarrierTypeStatus>(deviceName, new CarrierTypeStatus(message.Data)));
                    break;

                case GetCarrierTypeParameter.GetRealCarrierTypeOnly:
                    facade.SendEquipmentEvent(
                        (int)EFEMEvents.CarrierTypeReceived,
                        new StatusEventArgs<CarrierTypeStatus>(deviceName, new CarrierTypeStatus(message.Data)));
                    break;

                case GetCarrierTypeParameter.GetInfoPadInput:
                    facade.SendEquipmentEvent(
                        (int)EFEMEvents.InfoPadInputReceived,
                        new StatusEventArgs<InfoPadsInputStatus>(deviceName, new InfoPadsInputStatus(message.Data)));
                    break;

                default:
                    return false;
            }

            facade.SendEquipmentEvent((int)EFEMEvents.GetCarrierTypeCompleted, System.EventArgs.Empty);

            CommandComplete();
            return true;
        }

        protected override bool TreatEvent(RorzeMessage message)
        {
            if (message.Name != RorzeConstants.Commands.CarrierTypeAcquisition)
            {
                return false;
            }

            var deviceName       = message.DeviceType + message.DeviceId;
            var splitMessageData = message.Data.Split('/');

            facade.SendEquipmentEvent(
                (int)EFEMEvents.CarrierIdentificationMethodReceived,
                new StatusEventArgs<CarrierTypeStatus>(deviceName, new CarrierTypeStatus(splitMessageData[0])));
            facade.SendEquipmentEvent(
                (int)EFEMEvents.CarrierTypeReceived,
                new StatusEventArgs<CarrierTypeStatus>(deviceName, new CarrierTypeStatus(splitMessageData[1])));

            return true;
        }

        #endregion RorzeCommand
    }
}
