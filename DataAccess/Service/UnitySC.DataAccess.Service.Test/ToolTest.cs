using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.Shared.Tools;

namespace UnitySC.DataAccess.Service.Test
{
    [TestClass]
    public class ToolTest : BaseTest
    {
        private IToolService _toolService => ClassLocator.Default.GetInstance<IToolService>();

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {

        }

        [TestInitialize]
        public void TestInit()
        {
            base.Init();
        }

        [TestMethod]
        public void T000_CheckDatabaseVersion()
        {
            var response = _toolService.CheckDatabaseVersion();
            bool isDBupptodate = response.GetResultWithTest();
            string sMsg = "";
            foreach (var msg in response.Messages)
                sMsg += $"<{msg.UserContent}> ";
            Assert.IsTrue(isDBupptodate, $"Database version is not up to date - {sMsg}");
        }

        [TestMethod]
        public void T001_GetToolAndChamber()
        {
            var tool = _toolService.GetAllTools().Result.SingleOrDefault(x => x.Id == ToolId);
            Assert.IsNotNull(tool);
            Assert.IsTrue(tool.Name == ToolName);
            Assert.IsTrue(tool.ToolKey == 1);
            var chamber = _toolService.GetAllChambers().Result.SingleOrDefault(x => x.Id == ChamberId);
            Assert.IsNotNull(chamber);
            Assert.IsTrue(chamber.Name == ChamberName);
        }

        [TestMethod]
        public void T002_GetProductAndSteps()
        {
            var products = _toolService.GetProductAndSteps().GetResultWithTest();
            var product = products.SingleOrDefault(x => x.Id == ProductId);
            Assert.IsNotNull(product);
            Assert.IsNotNull(product.WaferCategory);
            Assert.IsTrue(product.Steps.Any(x => x.Id == StepId));
            var step = product.Steps.First(x => x.Id == StepId);
            var layer = step.Layers.FirstOrDefault(x => x.Name == LayerName);
            Assert.IsNotNull(layer);
        }

        [TestMethod]
        public void T003_GetStep()
        {
            var step = _toolService.GetStep(StepId).GetResultWithTest();
            Assert.IsTrue(step.Name == StepName);
            var layer = step.Layers.FirstOrDefault(x => x.Name == LayerName);
            Assert.IsNotNull(layer);
        }

        [TestMethod]
        public void T004_UpdateStep()
        {
            var step = _toolService.GetStep(StepId).GetResultWithTest();

            // Add
            step.Layers.Add(new Dto.Layer() { Name = "Update", StepId = step.Id});
            step.Layers.Add(new Dto.Layer() { Name = "Update1", StepId = step.Id });
            _toolService.SetStep(step, UserId).GetResultWithTest();
            var newStep = _toolService.GetStep(StepId).GetResultWithTest();
            Assert.IsTrue(newStep.Name == StepName);
            var layer = newStep.Layers.FirstOrDefault(x => x.Name == "Update");
            Assert.IsNotNull(layer);

            // Remove
           newStep.Layers.Remove(layer);
            _toolService.SetStep(newStep, UserId).GetResultWithTest();
            var lastStep = _toolService.GetStep(StepId).GetResultWithTest();
            Assert.IsNull(lastStep.Layers.FirstOrDefault(x => x.Name == "Update"));

        }

        [TestMethod]
        public void T005_GetChambers()
        {
#pragma warning disable CS0618
            var chamber = _toolService.GetChamber(ChamberId).GetResultWithTest();
#pragma warning restore CS0618
            Assert.IsNotNull(chamber);
            Assert.IsTrue(chamber.Id == ChamberId);

            var chamberfromkey = _toolService.GetChamberFromKeys(ToolKey, ChamberKey).GetResultWithTest();
            Assert.IsNotNull(chamberfromkey);
            Assert.IsTrue(chamberfromkey.ChamberKey == ChamberKey);
            Assert.IsTrue(chamberfromkey.Tool.ToolKey == ToolKey);
            Assert.IsTrue(chamberfromkey.Id == ChamberId);
        }

        [TestMethod]
        public void T006_GetMaterials()
        {
            var material = _toolService.GetMaterials().GetResultWithTest().FirstOrDefault();
            Assert.IsNotNull(material);
            Assert.IsTrue(material.Id == MaterialId);
            Assert.IsNotNull(material.Characteristic);
            Assert.IsTrue(material.Characteristic.RefractiveIndexList.Any());
        }

        [TestMethod]
        public void T007_ArchivedProduct()
        {
            var products = _toolService.GetProductAndSteps().GetResultWithTest();
            var product = products.SingleOrDefault(x => x.Id == ProductId);
            Assert.IsNotNull(product, $"Product has not been found : ProductName={ProductName} Id={ProductId}");
            _toolService.ArchiveAProduct(product.Id, UserId);
            products = _toolService.GetProductAndSteps().GetResultWithTest();
            Assert.IsTrue(!products.Any(x => x.Id == ProductId), $"Archived Products should not been listed : ProductName={ProductName} Id={ProductId}");

            // restore archived product
            _toolService.RestoreAProduct(ProductId, UserId);
            products = _toolService.GetProductAndSteps().GetResultWithTest();
            product = products.SingleOrDefault(x => x.Id == ProductId);
            Assert.IsNotNull(product, $"Restored Product has not been found : ProductName={ProductName} Id={ProductId}");

        }

        [TestMethod]
        public void T008_ArchivedStep()
        {
            var products = _toolService.GetProductAndSteps().GetResultWithTest();
            var product = products.SingleOrDefault(x => x.Id == ProductId);
            Assert.IsNotNull(product, $"Product has not been found : ProductName={ProductName} Id={ProductId}");
            var step = product.Steps.FirstOrDefault(x => x.Id == StepId);
            Assert.IsNotNull(step, $"Product Step has not been found : ProductName={ProductName} StepName={StepName} StepId={StepId}");
            _toolService.ArchiveAStep(step.Id, UserId);
            products = _toolService.GetProductAndSteps().GetResultWithTest();
            product = products.SingleOrDefault(x => x.Id == ProductId);
            Assert.IsTrue(!product.Steps.Any(x => x.Id == StepId), $"Archived Step should not been listed : ProductName={ProductName} StepName={StepName} StepId={StepId}");

            // restore archived Step
            _toolService.RestoreAStep(StepId, UserId);
            products = _toolService.GetProductAndSteps().GetResultWithTest();
            product = products.SingleOrDefault(x => x.Id == ProductId);
            step = product.Steps.FirstOrDefault(x => x.Id == StepId);
            Assert.IsNotNull(step, $"restored Step has not been found : ProductName={ProductName} StepName={StepName} StepId={StepId}");
            Assert.IsTrue(product.Steps.Any(x => x.Id == StepId), $"Restored Step should be listed : ProductName={ProductName}  StepName={StepName} StepId={StepId}");
        }

        
    }
}
