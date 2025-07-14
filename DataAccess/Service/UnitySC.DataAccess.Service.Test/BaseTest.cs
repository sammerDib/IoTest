using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.DataAccess.Service.Implementation;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.DataAccess.SQL;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.DataAccess.Service.Test
{
    [TestClass]
    public class BaseTest
    {
        public const string ToolName = "Tool1";
        public const string ChamberName = "PSD1";
        public const string UserName = "User1";
        public const string ProductName = "Product1";
        public const string StepName = "Step1";
        public const string DemeterRecipeName = "DemeterRecipe";
        public const string DemeterRecipeNameNotBindToDataflow = "DemeterRecipeNotBinded";
        public const string ADCRecipeName = "ADCRecipe";
        public const string DataflowName = "DataflowDMTADC";
        public const string LayerName = "SiLayer";
        public const string MaterialName = "Si";

        public int UserId { get; private set; }

        public int ToolId { get; private set; }

        public int ToolKey { get; private set; }

        public int ChamberId { get; private set; }

        public int ChamberKey { get; private set; }

        public int ProductId { get; private set; }

        public int StepId { get; private set; }

        public int RecipeDemeterId { get; private set; }
        public int RecipeDemeterNotBindedId { get; private set; }

        public int RecipeADCId { get; private set; }
        public int WaferCategoryId { get; private set; }
        public int MaterialId { get; private set; }
        public int LayerId { get; private set; }

        private IToolService _toolService => ClassLocator.Default.GetInstance<IToolService>();
        private IUserService _userService => ClassLocator.Default.GetInstance<IUserService>();
        private IDbRecipeService _recipeService => ClassLocator.Default.GetInstance<IDbRecipeService>();

        private TestContext _testContextInstance;
        public TestContext TestContext
        {
            get { return _testContextInstance; }
            set { _testContextInstance = value; }
        }

        private static BaseTest s_instance = null;
        private static readonly object s_padlock = new object();

        public static BaseTest BaseInstance
        {
            get
            {
                lock (s_padlock)
                {
                    if (s_instance == null)
                    {
                        s_instance = new BaseTest();
                    }
                    return s_instance;
                }
            }
        }

        [AssemblyInitialize]
        public static void AssemblyInit(TestContext testContext)
        {
            testContext.WriteLine($"AssemblyInitialize call from <{testContext.TestName}>");
            BaseInstance.InitDB();
        }

        public void Init()
        {
            UserId = BaseInstance.UserId;
            ToolId = BaseInstance.ToolId;
            ToolKey = BaseInstance.ToolKey;
            ChamberId = BaseInstance.ChamberId;
            ChamberKey = BaseInstance.ChamberKey;
            ProductId = BaseInstance.ProductId;
            StepId = BaseInstance.StepId;
            RecipeDemeterId = BaseInstance.RecipeDemeterId;
            RecipeDemeterNotBindedId = BaseInstance.RecipeDemeterNotBindedId;
            RecipeADCId = BaseInstance.RecipeADCId;
            WaferCategoryId = BaseInstance.WaferCategoryId;
            MaterialId = BaseInstance.MaterialId;
            LayerId = BaseInstance.LayerId;
        }

        public void InitDB()
        {
            Bootstrapper.Register();

            var logger = ClassLocator.Default.GetInstance<ILogger<object>>();

            // Clear database content before launching test. This function is avalaible only on test database
            using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
            {

                string ProviderConnectionString = unitOfWork.GetConnectionString();
                using (var sqlCon = new System.Data.SqlClient.SqlConnection(ProviderConnectionString))
                {
                    logger.Information("-------------------------------------");
                    logger.Information("--     SQL Database Connection     --");
                    logger.Information("-------------------------------------");
                    logger.Information($"-- Data Source = {sqlCon.DataSource}");
                    logger.Information($"-- Database = {sqlCon.Database}");
                    logger.Information("-------------------------------------");
                }
                logger.Information("Deleting all database contents...");
                unitOfWork.DeleteAllData();
                logger.Debug("Database cleared\n. . . . . . . . .\n");
            }

            CreateDefaultData();
        }

        public void CreateDefaultData()
        {
            var logger = ClassLocator.Default.GetInstance<ILogger<object>>();
            logger.Debug("CreateDefaultData");

            // Add tool
            int toolkey = 1;
            ToolKey = toolkey;
            ToolId = _toolService.SetTool(new Dto.Tool
            {
                Name = ToolName,
                ToolCategory = ToolName + "Category",
                ToolKey = toolkey
            }, null).GetResultWithTest();

            // Connect user
            UserId = _userService.ConnectUserFromToolId(UserName, UserProfiles.Administrator,
                ToolId).GetResultWithTest().Id;

            // Add Chamber
            int chamberkey = 1;
            ChamberKey = chamberkey;
            ChamberId = _toolService.SetChamber(new Dto.Chamber
            {
                Name = ChamberName,
                Actor = ActorType.DEMETER,
                ToolId = ToolId,
                ChamberKey = chamberkey
            }, UserId).GetResultWithTest();

            // Add WaferCategory
            var dimentionalCharacteristics = DefaultWaferDimensionalCharacteristics();
            foreach (var dimentionalCharac in dimentionalCharacteristics)
            {
                var waferCategory = new Dto.WaferCategory();
                waferCategory.Name = dimentionalCharac.ToString();
                waferCategory.DimentionalCharacteristic = dimentionalCharac;
                int id = _toolService.SetWaferCategory(waferCategory, UserId).GetResultWithTest();
                if (WaferCategoryId == 0)
                    WaferCategoryId = id;
            }

            // Add material
            var material = new Dto.Material();
            material.Name = MaterialName;
            material.Characteristic = new MaterialCharacteristic();
            material.Characteristic.RefractiveIndexList = new List<ComplexRefractiveIndex>();
            material.Characteristic.RefractiveIndexList.Add(new ComplexRefractiveIndex() { ExtinctionCoefficient = 0.1, RefractiveIndex = 0.2, WavelengthUm = 0.3 });
            MaterialId = _toolService.SetMaterial(material, UserId).GetResultWithTest();

            // Add product
            ProductId = _toolService.SetProduct(new Dto.Product
            {
                Name = ProductName,
                WaferCategoryId = WaferCategoryId,
                Comment = "Top product"
            }, UserId).GetResultWithTest();
            int ProductId2 = _toolService.SetProduct(new Dto.Product
            {
                Name = ProductName + "2",
                WaferCategoryId = WaferCategoryId,
                Comment = "Middle product"
            }, UserId).GetResultWithTest();
            int ProductId3 = _toolService.SetProduct(new Dto.Product
            {
                Name = ProductName + "3",
                WaferCategoryId = WaferCategoryId,
                Comment = "Low product"
            }, UserId).GetResultWithTest();

            // Add step
            var step = new Dto.Step()
            {
                Name = StepName,
                ProductId = ProductId
            };
            step.Layers = new List<Dto.Layer>();
            var layer = new Dto.Layer();
            layer.MaterialId = MaterialId;
            layer.Name = LayerName;
            step.Layers.Add(layer);
            step.Layers.Add(new Dto.Layer() { Name = "Layer2" });
            StepId = _toolService.SetStep(step, UserId).GetResultWithTest();

            // Add recipe Demeter
            var recipeDemeter = new Dto.Recipe();
            recipeDemeter.Type = ActorType.DEMETER;
            recipeDemeter.CreatorChamberId = ChamberId;
            recipeDemeter.Comment = "Comment " + DemeterRecipeName;
            recipeDemeter.Name = DemeterRecipeName;
            recipeDemeter.StepId = StepId;
            recipeDemeter.CreatorUserId = UserId;
            recipeDemeter.AddOutput(ResultType.DMT_CurvatureX_Front);
            recipeDemeter.AddOutput(ResultType.DMT_CurvatureX_Back);
            RecipeDemeterId = _recipeService.SetRecipe(recipeDemeter, true).GetResultWithTest();

            // Add recipe ADC
            var recipeADC = new Dto.Recipe();
            recipeADC.Type = ActorType.ADC;
            recipeADC.CreatorChamberId = ChamberId;
            recipeADC.Comment = "Comment " + ADCRecipeName;
            recipeADC.Name = ADCRecipeName;
            recipeADC.StepId = StepId;
            recipeADC.CreatorUserId = UserId;
            recipeADC.AddInput(ResultType.DMT_CurvatureX_Front);
            recipeADC.AddInput(ResultType.DMT_CurvatureX_Back);
            recipeADC.AddOutput(ResultType.ADC_Klarf);
            RecipeADCId = _recipeService.SetRecipe(recipeADC, true).GetResultWithTest();

            // Add dataflow with recipe Demeter and ADC
            var dataflowComponent = new DataflowRecipeComponent();
            dataflowComponent.Name = DataflowName;
            dataflowComponent.ActorType = ActorType.DataflowManager;
            var demeter = _recipeService.GetRecipe(RecipeDemeterId).GetResultWithTest().ToDataflowRecipeComponent();
            var adc = _recipeService.GetRecipe(RecipeADCId).GetResultWithTest().ToDataflowRecipeComponent();
            demeter.ChildRecipes.Add(new DataflowRecipeAssociation(Dto.ModelDto.Enum.AssociationRecipeType.All, adc));
            dataflowComponent.ChildRecipes.Add(new DataflowRecipeAssociation(Dto.ModelDto.Enum.AssociationRecipeType.All, demeter));
            dataflowComponent.Comment = "Comment" + DataflowName;
            _recipeService.SetDataflow(dataflowComponent, UserId, StepId, ToolKey);

            // Add recipe Demeter Not Binded to A Dataflow
            var recipeDemeterNotBinded = new Dto.Recipe();
            recipeDemeterNotBinded.Type = ActorType.DEMETER;
            recipeDemeterNotBinded.CreatorChamberId = ChamberId;
            recipeDemeterNotBinded.Comment = "Comment " + DemeterRecipeNameNotBindToDataflow;
            recipeDemeterNotBinded.Name = DemeterRecipeNameNotBindToDataflow;
            recipeDemeterNotBinded.StepId = StepId;
            recipeDemeterNotBinded.CreatorUserId = UserId;
            recipeDemeterNotBinded.AddOutput(ResultType.DMT_CurvatureX_Front);
            recipeDemeterNotBinded.AddOutput(ResultType.DMT_CurvatureX_Back);
            recipeDemeterNotBinded.AddOutput(ResultType.DMT_CurvatureY_Front);
            recipeDemeterNotBinded.AddOutput(ResultType.DMT_CurvatureY_Back);
            recipeDemeterNotBinded.AddOutput(ResultType.DMT_Brightfield_Front);
            recipeDemeterNotBinded.AddOutput(ResultType.DMT_Brightfield_Back);
            RecipeDemeterNotBindedId = _recipeService.SetRecipe(recipeDemeterNotBinded, true).GetResultWithTest();

        }

        private List<WaferDimensionalCharacteristic> DefaultWaferDimensionalCharacteristics()
        {
            var WaferDimensionalCharacteristics = new List<WaferDimensionalCharacteristic>();
            var wafer = new WaferDimensionalCharacteristic();
            wafer.Category = "1.5/1.6";
            wafer.WaferShape = WaferShape.Flat;
            wafer.Diameter = 100.Millimeters();
            wafer.Flats = new List<FlatDimentionalCharacteristic>();
            var flat = new FlatDimentionalCharacteristic
            {
                Angle = 0.Degrees(),
                ChordLength = 32.5.Millimeters()
            };
            wafer.Flats.Add(flat);

            WaferDimensionalCharacteristics.Add(wafer);

            wafer = new WaferDimensionalCharacteristic();
            wafer.Category = "1.8.1/1.8.2";
            wafer.WaferShape = WaferShape.Flat;
            wafer.Diameter = 150.Millimeters();
            wafer.Flats = new List<FlatDimentionalCharacteristic>();
            flat = new FlatDimentionalCharacteristic
            {
                Angle = 0.Degrees(),
                ChordLength = 57.5.Millimeters()
            };
            wafer.Flats.Add(flat);

            flat = new FlatDimentionalCharacteristic
            {
                Angle = 90.Degrees(),
                ChordLength = 37.5.Millimeters()
            };
            wafer.Flats.Add(flat);

            WaferDimensionalCharacteristics.Add(wafer);

            wafer = new WaferDimensionalCharacteristic();
            wafer.Category = "1.9";
            wafer.WaferShape = WaferShape.Notch;
            wafer.Diameter = 200.Millimeters();
            wafer.Notch = new NotchDimentionalCharacteristic()
            {
                Angle = 0.Degrees(),
                Depth = 1.Millimeters(),
                DepthPositiveTolerance = 0.25.Millimeters(),
                AngleNegativeTolerance = 1.Degrees(),
                AnglePositiveTolerance = 5.Degrees(),
            };
            WaferDimensionalCharacteristics.Add(wafer);

            wafer = new WaferDimensionalCharacteristic();
            wafer.Category = "Sample";
            wafer.WaferShape = WaferShape.Sample;
            wafer.SampleWidth = 100.Millimeters();
            wafer.SampleHeight = 50.Millimeters();

            WaferDimensionalCharacteristics.Add(wafer);

            return WaferDimensionalCharacteristics;
        }
    }
}
