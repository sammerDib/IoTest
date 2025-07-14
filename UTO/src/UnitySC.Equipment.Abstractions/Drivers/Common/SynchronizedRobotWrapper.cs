using System;
using System.Threading;

using Agileo.Common.Logging;
using Agileo.Drivers;

using UnitySC.Equipment.Abstractions.Drivers.Common.Exceptions;

namespace UnitySC.Equipment.Abstractions.Drivers.Common
{
    public class SynchronizedRobotWrapper
    {
        private readonly byte _expectedReplies;
        private SemaphoreSlim _sem;
        private Guid _owner;
        private Error _error;
        protected readonly ILogger Logger;

        public SynchronizedRobotWrapper(Abstractions.Devices.Robot.Robot device, byte expectedReplies = 1)
        {
            if (expectedReplies <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(expectedReplies));
            }

            _expectedReplies = expectedReplies;
            Logger           = Agileo.Common.Logging.Logger.GetLogger(device.QualifiedName);
        }

        public virtual void Set(Guid owner, Action action)
        {
            _owner = owner;
            _error = null;

            try
            {
                _sem = new SemaphoreSlim(0, 1);

                action();

                Logger.Info(FormattableString.Invariant(
                    $"SynchronizationLocker - Waiting for {_expectedReplies} unlock(s) for Guid {owner}."));

                for (int i = 0; i < _expectedReplies; i++)
                {
                    _sem.Wait();

                    if (_error != null)
                    {
                        break;
                    }
                }
            }
            finally
            {
                _sem.Dispose();
                _sem = null;
                Logger.Info(FormattableString.Invariant(
                    $"SynchronizationLocker - Locker fully released for Guid {owner}."));
            }

            if (_error != null)
            {
                throw new RobotCommandDeniedException(_error.ToString());
            }
        }

        public void CommandDenied(Guid commandUuid)
        {
            _error = commandUuid != _owner
                ? new Error { Description = "Command denied received on unknown GUID." }
                : new Error { Description = "Command denied by external." };

            Logger.Info(FormattableString.Invariant(
                $"SynchronizationLocker - 'commandDenied' unlock received for Guid {commandUuid}"));

            _sem.Release();
        }

        public void CommandGranted(Guid commandUuid)
        {
            // Check if the provided Guid correspond to the current command
            if (commandUuid != _owner)
            {
                _error = new Error { Description = "Command granted received on unknown GUID." };
            }

            Logger.Info(FormattableString.Invariant(
                $"SynchronizationLocker - 'commandGranted' unlock received for Guid {commandUuid}"));

            _sem.Release();
        }
    }
}
