using System;
using System.Collections.Generic;
using System.Drawing;

using Helper;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Format.Base;
using UnitySC.Shared.Format.Factory;
using UnitySC.Shared.Tools;
using System.Linq;

namespace Format.Factory.Test
{
    [TestClass]
    public class UnitTestFactory
    {
        public static bool IsRegisteringDone = false;
        internal Dictionary<ResultType, bool> FormatImplementedMap = new Dictionary<ResultType, bool>();

        public UnitTestFactory()
        {
            if (!IsRegisteringDone)
            {
                ClassLocator.Default.Register(typeof(IResultDataFactory), typeof(ResultDataFactory), true);
                IsRegisteringDone = true;
            }

            FormatImplementedMap.Add(ResultType.NotDefined, false);

            // Format Implemented
            FormatImplementedMap.Add(ResultType.ADC_Klarf, true);
            FormatImplementedMap.Add(ResultType.ADC_ASE, false);
            FormatImplementedMap.Add(ResultType.ADC_ASO, true);
            FormatImplementedMap.Add(ResultType.ADC_Haze, true);

            FormatImplementedMap.Add(ResultType.ANALYSE_TSV, true);
            FormatImplementedMap.Add(ResultType.ANALYSE_NanoTopo, true);
            FormatImplementedMap.Add(ResultType.ANALYSE_Thickness, true);
            FormatImplementedMap.Add(ResultType.ANALYSE_Topography, true);
            FormatImplementedMap.Add(ResultType.ANALYSE_Step, true);
            FormatImplementedMap.Add(ResultType.ANALYSE_EdgeTrim, true);
            FormatImplementedMap.Add(ResultType.ANALYSE_Trench, true);
            FormatImplementedMap.Add(ResultType.ANALYSE_Pillar, true);
            FormatImplementedMap.Add(ResultType.ANALYSE_PeriodicStructure, true);
            FormatImplementedMap.Add(ResultType.ANALYSE_Bow, true);
            FormatImplementedMap.Add(ResultType.ANALYSE_Warp, true);
            FormatImplementedMap.Add(ResultType.ANALYSE_XYCalibration, true);

            // FORMAT TO DO
            FormatImplementedMap.Add(ResultType.ADC_DFHaze, false);
            FormatImplementedMap.Add(ResultType.ADC_Crown, false);
            FormatImplementedMap.Add(ResultType.ADC_YieldMap, false);
            FormatImplementedMap.Add(ResultType.ADC_EyeEdge, false);
            FormatImplementedMap.Add(ResultType.ADC_GlobalTopo, false);
            FormatImplementedMap.Add(ResultType.ADC_HeightMes, false);

            FormatImplementedMap.Add(ResultType.ANALYSE_Overlay, false);
            FormatImplementedMap.Add(ResultType.ANALYSE_CD, false);
            FormatImplementedMap.Add(ResultType.ANALYSE_EBR, false);
   
            FormatImplementedMap.Add(ResultType.ANALYSE_Roughness, false);
        }

        [TestMethod]
        public void TestFormatCreate()
        {
            IResultDataFactory factory = ClassLocator.Default.GetInstance<IResultDataFactory>();

            IResultDataObject obj;
            Random rnd = new Random();
            long lDBResiD = (long)rnd.Next(0, Int32.MaxValue);

            // When all format will be created
            foreach (var restype in (ResultType[])Enum.GetValues(typeof(ResultType)))
            {
                if (restype.GetResultCategory() == ResultCategory.Result)
                {
                    bool bTestNotImplement = true;
                    if (FormatImplementedMap.ContainsKey(restype))
                    {
                        if (FormatImplementedMap[restype])
                        {
                            Console.WriteLine("Create type <" + restype.ToString() + ">");
                            bTestNotImplement = false;
                            lDBResiD = (long)rnd.Next(0, Int32.MaxValue);
                            obj = factory.Create(restype, lDBResiD);
                            Assert.AreEqual(restype, obj.ResType, string.Format("ResType assert for {0}", restype.ToString()));
                            Assert.AreEqual(lDBResiD, obj.DBResId, string.Format("DBResId assert for {0}", restype.ToString()));
                        }
                    }
                    if (bTestNotImplement)
                    {
                        Console.WriteLine("Create NotImplemented type <" + restype.ToString() + ">");
                        AssertEx.Throws<NotImplementedException>(() => obj = factory.Create(restype, lDBResiD));
                    }
                }
            }
        }

