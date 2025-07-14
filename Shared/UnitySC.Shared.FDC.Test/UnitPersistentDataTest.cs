using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.Shared.FDC.PersistentData;
using UnitySC.Shared.Tools;

namespace UnitySC.Shared.FDC.Test
{
    [TestClass]
    public class UnitPersistentDataTest
    {
        #region PersitentFDCDate

        [TestMethod]
        public void TestPersistentDateTime()
        {
            var pfdcdate = new PersistentFDCDate();
            Assert.IsNull(pfdcdate.FDCName);
            Assert.AreEqual(default(DateTime), pfdcdate.Date);

            var fdctestname = "UnitTestDateFDCName";
            DateTime dtNow = DateTime.Now;
            pfdcdate = new PersistentFDCDate(fdctestname);
            Assert.AreEqual(fdctestname, pfdcdate.FDCName);
            Assert.IsTrue(2.0 >= (pfdcdate.Date - dtNow).TotalSeconds, "Constructor without datetime should be DateTime.Now");

            DateTime dtOneHourFromNow = dtNow + new TimeSpan(1, 0, 0);
            pfdcdate = new PersistentFDCDate(fdctestname, dtOneHourFromNow);
            Assert.AreEqual(fdctestname, pfdcdate.FDCName);
            Assert.AreEqual(dtNow + new TimeSpan(1, 0, 0), pfdcdate.Date);

            DateTime dtTwoHourFromNow = dtNow + new TimeSpan(2, 0, 0);
            pfdcdate.Date = dtTwoHourFromNow;
            Assert.AreEqual(fdctestname, pfdcdate.FDCName);
            Assert.AreEqual(dtNow + new TimeSpan(2, 0, 0), pfdcdate.Date);


            string fullPath = Path.GetFullPath(@".\tempPfdcDt.xml");
            XML.Serialize(pfdcdate, fullPath);

            // text
            var pfdcdateres = XML.Deserialize<PersistentFDCDate>(fullPath);
            Assert.AreEqual(fdctestname, pfdcdateres.FDCName);
            Assert.AreEqual(dtNow + new TimeSpan(2, 0, 0), pfdcdateres.Date);

            // binary
            using (var ms = new MemoryStream())
            {
                var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formatter.Serialize(ms, pfdcdateres);

                ms.Position = 0;
                var deserialized = (PersistentFDCDate)formatter.Deserialize(ms);

                Assert.AreEqual(pfdcdateres.FDCName, deserialized.FDCName);
                Assert.AreEqual(pfdcdateres.Date, deserialized.Date);
            }

            File.Delete(fullPath);

        }

        #endregion

        #region PersitentFDCTimeSpan
        [TestMethod]
        public void TestPersistentTimeSpan()
        {
            var pfdcts = new PersistentFDCTimeSpan();
            Assert.IsNull(pfdcts.FDCName);
            Assert.AreEqual(default(TimeSpan), pfdcts.Timespan);

            var fdctestname = "UnitTestDateFDCNameTS";
            pfdcts = new PersistentFDCTimeSpan(fdctestname);
            Assert.AreEqual(fdctestname, pfdcts.FDCName);
            Assert.AreEqual(default(TimeSpan), pfdcts.Timespan);


            var ts1 = new TimeSpan(1, 5, 15);
            pfdcts = new PersistentFDCTimeSpan(fdctestname, ts1);
            Assert.AreEqual(fdctestname, pfdcts.FDCName);
            Assert.AreEqual(new TimeSpan(1, 5, 15), pfdcts.Timespan);


            var ts2 = new TimeSpan(3, 13, 43, 12, 512);
            pfdcts.Timespan = new TimeSpan(ts2.Ticks);
            Assert.AreEqual(fdctestname, pfdcts.FDCName);
            Assert.AreEqual(ts2, pfdcts.Timespan);


            string fullPath = Path.GetFullPath(@".\tempPfdcTs.xml");
            XML.Serialize(pfdcts, fullPath);

            var pfdcTSres = XML.Deserialize<PersistentFDCTimeSpan>(fullPath);
            Assert.AreEqual(fdctestname, pfdcTSres.FDCName);
            Assert.AreEqual(ts2, pfdcTSres.Timespan);

            using (var ms = new MemoryStream())
            {
                var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formatter.Serialize(ms, pfdcts);

                ms.Position = 0;
                var deserialized = (PersistentFDCTimeSpan)formatter.Deserialize(ms);

                Assert.AreEqual(pfdcts.FDCName, deserialized.FDCName);
                Assert.AreEqual(pfdcts.Timespan, deserialized.Timespan);
            }

            File.Delete(fullPath);

        }
        #endregion

        #region PersitentFDCCounter

