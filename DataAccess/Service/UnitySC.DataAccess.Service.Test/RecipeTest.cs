using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.ExternalFile;
using UnitySC.Shared.Tools;

namespace UnitySC.DataAccess.Service.Test
{
    [TestClass]
    public class RecipeTest : BaseTest
    {
        private IDbRecipeService _recipeService => ClassLocator.Default.GetInstance<IDbRecipeService>();

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            ClassLocator.Default.GetInstance<IDbRecipeService>();
        }

        [TestInitialize]
        public void TestInit()
        {
            base.Init();
        }


        [TestMethod]
        public void T000_CheckDatabaseVersion()
        {
            var response = _recipeService.CheckDatabaseVersion();
            bool isDBupptodate = response.GetResultWithTest();
            string sMsg = "";
            foreach (var msg in response.Messages)
                sMsg += $"<{msg.UserContent}> ";
            Assert.IsTrue(isDBupptodate, $"Database version is not up to date - {sMsg}");
        }

        [TestMethod]
        public void T001_GetDataflows()
        {
            var dataflowInfo = _recipeService.GetDataflowInfos(StepId, ToolKey).GetResultWithTest().First();
            Assert.IsTrue(dataflowInfo.Name == DataflowName);
            var dataflow = _recipeService.GetDataflow(dataflowInfo.Id).GetResultWithTest();
            var demeterRecipe = dataflow.ChildRecipes.First();
            Assert.IsTrue(demeterRecipe.Component.Name == DemeterRecipeName);
            Assert.IsTrue(demeterRecipe.Component.ActorType == ActorType.DEMETER);
            var adcRecipe = demeterRecipe.Component.ChildRecipes.First();
            Assert.IsTrue(adcRecipe.Component.Name == ADCRecipeName);
            Assert.IsTrue(adcRecipe.Component.ActorType == ActorType.ADC);
        }

        [TestMethod]
        public void T002_UpdateDataflow()
        {
            var dataflowV1 = _recipeService.GetLastDataflow(DataflowName, StepId).GetResultWithTest();
            Assert.IsTrue(dataflowV1.Version == 1);
            dataflowV1.Comment = "Version 2";
            _recipeService.SetDataflow(dataflowV1, UserId, StepId, ToolKey).GetResultWithTest();

            // By name
            var dataflowV2 = _recipeService.GetLastDataflow(DataflowName, StepId).GetResultWithTest();
            Assert.AreEqual(2, dataflowV2.Version);
            Assert.AreEqual("Version 2", dataflowV2.Comment);

            // By Guid
            dataflowV2 = _recipeService.GetLastDataflow(dataflowV1.Key).GetResultWithTest();
            Assert.AreEqual(2, dataflowV2.Version);
            Assert.AreEqual("Version 2", dataflowV2.Comment);

            // Without new version
            dataflowV2.Comment = "Version 2 with update";
            _recipeService.SetDataflow(dataflowV2, UserId, StepId, ToolKey, false).GetResultWithTest();

            // By name
            var updatedDataflowV2 = _recipeService.GetLastDataflow(DataflowName, StepId).GetResultWithTest();
            Assert.AreEqual(2, dataflowV2.Version);
            Assert.AreEqual("Version 2 with update", updatedDataflowV2.Comment);

            // By Guid
            updatedDataflowV2 = _recipeService.GetLastDataflow(dataflowV1.Key).GetResultWithTest();
            Assert.AreEqual(2, dataflowV2.Version);
            Assert.AreEqual("Version 2 with update", updatedDataflowV2.Comment);
        }

