using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace UnitySC.Shared.Tools
{
    /// <summary>
    /// Par ordre de performance:
    ///
    /// await ThreadPoolTools.InlineOrPost
    ///  lorsqu'on SAIT que le thread d'origine (début) de la fonction n'était pas le même que celui de l'appel à l'awaiter.
    ///
    /// Int32 ùThreadOrigine_s32 = Thread.CurrentThread.ManagedThreadId;
    /// ...
    /// await ThreadPoolTools.InlineOuPoste(ùThreadOrigine_s32)
    ///  lorsqu'on ne le sait pas (on teste).
    ///
    /// await ThreadPoolTools.Post
    ///  lorsqu'on sait qu'on est sur le même thread que le début de la fonction, ou lorsque'on veut explicitement poster en threadPool.
    ///
    /// La partie synchrone d'une fonction async doit toujours rester rapide:
    /// On pourraît être tenté de toujours inliner en threadPool: si on y est déjà, pourquoi ne pas se permettre de bloquer, et limiter la synchro entre threads?
    /// En pratique, c'est piégeux, car par exemple dans le cas d'un:
    ///  await Task.WhenAll(Fct1Async(), Fct2Async())
    /// appelé depuis la threadPool, si Fct1Async() se permet de bloquer, alors Fct2Async() ne sera pas exécuté en parallèle, mais après!
    /// On peut par contre inliner en threadPool si on est surs que l'origine de l'appel (début de la fonction en cours) s'est fait dans un autre thread!
    /// </summary>
    public static class ThreadPoolTools
    {
        static ThreadPoolTools()
        {
            // Don't limit the threadPool.
            ThreadPool.SetMaxThreads(Int32.MaxValue, Int32.MaxValue);
        }

        public struct ZZinlineOuPoste_ts : INotifyCompletion
        {
            public bool IsCompleted => false;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ZZinlineOuPoste_ts GetAwaiter()
            {
                return this;
            }

            /// <summary>
            /// Appelé une fois terminé.
            /// Ne sert qu'à renvoyer une éventuelle exception.
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void GetResult() { }

            /// <summary>
            /// OnCompleted porte mal son nom (décidément!): il est appelé en synchrone au moment du await, juste après la partie synchrone de la tâche awaitée.
            /// Son paramètre est la continuation qui suit le await.
            ///
            /// La continuation est en pratique un appel à GetResult() pour les exceptions, suivie du code de continuation.
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void OnCompleted(Action continuation)
            {
                if (Thread.CurrentThread.IsThreadPoolThread)
                {
                    // Inlining.
                    continuation();
                }
                else
                {
                    ThreadPool.QueueUserWorkItem((Object ù) =>
                    {
                        continuation();
                    }, false);
                }
            }
        }

        public struct ZZinliningSiThreadDifferent_ts : INotifyCompletion
        {
            public bool IsCompleted => false;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ZZinliningSiThreadDifferent_ts GetAwaiter()
            {
                return this;
            }

            /// <summary>
            /// Appelé une fois terminé.
            /// Ne sert qu'à renvoyer une éventuelle exception.
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void GetResult() { }

            /// <summary>
            /// OnCompleted porte mal son nom (décidément!): il est appelé en synchrone au moment du await, juste après la partie synchrone de la tâche awaitée.
            /// Son paramètre est la continuation qui suit le await.
            ///
            /// La continuation est en pratique un appel à GetResult() pour les exceptions, suivie du code de continuation.
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void OnCompleted(Action continuation)
            {
                if (Thread.CurrentThread.IsThreadPoolThread)
                {
                    if (Thread.CurrentThread.ManagedThreadId != _ThreadIdOrigine_s32)
                    {
                        // Inlining.
                        continuation();
                        return;
                    }
                }

                // Post.
                ThreadPool.QueueUserWorkItem((Object ù) =>
                {
                    continuation();
                }, false);
            }

            /// <summary>
            /// Thread d'origine de la fonction en cours lors de l'appel à cet awaiter.
            /// </summary>
            private readonly Int32 _ThreadIdOrigine_s32;

            /// <summary>
            /// On passe le thread d'origine de la fonction en cours lors de l'appel à cet awaiter.
            /// </summary>
            public ZZinliningSiThreadDifferent_ts(Int32 ùThreadIdOrigine_s32)
            {
                _ThreadIdOrigine_s32 = ùThreadIdOrigine_s32;
            }
        }

        public static ZZinliningSiThreadDifferent_ts InlineOuPoste(Int32 ùThreadOrigine_s32)
        {
            return new ZZinliningSiThreadDifferent_ts(ùThreadOrigine_s32);
        }

        public readonly static ZZinlineOuPoste_ts InlineOrPost;

        public struct ZZpost_ts : INotifyCompletion
        {
            /// <summary>
            /// Bien qu'il n'y aie rien à attendre, si on met isCompleted à true, alors il y a toujours inlining (OnCompleted n'est jamais appelé)!
            /// </summary>
            public bool IsCompleted => false;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ZZpost_ts GetAwaiter()
            {
                return this;
            }

            /// <summary>
            /// Appelé une fois terminé.
            /// Ne sert qu'à renvoyer une éventuelle exception.
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void GetResult() { }

            /// <summary>
            /// OnCompleted porte mal son nom (décidément!): il est appelé en synchrone au moment du await, juste après la partie synchrone de la tâche awaitée.
            /// Son paramètre est la continuation qui suit le await.
            ///
            /// La continuation est en pratique un appel à GetResult() pour les exceptions, suivie du code de continuation.
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void OnCompleted(Action continuation)
            {
                System.Threading.ThreadPool.QueueUserWorkItem((Object ù) =>
                {
                    continuation();
                }, false);
            }
        }

        public readonly static ZZpost_ts Post;
    }
}