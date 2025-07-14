using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Globalization;

namespace UnitySC.PM.PSD.Tools.NanoTopo.NanoTools.IniFile
{
    public class IniFile
    {
        public string m_sPath;

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string p_sSection, string p_sKey, string p_sValue, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string p_sSection, string p_sKey, string p_sDefalutValue, StringBuilder p_sbRetVal, int p_nSize, string p_sFilePath);

        // For Read Section Names
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(int p_nSection, string p_sKey, string p_sValue, [MarshalAs(UnmanagedType.LPArray)] byte[] p_arResult, int p_nSize, string p_sFilePath);

        // for Read Section Keys
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string p_sSection, int p_nKey, string p_sValue, [MarshalAs(UnmanagedType.LPArray)] byte[] p_arResult, int p_nSize, string p_sFilePath);

        public IniFile(string p_sINIPath)
        {
            if (!File.Exists(p_sINIPath))
                throw new FileNotFoundException(p_sINIPath + " does not exist", p_sINIPath);
   
            m_sPath = p_sINIPath;
        }

        // The Function called to obtain the SectionHeaders,
        // and returns them in an Dynamic Array.
        public string[] ReadSectionNames()
        {
            const int nCstMaxSizeBuf = 500;
           
            //    Sets the maxsize buffer to 500, if the more
            //    is required then doubles the size each time.
            for (int nMaxsize = nCstMaxSizeBuf; true; nMaxsize *= 2)
            {
                //    Obtains the information in bytes and stores
                //    them in the maxsize buffer (Bytes array)
                byte[] bytes = new byte[nMaxsize];
                int size = GetPrivateProfileString(0, "", "", bytes, nMaxsize, m_sPath);

                // Check the information obtained is not bigger
                // than the allocated maxsize buffer - 2 bytes.
                // if it is, then skip over the next section
                // so that the maxsize buffer can be doubled.
                if (size < nMaxsize - 2)
                {
                    // Converts the bytes value into an ASCII char. This is one long string.
                    string Selected = Encoding.ASCII.GetString(bytes, 0, size - (size > 0 ? 1 : 0));

                    // Splits the Long string into an array based on the "\0"
                    // or null (Newline) value and returns the value(s) in an array
                    return Selected.Split(new char[] { '\0' });
                }
            }
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
                int size = GetPrivateProfileString(p_sSection, 0, "", bytes, nMaxsize, m_sPath);

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

        public void IniWriteValue(string p_sSection, string p_sKey, string p_sValue)
        {
            WritePrivateProfileString(p_sSection, p_sKey, p_sValue, this.m_sPath);
        }

        public string IniReadValue(string p_sSection, string p_sKey, string p_sDefaultValue)
        {
            const int MAX_CHARS = 1023;
            StringBuilder temp = new StringBuilder(MAX_CHARS);
            int i = GetPrivateProfileString(p_sSection, p_sKey, (p_sDefaultValue != null ? p_sDefaultValue : string.Empty), temp, MAX_CHARS, this.m_sPath);
            return temp.ToString();
        }

        public void IniWriteValue(string p_sSection, string p_sKey, int p_nValue)
        {
            IniWriteValue(p_sSection, p_sKey, p_nValue.ToString());
        }

        public int IniReadValue(string p_sSection, string p_sKey, int? p_nDefaultValue)
        {
            string s = IniReadValue(p_sSection, p_sKey, (p_nDefaultValue != null ? p_nDefaultValue.ToString() : null));
            int i = 0;
            if (int.TryParse(s, out i))
                return i;
            else
                throw new IniFileException(string.Format("Tried to retrieve invalid value type from ini. In [{0}] Key={1} Tried to convert \"{2}\" to int.", p_sSection, p_sKey, s));

        }

        public void IniWriteValue(string p_sSection, string p_sKey, double p_dValue)
        {
            IniWriteValue(p_sSection, p_sKey, p_dValue.ToString());
        }

        public double IniReadValue(string p_sSection, string p_sKey, double? p_dDefaultValue)
        {
            string s = IniReadValue(p_sSection, p_sKey, (p_dDefaultValue != null ? p_dDefaultValue.ToString() : null));
            double d = 0.0;
            if (Double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out d))
                return d;
            else
                throw new IniFileException(string.Format("Tried to retrieve invalid value type from ini. In [{0}] Key={1} Tried to convert \"{2}\" to double.", p_sSection, p_sKey,s));

        }

        public void IniWriteValueHex(string p_sSection, string p_sKey, uint p_nValue)
        {
            string hex = p_nValue.ToString("X8");
            IniWriteValue(p_sSection, p_sKey, hex);
        }

        public uint IniReadValueHex(string p_sSection, string p_sKey, uint p_uDefaultValue)
        {
            string s = IniReadValue(p_sSection, p_sKey, p_uDefaultValue.ToString("X8"));
            uint i = 0;
            if (uint.TryParse(s, out i))
                return i;
            else
                throw new IniFileException(string.Format("Tried to retrieve invalid value type from ini. In [{0}] Key={1} Tried to convert \"{2}\" to uint.", p_sSection, p_sKey, s));

        }
    }

    public class IniFileException : Exception
    {
        public IniFileException(string Message)
            : base(Message)
        {

        }
    }
}
