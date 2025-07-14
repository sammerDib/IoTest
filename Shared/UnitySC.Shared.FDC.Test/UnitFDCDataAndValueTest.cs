using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.Shared.Data.FDC;
using UnitySC.Shared.Tools;

namespace UnitySC.Shared.FDC.Test
{
    [TestClass]
    public class UnitFDCDataAndValueTest
    {
        private void AssertWCFContract<T>(T expected, FDCValue fdcvalue)
        {
            string testfile = $"testOf{typeof(T).ToString()}.wcf";
            Assert.IsTrue(fdcvalue.Value.GetType() == typeof(T), $"fdcval is not of Type <{typeof(T)}>");
            Assert.AreEqual(expected, fdcvalue.Value, $"fdcval value differs");

            XML.DatacontractSerialize(fdcvalue, testfile);
            var wcfFdValue = XML.DatacontractDeserialize<FDCValue>(testfile);

            Assert.IsTrue(wcfFdValue.Value.GetType() == typeof(T), $"WCF FdcValue is not of Type <{typeof(T)}>");
            Assert.AreEqual(expected, wcfFdValue.Value, $"WCF FdcValue differs");

            System.IO.File.Delete(testfile);
        }

        [TestMethod]
        public void TestFDCValues_KnownType()
        {
            var nullVal = new FDCValue();
            Assert.IsNotNull(nullVal);
            Assert.IsNull(nullVal.Value);

            var fdcVal = new FDCValue();
            Assert.IsNotNull(fdcVal);
            Assert.IsNull(fdcVal.Value);

            //[KnownType(typeof(char))]
            fdcVal.Value = 'k';
            Assert.IsTrue(fdcVal.Value.GetType() == typeof(char));
            Assert.AreEqual('k', fdcVal.Value);
            AssertWCFContract('k', fdcVal);

            //[KnownType(typeof(byte))]
            fdcVal.Value = (byte)56;
            Assert.IsTrue(fdcVal.Value.GetType() == typeof(byte));
            Assert.AreEqual((byte)56, fdcVal.Value);
            byte ucVal = 135 ;
            fdcVal.Value = ucVal;
            Assert.IsTrue(fdcVal.Value.GetType() == typeof(byte));
            Assert.AreEqual((byte)135, fdcVal.Value);
            AssertWCFContract((byte)135, fdcVal);

            //[KnownType(typeof(short))]
            fdcVal.Value = (short)16543;
            Assert.IsTrue(fdcVal.Value.GetType() == typeof(short));
            Assert.AreEqual((short)16543, fdcVal.Value);
            short sVal = -25687;
            fdcVal.Value = sVal;
            Assert.IsTrue(fdcVal.Value.GetType() == typeof(short));
            Assert.AreEqual((short)-25687, fdcVal.Value);
            AssertWCFContract((short)-25687, fdcVal);

            //[KnownType(typeof(ushort))]
            fdcVal.Value = (ushort) 35000;
            Assert.IsTrue(fdcVal.Value.GetType() == typeof(ushort));
            Assert.AreEqual((ushort)35000, fdcVal.Value);
            AssertWCFContract((ushort)35000, fdcVal);

            //[KnownType(typeof(int))]
            fdcVal.Value = 68123;
            Assert.IsTrue(fdcVal.Value.GetType() == typeof(int));
            Assert.AreEqual(68123, fdcVal.Value);
            int nVal = -35456;
            fdcVal.Value = nVal;
            Assert.IsTrue(fdcVal.Value.GetType() == typeof(int));
            Assert.AreEqual(-35456, fdcVal.Value);
            AssertWCFContract(-35456, fdcVal);

            //[KnownType(typeof(uint))]
            fdcVal.Value = 1236546u;
            Assert.IsTrue(fdcVal.Value.GetType() == typeof(uint));
            Assert.AreEqual(1236546u, fdcVal.Value);
            uint uVal = 456u;
            fdcVal.Value = uVal;
            Assert.IsTrue(fdcVal.Value.GetType() == typeof(uint));
            Assert.AreEqual(456u, fdcVal.Value);
            AssertWCFContract(uVal, fdcVal);

            //[KnownType(typeof(long))]
            fdcVal.Value = -1234567890L;
            Assert.IsTrue(fdcVal.Value.GetType() == typeof(long));
            Assert.AreEqual(-1234567890L, fdcVal.Value);
            AssertWCFContract(-1234567890L, fdcVal);

            //[KnownType(typeof(ulong))]
            fdcVal.Value = 1234567890UL;
            Assert.IsTrue(fdcVal.Value.GetType() == typeof(ulong));
            Assert.AreEqual(1234567890UL, fdcVal.Value);
            AssertWCFContract(1234567890UL, fdcVal);

            //[KnownType(typeof(decimal))]
            fdcVal.Value = 12.456m;
            Assert.IsTrue(fdcVal.Value.GetType() == typeof(decimal));
            Assert.AreEqual(12.456m, fdcVal.Value);
            AssertWCFContract(12.456m, fdcVal);

            //[KnownType(typeof(float))]
            fdcVal.Value = 123.456f;
            Assert.IsTrue(fdcVal.Value.GetType() == typeof(float));
            Assert.AreEqual(123.456f, fdcVal.Value);
            AssertWCFContract(123.456f, fdcVal);

            //[KnownType(typeof(double))]
            fdcVal.Value = 987654123.456987;
            Assert.IsTrue(fdcVal.Value.GetType() == typeof(double));
            Assert.AreEqual(987654123.456987, fdcVal.Value);
            AssertWCFContract(987654123.456987, fdcVal);

            //[KnownType(typeof(string))]
            fdcVal.Value = "#My Value #";
            Assert.IsTrue(fdcVal.Value.GetType() == typeof(string));
            Assert.AreEqual("#My Value #", fdcVal.Value);
            string strVal = "MyVariableStrValue";
            fdcVal.Value = strVal;
            Assert.IsTrue(fdcVal.Value.GetType() == typeof(string));
            Assert.AreEqual(strVal, fdcVal.Value);
            AssertWCFContract(strVal, fdcVal);

            //[KnownType(typeof(DateTime))]
            DateTime dtVal = new DateTime(2012, 06, 12, 22, 45, 35);
            fdcVal.Value = dtVal;
            Assert.IsTrue(fdcVal.Value.GetType() == typeof(DateTime));
            Assert.AreEqual(dtVal, fdcVal.Value);
            AssertWCFContract(dtVal, fdcVal);
            Assert.AreEqual(new DateTime(2012, 06, 12, 22, 45, 35), fdcVal.Value);

            //[KnownType(typeof(TimeSpan))]
            TimeSpan ts = new TimeSpan(6, 42, 15);
            fdcVal.Value = ts;
            Assert.IsTrue(fdcVal.Value.GetType() == typeof(TimeSpan));
            Assert.AreEqual(ts, fdcVal.Value);
            AssertWCFContract(ts, fdcVal);
            Assert.AreEqual(ts.TotalSeconds, ((TimeSpan)(fdcVal.Value)).TotalSeconds);
        }

