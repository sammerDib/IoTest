using System;
using System.Threading;
using System.Threading.Tasks;

namespace UnitySC.Shared.Test.Tools
{
    public class TestTools
    {
        public static TimeSpan PollingDelay = TimeSpan.FromMilliseconds(10);

        public static async Task WaitUntil(Func<bool> predicate, TimeSpan timeout)
        {
            var cancellationTokenSource = new CancellationTokenSource(timeout);
            while (!predicate())
            {
                if (cancellationTokenSource.IsCancellationRequested)
                {
                    throw new Exception("Predicate was not satisfied before timeout");
                }

                await Task.Delay(PollingDelay, cancellationTokenSource.Token);
            }
        }
    }
}
