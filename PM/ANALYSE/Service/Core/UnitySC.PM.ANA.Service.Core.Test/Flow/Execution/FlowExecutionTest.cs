using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SimpleInjector;

using UnitySC.PM.ANA.Service.Shared.TestUtils;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Service.Core.Test.Flow.Execution
{
    [TestClass]
    public class FlowExecutionTest
    {
        private ILogger _logger;

        public async Task ExecuteFlowTest()
        {
            var flowComponent = new AutofocusFlowTest(new AutofocusInputTest());
            flowComponent.StatusChanged += FlowComponent_StatusChanged;
            var flowTask = new FlowTask<AutofocusInputTest, AutofocusResultTest>(flowComponent);
            _logger.Debug($"Start flow {flowComponent.Name}");
            flowTask.Start();
            var res = await flowTask;
            Assert.IsTrue(res.Status.State == FlowState.Success);
            _logger.Debug($"End flow {flowComponent.Name}");
        }

        public async Task ExecuteAndCancelFlowTest()
        {
            var flowComponent = new AutofocusFlowTest(new AutofocusInputTest());
            flowComponent.StatusChanged += FlowComponent_StatusChanged;
            var flowTask = new FlowTask<AutofocusInputTest, AutofocusResultTest>(flowComponent);
            _logger.Debug($"Start flow {flowComponent.Name}");
            var cancelTask = Task.Run(() =>
            {
                Thread.Sleep(1000);
                // Cancel the task
                flowTask.Cancel();
            });
            flowTask.Start();
            var res = await flowTask;
            Assert.IsTrue(res.Status.State == FlowState.Canceled);
            _logger.Debug($"End flow {flowComponent.Name}");
        }

        public async Task ExecuteWithErrorFlowTest()
        {
            var flowComponent = new AutofocusFlowTest(new AutofocusInputTest() { Error = true }); ;
            flowComponent.StatusChanged += FlowComponent_StatusChanged;
            var flowTask = new FlowTask<AutofocusInputTest, AutofocusResultTest>(flowComponent);
            _logger.Debug($"Start flow {flowComponent.Name}");
            flowTask.Start();
            var res = await flowTask;
            Assert.IsTrue(res.Status.State == FlowState.Error);
            _logger.Debug($"End flow {flowComponent.Name}");
        }

        public async Task ExecuteFlowTestTimeout()
        {
            var flowComponent = new AutofocusFlowTest(new AutofocusInputTest());
            flowComponent.StatusChanged += FlowComponent_StatusChanged;
            var flowTask = new FlowTask<AutofocusInputTest, AutofocusResultTest>(flowComponent, new TimeSpan(1000));
            _logger.Debug($"Start flow {flowComponent.Name}");
            flowTask.Start();
            var res = await flowTask;
            Assert.IsTrue(res.Status.State == FlowState.Canceled);
            _logger.Debug($"End flow {flowComponent.Name}");
        }

        public async Task ExecuteFlowTestWithConfig()
        {
            var flowComponent = new AutofocusFlowTestWithConfig(new AutofocusInputTest());
            flowComponent.StatusChanged += FlowComponent_StatusChanged;
            var flowTask = new FlowTask<AutofocusInputTest, AutofocusResultTest, AutofocusFlowTestConfig>(flowComponent);
            _logger.Debug($"Start flow {flowComponent.Name}");
            flowTask.Start();
            var res = await flowTask;
            Assert.IsTrue(res.Status.State == FlowState.Success);
            _logger.Debug($"End flow {flowComponent.Name}");
        }

        public async Task ExecuteAndCancelFlowTestWithConfig()
        {
            var flowComponent = new AutofocusFlowTestWithConfig(new AutofocusInputTest());
            flowComponent.StatusChanged += FlowComponent_StatusChanged;
            var flowTask = new FlowTask<AutofocusInputTest, AutofocusResultTest, AutofocusFlowTestConfig>(flowComponent);
            _logger.Debug($"Start flow {flowComponent.Name}");
            var cancelTask = Task.Run(() =>
            {
                Thread.Sleep(1000);
                // Cancel the task
                flowTask.Cancel();
            });
            flowTask.Start();
            var res = await flowTask;
            Assert.IsTrue(res.Status.State == FlowState.Canceled);
            _logger.Debug($"End flow {flowComponent.Name}");
        }

        public async Task ExecuteWithErrorFlowTestWithConfig()
        {
            var flowComponent = new AutofocusFlowTestWithConfig(new AutofocusInputTest() { Error = true });
            flowComponent.StatusChanged += FlowComponent_StatusChanged;
            var flowTask = new FlowTask<AutofocusInputTest, AutofocusResultTest, AutofocusFlowTestConfig>(flowComponent);
            _logger.Debug($"Start flow {flowComponent.Name}");
            flowTask.Start();
            var res = await flowTask;
            Assert.IsTrue(res.Status.State == FlowState.Error);
            _logger.Debug($"End flow {flowComponent.Name}");
        }

        public async Task ExecuteFlowTestTimeoutWithConfig()
        {
            var flowComponent = new AutofocusFlowTest(new AutofocusInputTest());
            flowComponent.StatusChanged += FlowComponent_StatusChanged;
            var flowTask = new FlowTask<AutofocusInputTest, AutofocusResultTest>(flowComponent, new TimeSpan(1000));
            _logger.Debug($"Start flow {flowComponent.Name}");
            flowTask.Start();
            var res = await flowTask;
            Assert.IsTrue(res.Status.State == FlowState.Canceled);
            _logger.Debug($"End flow {flowComponent.Name}");
        }

        private void FlowComponent_StatusChanged(FlowStatus status, IFlowResult statusData)
        {
            _logger.Debug($"Status change to {status.State}:  {status.Message}");
        }

        [TestInitialize]
        public void Init()
        {
            var container = new Container();
            Bootstrapper.Register(container);
            _logger = ClassLocator.Default.GetInstance<ILogger>();
        }

        [TestMethod]
        public void ExecuteFlow()
        {
            Task.Run(() => ExecuteFlowTest()).Wait();
            Task.Run(() => ExecuteAndCancelFlowTest()).Wait();
            Task.Run(() => ExecuteWithErrorFlowTest()).Wait();
            Task.Run(() => ExecuteFlowTestTimeout()).Wait();
        }

        [TestMethod]
        public void ExecuteFlowWithConfig()
        {
            Task.Run(() => ExecuteFlowTestWithConfig()).Wait();
            Task.Run(() => ExecuteAndCancelFlowTestWithConfig()).Wait();
            Task.Run(() => ExecuteWithErrorFlowTestWithConfig()).Wait();
            Task.Run(() => ExecuteFlowTestTimeoutWithConfig()).Wait();
        }
    }
}
