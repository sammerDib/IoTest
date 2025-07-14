using System;
using System.Threading;

using UnitySC.PM.Shared.Flow.Implementation;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow.Execution
{
    public class AutofocusFlowTestWithConfig : FlowComponent<AutofocusInputTest, AutofocusResultTest, AutofocusFlowTestConfig>
    {
        public AutofocusFlowTestWithConfig(AutofocusInputTest input) : base(input, "Autofocus")
        {
        }

        protected override void Process()
        {
            Logger.Debug($"Start AutofocusFlowTestWithConfig");

            CheckCancellation();
            for (int i = 1; i <= 100; i++)
            {
                Thread.Sleep(10);
                CheckCancellation();
            }
            if (Input.Error && Configuration.TestError)
                throw new Exception("Error test");

            Logger.Debug("Autofocus test finished");
        }
    }
}
