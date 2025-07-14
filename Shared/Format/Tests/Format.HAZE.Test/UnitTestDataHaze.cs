using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using Helper;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Format.HAZE;

namespace Format.HAZE.Test
{
    [TestClass]
    public class UnitTestDataHaze
    {
        private DateTime _dtNow = DateTime.Now;



        public UnitTestDataHaze()
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

            // add other prm here
        }

        public DataHaze CreateSomeHazeData_1()
        {
            var data = new DataHaze(ResultType.ADC_Haze);
            DateTime dtNow = _dtNow;

            data.HazeMaps.Add(CreateSomeHazeMap(1));
            data.HazeMaps.Add(CreateSomeHazeMap(2));

            return data;
        }

        public HazeMap CreateSomeHazeMap(int nId)
        {
            var map = new HazeMap();
            DateTime dtNow = _dtNow;

            // Source
            int Id = nId;
            int Width = 16;
            int Heigth = 8;
            float PixelSize_um = 50.32f;
            float[] HazeMeasures = new float[Width * Heigth];

            // stats
            float Max_ppm = -float.MaxValue;
            float Min_ppm = float.MaxValue;

            List<float> values = new List<float>(HazeMeasures.Length);
            float fsum = 0.0f;
            var rnd = new Random();
            for (int i = 0; i < HazeMeasures.Length; i++)
            {
                float rnfFloat = (float)(rnd.NextDouble() * 5000.0);
                if (Max_ppm < rnfFloat)
                    Max_ppm = rnfFloat;
                if (Min_ppm > rnfFloat)
                    Min_ppm = rnfFloat;

                HazeMeasures[i] = rnfFloat;

                fsum += rnfFloat;
                values.Add(rnfFloat);
            }
            // MEAN + STDDEV
            float Mean_ppm = fsum / (float)HazeMeasures.Length;
            fsum = 0.0f;
            for (int i = 0; i < HazeMeasures.Length; i++)
            {
                fsum += (HazeMeasures[i] - Mean_ppm) * (HazeMeasures[i] - Mean_ppm);
            }
            float Stddev_ppm = (float)Math.Sqrt((double)(fsum / (float)HazeMeasures.Length));


            // MEDIAN
            float Median_ppm = 0.0f;
            int nMid = (int)(values.Count / 2.0f);
            values.Sort();
            if (values.Count % 2.0f == 0.0f)
            {
                // le nb d'elt est pair on prends la moyenne des 2 valeurs du milieu  
                float a = values[nMid - 1];
                float b = values[nMid];
                Median_ppm = (a + b) / 2.0f;
            }
            else
            {
                //le nb d'elt est impair on prends la valeur du milieu 
                Median_ppm = values[nMid];
            }


            // ranges 
            List<HazeRange> Ranges = new List<HazeRange>(3);
            for (int i = 0; i < 3; i++)
            {
                var rg = new HazeRange();

                rg.Nrank = rnd.Next(0, 20);
                rg.Area_pct = (float)(rnd.NextDouble() * 100.0);
                rg.Max_ppm = (float)(rnd.NextDouble() * 5000.0);
                rg.Min_ppm = rg.Max_ppm - (float)(rnd.NextDouble() * 5000.0 + 100.0); ;
                rg.NbCount = (ulong)rnd.Next(50000, 2000000);
            }


            // Histo
            float HistLimitMax = (float)(rnd.NextDouble() * 5000.0) + 2000.0f;
            float HistLimitMin = (float)(rnd.NextDouble() * 2000.0);
            float HistNbStep = (float)rnd.Next(4, 10); ;
            UInt32[] Histo = new UInt32[(int)HistNbStep];
            for (int i = 0; i < HistNbStep; i++)
            {
                Histo[i] = (UInt32)rnd.Next((int)HistLimitMin, (int)HistLimitMax);
            }
            UInt32 HistMaxYBar = Histo.Max();


            // UPDATE
            map.Id = Id; // FW, BW, Tot
            map.Width = Width;
            map.Heigth = Heigth;
            map.PixelSize_um = PixelSize_um;
            map.HazeMeasures = HazeMeasures;

            // stats
            map.Max_ppm = Max_ppm;
            map.Min_ppm = Min_ppm;
            map.Mean_ppm = Mean_ppm;
            map.Stddev_ppm = Stddev_ppm;
            map.Median_ppm = Median_ppm;

            // ranges 
            map.Ranges = Ranges;

            // Histo
            map.HistLimitMax = HistLimitMax;
            map.HistLimitMin = HistLimitMin;
            map.HistNbStep = HistNbStep;
            map.Histo = Histo;
            map.HistMaxYBar = HistMaxYBar;

            return map;
        }

