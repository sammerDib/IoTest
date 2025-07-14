using System;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitySC.PM.DMT.Service.Host.Test
{
    [TestClass]
    public class HostTestSuite
    {
        [TestMethod]
        public void Expect_Bootstrapper_to_register_all_instances()
        {
            Action action = () => Bootstrapper.Register("-c 4VIS2562 -sh -sf -rf".Split(' '));
            action.Should().NotThrow("Exception thrown by Bootstrapper.Register");
        }
    }
}
