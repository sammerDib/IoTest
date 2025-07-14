using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Helper;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Format._001;

namespace Format._001.Test
{
    [TestClass]
    public class UnitTestDataKlarf
    {
        private class WaferDefect
        {
            public double Xrel; // bottom left origin defect X in wafer coordinates (unit micron)
            public double Yrel; // bottom left origin defect Y in wafer coordinates (unit micron)

            public double XSize; // defect size in microns X
            public double YSize; // defect size in microns X

            public double Area; // defcetg area in micron2
            public double Dsize;
            public int Roughbin; // defect type bin ID

            public WaferDefect(RectangleF rcmircon, int nBin)
            {
                Xrel = rcmircon.Left;
                Yrel = rcmircon.Bottom;

                XSize = rcmircon.Width;
                YSize = rcmircon.Height;

                Area = Math.Abs(XSize * YSize);
                Dsize = Math.Sqrt(Area);

                Roughbin = nBin;
            }
        }

        private class DieDefect : WaferDefect// you can do your own
        {
            public int XDieIndex; // Die index X
            public int YDieIndex; // Die index Y

            public DieDefect(RectangleF rcmircon, int nBin, int dieIndexX, int dieIndexY)
                : base(rcmircon, nBin)
            {
                XDieIndex = dieIndexX;
                YDieIndex = dieIndexY;
            }
        }

        private List<WaferDefect> _dummyDefectarray = new List<WaferDefect>();
        private List<DieDefect> _dummyDieDefectarray = new List<DieDefect>();

        private readonly string[] _sDefectTypeCategories =
        {
            "DEFECTID",
            "XREL",
            "YREL",
            "XINDEX",
            "YINDEX",
            "XSIZE",
            "YSIZE",
            "DEFECTAREA",
            "DSIZE",
            "CLASSNUMBER",
            "TEST",
            "CLUSTERNUMBER",
            "ROUGHBINNUMBER",
            "FINEBINNUMBER",
            "REVIEWSAMPLE",
            "IMAGECOUNT",
            "IMAGELIST"
        };

        private readonly string[] _sDieDefectTypeCategories =
      {
            "DEFECTID",
            "XREL",
            "YREL",
            "XINDEX",
            "YINDEX",
            "XSIZE",
            "YSIZE",
            "DEFECTAREA",
            "DSIZE",
            "CLASSNUMBER",
            "TEST",
            "CLUSTERNUMBER",
            "ROUGHBINNUMBER",
            "IMAGECOUNT",
            "IMAGELIST"
        };

        private readonly int _nWaferSize_mm = 300;
        private DateTime _dtNow = DateTime.Now;

        public UnitTestDataKlarf()
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

            float fHalf_um = _nWaferSize_mm * 500.0f;
            _dummyDefectarray.Add(new WaferDefect(new RectangleF(fHalf_um + 0.0f, fHalf_um + 0.0f, 12.0f, 12.0f), 100));
            _dummyDefectarray.Add(new WaferDefect(new RectangleF(fHalf_um + 15000.0f, fHalf_um + 1500.0f, 25.5f, 6.356f), 25));
            _dummyDefectarray.Add(new WaferDefect(new RectangleF(fHalf_um + -1500.0f, fHalf_um + -50000.0f, 35.5f, 5.356f), 25));
            _dummyDefectarray.Add(new WaferDefect(new RectangleF(fHalf_um + -45721.0f, fHalf_um + 25430.0f, 35.5f, 1655.0f), 1000));

            // die defect are expressed in die referential coordinates
            _dummyDieDefectarray.Add(new DieDefect(new RectangleF(0.0f, 0.0f, 12.0f, 12.0f), 100, 0, 0));
            _dummyDieDefectarray.Add(new DieDefect(new RectangleF(140.0f, 687.0f, 12.0f, 12.0f), 100, -5, -15));
            _dummyDieDefectarray.Add(new DieDefect(new RectangleF(140.0f, 687.0f, 25.0f, 58.0f), 254, 4, -9));
            _dummyDieDefectarray.Add(new DieDefect(new RectangleF(450.0f, 478.0f, 12.0f, 11.5f), 100, 6, 9));
            _dummyDieDefectarray.Add(new DieDefect(new RectangleF(550.0f, 480.0f, 13.0f, 10.0f), 100, 6, 9));
            _dummyDieDefectarray.Add(new DieDefect(new RectangleF(556.0f, 496.0f, 13.5f, 9.0f), 100, 6, 9));
            _dummyDieDefectarray.Add(new DieDefect(new RectangleF(258.0f, 10.0f, 130.5f, 10.0f), 25, 6, 9));
        }