        [TestMethod]
        public void TestFDCData_MakeNewArgument()
        {
            int i = 0;
            string unit = "%";
            var dtnow = DateTime.Now;
            var dt = dtnow - new TimeSpan(365, 0, 0, 1);
            FDCData fdcData;

            // all arguments
            fdcData = FDCData.MakeNew($"fdcItem{++i}", i, unit, dt);
            Assert.IsNotNull(fdcData);
            Assert.IsNotNull(fdcData.ValueFDC);
            Assert.AreEqual($"fdcItem{i}", fdcData.Name);
            Assert.AreEqual(i, fdcData.ValueFDC.Value);
            Assert.IsFalse(String.IsNullOrEmpty(fdcData.Unit));
            Assert.AreEqual(unit, fdcData.Unit);
            Assert.IsTrue((dtnow - fdcData.Date).TotalDays >= 365);


            // No Datetime
            dtnow = DateTime.Now;
            fdcData = FDCData.MakeNew($"fdcItem{++i}", i, unit);
            Assert.IsNotNull(fdcData);
            Assert.IsNotNull(fdcData.ValueFDC);
            Assert.AreEqual($"fdcItem{i}", fdcData.Name);
            Assert.AreEqual(i, fdcData.ValueFDC.Value);
            Assert.IsFalse(String.IsNullOrEmpty(fdcData.Unit));
            Assert.AreEqual(unit, fdcData.Unit);
            Assert.IsTrue((dtnow - fdcData.Date).TotalSeconds <= 2.0);

            // No unit, No Datetime
            dtnow = DateTime.Now;
            fdcData = FDCData.MakeNew($"fdcItem{++i}", i);
            Assert.IsNotNull(fdcData);
            Assert.IsNotNull(fdcData.ValueFDC);
            Assert.AreEqual($"fdcItem{i}", fdcData.Name);
            Assert.AreEqual(i, fdcData.ValueFDC.Value);
            Assert.IsTrue(String.IsNullOrEmpty(fdcData.Unit));
            Assert.IsTrue((dtnow - fdcData.Date).TotalSeconds <= 2.0);

            // No unit, but with some Datetime
            dtnow = DateTime.Now;
            fdcData = FDCData.MakeNew($"fdcItem{++i}", i, null, dt);
            Assert.IsNotNull(fdcData);
            Assert.IsNotNull(fdcData.ValueFDC);
            Assert.AreEqual($"fdcItem{i}", fdcData.Name);
            Assert.AreEqual(i, fdcData.ValueFDC.Value);
            Assert.IsTrue(String.IsNullOrEmpty(fdcData.Unit));
            Assert.IsTrue((dtnow - fdcData.Date).TotalDays >= 365);

        }

