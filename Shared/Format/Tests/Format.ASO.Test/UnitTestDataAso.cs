using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

using Helper;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Format.ASO;

namespace Format.ASO.Test
{
    [TestClass]
    public class UnitTestDataAso
    {
        private DateTime _dtNow = DateTime.Now;

        private List<DetailReport> _someDataDetails = new List<DetailReport>();
        private List<ClusterReport> _someDataCluster = new List<ClusterReport>();

        public UnitTestDataAso()
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

            /*float fHalf_um = _nWaferSize_mm * 500.0f;
            _dummyDefectarray.Add(new WaferDefect(new RectangleF(fHalf_um + 0.0f, fHalf_um + 0.0f, 12.0f, 12.0f), 100));
            _dummyDefectarray.Add(new WaferDefect(new RectangleF(fHalf_um + 15000.0f, fHalf_um + 1500.0f, 25.5f, 6.356f), 25));
            _dummyDefectarray.Add(new WaferDefect(new RectangleF(fHalf_um + -1500.0f, fHalf_um + -50000.0f, 35.5f, 5.356f), 25));
            _dummyDefectarray.Add(new WaferDefect(new RectangleF(fHalf_um + -45721.0f, fHalf_um + 25430.0f, 35.5f, 1655.0f), 1000));
            */
        }

        private DataAso CreateSomeWaferData()
        {
            var data = new DataAso(UnitySC.Shared.Data.Enum.ResultType.ADC_ASO);
            DateTime dtNow = _dtNow;

            // REPORT_VERSION
            data.Version = 2; //normal == 0 ?! no version field in ADCV9

            // REPORT_GLOBAL
            data.NumberOfBloc = 1;
            data.NumberOfCluster = 10;
            data.NumberOfDefect = 12;
            data.ADCRecipeFile = "TestU_ASO.adcrcp";
            data.OrientationMarkType = DataAso.OrientationMark.en_FLAT;

            // REPORT_HEADER
            data.WaferID = "6660";
            data.SlotID = 6;
            data.ToolRecipe = "UNITYSC_RECIPEID_TEST_U";
            data.LotID = "UNITYSC_LOTID_TEST_U";
            data.ToolName = "UNITYSC_TOOLNAME_TEST_U";
            data.IsSquareWafer = false;
            data.WaferSizeX_mm = 200;
            data.WaferSizeY_mm = 201;

            //REPORT_DIEGRID
            data.DieOriginX = 0.0;
            data.DieOriginY = 0.0;
            data.DiePitchX = 0.0;
            data.DiePitchY = 0.0;
            data.UseDieGridDisplay = false;

            //REPORT_DETAIL
            data.ReportDetailList.Add(new DetailReport() { Label = "Above 5 mm", Number = 1, Size = 7239.9638671875 });
            data.ReportDetailList.Add(new DetailReport() { Label = "500microns-1mm", Number = 1, Size = 868.796875 });
            data.ReportDetailList.Add(new DetailReport() { Label = "Stain1", Number = 0, Size = 0.0 });
            data.ReportDetailList.Add(new DetailReport() { Label = "1-5mm", Number = 8, Size = 18341.2523193359 });
            data.ReportDetailList.Add(new DetailReport() { Label = "Below 500  microns", Number = 0, Size = 0.0 });
            _someDataDetails.AddRange(data.ReportDetailList);

            //CLUSTER_DESCR
            int num = 0;// -- the following cluster are incomplete, check ClusterReportTest for all data info test
            data.ClusterList.Add(new ClusterReport() { ClusterNumber = ++num, UserLabel = "Above 5 mm", NumberOfDefect = 1, TotalclusterSize = 7239.9638671875, Color = System.Drawing.Color.LimeGreen });
            data.ClusterList.Add(new ClusterReport() { ClusterNumber = ++num, UserLabel = "1-5mm", NumberOfDefect = 1, TotalclusterSize = 1834.12, Color = System.Drawing.Color.Red });
            data.ClusterList.Add(new ClusterReport() { ClusterNumber = ++num, UserLabel = "1-5mm", NumberOfDefect = 1, TotalclusterSize = 1834.12, Color = System.Drawing.Color.Red });
            data.ClusterList.Add(new ClusterReport() { ClusterNumber = ++num, UserLabel = "500microns-1mm", NumberOfDefect = 1, TotalclusterSize = 868.796875, Color = System.Drawing.Color.BurlyWood });
            data.ClusterList.Add(new ClusterReport() { ClusterNumber = ++num, UserLabel = "1-5mm", NumberOfDefect = 1, TotalclusterSize = 1834.12, Color = System.Drawing.Color.Red });
            data.ClusterList.Add(new ClusterReport() { ClusterNumber = ++num, UserLabel = "1-5mm", NumberOfDefect = 3, TotalclusterSize = 1834.12, MaxClusterSize = 580.23, Color = System.Drawing.Color.Red });
            data.ClusterList.Add(new ClusterReport() { ClusterNumber = ++num, UserLabel = "1-5mm", NumberOfDefect = 1, TotalclusterSize = 1834.12, Color = System.Drawing.Color.Red });
            data.ClusterList.Add(new ClusterReport() { ClusterNumber = ++num, UserLabel = "1-5mm", NumberOfDefect = 1, TotalclusterSize = 1834.12, Color = System.Drawing.Color.Red });
            data.ClusterList.Add(new ClusterReport() { ClusterNumber = ++num, UserLabel = "1-5mm", NumberOfDefect = 1, TotalclusterSize = 1834.12, Color = System.Drawing.Color.Red });
            data.ClusterList.Add(new ClusterReport() { ClusterNumber = ++num, UserLabel = "1-5mm", NumberOfDefect = 1, TotalclusterSize = 1834.12, Color = System.Drawing.Color.Red, ThumbnailBinaryFilePath = $"{data.WaferID}\\Run_0\\CLU-{num}-bw.bmp", ThumbnailGreyLevelFilePath = $"{data.WaferID}\\Run_0\\CLU-{num}-grey.bmp" });
            _someDataCluster.AddRange(data.ClusterList);

            return data;
        }