        [TestMethod]
        public void TestFormatCreateFromFile()
        {
            IResultDataFactory factory = ClassLocator.Default.GetInstance<IResultDataFactory>();

            IResultDataObject obj;
            Random rnd = new Random();
            long lDBResiD = (long)rnd.Next(0, Int32.MaxValue);

            // scurdir = System.IO.Directory.GetCurrentDirectory();
            string sResPathBase = @"..\..\..\Samples\Test.";
            // When all format will be created
            foreach (var restype in (ResultType[])Enum.GetValues(typeof(ResultType)))
            {
                if (restype.GetResultCategory() != ResultCategory.Result)
                    continue;

                bool bTestNotImplement = true;
                string sExtension = String.Empty;
                try
                {
                    sExtension = ResultFormatExtension.GetExt(restype);
                }
                catch (ArgumentException)
                {
                    continue;
                }

                string sResPath = sResPathBase + ResultFormatExtension.GetExt(restype);
                if (FormatImplementedMap.ContainsKey(restype))
                {
                    if (FormatImplementedMap[restype])
                    {
                        bTestNotImplement = false;
                        lDBResiD = (long)rnd.Next(0, Int32.MaxValue);

                        bool bTestFileExist = System.IO.File.Exists(sResPath);
                        if (bTestFileExist)
                        {
                            Console.WriteLine("CreateFromFile type <" + restype.ToString() + "> ");

                            obj = factory.CreateFromFile(restype, lDBResiD, sResPath);
                            Assert.AreEqual((int)restype, (int)obj.ResType, string.Format("ResType assert for {0}", restype.ToString()));
                            Assert.AreEqual(lDBResiD, obj.DBResId, string.Format("DBResId assert for {0}", restype.ToString()));
                            Assert.AreEqual(sResPath, obj.ResFilePath, string.Format("ResFilePath assert for {0}", restype.ToString()));
                        }
                        else
                        {
                            Console.WriteLine("Test File is MissingFieldException for " + restype.ToString() + " : " + sResPath);
                            AssertEx.Throws<Exception>(() => obj = factory.CreateFromFile(restype, lDBResiD, sResPath));
                        }
                    }
                }
                if (bTestNotImplement)
                {
                    Console.WriteLine("CreateFromfile NotImplemented type <" + restype.ToString() + "> ");
                    AssertEx.Throws<NotImplementedException>(() => obj = factory.CreateFromFile(restype, lDBResiD, sResPath));
                }
            }
        }

        [TestMethod]
        public void TestFormatGetFormatView()
        {
            IResultDataFactory factory = ClassLocator.Default.GetInstance<IResultDataFactory>();

            IResultDisplay obj;
            Random rnd = new Random();
            long lDBResiD = (long)rnd.Next(0, Int32.MaxValue);

            // When all format will be created
            foreach (var restype in (ResultType[])Enum.GetValues(typeof(ResultType)))
            {
                if (restype.GetResultCategory() != ResultCategory.Result)
                    continue;

                if (restype.GetResultFormat() != ResultFormat.Metrology) // to remove when metro display will be implemented
                {
                    bool bTestNotImplement = true;
                    if (FormatImplementedMap.ContainsKey(restype))
                    {
                        if (FormatImplementedMap[restype])
                        {
                            bTestNotImplement = false;
                            obj = factory.GetDisplayFormat(restype);
                        }
                    }
                    if (bTestNotImplement)
                    {
                        AssertEx.Throws<NotImplementedException>(() => obj = factory.GetDisplayFormat(restype));
                    }
                }
            }
        }

