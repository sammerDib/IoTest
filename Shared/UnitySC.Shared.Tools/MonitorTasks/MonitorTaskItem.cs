using System;
using System.Globalization;


namespace UnitySC.Shared.Tools.MonitorTasks
{
    public enum TaskMoment { ukwn, Start, End }

    public class MonitorTaskItem : IFormattable, ICloneable
    {
        public static string DefaultFormat = "S";


        private TimeSpan _ts;
        public TimeSpan TSpan
        {
            get { return _ts; }
            private set { _ts = value; }
        }

        private string _lbl;
        public string Label
        {
            get { return _lbl; }
            private set { _lbl = value; }
        }
        public string FullLabel => $"{_lbl}_{_iD}";

        private TaskMoment _mom;
        public TaskMoment Moment
        {
            get { return _mom; }
            private set { _mom = value; }
        }

        private long _iD;
        public long ID
        {
            get { return _iD; }
            private set { _iD = value; }
        }

        public MonitorTaskItem(TimeSpan ts, string label, TaskMoment moment, long lUniqueId)
        {
            TSpan = ts;
            _lbl = label;
            _mom = moment;
            _iD = lUniqueId;
        }

        static public string Titles(string format)
        {
            if (String.IsNullOrEmpty(format)) format = DefaultFormat;

            switch (format)
            {
                case "S":
                    return string.Format("Time in S;Moment;Label;Id;");
                case "M":
                    return string.Format("Time in M;Moment;Label;Id;");
                case "mS":
                    return string.Format("Time in mS;Moment;Label;Id;");
                case "NOts":
                    return string.Format("Moment;Label;Id;");
                default:
                    throw new FormatException(string.Format("The {0} format string is not supported.", format));
            }
        }

        public static bool TryParse(string str, out MonitorTaskItem item, string format = null, IFormatProvider provider = null)
        {
            if (string.IsNullOrEmpty(format)) format = DefaultFormat;
            if (provider == null) provider = CultureInfo.CurrentCulture;

            bool bSucess = false;
            item = null;
            try
            {
                string mylbl;
                TaskMoment mymom;
                long myid;
                double dTime;
                TimeSpan myts;

                string[] split = str.Split(new char[] { ';', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length == 4)
                {

                    if (!double.TryParse(split[0], NumberStyles.Number, provider, out dTime))
                        throw new FormatException(string.Format("Try parse error : Bad Timespan ({0})", split[0]));


                    switch (format)
                    {
                        case "S":
                            myts = TimeSpan.FromSeconds(dTime);
                            break;
                        case "M":
                            myts = TimeSpan.FromMinutes(dTime);
                            break;
                        case "mS":
                            myts = TimeSpan.FromMilliseconds(dTime);
                            break;
                        default:
                            throw new FormatException(string.Format("Try parse error : The {0} format string is not supported.", format));
                    }

                    if (!Enum.TryParse<TaskMoment>(split[1], out mymom))
                        throw new FormatException(string.Format("Try parse error : Bad Moment ({0})", split[1]));

                    mylbl = split[2];

                    if (!long.TryParse(split[3], out myid))
                        myid = 666;

                    item = new MonitorTaskItem(myts, mylbl, mymom, myid);
                    bSucess = true;
                }
                else
                {
                    throw new FormatException(string.Format("Try parse error in Splits."));
                }

            }
#pragma warning disable 0168
            catch (Exception ex)
#pragma warning restore 0168
            {
                bSucess = false;
                item = null;
                throw;
            }
            return bSucess;
        }

        public override string ToString()
        {
            return this.ToString(DefaultFormat, CultureInfo.CurrentCulture);
        }

        public string ToString(string format)
        {
            return this.ToString(format, CultureInfo.CurrentCulture);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format)) format = DefaultFormat;
            if (formatProvider == null) formatProvider = CultureInfo.CurrentCulture;

            switch (format)
            {
                case "S":
                    return string.Format("{0};{1};{2};{3};", _ts.TotalSeconds.ToString("F3", formatProvider), _mom.ToString(), _lbl, _iD.ToString("G", formatProvider));
                case "M":
                    return string.Format("{0};{1};{2};{3};", _ts.TotalMinutes.ToString("F3", formatProvider), _mom.ToString(), _lbl, _iD.ToString("G", formatProvider));
                case "mS":
                    return string.Format("{0};{1};{2};{3};", _ts.TotalMilliseconds.ToString("F3", formatProvider), _mom.ToString(), _lbl, _iD.ToString("G", formatProvider));
                case "NOts":
                    return string.Format("{0};{1};{2};", _mom.ToString(), _lbl, _iD.ToString("G", formatProvider));
                default:
                    throw new FormatException($"The <{format}> format string is not supported.");
            }
        }

        public object Clone()
        {
            MonitorTaskItem cloned = this.MemberwiseClone() as MonitorTaskItem;
            if (this.TSpan != null)
                cloned.TSpan = this.TSpan.Duration();
            return cloned;
        }
    }
}