        private DataKlarf CreateSomeWaferData()
        {
            DataKlarf _dataKlarf = new DataKlarf(UnitySC.Shared.Data.Enum.ResultType.ADC_Klarf);
            DateTime dtNow = _dtNow;
            _dataKlarf.FileTimeStamp = dtNow;
            _dataKlarf.ResultTimeStamp = dtNow;
            // "UNITY-SC" "ToolCategory" "ToolName"
            _dataKlarf.InspectionStationID = string.Format("\"{0}\" \"{1}\" \"{2}\"", "UNITYSC", "4See", "MyWaferTestRecipeName");

            _dataKlarf.LotID = "LotIDName";
            _dataKlarf.DeviceID = "DeviceIDName";
            _dataKlarf.WaferID = "WaferIDName";
            _dataKlarf.StepID = "StepIDname";
            _dataKlarf.SetupID = new PrmSetupId("MyADCRecipeName", dtNow);
            _dataKlarf.SlotID = 1;

            int nSampleSize_mm = _nWaferSize_mm; // wafer diameter in mm
            _dataKlarf.SampleSize = new PrmSampleSize(nSampleSize_mm);

            PrmSampleOrientationMarkType.SomtType omttype = PrmSampleOrientationMarkType.SomtType.NOTCH; // PrmSampleOrientationMarkType.somtType.FLAT PrmSampleOrientationMarkType.somtType.DFLAT
            _dataKlarf.SampleOrientationMarkType = new PrmSampleOrientationMarkType(omttype); ;
            _dataKlarf.OrientationMarkLocation = new PrmOrientationMarkValue("DOWN");
            _dataKlarf.OrientationInstructions = new PrmOrientationInstruction("FRONT"); //BACK
            _dataKlarf.SampleCenterLocation = new PrmPtFloat((float)nSampleSize_mm * 500.0f, (float)nSampleSize_mm * 500.0f); // wafer size x/y  * 2
            _dataKlarf.CoordinatesMirrored = new PrmYesNo(false);

            _dataKlarf.SampleTestPlan = new PrmSampleTestPlan(0, 0);

            _dataKlarf.AddDefectTypes(_sDefectTypeCategories); // warning defect type should be registered or you will have to declare and defined type by yourself using DataKlarf.AddDefectType(Type p_type, string p_name)

            int i = 0;
            int indexThumbnail = 0;
            foreach (WaferDefect wd in _dummyDefectarray)
            {
                PrmDefect curDefect = _dataKlarf.NewDefect();
                int nClusterNum = i++;

                AssertEx.Throws<Exception>(() => curDefect.Set("TYPETHATISNOTREGISTRED", "bad")); //def type not registered (unknown type)

                Assert.AreEqual(null, curDefect.Get("DEFECTAREA"), "NOT Null ! def type not already inserted"); //def type not already inserted

                curDefect.Set("XREL", wd.Xrel);
                Assert.AreEqual(wd.Xrel, curDefect.Get("XREL"), "XREL");

                var myres = curDefect.Get("XREL");

                dynamic myresdouble = curDefect.Get("XREL");
                // inclure reference à Microsoft.CSharp pour le runtime binder
                double d = myresdouble;
                string str = myresdouble.ToString();

                // Type mytyp = (curDefect._deflist.List.Find(x => x.Name == "XREL").Type);
                // var totodbl = Convert.ChangeType(myres, mytyp);

                curDefect.Set("YREL", wd.Yrel);
                Assert.AreEqual(wd.Yrel, curDefect.Get("YREL"), "YREL");

                curDefect.Set("XSIZE", wd.XSize);
                Assert.AreEqual(wd.XSize, curDefect.Get("XSIZE"), "XSIZE");

                curDefect.Set("YSIZE", wd.YSize);
                Assert.AreEqual(wd.YSize, curDefect.Get("YSIZE"), "YSIZE");

                curDefect.Set("DEFECTAREA", wd.Area);
                Assert.AreEqual(wd.Area, curDefect.Get("DEFECTAREA"), "DEFECTAREA");

                var dic = curDefect.GetFeaturesDico();

                curDefect.Set("DSIZE", wd.Dsize);
                Assert.AreEqual(wd.Dsize, curDefect.Get("DSIZE"), "DSIZE");

                curDefect.Set("CLASSNUMBER", 0);
                curDefect.Set("TEST", 1);

                curDefect.Set("CLUSTERNUMBER", nClusterNum);
                Assert.AreEqual(nClusterNum, curDefect.Get("CLUSTERNUMBER"), "CLUSTERNUMBER");

                curDefect.Set("ROUGHBINNUMBER", wd.Roughbin);
                Assert.AreEqual(wd.Roughbin, curDefect.Get("ROUGHBINNUMBER"), "ROUGHBINNUMBER");

                // image could be added to multi tiff , one or sevrable depending on recipe
                PrmImageData imgData = new PrmImageData();
                if (true)
                {
                    // simulated grey scale thumbnail
                    imgData.List.Add(indexThumbnail++);
                    // simulated binary thumbnail
                    imgData.List.Add(indexThumbnail++);
                }
                int nLastDefectId = _dataKlarf.NDEFECT;
                curDefect.Set("DEFECTID", nLastDefectId + 1);
                Assert.AreEqual(nLastDefectId + 1, curDefect.Get(0), "DEFECTID");

                curDefect.Set("IMAGECOUNT", imgData.List.Count);
                curDefect.Set("IMAGELIST", imgData);

                _dataKlarf.AddDefect(curDefect);
            }

            return _dataKlarf;
        }

