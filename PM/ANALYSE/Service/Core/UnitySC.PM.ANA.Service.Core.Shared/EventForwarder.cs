using System;

using UnitySCSharedAlgosOpenCVWrapper;

using UnitySC.Shared.Logger;

namespace UnitySC.PM.ANA.Service.Core.Shared
{
    /// <summary>
    /// Forwarder for Events coming form algorithm library
    /// Forwards event from library's event queue to the ANALYSE logger service
    /// </summary>
    public class EventForwarder
    {
        private ILogger _logger;
        private string _messagePrefix;

        public EventForwarder(ILogger logger, string messagePrefix)
        {
            _logger = logger;
            _messagePrefix = messagePrefix;
        }

        public void ForwardEvent(Object source, EventArgs args)
        {
            if (args is AlgoEventArgs)
            {
                AlgoEventArgs aea = (AlgoEventArgs)args;
                string message = _messagePrefix + aea.Message;
                switch (aea.Severity)
                {
                    case Severity.Verbose:
                        _logger.Verbose(message);
                        break;

                    case Severity.Debug:
                        _logger.Debug(message);
                        break;

                    case Severity.Info:
                        _logger.Information(message);
                        break;

                    case Severity.Warning:
                        _logger.Warning(message);
                        break;

                    case Severity.Error:
                        _logger.Error(message);
                        break;

                    case Severity.Fatal:
                        _logger.Fatal(new ApplicationException(message), message);
                        break;

                    default:
                        throw new ApplicationException("Unknown Event severity");
                }
            }
        }
    };
}
