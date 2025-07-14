using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitySC.PM.AGS.Service.Host.Test
{

    [TestClass]
    public class ArgumentsParsingTest
    {
        [TestMethod]
        public void Expect_Bootstrapper_to_register_all_instances()
        {
            try
            {
                Bootstrapper.Register("-c Default -sh -sf -rf".Split(' '));
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
    }
}