        [TestMethod]
        public void TestFormatHelper()
        {
            IResultDataFactory factory = ClassLocator.Default.GetInstance<IResultDataFactory>();

            string sThumbnailPath = FormatHelper.ThumbnailPathOf(@"D:\Toto\Titi\tata\Myfile.001");
            Assert.AreEqual(@"D:\Toto\Titi\tata\LotThumbnail\Myfile_001.png", sThumbnailPath);
#if USE_ANYCPU
            string sResPath = @"..\..\..\Samples\Test.001";
#else
            string sResPath = @"..\..\..\..\Samples\Test.001";
#endif
            Assert.IsTrue(System.IO.File.Exists(sResPath));
            IResultDataObject dataobj = factory.CreateFromFile(ResultType.ADC_Klarf, 156, sResPath);
            string sThumbnailPathData = FormatHelper.ThumbnailPathOf(dataobj);
#if USE_ANYCPU
            Assert.AreEqual(@"..\..\..\Samples\LotThumbnail\Test_001.png", sThumbnailPathData);
#else
            Assert.AreEqual(@"..\..\..\..\Samples\LotThumbnail\Test_001.png", sThumbnailPathData);
#endif
        }

        //[TestMethod]
        public void TestKlarfThumbnail()
        {
            var resfmt = ResultType.ADC_Klarf;

            IResultDataFactory factory = ClassLocator.Default.GetInstance<IResultDataFactory>();
            IResultDisplay viewFormatobj = factory.GetDisplayFormat(resfmt);
            Assert.IsNotNull(viewFormatobj);

            Random rnd = new Random();
            long lDBResiD = (long)rnd.Next(0, Int32.MaxValue);
            string sResPathBase = @"..\..\..\Samples\Test.";

            string sResPath = sResPathBase + ResultFormatExtension.GetExt(resfmt);
            Assert.IsTrue(System.IO.File.Exists(sResPath));

            DefectBins DefBins = new DefectBins();
            DefBins.Add(new DefectBin() { RoughBin = 201, Label = "Def_201", Color = Color.Yellow.ToArgb() });
            DefBins.Add(new DefectBin() { RoughBin = 202, Label = "Def_202", Color = Color.Orange.ToArgb() });
            DefBins.Add(new DefectBin() { RoughBin = 203, Label = "Def_203", Color = Color.AliceBlue.ToArgb() });
            DefBins.Add(new DefectBin() { RoughBin = 204, Label = "Def_204", Color = Color.GreenYellow.ToArgb() });
            DefBins.Add(new DefectBin() { RoughBin = 205, Label = "Def_205", Color = Color.DarkGreen.ToArgb() });
            DefBins.Add(new DefectBin() { RoughBin = 207, Label = "Def_207", Color = Color.Purple.ToArgb() });

            IResultDataObject dataobj = factory.CreateFromFile(resfmt, lDBResiD, sResPath);
            List<int> RoughBinList = DefBins.RoughBinList;
            List<int> NewBinToAdd = (List<int>)(dataobj.InternalTableToUpdate((object)RoughBinList));
            Assert.AreEqual(1, NewBinToAdd.Count);
            Assert.AreEqual(206, NewBinToAdd[0]);

            DefBins.Add(new DefectBin() { RoughBin = 206, Label = "Def_206", Color = Color.Red.ToArgb() });

            SizeBins szBins = new SizeBins();
            szBins.AddBin(10000, 1000);
            szBins.AddBin(100000, 2000);
            szBins.AddBin(1000000, 5000);
            szBins.Arrange();

            AssertEx.Throws<ArgumentException>(() => viewFormatobj.GenerateThumbnailFile(dataobj));
            AssertEx.Throws<ArgumentException>(() => viewFormatobj.GenerateThumbnailFile(dataobj, DefBins));

            Assert.IsTrue(viewFormatobj.GenerateThumbnailFile(dataobj, DefBins, szBins));

            List<ResultDataStats> stat = viewFormatobj.GenerateStatisticsValues(dataobj);

            Assert.AreEqual(14, stat.Count);

            viewFormatobj.UpdateInternalDisplaySettingsPrm(DefBins, szBins);

            Bitmap bmp = viewFormatobj.DrawImage(dataobj, false, null);
            bmp.Save(sResPathBase + "_001_DRAWImage-all.png");
            bmp = viewFormatobj.DrawImage(dataobj, false, RoughBinList);
            bmp.Save(sResPathBase + "_001_DRAWImage-206.png");
            RoughBinList.Remove(201);
            bmp = viewFormatobj.DrawImage(dataobj, false, RoughBinList);
            bmp.Save(sResPathBase + "_001_DRAWImage-206-201.png");

#if DEBUG_Format001_thumbnail // need to include UnitySC.Shared.Format._001
            if (false)
            {
                RoughtBinList.Clear();
                UnitySC.Shared.Format._001.DataKlarf dataKlarfobj = dataobj as UnitySC.Shared.Format._001.DataKlarf;
                // create Pseudo die klarf
                dataKlarfobj.DiePitch = new UnitySC.Shared.Format._001.PrmPtFloat(2502.0f, 1502.0f);
                dataKlarfobj.DieOrigin = new UnitySC.Shared.Format._001.PrmPtFloat(5.0f, -10.0f); // die 0,0 position in wafer coordinates

                var ListDieIndexes = new List<KeyValuePair<int, int>>();
                dataKlarfobj.SampleTestPlan = new UnitySC.Shared.Format._001.PrmSampleTestPlan(ListDieIndexes);
                // dummy stupid sample plan creation only for test purpose
                double squaredistlimit = (double)(dataKlarfobj.SampleSize.WaferDiameter_mm - 5) * 500.0;
                squaredistlimit *= squaredistlimit;
                for (int x = -16; x <= 16; x++)
                {
                    for (int y = -30; y <= 30; y++)
                    {
                        if ((Math.Pow(x * dataKlarfobj.DiePitch.X, 2) + Math.Pow(y * dataKlarfobj.DiePitch.Y, 2)) <= squaredistlimit)
                            dataKlarfobj.SampleTestPlan.Add(x, y);
                    }
                }

                bmp = viewFormatobj.DrawImage(dataKlarfobj, true, RoughtBinList);
                bmp.Save(sResPathBase + "_001_DRAWImage-Empty-NOTCH.png");
                dataKlarfobj.SampleOrientationMarkType.Value = UnitySC.Shared.Format._001.PrmSampleOrientationMarkType.SomtType.FLAT;
                bmp = viewFormatobj.DrawImage(dataKlarfobj, true, RoughtBinList);
                bmp.Save(sResPathBase + "_001_DRAWImage-Empty-FLAT.png");
                dataKlarfobj.SampleOrientationMarkType.Value = UnitySC.Shared.Format._001.PrmSampleOrientationMarkType.SomtType.DFLAT;
                bmp = viewFormatobj.DrawImage(dataKlarfobj, true, RoughtBinList);
                bmp.Save(sResPathBase + "_001_DRAWImage-Empty-DFLAT.png");

                dataKlarfobj.IsSquaredWafer = true;
                dataKlarfobj.SquareSizemm = new UnitySC.Shared.Format._001.PrmPtFloat(47, 48);

                dataKlarfobj.SampleOrientationMarkType.Value = UnitySC.Shared.Format._001.PrmSampleOrientationMarkType.SomtType.DFLAT;
                bmp = viewFormatobj.DrawImage(dataKlarfobj, true, RoughtBinList);
                bmp.Save(sResPathBase + "_001_DRAWImage-Empty-SQUARE.png");
            }
#endif
        }