        private DataKlarf CreateSomeDieWaferData()
        {
            DataKlarf _dataKlarf = new DataKlarf(ResultType.ADC_Klarf);
            _dataKlarf.FileTimeStamp = _dtNow;
            _dataKlarf.ResultTimeStamp = _dtNow;
            // "UNITY-SC" "ToolCategory" "ToolName"
            _dataKlarf.InspectionStationID = string.Format("\"{0}\" \"{1}\" \"{2}\"", "UNITYSC", "4See", "MyWaferTestRecipeName_DIE");

            _dataKlarf.LotID = "LotIDName_DIE";
            _dataKlarf.DeviceID = "DeviceIDName_DIE";
            _dataKlarf.WaferID = "WaferIDName_DIE";
            _dataKlarf.StepID = "StepIDname_DIE";
            _dataKlarf.SetupID = new PrmSetupId("MyADCRecipeName_DIE", _dtNow);
            _dataKlarf.SlotID = 3;

            int nSampleSize_mm = _nWaferSize_mm; // wafer diameter in mm
            _dataKlarf.SampleSize = new PrmSampleSize(nSampleSize_mm);

            PrmSampleOrientationMarkType.SomtType omttype = PrmSampleOrientationMarkType.SomtType.NOTCH; // PrmSampleOrientationMarkType.somtType.FLAT PrmSampleOrientationMarkType.somtType.DFLAT
            _dataKlarf.SampleOrientationMarkType = new PrmSampleOrientationMarkType(omttype);
            _dataKlarf.OrientationMarkLocation = new PrmOrientationMarkValue("DOWN");
            _dataKlarf.OrientationInstructions = new PrmOrientationInstruction("FRONT"); //BACK
            _dataKlarf.SampleCenterLocation = new PrmPtFloat((float)nSampleSize_mm * 500.0f, (float)nSampleSize_mm * 500.0f); // wafer size x/y  * 2
            _dataKlarf.CoordinatesMirrored = new PrmYesNo(false);

            _dataKlarf.DiePitch = new PrmPtFloat(2502.0f, 1502.0f);
            _dataKlarf.DieOrigin = new PrmPtFloat(5.0f, -10.0f); // die 0,0 position in wafer coordinates

            List<KeyValuePair<int, int>> ListDieIndexes = new List<KeyValuePair<int, int>>();
            _dataKlarf.SampleTestPlan = new PrmSampleTestPlan(ListDieIndexes);

            // dummy stupid sample plan creation only for test purpose
            double squaredistlimit = (double)(nSampleSize_mm - 5) * 500.0;
            squaredistlimit *= squaredistlimit;
            for (int x = -16; x <= 16; x++)
            {
                for (int y = -30; y <= 30; y++)
                {
                    if ((Math.Pow(x * _dataKlarf.DiePitch.X, 2) + Math.Pow(y * _dataKlarf.DiePitch.Y, 2)) <= squaredistlimit)
                        _dataKlarf.SampleTestPlan.Add(x, y);
                }
            }

            _dataKlarf.AddDefectTypes(_sDieDefectTypeCategories); // warning defect type should be registered or you will have to declare and defined type by yourself using DataKlarf.AddDefectType(Type p_type, string p_name)

            int i = 0;
            int indexThumbnail = 0;
            bool useImgDta = false;
            foreach (DieDefect wd in _dummyDieDefectarray)
            {
                PrmDefect curDefect = _dataKlarf.NewDefect();
                int nClusterNum = i++;
                curDefect.Set("XSIZE", wd.XSize);
                curDefect.Set("YSIZE", wd.YSize);

                //
                // SPECIFIC DIE KLARF CLUSTER
                //
                curDefect.Set("XINDEX", wd.XDieIndex);
                curDefect.Set("YINDEX", wd.YDieIndex);
                //
                // END - SPECIFIC DIE KLARF
                //

                curDefect.Set("DEFECTAREA", wd.Area);
                curDefect.Set("DSIZE", wd.Dsize);

                curDefect.Set("CLASSNUMBER", 0);
                curDefect.Set("TEST", 1);
                curDefect.Set("CLUSTERNUMBER", nClusterNum);

                curDefect.Set("ROUGHBINNUMBER", wd.Roughbin);

                curDefect.Set("XREL", wd.Xrel);
                curDefect.Set("YREL", wd.Yrel);

                // image could be added to multi tiff , one or sevrable depending on recipe
                PrmImageData imgData = new PrmImageData();
                if (useImgDta)
                {
                    // simulated grey scale thumbnail
                    imgData.List.Add(indexThumbnail++);
                    // simulated binary thumbnail
                    imgData.List.Add(indexThumbnail++);
                }
                int nLastDefectId = _dataKlarf.NDEFECT;
                curDefect.Set("DEFECTID", nLastDefectId + 1);
                curDefect.Set("IMAGECOUNT", imgData.List.Count);
                curDefect.Set("IMAGELIST", imgData);

                _dataKlarf.AddDefect(curDefect);
            }

            _dataKlarf.NDIE = _dataKlarf.SampleTestPlan.NbDies;

            _dataKlarf.NDEFDIE = 4; // number of defectuous dies;

            return _dataKlarf;
        }

