using System;
using System.Runtime.InteropServices;

using UnitySC.EFEM.Controller.HostInterface;

namespace UnitySC.EFEM.Controller
{
    internal class TimeUpdater
    {
        [DllImport("kernel32.dll")]
        private static extern bool SetSystemTime([MarshalAs(UnmanagedType.LPStruct)] SYSTEMTIME systemtime);
        public static int TimeFormatInDigits = Constants.TimeFormat16Digits;

        [StructLayout(LayoutKind.Sequential)]
        private class SYSTEMTIME
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMilliseconds;
        }

        public static bool SetTime(DateTime dateValue)
        {
            TimeZone timeZone = TimeZone.CurrentTimeZone;
            TimeSpan timeOffset = timeZone.GetUtcOffset(DateTime.Now);

            SYSTEMTIME st = new SYSTEMTIME();
            try
            {
                st.wYear = (ushort)dateValue.Year;
                st.wMonth = (ushort)dateValue.Month;
                st.wDayOfWeek = 0;
                st.wDay = (ushort)dateValue.Day;

                ushort lhours = (ushort)dateValue.Hour;
                int offset = timeOffset.Hours;

                st.wHour = Convert.ToUInt16((Math.Abs((lhours >= Math.Abs(offset)) ? (lhours - offset) : 24 - (offset - lhours))) % 24);
                st.wMinute = (ushort)dateValue.Minute;
                st.wSecond = (ushort)dateValue.Second;
                st.wMilliseconds = 0;
            }
            catch (Exception)
            {
                return false;
            }
            return SetSystemTime(st);
        }

        public static bool SetTime(string Time)
        {
            //TimeUpdater.TimeFormatInDigits = Time.Length;
            if (TimeFormatInDigits == Constants.TimeFormat16Digits)
            {
                TimeZone timeZone = TimeZone.CurrentTimeZone;
                TimeSpan timeOffset = timeZone.GetUtcOffset(DateTime.Now);

                SYSTEMTIME st = new SYSTEMTIME();
                try
                {
                    st.wYear = Convert.ToUInt16(Time.Substring(0, 4));
                    st.wMonth = Convert.ToUInt16(Time.Substring(4, 2));
                    st.wDayOfWeek = 0;
                    st.wDay = Convert.ToUInt16(Time.Substring(6, 2));

                    ushort hours = Convert.ToUInt16(Time.Substring(8, 2));
                    int offset = timeOffset.Hours;

                    st.wHour = Convert.ToUInt16((Math.Abs((hours >= Math.Abs(offset)) ? (hours - offset) : 24 - (offset - hours))) % 24);
                    st.wMinute = Convert.ToUInt16(Time.Substring(10, 2));
                    st.wSecond = Convert.ToUInt16(Time.Substring(12, 2));
                    st.wMilliseconds = Convert.ToUInt16(Time.Substring(14, 2));
                }
                catch (Exception)
                {
                    return false;
                }
                return SetSystemTime(st);
            }
            else if (TimeUpdater.TimeFormatInDigits == Constants.TimeFormat12Digits)
            {
                TimeZone timeZone = TimeZone.CurrentTimeZone;
                TimeSpan timeOffset = timeZone.GetUtcOffset(DateTime.Now);

                SYSTEMTIME st = new SYSTEMTIME();
                try
                {
                    st.wYear = Convert.ToUInt16("20" + Time.Substring(0, 2));
                    st.wMonth = Convert.ToUInt16(Time.Substring(2, 2));
                    st.wDayOfWeek = 0;
                    st.wDay = Convert.ToUInt16(Time.Substring(4, 2));

                    ushort hours = Convert.ToUInt16(Time.Substring(6, 2));
                    int offset = timeOffset.Hours;

                    st.wHour = Convert.ToUInt16((Math.Abs((hours >= Math.Abs(offset)) ? (hours - offset) : 24 - (offset - hours))) % 24);
                    st.wMinute = Convert.ToUInt16(Time.Substring(8, 2));
                    st.wSecond = Convert.ToUInt16(Time.Substring(10, 2));
                    st.wMilliseconds = 0;
                }
                catch (Exception)
                {
                    return false;
                }
                return SetSystemTime(st);
            }
            else
            {
                return false;
            }
        }


        public static string GetTime()
        {
            string time;

            //if (is16ByteForm)
            //    time = DateTime.Now.ToString("yyyyMMddHHmmssff");
            //else
            //    time = DateTime.Now.ToString("yyMMddHHmmss");
            if (TimeFormatInDigits == Constants.TimeFormat16Digits)
                time = DateTime.Now.ToString("yyyyMMddHHmmssff");
            else
                time = DateTime.Now.ToString("yyMMddHHmmss");

            return time;
        }
    }
}