        private DataAso CreateSomeWaferDieData()
        {
            var data = new DataAso(ResultType.ADC_ASO);
            DateTime dtNow = _dtNow;

            // REPORT_VERSION
            //data.Version = 0; //normal == 0 ?! no version field in ADCV9

            // REPORT_GLOBAL
            data.NumberOfBloc = 1;
            data.NumberOfCluster = 2;
            data.NumberOfDefect = 2;
            data.ADCRecipeFile = "TestU_DIE_ASO.adcrcp";
            data.OrientationMarkType = DataAso.OrientationMark.en_DOUBLE_FLAT;

            // REPORT_HEADER
            data.WaferID = "6660";
            data.SlotID = 6;
            data.ToolRecipe = "UNITYSC_RECIPEID_TEST_U";
            data.LotID = "UNITYSC_LOTID_TEST_U";
            data.ToolName = "UNITYSC_TOOLNAME_TEST_U";
            data.IsSquareWafer = false;
            data.WaferSizeX_mm = 200;
            data.WaferSizeY_mm = 201;

            //REPORT_DIEGRID
            data.DieOriginX = 0.0;
            data.DieOriginY = 0.0;
            data.DiePitchX = 0.0;
            data.DiePitchY = 0.0;
            data.UseDieGridDisplay = false;

            //REPORT_DETAIL
            data.ReportDetailList.Add(new DetailReport() { Label = "Above 5 mm", Number = 1, Size = 7239.9638671875 });
            data.ReportDetailList.Add(new DetailReport() { Label = "500microns-1mm", Number = 1, Size = 868.796875 });
            data.ReportDetailList.Add(new DetailReport() { Label = "Stain1", Number = 0, Size = 0.0 });
            data.ReportDetailList.Add(new DetailReport() { Label = "1-5mm", Number = 8, Size = 18341.2523193359 });
            data.ReportDetailList.Add(new DetailReport() { Label = "Below 500  microns", Number = 0, Size = 0.0 });

            //CLUSTER_DESCR
            int num = 0;// -- the following cluster are incomplete, check ClusterReportTest for all data info test
            data.ClusterList.Add(new ClusterReport() { ClusterNumber = ++num, UserLabel = "Above 5 mm", NumberOfDefect = 1, TotalclusterSize = 7239.9638671875, Color = System.Drawing.Color.LimeGreen });
            data.ClusterList.Add(new ClusterReport() { ClusterNumber = ++num, UserLabel = "1-5mm", NumberOfDefect = 1, TotalclusterSize = 1834.12, Color = System.Drawing.Color.Red });
            data.ClusterList.Add(new ClusterReport() { ClusterNumber = ++num, UserLabel = "1-5mm", NumberOfDefect = 1, TotalclusterSize = 1834.12, Color = System.Drawing.Color.Red });
            data.ClusterList.Add(new ClusterReport() { ClusterNumber = ++num, UserLabel = "500microns-1mm", NumberOfDefect = 1, TotalclusterSize = 868.796875, Color = System.Drawing.Color.BurlyWood });
            data.ClusterList.Add(new ClusterReport() { ClusterNumber = ++num, UserLabel = "1-5mm", NumberOfDefect = 1, TotalclusterSize = 1834.12, Color = System.Drawing.Color.Red });
            data.ClusterList.Add(new ClusterReport() { ClusterNumber = ++num, UserLabel = "1-5mm", NumberOfDefect = 3, TotalclusterSize = 1834.12, MaxClusterSize = 580.23, Color = System.Drawing.Color.Red });
            data.ClusterList.Add(new ClusterReport() { ClusterNumber = ++num, UserLabel = "1-5mm", NumberOfDefect = 1, TotalclusterSize = 1834.12, Color = System.Drawing.Color.Red });
            data.ClusterList.Add(new ClusterReport() { ClusterNumber = ++num, UserLabel = "1-5mm", NumberOfDefect = 1, TotalclusterSize = 1834.12, Color = System.Drawing.Color.Red });
            data.ClusterList.Add(new ClusterReport() { ClusterNumber = ++num, UserLabel = "1-5mm", NumberOfDefect = 1, TotalclusterSize = 1834.12, Color = System.Drawing.Color.Red });
            data.ClusterList.Add(new ClusterReport() { ClusterNumber = ++num, UserLabel = "1-5mm", NumberOfDefect = 1, TotalclusterSize = 1834.12, Color = System.Drawing.Color.Red, ThumbnailBinaryFilePath = $"{data.WaferID}\\Run_0\\CLU-{num}-bw.bmp", ThumbnailGreyLevelFilePath = $"{data.WaferID}\\Run_0\\CLU-{num}-grey.bmp" });

            return data;
        }

