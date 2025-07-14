using System.Collections.Generic;
using System.Runtime.Serialization;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Status.Service.Interface
{
    [DataContract]
    public class GlobalStatus
    {
        public GlobalStatus(PMGlobalStates currentState)
        {
            CurrentState = currentState;
        }

        public GlobalStatus(Message statusMessage)
        {
            Messages = new List<Message>();
            Messages.Add(statusMessage);
        }

        public GlobalStatus(List<Message> statusMessages)
        {
            Messages = new List<Message>();
            Messages.AddRange(statusMessages);
        }

        public GlobalStatus(PMGlobalStates currentState, Message statusMessage) : this(statusMessage)
        {
            CurrentState = currentState;
        }

        public GlobalStatus(PMGlobalStates currentStatus, List<Message> statusMessages) : this(statusMessages)
        {
            CurrentState = currentStatus;
        }

        public GlobalStatus(PMControlModeSwitch controlModeSwitch)
        {
            ControlModeSwitch = controlModeSwitch;
        }


        [DataMember]
        public PMGlobalStates? CurrentState { get; set; } = null;

        [DataMember]
        public List<Message> Messages { get; set; }

        [DataMember]
        public PMControlModeSwitch ControlModeSwitch { get; set; } = PMControlModeSwitch.NoSwitch;
    }
}
