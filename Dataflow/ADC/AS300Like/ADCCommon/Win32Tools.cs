// 13/09/2007: Registry
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Xml.Serialization;

using Microsoft.Win32;// for registry keys

namespace UnitySC.ADCAS300Like.Common
{
    public class Win32Tools
    {


        [DllImport("KERNEL32.DLL", EntryPoint = "GetPrivateProfileStringW",
                   SetLastError = true,
                   CharSet = CharSet.Unicode, ExactSpelling = true,
                   CallingConvention = CallingConvention.StdCall)]
        private static extern int GetPrivateProfileString(
          string lpAppName,
          string lpKeyName,
          string lpDefault,
          string lpReturnString,
          int nSize,
          string lpFilename);

        [DllImport("kernel32.dll")]
        private static extern bool WritePrivateProfileString(
            string lpAppName,
            string lpKeyName,
            string lpString,
            string lpFileName);
        
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd,
            int msg, int wParam, int lParam);


        public static void WriteProfileString(string lpFileName, string lpAppName, string lpKeyName, string lpString)
        {
            WritePrivateProfileString(lpAppName, lpKeyName, lpString, lpFileName);
        }




        // lecture en fichier INI : integer
        public static int GetIniFileInt(string sFileName, string sSection, string sEntry, int defaultValue)
        {
            int Val = 0;

            string Param = GetIniFileString(sFileName, sSection, sEntry, defaultValue.ToString());
            try
            {
                if (Param.Length > 0)
                    Val = Convert.ToInt32(Param);
            }
            catch
            { Val = -1; }
            return Val;
        }

        // lecture en fichier INI : string
        public static string GetIniFileString(string iniFile, string section, string key, string defaultValue)
        {
            string returnString = new string(' ', 1024);
            GetPrivateProfileString(section, key, defaultValue, returnString, 1024, iniFile);
            return returnString.Split('\0')[0];
        }

