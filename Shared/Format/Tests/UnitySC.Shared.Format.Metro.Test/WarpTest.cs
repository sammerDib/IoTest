using System;
using System.Collections.Generic;
using System.Linq;

using Helper;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.Shared.Format.Metro.Warp;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Format.Metro.Test
{
    /// <summary>
    /// Summary description for WarpTest
    /// </summary>
    [TestClass]
    public class WarpTest
    {
        [TestMethod]
        public void TestWarpFileTypeConsistency()
        {
            string FileNameIn = @"Wafer1.anawarp";
            AssertEx.Throws<ArgumentException>(() => new MetroResult(UnitySC.Shared.Data.Enum.ResultType.DMT_CurvatureY_Front, -1, FileNameIn)); // Bad result category
            AssertEx.Throws<ArgumentException>(() => new MetroResult(UnitySC.Shared.Data.Enum.ResultType.ADC_Crown, -1, FileNameIn)); // Bad result format
            string BadFileNameIn = @"Wafer1.001";
            AssertEx.Throws<Exception>(() => new MetroResult(UnitySC.Shared.Data.Enum.ResultType.ANALYSE_Warp, -1, BadFileNameIn)); // Result Extension id not matched
        }

        [TestMethod]
        public void ReadWriteWarpBlankWaferResult()
        {
            var WarpResult = new WarpResult();
            WarpResult.Name = "WarpTest1";
            WarpResult.Settings.WarpMax = 64.Micrometers();

            WarpResult.Information = "Test blank wafer result";
            var rnd = new Random();
            WarpResult.State = (GlobalState)rnd.Next(0, 5);
            WarpResult.Wafer = MetroHelper.CreateWafer();
            WarpResult.Points = new List<MeasurePointResult>();

            WarpResult.Points.Add(CreateTestWarpPointResult());
            WarpResult.Points.Add(CreateTestWarpPointResult());
            WarpResult.Points.Add(CreateTestWarpPointResult());
            WarpResult.Points.Add(CreateTestWarpPointResult());
            WarpResult.Points.Add(CreateTestWarpPointResult());
            WarpResult.Points.Add(CreateTestWarpPointResult());
            WarpResult.Points.Add(CreateTestWarpPointResult());
            WarpResult.Points.Add(CreateTestWarpPointResult());
            WarpResult.Points.Add(CreateTestWarpPointResult());
            WarpResult.Points.Add(CreateTestWarpPointResult());

            var list = WarpResult.Points.OfType<WarpPointResult>().SelectMany(x => x.Datas.OfType<WarpPointData>()).ToList().Select(y => y.RPD.Micrometers).ToList();
            double rpdmax = list.Max();
            double rpdmin = list.Min();
            WarpResult.WarpWaferResults.Add(new Length(rpdmax - rpdmin, LengthUnit.Micrometer));
            WarpResult.WarpWaferResults.Add(new Length(146, LengthUnit.Micrometer));

            var metroResWrite = new MetroResult(Data.Enum.ResultType.ANALYSE_Warp);
            metroResWrite.MeasureResult = WarpResult;

            // Test WCF Serialize
            XML.DatacontractSerialize(WarpResult, "WarpBlankWafer.testwcf");
            var resWCF = XML.DatacontractDeserialize<WarpResult>("WarpBlankWafer.testwcf");

            string error;
            Assert.IsFalse(metroResWrite.WriteInFile(@"WarpBlankWafer.xml", out error), "Bad file name extension for Warp results");

            bool bRes = metroResWrite.WriteInFile(@"WarpBlankWafer.anawarp", out error);
            Assert.IsTrue(string.IsNullOrEmpty(error));

            var metroRes = new MetroResult(Data.Enum.ResultType.ANALYSE_Warp);
            metroRes.ReadFromFile(@"WarpBlankWafer.anawarp", out error);
            Assert.IsTrue(string.IsNullOrEmpty(error));

            var resXML = metroRes.MeasureResult as WarpResult;
            AssertResult(WarpResult, resXML, "XML Serialize");

            AssertResult(WarpResult, resWCF, "WCF Serialize");
        }

        private MeasurePointResult CreateTestWarpPointResult()
        {
            var point = new WarpPointResult();

            point.XPosition = (MetroHelper.StaticRandom.Instance.Next(0, 3000) * 0.1) - 150;
            point.YPosition = (MetroHelper.StaticRandom.Instance.Next(0, 3000) * 0.1) - 150;

            point.State = (MeasureState)MetroHelper.StaticRandom.Instance.Next(0, 4);
            var pointData1 = new WarpPointData();
            pointData1.RPD = (MetroHelper.StaticRandom.Instance.Next(-15000, 15000) * 0.01).Micrometers();
            pointData1.QualityScore = MetroHelper.StaticRandom.Instance.Next(0, 100) * 0.01;

            point.Datas.Add(pointData1);
            var pointData2 = new WarpPointData();

            pointData2.RPD = (MetroHelper.StaticRandom.Instance.Next(50, 150) * pointData1.RPD.Micrometers * 0.01).Micrometers();
            pointData2.QualityScore = MetroHelper.StaticRandom.Instance.Next(0, 100) * 0.01;
            pointData2.IndexRepeta = 1;

            point.Datas.Add(pointData2);
            return point;
        }

        private void AssertResult(WarpResult expected, WarpResult actual, string testName)
        {
            Assert.AreEqual(expected.Name, actual.Name, $"{testName} Name");
            Assert.AreEqual(expected.Information, actual.Information, $"{testName} Information");
            Assert.AreEqual(expected.State, actual.State, $"{testName} State");

            Assert.AreEqual(expected.Settings.WarpMax.ToString(), actual.Settings.WarpMax.ToString(), $"{testName} Settings WarpMax");

            Assert.AreEqual(expected.Wafer.ToString(), actual.Wafer.ToString(), $"{testName} Wafer");
            MetroHelper.AreEqualAutomInfo(expected.AutomationInfo, actual.AutomationInfo, testName);

            if (expected.Points != null)
            {
                Assert.AreEqual(expected.Points.Count, actual.Points.Count, $"{testName} Points.Count");
                for (int i = 0; i < expected.Points.Count; i++)
                {
                    Assert.AreEqual(expected.Points[i].XPosition, actual.Points[i].XPosition, $"{testName} Points[{i}].XPosition");
                    Assert.AreEqual(expected.Points[i].YPosition, actual.Points[i].YPosition, $"{testName} Points[{i}].YPosition");
                    Assert.AreEqual(expected.Points[i].State, actual.Points[i].State, $"{testName} Points[{i}].State");

                    Assert.AreEqual(expected.Points[i].Datas.Count, actual.Points[i].Datas.Count, $"{testName} Points[i].Datas.Count");
                    for (int j = 0; j < expected.Points[i].Datas.Count; j++)
                    {
                        var expectedData = (expected.Points[i].Datas[j] as WarpPointData);
                        var actualData = (actual.Points[i].Datas[j] as WarpPointData);

                        Assert.AreEqual(expectedData.IndexRepeta, actualData.IndexRepeta, $"{testName} Points[{i}].Datas[{j}].IndexRepeta ");
                        Assert.AreEqual(expectedData.State, actualData.State, $"{testName} Points[{i}].Datas[{j}].State ");
                        Assert.AreEqual(expectedData.QualityScore, actualData.QualityScore, $"{testName} Points[{i}].Datas[{j}].QualityScore ");

                        Assert.AreEqual(expectedData.RPD.ToString(), actualData.RPD.ToString(), $"{testName} Points[{i}].Datas[{j}].RPD ");
                    }
                }
            }

            Assert.IsTrue(actual.Dies == null || actual.Dies.Count == 0, "No dies warp allowed");
            Assert.IsNotNull(actual.WarpWaferResults, "WarpWaferResults should be not null");
            Assert.IsTrue(actual.WarpWaferResults.Count > 0, "WarpWaferResults should have some results");

            for (int i = 0; i < expected.WarpWaferResults.Count; i++)
            {
                Assert.AreEqual(expected.WarpWaferResults[i].ToString(), actual.WarpWaferResults[i].ToString(), $"{testName} WarpWaferResults[{i}]");
            }
        }
    }
}
