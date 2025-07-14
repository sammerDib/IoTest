using CommunityToolkit.Mvvm.Messaging;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.EME.Client.Proxy.Chiller;
using UnitySC.PM.EME.Service.Interface.Chiller;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.EME.Client.Proxy.Test
{
    [TestClass]
    public class ChillerSupervisorTests
    {
        [TestMethod]
        public void ShouldCallChillerService()
        {
            var messenger = new WeakReferenceMessenger();
            var systemUnderTest = new ChillerSupervisor(new SerilogLogger<IChillerService>(), messenger);
            systemUnderTest.SetConstFanSpeedMode(ConstFanSpeedMode.Enabled);
        }
    }
}
