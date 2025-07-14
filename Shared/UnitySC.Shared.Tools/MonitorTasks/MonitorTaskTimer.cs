using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace UnitySC.Shared.Tools.MonitorTasks
{
    public class MonitorTaskTimer
    {
        private readonly Stopwatch _gsw = new Stopwatch();

        public List<MonitorTaskItem> Items { get { return _tskList; } }
        public string Fmt = string.Empty;
        private List<MonitorTaskItem> _tskList = new List<MonitorTaskItem>(256000);
        private object _tlLock = new object();

        public bool IsActive { get; private set; }

        public MonitorTaskTimer()
        {
            IsActive = false;
        }

        public void FreshStart()
        {
            IsActive = false;
            lock (_tlLock)
            {
                _tskList.Clear();
            }
            _gsw.Restart();
            IsActive = true;
        }

        public void Stop()
        {
            IsActive = false;
            _gsw.Stop();
        }

        public void Clear()
        {
            if (IsActive)
            {
                Stop();
            }
            lock (_tlLock)
            {
                _tskList.Clear();
            }
        }

        public TimeSpan GetEllapsed()
        {
            return _gsw.Elapsed;
        }


        public void Tag_ts(TimeSpan ts, string lbl, TaskMoment mom, long lID)
        {
            lock (_tlLock)
            {
                _tskList.Add(new MonitorTaskItem(ts, lbl, mom, lID));
            }
        }

        public void Tag_ts(MonitorTaskItem item)
        {
            lock (_tlLock)
            {
                _tskList.Add(item);
            }
        }

        public void Tag_Start(string lbl, long lId = 0)
        {
            if (IsActive)
            {
                Tag_ts(_gsw.Elapsed, lbl, TaskMoment.Start, lId);
            }
        }

        public void Tag_End(string lbl, long lId = 0)
        {
            if (IsActive)
            {
                Tag_ts(_gsw.Elapsed, lbl, TaskMoment.End, lId);
            }
        }

        public void Tag_StartAndEnd(string lbl, ulong start_Ellapsed_ms, ulong end_Ellapsed_ms, long lId = 0)
        {
            if (IsActive)
            {
                lock (_tlLock)
                {
                    _tskList.Add(new MonitorTaskItem(TimeSpan.FromMilliseconds(start_Ellapsed_ms), lbl, TaskMoment.Start, lId));
                    _tskList.Add(new MonitorTaskItem(TimeSpan.FromMilliseconds(end_Ellapsed_ms), lbl, TaskMoment.End, lId));
                }
            }
        }

        public bool SaveMonitorCSV(string sFilePath)
        {
            bool isSuccess = false;
            try
            {
                if (IsActive)
                {
                    Stop();
                }

                using (StreamWriter sw = new StreamWriter(sFilePath, false))
                {
                    string fmt = string.IsNullOrEmpty(Fmt) ? MonitorTaskItem.DefaultFormat : Fmt; //millisec
                    sw.WriteLine(MonitorTaskItem.Titles(fmt));
                    lock (_tlLock)
                    {
                        foreach (MonitorTaskItem item in _tskList)
                        {
                            sw.WriteLine(item.ToString(fmt, System.Globalization.CultureInfo.InvariantCulture));
                        }
                    }
                    sw.Flush();
                }
                isSuccess = true;
            }
#pragma warning disable 0168
            catch (Exception ex)
#pragma warning restore 0168
            {
                isSuccess = false;
            }
            return isSuccess;
        }

        public bool ReadMonitorCSV(string sFilePath, out string errmsg)
        {
            errmsg = string.Empty;
            bool isSuccess = false;
            try
            {
                if (IsActive)
                {
                    Stop();
                }

                lock (_tlLock)
                {
                    _tskList.Clear();
                }

                string fmt = MonitorTaskItem.DefaultFormat; // par défault
                using (StreamReader sr = new StreamReader(sFilePath))
                {

                    string sTitles = sr.ReadLine(); // Title
                    if (sTitles != null)
                    {
                        String[] spl = sTitles.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        if (spl.Length >= 1)
                        {
                            String[] spl2 = spl[0].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (spl2.Length == 3)
                            {
                                fmt = spl2[2];
                            }
                            else
                                throw new Exception($"Cannot read Title format item from : {sTitles}");

                        }

                        while (sr.Peek() >= 0)
                        {
                            string sline = sr.ReadLine();
                            if (sline != null)
                            {
                                MonitorTaskItem it;
                                // Throw FormatException if fail
                                MonitorTaskItem.TryParse(sline, out it, fmt, System.Globalization.CultureInfo.InvariantCulture);

                                lock (_tlLock)
                                {
                                    _tskList.Add(it);
                                }
                            }
                        }
                    }
                    else
                        throw new Exception($"Empty file ? {sFilePath}");

                }

                Fmt = fmt;
                isSuccess = true;
            }
#pragma warning disable 0168
            catch (Exception ex)
#pragma warning restore 0168
            {
                errmsg = ex.Message;
                isSuccess = false;
            }
            return isSuccess;
        }
    }
}
