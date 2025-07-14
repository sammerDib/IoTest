using System;

namespace UnitySC.Shared.Tools.Tracking
{
    public interface ITracker : IDisposable
    {
        void StartTracking();

        void StopTracking();

        void Reset();
    }
}