        [TestMethod]
        public void TestFDCData_MakeNewInt()
        {
            Random rnd = new Random();
            for (int i = 0; i < 10; i++)
            {
                int val = rnd.Next();

                var fdcData = FDCData.MakeNew($"fdcItem{i}", val);
                Assert.IsNotNull(fdcData);
                Assert.IsNotNull(fdcData.ValueFDC);
                Assert.AreEqual($"fdcItem{i}", fdcData.Name);

                Assert.AreEqual(val, fdcData.ValueFDC.Value);
            }
        }

        [TestMethod]
        public void TestFDCData_MakeNewDouble()
        {
            Random rnd = new Random();
            for (int i = 0; i < 10; i++)
            {
                double val = 1024.0 * rnd.NextDouble();

                var fdcData = FDCData.MakeNew($"fdcItem{i}", val);
                Assert.IsNotNull(fdcData);
                Assert.IsNotNull(fdcData.ValueFDC);
                Assert.AreEqual($"fdcItem{i}", fdcData.Name);

                Assert.AreEqual(val, fdcData.ValueFDC.Value);
            }
        }

        [TestMethod]
        public void TestFDCData_MakeNewByte()
        {
            Random rnd = new Random();
            for (int i = 0; i < 10; i++)
            {
                byte val = (byte) rnd.Next(0,255);

                var fdcData = FDCData.MakeNew($"fdcItem{i}", val);
                Assert.IsNotNull(fdcData);
                Assert.IsNotNull(fdcData.ValueFDC);
                Assert.AreEqual($"fdcItem{i}", fdcData.Name);

                Assert.AreEqual(val, fdcData.ValueFDC.Value);
            }
        }

        [TestMethod]
        public void TestFDCData_MakeNewULong()
        {
            Random rnd = new Random();
            for (int i = 0; i < 10; i++)
            {
                ulong val = (ulong)(99999999.9 * rnd.NextDouble());

                var fdcData = FDCData.MakeNew($"fdcItem{i}", val);
                Assert.IsNotNull(fdcData);
                Assert.IsNotNull(fdcData.ValueFDC);
                Assert.AreEqual($"fdcItem{i}", fdcData.Name);

                Assert.AreEqual(val, fdcData.ValueFDC.Value);
            }

        }
    }
}