        [TestMethod]
        public void TestCreateSomeWaferData()
        {
            DataKlarf dt = CreateSomeWaferData();

            Assert.AreEqual(dt.LotID, "LotIDName", "LotID");
            Assert.AreEqual(dt.DeviceID, "DeviceIDName", "DeviceID");
            Assert.AreEqual(dt.WaferID, "WaferIDName", "WaferID");
            Assert.AreEqual(dt.StepID, "StepIDname", "StepID");
            Assert.AreEqual(dt.SlotID, 1, "SlotID");
            Assert.AreEqual(dt.SetupID.Name, "MyADCRecipeName", "SetupID.Name");
            Assert.AreEqual(dt.SetupID.Date.ToString(), _dtNow.ToString(), "SetupID.Date");

            Assert.AreEqual(_dummyDefectarray.Count, dt.DefectList.Count, "defect count");

            var defect0 = dt.DefectList[0];
            PrmImageData imgdata0 = (PrmImageData)defect0.Get("IMAGELIST");
            Assert.AreEqual(2, imgdata0.List.Count, "count PrmImageData 0");
            Assert.AreEqual(0, imgdata0.List[0], "count PrmImageData index 0 1");
            Assert.AreEqual(1, imgdata0.List[1], "count PrmImageData index 0 2");

            var defect1 = dt.DefectList[1];
            PrmImageData imgdata1 = (PrmImageData)defect1.Get("IMAGELIST");
            Assert.AreEqual(2, imgdata1.List.Count, "count PrmImageData 0");
            Assert.AreEqual(2, imgdata1.List[0], "count PrmImageData index 1 3");
            Assert.AreEqual(3, imgdata1.List[1], "count PrmImageData index 1 4");
        }

