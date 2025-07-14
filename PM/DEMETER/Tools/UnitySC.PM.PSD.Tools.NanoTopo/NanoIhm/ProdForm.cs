using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;

using UnitySC.PM.PSD.Tools.NanoTopo.NanoTools.Common;
using UnitySC.PM.PSD.Tools.NanoTopo.NanoTools.Logger;
using UnitySC.PM.PSD.Tools.NanoTopo.NanoTools.IniFile;

using LogWorker;
using UnitySC.PM.PSD.Tools.NanoTopo.GlobalNanotopography;

namespace UnitySC.PM.PSD.Tools.NanoTopo.NanoIhm
{
    public delegate void EventHandlerParam1(Object Param1);
    public delegate void Delg_EmergencyStop();
    public delegate void Delg_Enabled(bool p_bEnabled);

    public partial class ProdForm : Form
    {
        struct MesureData
        {
            public string sWaferId;
            public string sInPath;
            public string sOutPath;
            public int nPixelPeriod;
            public string sFoupId;
        };

        private Logger          m_oLog;
        private LogObsWorker    m_oLogObsWorker;
        private LogPanel        m_oLogPanel;
        private NanoCore        m_oNanoCore;
        public  IniFile m_oIniFile;
        string  m_sInputPath;
        string  m_sOutputPath;
        int     m_nPrecalibrateFile;
        int     m_nRecordDbgData;
        bool m_bStopProdThread;
        List<MesureData> m_MeasureTodoList;
        FileSystemWatcher       m_WatcherDirectory;
        EventHandlerParam1      m_EvtOnAddMesure;
        Dictionary<long, byte>  m_Dico;
        public EventHandlerParam1 EvtOnAddMesure
        {
            set { m_EvtOnAddMesure = value; }
        }

        private System.Threading.Mutex mut = new System.Threading.Mutex();

        private Delg_EmergencyStop m_delgStop;
        private Delg_Enabled m_delgEnabled;
        private Delg_Enabled m_delgSaveData;

        private Thread m_oProdThread;
        
        // Server Connection
        private CSocketServer m_SocketServerSerialization;
        OnSocketLog m_EvtSocketLog;
        OnClientConnectedDisconnected m_EvtOnClientDisconnected;
        OnClientConnectedDisconnected m_EvtOnClientConnected;
        OnServerConnectedDisconnected m_EvtOnServerDisconnected;
        OnDataExchange m_EvtOnMessageReceived;
        OnDataExchange m_EvtOnMessageReceived2;
        EventHandlerParam0 m_EvtOnStartClicked;
        EventHandlerParam0 m_EvtOnStopClicked;
        int m_PortNumber = 13900;
        bool m_bOnShutdown = false;

        // Data Exchange
        //CADCMessage.CWaferStatusByLoadport[] m_WaferStatusByLoadport;

        public ProdForm()
        {
            InitializeComponent();
        }
        
