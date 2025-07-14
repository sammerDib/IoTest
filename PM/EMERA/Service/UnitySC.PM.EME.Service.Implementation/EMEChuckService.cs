using System;
using System.Collections.Generic;
using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.EME.Service.Interface.Chuck;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Service.Implementation;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;
using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck;
using UnitySC.PM.Shared.ReformulationMessage;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.EME.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class EMEChuckService : USPChuckService<IEMEChuckServiceCallback>, IEMEChuckService, IChuckServiceCallbackProxy
    {
        protected HardwareManager HardwareManager;

        private const string DeviceName = "Chuck";

        public EMEChuckService(ILogger logger) : base(logger)
        {
            HardwareManager = ClassLocator.Default.GetInstance<HardwareManager>();
            IMessenger messenger = ClassLocator.Default.GetInstance<IMessenger>();
            messenger.Register<WaferPresenceMessage>(this, (r, m) => { UpdateWaferPresence(m.WaferPresence); });
            messenger.Register<ChuckIsInLoadingPositionMessage>(this, (r, m) => { UpdateChuckIsInLoadingPosition(m.IsInLoadingPosition); });
        }

        public override void Init()
        {
            base.Init();
        }

        private static void ReformulationMessage(List<Message> messageContainer, string message, MessageLevel defaultLevel = MessageLevel.Error)
        {
            var content = "Subscribed to light change";
            var userContent = ReformulationMessageManager.GetUserContent(DeviceName, content, message);
            var level = ReformulationMessageManager.GetLevel(DeviceName, content, defaultLevel);
            messageContainer.Add(new Message(level, userContent, message, DeviceName));
        }

        public void UpdateWaferPresence(MaterialPresence waferPresence)
        {
            InvokeCallback(i => i.UpdateWaferPresenceCallback(waferPresence));
        }

        public void UpdateChuckIsInLoadingPosition(bool loadingPosition)
        {
            InvokeCallback(i => i.UpdateChuckIsInLoadingPositionCallback(loadingPosition));
        }

        public Response<bool> ClampWafer(WaferDimensionalCharacteristic wafer)
        {
            return InvokeDataResponse(messageContainer =>
            {
                try
                {
                    HardwareManager.ClampHandler.ClampWafer(wafer.Diameter);
                    return true;
                }
                catch (Exception ex)
                {
                    ReformulationMessage(messageContainer, ex.Message);
                    return false;
                }
            });
        }

        public Response<bool> ReleaseWafer(WaferDimensionalCharacteristic wafer)
        {
            return InvokeDataResponse(messageContainer =>
            {
                try
                {
                    HardwareManager.ClampHandler.ReleaseWafer(wafer.Diameter);
                    return true;
                }
                catch (Exception ex)
                {
                    ReformulationMessage(messageContainer, ex.Message);
                    return false;
                }
            });
        }

        public void StateChanged(ChuckState chuckState)
        {
            InvokeCallback(chuckServiceCallback => chuckServiceCallback.StateChangedCallback(chuckState));
        }
    }
}