        [TestMethod]
        public void ClusterReportTest()
        {
            char _sep = ';'; //";" should be equal to DataAso.__sep

            string sClusterStringNotBlocSelected = @"CLUSTER_DESCR;1;0;4;1-5mm;1;3857.421875;3857.421875;228884;242963;3857.422;2410.891;µm;2338;596;2324;581;39;24;41570\Run_9\CLU-1-grey.bmp;41570\Run_9\CLU-1-bw.bmp;0;0;0;Pink;0;0;1-5mm;Length;0;Area|454;Length|172.904760306439;AbsolutePosition|{X=78884.34,Y=92963.93,Width=3857.422,Height=2410.891};RealDiameter|4548.85668354603;RealHeight|2410.890625;RealWidth|3857.421875;BlobAverageGreyLevel|95.8325991189427;SizingType|ByLength;DefectMaxSize|3857.421875;DefectTotalSize|3857.421875;Layer|Reflectivity";
            String[] sTab0 = sClusterStringNotBlocSelected.Split(_sep);
            ClusterReport Cluster0 = new ClusterReport();
            Assert.IsFalse(Cluster0.ParseCluster(sTab0));

            string sClusterString = @"CLUSTER_DESCR;1;104;104;1-5mm;1;3857.421875;3857.421875;228884;242963;3857.422;2410.891;µm;2338;596;2324;581;39;24;41570\Run_9\CLU-1-grey.bmp;41570\Run_9\CLU-1-bw.bmp;0;0;0;Pink;0;0;1-5mm;Length;0;Area|454;Length|172.904760306439;AbsolutePosition|{X=78884.34,Y=92963.93,Width=3857.422,Height=2410.891};RealDiameter|4548.85668354603;RealHeight|2410.890625;RealWidth|3857.421875;BlobAverageGreyLevel|95.8325991189427;SizingType|ByLength;DefectMaxSize|3857.421875;DefectTotalSize|3857.421875;Layer|Reflectivity";
            String[] sTab = sClusterString.Split(_sep);
            ClusterReport Cluster = new ClusterReport();
            Assert.IsTrue(Cluster.ParseCluster(sTab));

            Assert.AreEqual(104, Cluster.BlocNumber);
            Assert.AreEqual(104, Cluster.BlocSelected);
            Assert.AreEqual(1, Cluster.ClusterNumber);
            Assert.AreEqual("Pink", Cluster.Color.Name);
            Assert.AreEqual("1-5mm", Cluster.CustomerReportLabel);
            Assert.IsFalse(Cluster.IsKillingDefect);
            Assert.AreEqual("Length", Cluster.ReportTypeForSize);
            Assert.AreEqual("µm", Cluster.UnitUsed);
            Assert.AreEqual(@"41570\Run_9\CLU-1-bw.bmp", Cluster.ThumbnailBinaryFilePath);
            Assert.AreEqual(@"41570\Run_9\CLU-1-grey.bmp", Cluster.ThumbnailGreyLevelFilePath);
            Assert.AreEqual(3857.421875, Cluster.TotalclusterSize);
            Assert.AreEqual(3857.421875, Cluster.MaxClusterSize);

            Assert.AreEqual(39, Cluster.PixelSizeX);
            Assert.AreEqual(24, Cluster.PixelSizeY);
            Assert.AreEqual(2338, Cluster.PixelPositionX);
            Assert.AreEqual(596, Cluster.PixelPositionY);
            Assert.AreEqual(1, Cluster.NumberOfDefect);

            Assert.AreEqual(3857.422, Cluster.MicronSizeX);
            Assert.AreEqual(2410.891, Cluster.MicronSizeY);
            Assert.AreEqual(228884, Cluster.MicronPositionX);
            Assert.AreEqual(242963, Cluster.MicronPositionY);

            Assert.AreEqual(11, Cluster.CharacFeatures.Count);
            string[] caracKeys = new string[]
            {
                "Area",
                "Length",
                "AbsolutePosition",
                "RealDiameter",
                "RealHeight",
                "RealWidth",
                "BlobAverageGreyLevel",
                "SizingType",
                "DefectMaxSize",
                "DefectTotalSize",
                "Layer"
            };
            string[] caracValues = new string[]
            {
                "454",
                "172.904760306439",
                "{X=78884.34,Y=92963.93,Width=3857.422,Height=2410.891}",
                "4548.85668354603",
                "2410.890625",
                "3857.421875",
                "95.8325991189427",
                "ByLength",
                "3857.421875",
                "3857.421875",
                "Reflectivity"
            };
            int n = 0;
            foreach (var kvp in Cluster.CharacFeatures)
            {
                Assert.AreEqual(caracKeys[n], kvp.Key);
                Assert.AreEqual(caracValues[n], kvp.Value);
                n++;
            }

            string sClusterStringMulti = @"CLUSTER_DESCR;5;0;0;1-5mm;3;4918.21875;4532.4765625;50671;101010;1639.406;1639.406;µm;556;2023;542;2009;17;16;41570\Run_9\CLU-5-grey.bmp;41570\Run_9\CLU-5-bw.bmp;0;0;0;Pink;0;0;1-5mm;Length;0;Area|188;Length|90.7124911544281;AbsolutePosition|{X=-99617.99,Y=-50435.83,Width=3375.25,Height=4532.477};RealDiameter|5651.16415901289;RealHeight|4532.4765625;RealWidth|3375.25;BlobAverageGreyLevel|104.744680851064;SizingType|ByLength;DefectMaxSize|4532.4765625;DefectTotalSize|4918.21875;Layer|Reflectivity";
            ClusterReport ClusterMulti = new ClusterReport();
            String[] sTabMulti = sClusterStringMulti.Split(_sep);
            Assert.IsTrue(ClusterMulti.ParseCluster(sTabMulti));
            Assert.AreEqual(5, ClusterMulti.ClusterNumber);
            Assert.AreEqual(3, ClusterMulti.NumberOfDefect);
            Assert.AreEqual(4918.21875, ClusterMulti.TotalclusterSize);
            Assert.AreEqual(4532.4765625, ClusterMulti.MaxClusterSize);

            string sClusterStringDie = @"CLUSTER_DESCR;168;0;0;Below 500um;1;60;60;51635;53807;60;15.6001;µm;1599;1435;1598;1435;1;0;01237\Run_0\CLU-168-grey.bmp;01237\Run_0\CLU-168-bw.bmp;10;6;4;Yellow;-2;-1;Below 500microns;Length;0;RoiID|4;Area|592;Length|52.8110373729676;AbsolutePosition|{X=1635.6,Y=3807.6,Width=60,Height=15.6001};BlobAverageGreyLevel|250.743243243243;ClusterMaxGreyLevel|255;SizingType|ByLength;DefectMaxSize|60;DefectTotalSize|60;Layer|BF2D-Die";
            String[] sTabDie = sClusterStringDie.Split(_sep);
            ClusterReport ClusterDie = new ClusterReport();
            Assert.IsTrue(ClusterDie.ParseCluster(sTabDie));
            Assert.AreEqual(168, ClusterDie.ClusterNumber);
            Assert.AreEqual("Below 500um", ClusterDie.UserLabel);
            Assert.AreEqual(1, ClusterDie.NumberOfDefect);
            Assert.AreEqual(10, ClusterDie.SrcImageMosaic_Column);
            Assert.AreEqual(6, ClusterDie.SrcImageMosaic_Line);
            Assert.AreEqual(-2, ClusterDie.SrcImageMosaic_DieX);
            Assert.AreEqual(-1, ClusterDie.SrcImageMosaic_DieY);
            Assert.AreEqual(4, ClusterDie.SrcImageMosaic_VirtualBloc);
            Assert.AreEqual(10, ClusterDie.CharacFeatures.Count);
            Assert.AreEqual(60.0, ClusterDie.MaxClusterSize);
        }

