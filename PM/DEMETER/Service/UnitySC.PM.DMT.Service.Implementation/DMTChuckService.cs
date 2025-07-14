using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.DMT.Service.Interface.Chuck;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Service.Implementation;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;
using UnitySC.PM.Shared.Hardware.Service.Interface.Rfid;
using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck;
using UnitySC.PM.Shared.ReformulationMessage;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.DMT.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class DMTChuckService : USPChuckService<IDMTChuckServiceCallback>, IDMTChuckService, IChuckServiceCallbackProxy
    {
        protected HardwareManager HardwareManager;

        private const string DeviceName = "Chuck";

        public DMTChuckService(ILogger logger) : base(logger)
        {
            HardwareManager = ClassLocator.Default.GetInstance<HardwareManager>();
            IMessenger messenger = ClassLocator.Default.GetInstance<IMessenger>();
            messenger.Register<WaferPresenceMessage>(this, (r, m) => { UpdateWaferPresence(m.WaferPresence); });
            messenger.Register<TagMessage>(this, (r, m) => { OnTagChanged(m.Tag); });
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

        public void OnTagChanged(string value)
        {
            InvokeCallback(i => i.TagChangedCallback(value));
        }

        public Response<RfidTag> GetTag()
        {
            var rfid = HardwareManager.Rfids?.FirstOrDefault().Value;
            return InvokeDataResponse(messagesContainer =>
            {
                return rfid?.GetTag();
            });
        }

        public void StateChanged(ChuckState chuckState)
        {
            InvokeCallback(chuckServiceCallback => chuckServiceCallback.StateChangedCallback(chuckState));
        }
    }
}