        [TestMethod]
        public void TestCreateSomeDIEWaferData()
        {
            DataKlarf dt = CreateSomeDieWaferData();

            Assert.AreEqual(dt.LotID, "LotIDName_DIE", "LotID");
            Assert.AreEqual(dt.DeviceID, "DeviceIDName_DIE", "DeviceID");
            Assert.AreEqual(dt.WaferID, "WaferIDName_DIE", "WaferID");
            Assert.AreEqual(dt.StepID, "StepIDname_DIE", "StepID");
            Assert.AreEqual(dt.SlotID, 3, "SlotID");
            Assert.AreEqual(dt.SetupID.Name, "MyADCRecipeName_DIE", "SetupID.Name");
            Assert.AreEqual(dt.SetupID.Date.ToString(), _dtNow.ToString(), "SetupID.Date");

            Assert.IsTrue(dt.SampleTestPlan.PlanList.Exists(e => e.Key == 10 && e.Value == -12), "Sample Die plan has [10,-12] index");
            Assert.IsTrue(dt.SampleTestPlan.PlanList.Exists(e => e.Key == -4 && e.Value == 2), "Sample Die plan has [-4, 2] index");
            Assert.IsFalse(dt.SampleTestPlan.PlanList.Exists(e => e.Key == 123 && e.Value == 512), "Sample Die plan DO NOT HAVE [123, 512] index");
        }

        [TestMethod]
        public void TestKlarfFileRead_NoExist()
        {
            string FileNameIn = @"WaferKlarf_NoExist.001";
            DataKlarf klaread = new DataKlarf(UnitySC.Shared.Data.Enum.ResultType.ADC_Klarf);
            Assert.IsFalse(KlarfFile.Read(FileNameIn, klaread, out string smsgerror));
        }

        [TestMethod]
        public void TestKlarfFileTypeConsistency()
        {
            string FileNameIn = @"WaferKlarf1.001";
            AssertEx.Throws<ArgumentException>(() => new DataKlarf(UnitySC.Shared.Data.Enum.ResultType.DMT_CurvatureY_Front, -1, FileNameIn)); // Bad result category
            AssertEx.Throws<ArgumentException>(() => new DataKlarf(UnitySC.Shared.Data.Enum.ResultType.ADC_Crown, -1, FileNameIn)); // Bad result format
            string BadFileNameIn = @"WaferKlarf1.KLARF";
            AssertEx.Throws<ArgumentException>(() => new DataKlarf(UnitySC.Shared.Data.Enum.ResultType.ADC_Klarf, -1, BadFileNameIn)); // Result Extension id not matched
        }

        [TestMethod]
        public void TestKlarfFileReadWrite()
        {
            string FileNameIn = @"WaferKlarf1.001";
            string FileNameOut = @"WaferKlarf2.001";

            // Test that read and write Klarf data coming from a global wafer inspection
            DataKlarf klatowrite = CreateSomeWaferData();
            KlarfFile.Write(FileNameIn, klatowrite);

            DataKlarf klaread = new DataKlarf(UnitySC.Shared.Data.Enum.ResultType.ADC_Klarf);
            Assert.IsTrue(KlarfFile.Read(FileNameIn, klaread, out string smsgerror));
            Assert.AreEqual(smsgerror, string.Empty);

            // you could also compare klatowrite and klaread
            KlarfFile.Write(FileNameOut, klaread);

            AssertFileEx.AreContentEqual(FileNameIn, FileNameOut);

            File.Delete(FileNameIn);
            File.Delete(FileNameOut);
        }

