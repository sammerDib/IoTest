using System;
using System.Globalization;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UnitySC.Shared.Tools.MonitorTasks;

namespace UnitySC.Shared.Tools.Test.MonitorTasks
{
    [TestClass]
    public class UnitMonitorTasksTest
    {
        [TestMethod]
        public void TestMonitorItem_Constructor()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            string lbl1 = "lbl1";
            TimeSpan ts1 = new TimeSpan(0, 0, 1, 20, 255);
            double ts1ms = ts1.TotalMilliseconds;
            double ts1s = ts1.TotalSeconds;
            double ts1M = ts1.TotalMinutes;
            long lId = 123L;
            MonitorTaskItem Item1 = new MonitorTaskItem(ts1, lbl1, TaskMoment.Start, lId);

            Assert.AreEqual(lbl1, Item1.Label);
            Assert.AreEqual(ts1, Item1.TSpan);
            Assert.AreEqual(TaskMoment.Start, Item1.Moment);
            Assert.AreEqual(lId, Item1.ID);
            Assert.AreEqual($"{lbl1}_{lId}", Item1.FullLabel);
        }

        [TestMethod]
        public void TestMonitorItem_Formatable()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            string lbl1 = "lbl-format";
            TimeSpan ts1 = new TimeSpan(0, 0, 3, 48, 86);
            double ts1ms = ts1.TotalMilliseconds;
            double ts1s = ts1.TotalSeconds;
            double ts1M = ts1.TotalMinutes;
            long lId = 3L;
            MonitorTaskItem Item1 = new MonitorTaskItem(ts1, lbl1, TaskMoment.End, lId);

            Assert.AreEqual(lbl1, Item1.Label);
            Assert.AreEqual(ts1, Item1.TSpan);
            Assert.AreEqual(TaskMoment.End, Item1.Moment);
            Assert.AreEqual(lId, Item1.ID);
            Assert.AreEqual($"{lbl1}_{lId}", Item1.FullLabel);

            // formatable
            string strnots = string.Format("{0};{1};{2};", TaskMoment.End.ToString(), lbl1, lId.ToString("G"));
            string sec = string.Format("{0};{1}", ts1s.ToString("F3"), strnots);
            string msec = string.Format("{0};{1}", ts1ms.ToString("F3"), strnots);
            string min = string.Format("{0};{1}", ts1M.ToString("F3"), strnots);

            Assert.AreEqual(strnots, Item1.ToString("NOts"));

