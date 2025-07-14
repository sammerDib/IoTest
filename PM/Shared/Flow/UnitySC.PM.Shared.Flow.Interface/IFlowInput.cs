using System.Collections.Generic;

namespace UnitySC.PM.Shared.Flow.Interface
{
    public struct InputValidity
    {
        public InputValidity(bool isValid, List<string> message)
        {
            IsValid = isValid;
            Message = message;
        }

        public InputValidity(bool isValid)
        {
            IsValid = isValid;
            Message = new List<string>();
        }

        public bool IsValid;
        public List<string> Message;

        public void ComposeWith(InputValidity validity)
        {
            IsValid &= validity.IsValid;
            Message.AddRange(validity.Message);
        }

        public override string ToString()
        {
            var messageToDisplay = string.Join(" , ", Message);
            return messageToDisplay;
        }
    }

    public interface IFlowInput
    {
        InputValidity CheckInputValidity();
    }
}