        [TestMethod]
        public void TestPersistentCounter()
        {
            Random rnd = new Random();

            var pfdcNull = new PersistentFDCCounter<int>();
            Assert.IsNull(pfdcNull.FDCName);
            Assert.AreEqual(default(int), pfdcNull.Counter);

            var pfdcInt = new PersistentFDCCounter<int>("Countint");
            Assert.AreEqual("Countint", pfdcInt.FDCName);
            Assert.AreEqual(default(int), pfdcInt.Counter);

            pfdcInt.Counter = 100;
            Assert.AreEqual(100, pfdcInt.Counter);

            pfdcInt.Counter++;
            Assert.AreEqual(101, pfdcInt.Counter);

            pfdcInt.Counter--;
            Assert.AreEqual(100, pfdcInt.Counter);

            var pfdcU64 = new PersistentFDCCounter<UInt64>("CountUInt64");
            Assert.AreEqual("CountUInt64", pfdcU64.FDCName);
            Assert.AreEqual(default(UInt64), pfdcU64.Counter);

            for (int i = 0; i < 20; i++)
            {
                UInt64 u = (UInt64)(rnd.NextDouble() * (double)(UInt64.MaxValue / 2));
                pfdcU64.Counter = u;
                Assert.AreEqual(u, pfdcU64.Counter);

                pfdcU64.Counter++;
                Assert.AreEqual(u + 1, pfdcU64.Counter);
            }

            var pfdc = new PersistentFDCCounter<double>() { FDCName = "CountDbl" };
            Assert.AreEqual("CountDbl", pfdc.FDCName);
            Assert.AreEqual(default(double), pfdc.Counter);

            for (int i = 0; i < 20; i++)
            {
                var v = 1000.0 * (rnd.NextDouble() - 0.5);
                pfdc.Counter = v;
                Assert.AreEqual(v, pfdc.Counter);

                pfdc.Counter++;
                Assert.AreEqual(v + 1, pfdc.Counter);

                pfdc.Counter *= 2.0;
                Assert.AreEqual(2.0 * (v + 1.0), pfdc.Counter);
            }


            string fullPath = Path.GetFullPath(@".\tempPfdcCounterint.xml");
            XML.Serialize(pfdcInt, fullPath);
            var pfdcCNTint = XML.Deserialize<PersistentFDCCounter<int>>(fullPath);
            Assert.AreEqual(pfdcInt.FDCName, pfdcCNTint.FDCName);
            Assert.AreEqual(pfdcInt.Counter, pfdcCNTint.Counter);

            // binary
            using (var ms = new MemoryStream())
            {
                var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formatter.Serialize(ms, pfdcCNTint);

                ms.Position = 0;
                var deserialized = (PersistentFDCCounter<int>)formatter.Deserialize(ms);

                Assert.AreEqual(pfdcCNTint.FDCName, deserialized.FDCName);
                Assert.AreEqual(pfdcCNTint.Counter, deserialized.Counter);
            }

            File.Delete(fullPath);

            fullPath = Path.GetFullPath(@".\tempPfdcCounterDouble.xml");
            XML.Serialize(pfdc, fullPath);
            var pfdcCNTdouble = XML.Deserialize<PersistentFDCCounter<double>>(fullPath);
            Assert.AreEqual(pfdc.FDCName, pfdcCNTdouble.FDCName);
            Assert.AreEqual(pfdc.Counter, pfdcCNTdouble.Counter); // binary
            using (var ms = new MemoryStream())
            {
                var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formatter.Serialize(ms, pfdcCNTdouble);

                ms.Position = 0;
                var deserialized = (PersistentFDCCounter<double>)formatter.Deserialize(ms);

                Assert.AreEqual(pfdcCNTdouble.FDCName, deserialized.FDCName);
                Assert.AreEqual(pfdcCNTdouble.Counter, deserialized.Counter);
            }
            File.Delete(fullPath);
        }

        #endregion

        #region PersistenWindowData

        [TestMethod]
        public void TestPersistenWindowData_NULL()
        {
            var pfdcNULLwindata = new PersistentWindowData<int>();
            Assert.IsNull(pfdcNULLwindata.FDCName);
            Assert.AreEqual(0, pfdcNULLwindata.WindowSize);
            Assert.IsNull(pfdcNULLwindata.WindowData);
        }

        [TestMethod]
        public void TestPersistenWindowData_int_10_SingleAdd()
        {
            string fdcname = "Testfdcname_int_10";
            var pfdcwindata = new PersistentWindowData<int>(fdcname, 10);
            Assert.AreEqual(fdcname, pfdcwindata.FDCName);
            Assert.AreEqual(10, pfdcwindata.WindowSize);
            Assert.IsNotNull(pfdcwindata.WindowData);
            Assert.IsFalse(pfdcwindata.IsWindowFull());

            // Add single until last
            for (int i = 1; i <= 9; i++)
            {
                pfdcwindata.AddData(i);
                Assert.IsFalse(pfdcwindata.IsWindowFull());
                Assert.AreEqual(10, pfdcwindata.WindowSize);
            }
            // add last single data then window is full
            pfdcwindata.AddData(10);
            Assert.IsTrue(pfdcwindata.IsWindowFull());
            Assert.AreEqual(10, pfdcwindata.WindowSize);
            Assert.AreEqual(1, pfdcwindata.WindowData.First());
            Assert.AreEqual(10, pfdcwindata.WindowData.Last());

            // add single data in full window , first element is removed last elements is added
            for (int i = 1; i <= 5; i++)
            {
                pfdcwindata.AddData(10 + i);

                Assert.IsTrue(pfdcwindata.IsWindowFull());
                Assert.AreEqual(10, pfdcwindata.WindowSize);
                Assert.AreEqual(1 + i, pfdcwindata.WindowData.First());
                Assert.AreEqual(10 + i, pfdcwindata.WindowData.Last());
            }
        }

