using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace UnitySC.Shared.Tools
{
    /// <summary>
    /// Attention à l'utilisation de ContinueHorsContexte() pour un post...Async_ts(...):
    ///  la continuation se fera typiquement sur le thread du dispatcher, ce qui risque de charger la messageLoop!
    /// </summary>
    public static class DispatcherExt
    {
        public struct Awaiter : INotifyCompletion
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Awaiter(Dispatcher ùDispatcher_d)
            {
                _cDispatcher_d = ùDispatcher_d;
            }

            private readonly Dispatcher _cDispatcher_d;

            public bool IsCompleted => Thread.CurrentThread.ManagedThreadId == _cDispatcher_d.Thread.ManagedThreadId;

            /// <summary>
            /// Appelé une fois terminé.
            /// Ne sert qu'à renvoyer une éventuelle exception.
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void GetResult()
            {
#if DEBUG   // "When used by the compiler, the task will already be complete" https://referencesource.microsoft.com/#mscorlib/system/runtime/compilerservices/TaskAwaiter.cs,6f62d20209e01442
                if (!IsCompleted)
                {
                    Debugger.Break();
                }
#endif
            }

            /// <summary>
            /// OnCompleted porte mal son nom (décidément!): il est appelé en synchrone au moment du await, juste après la partie synchrone de la tâche awaitée.
            /// Son paramètre est la continuation qui suit le await.
            ///
            /// La continuation est en pratique un appel à GetResult() pour les exceptions, suivie du code de continuation.
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void OnCompleted(Action continuation)
            {
                if (IsCompleted)
                {
                    // Déjà terminé lors du await, inlining.
                    continuation();
                }
                else if (_cDispatcher_d.Thread.ManagedThreadId == Thread.CurrentThread.ManagedThreadId)
                {
                    // Inlining.
                    continuation();
                }
                else
                {
                    _cDispatcher_d.BeginInvoke(continuation);
                }
            }
        }

        /// <summary>
        /// Permet un "await Dispatcher" pour repasser en messageLoop... :)
        /// Inline les appels venant du thread du dispatcher.
        /// </summary>
        public static Awaiter GetAwaiter(this Dispatcher ùThis)
        {
            return new Awaiter(ùThis);
        }

        /// <summary>
        /// Dispatcher.InvokeAsync(...) ne permet pas d'appeler une fonction async. Cette extension le permet.
        /// Attention, les extensions async pour SL rendent bien Task, mais ne fonctionnent pas correctement... Privilégier ces fonctions!
        /// </summary>
        /// <summary>
        /// OBSOLETE privilégier await Dispatcher
        /// </summary>
        public static Task<_R> postAsync_ts<_R>(this Dispatcher this_d, Func<Task<_R>> callback_f)
        {
            var tcs = new TaskCompletionSource<_R>();

            this_d.BeginInvoke((Action)(async () =>
            {
                try
                {
                    tcs.TrySetResult(await callback_f().ConfigureAwait(false));
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            }));

            return tcs.Task;
        }

        /// <summary>
        /// Idem Dispatcher.InvokeAsync(...), mais rendant un classique Task et non une DispatcherOperation.
        /// Attention, les extensions async pour SL rendent bien Task, mais ne fonctionnent pas correctement... Privilégier ces fonctions!
        /// </summary>
        /// <summary>
        /// OBSOLETE privilégier await Dispatcher
        /// </summary>
        public static Task<_R> postSansAsync_ts<_R>(this Dispatcher this_d, Func<_R> callback_f)
        {
            var tcs = new TaskCompletionSource<_R>();

            this_d.BeginInvoke((Action)(() =>
            {
                try
                {
                    tcs.TrySetResult(callback_f());
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            }));

            return tcs.Task;
        }

        /// <summary>
        /// Dispatcher.InvokeAsync(...) ne permet pas d'appeler une fonction async. Cette extension le permet.
        /// Attention, les extensions async pour SL rendent bien Task, mais ne fonctionnent pas correctement... Privilégier ces fonctions!
        /// </summary>
        /// <summary>
        /// OBSOLETE privilégier await Dispatcher
        /// </summary>
        public static Task postAsync_ts(this Dispatcher this_d, Func<Task> callback_f)
        {
            var tcs = new TaskCompletionSource<bool>();

            this_d.BeginInvoke((Action)(async () =>
            {
                try
                {
                    await callback_f().ConfigureAwait(false);
                    tcs.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            }));

            return tcs.Task;
        }

        /// <summary>
        /// Idem Dispatcher.InvokeAsync(...), mais rendant un classique Task et non une DispatcherOperation.
        /// Attention, les extensions async pour SL rendent bien Task, mais ne fonctionnent pas correctement... Privilégier ces fonctions!
        /// </summary>
        /// <summary>
        /// OBSOLETE privilégier await Dispatcher
        /// </summary>
        public static Task postSansAsync_ts(this Dispatcher this_d, Action callback_f)
        {
            var tcs = new TaskCompletionSource<bool>();

            this_d.BeginInvoke((Action)(() =>
            {
                try
                {
                    callback_f();
                    tcs.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            }));

            return tcs.Task;
        }
    }
}