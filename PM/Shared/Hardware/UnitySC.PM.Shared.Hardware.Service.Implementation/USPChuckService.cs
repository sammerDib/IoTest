using System;
using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;
using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck;
using UnitySC.PM.Shared.Hardware.USPChuck;
using UnitySC.PM.Shared.ReformulationMessage;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class USPChuckService<T> : DuplexServiceBase<T>, IUSPChuckService
        where T : class
    {
        private readonly HardwareManager _hardwareManager;
        private const string DeviceName = "Chuck";

        public USPChuckService(ILogger logger) : base(logger, ExceptionType.HardwareException)
        {
            _hardwareManager = ClassLocator.Default.GetInstance<HardwareManager>();
        }

        public override void Init()
        {
            base.Init();
        }
        
        public Response<VoidResult> UnSubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                Unsubscribe();
            });
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                Subscribe();
            });
        }
        // TODO: Verifier la pertinence
        public Response<VoidResult> RefreshAllValues()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                lock (this)
                {
                    try
                    {
                        if (_hardwareManager.Chuck is USPChuckBase uspChuck)
                            uspChuck.TriggerUpdateEvent();
                    }
                    catch (Exception e)
                    {
                        ReformulationMessage(messageContainer, e.Message);
                    }
                }
            });
        }

        public Response<ChuckState> GetCurrentState()
        {
            return InvokeDataResponse(messageContainer =>
            {
                if (_hardwareManager.Chuck == null)
                    return null;

                ChuckState chuckState = null;
                try
                {
                    chuckState = _hardwareManager.Chuck.GetState();
                }
                catch (Exception ex)
                {
                    ReformulationMessage(messageContainer, ex.Message);
                }

                return chuckState;
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