        private static Barrier s_barrier = new Barrier(participantCount: 5);

        // private static Random s_rnd = new Random();
        public static void GetReadData(string sFilename, int index)
        {
            //string FmtDatetime = "dd-MM-yyyy HH:mm:ss.ffff";
            string FmtDatetime = "mm:ss.ffff";
            //Thread.Sleep(s_rnd.Next(index*500, index*1000));
            //Thread.Sleep(index * 1000);
            // Console.WriteLine("Wait : " + index + " : " + DateTime.Now.ToString(FmtDatetime));
            s_barrier.SignalAndWait();

            Console.WriteLine("Read " + index + " " + sFilename + " : " + DateTime.Now.ToString(FmtDatetime));

            DataKlarf klaread = new DataKlarf(ResultType.ADC_Klarf);
            Assert.IsTrue(KlarfFile.Read(sFilename, klaread, out string smsgerror));
            Assert.AreEqual(string.Empty, smsgerror);
            Assert.AreNotEqual(null, klaread);
            //Thread.Sleep(s_rnd.Next(2000, 4500));
            //Thread.Sleep(index * 1000);
            Console.WriteLine("Done " + index + " " + sFilename + " : " + DateTime.Now.ToString(FmtDatetime));

            s_barrier.SignalAndWait();
        }

        [TestMethod]
        public void TestMultiThreadAccessRead()
        {
            string FileNameIn = @"WaferKlarf1.001";

            // Test that read and write Klarf data coming from a global wafer inspection
            DataKlarf klatowrite = CreateSomeWaferData();
            KlarfFile.Write(FileNameIn, klatowrite);

            Task[] tasks = new Task[5];

            for (int i = 0; i < 5; ++i)
            {
                int j = i;
                tasks[j] = Task.Factory.StartNew(() =>
                {
                    GetReadData(FileNameIn, j);
                });
            }

            Task.WaitAll(tasks);
            File.Delete(FileNameIn);
        }

        [TestMethod]
        public void TestPrmDefect()
        {
            PrmDefectType prm1 = new PrmDefectType(typeof(double),
                "TestDouble",
                new PrmDefectType.ConvertDelegate(PrmDefectType.ToDouble),
                null);
            Assert.AreEqual("TestDouble", prm1.Name);
            Assert.AreEqual(typeof(double), prm1.Type);

            string sdblStr = "912.51065";
            double dDblVal = 912.51065;

            object oRes = prm1.ConvertFromString(sdblStr);
            Assert.AreEqual(dDblVal, (double)oRes);

            string sRes = prm1.ConvertToString(dDblVal);
            Assert.AreEqual(sdblStr, sRes);

            prm1 = new PrmDefectType(typeof(double),
               "TestDouble",
               new PrmDefectType.ConvertDelegate(PrmDefectType.ToDouble),
               new PrmDefectType.ToStringDelegate(PrmDefectType.DoubleToString_2));
            Assert.AreEqual("TestDouble", prm1.Name);
            Assert.AreEqual(typeof(double), prm1.Type);

            oRes = prm1.ConvertFromString(sdblStr);
            Assert.AreEqual(dDblVal, (double)oRes); ;

            sRes = prm1.ConvertToString(dDblVal);
            Assert.AreEqual("912.51", sRes);

            sRes = PrmDefectType.DoubleToString_3(dDblVal);
            Assert.AreEqual("912.511", sRes);

            sRes = "3687";
            int nVal = (int)PrmDefectType.ToInt(sRes);
            Assert.AreEqual(3687, nVal);

            sRes = "789.3546681352";
            dDblVal = (double)PrmDefectType.ToDouble(sRes);
            Assert.AreEqual(789.3546681352, dDblVal);

            nVal = (int)PrmDefectType.ToIntFromDbl(sRes);
            Assert.AreEqual(789, nVal);
        }