        [TestMethod]
        public void TestASOFileRead_NoExist()
        {
            string FileNameIn = @"WaferASO_NoExist.aso";
            var dataAso = new DataAso(UnitySC.Shared.Data.Enum.ResultType.ADC_ASO);
            Assert.IsFalse(dataAso.ReadFromFile(FileNameIn, out string smsgerror));
            Assert.AreEqual($"This file path {{{FileNameIn}}} doesn't exist !", smsgerror);
        }

        [TestMethod]
        public void TesASOFileTypeConsistency()
        {
            string FileNameIn = @"Wafer1.aso";
            AssertEx.Throws<ArgumentException>(() => new DataAso(UnitySC.Shared.Data.Enum.ResultType.DMT_CurvatureY_Front, -1, FileNameIn)); // Bad result category
            AssertEx.Throws<ArgumentException>(() => new DataAso(UnitySC.Shared.Data.Enum.ResultType.ADC_Crown, -1, FileNameIn)); // Bad result format
            string BadFileNameIn = @"Wafer1.001";
            AssertEx.Throws<Exception>(() => new DataAso(UnitySC.Shared.Data.Enum.ResultType.ADC_ASO, -1, BadFileNameIn)); // Result Extension id not matched
        }

        [TestMethod]
        public void TestASOfFileReadWrite()
        {
            string FileNameIn = @"Wafer1.aso";
            string FileNameOut = @"Wafer1-2.aso";

            // Test that read and write Klarf data coming from a global wafer inspection
            DataAso Asotowrite = CreateSomeWaferData();
            Assert.IsTrue(Asotowrite.WriteInFile(FileNameIn, out string sErrMsg1));

            DataAso Asoread = new DataAso(UnitySC.Shared.Data.Enum.ResultType.ADC_ASO);
            Assert.IsTrue(Asoread.ReadFromFile(FileNameIn, out string sErrMsg2));
            Assert.AreEqual(sErrMsg2, string.Empty);
            Assert.AreEqual(2, Asoread.Version);

            // you could also compare Asotowrite and asoread
            Assert.IsTrue(Asoread.WriteInFile(FileNameOut, out string sErrMsg3));

            AssertFileEx.AreContentEqual(FileNameIn, FileNameOut);
            File.Delete(FileNameIn);
            File.Delete(FileNameOut);

            Assert.IsTrue(_someDataDetails.Count == Asoread.ReportDetailList.Count);
            for (int i = 0; i < _someDataDetails.Count; i++)
            {
                Assert.AreEqual(_someDataDetails[i].Label, Asoread.ReportDetailList[i].Label);
                Assert.AreEqual(_someDataDetails[i].Number, Asoread.ReportDetailList[i].Number);
                Assert.AreEqual(_someDataDetails[i].Size, Asoread.ReportDetailList[i].Size);
            }

            Assert.IsTrue(_someDataCluster.Count == Asoread.ClusterList.Count);
            for (int i = 0; i < _someDataCluster.Count; i++)
            {
                Assert.AreEqual(_someDataCluster[i].ClusterNumber, Asoread.ClusterList[i].ClusterNumber);
                Assert.AreEqual(_someDataCluster[i].UserLabel, Asoread.ClusterList[i].UserLabel);
                Assert.AreEqual(_someDataCluster[i].NumberOfDefect, Asoread.ClusterList[i].NumberOfDefect);
                Assert.AreEqual(_someDataCluster[i].TotalclusterSize, Asoread.ClusterList[i].TotalclusterSize);
                Assert.AreEqual(_someDataCluster[i].MaxClusterSize, Asoread.ClusterList[i].MaxClusterSize);
                Assert.AreEqual(_someDataCluster[i].Color, Asoread.ClusterList[i].Color);
                if (String.IsNullOrEmpty(_someDataCluster[i].ThumbnailBinaryFilePath))
                    Assert.IsTrue(String.IsNullOrEmpty(Asoread.ClusterList[i].ThumbnailBinaryFilePath));
                else
                    Assert.AreEqual(_someDataCluster[i].ThumbnailBinaryFilePath, Asoread.ClusterList[i].ThumbnailBinaryFilePath);
                if (String.IsNullOrEmpty(_someDataCluster[i].ThumbnailGreyLevelFilePath))
                    Assert.IsTrue(String.IsNullOrEmpty(Asoread.ClusterList[i].ThumbnailGreyLevelFilePath));
                else
                    Assert.AreEqual(_someDataCluster[i].ThumbnailGreyLevelFilePath, Asoread.ClusterList[i].ThumbnailGreyLevelFilePath);
            }
        }

        [TestMethod]
        public void TestASOGetColorCategory()
        {
            // Test that we can retrive category color from data
            DataAso AsoDt = CreateSomeWaferData();
            AsoDt.InternalTableToUpdate(null);

            Assert.AreEqual(System.Drawing.Color.Transparent, AsoDt.GetColorCategory("Unknown Category"));

            Assert.AreEqual(System.Drawing.Color.LimeGreen, AsoDt.GetColorCategory("Above 5 mm"));
            Assert.AreEqual(System.Drawing.Color.BurlyWood, AsoDt.GetColorCategory("500microns-1mm"));
            Assert.AreEqual(System.Drawing.Color.Red, AsoDt.GetColorCategory("1-5mm"));

            // defect exists in report detail but not present in file hence return default color
            Assert.AreEqual(System.Drawing.Color.Transparent, AsoDt.GetColorCategory("Below 500  microns"));
        }
    }
}
