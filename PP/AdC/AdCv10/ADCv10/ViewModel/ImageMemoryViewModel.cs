using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

using AdcTools;

using CommunityToolkit.Mvvm.ComponentModel;


namespace ADC.ViewModel
{
    [System.Reflection.Obfuscation(Exclude = true)]
    internal class ImageMemoryViewModel : ObservableRecipient
    {
        public double Minimum { get { return 0; } }
        private double _maximum = Double.NaN;
        public double Maximum
        {
            get
            {
                if (Double.IsNaN(_maximum))
                {
                    _maximum = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory;
                }

                return _maximum;
            }
        }

        private double _watermark = 1;
        public double Watermark
        {
            get
            {
                // compute the next highest power of 2 of 32-bit
                long v = (long)_watermark;
                v--;
                for (int i = 0; i < 48; i++)
                    v |= v >> i;
                v++;
                return v;
            }
            set
            {
                _watermark = value;
                OnPropertyChanged(nameof(Watermark));
                OnPropertyChanged(nameof(Text));
            }
        }

        public double _value = 0;
        public double Value
        {
            get { return _value; }
            set
            {
                _value = value;
                if (_value > _watermark)
                    Watermark = _value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Text));
            }
        }

        public string Text
        {
            get
            {
                string v = SizeStringFormatter.SizeSuffix((long)_value);
                string m = SizeStringFormatter.SizeSuffix((long)_watermark);
                return "current = " + v + " / max reached = " + m;
            }
        }

        [DllImport("pdh.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern UInt32 PdhLookupPerfNameByIndex(string szMachineName, uint dwNameIndex, StringBuilder szNameBuffer, ref uint pcchNameBufferSize);

        private static PerformanceCounter GetSingleInstanceCounter(string englishCategoryName, string englishCounterName)
        {
            // Try first with english names
            try
            {
                return new PerformanceCounter(englishCategoryName, englishCounterName, Process.GetCurrentProcess().ProcessName);
            }
            catch { }

            // Get list of counters
            const string perfCountersKey = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Perflib\009";
            var englishNames = Microsoft.Win32.Registry.GetValue(perfCountersKey, "Counter", null) as string[];

            // Get localized category name
            var localizedCategoryId = FindNameId(englishNames, englishCategoryName);
            var localizedCategoryName = GetNameByIndex(localizedCategoryId);

            // Get localized counter name
            var localizedCounterId = FindNameId(englishNames, englishCounterName);
            var localizedCounterName = GetNameByIndex(localizedCounterId);

            return GetCounterIfExists(localizedCategoryName, localizedCounterName) ??
                   GetCounterIfExists(localizedCategoryName, englishCounterName) ??
                   GetCounterIfExists(englishCategoryName, localizedCounterName);
        }

        private static PerformanceCounter GetCounterIfExists(string categoryName, string counterName)
        {
            try
            {
                return new PerformanceCounter(categoryName, counterName, Process.GetCurrentProcess().ProcessName);
            }
            catch
            {
                return null;
            }
        }

        private static int FindNameId(string[] englishNames, string name)
        {
            // englishNames contains alternately id and name, that's why I check only odd lines
            for (int i = 1; i < englishNames.Length; i += 2)
            {
                if (englishNames[i] == name)
                {
                    return Int32.Parse(englishNames[i - 1]);
                }
            }

            return -1;
        }

        private static string GetNameByIndex(int id)
        {
            if (id < 0)
            {
                return null;
            }

            var buffer = new StringBuilder(1024);
            var bufSize = (uint)buffer.Capacity;
            var ret = PdhLookupPerfNameByIndex(null, (uint)id, buffer, ref bufSize);

            return ret == 0 && buffer.Length != 0 ? buffer.ToString() : null;
        }


        // private PerformanceCounter _performanceCounter = new PerformanceCounter("Process", "% Processor Time", Process.GetCurrentProcess().ProcessName);
        // Localized Perfomance counter
        private PerformanceCounter _performanceCounter = GetSingleInstanceCounter("Process", "% Processor Time");
        public double CpuLoad { get; private set; }
        public void UpdateCpuLoad()
        {
            CpuLoad = _performanceCounter.NextValue();
            OnPropertyChanged(nameof(CpuLoad));
            OnPropertyChanged(nameof(CpuLoadText));
        }

        public string CpuLoadText
        {
            get
            {
                return String.Format("{0, 4} %", (int)CpuLoad);
            }
        }

    }
}
