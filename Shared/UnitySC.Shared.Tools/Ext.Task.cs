using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace UnitySC.Shared.Tools
{
    /// <summary>
    /// Task extension methods.
    /// </summary>
    public static class TaskExt
    {
        /// <summary>
        /// Wait for all tasks passed as parameters.
        /// throws the exception of the first task that did not complete successfully.
        /// </summary>
        public static async Task WhenAllAsync(params Task[] tasks)
        {
            try
            {
                await Task.WhenAll(tasks);
            }
            catch (Exception) { }

            // Fetch the first error, avoiding agregation in an AgregateException.
            foreach (var t in tasks)
            {
                //>exception
                await t;// t is already completed: this is just to forward a possible exception.
            }
        }

        /// <summary>
        /// Attend la tâche, mais pas plus d'un certain temps.
        /// Exception de la tâche, ou E en cas de timeOut.
        /// Attention: la tâche continue normalement en cas de cancel ou de timeOut, même si son résultat est ignoré!
        /// Si la tâche se termine dans les temps, rend son résultat (ou envoie son éventuelle exception).
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<T> cTimeOutAsync<T, E>(this Task<T> ùthis, Int32 ùtimeOut, CancellationToken ùCancel = default(CancellationToken))
            where E : Exception, new()
        {
            await Task.WhenAny(ùthis, Task.Delay(ùtimeOut), ùCancel.cAsTask_t()).ConfigureAwait(false);

            if (ùCancel.IsCancellationRequested)
            {
                throw new OperationCanceledException();
            }

            if (ùthis.IsCompleted)
            {
                //>Exception
                return ùthis.GetAwaiter().GetResult();
            }

            throw new E();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task cTimeOutAsync<E>(this Task ùthis, Int32 ùtimeOut, CancellationToken ùCancel = default(CancellationToken))
            where E : Exception, new()
        {
            await Task.WhenAny(ùthis, Task.Delay(ùtimeOut), ùCancel.cAsTask_t()).ConfigureAwait(false);

            if (ùCancel.IsCancellationRequested)
            {
                throw new OperationCanceledException();
            }

            if (ùthis.IsCompleted)
            {
                //>Exception
                ùthis.GetAwaiter().GetResult();
                return;
            }

            throw new E();
        }
        /// <summary>
        /// Blocks while condition is true or timeout occurs.
        /// </summary>
        /// <param name="condition">The condition that will perpetuate the block.</param>
        /// <param name="frequency">The frequency at which the condition will be check, in milliseconds.</param>
        /// <param name="timeout">Timeout in milliseconds.</param>
        /// <exception cref="TimeoutException"></exception>
        /// <returns></returns>
        public static async Task WaitWhile(Func<bool> condition, int frequency = 100, int timeout = 60000)
        {
            var waitTask = Task.Run(async () =>
            {
                while (condition()) await Task.Delay(frequency);
            });

            if (waitTask != await Task.WhenAny(waitTask, Task.Delay(timeout)))
                throw new TimeoutException();
        }

        /// <summary>
        /// Blocks until condition is true or timeout occurs.
        /// </summary>
        /// <param name="condition">The break condition.</param>
        /// <param name="frequency">The frequency at which the condition will be checked.</param>
        /// <param name="timeout">The timeout in milliseconds.</param>
        /// <returns></returns>
        public static async Task WaitUntil(Func<bool> condition, int frequency = 100, int timeout = 60000)
        {
            var waitTask = Task.Run(async () =>
            {
                while (!condition()) await Task.Delay(frequency);
            });

            if (waitTask != await Task.WhenAny(waitTask, Task.Delay(timeout)))
            {
                throw new TimeoutException();
            }
        }
    }
}
