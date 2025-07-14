using System.Threading;
using System.Threading.Tasks;

namespace UnitySC.Shared.Tools
{
    public static class WaitHandleEx
    {
        /// <summary>
        /// Permet d'attendre qu'un WaitHandle soit signalé sans gaspiller de thread. :)
        /// Fast path ok (si le waitHandle est déjà signalé).
        /// Edit: apparemment, selon http://www.hanselman.com/blog/ComparingTwoTechniquesInNETAsynchronousCoordinationPrimitives.aspx, ça n'est pas vrai,
        ///  et un thread de la ThreadPool est bloqué pour chaque tranche de 1 à 63 WaitHandles... :\
        /// </summary>
        public static Task cAsTask_ts(this WaitHandle this_wh)
        {
            if (this_wh.WaitOne(0))
            {
                // Fast path.
                return Task.FromResult(true);
            }

            // A l'inverse de http://blog.nerdbank.net/2011/07/c-await-for-waithandle.html, on laisse le finaliseur de RegisteredWaitHandle libérer la mémoire.
            var tcs = new TaskCompletionSource<bool>();
            ThreadPool.RegisterWaitForSingleObject(
                this_wh,
                (state, timedOut) =>
                {
                    tcs.SetResult(true);
                },
                state: true,
                millisecondsTimeOutInterval: -1,
                executeOnlyOnce: true);

            return tcs.Task;
        }
    }
}