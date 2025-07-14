using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace UnitySC.Shared.Tools
{
    public static class SynchronizationContextExt
    {
        public struct Awaiter : INotifyCompletion
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Awaiter(SynchronizationContext context)
            {
                _context = context;
            }

            private readonly SynchronizationContext _context;

            public bool IsCompleted => ReferenceEquals(SynchronizationContext.Current, _context);

            /// <summary>
            /// Appelé une fois terminé.
            /// Ne sert qu'à renvoyer une éventuelle exception.
            /// </summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void GetResult()
            {
                // "When used by the compiler, the task will already be complete" https://referencesource.microsoft.com/#mscorlib/system/runtime/compilerservices/TaskAwaiter.cs,6f62d20209e01442
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
                else
                {
                    _context.Post((object state) =>
                    {
                        continuation();
                    }, false);
                }
            }
        }

        /// <summary>
        /// Permet un await SynchronizationContext, pour y revenir... :)
        /// </summary>
        public static Awaiter GetAwaiter(this SynchronizationContext ùThis)
        {
            return new Awaiter(ùThis);
        }
    }
}