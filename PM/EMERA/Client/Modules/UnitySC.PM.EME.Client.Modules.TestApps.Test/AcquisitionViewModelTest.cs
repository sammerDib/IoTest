using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.EME.Client.Modules.TestApps.Acquisition;
using UnitySC.PM.EME.Client.Proxy.Camera;
using UnitySC.PM.EME.Client.Proxy.FilterWheel;
using UnitySC.PM.EME.Client.TestUtils;
using UnitySC.Shared.Image;

namespace UnitySC.PM.EME.Client.Modules.TestApps.Test
{
    [TestClass]
    public class AcquisitionViewModelTest
    {
        private readonly string _acquisitionPath =
            Path.Combine(Path.GetTempPath(), "Emera-" + DateTime.Now.ToString("yyyy-MM-dd-hhmmss"));

        [TestMethod]
        public void CaptureAnImage_ShouldSaveAnImage()
        {
            // Given
            var messenger = new WeakReferenceMessenger();
            var cameraBench = new CameraBench(new FakeCameraSupervisor(messenger), new FakeCalibrationSupervisor(), messenger);
            var filterWheelBench = new FilterWheelBench(new FakeFilterWheelSupervisor(), new FakeCalibrationSupervisor(), null);
            var acquisitionViewModel =
                new AcquisitionViewModel(cameraBench, filterWheelBench, messenger, _acquisitionPath);

            // When
            acquisitionViewModel.CaptureImageCommand.ExecuteAsync(null).Wait();

            // Then
            Assert.IsTrue(Directory.Exists(_acquisitionPath));
            Assert.IsTrue(Directory.EnumerateFiles(_acquisitionPath, "Image-*.tiff").Any());
        }

        [TestMethod]
        public void CameraStreaming_ShouldProduceAnImage()
        {
            // Given
            var messenger = new WeakReferenceMessenger();
            var cameraSupervisor = new FakeCameraSupervisor(messenger);
            var filterWheelBench = new FilterWheelBench(new FakeFilterWheelSupervisor(), new FakeCalibrationSupervisor(), null);
            var cameraBench = new CameraBench(cameraSupervisor, new FakeCalibrationSupervisor(), messenger);
            var acquisitionViewModel =
                new AcquisitionViewModel(cameraBench, filterWheelBench, messenger, _acquisitionPath);

            ServiceImageWithStatistics image = null;
            messenger.Register<ServiceImageWithStatistics>(this, (_, i) => image = i);

            // When
            acquisitionViewModel.StartStreamingCommand.ExecuteAsync(null).Wait();
            cameraSupervisor.WaitForOneAcquisition();
            acquisitionViewModel.StopStreamingCommand.ExecuteAsync(null).Wait();

            // Then
            Assert.IsNotNull(image);
        }

        [TestMethod]
        public void SetExposureTimeValue_ShouldChangeExposureTimeInService()
        {
            // Given
            var messenger = new WeakReferenceMessenger();
            var cameraSupervisor = new FakeCameraSupervisor(messenger);
            var filterWheelBench = new FilterWheelBench(new FakeFilterWheelSupervisor(), new FakeCalibrationSupervisor(), null);
            var cameraBench = new CameraBench(cameraSupervisor, new FakeCalibrationSupervisor(), null);
            var acquisitionViewModel = new AcquisitionViewModel(cameraBench, filterWheelBench, null, _acquisitionPath);

            // When
            acquisitionViewModel.ExposureTime = 3.0;
            acquisitionViewModel.ApplyConfiguration.ExecuteAsync(null).Wait();

            // Then
            Assert.AreEqual(3.0, cameraSupervisor.GetCameraExposureTime().Result);
        }

        [TestMethod]
        public void SetGainValue_ShouldChangeCameraGainInService()
        {
            // Given
            var messenger = new WeakReferenceMessenger();
            var cameraSupervisor = new FakeCameraSupervisor(messenger);
            var filterWheelBench = new FilterWheelBench(new FakeFilterWheelSupervisor(), new FakeCalibrationSupervisor(), null);
            var cameraBench = new CameraBench(cameraSupervisor, new FakeCalibrationSupervisor(), null);
            var acquisitionViewModel = new AcquisitionViewModel(cameraBench, filterWheelBench, null, _acquisitionPath);

            // When
            acquisitionViewModel.Gain = 3.0;
            acquisitionViewModel.ApplyConfiguration.ExecuteAsync(null).Wait();

            // Then
            Assert.AreEqual(3.0, cameraSupervisor.GetCameraGain().Result);
        }

        [TestMethod]
        public async Task ChangeFilterPosition_ShouldMoveFilterWheel()
        {
            // Given
            var messenger = new WeakReferenceMessenger();
            var cameraSupervisor = new FakeCameraSupervisor(messenger);
            var fakeFilterWheelSupervisor = new FakeFilterWheelSupervisor();
            var filterWheelBench = new FilterWheelBench(fakeFilterWheelSupervisor, new FakeCalibrationSupervisor(), null);
            var cameraBench = new CameraBench(cameraSupervisor, new FakeCalibrationSupervisor(), messenger);
            var acquisitionViewModel =
                new AcquisitionViewModel(cameraBench, filterWheelBench, messenger, _acquisitionPath);

            // When
            acquisitionViewModel.FilterWheelBench.CurrentFilter = filterWheelBench.Filters[0];

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            while (filterWheelBench.IsLoading)
            {
                if (stopwatch.ElapsedMilliseconds >= 1000)
                    break;

                await Task.Delay(100);
            }
            stopwatch.Stop();
            
            // Then
            double actualPosition = fakeFilterWheelSupervisor.GetCurrentPosition().Result;
            double expectedFilterPosition = filterWheelBench.GetFilters()[0].Position;
            Assert.AreEqual(expectedFilterPosition, actualPosition);
        }
    }
}
