using System;
using System.IO;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.DataAccess.ResultScanner.Interface;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.ExternalFile;
using UnitySC.Shared.Tools;

namespace UnitySC.DataAccess.Service.Test
{
    [TestClass]
    public class RegisterTest : BaseTest
    {
        private IRegisterResultService _registerService => ClassLocator.Default.GetInstance<IRegisterResultService>();
        private IDbRecipeService _recipeService => ClassLocator.Default.GetInstance<IDbRecipeService>();

        internal IResultService ResultService => ClassLocator.Default.GetInstance<IResultService>();


        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            ClassLocator.Default.GetInstance<IDbRecipeService>();
            ClassLocator.Default.GetInstance<IRegisterResultService>();
            ClassLocator.Default.GetInstance<IResultService>();
        }

        [TestInitialize]
        public void TestInit()
        {
            base.Init();
            ClassLocator.Default.GetInstance<IResultScanner>().Start();
        }

        private RecipeInfo CreateRecipeInfo(string recipeName, ActorType actorTyp,  Guid? guid = null)
        {
            RecipeInfo recipeInfo;
            if (guid != null)
            {
                // retreive recipe
               var recipe = _recipeService.GetLastRecipe(guid.Value).GetResultWithTest();
                recipeInfo = new RecipeInfo() { Name = recipe.Name, Version = recipe.Version, Comment = recipe.Comment, StepId = recipe.StepId, IsShared = recipe.IsShared, IsTemplate = recipe.IsTemplate, ActorType = (ActorType)recipe.ActorType, Key = recipe.KeyForAllVersion };

            }
            else
            {
                // Add Brand New recipe 
                var rcp = new Dto.Recipe();
                rcp.Type = actorTyp;
                rcp.CreatorChamberId = ChamberId;
                rcp.Name = recipeName;
                rcp.StepId = StepId;
                rcp.CreatorUserId = UserId;
                rcp.AddOutput(ResultType.ANALYSE_TSV);
                var RecipeDBId = _recipeService.SetRecipe(rcp, false).GetResultWithTest();
                recipeInfo = _recipeService.GetRecipeInfo(RecipeDBId).GetResultWithTest();
            }
            return recipeInfo;
        }

        private RemoteProductionInfo CreateAutomationInfo(string jobName, string lotName, int slotID, string waferName)
        {
            var material = new Material()
            {
                ProcessJobID = jobName,
                LotID = lotName,
                SlotID = slotID,
                SubstrateID = waferName,
                JobStartTime = DateTime.Now,
            };
            var autominfo = new RemoteProductionInfo()
            {
                ProcessedMaterial = material,
                DFRecipeName = "DFRecipeName",
                ModuleRecipeName = "ModuleRecipeName",
                ModuleStartRecipeTime = DateTime.Now,
            };
            return autominfo;
        }


        [TestMethod]
        public void T000_NominalCase_RegisterResultAndUpdate()
        {
            var recipeInfo = CreateRecipeInfo("Test000", ActorType.ANALYSE);
            var autominfo =  CreateAutomationInfo("JobProcessId","LotID", 1, "MyWaferID");
           
            var outRegiterResult = _registerService.PreRegisterResult(ToolKey, ChamberKey, recipeInfo, autominfo, ResultType.ANALYSE_TSV);
            Assert.IsNotNull(outRegiterResult);
            Assert.IsNotNull(outRegiterResult.Result);

            bool bSuccess = _registerService.UpdateResultState(outRegiterResult.Result.InternalDBResItemId, Dto.ModelDto.Enum.ResultState.Ok).Result;
            Assert.IsTrue(bSuccess);
        }