        public ProdForm(ref Logger p_oLogger, ref LogObsWorker p_oLogObsWorker, ref NanoCore p_oNanoCore)
            :this()
        {

            SplashScreen.ShowSplashScreen();

            m_oLogPanel = new LogPanel();
            p_oLogObsWorker.AddObserver(m_oLogPanel);

            m_oLog          = p_oLogger;
            m_oLogObsWorker = p_oLogObsWorker;
            m_oNanoCore     = p_oNanoCore;

            ProdLogPanel.Controls.Add(m_oLogPanel);
            m_oLogPanel.Dock = DockStyle.Fill;

            m_delgStop = new Delg_EmergencyStop(EmergencyStop_Delegate);
            m_delgEnabled = new Delg_Enabled(Enabled_Delegate);
            m_delgSaveData = new Delg_Enabled(EnabledSaveData_Delegate);

            m_MeasureTodoList = new List<MesureData>();
            EvtOnAddMesure = new EventHandlerParam1(OnAddMesure);
            
            m_oNanoCore.UpdateTreatPrmFromIni();
            m_oNanoCore.InitNanoCoreDLL();

            // Read Data from ini file -- TO DO read those data from Socket
            m_oIniFile = m_oNanoCore.m_oIniFile;

            m_sInputPath = m_oIniFile.IniReadValue("ProdMode", "InputsPath", "C:\\Altasight\\Nano\\Data");
            m_sOutputPath = m_oIniFile.IniReadValue("ProdMode", "ResultsPath", "C:\\Altasight\\Nano\\Data\\Res");
            m_nPrecalibrateFile = 0;
            m_nRecordDbgData = m_oIniFile.IniReadValue("ProdMode", "RecDbgData", 0);

            int noffsetX = m_oIniFile.IniReadValue("Main_Parameters", "ExpdOffsetX", 0);
            int noffsetY = m_oIniFile.IniReadValue("Main_Parameters", "ExpdOffsetY", 0);
            m_oNanoCore.SetExpandOffsetsDLL(noffsetX, noffsetY);
            int nNUIENable = m_oIniFile.IniReadValue("Main_Parameters", "UseNUI", 1);
            m_oNanoCore.SetNUIDLL(nNUIENable);
           
            textBoxInputPath.Text = m_sInputPath;
            textBoxOutputPath.Text = m_sOutputPath;
            
            bool IsExists = System.IO.Directory.Exists(m_sInputPath);
            try
            {
                if (!IsExists)
                    System.IO.Directory.CreateDirectory(m_sInputPath);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error : Could not create directory to scan !\nCheck the input path (" + m_sInputPath + ")");
            }

            m_Dico = new Dictionary<long, byte>();
            m_WatcherDirectory = new FileSystemWatcher(m_sInputPath, "Pic_*_*.bin");
            m_WatcherDirectory.Changed += new FileSystemEventHandler(OnChanged);
            m_WatcherDirectory.Renamed += new RenamedEventHandler(OnRenamed);
            m_WatcherDirectory.NotifyFilter = NotifyFilters.Size | NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            m_WatcherDirectory.IncludeSubdirectories = false;
            // On hold - wait for start
            m_WatcherDirectory.EnableRaisingEvents = false;
            
            m_bStopProdThread = false;
                
            m_PortNumber = m_oIniFile.IniReadValue("ProdMode", "ConnectionPortNumber", 13900);
            m_EvtOnStartClicked = new EventHandlerParam0 (DoOnStartClicked);
            m_EvtOnStopClicked = new EventHandlerParam0 (DoOnStopClicked);
            frmConnectionStatus1.SetEventClick(m_EvtOnStartClicked, m_EvtOnStopClicked);
            
            m_EvtSocketLog = new OnSocketLog(OnDoSocketLog);
            m_EvtOnClientDisconnected = new OnClientConnectedDisconnected(DoOnClientDisconnected);
            m_EvtOnClientConnected = new OnClientConnectedDisconnected(DoOnClientConnected);
            m_EvtOnServerDisconnected = new OnServerConnectedDisconnected(DoOnServerDisconnected);
            m_EvtOnMessageReceived = new OnDataExchange(DoOnMessageReceived);
            m_EvtOnMessageReceived2 = new OnDataExchange(DoOnMessageReceived2);
        //    m_SocketServerSerialization = new CSocketServer(Connection.CONNECT_NANOTOPO, "ServerNanotopography", m_PortNumber, m_EvtSocketLog, m_EvtOnClientConnected, m_EvtOnClientDisconnected, m_EvtOnServerDisconnected, m_EvtOnMessageReceived);
        //    frmConnectionStatus1[enumSelectedLights.sConnectionState] = enumConnectionLights.clNC;
            frmConnectionStatus1.Visible = false;

            SplashScreen.CloseForm();
        }

        // Define the event handlers.
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            m_oLogObsWorker.WriteEntry(1, 1, e.ChangeType + " ==> " + e.FullPath);

            TreatIncomingFile(e.Name, e.FullPath);
            
        }
        private void OnRenamed(object source, RenamedEventArgs e)
        {
            m_oLogObsWorker.WriteEntry(1, 1, e.ChangeType + " ==> " + e.FullPath);

            TreatIncomingFile(e.Name, e.FullPath);
        }

        protected void TreatIncomingFile(string sFileName, string sFullFilePath)
        {
            var parts = sFileName.Split('_');
            if (parts[0] == "Pic")
            {
                long nWaferId = Convert.ToInt64(parts[1]);
                if (!m_Dico.ContainsKey(nWaferId))
                {
                    m_Dico.Add(nWaferId, 0);
                }
                byte myBit = 0;
                byte curBit = m_Dico[nWaferId];
                if (parts[2] == "wX.bin")
                {
                    if ((curBit & 1) == 0)
                    {
                        m_oLogObsWorker.WriteEntry(1, 1, "New PhaseX data : " + sFullFilePath);
                        myBit |= 1;
                    }
                }
                else if (parts[2] == "wY.bin")
                {
                    if ((curBit & 2) == 0)
                    {
                        m_oLogObsWorker.WriteEntry(1, 1, "New PhaseY data : " + sFullFilePath);
                        myBit |= 2;
                    }
                }
                else if (parts[2] == "msk.bin")
                {
                    if ((curBit & 4) == 0)
                    {
                        m_oLogObsWorker.WriteEntry(1, 1, "New Mask data : " + sFullFilePath);
                        myBit |= 4;
                    }
                }

                if (myBit != 0)
                {
                    m_Dico[nWaferId] |= myBit;
                    if (m_Dico[nWaferId] == 0x7)
                    {
                        Thread.Sleep(1500);
                        m_EvtOnAddMesure.Invoke(nWaferId);
                    }
                }

            }
        }