        [TestMethod]
        public void TestRegisterePrmDefectType()
        {
            string p_RegisteredName = "DEFECTID";
            Assert.AreEqual(typeof(int), RegisteredDefectType.Singleton.GetType(p_RegisteredName), "registered DEFECTID bad type");

            PrmDefectType.ConvertDelegate dlconv = RegisteredDefectType.Singleton.GetConverter(p_RegisteredName);
            Assert.IsNotNull(dlconv, "not null registered convert delegate");

            object obj = dlconv("1234");
            bool bs = obj is int Nval;
            Assert.IsTrue(bs);
            if (bs)
                Assert.AreEqual(1234, (int)obj, "convert delegate");

            p_RegisteredName = "YREL";
            PrmDefectType.ToStringDelegate dltostr = RegisteredDefectType.Singleton.GetToString(p_RegisteredName);
            Assert.IsNotNull(dltostr, "YREL to string delegate");

            double dVal = 15687.06554657;
            string sRes = dltostr(dVal);  // ==> DoubleToString_2
            Assert.AreEqual("15687.07", sRes);

            // unregistered type
            p_RegisteredName = "UNKNOWN-DUMMY";
            AssertEx.Throws<Exception>(() => RegisteredDefectType.Singleton.GetType(p_RegisteredName));
            AssertEx.Throws<Exception>(() => RegisteredDefectType.Singleton.GetConverter(p_RegisteredName));
            Assert.IsNull(RegisteredDefectType.Singleton.GetToString(p_RegisteredName), "Unregister Type GetToString");
        }

        [TestMethod]
        public void TestPrmDefectTypeList()
        {
            List<string> listDefTypetoCheck = new List<string>();

            PrmDefectTypeList prmList = new PrmDefectTypeList();
            prmList.Add(typeof(double), "DummyDefect", new PrmDefectType.ConvertDelegate(PrmDefectType.ToDouble), new PrmDefectType.ToStringDelegate(PrmDefectType.DoubleToString_2));
            listDefTypetoCheck.Add("DummyDefect");
            AssertEx.Throws<Exception>(() => prmList.Add(typeof(double), "DummyDefect"));
            string p_RegisteredName = "DEFECTID";

            prmList.Add(p_RegisteredName);
            listDefTypetoCheck.Add(p_RegisteredName);
            AssertEx.Throws<Exception>(() => prmList.Add(p_RegisteredName));

            string[] p_RegisteredName_Array = { "XREL", "YREL", "DSIZE", "DEFECTAREA" };
            prmList.Add(p_RegisteredName_Array);
            foreach (string s in p_RegisteredName_Array)
                listDefTypetoCheck.Add(s);

            foreach (string s in listDefTypetoCheck)
            {
                Assert.IsNotNull(prmList.Parse(s), s + " Not inserted");
            }

            Assert.IsNull(prmList.Parse("DefectTypeNotAdded"), "Null Defect Type Not Added");
        }

