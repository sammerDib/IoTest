using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SimpleInjector;

using UnitySC.PM.Shared.UI.Main;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Client.Test
{
    [TestClass]
    public class MainViewModelTests
    {
        [TestMethod]
        public void EmeraMainMenu_IsCreated()
        {
            // Given
            ClassLocator.ExternalInit(new Container(), true);
            Bootstrapper.Register("-c ALPHA".Split(' '));

            // When 
            var pmViewModel = ClassLocator.Default.GetInstance<PMViewModel>();

            // Then
            pmViewModel.ActorType.Should().Be(ActorType.EMERA);
            pmViewModel.MainMenuViewModel.Should().NotBeNull();
        }
    }
}
