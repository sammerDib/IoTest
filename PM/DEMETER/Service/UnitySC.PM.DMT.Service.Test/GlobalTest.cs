using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitySC.PM.Shared;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.DMT.Service.Test
{
    [TestClass]
    public class GlobalTest : BaseTest
    {
        [TestMethod]
        public void TestLog()
        {
            try
            {
                ClassLocator.Default.GetInstance<ClassLogWithCaller>().Log();
            }
            catch(Exception ex)
            {
                Assert.Fail("Log with caller fail" + ex.Message);
            }

            try
            {
                ClassLocator.Default.GetInstance<ClassLogWithoutCaller>().Log();
            }
            catch (Exception ex)
            {
                Assert.Fail("Log without caller fail" + ex.Message);
            }        
        }
    }
}