        [TestMethod]
        public void TestPrmClasses()
        {
            //PrmSampleSize
            PrmSampleSize prmSampleSize = new PrmSampleSize(300, 2);
            Assert.AreEqual("2 300", prmSampleSize.ToString());

            //PrmSetupId
            PrmSetupId prmSetupId = new PrmSetupId("MyADCRecipeName", null);
            Assert.AreEqual("\"MyADCRecipeName\"", prmSetupId.ToString());

            // check format in KlarfFile.FmtDatetime
            string sDtnow = _dtNow.ToString("MM-dd-yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            prmSetupId = new PrmSetupId("MyADCRecipeName", _dtNow);
            Assert.AreEqual("\"MyADCRecipeName\" " + sDtnow, prmSetupId.ToString());

            //PrmSampleOrientationMarkType
            PrmSampleOrientationMarkType pomt = new PrmSampleOrientationMarkType(PrmSampleOrientationMarkType.SomtType.FLAT);
            Assert.AreEqual("FLAT", pomt.ToString());
            pomt = new PrmSampleOrientationMarkType(2);
            Assert.AreEqual("DFLAT", pomt.ToString());
            pomt = new PrmSampleOrientationMarkType("Notch");
            Assert.AreEqual("NOTCH", pomt.ToString());
            AssertEx.Throws<Exception>(() => new PrmSampleOrientationMarkType("Square"));
            AssertEx.Throws<Exception>(() => new PrmSampleOrientationMarkType(5));

            //PrmOrientationMarkValue
            PrmOrientationMarkValue pomv = new PrmOrientationMarkValue(1);
            Assert.AreEqual("UP", pomv.ToString());
            pomv = new PrmOrientationMarkValue("down");
            Assert.AreEqual("DOWN", pomv.ToString());
            pomv = new PrmOrientationMarkValue("LEFT");
            Assert.AreEqual("LEFT", pomv.ToString());
            pomv = new PrmOrientationMarkValue(3);
            Assert.AreEqual("RIGHT", pomv.ToString());
            AssertEx.Throws<Exception>(() => new PrmOrientationMarkValue("SIDE"));
            AssertEx.Throws<Exception>(() => new PrmOrientationMarkValue(90));

            //PrmOrientationInstruction
            PrmOrientationInstruction poiv = new PrmOrientationInstruction(0);
            Assert.AreEqual("FRONT", poiv.ToString());
            poiv = new PrmOrientationInstruction("BacK");
            Assert.AreEqual("BACK", poiv.ToString());
            poiv = new PrmOrientationInstruction(2);
            Assert.AreEqual("BEVEL", poiv.ToString());
            AssertEx.Throws<Exception>(() => new PrmOrientationInstruction("SIDE"));
            AssertEx.Throws<Exception>(() => new PrmOrientationInstruction(90));

            //PrmPtFloat
            PrmPtFloat pptf = new PrmPtFloat(1.23f, 456.789f);
            Assert.AreEqual("1.2300000200E+000 4.5678900100E+002", pptf.ToString());
            pptf = new PrmPtFloat(10000.23f, -5456.789f);
            Assert.AreEqual("1.0000230500E+004 -5.4567890600E+003", pptf.ToString());

            //PrmYesNo
            PrmYesNo pyn = new PrmYesNo(true);
            Assert.AreEqual("Yes", pyn.ToString());
            pyn = new PrmYesNo(false);
            Assert.AreEqual("No", pyn.ToString());
            // implict conversion
            pyn = (PrmYesNo)true;
            Assert.IsTrue((bool)pyn);
            Assert.AreEqual(1, (int)pyn);
            Assert.AreEqual("Yes", pyn.ToString());
            pyn = (PrmYesNo)false;
            Assert.IsFalse((bool)pyn);
            Assert.AreEqual(0, (int)pyn);
            Assert.AreEqual("No", pyn.ToString());
            pyn = (PrmYesNo)65;
            Assert.IsTrue((bool)pyn);
            pyn = (PrmYesNo)0;
            Assert.IsFalse((bool)pyn);
            Assert.AreEqual("No", (string)pyn);
            pyn = (PrmYesNo)"YEs";
            Assert.AreEqual("Yes", pyn.ToString());
            pyn = (PrmYesNo)"no";
            Assert.AreEqual("No", pyn.ToString());

            //PrmSampleTestPlan
            PrmSampleTestPlan plan = new PrmSampleTestPlan();
            Assert.IsTrue(plan.TryAdd(0, 0));
            Assert.IsFalse(plan.TryAdd(0, 0));
            plan.Add(1, -10);
            plan.Add(2, 20);
            plan.Add(-2, -30);
            plan.Add(2, 3);
            Assert.AreEqual(5, plan.NbDies);
            plan.SortIndxList();

            string sPlanResult = "5\r\n" +
                "2 20\n" +
                "2 3\n" +
                "0 0\n" +
                "1 -10\n" +
                "-2 -30";
            Assert.AreEqual(sPlanResult, plan.ToString());

            plan.ClearPlan();
            Assert.AreEqual(0, plan.NbDies);

            //PrmImageData
            PrmImageData prmImgdata = new PrmImageData();
            prmImgdata.List.Add(1);
            prmImgdata.List.Add(2);
            prmImgdata.List.Add(3);

            string strimgLine = prmImgdata.ToString();
            Assert.AreEqual("3 1 0 2 0 3 0", strimgLine);
        }
    }
}
