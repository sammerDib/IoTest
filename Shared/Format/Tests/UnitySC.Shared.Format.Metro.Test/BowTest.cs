using System;
using System.Collections.Generic;

using Helper;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.Shared.Format.Metro.Bow;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.Shared.Format.Metro.Test
{
    /// <summary>
    /// Summary description for BowTest
    /// </summary>
    [TestClass]
    public class BowTest
    {
        [TestMethod]
        public void TestBowFileTypeConsistency()
        {
            string FileNameIn = @"Wafer1.anabow";
            AssertEx.Throws<ArgumentException>(() => new MetroResult(UnitySC.Shared.Data.Enum.ResultType.DMT_CurvatureY_Front, -1, FileNameIn)); // Bad result category
            AssertEx.Throws<ArgumentException>(() => new MetroResult(UnitySC.Shared.Data.Enum.ResultType.ADC_Crown, -1, FileNameIn)); // Bad result format
            string BadFileNameIn = @"Wafer1.001";
            AssertEx.Throws<Exception>(() => new MetroResult(UnitySC.Shared.Data.Enum.ResultType.ANALYSE_Bow, -1, BadFileNameIn)); // Result Extension id not matched
        }

        [TestMethod]
        public void ReadWriteBowBlankWaferResult()
        {
            var BowResult = new BowResult();
            BowResult.Name = "BowTest1";
            BowResult.Settings.BowTargetMax = 250.Micrometers();
            BowResult.Settings.BowTargetMin = -400.Micrometers();

            BowResult.Information = "Test blank wafer result";
            var rnd = new Random();
            BowResult.State = (GlobalState)rnd.Next(0, 5);
            BowResult.Wafer = MetroHelper.CreateWafer();
            BowResult.Points = new List<MeasurePointResult>();

            BowResult.Points.Add(CreateTestBowPointResult());

            var metroResWrite = new MetroResult(Data.Enum.ResultType.ANALYSE_Bow);
            metroResWrite.MeasureResult = BowResult;

            // Test WCF Serialize
            XML.DatacontractSerialize(BowResult, "BowBlankWafer.testwcf");
            var resWCF = XML.DatacontractDeserialize<BowResult>("BowBlankWafer.testwcf");

            string error;
            Assert.IsFalse(metroResWrite.WriteInFile(@"BowBlankWafer.xml", out error), "Bad file name extension for Bow results");

            bool bRes = metroResWrite.WriteInFile(@"BowBlankWafer.anabow", out error);
            Assert.IsTrue(string.IsNullOrEmpty(error));

            var metroRes = new MetroResult(Data.Enum.ResultType.ANALYSE_Bow);
            metroRes.ReadFromFile(@"BowBlankWafer.anabow", out error);
            Assert.IsTrue(string.IsNullOrEmpty(error));

            var resXML = metroRes.MeasureResult as BowResult;
            AssertResult(BowResult, resXML, "XML Serialize");

            AssertResult(BowResult, resWCF, "WCF Serialize");
        }

        private MeasurePointResult CreateTestBowPointResult()
        {
            var point = new BowPointResult();

            point.XPosition = 0.001;
            point.YPosition = -0.005;

            point.State = (MeasureState)MetroHelper.StaticRandom.Instance.Next(0, 4);
            int NumIdRnd = MetroHelper.StaticRandom.Instance.Next(0, 500);
            var pointData1 = new BowTotalPointData();
            pointData1.Bow = (MetroHelper.StaticRandom.Instance.Next(10, 50)).Micrometers();
            pointData1.QualityScore = MetroHelper.StaticRandom.Instance.Next(0, 100) * 0.01;

            point.Datas.Add(pointData1);
            var pointData2 = new BowTotalPointData();
            pointData2.Bow = (MetroHelper.StaticRandom.Instance.Next(19, 28)).Micrometers();
            pointData2.QualityScore = MetroHelper.StaticRandom.Instance.Next(50, 100) * 0.01;
            pointData2.IndexRepeta = 1;

            point.Datas.Add(pointData2);

            var pointData3 = new BowTotalPointData();
            pointData3.Bow = (MetroHelper.StaticRandom.Instance.Next(22, 26)).Micrometers();
            pointData3.QualityScore = MetroHelper.StaticRandom.Instance.Next(80, 100) * 0.01;
            pointData3.IndexRepeta = 2;

            point.Datas.Add(pointData3);
            return point;
        }

        private void AssertResult(BowResult expected, BowResult actual, string testName)
        {
            Assert.AreEqual(expected.Name, actual.Name, $"{testName} Name");
            Assert.AreEqual(expected.Information, actual.Information, $"{testName} Information");
            Assert.AreEqual(expected.State, actual.State, $"{testName} State");

            Assert.AreEqual(expected.Settings.BowTargetMax.ToString(), actual.Settings.BowTargetMax.ToString(), $"{testName} Settings BowTargetMax");
            Assert.AreEqual(expected.Settings.BowTargetMin.ToString(), actual.Settings.BowTargetMin.ToString(), $"{testName} Settings BowTargetMin");

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
                        var expectedData = (expected.Points[i].Datas[j] as BowTotalPointData); // TODO Bow : to verify
                        var actualData = (actual.Points[i].Datas[j] as BowTotalPointData);

                        Assert.AreEqual(expectedData.IndexRepeta, actualData.IndexRepeta, $"{testName} Points[{i}].Datas[{j}].Index ");
                        Assert.AreEqual(expectedData.State, actualData.State, $"{testName} Points[{i}].Datas[{j}].State ");
                        Assert.AreEqual(expectedData.QualityScore, actualData.QualityScore, $"{testName} Points[{i}].Datas[{j}].QualityScore ");

                        Assert.AreEqual(expectedData.Bow.ToString(), actualData.Bow.ToString(), $"{testName} Points[{i}].Datas[{j}].Bow ");
                    }
                }
            }

            Assert.IsTrue(expected.Dies == null);
        }
    }
}
