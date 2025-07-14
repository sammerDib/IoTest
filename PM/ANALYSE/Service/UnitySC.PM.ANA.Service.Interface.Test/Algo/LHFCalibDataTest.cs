using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Test.Algo
{
    [TestClass]
    public class LHFCalibDataTest
    {
        [TestMethod]
        public void LHF_CalibData_XML_Check()
        {
            var rand = new Random();

            var lhfCalib = new LiseHFCalibrationData();
            lhfCalib.User = "RTi";
            lhfCalib.CreationDate = DateTime.Now;

            Assert.AreEqual(LiseHFCalibrationData.CurrentFileVersion, lhfCalib.FileVersion);

            // Integration Times
            int basecount = 50000;
            lhfCalib.IntegrationTimes.Add(new LiseHFObjectiveIntegrationTimeCalibration()
            {
                Date = DateTime.Now - TimeSpan.FromMinutes(125.5),
                ObjectiveDeviceId = "ID-5XNIR01",
                StandardFilterIntegrationTime_ms = 2.05,  StandardFilterBaseCount = rand.Next((int)(0.95 * basecount), (int)(1.05 * basecount)),
                LowIllumFilterIntegrationTime_ms = 24.55, LowIllumFilterBaseCount = rand.Next((int)(0.95 * basecount), (int)(1.05 * basecount)),
            });
            lhfCalib.IntegrationTimes.Add(new LiseHFObjectiveIntegrationTimeCalibration()
            {
                Date = DateTime.Now - TimeSpan.FromMinutes(35),
                ObjectiveDeviceId = "ID-10XNIR01",
                StandardFilterIntegrationTime_ms = 1.55, StandardFilterBaseCount = rand.Next((int)(0.95 * basecount), (int)(1.05 * basecount)),
                LowIllumFilterIntegrationTime_ms = 7.55, LowIllumFilterBaseCount = rand.Next((int)(0.95 * basecount), (int)(1.05 * basecount)),
            });
            lhfCalib.IntegrationTimes.Add(new LiseHFObjectiveIntegrationTimeCalibration()
            {
                Date = DateTime.Now - TimeSpan.FromMinutes(125.5),
                ObjectiveDeviceId = "ID-20XNIR01",
                StandardFilterIntegrationTime_ms = 6.00, StandardFilterBaseCount = rand.Next((int)(0.95 * basecount), (int)(1.05 * basecount)),
                LowIllumFilterIntegrationTime_ms = 8.70, LowIllumFilterBaseCount = rand.Next((int)(0.95 * basecount), (int)(1.05 * basecount)),
            });

            lhfCalib.IntegrationTimes.Add(new LiseHFObjectiveIntegrationTimeCalibration()
            {
                Date = DateTime.Now - TimeSpan.FromDays(3.23),
                ObjectiveDeviceId = "ID-2XVIS01",
                StandardFilterIntegrationTime_ms = 0.72, StandardFilterBaseCount = rand.Next((int)(0.95 * basecount), (int)(1.05 * basecount)),
                LowIllumFilterIntegrationTime_ms = 4.03, LowIllumFilterBaseCount = rand.Next((int)(0.95 * basecount), (int)(1.05 * basecount)),
            });
            lhfCalib.IntegrationTimes.Add(new LiseHFObjectiveIntegrationTimeCalibration()
            {
                Date = DateTime.Now - TimeSpan.FromDays(3.23),
                ObjectiveDeviceId = "ID-50XVIS01",
                StandardFilterIntegrationTime_ms = 20.05, StandardFilterBaseCount = rand.Next((int)(0.95 * basecount), (int)(1.05 * basecount)),
                LowIllumFilterIntegrationTime_ms = 150.80, LowIllumFilterBaseCount = rand.Next((int)(0.95 * basecount), (int)(1.05 * basecount)),
            });


            // Spot Position

            lhfCalib.SpotPositions.Add(new LiseHFObjectiveSpotCalibration()
            {
                Date = DateTime.Now - TimeSpan.FromMinutes(25.5),
                ObjectiveDeviceId = "ID-5XNIR01",
                PixelSizeX = 2.12.Micrometers(),
                PixelSizeY = 2.12.Micrometers(),
                XOffset = 5.3.Nanometers(),
                YOffset = 1.2356.Micrometers(),
                CamExposureTime_ms = 12.2
            });
            lhfCalib.SpotPositions.Add(new LiseHFObjectiveSpotCalibration()
            {
                Date = DateTime.Now - TimeSpan.FromMinutes(25.5),
                ObjectiveDeviceId = "ID-10XNIR01",
                PixelSizeX = 1.063829.Micrometers(),
                PixelSizeY = 1.063829.Micrometers(),
                XOffset = 0.856.Micrometers(),
                YOffset = 1.2356.Micrometers(),
                CamExposureTime_ms = 15.3
            });
            lhfCalib.SpotPositions.Add(new LiseHFObjectiveSpotCalibration()
            {
                Date = DateTime.Now - TimeSpan.FromMinutes(25.5),
                ObjectiveDeviceId = "ID-20XNIR01",
                PixelSizeX = 0.5.Micrometers(),
                PixelSizeY = 0.5.Micrometers(),
                XOffset = 80.233.Nanometers(),
                YOffset = -10.98.Nanometers(),
                CamExposureTime_ms = 16.4
            });

            lhfCalib.SpotPositions.Add(new LiseHFObjectiveSpotCalibration()
            {
                Date = DateTime.Now - TimeSpan.FromDays(7.35),
                ObjectiveDeviceId = "ID-2XVIS01",
                PixelSizeX = 5.31245.Micrometers(),
                PixelSizeY = 5.31245.Micrometers(),
                XOffset = -8.361.Micrometers(),
                YOffset = -1.98.Micrometers(),
                CamExposureTime_ms = 5.5
            });
            lhfCalib.SpotPositions.Add(new LiseHFObjectiveSpotCalibration()
            {
                Date = DateTime.Now - TimeSpan.FromDays(7.35),
                ObjectiveDeviceId = "ID-50XVIS01",
                PixelSizeX = 0.213675.Micrometers(),
                PixelSizeY = 0.213675.Micrometers(),
                XOffset = -2.45.Nanometers(),
                YOffset = 9.564.Nanometers(),
                CamExposureTime_ms = 27.6
            });


            XML.Serialize(lhfCalib, "lhfCalib.xml");
           var res = XML.Deserialize<LiseHFCalibrationData>("lhfCalib.xml");

            Assert.IsNotNull(res);
            Assert.AreEqual(lhfCalib.FileVersion, res.FileVersion);
            Assert.AreEqual(lhfCalib.User,res.User);
            Assert.AreEqual(lhfCalib.CreationDate, res.CreationDate);
         
            Assert.AreEqual(lhfCalib.IntegrationTimes.Count, res.IntegrationTimes.Count);
            for (int i = 0; i < res.IntegrationTimes.Count; i++)
            {
                Assert.AreEqual(lhfCalib.IntegrationTimes[i].ObjectiveDeviceId, res.IntegrationTimes[i].ObjectiveDeviceId);
                Assert.AreEqual(lhfCalib.IntegrationTimes[i].StandardFilterIntegrationTime_ms, res.IntegrationTimes[i].StandardFilterIntegrationTime_ms);
                Assert.AreEqual(lhfCalib.IntegrationTimes[i].LowIllumFilterIntegrationTime_ms, res.IntegrationTimes[i].LowIllumFilterIntegrationTime_ms);
                Assert.AreEqual(lhfCalib.IntegrationTimes[i].StandardFilterBaseCount, res.IntegrationTimes[i].StandardFilterBaseCount);
                Assert.AreEqual(lhfCalib.IntegrationTimes[i].LowIllumFilterBaseCount, res.IntegrationTimes[i].LowIllumFilterBaseCount);
            }

            Assert.AreEqual(lhfCalib.SpotPositions.Count, res.SpotPositions.Count);
            for (int i = 0; i < res.SpotPositions.Count; i++)
            {
                Assert.AreEqual(lhfCalib.SpotPositions[i].ObjectiveDeviceId, res.SpotPositions[i].ObjectiveDeviceId);
                Assert.AreEqual(lhfCalib.SpotPositions[i].PixelSizeX, res.SpotPositions[i].PixelSizeX);
                Assert.AreEqual(lhfCalib.SpotPositions[i].PixelSizeY, res.SpotPositions[i].PixelSizeY);
                Assert.AreEqual(lhfCalib.SpotPositions[i].XOffset, res.SpotPositions[i].XOffset);
                Assert.AreEqual(lhfCalib.SpotPositions[i].YOffset, res.SpotPositions[i].YOffset);
                Assert.AreEqual(lhfCalib.SpotPositions[i].CamExposureTime_ms, res.SpotPositions[i].CamExposureTime_ms);
            }
        }
    }
}