        //[TestMethod]
        public void TestAsoThumbnail()
        {
            var resfmt = ResultType.ADC_ASO;

            IResultDataFactory factory = ClassLocator.Default.GetInstance<IResultDataFactory>();
            IResultDisplay viewFormatobj = factory.GetDisplayFormat(resfmt);
            Assert.IsNotNull(viewFormatobj);

            Random rnd = new Random();
            long lDBResiD = (long)rnd.Next(0, Int32.MaxValue);
            string sResPathBase = @"..\..\..\Samples\Test.";

            string sResPath = sResPathBase + ResultFormatExtension.GetExt(resfmt);
            Assert.IsTrue(System.IO.File.Exists(sResPath));

            IResultDataObject dataobj = factory.CreateFromFile(resfmt, lDBResiD, sResPath);
            Assert.IsTrue(viewFormatobj.GenerateThumbnailFile(dataobj));

            List<ResultDataStats> stat = viewFormatobj.GenerateStatisticsValues(dataobj);
            //            Assert.AreEqual(14, stat.Count);

            List<string> categories = new List<string>(7);
            categories.Add("less than 7 microns");
            categories.Add("7 - 10 microns");
            categories.Add("10 - 50 microns");
            categories.Add("50 - 100 microns");
            categories.Add("100 - 200 microns");
            categories.Add("200 - 500 microns");
            categories.Add("more than 500 microns");

            double dDisplayFactor = 1.0;
            int nDisplayMinSize = 1;
            viewFormatobj.UpdateInternalDisplaySettingsPrm(dDisplayFactor, nDisplayMinSize);

            Bitmap bmp = viewFormatobj.DrawImage(dataobj, false, null);
            bmp.Save(sResPathBase + "_ASO_DRAWImage-all.png");
            categories.RemoveAt(0);
            bmp = viewFormatobj.DrawImage(dataobj, false, categories);
            bmp.Save(sResPathBase + "_ASO_DRAWImage-cat0.png");
            categories.RemoveAt(0);
            bmp = viewFormatobj.DrawImage(dataobj, false, categories);
            bmp.Save(sResPathBase + "_ASO_DRAWImage-cat0-1.png");

            nDisplayMinSize = 10;
            viewFormatobj.UpdateInternalDisplaySettingsPrm(dDisplayFactor, nDisplayMinSize);
            bmp = viewFormatobj.DrawImage(dataobj, false, categories);
            bmp.Save(sResPathBase + "_ASO_DRAWImage-cat0-1-minsize10.png");

            dDisplayFactor = 5.0;
            viewFormatobj.UpdateInternalDisplaySettingsPrm(dDisplayFactor, nDisplayMinSize);
            bmp = viewFormatobj.DrawImage(dataobj, false, categories);
            bmp.Save(sResPathBase + "_ASO_DRAWImage-cat0-1-factor5.png");

            dDisplayFactor = 15.0;
            viewFormatobj.UpdateInternalDisplaySettingsPrm(dDisplayFactor, nDisplayMinSize);
            bmp = viewFormatobj.DrawImage(dataobj, false, categories);
            bmp.Save(sResPathBase + "_ASO_DRAWImage-cat0-1-factor15.png");
        }

