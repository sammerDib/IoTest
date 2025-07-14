using System;
using System.Collections.Generic;
using System.Reflection;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using Moq;
using System.IO;
using CommunityToolkit.Mvvm.Messaging;
using UnitySC.Shared.Data.FDC;
using System.Threading;

namespace UnitySC.Shared.FDC.Test
{
    
    [Serializable]
    public class DummyPersistentData : IPersistentFDCData
    {
        public string FDCName { get; set; }

    }

    [TestClass]
    public class UnitFDCManagerTest
    {
        [TestMethod]
        public void TestFDCManager()
        {
            string fdpfile = "FDCsPersistentData.fpd";

            // Logger without caller name
            ClassLocator.Default.Register(typeof(ILogger), typeof(SerilogLogger<object>));
            ClassLocator.Default.Register<IMessenger>(() => WeakReferenceMessenger.Default, true);
            var fdcManager =new FDCManager(@"FDCsConfiguration.xml", fdpfile);
            Assert.IsNotNull(fdcManager);
            fdcManager.StartMonitoringFDC();

            // Using reflection to access the private field
            FieldInfo privateCurrentFDCConfigurationInfo = typeof(FDCManager).GetField("_currentFDCConfiguration", BindingFlags.NonPublic | BindingFlags.Instance);

            // Assert
            Assert.IsNotNull(privateCurrentFDCConfigurationInfo?.GetValue(fdcManager));


            var mockProvider=new Mock<IFDCProvider>(); 

            // Test RegisterFDCProvider

           fdcManager.RegisterFDCProvider(mockProvider.Object, new List<string>(){"fdcName"});

            FieldInfo privateFdcsByProvidersInfo = typeof(FDCManager).GetField("_fdcsByProviders", BindingFlags.NonPublic | BindingFlags.Instance);

            var fdcsByProviders = (privateFdcsByProvidersInfo?.GetValue(fdcManager) as Dictionary<IFDCProvider, List<string>>);

            Assert.AreEqual((privateFdcsByProvidersInfo?.GetValue(fdcManager) as Dictionary<IFDCProvider,List<string>>).Count,1);

            // Assert
            mockProvider.Verify(d => d.StartFDCMonitor(), Times.Once);

            if (File.Exists(fdpfile))
            {
                File.Delete(fdpfile);
            }

            // Test SetPersistentFDCData
            var dummyPersistentData = new DummyPersistentData() { FDCName = "fdcName" };
            fdcManager.SetPersistentFDCData(dummyPersistentData);
            // We wait 11 seconds because the file is saved every 10 seconds
            Thread.Sleep(11000);
            // Check the persistent file has been created
            Assert.IsTrue(File.Exists(fdpfile));

            // Test SendFDC
            fdcManager.SendFDC(FDCData.MakeNew<int>("fdcName", 1, "m"));

            FieldInfo privatePersistentFDCDataInfo = typeof(FDCManager).GetField("_persistentFDCsData", BindingFlags.NonPublic | BindingFlags.Instance);

            var persistentFDCsData = (privatePersistentFDCDataInfo?.GetValue(fdcManager) as PersistentFDCsData);

            Assert.IsTrue(persistentFDCsData.FdcsStatus.ContainsKey("fdcName"));

            if (File.Exists(fdpfile))
            {
                File.Delete(fdpfile);
            }
        }
    }
}
