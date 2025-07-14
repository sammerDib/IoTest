// 13/09/2007: Registry
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;
using System.Security.Permissions; // for registry keys
using Microsoft.Win32;// for registry keys
using System.IO;
using System.Globalization;
using System.Runtime.Serialization.Formatters.Binary; // for DIRECTORIES
using System.Xml.Serialization; // for DIRECTORIES

namespace UnitySC.PM.PSD.Tools.NanoTopo.GlobalNanotopography
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

        static public string SaveToDirectoryBox(string SavePath)
        {
            FolderBrowserDialog fBrowser = new FolderBrowserDialog();
            fBrowser.Reset();
            fBrowser.Description = "Choisissez un répertoire";
            fBrowser.RootFolder = System.Environment.SpecialFolder.Desktop;
            fBrowser.ShowDialog();
            string Selected;

            Selected = fBrowser.SelectedPath;
            fBrowser.Dispose();
            return Selected;
        }





        // return zero si OK
        public static int SendSignalToProcess(string ProcName1, string ProcName2, int Option)
        {

//            return 0;   // TEST TEST 


            System.Diagnostics.Process[] ps = System.Diagnostics.Process.GetProcesses();
            foreach (System.Diagnostics.Process p in ps)
            {
                string TestName = p.ProcessName;
                if (TestName.Contains(ProcName1) || TestName.Contains(ProcName2))
                {
                    System.IntPtr pHandle = p.MainWindowHandle;
                    switch (Option)
                    {
                        case 0: Win32Tools.SendMessage((IntPtr)pHandle, 0x0112, 0xF060, 0); break;  // Close
                        case 1: Win32Tools.SendMessage((IntPtr)pHandle, 0x0112, 0xF020, 0); break; // Minimize
                        case 2: Win32Tools.SendMessage((IntPtr)pHandle, 0x0112, 0xF030, 0); break; // Maximize 
                        case 3: Win32Tools.SendMessage((IntPtr)pHandle, 0x0112, 0xF120, 0); break; // Restore 
                    }
                    return 0;
                }
            }
            return 1;
        }

        public static void ReadFileToBox(string FileName, TextBox MyText)
        {
            try
            {
                FileStream fs = File.OpenRead(FileName);
                StreamReader r = new StreamReader(fs, Encoding.ASCII);
                MyText.Text = r.ReadToEnd();
                r.Close();
                fs.Close();
            }
            catch
            {
                MyText.Text = "Cannot read file" + FileName;
            }
        }

        public static int SaveFileFromBox(string FileName, TextBox MyText)
        {
            try
            {
                FileStream fs = File.OpenWrite(FileName);
                StreamWriter w = new StreamWriter(fs, Encoding.ASCII);
                w.Write(MyText.Text);
                w.Close();
                fs.Close();
                return 0;
            }
            catch
            {
                MyText.Text = "Cannot write file" + FileName;
                return 1;
            }
        }

        public static void DirToList(string directory, string filter, ListBox MyBox)
        {
            string[] files;
            string Name;
            MyBox.Items.Clear();
            try
            {
                files = Directory.GetFileSystemEntries(directory, filter);

                int filecount = files.GetUpperBound(0) + 1;
                for (int i = 0; i < filecount; i++)
                {
                    Name = files[i];
                    Name = Path.GetFileNameWithoutExtension(Name);
                    MyBox.Items.Add(Name );
                }
            }
            catch
            {
                MyBox.Items.Add("No files");
            }
        }

        public static void Launcher(string FileName)
        {
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            try
            {
                Process.Start(FileName);
            }
            catch 
            {
                MessageBox.Show("Cannot start " + proc.StartInfo.FileName);
            }
        }

        public static void Killer(string ProcessName)
        {
            Process[] processes;
            processes = System.Diagnostics.Process.GetProcessesByName(ProcessName);
            if (processes.Length > 0)
            {
                int procID = processes[0].Id;
                Process tempProc = System.Diagnostics.Process.GetProcessById(procID);
                tempProc.CloseMainWindow();
                //      tempProc.WaitForExit();
            }
        }

        public static string GetPersistantWafer(string StationKey)
        {
            string Wafer = "";
            if (StationKey != null)
            {
                try
                {
                    RegistryKey Station = Registry.LocalMachine.CreateSubKey(StationKey);
                    if (Station.ValueCount > 0)
                    {
                        int Nb = (int)Station.GetValue("Slot Count");
                        if (Nb != 0)
                        {
                            RegistryKey Persist = Station.CreateSubKey("1");
                            if (Persist != null)
                            {
                                Wafer = (string)Persist.GetValue("Substrate ID");
                                Persist.Close();
                            }
                        }
                    }
                    Station.Close();
                }
                catch (Exception e3)
                {
                    MessageBox.Show(e3.ToString());
                }
            }
            return Wafer;        }

  
        public void DeleteDirectory(string strPath, string filter)
        {
            string[] files;
            string Name = filter;
            try
            {
                files = Directory.GetFileSystemEntries(strPath, filter);

                int filecount = files.GetUpperBound(0) + 1;
                for (int i = 0; i < filecount; i++)
                {
                    Name = files[i];
                    System.IO.File.Delete(Name);
//                    Name = Path.GetFileNameWithoutExtension(Name);
                }
            }
            catch 
            {
                MessageBox.Show("Cannot delete file " + Name + " from directory " + strPath);
            }
        
        
        }

        #region REGION_INIFILE
        // renvoie true si le fichier existe, effectue une sauvegarde préalable
        static public bool ToolFileExist(string FileName)
        {
            if (System.IO.File.Exists(FileName) == false)
            {
                MessageBox.Show("File " + FileName + " does not exist");
                return false;
            }
            return true;
        }

        private static string m_IniFileName = "";
        private static string m_IniSectionName = "";
        private static string m_LastKey = "";
        private static int m_LastLine = 0;


        public static int ConfigIniFile(string FileName)
        {
            if (!ToolFileExist(FileName)) return 1;
            m_IniFileName = FileName;
            return 0;
        }

        public static int ConfigIniSection(string SectionName)
        {
            m_IniSectionName = SectionName.ToUpper();
            m_LastKey = "";
            m_LastLine = 0;
            return 0;
        }

        // chaine vide si PB
        public static string ParamIniFile(string MySection, string MyKey)
        {
            StreamReader Reader = new StreamReader(m_IniFileName);
            string Line;
            string section = "";
            string[] split;
            string key = "";
            string val = "";

            MySection = MySection.ToUpper();
            MyKey = MyKey.ToUpper();
            while (true)
            {
                Line = Reader.ReadLine();
                if (Line == null) break;

                split = Line.Split(new Char[] { '[', ']' });    // Section
                if (split.Length > 1)
                {
                    section = split[1].ToUpper();
                }
                else
                {
                    split = Line.Split(new Char[] { '=' });     // Key
                    if (split.Length > 1)
                    {
                        key = split[0].ToUpper();
                        key = key.Trim();
                        if ((section == MySection) && (key == MyKey))
                        {
                            val = split[1];
                            val = val.Trim();
                            break;
                        }

                    }
                }
            }
            Reader.Close();
            return val;
        }


        // Dans le fichier et la section deja definis...récupere tout 
        public static string IniGetNext(out string KeyName)
        {
            KeyName = "";

            if (!ToolFileExist(m_IniFileName)) return "";

            StreamReader Reader = new StreamReader(m_IniFileName);
            string Line;
            string section = "";
            string[] split;
            string key = "";
            string val = "";
            int LineCount = 0;

            bool bReady = false;
            while (true)
            {
                Line = Reader.ReadLine();
                if (Line == null) break;
                LineCount++;

                split = Line.Split(new Char[] { '[', ']' });    // Section
                if (split.Length > 1)
                {
                    section = split[1].ToUpper();
                }
                else
                {
                    split = Line.Split(new Char[] { '=' });     // Key
                    if (split.Length > 1)
                    {
                        key = split[0].ToUpper();
                        key = key.Trim();
                        if (section == m_IniSectionName)
                        {
                          //  if (m_LastKey == "")    // premier passage
                            if (m_LastLine == 0)    // premier passage
                            {
                                KeyName = key;
                                m_LastLine = LineCount;
                                val = split[1];
                                if (split.Length > 2) val += "=" + split[2];  // le reste de la ligne contient AUSSI des "=" !!!
                                if (split.Length > 3) val += "=" + split[3];
                                if (split.Length > 4) val += "=" + split[4];
                                if (split.Length > 5) val += "=" + split[5];
                                val = val.Trim();
                                break;
                            }
                            else if (bReady)    // nouvelle clé
                            {
                                KeyName = key;
                                m_LastKey = key;
                                m_LastLine = LineCount;
                                val = split[1];
                                if (split.Length > 2) val += "=" + split[2];  // le reste de la ligne contient AUSSI des "=" !!!
                                if (split.Length > 3) val += "=" + split[3];
                                if (split.Length > 4) val += "=" + split[4];
                                if (split.Length > 5) val += "=" + split[5];
                                val = val.Trim();
                                break;
                            }
//                            else if (key == m_LastKey) // derniere position
                            else if (LineCount >= m_LastLine) // On a passé la derniere position
                            {
                                bReady = true;  // on prend la procahine clé
                            }
                        }
                        else bReady = false;
                    }
                }
            }
            Reader.Close();
            return val;
        }




        // version Integger, zero si Pb
        public static int IntParamIniFile(string MySection, string MyKey)
        {
            int Val = 0;
            string Param = ParamIniFile(MySection, MyKey);
            try
            {
                if (Param.Length > 0)
                    Val = Convert.ToInt32(Param);
            }
            catch
            { Val = 0; }
            return Val;
        }
        #endregion

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

        // Ecriture en fichier INI : Int
        public static void WriteIniFileString(string iniFile, string section, string key, String strValue)
        {
            WritePrivateProfileString(section, key, strValue, iniFile);
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
        private static String GetNextFileName(String FilePathName, int MaxSize)
        {
            // Separation FileName et Path
            String[] sTab = FilePathName.Trim ().Split('\\');
            String FileName=sTab[sTab.Length-1];
            String Path =FilePathName.Trim().Replace(FileName,"") ;                              
            sTab = FileName.Split('.');
            if (sTab.Length !=2) return "";

            String FileNameWithoutExt = sTab[0];
            String Ext = sTab[1];
            String[] AllFileName = new String[11];
            // Trouver le suivant qui  n'existe pas encore
            for (int i = 1; i <11; i++)
            {
                String NewName = Path + FileNameWithoutExt + "_" + i + "."  + Ext;
                if (!File.Exists(NewName))
                    return NewName;
                AllFileName[i] = NewName;
            }
            // Ils existent tous => prendre celui qui est de taille inferieure à la limite
            FileInfo InfoFile;
            for (int i = 1; i < 11; i++)
            {
                InfoFile = new FileInfo(AllFileName[i]);
                if (InfoFile.Length < MaxSize)
                    return AllFileName[i];
            }
            //Tous plein => retourne le 1er
            return Path + FileNameWithoutExt + "_1." + Ext;
        }
        public static void MyLog(string strLogMessage, string FileName)
        {
            try
            {
                FileStream fs;
                if (!File.Exists(FileName))
                {
                    fs = File.Create(FileName);
                    if (fs != null) fs.Close();
                }
                FileInfo InfoFile;
                StreamWriter sw = File.AppendText(FileName);
                if (sw != null)
                {
                    InfoFile = new FileInfo(FileName);
                    if (InfoFile.Length >= 5000000)
                    {
                        String NextFileName = GetNextFileName(FileName, 5000000);
                        sw.Close();
                        if (!File.Exists(NextFileName))
                            File.Move(FileName, NextFileName);
                        else
                             File.Replace(FileName, NextFileName, null);
                        fs = File.Create(FileName);
                        if (fs != null) fs.Close();
                        sw = File.AppendText(FileName);                        
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
        private static int m_MaxLogCount = 80000;

        public static void MyBackup(string FileName, ref int Count, ref int FileNum)
        {
            if (Count++ > m_MaxLogCount)
            {
                Count = 0;
                FileNum++;
                string NewName = FileName;
                NewName = NewName.Replace(".txt", FileNum.ToString() + ".txt");
                try
                { File.Copy(FileName, NewName, true); }
                catch
                { MessageBox.Show("Cannot copy file" + FileName + " to " + NewName); }
                try
                { File.Delete(FileName); }
                catch
                { MessageBox.Show("Cannot delete file" + FileName); }
            }
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
                return string.Format("{0:x}", decValue).ToUpper();
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

    } // class
} // Namespace