        [TestMethod]
        public void T001_MultiSlotWafer_RegisterResultAndUpdate()
        {
            var recipeInfo = CreateRecipeInfo("Test001", ActorType.ANALYSE);
            var autominfo = CreateAutomationInfo("JobProcessId2", "LotID2", 1, "MyWaferID1");

            var outRegiterResult = _registerService.PreRegisterResult(ToolKey, ChamberKey, recipeInfo, autominfo, ResultType.ANALYSE_TSV);
            Assert.IsNotNull(outRegiterResult);
            Assert.IsNotNull(outRegiterResult.Result);

            bool bSuccess = _registerService.UpdateResultState(outRegiterResult.Result.InternalDBResItemId, Dto.ModelDto.Enum.ResultState.Ok).Result;
            Assert.IsTrue(bSuccess);

            autominfo.ProcessedMaterial.SlotID = 2;
            autominfo.ProcessedMaterial.SubstrateID = "MyWaferID2";
            var outRegiterResult2 = _registerService.PreRegisterResult(ToolKey, ChamberKey, recipeInfo, autominfo, ResultType.ANALYSE_TSV);
            Assert.IsNotNull(outRegiterResult2);
            Assert.IsNotNull(outRegiterResult2.Result);

            bSuccess = _registerService.UpdateResultState(outRegiterResult2.Result.InternalDBResItemId, Dto.ModelDto.Enum.ResultState.Ok).Result;
            Assert.IsTrue(bSuccess);

            // Modified date
            autominfo.ProcessedMaterial.SlotID = 3;
            autominfo.ProcessedMaterial.SubstrateID = "MyWaferID2";
            var dateNowplusMin = autominfo.ProcessedMaterial.JobStartTime + new TimeSpan(0,10,0);
            autominfo.ProcessedMaterial.JobStartTime = dateNowplusMin;
            autominfo.ModuleStartRecipeTime = dateNowplusMin;
            var outRegiterResult3 = _registerService.PreRegisterResult(ToolKey, ChamberKey, recipeInfo, autominfo, ResultType.ANALYSE_TSV);
            Assert.IsNotNull(outRegiterResult3);
            Assert.IsNotNull(outRegiterResult3.Result);

            bSuccess = _registerService.UpdateResultState(outRegiterResult3.Result.InternalDBResItemId, Dto.ModelDto.Enum.ResultState.Ok).Result;
            Assert.IsTrue(bSuccess);
        }

        [TestMethod]
        public void T002_MultiSlotWaferMultiResType_RegisterResultAndUpdate()
        {
            var recipeInfo = CreateRecipeInfo("Test002", ActorType.ANALYSE);
            var autominfo = CreateAutomationInfo("JobProcessId3", "LotID3", 1, "MyWaferID1");

            var outRegiterResult_TSV = _registerService.PreRegisterResult(ToolKey, ChamberKey, recipeInfo, autominfo, ResultType.ANALYSE_TSV);
            Assert.IsNotNull(outRegiterResult_TSV);
            Assert.IsNotNull(outRegiterResult_TSV.Result);

            var outRegiterResult_NTP = _registerService.PreRegisterResult(ToolKey, ChamberKey, recipeInfo, autominfo, ResultType.ANALYSE_NanoTopo);
            Assert.IsNotNull(outRegiterResult_NTP);
            Assert.IsNotNull(outRegiterResult_NTP.Result);


            bool bSuccess = _registerService.UpdateResultState(outRegiterResult_TSV.Result.InternalDBResItemId, Dto.ModelDto.Enum.ResultState.Ok).Result;
            Assert.IsTrue(bSuccess);
            bSuccess = _registerService.UpdateResultState(outRegiterResult_NTP.Result.InternalDBResItemId, Dto.ModelDto.Enum.ResultState.Ok).Result;
            Assert.IsTrue(bSuccess);

            // Modified date
            autominfo.ProcessedMaterial.SlotID = 4;
            autominfo.ProcessedMaterial.SubstrateID = "MyWaferID4";
            var dateNowplusMin = autominfo.ProcessedMaterial.JobStartTime + new TimeSpan(0, 10, 0);
            autominfo.ProcessedMaterial.JobStartTime = dateNowplusMin;
            autominfo.ModuleStartRecipeTime = dateNowplusMin;

            var outRegiterResult4_TSV = _registerService.PreRegisterResult(ToolKey, ChamberKey, recipeInfo, autominfo, ResultType.ANALYSE_TSV);
            Assert.IsNotNull(outRegiterResult4_TSV);
            Assert.IsNotNull(outRegiterResult4_TSV.Result);

            var outRegiterResult4_NTP= _registerService.PreRegisterResult(ToolKey, ChamberKey, recipeInfo, autominfo, ResultType.ANALYSE_NanoTopo);
            Assert.IsNotNull(outRegiterResult4_NTP);
            Assert.IsNotNull(outRegiterResult4_NTP.Result);

            bSuccess = _registerService.UpdateResultState(outRegiterResult4_TSV.Result.InternalDBResItemId, Dto.ModelDto.Enum.ResultState.Ok).Result;
            Assert.IsTrue(bSuccess);
            bSuccess = _registerService.UpdateResultState(outRegiterResult4_NTP.Result.InternalDBResItemId, Dto.ModelDto.Enum.ResultState.Ok).Result;
            Assert.IsTrue(bSuccess);
        }

