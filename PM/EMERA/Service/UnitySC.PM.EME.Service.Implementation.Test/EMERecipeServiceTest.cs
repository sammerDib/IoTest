using System;

using CommunityToolkit.Mvvm.Messaging;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using SimpleInjector;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.EME.Service.Core.Shared;
using UnitySC.PM.EME.Service.Interface.Recipe;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Configuration;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Test.Tools;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.EME.Service.Implementation.Test
{
    [TestClass]
    public class EMERecipeServiceTest
    {
        private EMERecipeService _emeRecipeService;
        private WeakReferenceMessenger _messenger;

        [TestInitialize]
        public void SetupTest()
        {
            ClassLocator.ExternalInit(new Container(), true);
            ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));
            ClassLocator.Default.Register(typeof(ILogger), typeof(SerilogLogger<object>));
            ClassLocator.Default.Register<IMessenger, WeakReferenceMessenger>(true);
            ClassLocator.Default.Register<ServiceInvoker<IDbRecipeService>>(() => new FakeDbRecipeService("RecipeService", ClassLocator.Default.GetInstance<SerilogLogger<IDbRecipeService>>(), ClassLocator.Default.GetInstance<IMessenger>(), ClientConfiguration.GetDataAccessAddress()));

            var pmConfiguration = new PMConfiguration();
            _messenger = new WeakReferenceMessenger();
            var mapper = new Mapper();            
            var logger = new Mock<ILogger>();

            _emeRecipeService = new EMERecipeService(pmConfiguration, mapper, _messenger, logger.Object);
        }

        [TestMethod]
        public void CreateRecipe_WithValidParameters_ReturnsValidResponse()
        {
            // Act
            var response = _emeRecipeService.CreateRecipe();
            // Assert
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Result);
            Assert.IsTrue(typeof(EMERecipe).IsAssignableFrom(response.Result.GetType()));
            Assert.AreEqual(UnitySC.Shared.Data.Enum.ActorType.EMERA, response.Result.ActorType);
            Assert.IsNotNull(response.Result.Key);
            Assert.AreEqual(-1, response.Result.StepId);
            Assert.AreEqual(0, response.Result.UserId);
        }       
    }
}