            // if default is Second
            Assert.AreEqual(sec, Item1.ToString());
            Assert.AreEqual(sec, Item1.ToString("S"));
            Assert.AreEqual(msec, Item1.ToString("mS"));
            Assert.AreEqual(min, Item1.ToString("M"));
        }


        [TestMethod]
        public void TestMonitorItem_Clone()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            string lbl1 = "lbl_Clone";
            TimeSpan ts1 = new TimeSpan(0, 0, 0, 45, 326);
            double ts1ms = ts1.TotalMilliseconds;
            double ts1s = ts1.TotalSeconds;
            double ts1M = ts1.TotalMinutes;
            long lId = 69L;
            MonitorTaskItem Item1 = new MonitorTaskItem(ts1, lbl1, TaskMoment.Start, lId);

            Assert.AreEqual(lbl1, Item1.Label);
            Assert.AreEqual(ts1, Item1.TSpan);
            Assert.AreEqual(TaskMoment.Start, Item1.Moment);
            Assert.AreEqual(lId, Item1.ID);
            Assert.AreEqual($"{lbl1}_{lId}", Item1.FullLabel);

            string strnots = string.Format("{0};{1};{2};", TaskMoment.Start.ToString(), lbl1, lId.ToString("G"));
            string sec = string.Format("{0};{1}", ts1s.ToString("F3"), strnots);
            Assert.AreEqual(sec, Item1.ToString());

            MonitorTaskItem Item2 = Item1.Clone() as MonitorTaskItem;
            Assert.AreEqual(lbl1, Item2.Label);
            Assert.AreEqual(ts1, Item2.TSpan);
            Assert.AreEqual(Item1.Moment, Item2.Moment);
            Assert.AreEqual(lId, Item1.ID);
            Assert.AreEqual($"{lbl1}_{lId}", Item2.FullLabel);
        }

        [TestMethod]
        public void TestMonitorItem_TryParse()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            string lbl1 = "lbl_TryParse";
            TimeSpan ts1 = new TimeSpan(0, 0, 12, 45, 326);
            double ts1mS = ts1.TotalMilliseconds;
            long lId = 5L;
            MonitorTaskItem Item1 = new MonitorTaskItem(ts1, lbl1, TaskMoment.End, lId);

            Assert.AreEqual(lbl1, Item1.Label);
            Assert.AreEqual(ts1, Item1.TSpan);
            Assert.AreEqual(TaskMoment.End, Item1.Moment);
            Assert.AreEqual(lId, Item1.ID);
            Assert.AreEqual($"{lbl1}_{lId}", Item1.FullLabel);

            string strnots = string.Format("{0};{1};{2};", TaskMoment.End.ToString(), lbl1, lId.ToString("G"));
            string ms = string.Format("{0};{1}", ts1mS.ToString("F3"), strnots);
  
            Assert.AreEqual(ms, Item1.ToString("mS"));

            bool success = MonitorTaskItem.TryParse(ms, out MonitorTaskItem ItemParsed, "mS");
            Assert.IsTrue(success);
            Assert.AreEqual(lbl1, ItemParsed.Label);
            Assert.AreEqual(ts1, ItemParsed.TSpan);
            Assert.AreEqual(Item1.Moment, ItemParsed.Moment);
            Assert.AreEqual(lId, ItemParsed.ID);
            Assert.AreEqual($"{lbl1}_{lId}", ItemParsed.FullLabel);

            Assert.ThrowsException<FormatException>(() => MonitorTaskItem.TryParse("12;3;", out MonitorTaskItem ItemParsedFail)); 

        }

        [TestMethod]
        public void TestMonitorTaskTimer_tagTimespan()
        {
            MonitorTaskTimer tt = new MonitorTaskTimer();
            tt.Fmt = "mS";
            Assert.IsFalse(tt.IsActive);

            tt.FreshStart();

            Assert.IsTrue(tt.IsActive);

            tt.Tag_ts(new TimeSpan(0, 0, 0, 0, 003), "SpindleAccel", TaskMoment.End, 10);

            tt.Tag_ts(new TimeSpan(0, 0, 0, 0, 532), "Acquisition", TaskMoment.Start, 0);
            tt.Tag_ts(new TimeSpan(0, 0, 0, 4, 142), "SetUp", TaskMoment.Start, 0);

            tt.Tag_ts(new TimeSpan(0, 0, 0, 5, 142), "StageMotionX", TaskMoment.Start, 0);
            tt.Tag_ts(new TimeSpan(0, 0, 0, 6, 422), "StageMotionX", TaskMoment.End, 0);

            tt.Tag_ts(new TimeSpan(0, 0, 0, 14, 342), "SetUp", TaskMoment.End, 0);

            tt.Tag_ts(new TimeSpan(0, 0, 0, 14, 343), "StageMotionX", TaskMoment.Start, 1);
            tt.Tag_ts(new TimeSpan(0, 0, 0, 14, 343), "StageMotionZ", TaskMoment.Start, 0);

            tt.Tag_ts(new TimeSpan(0, 0, 0, 15, 02), "StageMotionZ", TaskMoment.End, 0);
            tt.Tag_ts(new TimeSpan(0, 0, 0, 16, 422), "StageMotionX", TaskMoment.End, 1);


            tt.Tag_ts(new TimeSpan(0, 0, 0, 16, 342), "SpectrumGrab", TaskMoment.Start, 5);
            tt.Tag_ts(new TimeSpan(0, 0, 0, 24, 942), "SpectrumGrab", TaskMoment.End, 5);

            tt.Tag_ts(new TimeSpan(0, 0, 0, 24, 943), "StageMotionX", TaskMoment.Start, 2);

            tt.Tag_ts(new TimeSpan(0, 0, 1, 25, 56), "Acquisition", TaskMoment.End, 0);

            Assert.IsTrue(tt.IsActive);

            tt.Stop();

            Assert.IsFalse(tt.IsActive);

            bool success = tt.SaveMonitorCSV(@".\Test_ts.mtt");
            Assert.IsTrue(success);


            var ttLoad = new MonitorTaskTimer();

            bool fail =  ttLoad.ReadMonitorCSV(@".\Test_NOT_EXiST.mtt", out string errormsg);
            Assert.IsFalse(fail);
            Assert.IsFalse(ttLoad.IsActive);

            ttLoad.ReadMonitorCSV(@".\Test_ts.mtt", out errormsg);
            Assert.IsTrue(success);
            Assert.IsFalse(ttLoad.IsActive);

            int i = 0;
            foreach (var item in ttLoad.Items)
            {  
                Assert.AreEqual(tt.Items[i].ToString(), item.ToString(), $"item match fail at idx = {i}");
                i++;
            }
        }

        [TestMethod]
        public void TestMonitorTaskTimer_tagReal()
        {
            MonitorTaskTimer tt = new MonitorTaskTimer();
            tt.Fmt = "mS";

            Assert.IsFalse(tt.IsActive);

            tt.Tag_Start("NotActive");

            tt.FreshStart();

            Assert.IsTrue(tt.IsActive);

            tt.Tag_End("NotActive");
            tt.Tag_Start("Active");

            var rnd = new Random();
            for (int i = 0; i < 3; i++)
            {
                tt.Tag_Start("Loop1", i);
                for (int j = 0; j < 2; j++)
                {
                    tt.Tag_Start("Loop2", j);
                    double acc = 0.0;
                    int n = rnd.Next(5000,50000);
                    for (int k = 0; k < n; k++)
                    {
                        acc += rnd.NextDouble();
                    }
                    tt.Tag_End("Loop2", j);
                }
               tt.Tag_End("Loop1", i);
            }

            tt.Tag_End("Active");

            bool success = tt.SaveMonitorCSV(@".\Test_tagreal.mtt");
            Assert.IsTrue(success);
            Assert.IsFalse(tt.IsActive);

            Assert.AreEqual(3 + 3 * ( 2 + 2 * 2 ), tt.Items.Count);
        }

    }
}