        [TestMethod]
        public void TestPersistenWindowData_int_5_AddRange3()
        {
            string fdcname = "Testfdcname_int_5-3";
            var pfdcwindata = new PersistentWindowData<int>(fdcname, 5);
            Assert.AreEqual(fdcname, pfdcwindata.FDCName);
            Assert.AreEqual(5, pfdcwindata.WindowSize);
            Assert.IsNotNull(pfdcwindata.WindowData);
            Assert.IsFalse(pfdcwindata.IsWindowFull());

            var list123 = new List<int>() { 1, 2, 3 };
            var list456 = new List<int>() { 4, 5, 6 };
            var list789 = new List<int>() { 7, 8, 9 };

            pfdcwindata.AddDataRange(list123);
            Assert.IsFalse(pfdcwindata.IsWindowFull());
            Assert.AreEqual(1, pfdcwindata.WindowData.First());
            Assert.AreEqual(3, pfdcwindata.WindowData.Last());

            pfdcwindata.AddDataRange(list456);
            Assert.IsTrue(pfdcwindata.IsWindowFull());
            Assert.AreEqual(2, pfdcwindata.WindowData.First());
            Assert.AreEqual(6, pfdcwindata.WindowData.Last());

            pfdcwindata.AddDataRange(list789);
            Assert.IsTrue(pfdcwindata.IsWindowFull());
            Assert.AreEqual(5, pfdcwindata.WindowData.First());
            Assert.AreEqual(9, pfdcwindata.WindowData.Last());

            pfdcwindata.AddData(10);
            Assert.AreEqual(6, pfdcwindata.WindowData.First());
            Assert.AreEqual(10, pfdcwindata.WindowData.Last());
        }

        [TestMethod]
        public void TestPersistenWindowData_int_5_AddRange7()
        {
            string fdcname = "Testfdcname_int_5-7";
            var pfdcwindata = new PersistentWindowData<int>(fdcname, 5);
            Assert.AreEqual(fdcname, pfdcwindata.FDCName);
            Assert.AreEqual(5, pfdcwindata.WindowSize);
            Assert.IsNotNull(pfdcwindata.WindowData);
            Assert.IsFalse(pfdcwindata.IsWindowFull());

            var list1 = new List<int>() { 1, 2, 3, 4, 5, 6, 7 };
            var list10 = new List<int>() { 10, 20, 30, 40, 50, 60, 70 };
            var list100 = new List<int>() { 100, 200, 300, 400, 500, 600, 700 };

            pfdcwindata.AddDataRange(list1);
            Assert.IsTrue(pfdcwindata.IsWindowFull());
            Assert.AreEqual(3, pfdcwindata.WindowData.First());
            Assert.AreEqual(7, pfdcwindata.WindowData.Last());

            pfdcwindata.AddDataRange(list10);
            Assert.IsTrue(pfdcwindata.IsWindowFull());
            Assert.AreEqual(30, pfdcwindata.WindowData.First());
            Assert.AreEqual(70, pfdcwindata.WindowData.Last());

            pfdcwindata.AddDataRange(list100);
            Assert.IsTrue(pfdcwindata.IsWindowFull());
            Assert.AreEqual(300, pfdcwindata.WindowData.First());
            Assert.AreEqual(700, pfdcwindata.WindowData.Last());

            pfdcwindata.AddData(10);
            Assert.AreEqual(400, pfdcwindata.WindowData.First());
            Assert.AreEqual(10, pfdcwindata.WindowData.Last());
        }

        [TestMethod]
        public void TestPersistentWindowData_Byte()
        {
            string fdcname = "Testfdcname_byte";
            var pfdcwindata = new PersistentWindowData<byte>(fdcname, 5);
            Assert.AreEqual(fdcname, pfdcwindata.FDCName);
            Assert.AreEqual(5, pfdcwindata.WindowSize);
            Assert.IsNotNull(pfdcwindata.WindowData);
            Assert.IsFalse(pfdcwindata.IsWindowFull());

            Random rnd = new Random();
            rnd.Next();

            double sum = 0;
            // Add single until last
            for (int i = 1; i <= 5; i++)
            {
                byte b = (byte)rnd.Next(255);
                pfdcwindata.AddData(b);

                Assert.AreEqual(b, pfdcwindata.WindowData.Last());

                sum += (double)b;
            }
            Assert.IsTrue(pfdcwindata.IsWindowFull());

            double avg = sum / 5.0;
            Assert.AreEqual(avg, Helpers.HelperFDCWindow.ComputeDblAverage(pfdcwindata), "Bad double average");

            byte b6 = (byte)rnd.Next(255);
            sum -= pfdcwindata.WindowData.First();
            pfdcwindata.AddData(b6);
            sum += (double)b6;
            avg = sum / 5.0;

            Assert.AreEqual(avg, Helpers.HelperFDCWindow.ComputeDblAverage(pfdcwindata), "Bad double average slided");

            Assert.AreEqual((int)avg, Helpers.HelperFDCWindow.ComputeIntAverage(pfdcwindata), "Bad int average slided");

            Assert.AreEqual((long)avg, Helpers.HelperFDCWindow.ComputeLongAverage(pfdcwindata), "Bad long average slided");
        }