        [TestMethod]
        public void T003_UpdateSharedState()
        {
            var recipe = _recipeService.GetRecipe(ActorType.DEMETER, StepId, DemeterRecipeName).GetResultWithTest();
            Assert.IsFalse(recipe.IsShared);
            Assert.IsFalse(recipe.IsTemplate);
            Assert.IsFalse(recipe.IsValidated);
            _recipeService.ChangeRecipeSharedState(recipe.KeyForAllVersion, UserId, true).GetResultWithTest();
            var sharedRecipe = _recipeService.GetRecipe(ActorType.DEMETER, StepId, DemeterRecipeName).GetResultWithTest();
            Assert.IsTrue(sharedRecipe.IsShared);
            Assert.IsFalse(recipe.IsTemplate);
            Assert.IsFalse(recipe.IsValidated);
        }

        [TestMethod]
        public void T004_UpdateRecipe()
        {
            var recipeV1 = _recipeService.GetRecipe(ActorType.DEMETER, StepId, DemeterRecipeName).GetResultWithTest();
            Assert.AreEqual(1, recipeV1.Version);
            recipeV1.Comment = "Version 2";
            Assert.IsTrue(recipeV1.Outputs.Any()); ;
            _recipeService.SetRecipe(recipeV1, true);

            // By Name
            var recipeV2 = _recipeService.GetLastRecipe(ActorType.DEMETER, StepId, DemeterRecipeName).GetResultWithTest();
            Assert.AreEqual(2, recipeV2.Version);
            Assert.IsTrue(recipeV2.Outputs.Any()); ;

            // By Key
            recipeV2 = _recipeService.GetLastRecipe(recipeV1.KeyForAllVersion).GetResultWithTest();
            Assert.AreEqual(2, recipeV2.Version);
            Assert.IsTrue(recipeV2.Outputs.Any());

            // Without new version
            recipeV2.Comment = "Version 2 with update";
            _recipeService.SetRecipe(recipeV2, false);

            // By Name
            var updatedRecipeV2 = _recipeService.GetLastRecipe(ActorType.DEMETER, StepId, DemeterRecipeName).GetResultWithTest();
            Assert.AreEqual(2, updatedRecipeV2.Version);
            Assert.AreEqual("Version 2 with update", updatedRecipeV2.Comment);
            Assert.IsTrue(updatedRecipeV2.Outputs.Any());

            updatedRecipeV2 = _recipeService.GetLastRecipe(recipeV1.KeyForAllVersion).GetResultWithTest();
            Assert.AreEqual(2, updatedRecipeV2.Version);
            Assert.AreEqual("Version 2 with update", updatedRecipeV2.Comment);
            Assert.IsTrue(updatedRecipeV2.Outputs.Any());
        }

        [TestMethod]
        public void T005_CompatibleRecipes()
        {
            var recipeDemeter = _recipeService.GetRecipe(ActorType.DEMETER, StepId, DemeterRecipeName).GetResultWithTest();
            var recipeADC = _recipeService.GetRecipe(ActorType.ADC, StepId, ADCRecipeName).GetResultWithTest();
            // compatible aprés le Demeter => la recette ADC
            Assert.AreEqual(1, _recipeService.GetCompatibleRecipes(recipeDemeter.KeyForAllVersion, StepId, ToolKey).GetResultWithTest().Count());
            // compatible aprés l'adc => aucune autre recette dispo
            Assert.AreEqual(0, _recipeService.GetCompatibleRecipes(recipeADC.KeyForAllVersion, StepId, ToolKey).GetResultWithTest().Count());
            // compatible ua debut les 2 recette Demeter celle binded et celle qui nel'ai pas encore
            Assert.AreEqual(2, _recipeService.GetCompatibleRecipes(null, StepId, ToolKey).GetResultWithTest().Count());
        }