        protected void SpyInputPath()
        {
            string[] fileEntries = Directory.GetFiles(m_sInputPath, "Pic_*_*.bin");
            foreach (string fileName in fileEntries)
            {
                TreatIncomingFile(Path.GetFileName(fileName),fileName);
            }
        }

        public void DoOnStartClicked()
        {            
            m_SocketServerSerialization = new CSocketServer(Connection.CONNECT_NANOTOPO, "ServerNanotopography", (short)m_PortNumber, m_EvtSocketLog, m_EvtOnClientConnected, m_EvtOnClientDisconnected, m_EvtOnServerDisconnected, m_EvtOnMessageReceived);
            frmConnectionStatus1[enumSelectedLights.sConnectionState] = enumConnectionLights.clNC;

            m_bStopProdThread = false;
            this.Invoke(m_delgEnabled, false);
            m_oProdThread = new Thread(new ThreadStart(ProcessProduction));
            m_oProdThread.Start();

        }
        public void DoOnStopClicked()
        {
            this.Invoke(m_delgStop);
            m_bStopProdThread = true;

            m_SocketServerSerialization.SocketShutdown();
            frmConnectionStatus1[enumSelectedLights.sConnectionState] = enumConnectionLights.clABS;
        }

        public void OnDoSocketLog(Connection ServerType, String Msg1, String Msg2)
        {
            if (m_bOnShutdown) return;
            if (this.InvokeRequired)
                Invoke(new OnSocketLog(DoSocketLog), ServerType, Msg1, Msg2);
            else
                DoSocketLog(ServerType, Msg1, Msg2);
        }
        public void DoSocketLog(Connection ServerType, String Msg1, String Msg2)
        {
            DateTime NowDT = DateTime.Now;
            String strLine = Msg1;
            if(Msg2.Length >0)
                strLine +=Msg2;
            m_oLogObsWorker.WriteEntry(1, 1, strLine);
        }

        public bool IsConnected()
        {
            bool pbIsConnected = (m_SocketServerSerialization != null) && m_SocketServerSerialization.IsConnected;
            // Display
            if (InvokeRequired)
                Invoke(new EventHandlerParam1(UpdateConnectionData), (object)pbIsConnected);
            else
                UpdateConnectionData(pbIsConnected);
            return pbIsConnected;
        }

        public void DoOnClientDisconnected(IAsyncResult Async)
        {
            if (m_SocketServerSerialization != null)
            {
                m_SocketServerSerialization.OnClientDisconnect();

                if (!m_bOnShutdown)
                {
                    if (m_SocketServerSerialization == null) return;
                    // Display
                    if (InvokeRequired)
                        Invoke(new EventHandlerParam1(UpdateConnectionData), (object)m_SocketServerSerialization.IsConnected);
                    else
                        UpdateConnectionData(m_SocketServerSerialization.IsConnected);
                }
            }
        }
        public void DoOnClientConnected(IAsyncResult Async)
        {
            if (!m_bOnShutdown && (m_SocketServerSerialization != null))
            {
                m_SocketServerSerialization.OnClientConnect(Async);

                if (!m_bOnShutdown)
                {
                    // Display
                    if (InvokeRequired)
                        Invoke(new EventHandlerParam1(UpdateConnectionData), (object)m_SocketServerSerialization.IsConnected);
                    else
                        UpdateConnectionData(m_SocketServerSerialization.IsConnected);
                }
            }
        }

        public void UpdateConnectionData(Object bConnected)
        {
            if (m_SocketServerSerialization == null)            
                frmConnectionStatus1[enumSelectedLights.sConnectionState] = enumConnectionLights.clABS;
            else 
            if((bool)bConnected)
                frmConnectionStatus1[enumSelectedLights.sConnectionState] = enumConnectionLights.clOK;
            else
                frmConnectionStatus1[enumSelectedLights.sConnectionState] = enumConnectionLights.clNC;
        }