        [TestMethod]
        public void TestPersistentWindowData_Int32()
        {
            string fdcname = "Testfdcname_Int32";
            var pfdcwindata = new PersistentWindowData<Int32>(fdcname, 5);
            Assert.AreEqual(fdcname, pfdcwindata.FDCName);
            Assert.AreEqual(5, pfdcwindata.WindowSize);
            Assert.IsNotNull(pfdcwindata.WindowData);
            Assert.IsFalse(pfdcwindata.IsWindowFull());

            Random rnd = new Random();
            rnd.Next();

            double sum = 0;
            // Add single until last
            for (int i = 1; i <= 5; i++)
            {
                Int32 n = rnd.Next();
                pfdcwindata.AddData(n);

                Assert.AreEqual(n, pfdcwindata.WindowData.Last());

                sum += (double)n;
            }
            Assert.IsTrue(pfdcwindata.IsWindowFull());

            double avg = sum / 5.0;
            Assert.AreEqual(avg, Helpers.HelperFDCWindow.ComputeDblAverage(pfdcwindata), "Bad double average");

            Int32 n6 = rnd.Next();
            sum -= pfdcwindata.WindowData.First();
            pfdcwindata.AddData(n6);
            sum += (double)n6;
            avg = sum / 5.0;

            Assert.AreEqual(avg, Helpers.HelperFDCWindow.ComputeDblAverage(pfdcwindata), "Bad double average slided");

            Assert.AreEqual((int)avg, Helpers.HelperFDCWindow.ComputeIntAverage(pfdcwindata), "Bad int average slided");

            Assert.AreEqual((long)avg, Helpers.HelperFDCWindow.ComputeLongAverage(pfdcwindata), "Bad long average slided");
        }

        [TestMethod]
        public void TestPersistentWindowData_Int64()
        {
            string fdcname = "Testfdcname_Int64";
            var pfdcwindata = new PersistentWindowData<long>(fdcname, 5);
            Assert.AreEqual(fdcname, pfdcwindata.FDCName);
            Assert.AreEqual(5, pfdcwindata.WindowSize);
            Assert.IsNotNull(pfdcwindata.WindowData);
            Assert.IsFalse(pfdcwindata.IsWindowFull());

            Random rnd = new Random();
            rnd.Next();

            double sum = 0;
            // Add single until last
            for (int i = 1; i <= 5; i++)
            {
                long n = ((long)rnd.Next() << 5);
                pfdcwindata.AddData(n);

                Assert.AreEqual(n, pfdcwindata.WindowData.Last());

                sum += (double)n;
            }
            Assert.IsTrue(pfdcwindata.IsWindowFull());

            double avg = sum / 5.0;
            Assert.AreEqual(avg, Helpers.HelperFDCWindow.ComputeDblAverage(pfdcwindata), "Bad double average");

            long n6 = ((long)rnd.Next() << 5);
            sum -= pfdcwindata.WindowData.First();
            pfdcwindata.AddData(n6);
            sum += (double)n6;
            avg = sum / 5.0;

            Assert.AreEqual(avg, Helpers.HelperFDCWindow.ComputeDblAverage(pfdcwindata), "Bad double average slided");

            Assert.AreEqual((long)avg, Helpers.HelperFDCWindow.ComputeLongAverage(pfdcwindata), "Bad long average slided");
        }

        [TestMethod]
        public void TestPersistentWindowData_Float()
        {
            string fdcname = "Testfdcname_float";
            var pfdcwindata = new PersistentWindowData<float>(fdcname, 8);
            Assert.AreEqual(fdcname, pfdcwindata.FDCName);
            Assert.AreEqual(8, pfdcwindata.WindowSize);
            Assert.IsNotNull(pfdcwindata.WindowData);
            Assert.IsFalse(pfdcwindata.IsWindowFull());

            Random rnd = new Random();
            rnd.Next();

            double sum = 0;
            // Add single until last
            for (int i = 1; i <= 8; i++)
            {
                float n = (float)rnd.Next() * 0.01f;
                pfdcwindata.AddData(n);

                Assert.AreEqual(n, pfdcwindata.WindowData.Last());

                sum += (double)n;
            }
            Assert.IsTrue(pfdcwindata.IsWindowFull());

            double avg = sum / 8.0;
            Assert.AreEqual(avg, Helpers.HelperFDCWindow.ComputeDblAverage(pfdcwindata), 1e-12, "Bad double average");

            float f6 = (float)rnd.Next() * 0.01f;
            sum -= pfdcwindata.WindowData.First();
            pfdcwindata.AddData(f6);
            sum += (double)f6;
            avg = sum / 8.0;

            Assert.AreEqual(avg, Helpers.HelperFDCWindow.ComputeDblAverage(pfdcwindata), 1e-12, "Bad double average slided");
        }

