using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.DataAccess.Dto.ModelDto.Enum;
using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.DataAccess.ResultScanner.Interface;
using UnitySC.DataAccess.Service.Implementation;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Format.Base;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.DataAccess.Service.Test
{
    [TestClass]
    public class ResultServiceTest : BaseTest
    {
        internal IResultService ResultService => ClassLocator.Default.GetInstance<IResultService>();
       
        public const string ToolName2 = "Tool2";
        public const string ChamberName_T2_1 = "LS2-1";
        public const string ChamberName_T2_2 = "PSD2-2";

        public const string ToolName3 = "Tool3";
        public const string ChamberName_T3_1 = "BF3-1";

        public int ToolId_T2 { get; private set; }
        public int ChamberId_T2_1 { get; private set; }
        public int ChamberId_T2_2 { get; private set; }

        public int ToolId_T3 { get; private set; }
        public int ChamberId_T3_1 { get; private set; }

        private static ResultServiceTest s_resultinstance = null;
        private static readonly object s_resultpadlock = new object();

        public static ResultServiceTest ResultServiceTestInstance
        {
            get
            {
                lock (s_resultpadlock)
                {
                    if (s_resultinstance == null)
                    {
                        s_resultinstance = new ResultServiceTest();
                    }
                    return s_resultinstance;
                }
            }
        }

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            ClassLocator.Default.GetInstance<IDbRecipeService>();
            ClassLocator.Default.GetInstance<IRegisterResultService>();
            ClassLocator.Default.GetInstance<IResultService>();

            ResultServiceTestInstance.Init();
            CreateAdditionnalsToolsData();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            ClassLocator.Default.GetInstance<IResultScanner>().Stop();
        }

        [TestInitialize]
        public void TestInit()
        {
            base.Init();

            ToolId_T2 = ResultServiceTestInstance.ToolId_T2;
            ChamberId_T2_1 = ResultServiceTestInstance.ChamberId_T2_1;
            ChamberId_T2_2 = ResultServiceTestInstance.ChamberId_T2_2;
            ToolId_T3 = ResultServiceTestInstance.ToolId_T3;
            ChamberId_T3_1 = ResultServiceTestInstance.ChamberId_T3_1;
        }

        public static void CreateAdditionnalsToolsData()
        {
            var logger = ClassLocator.Default.GetInstance<ILogger<object>>();
            logger.Debug("CreateAdditionnalsToolsData");

            ClassLocator.Default.GetInstance<IResultScanner>().Start();

            var toolservice = ClassLocator.Default.GetInstance<IToolService>();
            // Add tool 2
            ResultServiceTestInstance.ToolId_T2 = toolservice.SetTool(new Dto.Tool
            {
                Name = ToolName2,
                ToolCategory = ToolName2 + "Category",
                ToolKey = 2
            }, null).GetResultWithTest();

            // Add Chamber 1 -  LS tool 2
            ResultServiceTestInstance.ChamberId_T2_1 = toolservice.SetChamber(new Dto.Chamber
            {
                Name = ChamberName_T2_1,
                Actor = ActorType.LIGHTSPEED,
                ToolId = ResultServiceTestInstance.ToolId_T2,
                ChamberKey = 1
            }, ResultServiceTestInstance.UserId).GetResultWithTest();
            // Add Chambers 2 - tool 2
            ResultServiceTestInstance.ChamberId_T2_2 = toolservice.SetChamber(new Dto.Chamber
            {
                Name = ChamberName_T2_2,
                Actor = ActorType.DEMETER,
                ToolId = ResultServiceTestInstance.ToolId_T2,
                ChamberKey = 2
            }, ResultServiceTestInstance.UserId).GetResultWithTest();

            // Add tool 3
            ResultServiceTestInstance.ToolId_T3 = toolservice.SetTool(new Dto.Tool
            {
                Name = ToolName3,
                ToolCategory = ToolName3 + "Category",
                ToolKey = 3
            }, null).GetResultWithTest();
            // Add Chamber 1 -  BF tool 3
            ResultServiceTestInstance.ChamberId_T3_1 = toolservice.SetChamber(new Dto.Chamber
            {
                Name = ChamberName_T3_1,
                Actor = ActorType.BrightField2D,
                ToolId = ResultServiceTestInstance.ToolId_T3,
                ChamberKey = 1
            }, ResultServiceTestInstance.UserId).GetResultWithTest();

            RegisterSomeT2Result();
            RegisterSomeT3Result();
        }

        private static void RegisterSomeT2Result()
        {
            var ts1 = new TimeSpan(0, 5, 30);
            var ts2 = new TimeSpan(0, 0, 05);
            var dtNow = DateTime.Now - ts1;

            //
            // JOB 1 - chamber T2-1
            var indata = new InPreRegister()
            {
                JobName = "TESTU_JOBID_T2-1",
                LotName = "TESTU_LOTNAME_T2-1",
                TCRecipeName = "TESTU_RCP_T2-1",
                DateTimeRun = dtNow - ts1,
                ToolId = ResultServiceTestInstance.ToolId_T2,

                ChamberId = ResultServiceTestInstance.ChamberId_T2_1,
                ProductId = ResultServiceTestInstance.ProductId,
                RecipeId = ResultServiceTestInstance.RecipeADCId,

                WaferBaseName = "TestU_Wafer_T2-1",
                SlotId = 21,
                ResultType = ResultType.ADC_Klarf,

                Idx = 0,
            };

            var RegisterResultService = ClassLocator.Default.GetInstance<IRegisterResultService>();

            var outDtoData = RegisterResultService.PreRegisterResultWithPreRegisterObject(indata);
            long InternalDBResItemId = outDtoData.Result.InternalDBResItemId;
            int RunIter = outDtoData.Result.RunIter;
            string ResultFileName = outDtoData.Result.ResultFileName;
            string ResultPathRoot = outDtoData.Result.ResultPathRoot;

            indata.ParentResultId = outDtoData.Result.InternalDBResId;
            indata.ResultType = ResultType.ADC_ASO;
            indata.LabelName = "ASO";

            var outDtoDataASO = RegisterResultService.PreRegisterResultWithPreRegisterObject(indata);
            long InternalDBResItemId2 = outDtoDataASO.Result.InternalDBResItemId;
            int RunIter2 = outDtoDataASO.Result.RunIter;
            string ResultFileName2 = outDtoDataASO.Result.ResultFileName;
            string ResultPathRoot2 = outDtoDataASO.Result.ResultPathRoot;

            var valuesKlarf = new List<ResultDataStats>
            {
                new ResultDataStats(InternalDBResItemId, (int)ResultValueType.Count, "Defect001_1", 155.0, (int)UnitType.Nb),
                new ResultDataStats(InternalDBResItemId, (int)ResultValueType.Count, "Defect001_2", 5.0, (int)UnitType.Nb),
                new ResultDataStats(InternalDBResItemId, (int)ResultValueType.Count, "Defect001_3", 66.0, (int)UnitType.Nb)
            };
            AddResultValues(valuesKlarf);

            var valuesASO = new List<ResultDataStats>
            {
                new ResultDataStats(InternalDBResItemId2, (int)ResultValueType.Count, "AS0_1", 15.0, (int)UnitType.Nb),
                new ResultDataStats(InternalDBResItemId2, (int)ResultValueType.AreaSize, "AS0_1", 21385.330, (int)UnitType.um2),
                new ResultDataStats(InternalDBResItemId2, (int)ResultValueType.Count, "AS0_2", 6.0, (int)UnitType.Nb),
                new ResultDataStats(InternalDBResItemId2, (int)ResultValueType.AreaSize, "AS0_1", 1556345.055, (int)UnitType.um2)
            };
            AddResultValues(valuesASO);

            //
            // JOB 2 - Chamber T2-2
            dtNow = DateTime.Now;
            var indata2 = new InPreRegister()
            {
                JobName = "TESTU_JOBID_T2-2",
                LotName = "TESTU_LOTNAME_T2-2",
                TCRecipeName = "TESTU_RCP_T2-2",
                DateTimeRun = dtNow - ts2,
                ToolId = ResultServiceTestInstance.ToolId_T2,

                ChamberId = ResultServiceTestInstance.ChamberId_T2_2,
                ProductId = ResultServiceTestInstance.ProductId,
                RecipeId = ResultServiceTestInstance.RecipeADCId,

                WaferBaseName = "TestU_Wafer_T2-2",
                SlotId = 21,
                ResultType = ResultType.ADC_Klarf,

                Idx = 0,
            };

            var outDtoData2 = RegisterResultService.PreRegisterResultWithPreRegisterObject(indata2);
            long InternalDBResItemId_2 = outDtoData2.Result.InternalDBResItemId;
            int RunIter_2 = outDtoData2.Result.RunIter;
            Assert.AreEqual(0, RunIter_2);
            string ResultFileName_2 = outDtoData2.Result.ResultFileName;
            string ResultPathRoot_2 = outDtoData2.Result.ResultPathRoot;

        }

        private static void RegisterSomeT3Result()
        {
            var dtNow = DateTime.Now;
            var ts1 = new TimeSpan(0, 15, 30);
            var ts2 = new TimeSpan(0, 6, 25);
            var ts3 = new TimeSpan(0, 5, 3);

            //
            // JOB 3 - chamber T3-1 
            var indata = new InPreRegister()
            {
                JobName = "TESTU_JOBID_T3-1",
                LotName = "TESTU_LOTNAME_T3-1",
                TCRecipeName = "TESTU_RCP_T3-1",
                DateTimeRun = dtNow - ts1,
                ToolId = ResultServiceTestInstance.ToolId_T3,

                ChamberId = ResultServiceTestInstance.ChamberId_T3_1,
                ProductId = ResultServiceTestInstance.ProductId,
                RecipeId = ResultServiceTestInstance.RecipeADCId,

                WaferBaseName = "TestU_Wafer_T3-1",
                SlotId = 4,
                ResultType = ResultType.ADC_ASO,
                LabelName = "ASO",

                Idx = 0,
            };

            var RegisterResultService = ClassLocator.Default.GetInstance<IRegisterResultService>();

            var outDtoData = RegisterResultService.PreRegisterResultWithPreRegisterObject(indata);
            long InternalDBResItemId = outDtoData.Result.InternalDBResItemId;
            int RunIter = outDtoData.Result.RunIter;
            Assert.AreEqual(0, RunIter);
            string ResultFileName = outDtoData.Result.ResultFileName;
            string ResultPathRoot = outDtoData.Result.ResultPathRoot;

            //
            // JOB 3 - chamber T3-1 - reprocess Runiter == 1
            //
            indata.DateTimeRun += ts3;
            
            indata.ParentResultId = -1;
            outDtoData = RegisterResultService.PreRegisterResultWithPreRegisterObject(indata);
            InternalDBResItemId = outDtoData.Result.InternalDBResItemId;
            RunIter = outDtoData.Result.RunIter;
            Assert.AreEqual(1, RunIter);
            ResultFileName = outDtoData.Result.ResultFileName;
            ResultPathRoot = outDtoData.Result.ResultPathRoot;

        }

        private static void AddResultValues(List<ResultDataStats> stats)
        {
          
            using (var unitOfWork = new SQL.UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
            {
                foreach (var dt in stats)
                {
                    long dbresid = dt.DBResultItemId;
                    var restatvalue = unitOfWork.ResultItemValueRepository.CreateQuery(true).Where(x => (x.ResultItemId == dt.DBResultItemId) && (x.Name == dt.Name) && (x.Type == dt.Type)).FirstOrDefault();
                    if (restatvalue == null)
                    {
                        unitOfWork.ResultItemValueRepository.Add(new SQL.ResultItemValue()
                        {
                            ResultItemId = dt.DBResultItemId,
                            Name = dt.Name,
                            Value = dt.Value,
                            Type = dt.Type,
                            UnitType = dt.UnitType,
                        }); ;
                    }
                    else
                    {
                        restatvalue.Value = dt.Value;
                        restatvalue.UnitType = dt.UnitType;
                    }
                }
                unitOfWork.Save();
            }
        }


        [TestMethod]
        public void T000_CheckDatabaseVersion()
        {
            var response = ResultService.CheckDatabaseVersion();
            bool isDBupptodate = response.GetResultWithTest();
            string sMsg = "";
            foreach (var msg in response.Messages)
                sMsg += $"<{msg.UserContent}> ";
            Assert.IsTrue(isDBupptodate, $"Database version is not up to date - {sMsg}");
        }

        [TestMethod]
        public void T001_GetTools()
        {
            // Tools
            var tools = ResultService.GetTools();
            Assert.AreEqual(3, tools.Result.Count);
            Assert.AreEqual(ToolName, tools.Result[0].Name);
            Assert.AreEqual(ToolId, tools.Result[0].Id);

            Assert.AreEqual(ToolName2, tools.Result[1].Name);
            Assert.AreEqual(ToolId_T2, tools.Result[1].Id);

            Assert.AreEqual(ToolName3, tools.Result[2].Name);
            Assert.AreEqual(ToolId_T3, tools.Result[2].Id);

            var toolsKey = ResultService.GetToolsKey();
            Assert.AreEqual(3, toolsKey.Result.Count);
            Assert.AreEqual(ToolName, toolsKey.Result[0].Name);
            Assert.AreEqual(1, toolsKey.Result[0].Id);

            Assert.AreEqual(ToolName2, toolsKey.Result[1].Name);
            Assert.AreEqual(2, toolsKey.Result[1].Id);

            Assert.AreEqual(ToolName3, toolsKey.Result[2].Name);
            Assert.AreEqual(3, toolsKey.Result[2].Id);
        }

        [TestMethod]
        public void T002_GetProducts()
        {
            // Products
            var products = ResultService.GetProducts();
            Assert.AreEqual(3, products.Result.Count);
            Assert.AreEqual(ProductName, products.Result.FirstOrDefault().Name);
            Assert.AreEqual(ProductId, products.Result.FirstOrDefault().Id);
            Assert.AreEqual(ProductName + "2", products.Result[1].Name);
            Assert.AreEqual(ProductName + "3", products.Result[2].Name);
        }

        [TestMethod]
        public void T003_GetLots()
        {
            // Lots
            var Lots = ResultService.GetLots(null);
            Assert.AreEqual(3, Lots.Result.Count);
            Assert.AreEqual("TESTU_LOTNAME_T2-1", Lots.Result[0]);
            Assert.AreEqual("TESTU_LOTNAME_T2-2", Lots.Result[1]);
            Assert.AreEqual("TESTU_LOTNAME_T3-1", Lots.Result[2]);

            Lots = ResultService.GetLots(ToolKey);
            Assert.AreEqual(0, Lots.Result.Count);

            Lots = ResultService.GetLots(2); // toolkey de ToolId_T2 == 2
            Assert.AreEqual(2, Lots.Result.Count);
            Assert.AreEqual("TESTU_LOTNAME_T2-1", Lots.Result[0]);
            Assert.AreEqual("TESTU_LOTNAME_T2-2", Lots.Result[1]);

            Lots = ResultService.GetLots(3); // toolkey de ToolId_T3 == 3
            Assert.AreEqual(1, Lots.Result.Count);
            Assert.AreEqual("TESTU_LOTNAME_T3-1", Lots.Result[0]);
        }

        [TestMethod]
        public void T004_GetJobRecipes()
        {
            // Recipes
            var recipes = ResultService.GetRecipes(null);
            Assert.AreEqual(3, recipes.Result.Count);
            Assert.AreEqual("TESTU_RCP_T2-1", recipes.Result[0]);
            Assert.AreEqual("TESTU_RCP_T2-2", recipes.Result[1]);
            Assert.AreEqual("TESTU_RCP_T3-1", recipes.Result[2]);

            recipes = ResultService.GetRecipes(ToolKey);
            Assert.AreEqual(0, recipes.Result.Count);

            recipes = ResultService.GetRecipes(2); // toolkey de ToolId_T2 == 2
            Assert.AreEqual(2, recipes.Result.Count);
            Assert.AreEqual("TESTU_RCP_T2-1", recipes.Result[0]);
            Assert.AreEqual("TESTU_RCP_T2-2", recipes.Result[1]);

            recipes = ResultService.GetRecipes(3); // toolkey de ToolId_T3 == 3
            Assert.AreEqual(1, recipes.Result.Count);
            Assert.AreEqual("TESTU_RCP_T3-1", recipes.Result[0]);
        }

        [TestMethod]
        public void T005_GetProcessModules()
        {
            // Process modules with some results
            var pms = ResultService.GetChambers(null);
            Assert.AreEqual(3, pms.Result.Count);

            // tool1 - 0 results
            pms = ResultService.GetChambers(ToolKey);
            Assert.AreEqual(0, pms.Result.Count);

            // tool2 - 3 results  Ch1[LS] (Klarf + ASO) + Ch2[PSD] (Klarf)
            pms = ResultService.GetChambers(2); // toolkey de ToolId_T2 == 2
            Assert.AreEqual(2, pms.Result.Count);
            Assert.AreNotSame(pms.Result[0].Name, pms.Result[1].Name);
            Assert.AreEqual(ChamberName_T2_1, pms.Result[0].Name);
            Assert.AreEqual(ActorType.LIGHTSPEED, pms.Result[0].ActorType);
            Assert.IsFalse(pms.Result[0].IsArchived);
            Assert.AreEqual(ToolId_T2, pms.Result[0].ToolIdOwner);

            Assert.AreEqual(ChamberName_T2_2, pms.Result[1].Name);
            Assert.AreEqual(ActorType.DEMETER, pms.Result[1].ActorType);
            Assert.IsFalse(pms.Result[1].IsArchived);
            Assert.AreEqual(ToolId_T2, pms.Result[1].ToolIdOwner);


            // tool3 - 2  results Ch1[BF2D] (ASO) |RunIter0 + RunIter1
            pms = ResultService.GetChambers(3); // toolkey de ToolId_T3 == 3
            Assert.AreEqual(1, pms.Result.Count);
            Assert.AreEqual(ChamberName_T3_1, pms.Result[0].Name);
            Assert.AreEqual(ActorType.BrightField2D, pms.Result[0].ActorType);
            Assert.AreEqual(ToolId_T3, pms.Result[0].ToolIdOwner);
        }

        [TestMethod]
        public void T006_GetJobs()
        {
            // Jobs
            var param = new SearchParam();
            var jobs = ResultService.GetSearchJobs(param);
            Assert.AreEqual(4, jobs.Result.Count); // trié par date

            var jobNames = jobs.Result.Select(x => x.JobName);
            Assert.IsTrue(jobNames.Contains("TESTU_JOBID_T2-2"), "<TESTU_JOBID_T2-2> should be contains in jobnames");
            Assert.IsTrue(jobNames.Contains("TESTU_JOBID_T2-1"), "<TESTU_JOBID_T2-1> should be contains in jobnames");
            Assert.IsTrue(jobNames.Contains("TESTU_JOBID_T3-1"), "<TESTU_JOBID_T3-1> should be contains in jobnames");

            // ToolId test
            param.ToolId = ToolId;
            jobs = ResultService.GetSearchJobs(param);
            Assert.AreEqual(0, jobs.Result.Count);

            param.ToolId = ToolId_T2;
            jobs = ResultService.GetSearchJobs(param);
            Assert.AreEqual(2, jobs.Result.Count);
            Assert.AreEqual("TESTU_JOBID_T2-2", jobs.Result[0].JobName);
            Assert.AreEqual("TESTU_JOBID_T2-1", jobs.Result[1].JobName);

            param.ToolId = ToolId_T3;
            jobs = ResultService.GetSearchJobs(param);
            Assert.AreEqual(2, jobs.Result.Count);
            Assert.AreEqual("TESTU_JOBID_T3-1", jobs.Result[0].JobName);
            Assert.AreEqual(jobs.Result[0].JobName, jobs.Result[1].JobName);
            Assert.AreEqual(1, jobs.Result[0].RunIter);
            Assert.AreEqual(0, jobs.Result[1].RunIter);

            //Test LotName
            param = new SearchParam
            {
                LotName = "BAD_LOT_NAME"
            };
            jobs = ResultService.GetSearchJobs(param);
            Assert.AreEqual(0, jobs.Result.Count);

            param.LotName = "TESTU_LOTNAME_T2-1";
            jobs = ResultService.GetSearchJobs(param);
            Assert.AreEqual(1, jobs.Result.Count);

            param.LotName = "TESTU_LOTNAME_T3-1";
            jobs = ResultService.GetSearchJobs(param);
            Assert.AreEqual(2, jobs.Result.Count);
            Assert.AreEqual(1, jobs.Result[0].RunIter);
            Assert.AreEqual(0, jobs.Result[1].RunIter);

            // Test RecipeName
            param = new SearchParam
            {
                RecipeName = "BAD_RCP_NAME"
            };
            jobs = ResultService.GetSearchJobs(param);
            Assert.AreEqual(0, jobs.Result.Count);

            param.RecipeName = "TESTU_RCP_T2-2";
            jobs = ResultService.GetSearchJobs(param);
            Assert.AreEqual(1, jobs.Result.Count);
            Assert.AreEqual("TESTU_JOBID_T2-2", jobs.Result[0].JobName);

            param.RecipeName = "TESTU_RCP_T3-1";
            jobs = ResultService.GetSearchJobs(param);
            Assert.AreEqual(2, jobs.Result.Count);
            Assert.AreEqual(1, jobs.Result[0].RunIter);
            Assert.AreEqual(0, jobs.Result[1].RunIter);

            // Test process module
            param = new SearchParam
            {
                ActorType = ActorType.Darkfield
            };
            jobs = ResultService.GetSearchJobs(param);
            Assert.AreEqual(0, jobs.Result.Count);

            param.ActorType = ActorType.DEMETER;
            jobs = ResultService.GetSearchJobs(param);
            Assert.AreEqual(1, jobs.Result.Count);
            Assert.AreEqual("TESTU_JOBID_T2-2", jobs.Result[0].JobName);

            param.ActorType = ActorType.LIGHTSPEED;
            jobs = ResultService.GetSearchJobs(param);
            Assert.AreEqual(1, jobs.Result.Count);
            Assert.AreEqual("TESTU_JOBID_T2-1", jobs.Result[0].JobName);

            param.ActorType = ActorType.BrightField2D;
            jobs = ResultService.GetSearchJobs(param);
            Assert.AreEqual(2, jobs.Result.Count);
            Assert.AreEqual("TESTU_JOBID_T3-1", jobs.Result[0].JobName);
            Assert.AreEqual(1, jobs.Result[0].RunIter, "the runiter 1 should appeared first");
            Assert.AreEqual(0, jobs.Result[1].RunIter, "the runoiter 0 should appeared in second position");

            // Test product type
            param = new SearchParam
            {
                ProductId = 666999666
            };
            jobs = ResultService.GetSearchJobs(param);
            Assert.AreEqual(0, jobs.Result.Count);

            param.ProductId = ProductId;
            jobs = ResultService.GetSearchJobs(param);
            Assert.AreEqual(4, jobs.Result.Count);

            // Test ToolId and LotName
            param = new SearchParam
            {
                ToolId = ToolId_T3,
                LotName = "TESTU_LOTNAME_T2-2"
            };
            jobs = ResultService.GetSearchJobs(param);
            Assert.AreEqual(0, jobs.Result.Count);

            param.ToolId = ToolId_T2;
            jobs = ResultService.GetSearchJobs(param);
            Assert.AreEqual(1, jobs.Result.Count);
        }

        [TestMethod]
        public void T007_GetSelectedJobResults()
        {
            // Test job
            var param = new SearchParam
            {
                ToolId = ToolId_T2,
                LotName = "TESTU_LOTNAME_T2-1"
            };
            var jobs = ResultService.GetSearchJobs(param);
            Assert.AreEqual(1, jobs.Result.Count);
            int jobid = jobs.Result[0].Id;

            var pmsLists = ResultService.GetJobProcessModulesResults(ToolId_T2, jobid, false); // sans acquisition 
            Assert.AreEqual(1, pmsLists.Result.Count);
            Assert.AreEqual(ActorType.LIGHTSPEED, pmsLists.Result[0].ActorType);

            ///////////// Test Karf data ////////////////////////////////////////////////////
            var klarfWafers = pmsLists.Result[0].PostProcessingResults[ResultFormatExtension.GetLabelName(ResultType.ADC_Klarf)];
            Assert.IsTrue(klarfWafers.Length == 25);

            // Stats data for klarf result
            var klarfStatsData = GetStatsData(klarfWafers);
            // Test klarf total stats data
            Assert.AreEqual(3, klarfStatsData[ResultValueType.Count].Count);
            // Klarf count stats
            Assert.IsTrue(klarfStatsData.ContainsKey(ResultValueType.Count));
            Assert.AreEqual(3, klarfStatsData[ResultValueType.Count].Count);
            // Klarf AreaSize stats
            Assert.IsTrue(!klarfStatsData.ContainsKey(ResultValueType.AreaSize));

            ///////////// Test Aso data ////////////////////////////////////////////////////
            var asoWafers = pmsLists.Result[0].PostProcessingResults["ASO"];
            Assert.IsTrue(asoWafers.Length == 25); // a verifier

            // Stats data for aso result
            var AsoStatsData = GetStatsData(asoWafers);
            // Test klarf total stats data
            Assert.IsTrue(AsoStatsData.ContainsKey(ResultValueType.Count) && AsoStatsData.ContainsKey(ResultValueType.AreaSize));
            // Klarf count stats
            Assert.AreEqual(2, AsoStatsData[ResultValueType.AreaSize].Count);
            Assert.AreEqual(2, AsoStatsData[ResultValueType.Count].Count);

            // Test histogram average data
        }

        [TestMethod]
        public void T008_RegisteringResultAcquisition()
        {
            string currentpath =  Directory.GetCurrentDirectory();
            string fullPathFile = "dummyresult.png";
            using (var wstream = new StreamWriter(fullPathFile))
            {
                wstream.WriteLine("this is a dummy result test file");
            }
            string fullPathFile_thumbs = InPreRegisterAcqHelper.FileNameToThumbName(fullPathFile);
            using (var wstream = new StreamWriter(fullPathFile_thumbs))
            {
                wstream.WriteLine("this is a dummy result test file - Thumbnail");
            }

            // ADD result to existing job and result (cas peu commun | normalement c'est l'acqusition qui commence)
            //
            var RegisterResultService = ClassLocator.Default.GetInstance<IRegisterResultService>();

            var dtrunTime = DateTime.Now;
            var indata = new InPreRegisterAcquisition()
            {
                JobName = "TESTU_JOBID_T2-2",
                LotName = "TESTU_LOTNAME_T2-2",
                TCRecipeName = "TESTU_RCP_T2-2",
                DateTimeRun = dtrunTime,
                ToolId = ResultServiceTestInstance.ToolId_T2,

                ChamberId = ResultServiceTestInstance.ChamberId_T2_2,
                ProductId = ResultServiceTestInstance.ProductId,
                RecipeId = ResultServiceTestInstance.RecipeADCId,

                WaferBaseName = "TestU_Acq_T2-2",
                SlotId = 22,
                ResultType = ResultType.DMT_AmplitudeX_Front,
                LabelName = "CX",

                Idx = 0,

                PathName = currentpath,
                FileName = fullPathFile,
            };

            var outDtoData = RegisterResultService.PreRegisterAcquisitionWithPreRegisterObject(indata);
            Assert.IsNotNull(outDtoData.Result, "PreRegisterAcquisition #1 should not be null");

            using (var unitOfWork = new SQL.UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
            {
                var resNewacqitem = unitOfWork.ResultAcqItemRepository.CreateQuery(false).Where(x => (x.Id == outDtoData.Result.InternalDBResItemId)).FirstOrDefault();
                Assert.IsNotNull(resNewacqitem, "New Acquisition item inserted not found #1");

                Assert.AreEqual(outDtoData.Result.InternalDBResItemId, resNewacqitem.Id);
                Assert.AreEqual(outDtoData.Result.InternalDBResId, resNewacqitem.ResultAcqId);
                Assert.AreEqual(indata.ResultType, (ResultType)resNewacqitem.ResultType);
                Assert.AreEqual(indata.FileName, resNewacqitem.FileName);
                Assert.AreEqual(indata.LabelName, resNewacqitem.Name.Trim());
                Assert.AreEqual(indata.Idx, resNewacqitem.Idx);
                Assert.AreEqual((int)ResultState.NotProcess, resNewacqitem.State);
                Assert.AreEqual((int)ResultInternalState.NotProcess, resNewacqitem.InternalState);
            }

            var state = ResultState.Ok;
            var response = RegisterResultService.UpdateResultAcquisitionState(outDtoData.Result.InternalDBResItemId, state);
            Assert.IsTrue(response.Result, "UpdateResultAcquisitionState #1 should return true");

            using (var unitOfWork = new SQL.UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
            {
                var resNewacqitem = unitOfWork.ResultAcqItemRepository.CreateQuery(false).Where(x => (x.Id == outDtoData.Result.InternalDBResItemId)).FirstOrDefault();
                Assert.IsNotNull(resNewacqitem, "New Acquisition item inserted not found #2");

                Assert.AreEqual(outDtoData.Result.InternalDBResItemId, resNewacqitem.Id);
                Assert.AreEqual(outDtoData.Result.InternalDBResId, resNewacqitem.ResultAcqId);
                Assert.AreEqual(indata.ResultType, (ResultType)resNewacqitem.ResultType);
                Assert.AreEqual(indata.FileName, resNewacqitem.FileName);
                Assert.AreEqual(indata.LabelName, resNewacqitem.Name.Trim());
                Assert.AreEqual(indata.Idx, resNewacqitem.Idx);
                Assert.AreEqual(state, (ResultState)resNewacqitem.State);
                Assert.AreEqual((int)ResultInternalState.Ok, resNewacqitem.InternalState);
            }

            // ADD another result to this acquisition on le gère avec le parent id)
            //
            var indata2 = new InPreRegisterAcquisition()
            {
                ParentResultId = outDtoData.Result.InternalDBResId,

                WaferBaseName = "TestU_Acq_T2-2",
                SlotId = 22,
                ResultType = ResultType.DMT_AmplitudeY_Front,
                LabelName = "CY",

                Idx = 0,

                PathName = currentpath,
                FileName = fullPathFile,
            };

            outDtoData = RegisterResultService.PreRegisterAcquisitionWithPreRegisterObject(indata2);
            Assert.IsNotNull(outDtoData.Result, "PreRegisterAcquisition #2 should not be null");

            response = RegisterResultService.UpdateResultAcquisitionState(outDtoData.Result.InternalDBResItemId, state);
            Assert.IsTrue(response.Result, "UpdateResultAcquisitionState #2 should return true");

            int DBJobId = 0;
            long DBWaferResultS22 = 0;
            using (var unitOfWork = new SQL.UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
            {
                var resNewacqitem = unitOfWork.ResultAcqItemRepository.CreateQuery(false).Where(x => (x.Id == outDtoData.Result.InternalDBResItemId)).FirstOrDefault();
                Assert.IsNotNull(resNewacqitem, "New Acquisition item inserted not found #2");

                Assert.AreEqual(outDtoData.Result.InternalDBResItemId, resNewacqitem.Id);
                Assert.AreEqual(outDtoData.Result.InternalDBResId, resNewacqitem.ResultAcqId);
                Assert.AreEqual(indata2.ResultType, (ResultType)resNewacqitem.ResultType);
                Assert.AreEqual(indata2.FileName, resNewacqitem.FileName);
                Assert.AreEqual(indata2.LabelName, resNewacqitem.Name.Trim());
                Assert.AreEqual(indata2.Idx, resNewacqitem.Idx);
                Assert.AreEqual(state, (ResultState)resNewacqitem.State); 
                Assert.AreEqual((int)ResultInternalState.Ok, resNewacqitem.InternalState);

                var resacq = unitOfWork.ResultAcqRepository.CreateQuery(false, x => x.WaferResult.Job).Where(x => (x.Id == outDtoData.Result.InternalDBResId)).FirstOrDefault();
                DBJobId = resacq.WaferResult.Job.Id;
                DBWaferResultS22 = resacq.WaferResultId;
                Assert.AreEqual(indata.JobName, resacq.WaferResult.Job.JobName);
                Assert.AreEqual(indata.LotName, resacq.WaferResult.Job.LotName);
                Assert.AreEqual(indata.TCRecipeName, resacq.WaferResult.Job.RecipeName);
                
                Assert.AreEqual(indata2.SlotId, resacq.WaferResult.SlotId);
                
                Assert.AreEqual(indata.ProductId, resacq.WaferResult.ProductId);
                Assert.AreEqual(indata.RecipeId, resacq.RecipeId);
                Assert.AreEqual(indata.ChamberId, resacq.ChamberId);
            }

            // ADD another result to this acquisition ici a titre d'exemple on connais le jobid
            // New Slot S23 - pas de label name
            //
            var indata3 = new InPreRegisterAcquisition()
            {
                JobId = DBJobId,

                ChamberId = ResultServiceTestInstance.ChamberId_T2_2,
                ProductId = ResultServiceTestInstance.ProductId,
                RecipeId = ResultServiceTestInstance.RecipeADCId,

                WaferBaseName = "TestU_Acq_T2-2",
                SlotId = 23,
                ResultType = ResultType.DMT_AmplitudeX_Front,
                LabelName = null, // pas de labelname
        
                Idx = 0,

                PathName = currentpath,
                FileName = fullPathFile,
            };

            outDtoData = RegisterResultService.PreRegisterAcquisitionWithPreRegisterObject(indata3);
            Assert.IsNotNull(outDtoData.Result, "PreRegisterAcquisition #2 should not be null");

            state = ResultState.Partial;
            response = RegisterResultService.UpdateResultAcquisitionState(outDtoData.Result.InternalDBResItemId, state);
            Assert.IsTrue(response.Result, "UpdateResultAcquisitionState #3 should return true");

            using (var unitOfWork = new SQL.UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
            {
                var resNewacqitem = unitOfWork.ResultAcqItemRepository.CreateQuery(false).Where(x => (x.Id == outDtoData.Result.InternalDBResItemId)).FirstOrDefault();
                Assert.IsNotNull(resNewacqitem, "New Acquisition item inserted not found #3");

                Assert.AreEqual(outDtoData.Result.InternalDBResItemId, resNewacqitem.Id);
                Assert.AreEqual(outDtoData.Result.InternalDBResId, resNewacqitem.ResultAcqId);
                Assert.AreEqual(indata3.ResultType, (ResultType) resNewacqitem.ResultType);
                Assert.AreEqual(indata3.FileName, resNewacqitem.FileName);
                Assert.AreEqual(indata3.ResultType.DefaultLabelName(indata3.Idx), resNewacqitem.Name.Trim());
                Assert.AreEqual(indata3.Idx, resNewacqitem.Idx);
                Assert.AreEqual(state, (ResultState)resNewacqitem.State);
                Assert.AreEqual((int)ResultInternalState.Ok, resNewacqitem.InternalState);

                var resacq = unitOfWork.ResultAcqRepository.CreateQuery(false, x => x.WaferResult.Job).Where(x => (x.Id == outDtoData.Result.InternalDBResId)).FirstOrDefault();
                Assert.AreEqual(indata.JobName, resacq.WaferResult.Job.JobName);
                Assert.AreEqual(indata.LotName, resacq.WaferResult.Job.LotName);
                Assert.AreEqual(indata.TCRecipeName, resacq.WaferResult.Job.RecipeName);

                Assert.AreEqual(indata3.SlotId, resacq.WaferResult.SlotId);
                Assert.AreNotEqual(DBWaferResultS22, resacq.WaferResultId);

                Assert.AreEqual(indata3.ProductId, resacq.WaferResult.ProductId);
                Assert.AreEqual(indata3.RecipeId, resacq.RecipeId);
                Assert.AreEqual(indata3.ChamberId, resacq.ChamberId);
            }

            indata3.Idx++;
            indata3.LabelName = null; // reset label name
            outDtoData = RegisterResultService.PreRegisterAcquisitionWithPreRegisterObject(indata3);
            Assert.IsNotNull(outDtoData.Result, "PreRegisterAcquisition #4 should not be null");
            state = ResultState.Ok;
            response = RegisterResultService.UpdateResultAcquisitionState(outDtoData.Result.InternalDBResItemId, state);
            Assert.IsTrue(response.Result, "UpdateResultAcquisitionState #4 should return true");

            using (var unitOfWork = new SQL.UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
            {
                var resNewacqitem = unitOfWork.ResultAcqItemRepository.CreateQuery(false, x => x.ResultAcq, x => x.ResultAcq.WaferResult).Where(x => (x.Id == outDtoData.Result.InternalDBResItemId)).FirstOrDefault();
                Assert.IsNotNull(resNewacqitem, "New Acquisition item inserted not found #4");

                Assert.AreEqual(indata3.Idx, resNewacqitem.Idx);
                Assert.AreEqual(state, (ResultState)resNewacqitem.State);
                Assert.AreEqual((int)ResultInternalState.Ok, resNewacqitem.InternalState);

                Assert.AreEqual(indata3.SlotId, resNewacqitem.ResultAcq.WaferResult.SlotId);
                Assert.AreEqual(indata3.ProductId, resNewacqitem.ResultAcq.WaferResult.ProductId);
                Assert.AreEqual(indata3.RecipeId, resNewacqitem.ResultAcq.RecipeId);
                Assert.AreEqual(indata3.ChamberId, resNewacqitem.ResultAcq.ChamberId);
            }

            indata3.FileName = "FileThatDoesnotexist.png";
            string fullPathFile2 = indata3.FileName;
            string fullPathFile_thumbs2 = indata3.FileNameThumbnail();
            if (File.Exists(fullPathFile2))
                File.Delete(fullPathFile2);
            if (File.Exists(fullPathFile_thumbs2))
                File.Delete(fullPathFile_thumbs2);
            outDtoData = RegisterResultService.PreRegisterAcquisitionWithPreRegisterObject(indata3);
            Assert.IsNotNull(outDtoData.Result, "PreRegisterAcquisition #5 should not be null");
            state = ResultState.Rework;
            response = RegisterResultService.UpdateResultAcquisitionState(outDtoData.Result.InternalDBResItemId, state);
            Assert.IsFalse(response.Result, "UpdateResultAcquisitionState #5 should return false, FilePath should not exist");
            
            File.Move(fullPathFile, indata3.FileName);
            state = ResultState.Reject;
            // waiting for exception since thumbnail does not exist
            response = RegisterResultService.UpdateResultAcquisitionState(outDtoData.Result.InternalDBResItemId, state);
            Assert.IsFalse(response.Result, "UpdateResultAcquisitionState #6 should return false, Thumbnail FilePath should not exist");

            File.Move(fullPathFile_thumbs, fullPathFile_thumbs2);
            state = ResultState.Ok;
            response = RegisterResultService.UpdateResultAcquisitionState(outDtoData.Result.InternalDBResItemId, state);
            Assert.IsTrue(response.Result, "UpdateResultAcquisitionState #7 should return true");

            File.Delete(fullPathFile2);
            File.Delete(fullPathFile_thumbs2);

        }


        // to test
        // Response<bool> IsConnectionAvailable();
        // Response<VoidResult> ResultScanRequest(long resultDBId, bool isAcquisition);
        // Response<VoidResult> ResultReScanRequest(long resultDBId, bool isAcquisition);
        // Response<KlarfSettingsData> GetKlarfSettingsFromScanner();
        // Response<KlarfSettingsData> GetKlarfSettingsFromTables();
        // Response<VoidResult> RemoteUpdateKlarfSizeBins(SizeBins szbins);
        // Response<VoidResult> RemoteUpdateKlarfDefectBins(DefectBins defbins);

        private class waferStatsData
        {
            public int SlotId;
            public ResultState State;
            public Dto.ResultItemValue ResValue;
        }

        private Dictionary<ResultValueType, List<waferStatsData>> GetStatsData(WaferResultData[] waferResults)
        {
            var statsData = new Dictionary<ResultValueType, List<waferStatsData>>();
            if (waferResults != null && waferResults.Length > 0)
            {
                var resultItems = waferResults.Where(x => x != null && x.ResultItem != null && x.ResultItem.ResultItemValues.Count > 0)
                                       .Select(x => x.ResultItem).ToList();
                foreach (var resultitem in resultItems)
                {
                    foreach (var resultValue in resultitem.ResultItemValues)
                    {
                        var waferStatsData = new waferStatsData
                        {
                            SlotId = resultitem.Result.WaferResult.SlotId,
                            State = (ResultState)resultitem.State,
                            ResValue = resultValue
                        };
                        var resultValueType = (ResultValueType)resultValue.Type;
                        if (!statsData.Keys.Contains(resultValueType))
                            statsData.Add(resultValueType, new List<waferStatsData>());
                        statsData[resultValueType].Add(waferStatsData);
                    }
                }
            }
            return statsData;
        }
    }
}
