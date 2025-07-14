using System;
using System.IO;
using System.Linq;

using CommunityToolkit.Mvvm.Messaging;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.EME.Client.Recipe.ViewModel;
using UnitySC.PM.EME.Client.Recipe.ViewModel.Navigation;
using UnitySC.PM.EME.Service.Interface.Recipe;
using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Data;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Test.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.EME.Client.Recipe.Test
{
    [TestClass]
    public class RecipeEditionVMTests
    {
        private Mock<ILogger<RecipeEditionVM>> _loggerMock;
        private Mock<IDialogOwnerService> _dialogServiceMock;
        private Mock<IUserSupervisor> _userSupervisorMock;
        private Mock<INavigationManagerForRecipeEdition> _navigationManager;
        private WeakReferenceMessenger _messengerMock;

        [TestInitialize]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<RecipeEditionVM>>();
            _dialogServiceMock = new Mock<IDialogOwnerService>();
            _userSupervisorMock = new Mock<IUserSupervisor>();
            _navigationManager = new Mock<INavigationManagerForRecipeEdition>();
            _messengerMock = new WeakReferenceMessenger();
            var testUser = new UnifiedUser { Id = 1, Name = "Test User" };
            _userSupervisorMock.SetupGet(us => us.CurrentUser).Returns(testUser);
        }
        
        [TestMethod]
        public void ExportRecipe_WhenRecipeNotFound()
        {
            // Arrange          
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);

            // Act                              
            var supervisor = new Mock<IEMERecipeService>();
            supervisor.Setup(s => s.GetRecipeFromKey(It.IsAny<Guid>()))
                             .Returns((Guid key) => new Response<EMERecipe> { Result = null });

            var recipeEditionViewModel = new RecipeEditionVM(supervisor.Object,null, null, null, null, null, null, null, _dialogServiceMock.Object, null, _loggerMock.Object, _messengerMock, _navigationManager.Object);

            recipeEditionViewModel.ExportRecipe(Guid.NewGuid(), tempDirectory);

            Assert.IsTrue(Directory.Exists(tempDirectory));

            var filesAndDirectories = Directory.EnumerateFileSystemEntries(tempDirectory);

            Assert.IsFalse(filesAndDirectories.Any(), "Le répertoire n'est pas vide.");

            Directory.Delete(tempDirectory, true);
        }
        
        [TestMethod]
        public void ExportRecipe_WhenSuccessful()
        {
            // Arrange
            var recipeKey = Guid.NewGuid();
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);

            // Act                                         
            var supervisor = new Mock<IEMERecipeService>();
            supervisor.Setup(s => s.GetRecipeFromKey(It.IsAny<Guid>()))
                             .Returns((Guid key) => new Response<EMERecipe> { Result = new EMERecipe() { Name = "EMERecipe123" } });

            var recipeEditionViewModel = new RecipeEditionVM(supervisor.Object, null, null, null, null, null, null, null, _dialogServiceMock.Object, _userSupervisorMock.Object, _loggerMock.Object, _messengerMock, _navigationManager.Object);


            recipeEditionViewModel.ExportRecipe(recipeKey, tempDirectory);

            //Assert
            Assert.IsTrue(Directory.Exists(tempDirectory));

            var filesAndDirectories = Directory.EnumerateFileSystemEntries(tempDirectory);

            Assert.IsTrue(filesAndDirectories.Any(), "Le répertoire est vide.");

            Directory.Delete(tempDirectory, true);
        }
        
        [TestMethod]
        public void ImportRecipe_WhenSuccessful()
        {
            // Arrange
            var recipeKey = Guid.NewGuid();
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            var supervisor = new Mock<IEMERecipeService>();
            supervisor.Setup(s => s.GetRecipeFromKey(It.IsAny<Guid>()))
                             .Returns((Guid key) => new Response<EMERecipe> { Result = new EMERecipe { Name = "EMERecipe123" } });
            supervisor.Setup(s => s.CreateRecipe(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                         .Returns((string name, int stepId, int userId) => new Response<EMERecipe>
                         {
                             Result = new EMERecipe { Name = name ?? "DefaultRecipe" }
                         });
            var loggerDBMock = new Mock<ILogger<IDbRecipeService>>();
            var dbRecipeService = new FakeDbRecipeService("RecipeService", loggerDBMock.Object, _messengerMock, ClientConfiguration.GetDataAccessAddress());


            var recipeEditionViewModel = new RecipeEditionVM(supervisor.Object, null, null, null, dbRecipeService, null, null, null, _dialogServiceMock.Object, _userSupervisorMock.Object, _loggerMock.Object, _messengerMock, _navigationManager.Object);

            recipeEditionViewModel.ExportRecipe(recipeKey, tempDirectory);

            string recipePath = Path.Combine(tempDirectory, "EMERecipe123.emer");

            var recipe = recipeEditionViewModel.ImportRecipe(1, 1, recipePath);

            Assert.IsNotNull(recipe);
            Directory.Delete(tempDirectory, true);
        }
        
        [TestMethod]
        public void SaveRecipe_SuccessfulSave_ReturnsTrue()
        {
            var supervisor = new Mock<IEMERecipeService>();
            supervisor.Setup(s => s.GetRecipeFromKey(It.IsAny<Guid>()))
                             .Returns((Guid key) => new Response<EMERecipe> { Result = new EMERecipe { Name = "EMERecipe123" } });
            var recipeEditionViewModel = new RecipeEditionVM(supervisor.Object, null, null, null, null, null, null, null, _dialogServiceMock.Object, _userSupervisorMock.Object, _loggerMock.Object, _messengerMock, _navigationManager.Object)
            {
                // Arrange                                  
                EditedRecipe = new EMERecipeVM()
            };

            // Act
            bool result = recipeEditionViewModel.SaveRecipe();

            // Assert
            Assert.IsTrue(result);
            Assert.IsFalse(recipeEditionViewModel.EditedRecipe.IsModified);
        }
    }
}