        [TestMethod]
        public void T006_CloneRecipe()
        {
            var recipeToCloneV1 = _recipeService.GetRecipe(ActorType.DEMETER, StepId, DemeterRecipeName).GetResultWithTest();
            int lastversion = recipeToCloneV1.Version;
            int clonedId = _recipeService.CloneRecipe(recipeToCloneV1.KeyForAllVersion, "Clone Recipe", UserId).GetResultWithTest();
            var clonedRecipe = _recipeService.GetRecipe(clonedId, false).GetResultWithTest();
            Assert.AreEqual("Clone Recipe", clonedRecipe.Name);
            Assert.AreEqual(1, clonedRecipe.Version);

            recipeToCloneV1 = _recipeService.GetRecipe(recipeToCloneV1.Id).GetResultWithTest();
            Assert.AreEqual(DemeterRecipeName, recipeToCloneV1.Name);
            Assert.AreEqual(lastversion, recipeToCloneV1.Version);
        }

        [TestMethod]
        public void T007_TCPMRecipe()
        {
            var Lastrecipe = _recipeService.GetLastRecipe(ActorType.DEMETER, StepId, DemeterRecipeName).GetResultWithTest();

            var tcRecipes = _recipeService.GetTCPMRecipeList(ActorType.DEMETER, ToolKey).GetResultWithTest();
            Assert.IsTrue(tcRecipes.Any(), $"No TCPM recipe found for toolKey = {ToolKey}");
            var recipe = _recipeService.GetPMRecipeWithTC(tcRecipes.First().Name).GetResultWithTest();
            Assert.IsNotNull(recipe, $"No Recipes with TC have been found ({tcRecipes.First().Name})");
            Assert.AreEqual(ActorType.DEMETER, recipe.Type, "Not a DEMETER recipe");
            Assert.AreEqual(Lastrecipe.Id, recipe.Id, $"DEMETER recipe id not matched");
        }

        [TestMethod]
        public void T008_ArchivedRecipe()
        {
            var recipe = _recipeService.GetRecipeList(ActorType.DEMETER, StepId, ChamberKey, ToolKey).GetResultWithTest().FirstOrDefault(x => x.Name == DemeterRecipeName);
            Assert.IsNotNull(recipe);
            var serviceresponse = _recipeService.ArchiveAllVersionOfRecipe(recipe.Key, UserId);
            Assert.IsNotNull(serviceresponse.Exception, "Exception should be raised since we cannot archives a recipe binded to a dataflow");
            Assert.AreEqual("Can't archive a recipe used in a dataflow", serviceresponse.Exception.Message);
            Assert.IsTrue(_recipeService.GetRecipeList(ActorType.DEMETER, StepId, ChamberKey, ToolKey).GetResultWithTest().Any(x => x.Name == DemeterRecipeName));

            var recipeNotBinded = _recipeService.GetRecipeList(ActorType.DEMETER, StepId, ChamberKey, ToolKey).GetResultWithTest().FirstOrDefault(x => x.Name == DemeterRecipeNameNotBindToDataflow);
            Assert.IsNotNull(recipeNotBinded);
            Assert.IsTrue(_recipeService.GetRecipeList(ActorType.DEMETER, StepId, ChamberKey, ToolKey).GetResultWithTest().Any(x => x.Name == DemeterRecipeNameNotBindToDataflow));
            var serviceresponseNotBinded = _recipeService.ArchiveAllVersionOfRecipe(recipeNotBinded.Key, UserId);
            Assert.IsNull(serviceresponseNotBinded.Exception, "No Exception should be raised");
            Assert.IsFalse(_recipeService.GetRecipeList(ActorType.DEMETER, StepId, ChamberKey, ToolKey).GetResultWithTest().Any(x => x.Name == DemeterRecipeNameNotBindToDataflow));

            // restore
            serviceresponseNotBinded = _recipeService.RestoreAllVersionOfRecipe(recipeNotBinded.Key, UserId);
            Assert.IsNull(serviceresponseNotBinded.Exception, "No Exception should be raised");
            Assert.IsTrue(_recipeService.GetRecipeList(ActorType.DEMETER, StepId, ChamberKey, ToolKey).GetResultWithTest().Any(x => x.Name == DemeterRecipeNameNotBindToDataflow));

            // restore a non archived recipe
            serviceresponseNotBinded = _recipeService.RestoreAllVersionOfRecipe(recipeNotBinded.Key, UserId);
            Assert.IsNull(serviceresponseNotBinded.Exception, "No Exception should be raised");
            Assert.IsTrue(_recipeService.GetRecipeList(ActorType.DEMETER, StepId, ChamberKey, ToolKey).GetResultWithTest().Any(x => x.Name == DemeterRecipeNameNotBindToDataflow));

        }

