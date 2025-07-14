using System;
using System.Runtime.CompilerServices;

using Agileo.Common.Logging;
using Agileo.Semi.Gem.Abstractions.E30;

namespace UnitySC.UTO.Controller.Remote.Services
{
    internal class TerminalServices : E30StandardSupport
    {
        #region Constructors

        public TerminalServices(IE30Standard e30Standard, ILogger logger)
            : base(e30Standard, logger)
        {
        }

        #endregion Constructors

        #region Methods

        public void SendTerminalMessage(string message)
        {
            E30Standard?.TerminalServices.SendMessage(Constants.Services.TerminalId, message);
            LogMethodCalled(attachment: message);
        }

        public void SendTerminalMessageRecognized()
        {
            E30Standard?.TerminalServices.RecognizeAllMessages();
            LogMethodCalled();
        }

        private void LogMethodCalled(string attachment = null, [CallerMemberName] string memberName = null)
        {
            Logger.Info($"{memberName} called", attachment);
        }

        #endregion Methods

        #region BublingEvents

        public event EventHandler<TerminalMessageReceivedEventArgs> OnMessageReceived
        {
            add { E30Standard.TerminalServices.MessageReceived += value; }
            remove { E30Standard.TerminalServices.MessageReceived -= value; }
        }

        #endregion BublingEvents
    }
}