        [TestMethod]
        public void TestPersistentWindowData_Double()
        {
            string fdcname = "Testfdcname_float";
            var pfdcwindata = new PersistentWindowData<double>(fdcname, 21);
            Assert.AreEqual(fdcname, pfdcwindata.FDCName);
            Assert.AreEqual(21, pfdcwindata.WindowSize);
            Assert.IsNotNull(pfdcwindata.WindowData);
            Assert.IsFalse(pfdcwindata.IsWindowFull());

            Random rnd = new Random();
            rnd.Next();

            double sum = 0;
            // Add single until last
            for (int i = 1; i <= 21; i++)
            {
                double d = (rnd.NextDouble() - 0.5) * 156.1;
                pfdcwindata.AddData(d);

                Assert.AreEqual(d, pfdcwindata.WindowData.Last());

                sum += d;
            }
            Assert.IsTrue(pfdcwindata.IsWindowFull());

            double avg = sum / 21.0;
            Assert.AreEqual(avg, Helpers.HelperFDCWindow.ComputeDblAverage(pfdcwindata), 1e-12, "Bad double average");

            double d6 = (rnd.NextDouble() - 0.5) * 156.1;
            sum -= pfdcwindata.WindowData.First();
            pfdcwindata.AddData(d6);
            sum += d6;
            avg = sum / 21.0;

            Assert.AreEqual(avg, Helpers.HelperFDCWindow.ComputeDblAverage(pfdcwindata), 1e-12, "Bad double average slided");
        }

        #endregion

        #region PersistentSumData

        [TestMethod]
        public void TestPersistentSumData_NULL()
        {
            var pfdcNULLSumdata = new PersistentSumData<int>();
            Assert.IsNull(pfdcNULLSumdata.FDCName);
            Assert.AreEqual(0uL, pfdcNULLSumdata.NbItems);
            Assert.AreEqual(0.0, pfdcNULLSumdata.Sum);
            Assert.AreEqual(0.0, pfdcNULLSumdata.Average());
        }

        [TestMethod]
        public void TestPersistentSumData_int()
        {
            string fdcname = "Testfdcname_sumInt";
            var pfdcSumdata = new PersistentSumData<int>(fdcname);
            Assert.AreEqual(fdcname, pfdcSumdata.FDCName);
            Assert.AreEqual(0uL, pfdcSumdata.NbItems);
            Assert.AreEqual(0.0, pfdcSumdata.Sum);
            Assert.AreEqual(0.0, pfdcSumdata.Average());

            pfdcSumdata.Add(2);
            Assert.AreEqual(1uL, pfdcSumdata.NbItems);
            Assert.AreEqual(2.0, pfdcSumdata.Sum);
            Assert.AreEqual(2.0, pfdcSumdata.Average());

            pfdcSumdata.Add(3);
            Assert.AreEqual(2uL, pfdcSumdata.NbItems);
            Assert.AreEqual(5.0, pfdcSumdata.Sum);
            Assert.AreEqual(2.5, pfdcSumdata.Average());

            var range = new List<int>() { 4, 5, 6 };
            pfdcSumdata.AddRange(range);
            Assert.AreEqual(5uL, pfdcSumdata.NbItems);
            Assert.AreEqual(20.0, pfdcSumdata.Sum);
            Assert.AreEqual(4.0, pfdcSumdata.Average());
        }

        [TestMethod]
        public void TestPersistentSumData_Double()
        {
            string fdcname = "Testfdcname_sumDbl";
            var pfdcSumdata = new PersistentSumData<double>(fdcname);
            Assert.AreEqual(fdcname, pfdcSumdata.FDCName);
            Assert.AreEqual(0uL, pfdcSumdata.NbItems);
            Assert.AreEqual(0.0, pfdcSumdata.Sum);
            Assert.AreEqual(0.0, pfdcSumdata.Average());

            Random rnd = new Random();
            double mysum = 0.0;
            // Add single until last
            for (int i = 0; i < 100; i++)
            {
                double d = (rnd.NextDouble() - 0.5) * 100.0;
                pfdcSumdata.Add(d);
                mysum += d;
            }
            double avg = mysum / 100.0;
            Assert.AreEqual(100uL, pfdcSumdata.NbItems);
            Assert.AreEqual(mysum, pfdcSumdata.Sum);
            Assert.AreEqual(avg, pfdcSumdata.Average());
        }

        #endregion

        #region PersistentWindowTimeData

