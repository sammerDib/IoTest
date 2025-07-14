using System;
using System.Collections.Generic;
using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.DMT.Hardware;
using UnitySC.PM.DMT.Hardware.Manager;
using UnitySC.PM.DMT.Service.Interface.Chamber;
using UnitySC.PM.Shared.Hardware;
using UnitySC.PM.Shared.Hardware.Chamber;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Chambers;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chamber;
using UnitySC.PM.Shared.ReformulationMessage;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.DMT.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class DMTChamberService : DuplexServiceBase<IDMTChamberServiceCallback>, IDMTChamberService
    {
        private HardwareManager _hardwareManager;
        private const string DeviceName = "Chamber";

        public DMTChamberService(ILogger logger) : base(logger, ExceptionType.HardwareException)
        {
            _hardwareManager = ClassLocator.Default.GetInstance<HardwareManager>();

            IMessenger messenger = ClassLocator.Default.GetInstance<IMessenger>();
            messenger.Register<IsInMaintenanceMessage>(this, (r, m) => { UpdateIsInMaintenance(m.IsInMaintenance); });
            messenger.Register<ArmNotExtendedMessage>(this, (r, m) => { UpdateArmNotExtended(m.ArmNotExtended); });
            messenger.Register<EfemSlitDoorOpenPositionMessage>(this, (r, m) => { UpdateEfemSlitDoorOpenPosition(m.EfemSlitDoorOpenPosition); });
            messenger.Register<IsReadyToLoadUnloadMessage>(this, (r, m) => { UpdateIsReadyToLoadUnload(m.IsReadyToLoadUnload); });
            messenger.Register<InterlockMessage>(this, (r, m) => UpdateInterlocks(m));
            messenger.Register<SlitDoorPositionMessage>(this, (r, m) => { UpdateSlitDoorPosition(m.SlitDoorPosition); });
            messenger.Register<SlitDoorOpenPositionMessage>(this, (r, m) => { UpdateSlitDoorOpenPosition(m.SlitDoorOpenPosition); });
            messenger.Register<SlitDoorClosePositionMessage>(this, (r, m) => { UpdateSlitDoorClosePosition(m.SlitDoorClosePosition); });
            messenger.Register<CdaPneumaticValveMessage>(this, (r, m) => { UpdateCdaPneumaticValve(m.ValveIsOpened); });
        }

        public override void Init()
        {
            base.Init();
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                base.Unsubscribe();
            });
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                base.Subscribe();
            });
        }

        public Response<VoidResult> TriggerUpdateEvent()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _hardwareManager.Chamber.TriggerUpdateEvent();
            });
        }

        public Response<DMTChamberConfig> GetChamberConfiguration()
        {
            return InvokeDataResponse(messageContainer =>
            {
                if (_hardwareManager.Chamber == null) // Initialization not terminated
                    return null;

                DMTChamberConfig config = null;                
                try
                {
                    config = (DMTChamberConfig)_hardwareManager.Chamber.Configuration;
                }
                catch (Exception ex)
                {
                    ReformulationMessage(messageContainer, ex.Message);
                }
                return config;
            });
        }

        public void UpdateIsInMaintenance(bool value)
        {
            InvokeCallback(i => i.UpdateIsInMaintenanceCallback(value));
        }

        public void UpdateArmNotExtended(bool value)
        {
            InvokeCallback(i => i.UpdateArmNotExtendedCallback(value));
        }

        private void UpdateEfemSlitDoorOpenPosition(bool value)
        {
            InvokeCallback(i => i.UpdateEfemSlitDoorOpenPositionCallback(value));
        }

        private void UpdateIsReadyToLoadUnload(bool value)
        {
            InvokeCallback(i => i.UpdateIsReadyToLoadUnloadCallback(value));
        }

        public void UpdateInterlocks(InterlockMessage interlock)
        {
            InvokeCallback(i => i.UpdateInterlocksCallback(interlock));
        }

        public void UpdateSlitDoorPosition(SlitDoorPosition position)
        {
            InvokeCallback(i => i.UpdateSlitDoorPositionCallback(position));
        }

        public void UpdateSlitDoorOpenPosition(bool position)
        {
            InvokeCallback(i => i.UpdateSlitDoorOpenPositionCallback(position));
        }

        public void UpdateSlitDoorClosePosition(bool position)
        {
            InvokeCallback(i => i.UpdateSlitDoorClosePositionCallback(position));
        }

        public void UpdateCdaPneumaticValve(bool valveIsOpened)
        {
            InvokeCallback(i => i.UpdateCdaPneumaticValveCallback(valveIsOpened));
        }

        public Response<VoidResult> OpenSlitDoor()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                IPSDChamber chamber = (IPSDChamber)_hardwareManager.Chamber;
                if (chamber != null)
                {
                    chamber.OpenSlitDoor();
                }
            });
        }

        public Response<VoidResult> CloseSlitDoor()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                IPSDChamber chamber = (IPSDChamber)_hardwareManager.Chamber;
                if (chamber != null)
                {
                    chamber.CloseSlitDoor();
                }
            });
        }

        public Response<VoidResult> OpenCdaPneumaticValve()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                IPSDChamber chamber = (IPSDChamber)_hardwareManager.Chamber;
                if (chamber != null)
                {
                    chamber.OpenCdaPneumaticValve();
                }
            });
        }

        public Response<VoidResult> CloseCdaPneumaticValve()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                IPSDChamber chamber = (IPSDChamber)_hardwareManager.Chamber;
                if (chamber != null)
                {
                    chamber.CloseCdaPneumaticValve();
                }
            });
        }

        private static void ReformulationMessage(List<Message> messageContainer, string message, MessageLevel defaultLevel = MessageLevel.Error)
        {
            var userContent = ReformulationMessageManager.GetUserContent(DeviceName, message, message);
            var level = ReformulationMessageManager.GetLevel(DeviceName, message, defaultLevel);
            messageContainer.Add(new Message(level, userContent, message, DeviceName));
        }
    }
}