        [TestMethod]
        public void T009_ArchivedDataflow()
        {
            var dataflow = _recipeService.GetLastDataflow(DataflowName, StepId).GetResultWithTest();
            Assert.IsNotNull(dataflow, $"Dataflow has not been found WF={DataflowName} StepId={StepId}");

            _recipeService.ArchiveAllVersionOfDataflow(dataflow.Key, UserId);
            var archiveddataflow = _recipeService.GetLastDataflow(DataflowName, StepId).GetResultWithTest();
            Assert.IsNull(archiveddataflow, $"archived dataflow should not been listed WF={DataflowName} StepId={StepId}");

            _recipeService.RestoreAllVersionOfDataflow(dataflow.Key, UserId);
            var restorededdataflow = _recipeService.GetLastDataflow(DataflowName, StepId).GetResultWithTest();
            Assert.IsNotNull(restorededdataflow, $"restored Dataflow has not been found WF={DataflowName} StepId={StepId}");
        }

        [TestMethod]
        public void T010_RecipeFile()
        {
            var externalImage = new ExternalImage
            {
                FileNameKey = "TestExternalFile.png"
            };
            externalImage.LoadFromFile("TestExternalFile.png");
            int dataLength = externalImage.Data.Length;
            int externalFileId = _recipeService.SetExternalFile(externalImage, RecipeDemeterId, UserId).GetResultWithTest();
            Assert.IsTrue(externalFileId != 0);
            var key = _recipeService.GetRecipeInfo(RecipeDemeterId).GetResultWithTest().Key;
            var externalImageRes = _recipeService.GetExternalFile(externalImage.FileNameKey, key).GetResultWithTest();
            Assert.IsTrue(dataLength == externalImageRes.Data.Length);
            var externalImages = _recipeService.GetExternalFiles(key).GetResultWithTest();
            Assert.IsTrue(dataLength == externalImages.First().Data.Length);

            // New version of file with same key
            var externalImage2 = new ExternalImage
            {
                FileNameKey = "TestExternalFile.png"
            };
            externalImage2.LoadFromFile("TestExternalFileV2.png");
            int dataLength2 = externalImage2.Data.Length;
            int externalFileId2 = _recipeService.SetExternalFile(externalImage2, RecipeDemeterId, UserId).GetResultWithTest();
            Assert.IsTrue(externalFileId2 != 0);
            var externalImageRes2 = _recipeService.GetExternalFile(externalImage2.FileNameKey, key).GetResultWithTest();
            Assert.IsTrue(dataLength2 == externalImageRes2.Data.Length);
            var externalImages2 = _recipeService.GetExternalFiles(key).GetResultWithTest();
            Assert.IsTrue(dataLength2 == externalImages2.First().Data.Length);
        }


        [TestMethod]
        public void T011_TCDataflowRecipe()
        {
            var dataflowInfo = _recipeService.GetDataflowInfos(StepId, ToolKey).GetResultWithTest().First();
            var tcRecipes = _recipeService.GetTCPMRecipeList(ActorType.DEMETER, ToolKey).GetResultWithTest();
            Assert.IsTrue(tcRecipes.Any(), $"No TCPM recipe found for toolid = {ToolKey}");
            var wfComponentsWithTC = _recipeService.GetDataflowWithTC(tcRecipes.First().Name).GetResultWithTest();
            var wf = _recipeService.GetDataflow(dataflowInfo.Id).GetResultWithTest();
            Assert.AreEqual(wf.Key, wfComponentsWithTC.Key, "Bad Dataflow key");

        }
    }
}
