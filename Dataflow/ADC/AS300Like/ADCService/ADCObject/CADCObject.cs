using System;
using System.IO;
using System.Threading;

using ADCCommon;

using COHTServiceObject;

using UnitySC.ADCAS300Like.Common;
using UnitySC.ADCAS300Like.Common.EException;

using UnitySC.ADCAS300Like.Common.CVIDObj;
using UnitySC.Shared.Logger;

namespace UnitySC.ADCAS300Like.Service
{
    
    public abstract class CADCObject
    {
        protected const string PROCESS_INI_FILE = "C:\\CIMConnectProjects\\Equipment1\\Projects\\PMAltasight.ini";
        protected const string FICHIER_DATA_ADC_RECIPE_PATH = @"C:\CIMConnectProjects\Equipment1\RecipesManagerFolder\Data\Fichier_Data.txt";
        private String _serverName;
        private short _portNum;
        public Thread ADCThread;
        private CCommandObject _commandObj;
        private AutoResetEvent _evtStartReceiving = new AutoResetEvent(false);
        private bool _bExitThread;
        private bool _bExitThreadStatus;
        private bool _bExitThreadFDC;
        private CADCCallBack _adcCallbacks;
        private ADCType _serverType;
        private OnSocketLog _evtSocketLog;
        private Thread _aDCStatusThread;
        private int _sleepTime;
        private int _delayAutoConnect_Sec;
        private bool _bAutoConnect;
        private String _affichageJobStatus;
        private CxValueObject _defectList = new CxValueObject();
        private int _tempoADCResultx10ms;
        private bool _bClearAllVIDBefore = false;
        private AutoResetEvent _evtSleepFDCThread = new AutoResetEvent(false);

        public Object SynchroADCObject;

        
        private LocalLogger _resultsLogger;

        public CADCObject(ADCType serverType, string serverName, short portNum, OnSocketLog pSocketLog, CADCCallBack adcCallbacks, object pSynchroADCObject, LocalLogger resultsLogger)
        {
            ServerName = serverName;
            ServerType = serverType;
            PortNum = portNum;
            AdcCallbacks = adcCallbacks;
            SynchroADCObject = pSynchroADCObject;
            EvtSocketLog = pSocketLog;
            _resultsLogger = resultsLogger;
        }

        protected void Finalize_Construction()
        {
            // Thread de gestion d'appel de fonction
            CommandObj = new CCommandObject();
            ADCThread = new Thread(new ParameterizedThreadStart(ADCThread_Execute));
            ADCThread.Name = ServerType.ToString().Remove(0,8) +  "_ADCThread_Execute";
            ADCThread.Start(CommandObj);      
      
            // Thread d'ecoute
            ADCStatusThread = new Thread(new ParameterizedThreadStart(ADCThread_Status));
            ADCStatusThread.Name = ServerType.ToString().Remove(0, 8) + "_ADCThread_Status";
            ADCStatusThread.Start();

            SleepTime = 2000;
            DelayAutoConnect_Sec = 30000;
            switch (ServerType )
	        {
		        case ADCType.atPSD_FRONTSIDE:
                    AffichageJobStatus = "ADC Frontside Analyze ";
                 break;
                case ADCType.atPSD_BACKSIDE:
                    AffichageJobStatus = "ADC Backside Analyze ";
                    break;
                case ADCType.atEDGE:
                    AffichageJobStatus = "ADC Edge Analyze ";
                    break;
                case ADCType.atLIGHTSPEED:
                    AffichageJobStatus = "ADC LightSpeed Analyze ";
                    break;                
                default:
                    AffichageJobStatus = "ADC Analyze ";
                 break;
	        }                           
        }
        
        public int DelayAutoConnect
        {
            get { return DelayAutoConnect_Sec; }
            set { DelayAutoConnect_Sec = value; }
        }

        // Abstract methods
        public abstract bool IsADCConnected { get; }
        public abstract bool IsADCConnectedToDatabase { get; }
        protected string ServerName { get => _serverName; set => _serverName = value; }
        protected short PortNum { get => _portNum; set => _portNum = value; }
        protected CCommandObject CommandObj { get => _commandObj; set => _commandObj = value; }
        protected AutoResetEvent EvtStartReceiving { get => _evtStartReceiving; set => _evtStartReceiving = value; }
        protected bool ExitThread { get => _bExitThread; set => _bExitThread = value; }
        protected bool ExitThreadStatus { get => _bExitThreadStatus; set => _bExitThreadStatus = value; }
        protected bool ExitThreadFDC { get => _bExitThreadFDC; set => _bExitThreadFDC = value; }
        protected CADCCallBack AdcCallbacks { get => _adcCallbacks; set => _adcCallbacks = value; }
        protected ADCType ServerType { get => _serverType; set => _serverType = value; }
        protected OnSocketLog EvtSocketLog { get => _evtSocketLog; set => _evtSocketLog = value; }
        protected Thread ADCStatusThread { get => _aDCStatusThread; set => _aDCStatusThread = value; }
        protected int SleepTime { get => _sleepTime; set => _sleepTime = value; }
        protected int DelayAutoConnect_Sec { get => _delayAutoConnect_Sec; set => _delayAutoConnect_Sec = value; }
        public bool AutoConnect { get => _bAutoConnect; set => _bAutoConnect = value; }
        public string AffichageJobStatus { get => _affichageJobStatus; set => _affichageJobStatus = value; }
        public CxValueObject DefectList { get => _defectList; set => _defectList = value; }

        public abstract void ADCThread_Status(Object oData);
        public abstract void ADCThread_Execute(Object oData);
        public abstract void ShutdownSocket();

        // --- ConnectADC
        public void ConnectADC()
        {
            Command NewCommand = new Command(FunctionCode.Fct_ConnectADC, null, null, null, null);
            lock (CommandObj.SynchronizationObject)
            {
                CommandObj.CommandList.Add(NewCommand);
            }
            // Signal l'arrivee d'une commande
            EvtStartReceiving.Set();
        }
        // Function trigger thread ADCExecute
        // --- ConnectADC
        public void DisconnectADC()
        {
            Command NewCommand = new Command(FunctionCode.Fct_DisconnectADC, null, null, null, null);
            lock (CommandObj.SynchronizationObject)
            {
                CommandObj.CommandList.Add(NewCommand);
            }
            // Signal l'arrivee d'une commande
            EvtStartReceiving.Set();
        }


        internal void Shutdown()
        {
            // Stop ADCThread_Execute
            ExitThread = true;
            EvtStartReceiving.Set();

            // Stop ADCThread_Status
            ExitThreadStatus = true;

            ShutdownSocket();
        }
    }
}