        [TestMethod]
        public void T003_SameWaferDifferentJob_RegisterResultAndUpdate()
        {
            var recipeInfo = CreateRecipeInfo("Test003", ActorType.ANALYSE);
            var autominfo = CreateAutomationInfo("JobProcessId_003_A", "LotID_003", 1, "MyWaferID");

            var outRegiterResult = _registerService.PreRegisterResult(ToolKey, ChamberKey, recipeInfo, autominfo, ResultType.ANALYSE_TSV);
            Assert.IsNotNull(outRegiterResult);
            Assert.IsNotNull(outRegiterResult.Result);

            bool bSuccess = _registerService.UpdateResultState(outRegiterResult.Result.InternalDBResItemId, Dto.ModelDto.Enum.ResultState.Ok).Result;
            Assert.IsTrue(bSuccess);

            var autominfo_B = CreateAutomationInfo("JobProcessId_003_B", "LotID_003", 1, "MyWaferID");
            var dateNowplusMin = autominfo.ProcessedMaterial.JobStartTime + new TimeSpan(1, 10, 0);
            autominfo_B.ProcessedMaterial.JobStartTime = dateNowplusMin;
            autominfo_B.ModuleStartRecipeTime = dateNowplusMin;

            var outRegiterResult_B = _registerService.PreRegisterResult(ToolKey, ChamberKey, recipeInfo, autominfo_B, ResultType.ANALYSE_TSV);
            Assert.IsNotNull(outRegiterResult_B);
            Assert.IsNotNull(outRegiterResult_B.Result);

            string filename = outRegiterResult_B.Result.ResultFileName + "." + ResultFormatExtension.GetExt(ResultType.ANALYSE_TSV);
            string spath = Path.Combine(outRegiterResult_B.Result.ResultPathRoot, filename);

            bool bSuccess_B = _registerService.UpdateResultState(outRegiterResult_B.Result.InternalDBResItemId, Dto.ModelDto.Enum.ResultState.Ok).Result;
            Assert.IsTrue(bSuccess_B);

        }

        [TestMethod]
        public void T004_SameWafeSameJob_RUNITERCASE_RegisterResultAndUpdate()
        {
            var recipeInfo = CreateRecipeInfo("Test004", ActorType.ANALYSE);
            var autominfo = CreateAutomationInfo("JobProcessId_004", "LotID_004", 2, "MyWaferID");

            var outRegiterResult = _registerService.PreRegisterResult(ToolKey, ChamberKey, recipeInfo, autominfo, ResultType.ANALYSE_TSV);
            Assert.IsNotNull(outRegiterResult);
            Assert.IsNotNull(outRegiterResult.Result);

            bool bSuccess = _registerService.UpdateResultState(outRegiterResult.Result.InternalDBResItemId, Dto.ModelDto.Enum.ResultState.Ok).Result;
            Assert.IsTrue(bSuccess);

            var dateNowplusMin = autominfo.ProcessedMaterial.JobStartTime + new TimeSpan(1, 10, 0);
            autominfo.ProcessedMaterial.JobStartTime = dateNowplusMin;
            autominfo.ModuleStartRecipeTime = dateNowplusMin;

            var outRegiterResult_B = _registerService.PreRegisterResult(ToolKey, ChamberKey, recipeInfo, autominfo, ResultType.ANALYSE_TSV);
            Assert.IsNotNull(outRegiterResult_B);
            Assert.IsNotNull(outRegiterResult_B.Result);

          
            string filename = outRegiterResult_B.Result.ResultFileName + "." + ResultFormatExtension.GetExt(ResultType.ANALYSE_TSV);
            string spath = Path.Combine(outRegiterResult_B.Result.ResultPathRoot, filename);


            bool bSuccess_B = _registerService.UpdateResultState(outRegiterResult_B.Result.InternalDBResItemId, Dto.ModelDto.Enum.ResultState.Ok).Result;
            Assert.IsTrue(bSuccess_B);

        }
    }
}