        [TestMethod]
        public void TestHazeFileRead_NoExist()
        {
            string FileNameIn = @"WaferHAZE_NoExist.haze";
            var data = new DataHaze(ResultType.ADC_Haze);
            Assert.IsFalse(data.ReadFromFile(FileNameIn, out string smsgerror));
            Assert.IsFalse(string.IsNullOrEmpty(smsgerror));
        }

        [TestMethod]
        public void TestHazeFileTypeConsistency()
        {
            string FileNameIn = @"Wafer1.haze";
            AssertEx.Throws<ArgumentException>(() => new DataHaze(UnitySC.Shared.Data.Enum.ResultType.DMT_CurvatureY_Front, -1, FileNameIn)); // Bad result category
            AssertEx.Throws<ArgumentException>(() => new DataHaze(UnitySC.Shared.Data.Enum.ResultType.ADC_Crown, -1, FileNameIn)); // Bad result format
            string BadFileNameIn = @"Wafer1.001";
            AssertEx.Throws<Exception>(() => new DataHaze(UnitySC.Shared.Data.Enum.ResultType.ADC_Haze, -1, BadFileNameIn)); // Result Extension id not matched
        }

        [TestMethod]
        public void TestHazeFileReadWrite()
        {
            string FileNameIn = @"Wafer1.haze";
            string FileNameOut = @"Wafer1-2.haze";

            // Test that read and write Klarf data coming from a global wafer inspection
            DataHaze hazetowrite = CreateSomeHazeData_1();
            Assert.IsTrue(hazetowrite.WriteInFile(FileNameIn, out string sErrMsg1));

            DataHaze hazeread = new DataHaze(ResultType.ADC_Haze);
            Assert.IsTrue(hazeread.ReadFromFile(FileNameIn, out string sErrMsg2));
            Assert.AreEqual(sErrMsg2, string.Empty);
            Assert.AreEqual(1, hazeread.Version);

            AreEqual(hazetowrite, hazeread);

            Assert.IsTrue(hazeread.WriteInFile(FileNameOut, out string sErrMsg3));

            AssertFileEx.AreContentEqual(FileNameIn, FileNameOut);
            File.Delete(FileNameIn);
            File.Delete(FileNameOut);
        }

        [TestMethod]
        public void TestHazeRangeSerialization()
        {
            int Nrank = 1;
            float Area_pct = 56.32f;
            float Max_ppm = 84632.52f;
            float Min_ppm = 0.0001512f;
            ulong NbCount = 1234567890;

            string Range1Path = @"Range1.rg";
            var Range1 = new HazeRange();

            Range1.Nrank = Nrank;
            Range1.Area_pct = Area_pct;
            Range1.Max_ppm = Max_ppm;
            Range1.Min_ppm = Min_ppm;
            Range1.NbCount = NbCount;

            using (var lStream = new FileStream(Range1Path, FileMode.Create))
            {
                using (var lBinaryWriter = new BinaryWriter(lStream))
                {
                    Range1.Write(lBinaryWriter);
                }
            }

            var Range1Read = new HazeRange();
            using (var lStream = new FileStream(Range1Path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var lBinaryReader = new BinaryReader(lStream))
                {
                    Range1Read.Read(lBinaryReader);
                }
            }

            Assert.AreEqual(Nrank, Range1Read.Nrank);
            Assert.AreEqual(Area_pct, Range1Read.Area_pct);
            Assert.AreEqual(Max_ppm, Range1Read.Max_ppm);
            Assert.AreEqual(Min_ppm, Range1Read.Min_ppm);
            Assert.AreEqual(NbCount, Range1Read.NbCount);

            File.Delete(Range1Path);
        }

