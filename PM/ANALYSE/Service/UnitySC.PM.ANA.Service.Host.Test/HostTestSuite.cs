using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitySC.PM.ANA.Service.Host.Test
{
    [TestClass]
    public class HostTestSuite
    {
        [TestMethod]
        public void Expect_Bootstrapper_to_register_all_instances()
        {
            try
            {
                Bootstrapper.Register("-c 4MET2229 -sh -sf -rf".Split(' '));
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
    }
}
