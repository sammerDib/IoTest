using System;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SimpleInjector;

using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Service.Host.Test
{
    [TestClass]
    public class HostTestSuite
    {
        [TestMethod]
        public void Expect_HostEmeraServer_ToStartAndStop_WithDummyConfigurationn()
        {
            ClassLocator.ExternalInit(new Container(), true);

            Action action = () => Bootstrapper.Register("-c ALPHA -sh -sf -rf".Split(' '));
            action.Should().NotThrow();

            var emeServer = new EmeServer(ClassLocator.Default.GetInstance<ILogger>());
            Action startingServer = () => emeServer.Start();
            startingServer.Should().NotThrow();

            Action stoppingServer = () => emeServer.Stop();
            stoppingServer.Should().NotThrow();
        }
    }
}
