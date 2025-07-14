using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

using Moq;

namespace UnitySC.PM.Shared.Hardware.ControllersTests
{
    public static class MoqExtensions
    {
        private static readonly TimeSpan s_pollingDelay = TimeSpan.FromMilliseconds(10);

        public static void AssertInvocationUntil<T>(
            this Mock<T> mock,
            Expression<Action<T>> expression,
            TimeSpan timeout,
            CancellationToken cancellationToken
        ) where T : class
        {
            WaitUntil(mock, expression, timeout, cancellationToken);
        }

        /// <summary>
        /// Wait until given expression is called.
        /// </summary>
        public static void WaitUntil<T>(
            this Mock<T> mock,
            Expression<Action<T>> expression,
            TimeSpan timeout,
            CancellationToken cancellationToken
        )
            where T : class
        {
            bool calledInTime = SpinWait.SpinUntil(() =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return true;
                    }

                    try
                    {
                        mock.Verify(expression);
                        return true;
                    }
                    // Note: Exceptions should not be used here (for flow control), but this is the only way we found
                    // to do this with current methods exposed by Moq.
                    catch (MockException)
                    {
                        Thread.Sleep(s_pollingDelay);
                        return false;
                    }
                },
                timeout
            );

            if (!calledInTime && !cancellationToken.IsCancellationRequested)
            {
                throw new Exception($"Expression not called within the time limit ({timeout})");
            }
        }
    }
}
