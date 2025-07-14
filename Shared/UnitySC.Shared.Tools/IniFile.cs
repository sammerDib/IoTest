using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace UnitySC.Shared.Tools
{
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Helper pour lire un fichier au format .INI
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    public class IniFile
    {
        private static class NativeMethods
        {
            [DllImport("kernel32")]
            public static extern int GetPrivateProfileString(string section,
                     string key, string def, System.Text.StringBuilder retVal,
                int size, string filePath);

            [DllImport("kernel32")]
            public static extern int GetPrivateProfileInt(string section,
                     string key, int def, string filePath);

            [DllImport("kernel32")]
            public static extern long WritePrivateProfileString(string Section,
                string Key, string Value, string FilePath);

            // for Read Section Keys
            [DllImport("kernel32")]
            public static extern int GetPrivateProfileString(string p_sSection, int p_nKey, string p_sValue, [MarshalAs(UnmanagedType.LPArray)] byte[] p_arResult, int p_nSize, string p_sFilePath);
        }

        public string Filename { get; private set; }

        //=================================================================
        // Constructeur
        //=================================================================
        public IniFile(string filename)
        {
            Filename = filename;
        }

        /// <summary>
        /// Write a value in ini file
        /// </summary>
        /// <param name="section">Section name</param>
        /// <param name="key">Key</param>
        /// <param name="value">string Value</param>
        public void Write(string section, string key, string value)
        {
            NativeMethods.WritePrivateProfileString(section, key, value, Filename);
        }

        /// <summary>
        /// Write a value in ini file
        /// </summary>
        /// <param name="section">Section name</param>
        /// <param name="key">Key</param>
        /// <param name="value">object Value (ToString() called)</param>
        public void Write(string section, string key, object value)
        {
            string str = value == null ? "" : value.ToString();
            NativeMethods.WritePrivateProfileString(section, key, str, Filename);
        }

        // The Function called to obtain the EntryKey's from the given
        // SectionHeader string passed and returns them in an Dynamic Array
        public string[] ReadSectionKeysNames(string p_sSection)
        {
            const int nCstMaxSizeBuf = 500;
            //    Sets the maxsize buffer to 500, if the more
            //    is required then doubles the size each time.
            for (int nMaxsize = nCstMaxSizeBuf; true; nMaxsize *= 2)
            {
                //    Obtains the EntryKey information in bytes
                //    and stores them in the maxsize buffer (Bytes array).
                //    Note that the SectionHeader value has been passed.
                byte[] bytes = new byte[nMaxsize];
                int size = NativeMethods.GetPrivateProfileString(p_sSection, 0, "", bytes, nMaxsize, Filename);

                // Check the information obtained is not bigger
                // than the allocated maxsize buffer - 2 bytes.
                // if it is, then skip over the next section
                // so that the maxsize buffer can be doubled.
                if (size < nMaxsize - 2)
                {
                    // Converts the bytes value into an ASCII char.
                    // This is one long string.
                    string entries = Encoding.ASCII.GetString(bytes, 0, size - (size > 0 ? 1 : 0));
                    // Splits the Long string into an array based on the "\0"
                    // or null (Newline) value and returns the value(s) in an array
                    return entries.Split(new char[] { '\0' });
                }
            }
        }

        //=================================================================
        // String
        //=================================================================
        public string GetString(string section, string key)
        {
            var sb = new System.Text.StringBuilder(1024);
            int n = NativeMethods.GetPrivateProfileString(section, key, null, sb, sb.Capacity, Filename);
            if (n > 0)
                return sb.ToString();
            else
                throw new Exception("missing section:[" + section + "] key:" + key + " in file " + Filename);
        }

        public string GetString(string section, string key, string defaultValue)
        {
            var sb = new System.Text.StringBuilder(1024);
            int n = NativeMethods.GetPrivateProfileString(section, key, null, sb, sb.Capacity, Filename);
            if (n > 0)
                return sb.ToString();
            else
                return defaultValue;
        }

        //=================================================================
        // Int
        //=================================================================
        public int GetInt(string section, string key)
        {
            string str = GetString(section, key);
            bool ok = int.TryParse(str, out int x);
            if (ok)
                return x;
            else
                throw new Exception("invalid value:\"" + str + "\" in section:[" + section + "] key:" + key + " in file " + Filename);
        }

        public int GetInt(string section, string key, int defaultValue)
        {
            string str = GetString(section, key, "");
            bool ok = int.TryParse(str, out int x);
            if (ok)
                return x;
            else
                return defaultValue;
        }

        //=================================================================
        // Float
        //=================================================================
        public float GetFloat(string section, string key)
        {
            string str = GetString(section, key);
            bool ok = float.TryParse(str, out float x);
            if (ok)
                return x;
            else
                throw new Exception("invalid value:\"" + str + "\" in section:[" + section + "] key:" + key + " in file " + Filename);
        }

        public float GetFloat(string section, string key, float defaultValue)
        {
            string str = GetString(section, key, "");
            bool ok = float.TryParse(str, out float x);
            if (ok)
                return x;
            else
                return defaultValue;
        }

        //=================================================================
        // Double
        //=================================================================
        public double GetDouble(string section, string key, CultureInfo culture = null)
        {
            string str = GetString(section, key);
            double x;
            bool ok;
            if (culture == null)
            {
                ok = double.TryParse(str, out x);
            }
            else
            {
                ok = double.TryParse(str, NumberStyles.Any, culture, out x);
            }
            if (ok)
                return x;
            else
                throw new Exception("invalid value:\"" + str + "\" in section:[" + section + "] key:" + key + " in file " + Filename);
        }

        public double GetDouble(string section, string key, double defaultValue)
        {
            string str = GetString(section, key, "");
            bool ok = double.TryParse(str, out double x);
            if (ok)
                return x;
            else
                return defaultValue;
        }

        //=================================================================
        // Enum
        //=================================================================
        public T GetEnum<T>(string section, string key) where T : struct
        {
            string str = GetString(section, key, "");
            bool ok = Enum.TryParse(str, out T t);
            if (ok)
                return t;
            else
                throw new Exception("invalid value:\"" + str + "\" in section:[" + section + "] key:" + key + " in file " + Filename);
        }

        //=================================================================
        // Bool
        //=================================================================
        public bool GetBool(string section, string key)
        {
            string str = GetString(section, key);
            bool ok = bool.TryParse(str, out bool b);
            if (ok)
                return b;
            else
                throw new Exception("invalid value:\"" + str + "\" in section:[" + section + "] key:" + key + " in file " + Filename);
        }

        public bool GetBool(string section, string key, bool defaultValue)
        {
            string str = GetString(section, key, "");
            bool ok = bool.TryParse(str, out bool b);
            if (ok)
                return b;
            else
                return defaultValue;
        }
    }
}