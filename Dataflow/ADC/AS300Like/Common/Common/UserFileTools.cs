using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Security.Permissions; // for registry keys
using Microsoft.Win32;// for registry keys
using System.IO; // for DIRECTORIES
using System.Runtime.InteropServices;


namespace FileTools
{
    class UserFileTools
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

        
        // --------------------------------------------------------------------------------

        // lecture en fichier INI : integer
        public static int GetIniFileInt(string sFileName, string sSection, string sEntry, int DefaultValue)
        {
            int Val = 0;

            string Param = GetIniFileString(sFileName, sSection, sEntry, DefaultValue.ToString());
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

        // Ecriture en fichier INI : Int
        public static void WriteIniFileInt(string iniFile, string section, string key, int iValue)
        {
            WritePrivateProfileString(section, key, iValue.ToString(), iniFile);            
        }

        // Boite de dialogue de fichier
        // Directory
        public static string SelectedDirectory(string sInitialDirectory, string sTitle)
        {
            string CurrentDirectory;
            FolderBrowserDialog eFileDlg = new FolderBrowserDialog();

            eFileDlg.Description = sTitle;
            eFileDlg.SelectedPath = sInitialDirectory;
            eFileDlg.ShowDialog();

            CurrentDirectory = eFileDlg.SelectedPath;
            eFileDlg.Dispose();

            return CurrentDirectory;
        }

        // Open file dialog
        public static string SelectedLoadFile(string sInitialDirectory, string sTitle, string sFilter)
        {
            string CurrentFile = "";

            OpenFileDialog eFileDlg = new OpenFileDialog();
            eFileDlg.Filter = sFilter;
            eFileDlg.InitialDirectory = sInitialDirectory;

            DialogResult eResult = eFileDlg.ShowDialog();

            if (eResult == DialogResult.OK)
                CurrentFile = eFileDlg.FileName;

            return CurrentFile;
        }

        // Save file dialog
        public static string SelectedSaveFile(string sInitialDirectory, string sTitle, string sFilter)
        {
            string CurrentFile = "";

            SaveFileDialog eFileDlg = new SaveFileDialog();
            eFileDlg.Filter = sFilter;
            eFileDlg.InitialDirectory = sInitialDirectory;

            DialogResult eResult = eFileDlg.ShowDialog();

            if (eResult == DialogResult.OK)
                CurrentFile = eFileDlg.FileName;

            return CurrentFile;
        }
    }
}