        [TestMethod]
        public void TestPersistenWindowTimeData_NULL()
        {
            var pfdcNULLwindata = new PersistentWindowTimeData<int>();
            Assert.IsNull(pfdcNULLwindata.FDCName);
            Assert.AreEqual(TimeSpan.Zero, pfdcNULLwindata.WinPeriod);
            Assert.IsNull(pfdcNULLwindata.WindowTimeData);
        }

        [TestMethod]
        public void TestPersistenWindowTimeData_int_SingleAdd()
        {
            string fdcname = "Testfdcname_int_Time";
            TimeSpan ts10Sec = new TimeSpan(0, 0, 10);

            var pfdcwindata = new PersistentWindowTimeData<int>(fdcname, ts10Sec);
            Assert.AreEqual(fdcname, pfdcwindata.FDCName);
            Assert.AreEqual(10, pfdcwindata.WinPeriod.Seconds);
            Assert.IsNotNull(pfdcwindata.WindowTimeData);
            Assert.AreEqual(0, pfdcwindata.WindowTimeData.Count);

            // Add single until last
            DateTime dt = DateTime.Now;
            TimeSpan timegap = new TimeSpan(0, 0, 1);
            dt = dt + timegap;
            for (int i = 1; i <= 10; i++)
            {
                pfdcwindata.AddData(i, dt);
                dt = dt + timegap;
            }
            // add last single data then window period has timespan expectd
            pfdcwindata.AddData(11, dt);
            dt = dt + timegap;
            Assert.AreEqual(1, pfdcwindata.WindowTimeData.First().Value);
            Assert.AreEqual(11, pfdcwindata.WindowTimeData.Last().Value);
            Assert.AreEqual(11, pfdcwindata.WindowTimeData.Count); // since we remove dates strictly < to 10 seconds

            // add single data when period time is exceeded, first element is removed last elements is added
            // since timegap is a regular 1 sec we should have 11 elements in windows 
            int nbExt = 15;
            for (int i = 1; i <= nbExt; i++)
            {
                pfdcwindata.AddData(11 + i, dt);
                dt = dt + timegap;
                Assert.AreEqual(1 + i, pfdcwindata.WindowTimeData.First().Value);
                Assert.AreEqual(11 + i, pfdcwindata.WindowTimeData.Last().Value);
                Assert.AreEqual(11, pfdcwindata.WindowTimeData.Count); // since we remove dates strictly < to 10 seconds
            }

            // add a larger gap then we should remove some older samples after add  
            dt = dt + new TimeSpan(0, 0, 5);
            pfdcwindata.AddData(100, dt);
            // now window size should be reduced
            Assert.AreEqual(6, pfdcwindata.WindowTimeData.Count);
            Assert.AreEqual(11 + nbExt - 5 + 1, pfdcwindata.WindowTimeData.First().Value);
            Assert.AreEqual(100, pfdcwindata.WindowTimeData.Last().Value);

            dt = dt + new TimeSpan(0, 0, 20);
            pfdcwindata.AddData(200, dt);
            // windows should only have this last measure all other measures are out of scope
            Assert.AreEqual(1, pfdcwindata.WindowTimeData.Count);
            Assert.AreEqual(200, pfdcwindata.WindowTimeData.First().Value);
            Assert.AreEqual(200, pfdcwindata.WindowTimeData.Last().Value);
        }

        [TestMethod]
        public void TestPersistenWindowTimeData_int_AddOlderData()
        {
            string fdcname = "Testfdcname_int_TimeAddOlderData(";
            TimeSpan ts10Sec = new TimeSpan(0, 0, 10);

            var pfdcwindata = new PersistentWindowTimeData<int>(fdcname, ts10Sec);
            Assert.AreEqual(fdcname, pfdcwindata.FDCName);
            Assert.AreEqual(10, pfdcwindata.WinPeriod.Seconds);
            Assert.IsNotNull(pfdcwindata.WindowTimeData);
            Assert.AreEqual(0, pfdcwindata.WindowTimeData.Count);

            // Add single until last
            DateTime start = DateTime.Now;
            DateTime dt = start;
            TimeSpan timegap = new TimeSpan(0, 0, 1);
            dt = dt + timegap;
            for (int i = 1; i <= 5; i++)
            {
                pfdcwindata.AddData(i, dt);
                dt = dt + timegap;
                Assert.AreEqual(i, pfdcwindata.WindowTimeData.Last().Value);
            }
            Assert.AreEqual(5, pfdcwindata.WindowTimeData.Count);

            var badentry = new TimedValue<int>(666, dt - new TimeSpan(0, 6, 6));
            pfdcwindata.AddData(badentry);
            // Add data has been silently skipped since data 666 is older than the last entry
            Assert.AreEqual(5, pfdcwindata.WindowTimeData.Count);
            Assert.AreEqual(5, pfdcwindata.WindowTimeData.Last().Value);

            pfdcwindata.AddData(667, dt - new TimeSpan(0, 6, 7));
            // Add data has been silently skipped since data 667 is older than the last entry
            Assert.AreEqual(5, pfdcwindata.WindowTimeData.Count);
            Assert.AreEqual(5, pfdcwindata.WindowTimeData.Last().Value);

            dt = dt + timegap;
            var goodentry = new TimedValue<int>(668, dt);
            pfdcwindata.AddData(goodentry);
            Assert.AreEqual(6, pfdcwindata.WindowTimeData.Count);
            Assert.AreEqual(668, pfdcwindata.WindowTimeData.Last().Value);

        }

