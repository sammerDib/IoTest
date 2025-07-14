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
using System.Drawing;
using System.Globalization;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using Common.SocketMessage;
using System.Data; // for DIRECTORIES
using System.Reflection;

namespace Common
{
    public class Win32Tools
    {
        const string CXTOOLCONTROL_INI = "C:\\CIMConnectProjects\\Equipment1\\Projects\\CxToolControl.ini";
        public class TestPresenceObject
        {
            public String m_HostName;
            public int m_Port;
            public AutoResetEvent m_EvtConnected;

            public TestPresenceObject (String pHostName, int pPort, AutoResetEvent pEvtConnected)
            {
                m_HostName = pHostName;
                m_Port = pPort;
                m_EvtConnected = pEvtConnected;
            }
        }


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



        public static void WriteProfileString(string lpFileName, string lpAppName, string lpKeyName, string lpString)
        {
            WritePrivateProfileString(lpAppName, lpKeyName, lpString, lpFileName);
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

        public static void DirToList(string pDirectory, string pFilter, ListBox pListBox)
        {
            string[] lfiles;
            string Name;
            pListBox.Items.Clear();
            try
            {
                lfiles = Directory.GetFileSystemEntries(pDirectory, pFilter);

                int lfilecount = lfiles.GetUpperBound(0) + 1;
                for (int i = 0; i < lfilecount; i++)
                {
                    Name = lfiles[i];
                    Name = Path.GetFileNameWithoutExtension(Name);
                    pListBox.Items.Add(Name );
                }
            }
            catch 
            {
                pListBox.Items.Add("No files");
            }
        }
        public static void DirToList(string pDirectory, string pFilter, ListView pListView)
        {
            string[] lfiles;
            string Name;
            pListView.Items.Clear();
            try
            {
                lfiles = Directory.GetFileSystemEntries(pDirectory, pFilter);

                int lfilecount = lfiles.GetUpperBound(0) + 1;
                for (int i = 0; i < lfilecount; i++)
                {
                    Name = lfiles[i];
                    Name = Path.GetFileNameWithoutExtension(Name);
                    pListView.Items.Add(Name);
                    if (pDirectory.Contains(CValues.RECIPE_PRODUCTION_DB_FOLDER_PATH))
                        pListView.Items[pListView.Items.Count - 1].ImageIndex = 1;
                    else
                        pListView.Items[pListView.Items.Count - 1].ImageIndex = 0;
                }
            }
            catch 
            {
                pListView.Items.Add("No files");
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
            return Wafer;        
        }

        public static String RegistryGetValueSZ(String pKeyWithPathComplate, String pKeyName)
        {
            RegistryKey vpRegistryKey = null;
            //String strValue = @"SOFTWARE\WOW6432Node\Cimetrix\ADC DB";
            try
            {
                vpRegistryKey = Registry.LocalMachine.CreateSubKey(pKeyWithPathComplate);
            }
            catch (Exception Ex)
            {
                throw new Exception(Ex.Message);
            }
            String ValueSZ = "";
            try
            {
                ValueSZ = (String)vpRegistryKey.GetValue(pKeyName);
            }
            catch (Exception Ex)
            {
                throw new Exception("Error in fct RegistryGetValueSZ() : Error = " + Ex.Message);
            }
            return ValueSZ;
        }
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
                if (Line.Length>0 && Line[0] == '#') continue;
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
      // Efface tout 19/03/2008
       public static void RemoveLoadPortPersistance(int Port)
        {
            const string LP1Key = "SOFTWARE\\Cimetrix\\WaferPersistance\\LoadPort_1";
            const string LP2Key = "SOFTWARE\\Cimetrix\\WaferPersistance\\LoadPort_2";

            try
            {

                RegistryKey Station = null;
                if (Port == 1) 
                    Station = Registry.LocalMachine.CreateSubKey(LP1Key);
                else
                    Station = Registry.LocalMachine.CreateSubKey(LP2Key);
                if (Station.SubKeyCount > 0)
                {
                    for (int i = 0; i < 26; i++)
                        Station.DeleteSubKey(i.ToString(), false);    
                }
                Station.Close();
            }
            catch (Exception e3)
            {
                MessageBox.Show(e3.ToString());
            }
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
        private static String GetNextFileName(String FilePathName, int pRotationNbrFiles)
        {
            // Separation FileName et Path
            String[] sTab = FilePathName.Trim ().Split('\\');
            String FileName=sTab[sTab.Length-1];
            String Path =FilePathName.Trim().Replace(FileName, "");                              
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
        public static void MyLog(string strLogMessage, string FileName, int pSizeInMo, int pRotationNbr)
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
                    if (InfoFile.Length >= (pSizeInMo * 1000000))
                    {
                        String NextFileName = GetNextFileName(FileName, pRotationNbr);
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

        public static void ChangeEnableColorButton(Button pButton)
        {
            if (pButton.Enabled)
                pButton.BackColor = Color.White;
            else
                pButton.BackColor = Color.DarkSlateGray;
        }

        public static bool IsComputerPresent(String HostName, int Port)
        {
            AutoResetEvent lEvt_Connected = new AutoResetEvent(false);
            TestPresenceObject lTestPresenceData = new TestPresenceObject(HostName, Port, lEvt_Connected);
            Thread lThreadTestPresence = new Thread(new ParameterizedThreadStart(TestPresence));
            lThreadTestPresence.Name = "Thread_TestPresence";
            lThreadTestPresence.Start((Object)lTestPresenceData);
            DateTime StartTime=DateTime.Now;
            bool  bStopTimeOut = false;
            while (!lTestPresenceData.m_EvtConnected.WaitOne(100, true))
            {
                Application.DoEvents();
                bStopTimeOut = (((TimeSpan)DateTime.Now.Subtract(StartTime)).TotalSeconds > 2);
                if (bStopTimeOut)
                   break;
            }
            if(bStopTimeOut) 
                return false;
            else 
                return true;
        }

        public static void TestPresence(Object pTestPresenceObject)
        {
            TestPresenceObject lTestPresenceObject = pTestPresenceObject as TestPresenceObject;
            TcpClient TCPClient1 = new TcpClient();
            try
            {
                String IPAddress = Win32Tools.GetAdresseIP(lTestPresenceObject.m_HostName);
                if(IPAddress!= String.Empty)
                    TCPClient1.Connect(IPAddress, lTestPresenceObject.m_Port);
            }
            catch
            {                
            }
            if (TCPClient1.Connected)
            {
                TCPClient1.GetStream().Close();
                TCPClient1.Close();                
                lTestPresenceObject.m_EvtConnected.Set();
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

        public static String SerializeCBaseMessage(CBaseMessage obj)
        {

            XmlSerializer serializer = new XmlSerializer(typeof(CBaseMessage));
            MemoryStream ms = new MemoryStream();
            serializer.Serialize(ms, obj);


            //bf.Serialize(ms, obj);
            byte[] buff = ms.ToArray();
            return ASCIIEncoding.ASCII.GetString(buff);
        }


        public static T DeSerializeCBaseMessage<T>(String Response)
        {
            byte[] buff = ASCIIEncoding.ASCII.GetBytes(Response);
            T result;
            XmlSerializer serializer = new XmlSerializer(typeof(CBaseMessage));
            MemoryStream ms = new MemoryStream(buff);
            result = (T)serializer.Deserialize(ms);
            return result;
        }

        public static String SerializeCBaseMessageBrightField(CBaseMessageBrightField obj)
        {

            XmlSerializer serializer = new XmlSerializer(typeof(CBaseMessageBrightField));
            MemoryStream ms = new MemoryStream();
            serializer.Serialize(ms, obj);


            //bf.Serialize(ms, obj);
            byte[] buff = ms.ToArray();
            return ASCIIEncoding.ASCII.GetString(buff);
        }


        public static T DeSerializeCBaseMessageBrightField<T>(String Response)
        {
            byte[] buff = ASCIIEncoding.ASCII.GetBytes(Response);
            T result;
            XmlSerializer serializer = new XmlSerializer(typeof(CBaseMessageBrightField));
            MemoryStream ms = new MemoryStream(buff);
            result = (T)serializer.Deserialize(ms);
            return result;
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

        public static String GetRecipePathNameInManagerRecipeFolder(String pRecipeName, String pExt)
        {
            String pRecipeNameWithoutExt;
            if(pRecipeName.ToUpper().Contains("." + pExt.ToUpper()))
                pRecipeNameWithoutExt = Path.GetFileNameWithoutExtension(pRecipeName);
            else
                pRecipeNameWithoutExt = Path.GetFileName(pRecipeName);
            if (pRecipeNameWithoutExt.IndexOf("\\") != -1)
                return "";
            String[] lFilesList = Directory.GetFiles(CValues.RECIPE_PATH, "*.rcp", SearchOption.AllDirectories);
            for (int i = 0; i < lFilesList.Length; i++)
            {
                String lRecipeName = Path.GetFileNameWithoutExtension(lFilesList[i]);
                if (lRecipeName.Contains(pRecipeNameWithoutExt))
                    return lFilesList[i];
            }            
            return "";
        }
        public static bool IsWaferSizeAvailableForOCRReading(int pWaferSize)
        {
            switch (pWaferSize)
            {
                case 75: return ((1 & CConfiguration.GetOCR_WaferSizeAvailable) == 1);
                case 100: return ((2 & CConfiguration.GetOCR_WaferSizeAvailable) == 2);
                case 150: return ((4 & CConfiguration.GetOCR_WaferSizeAvailable) == 4);
                case 200: return ((8 & CConfiguration.GetOCR_WaferSizeAvailable) == 8);
                case 300: return ((16 & CConfiguration.GetOCR_WaferSizeAvailable) == 16);
                default: return false;
            }
        }

        public static int TryGetIntValueInRecipe(String pFileName, String pParentSection, String pChildSection, String pKey, int pDefaultValue)
        {
            int iValue;
            String sValue;
            String sSection = pParentSection;
            sValue = Win32Tools.GetIniFileString(pFileName, sSection, pKey, "NotFound");
            if (sValue == "NotFound")
            {
                sSection = pParentSection + " CHILD1 " + pChildSection;
                sValue = Win32Tools.GetIniFileString(pFileName, sSection, pKey, "NotFound");
            }
            if (sValue == "NotFound")
            {
                LogObject.ReciepErrorLog("Parameter [" + sSection + "]." + pKey + " in " + pFileName + " not found. DefaultValue returned");
                iValue = pDefaultValue;
            }
            else
                try
                {
                    iValue = Convert.ToInt32(sValue);
                }
                catch
                {
                    LogObject.ReciepErrorLog("Parameter [" + sSection + "]." + pKey + " in " + pFileName + " is invalid");
                    return pDefaultValue;
                }
            return iValue; // Check if conversion will be success                        
        }

        public static String TryGetValueInRecipe(String pFileName, String pParentSection, String pChildSection, String pKey, String pDefaultValue)
        {
            String sSection = pParentSection;
            String sValue = Win32Tools.GetIniFileString(pFileName, pParentSection, pKey, "NotFound");
            if (sValue == "NotFound")
            {
                sSection = pParentSection + " CHILD1 " + pChildSection;
                sValue = Win32Tools.GetIniFileString(pFileName, sSection, pKey, "NotFound");
            }
            if (sValue == "NotFound")
            {
                sValue = pDefaultValue;
                LogObject.ReciepErrorLog("Parameter [" + sSection + "]." + pKey + " in " + pFileName + " not found. DefaultValue returned");
            }
            return sValue;
        }

        public static bool TryWriteValueInRecipe(String pFileName, String pParentSection, String ChildSection, String pKey, String pValue)
        {
            String sValue = Win32Tools.GetIniFileString(pFileName, pParentSection, pKey, "NotFound");
            if (sValue != "NotFound")
                Win32Tools.WriteProfileString(pFileName, pParentSection, pKey, pValue);
            else
            {
                sValue = Win32Tools.GetIniFileString(pFileName, pParentSection + " CHILD1 " + ChildSection, pKey, "NotFound");
                if (sValue != "NotFound")
                    Win32Tools.WriteProfileString(pFileName, pParentSection + " CHILD1 " + ChildSection, pKey, pValue);
            }
            return (sValue != "NotFound");
        }
        public static DataSet ArrayObjectToDataSet(object[] arrCollection)
        {
            DataSet ds = new DataSet();
            try
            {
                XmlSerializer serializer = new XmlSerializer(arrCollection.GetType());
                System.IO.StringWriter sw = new System.IO.StringWriter();
                serializer.Serialize(sw, arrCollection);
                System.IO.StringReader reader = new System.IO.StringReader(sw.ToString());
                ds.ReadXml(reader);
            }
            catch (Exception)
            {
                throw (new Exception("Error While Converting Array of Object to Dataset."));
            }
            return ds;
        }


		public static enumLoadWaferType ConvertLoadWaferType(short pLoadWaferType)
		{
			enumLoadWaferType lResult = enumLoadWaferType.lwStandard;

			if ((pLoadWaferType & 1) == 1)
				lResult = enumLoadWaferType.lwFilmFrame;
			else
				lResult = enumLoadWaferType.lwStandard;
			return lResult;
        }

        // Prevent control flickering on design change and form lag when rezise
        public static void SetDoubleBuffer(Control Ctrl, bool bDoubleBuffer)
        {
            try
            {
                typeof(Control).InvokeMember("DoubleBuffered",
                                            BindingFlags.NonPublic |
                                            BindingFlags.Instance |
                                            BindingFlags.SetProperty,
                                            null, Ctrl, new object[] { bDoubleBuffer });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ex.StackTrace);
            }
        }
    } // class

	public static class RegistryUtilities
	{
		/// <summary>
		/// Renames a subkey of the passed in registry key since 
		/// the Framework totally forgot to include such a handy feature.
		/// </summary>
		/// <param name="regKey">The RegistryKey that contains the subkey 
		/// you want to rename (must be writeable)</param>
		/// <param name="subKeyName">The name of the subkey that you want to rename
		/// </param>
		/// <param name="newSubKeyName">The new name of the RegistryKey</param>
		/// <returns>True if succeeds</returns>
		public static bool RenameSubKey(RegistryKey parentKey,
			string subKeyName, string newSubKeyName)
		{
			CopyKey(parentKey, subKeyName, newSubKeyName);
			parentKey.DeleteSubKeyTree(subKeyName);
			return true;
		}

		/// <summary>
		/// Copy a registry key.  The parentKey must be writeable.
		/// </summary>
		/// <param name="parentKey"></param>
		/// <param name="keyNameToCopy"></param>
		/// <param name="newKeyName"></param>
		/// <returns></returns>
		public static bool CopyKey(RegistryKey parentKey,
			string keyNameToCopy, string newKeyName)
		{
			//Create new key
			RegistryKey destinationKey = parentKey.CreateSubKey(newKeyName);

			//Open the sourceKey we are copying from
			RegistryKey sourceKey = parentKey.OpenSubKey(keyNameToCopy);

			RecurseCopyKey(sourceKey, destinationKey);

			return true;
		}

		public static void RecurseCopyKey(RegistryKey sourceKey, RegistryKey destinationKey)
		{
			//copy all the values
			foreach (string valueName in sourceKey.GetValueNames())
			{
				object objValue = sourceKey.GetValue(valueName);
				RegistryValueKind valKind = sourceKey.GetValueKind(valueName);
				destinationKey.SetValue(valueName, objValue, valKind);
			}

			//For Each subKey 
			//Create a new subKey in destinationKey 
			//Call myself 
			foreach (string sourceSubKeyName in sourceKey.GetSubKeyNames())
			{
				RegistryKey sourceSubKey = sourceKey.OpenSubKey(sourceSubKeyName);
				RegistryKey destSubKey = destinationKey.CreateSubKey(sourceSubKeyName);
				RecurseCopyKey(sourceSubKey, destSubKey);
			}
		}



	}
} // Namespace