        [TestMethod]
        public void TestHazeMapSerialization()
        {
            var hazemap1 = CreateSomeHazeMap(1);

            string map1Path = @"Map1.map";

            using (var lStream = new FileStream(map1Path, FileMode.Create))
            {
                using (var lBinaryWriter = new BinaryWriter(lStream))
                {
                    hazemap1.Write(lBinaryWriter);
                }
            }

            var map1Read = new HazeMap();
            using (var lStream = new FileStream(map1Path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var lBinaryReader = new BinaryReader(lStream))
                {
                    map1Read.Read(lBinaryReader);
                }
            }

            AreEqual(hazemap1, map1Read);
            File.Delete(map1Path);

        }

        public void AreEqual(DataHaze dt1, DataHaze dt2)
        {
            Assert.IsNotNull(dt1);
            Assert.IsNotNull(dt2);

            Assert.AreEqual(dt1.ResType, dt2.ResType);
            Assert.AreEqual(dt1.DBResId, dt2.DBResId);
            Assert.AreEqual(dt1.ResFilePath, dt2.ResFilePath);

            Assert.IsNotNull(dt1.HazeMaps);
            Assert.IsNotNull(dt2.HazeMaps);
            Assert.AreEqual(dt1.HazeMaps.Count, dt1.HazeMaps.Count);

            for (int i = 0; i < dt1.HazeMaps.Count; i++)
            {
                AreEqual(dt1.HazeMaps[i], dt2.HazeMaps[i]);
            }
        }

        public void AreEqual(HazeMap map1, HazeMap map2)
        {
            Assert.IsNotNull(map1);
            Assert.IsNotNull(map2);

            Assert.AreEqual(map1.Id, map2.Id);
            Assert.AreEqual(map1.Width, map2.Width);
            Assert.AreEqual(map1.Heigth, map2.Heigth);
            Assert.AreEqual(map1.PixelSize_um, map2.PixelSize_um);
            Assert.AreEqual(map1.Max_ppm, map2.Max_ppm);
            Assert.AreEqual(map1.Min_ppm, map2.Min_ppm);
            Assert.AreEqual(map1.Mean_ppm, map2.Mean_ppm);
            Assert.AreEqual(map1.Stddev_ppm, map2.Stddev_ppm);
            Assert.AreEqual(map1.Median_ppm, map2.Median_ppm);
            Assert.AreEqual(map1.HistLimitMax, map2.HistLimitMax);
            Assert.AreEqual(map1.HistLimitMin, map2.HistLimitMin);
            Assert.AreEqual(map1.HistNbStep, map2.HistNbStep);
            Assert.AreEqual(map1.HistMaxYBar, map2.HistMaxYBar);


            Assert.AreEqual(map1.Ranges.Count, map2.Ranges.Count);
            for (int i = 0; i < map1.Ranges.Count; i++)
            {
                AreEqual(map1.Ranges[i], map2.Ranges[i]);
            }

            CollectionAssert.AreEqual(map1.HazeMeasures, map2.HazeMeasures);
            CollectionAssert.AreEqual(map1.Histo, map2.Histo);

        }

        public void AreEqual(HazeRange rg1, HazeRange rg2)
        {
            Assert.AreEqual(rg1.Nrank, rg2.Nrank);
            Assert.AreEqual(rg1.Area_pct, rg2.Area_pct);
            Assert.AreEqual(rg1.Max_ppm, rg2.Max_ppm);
            Assert.AreEqual(rg1.Min_ppm, rg2.Min_ppm);
            Assert.AreEqual(rg1.NbCount, rg2.NbCount);
        }
    }
}