        [TestMethod]
        public void TestPersistentWindowTimeData_Int32()
        {
            string fdcname = "Testfdcname_TimesInt32";
            var pfdcwindata = new PersistentWindowTimeData<Int32>(fdcname, new TimeSpan(0, 0, 4));
            Assert.AreEqual(fdcname, pfdcwindata.FDCName);
            Assert.IsNotNull(pfdcwindata.WindowTimeData);

            Random rnd = new Random();
            rnd.Next();

            DateTime dt = DateTime.Now;
            TimeSpan timegap = new TimeSpan(0, 0, 1);
            dt = dt + timegap;

            double sum = 0;
            // Add single until last
            for (int i = 1; i <= 5; i++)
            {
                Int32 n = rnd.Next();
                pfdcwindata.AddData(n, dt);
                dt = dt + timegap;

                Assert.AreEqual(n, pfdcwindata.WindowTimeData.Last().Value);

                sum += (double)n;
            }
            Assert.AreEqual(5, pfdcwindata.WindowTimeData.Count);

            double avg = sum / 5.0;
            Assert.AreEqual(avg, Helpers.HelperFDCWindowTime.ComputeDblAverage(pfdcwindata), "Bad double average");

            Int32 n6 = rnd.Next();
            sum -= pfdcwindata.WindowTimeData.First().Value;
            pfdcwindata.AddData(n6, dt);
            dt = dt + timegap;
            Assert.AreEqual(5, pfdcwindata.WindowTimeData.Count);
            Assert.AreEqual(n6, pfdcwindata.WindowTimeData.Last().Value);
            sum += (double)n6;
            avg = sum / 5.0;

            Assert.AreEqual(avg, Helpers.HelperFDCWindowTime.ComputeDblAverage(pfdcwindata), "Bad double average slided");

            Assert.AreEqual((int)avg, Helpers.HelperFDCWindowTime.ComputeIntAverage(pfdcwindata), "Bad int average slided");

            Assert.AreEqual((long)avg, Helpers.HelperFDCWindowTime.ComputeLongAverage(pfdcwindata), "Bad long average slided");
        }

        #endregion

        #region PersistentFDCRate
        [TestMethod]
        public void TestPersistentFDCRate_NULL()
        {
            var pfdcNullRate = new PersistentFDCRate<int>();
            Assert.IsNull(pfdcNullRate.FDCName);
            Assert.AreEqual(0, pfdcNullRate.Amount);
            Assert.AreEqual(0, pfdcNullRate.Total);
            Assert.AreEqual(0.0, pfdcNullRate.GetRate());
            Assert.AreEqual(0.0, pfdcNullRate.GetPercentageRate());
        }

        [TestMethod]
        public void TestPersistentFDCRate_int()
        {
            string fdcname = "Testfdcname_rateInt";
            var pfdcIntRate = new PersistentFDCRate<int>(fdcname);
            Assert.AreEqual(fdcname, pfdcIntRate.FDCName);

            pfdcIntRate.Amount++;
            pfdcIntRate.Total++;
            Assert.AreEqual(1, pfdcIntRate.Amount);
            Assert.AreEqual(1, pfdcIntRate.Total);
            Assert.AreEqual(1.0, pfdcIntRate.GetRate());
            Assert.AreEqual(100.0, pfdcIntRate.GetPercentageRate());

            pfdcIntRate.Amount++;
            pfdcIntRate.Total++;
            Assert.AreEqual(2, pfdcIntRate.Amount);
            Assert.AreEqual(2, pfdcIntRate.Total);
            Assert.AreEqual(1.0, pfdcIntRate.GetRate());
            Assert.AreEqual(100.0, pfdcIntRate.GetPercentageRate());

            pfdcIntRate.Total = 4;
            Assert.AreEqual(2, pfdcIntRate.Amount);
            Assert.AreEqual(4, pfdcIntRate.Total);
            Assert.AreEqual(0.5, pfdcIntRate.GetRate());
            Assert.AreEqual(50.0, pfdcIntRate.GetPercentageRate());

            pfdcIntRate.Clear();
            Assert.AreEqual(0, pfdcIntRate.Amount);
            Assert.AreEqual(0, pfdcIntRate.Total);
            Assert.AreEqual(0.0, pfdcIntRate.GetRate());
            Assert.AreEqual(0.0, pfdcIntRate.GetPercentageRate());
        }