        [TestMethod]
        public void TestSizeBins()
        {
            SizeBin bintst = new SizeBin(900, 30);
            Assert.AreEqual(900, bintst.AreaMax_um, string.Format("bintst area assert"));
            Assert.AreEqual(30, bintst.Size_um, string.Format("bintst size assert"));

            List<SizeBin> lbins = new List<SizeBin>();
            lbins.Add(bintst);
            lbins.Add(new SizeBin(1500, 400));

            SizeBins szbins1 = new SizeBins();
            // Empty size bons should return a default size
            Assert.AreEqual(szbins1.DefaultSize, szbins1.GetSquareWidth(56));
            Assert.AreEqual(szbins1.DefaultSize, szbins1.GetSquareWidth(45656));
            //add some bins
            szbins1.AddBin(1000, 100);
            szbins1.AddBin(500, 10);
            szbins1.AddRange(lbins);
            szbins1.Arrange();

            Assert.AreEqual(10, szbins1.GetSquareWidth(3), string.Format("GetSquareWidth 1 "));
            Assert.AreEqual(10, szbins1.GetSquareWidth(100), string.Format("GetSquareWidth 2 "));
            Assert.AreEqual(30, szbins1.GetSquareWidth(620), string.Format("GetSquareWidth 3 "));
            Assert.AreEqual(30, szbins1.GetSquareWidth(900), string.Format("GetSquareWidth 4 "));
            Assert.AreEqual(100, szbins1.GetSquareWidth(901), string.Format("GetSquareWidth 5 "));
            Assert.AreEqual(400, szbins1.GetSquareWidth(1368), string.Format("GetSquareWidth 6 "));
            Assert.AreEqual(400, szbins1.GetSquareWidth(10568), string.Format("GetSquareWidth 7 "));
        }

