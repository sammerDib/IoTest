using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Client.Proxy.Light;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Client.Proxy.Test
{
    [TestClass]
    public class UnitTestLight
    {

        [TestMethod]
        public void GetLightIntensityForHalogenUoh()
        {
            var lightSupervisor = ClassLocator.Default.GetInstance<LightsSupervisor>();
            Assert.AreEqual(0.0, lightSupervisor.GetLightIntensity("Halogen_UOH")?.Result);
        }

        [TestMethod]
        public void GetLightIntensityForLightNotExists()
        {
            var lightSupervisor = ClassLocator.Default.GetInstance<LightsSupervisor>();
            Assert.AreEqual(1, lightSupervisor.GetLightIntensity("Light Not Exists").Messages.Count);
        }


        [TestMethod]
        public void SetLightIntensityToWhiteLed()
        {
            var lightSupervisor = ClassLocator.Default.GetInstance<LightsSupervisor>();
            Assert.AreEqual(0, lightSupervisor.SetLightIntensity("VIS_WHITE_LED", 50.0).Messages.Count);
            Assert.AreEqual(50.0, lightSupervisor.GetLightIntensity("VIS_WHITE_LED")?.Result);
        }

        [TestMethod]
        public void SetLightIntensityToLightNotExists()
        {
            var lightSupervisor = ClassLocator.Default.GetInstance<LightsSupervisor>();
            Assert.AreEqual(1, lightSupervisor.SetLightIntensity("Light Not Exists", 50.0).Messages.Count);
        }

    }
}