        [TestMethod]
        public void TestPersistentFDCRate_double()
        {
            string fdcname = "Testfdcname_rateDouble";
            var pfdcDoubleRate = new PersistentFDCRate<double>(fdcname);
            Assert.AreEqual(fdcname, pfdcDoubleRate.FDCName);

            pfdcDoubleRate.Amount++;
            pfdcDoubleRate.Total++;
            Assert.AreEqual(1.0, pfdcDoubleRate.Amount);
            Assert.AreEqual(1.0, pfdcDoubleRate.Total);
            Assert.AreEqual(1.0, pfdcDoubleRate.GetRate());
            Assert.AreEqual(100, pfdcDoubleRate.GetPercentageRate());

            pfdcDoubleRate.Amount++;
            pfdcDoubleRate.Total++;
            Assert.AreEqual(2.0, pfdcDoubleRate.Amount);
            Assert.AreEqual(2.0, pfdcDoubleRate.Total);
            Assert.AreEqual(1.0, pfdcDoubleRate.GetRate());
            Assert.AreEqual(100.0, pfdcDoubleRate.GetPercentageRate());

            pfdcDoubleRate.Total = 4.0;
            Assert.AreEqual(2.0, pfdcDoubleRate.Amount);
            Assert.AreEqual(4.0, pfdcDoubleRate.Total);
            Assert.AreEqual(0.5, pfdcDoubleRate.GetRate());
            Assert.AreEqual(50.0, pfdcDoubleRate.GetPercentageRate());

            pfdcDoubleRate.Clear();
            Assert.AreEqual(0.0, pfdcDoubleRate.Amount);
            Assert.AreEqual(0.0, pfdcDoubleRate.Total);
            Assert.AreEqual(0.0, pfdcDoubleRate.GetRate());
            Assert.AreEqual(0.0, pfdcDoubleRate.GetPercentageRate());
        }
        #endregion PersistentFDCRate

        #region PersitentFDCCountdown

        [TestMethod]
        public void TestPersistentPersitentFDCCountdown()
        {
            var pfdcCountdown = new PersitentFDCCountdown();
            Assert.IsNull(pfdcCountdown.FDCName);
            Assert.AreEqual(default(DateTime), pfdcCountdown.ResetDate);

            var fdctestname = "UnitTestCountdownFDCName";
            DateTime dtNow = DateTime.Now;
            pfdcCountdown = new PersitentFDCCountdown(fdctestname);
            Assert.AreEqual(fdctestname, pfdcCountdown.FDCName);
            Assert.AreEqual(TimeSpan.Zero, pfdcCountdown.InitialCountTime);
            Assert.IsTrue(2.0 >= (pfdcCountdown.ResetDate - dtNow).TotalSeconds, "Constructor without datetime should be DateTime.Now");

            DateTime dtOneHourAgo = dtNow - new TimeSpan(1, 0, 0);
            pfdcCountdown = new PersitentFDCCountdown(fdctestname, dtOneHourAgo, new TimeSpan(10, 0, 0));
            Assert.AreEqual(fdctestname, pfdcCountdown.FDCName);
            Assert.AreEqual(10.0, pfdcCountdown.InitialCountTime.TotalHours);
            Assert.AreEqual(dtNow - new TimeSpan(1, 0, 0), pfdcCountdown.ResetDate);
            Assert.IsTrue(9.0 >=  pfdcCountdown.CountdownHours);

            DateTime dtTwoHourAgo = dtNow - new TimeSpan(2, 0, 0);
            pfdcCountdown = new PersitentFDCCountdown(fdctestname, dtTwoHourAgo, new TimeSpan(20, 0, 0));
            Assert.AreEqual(fdctestname, pfdcCountdown.FDCName);
            Assert.AreEqual(20.0, pfdcCountdown.InitialCountTime.TotalHours);
            Assert.AreEqual(dtNow - new TimeSpan(2, 0, 0), pfdcCountdown.ResetDate);
            Assert.IsTrue(18.0 >= pfdcCountdown.CountdownHours);

            string fullPath = Path.GetFullPath(@".\tempPfdccnt.xml");
            XML.Serialize(pfdcCountdown, fullPath);

            var pfdcdateres = XML.Deserialize<PersitentFDCCountdown>(fullPath);
            Assert.AreEqual(fdctestname, pfdcdateres.FDCName);
            Assert.AreEqual(20.0, pfdcCountdown.InitialCountTime.TotalHours);
            Assert.AreEqual(dtNow - new TimeSpan(2, 0, 0), pfdcdateres.ResetDate);
            Assert.IsTrue(18.0 >= pfdcCountdown.CountdownHours);
            using (var ms = new MemoryStream())
            {
                var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formatter.Serialize(ms, pfdcdateres);

                ms.Position = 0;
                var deserialized = (PersitentFDCCountdown)formatter.Deserialize(ms);

                Assert.AreEqual(pfdcdateres.FDCName, deserialized.FDCName);
                Assert.AreEqual(pfdcdateres.InitialCountTime.TotalHours, deserialized.InitialCountTime.TotalHours);
                Assert.AreEqual(pfdcdateres.ResetDate, deserialized.ResetDate);
                Assert.IsTrue(18.0 >= deserialized.CountdownHours);
            }
            File.Delete(fullPath);

        }

        #endregion
    }
}
