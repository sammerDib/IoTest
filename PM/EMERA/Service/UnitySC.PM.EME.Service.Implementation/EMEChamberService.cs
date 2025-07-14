using System;
using System.Collections.Generic;
using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.EME.Hardware.Chamber;
using UnitySC.PM.EME.Service.Interface.Chamber;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chamber;
using UnitySC.PM.Shared.ReformulationMessage;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.EME.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class EMEChamberService : DuplexServiceBase<IEMEChamberServiceCallback>, IEMEChamberService
    {
        private readonly HardwareManager _hardwareManager;
        private const string DeviceName = "Chamber";

        public EMEChamberService(ILogger logger) : base(logger, ExceptionType.HardwareException)
        {
            _hardwareManager = ClassLocator.Default.GetInstance<HardwareManager>();

            IMessenger messenger = ClassLocator.Default.GetInstance<IMessenger>();
            messenger.Register<IsInMaintenanceMessage>(this, (r, m) => { UpdateIsInMaintenance(m.IsInMaintenance); });
            messenger.Register<ArmNotExtendedMessage>(this, (r, m) => { UpdateArmNotExtended(m.ArmNotExtended); });
            messenger.Register<InterlockMessage>(this, (r, m) => UpdateInterlocks(m));
            messenger.Register<SlitDoorPositionMessage>(this, (r, m) => { UpdateSlitDoorPosition(m.SlitDoorPosition); });
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
                if (_hardwareManager.Chamber != null)
                    _hardwareManager.Chamber.TriggerUpdateEvent();
            });
        }

        public Response<EMEChamberConfig> GetChamberConfiguration()
        {
            return InvokeDataResponse(messageContainer =>
            {
                if (_hardwareManager.Chamber == null) // Initialization not terminated
                    return null;

                EMEChamberConfig config = null;
                try
                {
                    if(_hardwareManager.Chamber.Configuration is EMEChamberConfig emeChamberConfig)
                        config = emeChamberConfig;                    
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

        public void UpdateInterlocks(InterlockMessage interlock)
        {
            InvokeCallback(i => i.UpdateInterlocksCallback(interlock));
        }

        public void UpdateSlitDoorPosition(SlitDoorPosition position)
        {
            InvokeCallback(i => i.UpdateSlitDoorPositionCallback(position));
        }
        
        public Response<VoidResult> OpenSlitDoor()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                IEMEChamber chamber = (IEMEChamber)_hardwareManager.Chamber;
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
                IEMEChamber chamber = (IEMEChamber)_hardwareManager.Chamber;
                if (chamber != null)
                {
                    chamber.CloseSlitDoor();
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
