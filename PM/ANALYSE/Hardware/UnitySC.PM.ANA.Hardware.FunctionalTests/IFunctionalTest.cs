using System.Threading.Tasks;

namespace UnitySC.PM.ANA.Hardware.FunctionalTests
{
    public interface IFunctionalTest
    {
        void Run();

        Task RunAsync();
    }
}