        public void DoOnServerDisconnected(Connection ServerType)
        {
            try
            {
                if ((m_SocketServerSerialization != null) && m_SocketServerSerialization.IsConnected) // Assure la deconnexion
                    m_SocketServerSerialization.Disconnect();
            }
            catch (ENanotopographyException Ex)
            {
                
                Ex.DisplayError(Ex.ErrorCode);
            }
        }
        public void DoOnMessageReceived(String Data)
        {
            Invoke(m_EvtOnMessageReceived2, Data);
        }


        public void DoOnMessageReceived2(String Data)
        {
            CADCMessage MsgReceived = Win32Tools.DeSerialize<CADCMessage>(Data);
            CADCMessage lResponse = new CADCMessage();

            lResponse.acCommand = MsgReceived.acCommand;
            // lResponse.Status = enumStatusExchange.saOk;
            //lResponse.Error = enumError.eNoError;

            switch (MsgReceived.acCommand)
            {
                //case enumCommand.ceAbort:
                //    lResponse.Description = "Abort";
                //    break;
                case enumCommand.acGetStatus:
                    // Toujours Ok
                    lResponse.Description = "Get wafers Status";
                    //lResponse.WaferStatusByLoadport = m_WaferStatusByLoadport;
                    break;
                case enumCommand.acGetVersion:
                    lResponse.Description = "Nanotopography Module Vers 1.0";
                    break;

                //case enumCommand.ceGetWaferResult:
                //    lResponse.Description = "Get Wafer Results";
                //    break;
                //case enumCommand.ceResetWaferStatus:
                //    lResponse.Description = "Reset Wafer Results";
                //    break;
                default:
                    break;
            }
            m_SocketServerSerialization.SendMsg(lResponse);
        } 
        
        public new void Dispose()
        {
            if (m_oProdThread != null)
            {
                m_oProdThread.Abort();
            }
            m_oLogPanel.Dispose();
            base.Dispose();
        }

        private void ProdForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Invoke(m_delgStop);
            m_bStopProdThread = true;

            m_oIniFile.IniWriteValue("ProdMode", "InputsPath", textBoxInputPath.Text);
            m_oIniFile.IniWriteValue("ProdMode", "ResultsPath", textBoxOutputPath.Text);
            int nRecPrecalibrate = Convert.ToInt32(checkBox1.Checked);
            m_oIniFile.IniWriteValue("ProdMode", "RecDbgData", nRecPrecalibrate);
           
