using System.Threading.Tasks;

namespace UnitySC.PM.Shared.Flow.Implementation
{
    public interface IFlowTask
    {
        TaskStatus Status { get; }
        
        void Cancel();

        void Start();

        Task ToTask();
    }
}
