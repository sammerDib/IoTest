using System.Collections.Generic;

using CommunityToolkit.Mvvm.Messaging;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.DataAccess.Dto;
using UnitySC.PM.EME.Client.Proxy.Chuck;
using UnitySC.PM.EME.Client.TestUtils;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Client.Test
{


    [TestClass]
    public class ChuckVMTests
    {
        private IMessenger _messenger;

        [TestInitialize]
        public void Setup() 
        {
            ClassLocator.ExternalInit(new SimpleInjector.Container(), true);
            ClassLocator.Default.Register(typeof(ILogger<>), typeof(SerilogLogger<>));
            ClassLocator.Default.Register(typeof(ILogger), typeof(SerilogLogger<object>));
            _messenger = new WeakReferenceMessenger();
            ClassLocator.Default.Register<IMessenger>(() => _messenger, true);           
            ClassLocator.Default.Register(() => ClassLocator.Default.GetInstance<SharedSupervisors>().GetGlobalStatusSupervisor(ActorType.EMERA), true);
        }
        private WaferCategory CreateWaferCategory(int id, Length diameter)
        {
            var dims = new WaferDimensionalCharacteristic()
            {
                Diameter = diameter,
            };
            return new WaferCategory()
            {
                DimentionalCharacteristic = dims,
                Id = id
            };
        }

        [TestMethod]
        public void ChuckVM_Selected_Wafer_Categories_And_Clamp_States_Should_Change_Accordingly()
        {
            var calibrationSupervisor = new FakeCalibrationSupervisor();
            var referentialSupervisor = new FakeReferentialSupervisor();
            var motionAxesSupersivor = new FakeMotionAxesSupervisor(new WeakReferenceMessenger());
            var waferCategories = new List<WaferCategory>()
            {
                CreateWaferCategory(1, 100.Millimeters()),
                CreateWaferCategory(2, 150.Millimeters()),
                CreateWaferCategory(3, 200.Millimeters())
            };
            var chuckSupervisor = new FakeChuckSupervisor(_messenger, waferCategories);
            var globalStatusSupervisor = ClassLocator.Default.GetInstance<SharedSupervisors>().GetGlobalStatusSupervisor(ActorType.EMERA);
            var chuckVM = new ChuckVM(chuckSupervisor, calibrationSupervisor, referentialSupervisor, _messenger);
            chuckVM.WaferCategories = waferCategories;
            chuckVM.Init();
            waferCategories.ForEach(cat =>
                                          {                                              
                                              chuckVM.ChangeClampStatus.Execute(null);
                                              Assert.IsTrue(chuckVM.Status.IsWaferClamped);
                                              chuckVM.ChangeClampStatus.Execute(null);
                                              Assert.IsFalse(chuckVM.Status.IsWaferClamped);
                                          });
        }
    }
}