        private static String GetNextFileName(String filePathName, int pRotationNbrFiles)
        {
            // Separation FileName et Path
            String[] sTab = filePathName.Trim ().Split('\\');
            String FileName=sTab[sTab.Length-1];
            String Path =filePathName.Trim().Replace(FileName, "");                              
            sTab = FileName.Split('.');
            if (sTab.Length !=2) return "";

            String FileNameWithoutExt = sTab[0];
            String Ext = sTab[1];
            String[] AllFileName = new String[pRotationNbrFiles];
            
            // Trouver le suivant qui n'existe pas encore
            for (int i = 1; i < pRotationNbrFiles; i++)
            {
                String NewName = Path + FileNameWithoutExt + "_" + i + "." + Ext;
                if (!File.Exists(NewName))
                    return NewName;
                AllFileName[i] = NewName;
            }
            // Ils existent tous => prendre celui qui est de taille inferieure à la limite
            FileInfo InfoFile;
            DateTime lOlderFile = DateTime.Now;
            TimeSpan lMaxTempsPasse = TimeSpan.Zero;
            TimeSpan lCurrTempsPasse = TimeSpan.Zero;
            int iIndexFound = 0;
            for (int j = 1; j < pRotationNbrFiles; j++)
            {
                InfoFile = new FileInfo(AllFileName[j]);                
                lCurrTempsPasse = lOlderFile.Subtract(InfoFile.LastWriteTime);
                if (lCurrTempsPasse > lMaxTempsPasse)
                {
                    lMaxTempsPasse = lCurrTempsPasse; 
                    lOlderFile = InfoFile.LastWriteTime;
                    iIndexFound = j;
                }
            }
            //Tous plein => retourne le 1er
            return AllFileName[iIndexFound];
        }
        public static void MyLog(string strLogMessage, string fileName, int pSizeInMo, int pRotationNbr)
        {
            try
            {
                FileStream fs;
                if (!File.Exists(fileName))
                {
                    fs = File.Create(fileName);
                    if (fs != null) fs.Close();
                }
                FileInfo InfoFile;
                StreamWriter sw = File.AppendText(fileName);
                if (sw != null)
                {
                    InfoFile = new FileInfo(fileName);
                    if (InfoFile.Length >= (pSizeInMo * 1000000))
                    {
                        String NextFileName = GetNextFileName(fileName, pRotationNbr);
                        sw.Close();
                        if (!File.Exists(NextFileName))
                            File.Move(fileName, NextFileName);
                        else
                             File.Replace(fileName, NextFileName, null);
                        fs = File.Create(fileName);
                        if (fs != null) fs.Close();
                        sw = File.AppendText(fileName);                        
                    }
                    CultureInfo MyCulture = new CultureInfo("fr-FR");
                    DateTime lNow = System.DateTime.Now;
                    string strLogText = lNow.ToShortDateString() + " " + lNow.ToString("T", MyCulture) + "-" + lNow.Millisecond + " : " + strLogMessage;
                    strLogText += "\r\n";
                    sw.Write(strLogText);
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception exception)
            {
                string strExcMsg = exception.Message;
            }
        }


        //-------------------------------------------------------------------------------------------------------------------------------------------------------       
        // Get IP address according to the server name
        public static String GetAdresseIP(String strServerName)
        {
            try
            {
                Regex MyExp = new Regex("[0-2]?[0-9]?[0-9][.][0-2]?[0-9]?[0-9][.][0-2]?[0-9]?[0-9][.][0-2]?[0-9]?[0-9]");
                if (MyExp.IsMatch(strServerName)) 
                    return strServerName;
                else
                    return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
        // Hexadecimal to Decimal
        public static int HexToDec(String hexValue)
        {
            return Int32.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
        }

        public static int HexToDec(Char hexValue)
        {
            String strHex = Convert.ToString(hexValue);
            return HexToDec(strHex);
        }

        // Decimal to Hexadecimal
        public static string DecToHex(int decValue)
        {
            return string.Format("{0:x}", decValue);
        }
        public static string DecToHex(int decValue, String pFormat)
        {
            return string.Format(pFormat, decValue);
        }
        public static char DecToHexInAChar(int decValue)
        {
            if ((decValue < 0) || (decValue > 15))
                MessageBox.Show("fct DecToHex() : Bad input value");
            String lResult = String.Format("{0:x}", decValue);
            return lResult[0];
        }

        public static String ByteToHex(byte[] pBuf, int pSize)
        {
            String StResult = "";
            String stTmp;
            int iTmp = 0;

            stTmp = "";
            int i = 0;
            while (i < pSize)
            {
                iTmp = (int)(pBuf[i++]);
                stTmp = iTmp.ToString("x2").ToUpper();

                StResult += stTmp;
            }

            return StResult;
        }
        public static void CreateDirectoryAndFileIfDoesNotExist(String FilePathName, bool bCreateFile)
        {
            String strPath = FilePathName;
            if (strPath.Contains("."))
            {
                int Pos = strPath.LastIndexOf('\\');
                strPath = strPath.Remove(Pos, strPath.Length - Pos);
            }
            if (!Directory.Exists(strPath))
                Directory.CreateDirectory(strPath);

            if (bCreateFile && !File.Exists(FilePathName))
            {
                StreamWriter sw = null;
                try
                {
                    sw = new StreamWriter(FilePathName);
                }
                finally
                {
                    if (sw != null)
                        sw.Close();
                }
            }
        }

        public static String GetNameOfTheMostRecentFileAndRemoveWithLimitFileNbr(String pDirectoryName, int pImageBackupNbr)
        {
            if (!Directory.Exists(pDirectoryName))
                return "Invalid Directory";
            DateTime LastDateTime = DateTime.Parse("1/1/2000");
            DateTime lOlderFileDateTime = DateTime.Now;
            String strLastFileName = "Invalid Directory";
            String strOlderFile = "Invalid File";
            int iNbFileKept = pImageBackupNbr;
            int iCptNbFile=0;
            foreach (String strFileName in Directory.GetFiles(pDirectoryName, "*.Tif"))
            {
                DateTime lCurrentDate =  File.GetLastWriteTime(strFileName);
                if ((LastDateTime == null) || (LastDateTime.CompareTo(lCurrentDate) < 0))
                {
                    LastDateTime = lCurrentDate;
                    int pos = strFileName.LastIndexOf("\\");
                    strLastFileName = strFileName.Remove(0, pos+1) ;
                }
                if (lOlderFileDateTime.CompareTo(lCurrentDate) > 0)
                {
                    lOlderFileDateTime = lCurrentDate;
                    strOlderFile = strFileName;
                }
                iCptNbFile++;                
            }
            if (iCptNbFile > iNbFileKept)
            {
                if (File.Exists(strOlderFile))
                    File.Delete(strOlderFile);
            }
            return strLastFileName;
        }

        public static String Serialize<T>(T obj)
        {

            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            MemoryStream ms = new MemoryStream();
            serializer.Serialize(ms, obj);


            //bf.Serialize(ms, obj);
            byte[] buff = ms.ToArray();
            return ASCIIEncoding.ASCII.GetString(buff);
        }

        public static T DeSerialize<T>(String Response)
        {
            byte[] buff = ASCIIEncoding.ASCII.GetBytes(Response);
            T result;
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(buff);
            result = (T)serializer.Deserialize(ms);
            return result;
        }

        public static void DeleteEmptyFile(String pFileName)
        {
            if (File.Exists(pFileName))
            {
                FileInfo finfo = new FileInfo(pFileName);
                if (finfo.Length == 0)
                    File.Delete(pFileName);
            }
        }

        public static int LogParametersRotationNbrFiles(String pLogFileSettings, String pLogName)
        {
            String lParam = Win32Tools.GetIniFileString(pLogFileSettings, "CUSTOMIZED LOG", pLogName, "NotFound");
            if (lParam == "NotFound")
            {
                Win32Tools.WriteProfileString(pLogFileSettings, "CUSTOMIZED LOG", pLogName, "1;10;20");
                lParam = "1;10;20";
            }
            String[] sTab = lParam.Split(new String[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            if (sTab.Length > 2)
            {
                try
                {
                    return Convert.ToInt32(sTab[2]);
                }
                catch
                {
                }
            }
            return 20;
        }

        public static int LogParametersSizeInMo(String pLogFileSettings, String pLogName)
        {
            String lParam = Win32Tools.GetIniFileString(pLogFileSettings, "CUSTOMIZED LOG", pLogName, "NotFound");
            if (lParam == "NotFound")
            {
                Win32Tools.WriteProfileString(pLogFileSettings, "CUSTOMIZED LOG", pLogName, "1;10;20");
                lParam = "1;10;20";
            }
            String[] sTab = lParam.Split(new String[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            if (sTab.Length > 1)
            {
                try
                {
                    return Convert.ToInt32(sTab[1]);
                }
                catch
                {
                }
            }
            return 10;
        }


        public static int LogParametersEnable(String pLogFileSettings, String pLogName)
        {
            String lParam = Win32Tools.GetIniFileString(pLogFileSettings, "CUSTOMIZED LOG", pLogName, "NotFound");
            if (lParam == "NotFound")
            {
                Win32Tools.WriteProfileString(pLogFileSettings, "CUSTOMIZED LOG", pLogName, "1;10;20");
                lParam = "1;10;20";
            }
            String[] sTab = lParam.Split(new String[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            if (sTab.Length > 0)
            {
                try
                {
                    return Convert.ToInt32(sTab[0]);
                }
                catch
                {
                }
            }
            return 1;
        }

    } // class
} // Namespace
