using System;
using System.Threading;

using UnitySC.PM.Shared.Flow.Implementation;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow.Execution
{
    public class AutofocusFlowTest : FlowComponent<AutofocusInputTest, AutofocusResultTest>
    {
        public AutofocusFlowTest(AutofocusInputTest input) : base(input, "Autofocus")
        {
        }

        protected override void Process()
        {
            Logger.Debug($"Start AutofocusFlowTest");

            for (int i = 1; i <= 500; i++)
            {
                Thread.Sleep(10);
                CheckCancellation();
            }
            if (Input.Error)
                throw new Exception("Error test");

            Logger.Debug("Dummy Autofocus finished");
        }
    }
}
