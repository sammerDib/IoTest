using System;
using System.Text.RegularExpressions;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Reliance.Simulation
{
    internal class Handler
    {
        private readonly string _messagePattern;
        private readonly Action<string[]> _action;

        public Handler(string messagePattern, Action<string[]> action)
        {
            _messagePattern = messagePattern;
            _action = action;
        }

        public Match Matches(string message)
        {
            return new Regex(_messagePattern).Match(message);
        }

        public void Handle(string[] parameters)
        {
            _action.Invoke(parameters);
        }
    }
}
