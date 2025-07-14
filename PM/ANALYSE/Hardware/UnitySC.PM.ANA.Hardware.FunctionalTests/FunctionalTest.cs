using System;
using System.Threading.Tasks;

using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Hardware.FunctionalTests
{
    public abstract class FunctionalTest : IFunctionalTest
    {
        protected readonly AnaHardwareManager HardwareManager;
        protected readonly ILogger Logger;

        protected FunctionalTest()
        {
            HardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            Logger = ClassLocator.Default.GetInstance<ILogger>();
        }

        public virtual void Run()
        {
            RunAsync().Wait();
        }

        public virtual Task RunAsync()
        {
            throw new NotImplementedException("RunAsync not implemented. If you don't need async code, please consider overrive the Run method.");
        }
    }
}
