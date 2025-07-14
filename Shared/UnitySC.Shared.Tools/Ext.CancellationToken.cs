using System.Threading;
using System.Threading.Tasks;

namespace UnitySC.Shared.Tools
{
    public static class CancellationTokenEx
    {
        /// <summary>
        /// Permet d'attendre qu'un CancellationToken soit signalé sans gaspiller de thread. :)
        /// Fast path ok (si le CancellationToken est déjà signalé).
        /// </summary>
        public static Task cAsTask_t(this CancellationToken ùThis)
        {
            return ùThis.WaitHandle.cAsTask_ts();
        }
    }
}