            m_oLogObsWorker.RemoveObserver(m_oLogPanel);
            m_oNanoCore.ReleaseNanoCoreDLL();
            ShutdownService();
        }
        private void Enabled_Delegate(bool p_bEnable)
        {
            button1.Enabled = p_bEnable;
            buttonStart.Enabled = p_bEnable;
            buttonStop.Enabled = ! p_bEnable;
            textBoxInputPath.Enabled = p_bEnable;
            textBoxOutputPath.Enabled = p_bEnable;

            if (p_bEnable)
                Cursor.Current = Cursors.Default;         // Back to normal 
            else
                Cursor.Current = Cursors.WaitCursor;       // Waiting / hour glass 
        }

        private void EnabledSaveData_Delegate(bool p_bEnable)
        {
            long uFlag = 0;
            if (p_bEnable)
                uFlag |= 0x4;
            m_oNanoCore.SetFilesGenerationDLL(uFlag);
        }

        private void EmergencyStop_Delegate()
        {
            m_oNanoCore.EmergencyStop();
        }

        private void OnAddMesure(Object WaferId)
        {
            MesureData oData;
            oData.sWaferId = WaferId.ToString();
            oData.sInPath = m_sInputPath;
            oData.sOutPath = m_sOutputPath;
            oData.sFoupId = "";

            oData.nPixelPeriod = 16;
            string path = oData.sInPath + "\\Pic_" + oData.sWaferId + "_dat.bin";
            try
            {
                if (File.Exists(path))
                {
                    using (StreamReader sr = new StreamReader(path))
                    {
                        while(sr.Peek() >= 0)
                        {
                            string sval = sr.ReadLine();
                            var parts = sval.Split('=');
                            if (parts[0] == "PixelPeriod")
                            {
                                oData.nPixelPeriod = Convert.ToInt32(parts[1]);
                            }
                            else if (parts[0] == "LotID")
                            {
                                oData.sFoupId = parts[1];
                            }

                        }
                        sr.Close();
                    }
                }
            }
            catch (Exception e)
            {
                //  Console.WriteLine("The process failed: {0}", e.ToString());
            }
            mut.WaitOne();
            m_MeasureTodoList.Add(oData);
            mut.ReleaseMutex();
        }

        private void ProcessProduction()
        {
            SpyInputPath();
            do 
            {
                if (m_MeasureTodoList.Count > 0)
                {
                    int na = m_MeasureTodoList.Count;
                    MesureData odata;
                    mut.WaitOne();
                    odata = m_MeasureTodoList[0];
                    m_MeasureTodoList.RemoveAt(0);
                    mut.ReleaseMutex();
                    try
                    {
                        int nRes = 0;// m_oNanoCore.LaunchMesureDLL(odata.sInPath,odata.sOutPath,odata.sWaferId,m_nPrecalibrateFile,odata.nPixelPeriod,odata.sFoupId);
                        if (nRes == 0)
                        {
                            m_oLogObsWorker.WriteEntry(1, 4, "Error in Measure of LotID " + odata.sWaferId);     
                        }
                        else
                        {
                            string targetPath = odata.sOutPath + "\\WaferDone";
                            String CurrentDate = DateTime.Now.ToString("yyyyMMdd-HHmmss");
                            System.IO.Directory.CreateDirectory(targetPath);
                            MoveFileTo(odata.sInPath + "\\Pic_" + odata.sWaferId + "_wX.bin", targetPath + "\\" + CurrentDate + "-Pic_" + odata.sWaferId + "_wX.bin");
                            MoveFileTo(odata.sInPath + "\\Pic_" + odata.sWaferId + "_wY.bin", targetPath + "\\" + CurrentDate + "-Pic_" + odata.sWaferId + "_wY.bin");
                            MoveFileTo(odata.sInPath + "\\Pic_" + odata.sWaferId + "_msk.bin", targetPath + "\\" + CurrentDate + "-Pic_" + odata.sWaferId + "_msk.bin");
                            MoveFileTo(odata.sInPath + "\\Pic_" + odata.sWaferId + "_dat.bin", targetPath + "\\" + CurrentDate + "-Pic_" + odata.sWaferId + "_dat.bin");
                            m_Dico.Remove(Convert.ToInt64(odata.sWaferId));
                        }
                    }
                    catch
                    {
                        System.Diagnostics.Trace.WriteLine("### LaunchMes : EXCEPTION - LotID" + odata.sWaferId);
                        m_oLogObsWorker.WriteEntry(1, 4, "Exception Occurred in Measure of LotID " + odata.sWaferId);   
                    }
                }
                else
                {
                    Thread.Sleep(500); // avoid cpu useless usage
                }
            } while (! m_bStopProdThread);

            this.Invoke(m_delgEnabled, true);
            m_oProdThread.Abort();
        }

        private void MoveFileTo(string path1, string path2)
        {
            try
            {
                if (File.Exists(path2))
                    File.Delete(path2);
                File.Move(path1, path2);
            }
            catch (System.Exception ex)
            {
                m_oLogObsWorker.WriteEntry(1, 4, "Exception Occurred in MoveFileTo : " + ex.ToString());   
            }
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {

            m_sInputPath = textBoxInputPath.Text;
            m_sOutputPath = textBoxOutputPath.Text;

            bool IsExists = System.IO.Directory.Exists(m_sInputPath);
            try
            {
                if (!IsExists)
                    System.IO.Directory.CreateDirectory(m_sInputPath);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error : Could not create directory to scan !\nCheck the input path (" + m_sInputPath + ")");
                return;
            }
            m_WatcherDirectory.Path = m_sInputPath;
            // start file scanning
            m_WatcherDirectory.EnableRaisingEvents = true;

            m_bStopProdThread = false;
            this.Invoke(m_delgEnabled, false);
            m_oProdThread = new Thread(new ThreadStart(ProcessProduction));
            m_oProdThread.Start();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            // on hold scanning
            m_WatcherDirectory.EnableRaisingEvents = false;

            this.Invoke(m_delgStop); 
            m_bStopProdThread = true;
        }

        internal void ShutdownService()
        {
            try
            {
                m_bOnShutdown = true;
                if (m_SocketServerSerialization != null)
                {
                    m_SocketServerSerialization.SocketShutdown();
                }
            }
            catch (ETCPException Ex)
            {
                Ex.DisplayError(Ex.ErrorCode);
            }

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.BeginInvoke(m_delgSaveData, checkBox1.Checked);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //  int nRes = m_oNanoCore.LaunchMesureDLL();
        }

        private void ProdForm_Load(object sender, EventArgs e)
        {
            checkBox1.Checked = (m_nRecordDbgData != 0);
        }

       
    }
}