        [TestMethod]
        public void TestSizeBinsSerialization()
        {
            SizeBins szbins = new SizeBins();
            szbins.AddBin(1000, 100);
            szbins.AddBin(500, 10);
            szbins.AddBin(2504, 160);
            szbins.Arrange();

#if USE_ANYCPU
            string sResPathBase = @"..\..\..\Samples\Test";
#else
            string sResPathBase = @"..\..\..\..\Samples\Test";
#endif

            string sPathSizeBins1 = sResPathBase + @"SizeBins1.xml";

            bool bSuccess = szbins.ExportToXml(sPathSizeBins1);
            Assert.IsTrue(bSuccess);

            SizeBins szbinsnull;
            AssertEx.Throws<Exception>(() => szbinsnull = SizeBins.ImportFromXml("notexistingpath.xml"));

            SizeBins szbins2 = SizeBins.ImportFromXml(sPathSizeBins1);
            Assert.IsNotNull(szbins2);

            Assert.AreEqual(10, szbins.GetSquareWidth(3), string.Format("GetSquareWidth 1 "));
            Assert.AreEqual(10, szbins.GetSquareWidth(100), string.Format("GetSquareWidth 2 "));
            Assert.AreEqual(100, szbins.GetSquareWidth(620), string.Format("GetSquareWidth 3 "));
            Assert.AreEqual(160, szbins.GetSquareWidth(2300), string.Format("GetSquareWidth 4 "));

            Assert.AreEqual(10, szbins2.GetSquareWidth(3), string.Format("GetSquareWidth Imported 1 "));
            Assert.AreEqual(10, szbins2.GetSquareWidth(100), string.Format("GetSquareWidth Imported  2 "));
            Assert.AreEqual(100, szbins2.GetSquareWidth(620), string.Format("GetSquareWidth Imported 3 "));
            Assert.AreEqual(160, szbins2.GetSquareWidth(2300), string.Format("GetSquareWidth Imported 4 "));
        }

        [TestMethod]
        public void TestDefectBin()
        {
            DefectBin bn = new DefectBin();
            bn.RoughBin = 56436;
            bn.Color = 65424;
            bn.Label = "MyDefect";

            Assert.AreEqual(56436, bn.RoughBin);
            Assert.AreEqual(65424, bn.Color);
            Assert.AreEqual("MyDefect", bn.Label);
        }

        [TestMethod]
        public void TestDefectBinsSerialization()
        {
            DefectBins DefBins = new DefectBins();
            DefBins.Add(new DefectBin() { RoughBin = 201, Label = "Def_201", Color = Color.Yellow.ToArgb() });
            DefBins.Add(new DefectBin() { RoughBin = 202, Label = "Def_202", Color = Color.Orange.ToArgb() });
            DefBins.Add(new DefectBin() { RoughBin = 203, Label = "Def_203", Color = Color.AliceBlue.ToArgb() });
            DefBins.Add(new DefectBin() { RoughBin = 204, Label = "Def_204", Color = Color.GreenYellow.ToArgb() });
            DefBins.Add(new DefectBin() { RoughBin = 205, Label = "Def_205", Color = Color.DarkGreen.ToArgb() });
            DefBins.Add(new DefectBin() { RoughBin = 207, Label = "Def_207", Color = Color.Purple.ToArgb() });

#if USE_ANYCPU
            string sResPathBase = @"..\..\..\Samples\Test";
#else
            string sResPathBase = @"..\..\..\..\Samples\Test";
#endif
            string sPathDefBins1 = sResPathBase + @"DefectRoughBins1.xml";

            bool bSuccess = DefBins.ExportToXml(sPathDefBins1);
            Assert.IsTrue(bSuccess);

            DefectBins defbinsnull;
            AssertEx.Throws<Exception>(() => defbinsnull = DefectBins.ImportFromXml("notexistingpath.xml"));

            DefectBins DefBins2 = DefectBins.ImportFromXml(sPathDefBins1);
            Assert.IsNotNull(DefBins2);

            DefectBin def204 = DefBins.GetDefectBin(204);
            Assert.AreEqual(204, def204.RoughBin);
            Assert.AreEqual("Def_204", def204.Label);
            Assert.AreEqual(Color.GreenYellow.ToArgb(), def204.Color);

            DefectBin def204_2 = DefBins2.GetDefectBin(204);
            Assert.AreEqual(204, def204_2.RoughBin);
            Assert.AreEqual("Def_204", def204_2.Label);
            Assert.AreEqual(Color.GreenYellow.ToArgb(), def204_2.Color);
        }
    }
}
