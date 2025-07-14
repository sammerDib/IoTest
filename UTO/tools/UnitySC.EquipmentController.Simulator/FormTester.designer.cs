
namespace UnitySC.EquipmentController.Simulator
{
    partial class FormTester
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTester));
            this.ofdHard = new System.Windows.Forms.OpenFileDialog();
            this.ofdJob = new System.Windows.Forms.OpenFileDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.lLDSPAE = new System.Windows.Forms.Label();
            this.lLDSPZU = new System.Windows.Forms.Label();
            this.lLDSPAR = new System.Windows.Forms.Label();
            this.lULSPAE = new System.Windows.Forms.Label();
            this.lULSPZD = new System.Windows.Forms.Label();
            this.lULSPAR = new System.Windows.Forms.Label();
            this.lLDAEWT = new System.Windows.Forms.Label();
            this.bEfemStop = new System.Windows.Forms.Button();
            this.lTP1 = new System.Windows.Forms.Label();
            this.lTP2 = new System.Windows.Forms.Label();
            this.lTP3 = new System.Windows.Forms.Label();
            this.lTP4 = new System.Windows.Forms.Label();
            this.lTP5 = new System.Windows.Forms.Label();
            this.lCarrierType = new System.Windows.Forms.Label();
            this.tbLog = new System.Windows.Forms.TextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tcMain = new System.Windows.Forms.TabControl();
            this.tStart = new System.Windows.Forms.TabPage();
            this.tbPort = new System.Windows.Forms.TextBox();
            this.tbIPAddress = new System.Windows.Forms.TextBox();
            this.lPort = new System.Windows.Forms.Label();
            this.lIpAddress = new System.Windows.Forms.Label();
            this.bDisconnect = new System.Windows.Forms.Button();
            this.bConnect = new System.Windows.Forms.Button();
            this.bCreate = new System.Windows.Forms.Button();
            this.pgEFEM = new System.Windows.Forms.PropertyGrid();
            this.tControl = new System.Windows.Forms.TabPage();
            this.gbHighLevel = new System.Windows.Forms.GroupBox();
            this.bClearErrors = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.bHLJobStop = new System.Windows.Forms.Button();
            this.bHLJobAbortActivity = new System.Windows.Forms.Button();
            this.chColdInit = new System.Windows.Forms.CheckBox();
            this.gbHLMacro = new System.Windows.Forms.GroupBox();
            this.tbMacroFiducial = new System.Windows.Forms.ComboBox();
            this.tbMacroAlignAngle = new System.Windows.Forms.TextBox();
            this.cbHLSlotMacro = new System.Windows.Forms.ComboBox();
            this.cbHLLoadPortsMacro = new System.Windows.Forms.ComboBox();
            this.bHLLoadWaferMacro = new System.Windows.Forms.Button();
            this.bHLUnloadWaferMacro = new System.Windows.Forms.Button();
            this.bEmergencyStop = new System.Windows.Forms.Button();
            this.bHLInit = new System.Windows.Forms.Button();
            this.gbLowLevel = new System.Windows.Forms.GroupBox();
            this.bEfemGetPressure = new System.Windows.Forms.Button();
            this.tbEFEMPressure = new System.Windows.Forms.TextBox();
            this.gbFfu = new System.Windows.Forms.GroupBox();
            this.nudFfuSpeed = new System.Windows.Forms.NumericUpDown();
            this.lFfuSpeed = new System.Windows.Forms.Label();
            this.bFfuGetRpm = new System.Windows.Forms.Button();
            this.bFfuStop = new System.Windows.Forms.Button();
            this.bFfuSetRpm = new System.Windows.Forms.Button();
            this.lFfuSetPoint = new System.Windows.Forms.Label();
            this.nudFfuSetPoint = new System.Windows.Forms.NumericUpDown();
            this.gbEfem = new System.Windows.Forms.GroupBox();
            this.bEfemSetTime = new System.Windows.Forms.Button();
            this.tbEFEMTime = new System.Windows.Forms.TextBox();
            this.bEfemGetStat = new System.Windows.Forms.Button();
            this.bEfemInitialization = new System.Windows.Forms.Button();
            this.tbEFEMDeviceState = new System.Windows.Forms.TextBox();
            this.gbEfemSize = new System.Windows.Forms.GroupBox();
            this.cbEfemSizeLocation = new System.Windows.Forms.ComboBox();
            this.cbEfemSizeTarget = new System.Windows.Forms.ComboBox();
            this.bEfemGetSize = new System.Windows.Forms.Button();
            this.bEfemSetSize = new System.Windows.Forms.Button();
            this.bEfemResume = new System.Windows.Forms.Button();
            this.bEfemPause = new System.Windows.Forms.Button();
            this.gbLLProcessModule = new System.Windows.Forms.GroupBox();
            this.chPMHotTemp = new System.Windows.Forms.CheckBox();
            this.cbPMSize = new System.Windows.Forms.ComboBox();
            this.chPMPresence = new System.Windows.Forms.CheckBox();
            this.bPMWaferDeposit = new System.Windows.Forms.Button();
            this.tbPMWaferPresence = new System.Windows.Forms.TextBox();
            this.gbLLRobot = new System.Windows.Forms.GroupBox();
            this.bGetWaferPresenceOnArm = new System.Windows.Forms.Button();
            this.bUnclampWaferOnArm = new System.Windows.Forms.Button();
            this.bClampWaferOnArm = new System.Windows.Forms.Button();
            this.gbSetRobotSpeed = new System.Windows.Forms.GroupBox();
            this.tbRobotSpeed = new System.Windows.Forms.TextBox();
            this.cbSelectRobotSpeed = new System.Windows.Forms.ComboBox();
            this.bSetRobotSpeed = new System.Windows.Forms.Button();
            this.bPreparePlace = new System.Windows.Forms.Button();
            this.gbRobotArmSpeed = new System.Windows.Forms.GroupBox();
            this.nudLDAEWT = new System.Windows.Forms.NumericUpDown();
            this.cbRobotLoadLockTarget = new System.Windows.Forms.ComboBox();
            this.nudULSPAR = new System.Windows.Forms.NumericUpDown();
            this.nudULSPZD = new System.Windows.Forms.NumericUpDown();
            this.nudULSPAE = new System.Windows.Forms.NumericUpDown();
            this.nudLDSPAR = new System.Windows.Forms.NumericUpDown();
            this.nudLDSPZU = new System.Windows.Forms.NumericUpDown();
            this.nudLDSPAE = new System.Windows.Forms.NumericUpDown();
            this.bRobotGetSettings = new System.Windows.Forms.Button();
            this.bRobotSetSettings = new System.Windows.Forms.Button();
            this.chRobotCmdGranted = new System.Windows.Forms.CheckBox();
            this.tbRobotDeviceState = new System.Windows.Forms.TextBox();
            this.cbRobotDestinationSlot = new System.Windows.Forms.ComboBox();
            this.cbRobotDestinationLocation = new System.Windows.Forms.ComboBox();
            this.bRobotTransfer = new System.Windows.Forms.Button();
            this.bRobotPlace = new System.Windows.Forms.Button();
            this.cbRobotArm = new System.Windows.Forms.ComboBox();
            this.cbRobotSourceSlot = new System.Windows.Forms.ComboBox();
            this.cbRobotSourceLocation = new System.Windows.Forms.ComboBox();
            this.bRobotPick = new System.Windows.Forms.Button();
            this.bPreparePick = new System.Windows.Forms.Button();
            this.bRobotHome = new System.Windows.Forms.Button();
            this.bRobotInit = new System.Windows.Forms.Button();
            this.tbRobotLowerArmWaferPresence = new System.Windows.Forms.TextBox();
            this.tbRobotUpperArmWaferPresence = new System.Windows.Forms.TextBox();
            this.gbLLLoadPorts = new System.Windows.Forms.GroupBox();
            this.bLPGetCarrierType = new System.Windows.Forms.Button();
            this.tbLPCarrierType = new System.Windows.Forms.TextBox();
            this.bLPUnclamp = new System.Windows.Forms.Button();
            this.bLPClamp = new System.Windows.Forms.Button();
            this.tbLPWaferSize = new System.Windows.Forms.TextBox();
            this.bLPGetSize = new System.Windows.Forms.Button();
            this.chIsPurgeEnabled = new System.Windows.Forms.CheckBox();
            this.gbE84 = new System.Windows.Forms.GroupBox();
            this.bAbortE84 = new System.Windows.Forms.Button();
            this.bResetE84 = new System.Windows.Forms.Button();
            this.bEnableE84 = new System.Windows.Forms.Button();
            this.bDisableE84 = new System.Windows.Forms.Button();
            this.bGetE84Inputs = new System.Windows.Forms.Button();
            this.bE84Stop = new System.Windows.Forms.Button();
            this.bE84Unload = new System.Windows.Forms.Button();
            this.bGetE84Outputs = new System.Windows.Forms.Button();
            this.bLPMoveToWritePosition = new System.Windows.Forms.Button();
            this.bLPMoveToReadPosition = new System.Windows.Forms.Button();
            this.tbLPCarrierID = new System.Windows.Forms.TextBox();
            this.gbCarrierID = new System.Windows.Forms.GroupBox();
            this.bCarrierIDEnter = new System.Windows.Forms.Button();
            this.tbCarrierID = new System.Windows.Forms.TextBox();
            this.bLPRelease = new System.Windows.Forms.Button();
            this.bLPInAccess = new System.Windows.Forms.Button();
            this.tbLPMapping = new System.Windows.Forms.TextBox();
            this.tbLPDeviceState = new System.Windows.Forms.TextBox();
            this.bLPMap = new System.Windows.Forms.Button();
            this.bLPLastMapping = new System.Windows.Forms.Button();
            this.tbLPInAccess = new System.Windows.Forms.TextBox();
            this.tbLPPlacement = new System.Windows.Forms.TextBox();
            this.tbLPPresence = new System.Windows.Forms.TextBox();
            this.cbLLLoadPorts = new System.Windows.Forms.ComboBox();
            this.bLPDock = new System.Windows.Forms.Button();
            this.bLPUndock = new System.Windows.Forms.Button();
            this.bLPInit = new System.Windows.Forms.Button();
            this.bLPOpen = new System.Windows.Forms.Button();
            this.bLPClose = new System.Windows.Forms.Button();
            this.bLPHome = new System.Windows.Forms.Button();
            this.bLPReadCarrierID = new System.Windows.Forms.Button();
            this.gbLLAligner = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.gbOcr = new System.Windows.Forms.GroupBox();
            this.cbOcrBackSideRecipes = new System.Windows.Forms.ComboBox();
            this.tbOCRWaferIDBackSide = new System.Windows.Forms.TextBox();
            this.tbOCRWaferIDFrontSide = new System.Windows.Forms.TextBox();
            this.bOcrRead = new System.Windows.Forms.Button();
            this.cbOcrWaferType = new System.Windows.Forms.ComboBox();
            this.cbOcrWaferSide = new System.Windows.Forms.ComboBox();
            this.cbOcrFrontSideRecipes = new System.Windows.Forms.ComboBox();
            this.bOcrGetRecipes = new System.Windows.Forms.Button();
            this.bUnclampOnAligner = new System.Windows.Forms.Button();
            this.bClampOnAligner = new System.Windows.Forms.Button();
            this.tbAlignerDeviceState = new System.Windows.Forms.TextBox();
            this.tbAlignerWaferPresence = new System.Windows.Forms.TextBox();
            this.bAlignerInit = new System.Windows.Forms.Button();
            this.bAlignerAlign = new System.Windows.Forms.Button();
            this.tbAlignerAlignAngle = new System.Windows.Forms.TextBox();
            this.bClear = new System.Windows.Forms.Button();
            this.tLoadPorts = new System.Windows.Forms.TabPage();
            this.bSetCarrierType = new System.Windows.Forms.Button();
            this.nudCarrierType = new System.Windows.Forms.NumericUpDown();
            this.bLPE84Abort = new System.Windows.Forms.Button();
            this.bLPE84Reset = new System.Windows.Forms.Button();
            this.bSetE84Timeout = new System.Windows.Forms.Button();
            this.nudTP5 = new System.Windows.Forms.NumericUpDown();
            this.nudTP4 = new System.Windows.Forms.NumericUpDown();
            this.nudTP3 = new System.Windows.Forms.NumericUpDown();
            this.nudTP2 = new System.Windows.Forms.NumericUpDown();
            this.nudTP1 = new System.Windows.Forms.NumericUpDown();
            this.bLPE84AutoHandling = new System.Windows.Forms.Button();
            this.bLPE84ManualHandling = new System.Windows.Forms.Button();
            this.bLPAuto = new System.Windows.Forms.Button();
            this.bLPManual = new System.Windows.Forms.Button();
            this.tbLP3AccessMode = new System.Windows.Forms.TextBox();
            this.tbLP3ServiceState = new System.Windows.Forms.TextBox();
            this.tbLP2AccessMode = new System.Windows.Forms.TextBox();
            this.tbLP2ServiceState = new System.Windows.Forms.TextBox();
            this.tbLP1AccessMode = new System.Windows.Forms.TextBox();
            this.tbLP1ServiceState = new System.Windows.Forms.TextBox();
            this.bLPInService = new System.Windows.Forms.Button();
            this.bLPNotInService = new System.Windows.Forms.Button();
            this.cbLoadPorts = new System.Windows.Forms.ComboBox();
            this.tIO = new System.Windows.Forms.TabPage();
            this.gbSignals = new System.Windows.Forms.GroupBox();
            this.pgSignals = new System.Windows.Forms.PropertyGrid();
            this.gbLPIndicators = new System.Windows.Forms.GroupBox();
            this.tbAlarmIndicator = new System.Windows.Forms.TextBox();
            this.bAlarmLightBlink = new System.Windows.Forms.Button();
            this.bAlarmLightOff = new System.Windows.Forms.Button();
            this.bAlarmLightOn = new System.Windows.Forms.Button();
            this.tbReserveIndicator = new System.Windows.Forms.TextBox();
            this.bReserveLightBlink = new System.Windows.Forms.Button();
            this.bReserveLightOff = new System.Windows.Forms.Button();
            this.bReserveLightOn = new System.Windows.Forms.Button();
            this.tbAutoIndicator = new System.Windows.Forms.TextBox();
            this.bAutoLightBlink = new System.Windows.Forms.Button();
            this.bAutoLightOff = new System.Windows.Forms.Button();
            this.bAutoLightOn = new System.Windows.Forms.Button();
            this.cbIOLoadPorts = new System.Windows.Forms.ComboBox();
            this.tbAccessIndicator = new System.Windows.Forms.TextBox();
            this.bOpAccessLightBlink = new System.Windows.Forms.Button();
            this.bOpAccessLightOff = new System.Windows.Forms.Button();
            this.bOpAccessLightOn = new System.Windows.Forms.Button();
            this.tbManualIndicator = new System.Windows.Forms.TextBox();
            this.bManualLightBlink = new System.Windows.Forms.Button();
            this.bManualLightOff = new System.Windows.Forms.Button();
            this.bManualLightOn = new System.Windows.Forms.Button();
            this.tbUnloadIndicator = new System.Windows.Forms.TextBox();
            this.bUnloadLightBlink = new System.Windows.Forms.Button();
            this.bUnloadLightOff = new System.Windows.Forms.Button();
            this.bUnloadLightOn = new System.Windows.Forms.Button();
            this.tbLoadIndicator = new System.Windows.Forms.TextBox();
            this.bLoadLightBlink = new System.Windows.Forms.Button();
            this.bLoadLightOff = new System.Windows.Forms.Button();
            this.bLoadLightOn = new System.Windows.Forms.Button();
            this.gbTower = new System.Windows.Forms.GroupBox();
            this.tbRedLightStatus = new System.Windows.Forms.TextBox();
            this.bRedLightBlink = new System.Windows.Forms.Button();
            this.bRedLightOff = new System.Windows.Forms.Button();
            this.bRedLightOn = new System.Windows.Forms.Button();
            this.tbOrangeLightStatus = new System.Windows.Forms.TextBox();
            this.bOrangeLightBlink = new System.Windows.Forms.Button();
            this.bOrangeLightOff = new System.Windows.Forms.Button();
            this.bOrangeLightOn = new System.Windows.Forms.Button();
            this.tbBlueLightStatus = new System.Windows.Forms.TextBox();
            this.bBlueLightBlink = new System.Windows.Forms.Button();
            this.bBlueLightOff = new System.Windows.Forms.Button();
            this.bBlueLightOn = new System.Windows.Forms.Button();
            this.tbGreenLightStatus = new System.Windows.Forms.TextBox();
            this.bGreenLightBlink = new System.Windows.Forms.Button();
            this.bGreenLightOff = new System.Windows.Forms.Button();
            this.bGreenLightOn = new System.Windows.Forms.Button();
            this.gbBuzzer = new System.Windows.Forms.GroupBox();
            this.tbBuzzerStatus = new System.Windows.Forms.TextBox();
            this.bBuzzerOff = new System.Windows.Forms.Button();
            this.bBuzzerOn = new System.Windows.Forms.Button();
            this.tComLogs = new System.Windows.Forms.TabPage();
            this.bClearComLogs = new System.Windows.Forms.Button();
            this.tbComLogs = new System.Windows.Forms.TextBox();
            this.tPostmanCom = new System.Windows.Forms.TabPage();
            this.bClearPostman = new System.Windows.Forms.Button();
            this.tbPostman = new System.Windows.Forms.TextBox();
            this.tSendMessage = new System.Windows.Forms.TabPage();
            this.rtbSendMsgDesc = new System.Windows.Forms.RichTextBox();
            this.tbMessageToSend3 = new System.Windows.Forms.TextBox();
            this.tbMessageToSend2 = new System.Windows.Forms.TextBox();
            this.tbMessageToSend = new System.Windows.Forms.TextBox();
            this.bSend3 = new System.Windows.Forms.Button();
            this.lMessage3 = new System.Windows.Forms.Label();
            this.bSend2 = new System.Windows.Forms.Button();
            this.lMessage2 = new System.Windows.Forms.Label();
            this.bSend = new System.Windows.Forms.Button();
            this.lMessage = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tcMain.SuspendLayout();
            this.tStart.SuspendLayout();
            this.tControl.SuspendLayout();
            this.gbHighLevel.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.gbHLMacro.SuspendLayout();
            this.gbLowLevel.SuspendLayout();
            this.gbFfu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFfuSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFfuSetPoint)).BeginInit();
            this.gbEfem.SuspendLayout();
            this.gbEfemSize.SuspendLayout();
            this.gbLLProcessModule.SuspendLayout();
            this.gbLLRobot.SuspendLayout();
            this.gbSetRobotSpeed.SuspendLayout();
            this.gbRobotArmSpeed.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLDAEWT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudULSPAR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudULSPZD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudULSPAE)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLDSPAR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLDSPZU)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLDSPAE)).BeginInit();
            this.gbLLLoadPorts.SuspendLayout();
            this.gbE84.SuspendLayout();
            this.gbCarrierID.SuspendLayout();
            this.gbLLAligner.SuspendLayout();
            this.gbOcr.SuspendLayout();
            this.tLoadPorts.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCarrierType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTP5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTP4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTP3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTP2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTP1)).BeginInit();
            this.tIO.SuspendLayout();
            this.gbSignals.SuspendLayout();
            this.gbLPIndicators.SuspendLayout();
            this.gbTower.SuspendLayout();
            this.gbBuzzer.SuspendLayout();
            this.tComLogs.SuspendLayout();
            this.tPostmanCom.SuspendLayout();
            this.tSendMessage.SuspendLayout();
            this.SuspendLayout();
            // 
            // ofdJob
            // 
            this.ofdJob.FileName = "job.xml";
            // 
            // lLDSPAE
            // 
            this.lLDSPAE.AutoSize = true;
            this.lLDSPAE.Location = new System.Drawing.Point(6, 51);
            this.lLDSPAE.Name = "lLDSPAE";
            this.lLDSPAE.Size = new System.Drawing.Size(49, 13);
            this.lLDSPAE.TabIndex = 1;
            this.lLDSPAE.Text = "LDSPAE";
            this.toolTip1.SetToolTip(this.lLDSPAE, "Load Speed Arm Extend");
            // 
            // lLDSPZU
            // 
            this.lLDSPZU.AutoSize = true;
            this.lLDSPZU.Location = new System.Drawing.Point(6, 77);
            this.lLDSPZU.Name = "lLDSPZU";
            this.lLDSPZU.Size = new System.Drawing.Size(50, 13);
            this.lLDSPZU.TabIndex = 3;
            this.lLDSPZU.Text = "LDSPZU";
            this.toolTip1.SetToolTip(this.lLDSPZU, "Load Speed Z Up");
            // 
            // lLDSPAR
            // 
            this.lLDSPAR.AutoSize = true;
            this.lLDSPAR.Location = new System.Drawing.Point(6, 103);
            this.lLDSPAR.Name = "lLDSPAR";
            this.lLDSPAR.Size = new System.Drawing.Size(50, 13);
            this.lLDSPAR.TabIndex = 5;
            this.lLDSPAR.Text = "LDSPAR";
            this.toolTip1.SetToolTip(this.lLDSPAR, "Load Speed Arm Retract");
            // 
            // lULSPAE
            // 
            this.lULSPAE.AutoSize = true;
            this.lULSPAE.Location = new System.Drawing.Point(6, 129);
            this.lULSPAE.Name = "lULSPAE";
            this.lULSPAE.Size = new System.Drawing.Size(49, 13);
            this.lULSPAE.TabIndex = 7;
            this.lULSPAE.Text = "ULSPAE";
            this.toolTip1.SetToolTip(this.lULSPAE, "Unload Speed Arm Extend");
            // 
            // lULSPZD
            // 
            this.lULSPZD.AutoSize = true;
            this.lULSPZD.Location = new System.Drawing.Point(6, 155);
            this.lULSPZD.Name = "lULSPZD";
            this.lULSPZD.Size = new System.Drawing.Size(50, 13);
            this.lULSPZD.TabIndex = 9;
            this.lULSPZD.Text = "ULSPZD";
            this.toolTip1.SetToolTip(this.lULSPZD, "Unload Speed Z Down");
            // 
            // lULSPAR
            // 
            this.lULSPAR.AutoSize = true;
            this.lULSPAR.Location = new System.Drawing.Point(6, 181);
            this.lULSPAR.Name = "lULSPAR";
            this.lULSPAR.Size = new System.Drawing.Size(50, 13);
            this.lULSPAR.TabIndex = 11;
            this.lULSPAR.Text = "ULSPAR";
            this.toolTip1.SetToolTip(this.lULSPAR, "Unload Speed  Arm Retract");
            // 
            // lLDAEWT
            // 
            this.lLDAEWT.AutoSize = true;
            this.lLDAEWT.Location = new System.Drawing.Point(6, 207);
            this.lLDAEWT.Name = "lLDAEWT";
            this.lLDAEWT.Size = new System.Drawing.Size(53, 13);
            this.lLDAEWT.TabIndex = 13;
            this.lLDAEWT.Text = "LDAEWT";
            this.toolTip1.SetToolTip(this.lLDAEWT, "Load Arm Extend Wait Time");
            // 
            // bEfemStop
            // 
            this.bEfemStop.Enabled = false;
            this.bEfemStop.Location = new System.Drawing.Point(112, 45);
            this.bEfemStop.Name = "bEfemStop";
            this.bEfemStop.Size = new System.Drawing.Size(85, 23);
            this.bEfemStop.TabIndex = 2;
            this.bEfemStop.Text = "Stop";
            this.toolTip1.SetToolTip(this.bEfemStop, "Refresh Firmware version");
            this.bEfemStop.UseVisualStyleBackColor = true;
            this.bEfemStop.Click += new System.EventHandler(this.EfemStop_Click);
            // 
            // lTP1
            // 
            this.lTP1.AutoSize = true;
            this.lTP1.Location = new System.Drawing.Point(731, 26);
            this.lTP1.Name = "lTP1";
            this.lTP1.Size = new System.Drawing.Size(27, 13);
            this.lTP1.TabIndex = 14;
            this.lTP1.Text = "TP1";
            this.toolTip1.SetToolTip(this.lTP1, "Load Speed Arm Extend");
            // 
            // lTP2
            // 
            this.lTP2.AutoSize = true;
            this.lTP2.Location = new System.Drawing.Point(731, 52);
            this.lTP2.Name = "lTP2";
            this.lTP2.Size = new System.Drawing.Size(27, 13);
            this.lTP2.TabIndex = 16;
            this.lTP2.Text = "TP2";
            this.toolTip1.SetToolTip(this.lTP2, "Load Speed Arm Extend");
            // 
            // lTP3
            // 
            this.lTP3.AutoSize = true;
            this.lTP3.Location = new System.Drawing.Point(731, 78);
            this.lTP3.Name = "lTP3";
            this.lTP3.Size = new System.Drawing.Size(27, 13);
            this.lTP3.TabIndex = 18;
            this.lTP3.Text = "TP3";
            this.toolTip1.SetToolTip(this.lTP3, "Load Speed Arm Extend");
            // 
            // lTP4
            // 
            this.lTP4.AutoSize = true;
            this.lTP4.Location = new System.Drawing.Point(731, 104);
            this.lTP4.Name = "lTP4";
            this.lTP4.Size = new System.Drawing.Size(27, 13);
            this.lTP4.TabIndex = 20;
            this.lTP4.Text = "TP4";
            this.toolTip1.SetToolTip(this.lTP4, "Load Speed Arm Extend");
            // 
            // lTP5
            // 
            this.lTP5.AutoSize = true;
            this.lTP5.Location = new System.Drawing.Point(731, 130);
            this.lTP5.Name = "lTP5";
            this.lTP5.Size = new System.Drawing.Size(27, 13);
            this.lTP5.TabIndex = 22;
            this.lTP5.Text = "TP5";
            this.toolTip1.SetToolTip(this.lTP5, "Load Speed Arm Extend");
            // 
            // lCarrierType
            // 
            this.lCarrierType.AutoSize = true;
            this.lCarrierType.Location = new System.Drawing.Point(905, 131);
            this.lCarrierType.Name = "lCarrierType";
            this.lCarrierType.Size = new System.Drawing.Size(64, 13);
            this.lCarrierType.TabIndex = 27;
            this.lCarrierType.Text = "Carrier Type";
            this.toolTip1.SetToolTip(this.lCarrierType, "Carrier type from 1 to 16");
            // 
            // tbLog
            // 
            this.tbLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbLog.Location = new System.Drawing.Point(0, 0);
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.ReadOnly = true;
            this.tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbLog.Size = new System.Drawing.Size(1216, 259);
            this.tbLog.TabIndex = 3;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tcMain);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tbLog);
            this.splitContainer1.Size = new System.Drawing.Size(1216, 884);
            this.splitContainer1.SplitterDistance = 621;
            this.splitContainer1.TabIndex = 3;
            // 
            // tcMain
            // 
            this.tcMain.Controls.Add(this.tStart);
            this.tcMain.Controls.Add(this.tControl);
            this.tcMain.Controls.Add(this.tLoadPorts);
            this.tcMain.Controls.Add(this.tIO);
            this.tcMain.Controls.Add(this.tComLogs);
            this.tcMain.Controls.Add(this.tPostmanCom);
            this.tcMain.Controls.Add(this.tSendMessage);
            this.tcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcMain.Location = new System.Drawing.Point(0, 0);
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(1216, 621);
            this.tcMain.TabIndex = 0;
            this.tcMain.TabStop = false;
            // 
            // tStart
            // 
            this.tStart.Controls.Add(this.tbPort);
            this.tStart.Controls.Add(this.tbIPAddress);
            this.tStart.Controls.Add(this.lPort);
            this.tStart.Controls.Add(this.lIpAddress);
            this.tStart.Controls.Add(this.bDisconnect);
            this.tStart.Controls.Add(this.bConnect);
            this.tStart.Controls.Add(this.bCreate);
            this.tStart.Controls.Add(this.pgEFEM);
            this.tStart.Location = new System.Drawing.Point(4, 22);
            this.tStart.Name = "tStart";
            this.tStart.Size = new System.Drawing.Size(1208, 595);
            this.tStart.TabIndex = 2;
            this.tStart.Text = "Start";
            this.tStart.UseVisualStyleBackColor = true;
            // 
            // tbPort
            // 
            this.tbPort.Location = new System.Drawing.Point(240, 19);
            this.tbPort.Name = "tbPort";
            this.tbPort.Size = new System.Drawing.Size(131, 20);
            this.tbPort.TabIndex = 15;
            // 
            // tbIPAddress
            // 
            this.tbIPAddress.Location = new System.Drawing.Point(83, 19);
            this.tbIPAddress.Name = "tbIPAddress";
            this.tbIPAddress.Size = new System.Drawing.Size(101, 20);
            this.tbIPAddress.TabIndex = 13;
            // 
            // lPort
            // 
            this.lPort.AutoSize = true;
            this.lPort.Location = new System.Drawing.Point(205, 22);
            this.lPort.Name = "lPort";
            this.lPort.Size = new System.Drawing.Size(29, 13);
            this.lPort.TabIndex = 14;
            this.lPort.Text = "Port:";
            // 
            // lIpAddress
            // 
            this.lIpAddress.AutoSize = true;
            this.lIpAddress.Location = new System.Drawing.Point(19, 22);
            this.lIpAddress.Name = "lIpAddress";
            this.lIpAddress.Size = new System.Drawing.Size(61, 13);
            this.lIpAddress.TabIndex = 12;
            this.lIpAddress.Text = "IP Address:";
            // 
            // bDisconnect
            // 
            this.bDisconnect.Location = new System.Drawing.Point(205, 107);
            this.bDisconnect.Name = "bDisconnect";
            this.bDisconnect.Size = new System.Drawing.Size(166, 23);
            this.bDisconnect.TabIndex = 11;
            this.bDisconnect.Text = "Disconnect";
            this.bDisconnect.UseVisualStyleBackColor = true;
            this.bDisconnect.Click += new System.EventHandler(this.bDisconnect_Click);
            // 
            // bConnect
            // 
            this.bConnect.Location = new System.Drawing.Point(19, 107);
            this.bConnect.Name = "bConnect";
            this.bConnect.Size = new System.Drawing.Size(166, 23);
            this.bConnect.TabIndex = 11;
            this.bConnect.Text = "Connect";
            this.bConnect.UseVisualStyleBackColor = true;
            this.bConnect.Click += new System.EventHandler(this.bConnect_Click);
            // 
            // bCreate
            // 
            this.bCreate.Location = new System.Drawing.Point(19, 69);
            this.bCreate.Name = "bCreate";
            this.bCreate.Size = new System.Drawing.Size(166, 23);
            this.bCreate.TabIndex = 11;
            this.bCreate.Text = "Create";
            this.bCreate.UseVisualStyleBackColor = true;
            this.bCreate.Click += new System.EventHandler(this.Create_Click);
            // 
            // pgEFEM
            // 
            this.pgEFEM.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pgEFEM.HelpVisible = false;
            this.pgEFEM.Location = new System.Drawing.Point(402, 3);
            this.pgEFEM.Name = "pgEFEM";
            this.pgEFEM.Size = new System.Drawing.Size(803, 592);
            this.pgEFEM.TabIndex = 10;
            // 
            // tControl
            // 
            this.tControl.Controls.Add(this.gbHighLevel);
            this.tControl.Controls.Add(this.gbLowLevel);
            this.tControl.Controls.Add(this.bClear);
            this.tControl.Location = new System.Drawing.Point(4, 22);
            this.tControl.Name = "tControl";
            this.tControl.Padding = new System.Windows.Forms.Padding(3);
            this.tControl.Size = new System.Drawing.Size(1208, 595);
            this.tControl.TabIndex = 0;
            this.tControl.Text = "Control";
            this.tControl.UseVisualStyleBackColor = true;
            // 
            // gbHighLevel
            // 
            this.gbHighLevel.Controls.Add(this.bClearErrors);
            this.gbHighLevel.Controls.Add(this.groupBox1);
            this.gbHighLevel.Controls.Add(this.chColdInit);
            this.gbHighLevel.Controls.Add(this.gbHLMacro);
            this.gbHighLevel.Controls.Add(this.bEmergencyStop);
            this.gbHighLevel.Controls.Add(this.bHLInit);
            this.gbHighLevel.Enabled = false;
            this.gbHighLevel.Location = new System.Drawing.Point(999, 6);
            this.gbHighLevel.Name = "gbHighLevel";
            this.gbHighLevel.Size = new System.Drawing.Size(205, 267);
            this.gbHighLevel.TabIndex = 1;
            this.gbHighLevel.TabStop = false;
            this.gbHighLevel.Text = "High level management";
            // 
            // bClearErrors
            // 
            this.bClearErrors.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.bClearErrors.Enabled = false;
            this.bClearErrors.Location = new System.Drawing.Point(8, 208);
            this.bClearErrors.Name = "bClearErrors";
            this.bClearErrors.Size = new System.Drawing.Size(193, 23);
            this.bClearErrors.TabIndex = 6;
            this.bClearErrors.Text = "Clear Errors";
            this.bClearErrors.UseVisualStyleBackColor = false;
            this.bClearErrors.Click += new System.EventHandler(this.ClearErrors_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.bHLJobStop);
            this.groupBox1.Controls.Add(this.bHLJobAbortActivity);
            this.groupBox1.Enabled = false;
            this.groupBox1.Location = new System.Drawing.Point(8, 158);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(195, 47);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Activity actions";
            // 
            // bHLJobStop
            // 
            this.bHLJobStop.Enabled = false;
            this.bHLJobStop.Location = new System.Drawing.Point(6, 18);
            this.bHLJobStop.Name = "bHLJobStop";
            this.bHLJobStop.Size = new System.Drawing.Size(85, 23);
            this.bHLJobStop.TabIndex = 0;
            this.bHLJobStop.Text = "Stop";
            this.bHLJobStop.UseVisualStyleBackColor = true;
            // 
            // bHLJobAbortActivity
            // 
            this.bHLJobAbortActivity.Enabled = false;
            this.bHLJobAbortActivity.Location = new System.Drawing.Point(100, 18);
            this.bHLJobAbortActivity.Name = "bHLJobAbortActivity";
            this.bHLJobAbortActivity.Size = new System.Drawing.Size(85, 23);
            this.bHLJobAbortActivity.TabIndex = 1;
            this.bHLJobAbortActivity.Text = "Abort";
            this.bHLJobAbortActivity.UseVisualStyleBackColor = true;
            // 
            // chColdInit
            // 
            this.chColdInit.AutoSize = true;
            this.chColdInit.Enabled = false;
            this.chColdInit.Location = new System.Drawing.Point(78, 23);
            this.chColdInit.Name = "chColdInit";
            this.chColdInit.Size = new System.Drawing.Size(61, 17);
            this.chColdInit.TabIndex = 1;
            this.chColdInit.Text = "ColdInit";
            this.chColdInit.UseVisualStyleBackColor = true;
            // 
            // gbHLMacro
            // 
            this.gbHLMacro.Controls.Add(this.tbMacroFiducial);
            this.gbHLMacro.Controls.Add(this.tbMacroAlignAngle);
            this.gbHLMacro.Controls.Add(this.cbHLSlotMacro);
            this.gbHLMacro.Controls.Add(this.cbHLLoadPortsMacro);
            this.gbHLMacro.Controls.Add(this.bHLLoadWaferMacro);
            this.gbHLMacro.Controls.Add(this.bHLUnloadWaferMacro);
            this.gbHLMacro.Enabled = false;
            this.gbHLMacro.Location = new System.Drawing.Point(8, 48);
            this.gbHLMacro.Name = "gbHLMacro";
            this.gbHLMacro.Size = new System.Drawing.Size(191, 104);
            this.gbHLMacro.TabIndex = 3;
            this.gbHLMacro.TabStop = false;
            this.gbHLMacro.Text = "Macro";
            // 
            // tbMacroFiducial
            // 
            this.tbMacroFiducial.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tbMacroFiducial.Enabled = false;
            this.tbMacroFiducial.FormattingEnabled = true;
            this.tbMacroFiducial.Location = new System.Drawing.Point(100, 74);
            this.tbMacroFiducial.Name = "tbMacroFiducial";
            this.tbMacroFiducial.Size = new System.Drawing.Size(85, 21);
            this.tbMacroFiducial.TabIndex = 5;
            // 
            // tbMacroAlignAngle
            // 
            this.tbMacroAlignAngle.Enabled = false;
            this.tbMacroAlignAngle.Location = new System.Drawing.Point(100, 46);
            this.tbMacroAlignAngle.Name = "tbMacroAlignAngle";
            this.tbMacroAlignAngle.Size = new System.Drawing.Size(85, 20);
            this.tbMacroAlignAngle.TabIndex = 3;
            this.tbMacroAlignAngle.Text = "Angle (°)";
            this.tbMacroAlignAngle.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // cbHLSlotMacro
            // 
            this.cbHLSlotMacro.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbHLSlotMacro.Enabled = false;
            this.cbHLSlotMacro.FormattingEnabled = true;
            this.cbHLSlotMacro.Location = new System.Drawing.Point(100, 19);
            this.cbHLSlotMacro.Name = "cbHLSlotMacro";
            this.cbHLSlotMacro.Size = new System.Drawing.Size(85, 21);
            this.cbHLSlotMacro.TabIndex = 1;
            // 
            // cbHLLoadPortsMacro
            // 
            this.cbHLLoadPortsMacro.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbHLLoadPortsMacro.Enabled = false;
            this.cbHLLoadPortsMacro.FormattingEnabled = true;
            this.cbHLLoadPortsMacro.Location = new System.Drawing.Point(6, 19);
            this.cbHLLoadPortsMacro.Name = "cbHLLoadPortsMacro";
            this.cbHLLoadPortsMacro.Size = new System.Drawing.Size(85, 21);
            this.cbHLLoadPortsMacro.TabIndex = 0;
            // 
            // bHLLoadWaferMacro
            // 
            this.bHLLoadWaferMacro.Enabled = false;
            this.bHLLoadWaferMacro.Location = new System.Drawing.Point(6, 45);
            this.bHLLoadWaferMacro.Name = "bHLLoadWaferMacro";
            this.bHLLoadWaferMacro.Size = new System.Drawing.Size(85, 23);
            this.bHLLoadWaferMacro.TabIndex = 2;
            this.bHLLoadWaferMacro.Text = "Load Wafer";
            this.bHLLoadWaferMacro.UseVisualStyleBackColor = true;
            this.bHLLoadWaferMacro.Click += new System.EventHandler(this.HLLoadWaferMacro_Click);
            // 
            // bHLUnloadWaferMacro
            // 
            this.bHLUnloadWaferMacro.Enabled = false;
            this.bHLUnloadWaferMacro.Location = new System.Drawing.Point(6, 73);
            this.bHLUnloadWaferMacro.Name = "bHLUnloadWaferMacro";
            this.bHLUnloadWaferMacro.Size = new System.Drawing.Size(85, 23);
            this.bHLUnloadWaferMacro.TabIndex = 4;
            this.bHLUnloadWaferMacro.Text = "Unload Wafer";
            this.bHLUnloadWaferMacro.UseVisualStyleBackColor = true;
            this.bHLUnloadWaferMacro.Click += new System.EventHandler(this.HLUnloadWaferMacro_Click);
            // 
            // bEmergencyStop
            // 
            this.bEmergencyStop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.bEmergencyStop.Enabled = false;
            this.bEmergencyStop.Location = new System.Drawing.Point(8, 237);
            this.bEmergencyStop.Name = "bEmergencyStop";
            this.bEmergencyStop.Size = new System.Drawing.Size(193, 23);
            this.bEmergencyStop.TabIndex = 7;
            this.bEmergencyStop.Text = "Emergency Stop";
            this.bEmergencyStop.UseVisualStyleBackColor = false;
            this.bEmergencyStop.Click += new System.EventHandler(this.EmergencyStop_Click);
            // 
            // bHLInit
            // 
            this.bHLInit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.bHLInit.Enabled = false;
            this.bHLInit.Location = new System.Drawing.Point(6, 19);
            this.bHLInit.Name = "bHLInit";
            this.bHLInit.Size = new System.Drawing.Size(70, 23);
            this.bHLInit.TabIndex = 0;
            this.bHLInit.Text = "Initialization";
            this.bHLInit.UseVisualStyleBackColor = false;
            this.bHLInit.Click += new System.EventHandler(this.HLInit_Click);
            // 
            // gbLowLevel
            // 
            this.gbLowLevel.Controls.Add(this.bEfemGetPressure);
            this.gbLowLevel.Controls.Add(this.tbEFEMPressure);
            this.gbLowLevel.Controls.Add(this.gbFfu);
            this.gbLowLevel.Controls.Add(this.gbEfem);
            this.gbLowLevel.Controls.Add(this.gbLLProcessModule);
            this.gbLowLevel.Controls.Add(this.gbLLRobot);
            this.gbLowLevel.Controls.Add(this.gbLLLoadPorts);
            this.gbLowLevel.Controls.Add(this.gbLLAligner);
            this.gbLowLevel.Location = new System.Drawing.Point(6, 6);
            this.gbLowLevel.Name = "gbLowLevel";
            this.gbLowLevel.Size = new System.Drawing.Size(988, 588);
            this.gbLowLevel.TabIndex = 0;
            this.gbLowLevel.TabStop = false;
            this.gbLowLevel.Text = "Low level management";
            // 
            // bEfemGetPressure
            // 
            this.bEfemGetPressure.Location = new System.Drawing.Point(887, 329);
            this.bEfemGetPressure.Name = "bEfemGetPressure";
            this.bEfemGetPressure.Size = new System.Drawing.Size(85, 23);
            this.bEfemGetPressure.TabIndex = 7;
            this.bEfemGetPressure.Text = "Get Pressure";
            this.bEfemGetPressure.UseVisualStyleBackColor = true;
            this.bEfemGetPressure.Click += new System.EventHandler(this.bEfemGetPressure_Click);
            // 
            // tbEFEMPressure
            // 
            this.tbEFEMPressure.BackColor = System.Drawing.SystemColors.Control;
            this.tbEFEMPressure.Enabled = false;
            this.tbEFEMPressure.Location = new System.Drawing.Point(781, 331);
            this.tbEFEMPressure.Name = "tbEFEMPressure";
            this.tbEFEMPressure.ReadOnly = true;
            this.tbEFEMPressure.Size = new System.Drawing.Size(100, 20);
            this.tbEFEMPressure.TabIndex = 40;
            this.tbEFEMPressure.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // gbFfu
            // 
            this.gbFfu.Controls.Add(this.nudFfuSpeed);
            this.gbFfu.Controls.Add(this.lFfuSpeed);
            this.gbFfu.Controls.Add(this.bFfuGetRpm);
            this.gbFfu.Controls.Add(this.bFfuStop);
            this.gbFfu.Controls.Add(this.bFfuSetRpm);
            this.gbFfu.Controls.Add(this.lFfuSetPoint);
            this.gbFfu.Controls.Add(this.nudFfuSetPoint);
            this.gbFfu.Location = new System.Drawing.Point(775, 457);
            this.gbFfu.Name = "gbFfu";
            this.gbFfu.Size = new System.Drawing.Size(205, 113);
            this.gbFfu.TabIndex = 7;
            this.gbFfu.TabStop = false;
            this.gbFfu.Text = "Fan filter unit";
            // 
            // nudFfuSpeed
            // 
            this.nudFfuSpeed.Location = new System.Drawing.Point(115, 48);
            this.nudFfuSpeed.Name = "nudFfuSpeed";
            this.nudFfuSpeed.ReadOnly = true;
            this.nudFfuSpeed.Size = new System.Drawing.Size(82, 20);
            this.nudFfuSpeed.TabIndex = 6;
            // 
            // lFfuSpeed
            // 
            this.lFfuSpeed.AutoSize = true;
            this.lFfuSpeed.Location = new System.Drawing.Point(21, 50);
            this.lFfuSpeed.Name = "lFfuSpeed";
            this.lFfuSpeed.Size = new System.Drawing.Size(71, 13);
            this.lFfuSpeed.TabIndex = 5;
            this.lFfuSpeed.Text = "Speed (RPM)";
            // 
            // bFfuGetRpm
            // 
            this.bFfuGetRpm.Location = new System.Drawing.Point(6, 80);
            this.bFfuGetRpm.Name = "bFfuGetRpm";
            this.bFfuGetRpm.Size = new System.Drawing.Size(60, 23);
            this.bFfuGetRpm.TabIndex = 4;
            this.bFfuGetRpm.Text = "Get";
            this.bFfuGetRpm.UseVisualStyleBackColor = true;
            this.bFfuGetRpm.Click += new System.EventHandler(this.bFfuGetRpm_Click);
            // 
            // bFfuStop
            // 
            this.bFfuStop.Location = new System.Drawing.Point(137, 80);
            this.bFfuStop.Name = "bFfuStop";
            this.bFfuStop.Size = new System.Drawing.Size(60, 23);
            this.bFfuStop.TabIndex = 3;
            this.bFfuStop.Text = "Stop";
            this.bFfuStop.UseVisualStyleBackColor = true;
            this.bFfuStop.Click += new System.EventHandler(this.bFfuStop_Click);
            // 
            // bFfuSetRpm
            // 
            this.bFfuSetRpm.Location = new System.Drawing.Point(72, 80);
            this.bFfuSetRpm.Name = "bFfuSetRpm";
            this.bFfuSetRpm.Size = new System.Drawing.Size(60, 23);
            this.bFfuSetRpm.TabIndex = 2;
            this.bFfuSetRpm.Text = "Set";
            this.bFfuSetRpm.UseVisualStyleBackColor = true;
            this.bFfuSetRpm.Click += new System.EventHandler(this.bFfuSetRpm_Click);
            // 
            // lFfuSetPoint
            // 
            this.lFfuSetPoint.AutoSize = true;
            this.lFfuSetPoint.Location = new System.Drawing.Point(21, 24);
            this.lFfuSetPoint.Name = "lFfuSetPoint";
            this.lFfuSetPoint.Size = new System.Drawing.Size(79, 13);
            this.lFfuSetPoint.TabIndex = 1;
            this.lFfuSetPoint.Text = "Setpoint (RPM)";
            // 
            // nudFfuSetPoint
            // 
            this.nudFfuSetPoint.Location = new System.Drawing.Point(115, 22);
            this.nudFfuSetPoint.Maximum = new decimal(new int[] {
            1650,
            0,
            0,
            0});
            this.nudFfuSetPoint.Minimum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.nudFfuSetPoint.Name = "nudFfuSetPoint";
            this.nudFfuSetPoint.Size = new System.Drawing.Size(82, 20);
            this.nudFfuSetPoint.TabIndex = 0;
            this.nudFfuSetPoint.Value = new decimal(new int[] {
            300,
            0,
            0,
            0});
            // 
            // gbEfem
            // 
            this.gbEfem.Controls.Add(this.bEfemSetTime);
            this.gbEfem.Controls.Add(this.tbEFEMTime);
            this.gbEfem.Controls.Add(this.bEfemGetStat);
            this.gbEfem.Controls.Add(this.bEfemInitialization);
            this.gbEfem.Controls.Add(this.bEfemStop);
            this.gbEfem.Controls.Add(this.tbEFEMDeviceState);
            this.gbEfem.Controls.Add(this.gbEfemSize);
            this.gbEfem.Controls.Add(this.bEfemResume);
            this.gbEfem.Controls.Add(this.bEfemPause);
            this.gbEfem.Location = new System.Drawing.Point(775, 117);
            this.gbEfem.Name = "gbEfem";
            this.gbEfem.Size = new System.Drawing.Size(205, 337);
            this.gbEfem.TabIndex = 6;
            this.gbEfem.TabStop = false;
            this.gbEfem.Text = "Efem";
            // 
            // bEfemSetTime
            // 
            this.bEfemSetTime.Location = new System.Drawing.Point(112, 247);
            this.bEfemSetTime.Name = "bEfemSetTime";
            this.bEfemSetTime.Size = new System.Drawing.Size(85, 23);
            this.bEfemSetTime.TabIndex = 41;
            this.bEfemSetTime.Text = "Set Time";
            this.bEfemSetTime.UseVisualStyleBackColor = true;
            this.bEfemSetTime.Click += new System.EventHandler(this.bEfemSetTime_Click);
            // 
            // tbEFEMTime
            // 
            this.tbEFEMTime.BackColor = System.Drawing.SystemColors.Control;
            this.tbEFEMTime.Location = new System.Drawing.Point(6, 247);
            this.tbEFEMTime.Name = "tbEFEMTime";
            this.tbEFEMTime.Size = new System.Drawing.Size(100, 20);
            this.tbEFEMTime.TabIndex = 42;
            this.tbEFEMTime.Text = "yyyymmddhhmmss";
            this.tbEFEMTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // bEfemGetStat
            // 
            this.bEfemGetStat.Location = new System.Drawing.Point(14, 104);
            this.bEfemGetStat.Name = "bEfemGetStat";
            this.bEfemGetStat.Size = new System.Drawing.Size(85, 23);
            this.bEfemGetStat.TabIndex = 6;
            this.bEfemGetStat.Text = "Get Stat";
            this.bEfemGetStat.UseVisualStyleBackColor = true;
            this.bEfemGetStat.Click += new System.EventHandler(this.bEfemGetStat_Click);
            // 
            // bEfemInitialization
            // 
            this.bEfemInitialization.Location = new System.Drawing.Point(112, 19);
            this.bEfemInitialization.Name = "bEfemInitialization";
            this.bEfemInitialization.Size = new System.Drawing.Size(85, 23);
            this.bEfemInitialization.TabIndex = 1;
            this.bEfemInitialization.Text = "Initialization";
            this.bEfemInitialization.UseVisualStyleBackColor = true;
            this.bEfemInitialization.Click += new System.EventHandler(this.EfemInitialization_Click);
            // 
            // tbEFEMDeviceState
            // 
            this.tbEFEMDeviceState.BackColor = System.Drawing.SystemColors.Control;
            this.tbEFEMDeviceState.Enabled = false;
            this.tbEFEMDeviceState.Location = new System.Drawing.Point(6, 21);
            this.tbEFEMDeviceState.Name = "tbEFEMDeviceState";
            this.tbEFEMDeviceState.ReadOnly = true;
            this.tbEFEMDeviceState.Size = new System.Drawing.Size(100, 20);
            this.tbEFEMDeviceState.TabIndex = 0;
            this.tbEFEMDeviceState.Text = "Device State";
            this.tbEFEMDeviceState.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // gbEfemSize
            // 
            this.gbEfemSize.Controls.Add(this.cbEfemSizeLocation);
            this.gbEfemSize.Controls.Add(this.cbEfemSizeTarget);
            this.gbEfemSize.Controls.Add(this.bEfemGetSize);
            this.gbEfemSize.Controls.Add(this.bEfemSetSize);
            this.gbEfemSize.Enabled = false;
            this.gbEfemSize.Location = new System.Drawing.Point(8, 137);
            this.gbEfemSize.Name = "gbEfemSize";
            this.gbEfemSize.Size = new System.Drawing.Size(191, 74);
            this.gbEfemSize.TabIndex = 5;
            this.gbEfemSize.TabStop = false;
            this.gbEfemSize.Text = "Size";
            // 
            // cbEfemSizeLocation
            // 
            this.cbEfemSizeLocation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbEfemSizeLocation.Enabled = false;
            this.cbEfemSizeLocation.FormattingEnabled = true;
            this.cbEfemSizeLocation.Location = new System.Drawing.Point(6, 18);
            this.cbEfemSizeLocation.Name = "cbEfemSizeLocation";
            this.cbEfemSizeLocation.Size = new System.Drawing.Size(85, 21);
            this.cbEfemSizeLocation.TabIndex = 0;
            // 
            // cbEfemSizeTarget
            // 
            this.cbEfemSizeTarget.Enabled = false;
            this.cbEfemSizeTarget.FormattingEnabled = true;
            this.cbEfemSizeTarget.Items.AddRange(new object[] {
            "200",
            "300",
            "200",
            "300"});
            this.cbEfemSizeTarget.Location = new System.Drawing.Point(100, 18);
            this.cbEfemSizeTarget.Name = "cbEfemSizeTarget";
            this.cbEfemSizeTarget.Size = new System.Drawing.Size(82, 21);
            this.cbEfemSizeTarget.TabIndex = 1;
            this.cbEfemSizeTarget.Text = "Target size";
            // 
            // bEfemGetSize
            // 
            this.bEfemGetSize.Enabled = false;
            this.bEfemGetSize.Location = new System.Drawing.Point(6, 45);
            this.bEfemGetSize.Name = "bEfemGetSize";
            this.bEfemGetSize.Size = new System.Drawing.Size(85, 23);
            this.bEfemGetSize.TabIndex = 2;
            this.bEfemGetSize.Text = "Get";
            this.bEfemGetSize.UseVisualStyleBackColor = true;
            this.bEfemGetSize.Click += new System.EventHandler(this.EfemGetSize_Click);
            // 
            // bEfemSetSize
            // 
            this.bEfemSetSize.Enabled = false;
            this.bEfemSetSize.Location = new System.Drawing.Point(99, 45);
            this.bEfemSetSize.Name = "bEfemSetSize";
            this.bEfemSetSize.Size = new System.Drawing.Size(85, 23);
            this.bEfemSetSize.TabIndex = 3;
            this.bEfemSetSize.Text = "Set";
            this.bEfemSetSize.UseVisualStyleBackColor = true;
            this.bEfemSetSize.Click += new System.EventHandler(this.EfemSetSize_Click);
            // 
            // bEfemResume
            // 
            this.bEfemResume.Enabled = false;
            this.bEfemResume.Location = new System.Drawing.Point(112, 104);
            this.bEfemResume.Name = "bEfemResume";
            this.bEfemResume.Size = new System.Drawing.Size(85, 23);
            this.bEfemResume.TabIndex = 4;
            this.bEfemResume.Text = "Resume";
            this.bEfemResume.UseVisualStyleBackColor = true;
            this.bEfemResume.Click += new System.EventHandler(this.EfemResume_Click);
            // 
            // bEfemPause
            // 
            this.bEfemPause.Enabled = false;
            this.bEfemPause.Location = new System.Drawing.Point(112, 74);
            this.bEfemPause.Name = "bEfemPause";
            this.bEfemPause.Size = new System.Drawing.Size(85, 23);
            this.bEfemPause.TabIndex = 3;
            this.bEfemPause.Text = "Pause";
            this.bEfemPause.UseVisualStyleBackColor = true;
            this.bEfemPause.Click += new System.EventHandler(this.EfemPause_Click);
            // 
            // gbLLProcessModule
            // 
            this.gbLLProcessModule.Controls.Add(this.chPMHotTemp);
            this.gbLLProcessModule.Controls.Add(this.cbPMSize);
            this.gbLLProcessModule.Controls.Add(this.chPMPresence);
            this.gbLLProcessModule.Controls.Add(this.bPMWaferDeposit);
            this.gbLLProcessModule.Controls.Add(this.tbPMWaferPresence);
            this.gbLLProcessModule.Enabled = false;
            this.gbLLProcessModule.Location = new System.Drawing.Point(688, 14);
            this.gbLLProcessModule.Name = "gbLLProcessModule";
            this.gbLLProcessModule.Size = new System.Drawing.Size(292, 100);
            this.gbLLProcessModule.TabIndex = 1;
            this.gbLLProcessModule.TabStop = false;
            this.gbLLProcessModule.Text = "Process Module";
            // 
            // chPMHotTemp
            // 
            this.chPMHotTemp.AutoSize = true;
            this.chPMHotTemp.Enabled = false;
            this.chPMHotTemp.Location = new System.Drawing.Point(112, 49);
            this.chPMHotTemp.Name = "chPMHotTemp";
            this.chPMHotTemp.Size = new System.Drawing.Size(43, 17);
            this.chPMHotTemp.TabIndex = 3;
            this.chPMHotTemp.Text = "Hot";
            this.chPMHotTemp.UseVisualStyleBackColor = true;
            // 
            // cbPMSize
            // 
            this.cbPMSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPMSize.Enabled = false;
            this.cbPMSize.FormattingEnabled = true;
            this.cbPMSize.Location = new System.Drawing.Point(207, 23);
            this.cbPMSize.Name = "cbPMSize";
            this.cbPMSize.Size = new System.Drawing.Size(75, 21);
            this.cbPMSize.TabIndex = 2;
            // 
            // chPMPresence
            // 
            this.chPMPresence.AutoSize = true;
            this.chPMPresence.Enabled = false;
            this.chPMPresence.Location = new System.Drawing.Point(211, 50);
            this.chPMPresence.Name = "chPMPresence";
            this.chPMPresence.Size = new System.Drawing.Size(71, 17);
            this.chPMPresence.TabIndex = 4;
            this.chPMPresence.Text = "Presence";
            this.chPMPresence.UseVisualStyleBackColor = true;
            // 
            // bPMWaferDeposit
            // 
            this.bPMWaferDeposit.Enabled = false;
            this.bPMWaferDeposit.Location = new System.Drawing.Point(112, 22);
            this.bPMWaferDeposit.Name = "bPMWaferDeposit";
            this.bPMWaferDeposit.Size = new System.Drawing.Size(85, 23);
            this.bPMWaferDeposit.TabIndex = 1;
            this.bPMWaferDeposit.Text = "Wafer Deposit";
            this.bPMWaferDeposit.UseVisualStyleBackColor = true;
            this.bPMWaferDeposit.Click += new System.EventHandler(this.PMWaferDeposit_Click);
            // 
            // tbPMWaferPresence
            // 
            this.tbPMWaferPresence.BackColor = System.Drawing.SystemColors.Control;
            this.tbPMWaferPresence.Enabled = false;
            this.tbPMWaferPresence.Location = new System.Drawing.Point(6, 22);
            this.tbPMWaferPresence.Name = "tbPMWaferPresence";
            this.tbPMWaferPresence.ReadOnly = true;
            this.tbPMWaferPresence.Size = new System.Drawing.Size(100, 20);
            this.tbPMWaferPresence.TabIndex = 0;
            this.tbPMWaferPresence.Text = "Wafer Presence";
            this.tbPMWaferPresence.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // gbLLRobot
            // 
            this.gbLLRobot.Controls.Add(this.bGetWaferPresenceOnArm);
            this.gbLLRobot.Controls.Add(this.bUnclampWaferOnArm);
            this.gbLLRobot.Controls.Add(this.bClampWaferOnArm);
            this.gbLLRobot.Controls.Add(this.gbSetRobotSpeed);
            this.gbLLRobot.Controls.Add(this.bPreparePlace);
            this.gbLLRobot.Controls.Add(this.gbRobotArmSpeed);
            this.gbLLRobot.Controls.Add(this.bRobotGetSettings);
            this.gbLLRobot.Controls.Add(this.bRobotSetSettings);
            this.gbLLRobot.Controls.Add(this.chRobotCmdGranted);
            this.gbLLRobot.Controls.Add(this.tbRobotDeviceState);
            this.gbLLRobot.Controls.Add(this.cbRobotDestinationSlot);
            this.gbLLRobot.Controls.Add(this.cbRobotDestinationLocation);
            this.gbLLRobot.Controls.Add(this.bRobotTransfer);
            this.gbLLRobot.Controls.Add(this.bRobotPlace);
            this.gbLLRobot.Controls.Add(this.cbRobotArm);
            this.gbLLRobot.Controls.Add(this.cbRobotSourceSlot);
            this.gbLLRobot.Controls.Add(this.cbRobotSourceLocation);
            this.gbLLRobot.Controls.Add(this.bRobotPick);
            this.gbLLRobot.Controls.Add(this.bPreparePick);
            this.gbLLRobot.Controls.Add(this.bRobotHome);
            this.gbLLRobot.Controls.Add(this.bRobotInit);
            this.gbLLRobot.Controls.Add(this.tbRobotLowerArmWaferPresence);
            this.gbLLRobot.Controls.Add(this.tbRobotUpperArmWaferPresence);
            this.gbLLRobot.Location = new System.Drawing.Point(431, 117);
            this.gbLLRobot.Name = "gbLLRobot";
            this.gbLLRobot.Size = new System.Drawing.Size(338, 465);
            this.gbLLRobot.TabIndex = 5;
            this.gbLLRobot.TabStop = false;
            this.gbLLRobot.Text = "Robot";
            // 
            // bGetWaferPresenceOnArm
            // 
            this.bGetWaferPresenceOnArm.Location = new System.Drawing.Point(176, 262);
            this.bGetWaferPresenceOnArm.Name = "bGetWaferPresenceOnArm";
            this.bGetWaferPresenceOnArm.Size = new System.Drawing.Size(156, 23);
            this.bGetWaferPresenceOnArm.TabIndex = 21;
            this.bGetWaferPresenceOnArm.Text = "Get Wafer Presence";
            this.bGetWaferPresenceOnArm.UseVisualStyleBackColor = true;
            this.bGetWaferPresenceOnArm.Click += new System.EventHandler(this.bGetWaferPresenceOnArm_Click);
            // 
            // bUnclampWaferOnArm
            // 
            this.bUnclampWaferOnArm.Location = new System.Drawing.Point(257, 217);
            this.bUnclampWaferOnArm.Name = "bUnclampWaferOnArm";
            this.bUnclampWaferOnArm.Size = new System.Drawing.Size(75, 23);
            this.bUnclampWaferOnArm.TabIndex = 20;
            this.bUnclampWaferOnArm.Text = "Unclamp";
            this.bUnclampWaferOnArm.UseVisualStyleBackColor = true;
            this.bUnclampWaferOnArm.Click += new System.EventHandler(this.bUnclampWaferOnArm_Click);
            // 
            // bClampWaferOnArm
            // 
            this.bClampWaferOnArm.Location = new System.Drawing.Point(176, 217);
            this.bClampWaferOnArm.Name = "bClampWaferOnArm";
            this.bClampWaferOnArm.Size = new System.Drawing.Size(75, 23);
            this.bClampWaferOnArm.TabIndex = 19;
            this.bClampWaferOnArm.Text = "Clamp";
            this.bClampWaferOnArm.UseVisualStyleBackColor = true;
            this.bClampWaferOnArm.Click += new System.EventHandler(this.bClampWaferOnArm_Click);
            // 
            // gbSetRobotSpeed
            // 
            this.gbSetRobotSpeed.Controls.Add(this.tbRobotSpeed);
            this.gbSetRobotSpeed.Controls.Add(this.cbSelectRobotSpeed);
            this.gbSetRobotSpeed.Controls.Add(this.bSetRobotSpeed);
            this.gbSetRobotSpeed.Location = new System.Drawing.Point(15, 117);
            this.gbSetRobotSpeed.Name = "gbSetRobotSpeed";
            this.gbSetRobotSpeed.Size = new System.Drawing.Size(135, 94);
            this.gbSetRobotSpeed.TabIndex = 6;
            this.gbSetRobotSpeed.TabStop = false;
            this.gbSetRobotSpeed.Text = "Set Speed";
            // 
            // tbRobotSpeed
            // 
            this.tbRobotSpeed.BackColor = System.Drawing.SystemColors.Control;
            this.tbRobotSpeed.Location = new System.Drawing.Point(6, 17);
            this.tbRobotSpeed.Name = "tbRobotSpeed";
            this.tbRobotSpeed.ReadOnly = true;
            this.tbRobotSpeed.Size = new System.Drawing.Size(118, 20);
            this.tbRobotSpeed.TabIndex = 19;
            this.tbRobotSpeed.Text = "RobotSpeed";
            this.tbRobotSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // cbSelectRobotSpeed
            // 
            this.cbSelectRobotSpeed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSelectRobotSpeed.FormattingEnabled = true;
            this.cbSelectRobotSpeed.Location = new System.Drawing.Point(6, 40);
            this.cbSelectRobotSpeed.Name = "cbSelectRobotSpeed";
            this.cbSelectRobotSpeed.Size = new System.Drawing.Size(118, 21);
            this.cbSelectRobotSpeed.TabIndex = 1;
            // 
            // bSetRobotSpeed
            // 
            this.bSetRobotSpeed.Location = new System.Drawing.Point(6, 64);
            this.bSetRobotSpeed.Name = "bSetRobotSpeed";
            this.bSetRobotSpeed.Size = new System.Drawing.Size(119, 23);
            this.bSetRobotSpeed.TabIndex = 3;
            this.bSetRobotSpeed.Text = "Set";
            this.bSetRobotSpeed.UseVisualStyleBackColor = true;
            this.bSetRobotSpeed.Click += new System.EventHandler(this.bSetRobotSpeed_Click);
            // 
            // bPreparePlace
            // 
            this.bPreparePlace.Location = new System.Drawing.Point(176, 188);
            this.bPreparePlace.Name = "bPreparePlace";
            this.bPreparePlace.Size = new System.Drawing.Size(107, 23);
            this.bPreparePlace.TabIndex = 18;
            this.bPreparePlace.Text = "Prepare Place";
            this.bPreparePlace.UseVisualStyleBackColor = true;
            this.bPreparePlace.Click += new System.EventHandler(this.bPreparePlace_Click);
            // 
            // gbRobotArmSpeed
            // 
            this.gbRobotArmSpeed.Controls.Add(this.lLDAEWT);
            this.gbRobotArmSpeed.Controls.Add(this.nudLDAEWT);
            this.gbRobotArmSpeed.Controls.Add(this.cbRobotLoadLockTarget);
            this.gbRobotArmSpeed.Controls.Add(this.lULSPAR);
            this.gbRobotArmSpeed.Controls.Add(this.nudULSPAR);
            this.gbRobotArmSpeed.Controls.Add(this.lULSPZD);
            this.gbRobotArmSpeed.Controls.Add(this.nudULSPZD);
            this.gbRobotArmSpeed.Controls.Add(this.lULSPAE);
            this.gbRobotArmSpeed.Controls.Add(this.nudULSPAE);
            this.gbRobotArmSpeed.Controls.Add(this.lLDSPAR);
            this.gbRobotArmSpeed.Controls.Add(this.nudLDSPAR);
            this.gbRobotArmSpeed.Controls.Add(this.lLDSPZU);
            this.gbRobotArmSpeed.Controls.Add(this.nudLDSPZU);
            this.gbRobotArmSpeed.Controls.Add(this.lLDSPAE);
            this.gbRobotArmSpeed.Controls.Add(this.nudLDSPAE);
            this.gbRobotArmSpeed.Enabled = false;
            this.gbRobotArmSpeed.Location = new System.Drawing.Point(15, 216);
            this.gbRobotArmSpeed.Name = "gbRobotArmSpeed";
            this.gbRobotArmSpeed.Size = new System.Drawing.Size(135, 231);
            this.gbRobotArmSpeed.TabIndex = 4;
            this.gbRobotArmSpeed.TabStop = false;
            this.gbRobotArmSpeed.Text = "Arm Speed Parameters";
            // 
            // nudLDAEWT
            // 
            this.nudLDAEWT.Enabled = false;
            this.nudLDAEWT.Location = new System.Drawing.Point(61, 204);
            this.nudLDAEWT.Name = "nudLDAEWT";
            this.nudLDAEWT.Size = new System.Drawing.Size(63, 20);
            this.nudLDAEWT.TabIndex = 14;
            this.nudLDAEWT.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // cbRobotLoadLockTarget
            // 
            this.cbRobotLoadLockTarget.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRobotLoadLockTarget.Enabled = false;
            this.cbRobotLoadLockTarget.FormattingEnabled = true;
            this.cbRobotLoadLockTarget.Location = new System.Drawing.Point(6, 19);
            this.cbRobotLoadLockTarget.Name = "cbRobotLoadLockTarget";
            this.cbRobotLoadLockTarget.Size = new System.Drawing.Size(119, 21);
            this.cbRobotLoadLockTarget.TabIndex = 0;
            // 
            // nudULSPAR
            // 
            this.nudULSPAR.Enabled = false;
            this.nudULSPAR.Location = new System.Drawing.Point(61, 178);
            this.nudULSPAR.Name = "nudULSPAR";
            this.nudULSPAR.Size = new System.Drawing.Size(63, 20);
            this.nudULSPAR.TabIndex = 12;
            this.nudULSPAR.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // nudULSPZD
            // 
            this.nudULSPZD.Enabled = false;
            this.nudULSPZD.Location = new System.Drawing.Point(61, 152);
            this.nudULSPZD.Name = "nudULSPZD";
            this.nudULSPZD.Size = new System.Drawing.Size(63, 20);
            this.nudULSPZD.TabIndex = 10;
            this.nudULSPZD.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // nudULSPAE
            // 
            this.nudULSPAE.Enabled = false;
            this.nudULSPAE.Location = new System.Drawing.Point(61, 126);
            this.nudULSPAE.Name = "nudULSPAE";
            this.nudULSPAE.Size = new System.Drawing.Size(63, 20);
            this.nudULSPAE.TabIndex = 8;
            this.nudULSPAE.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // nudLDSPAR
            // 
            this.nudLDSPAR.Enabled = false;
            this.nudLDSPAR.Location = new System.Drawing.Point(61, 100);
            this.nudLDSPAR.Name = "nudLDSPAR";
            this.nudLDSPAR.Size = new System.Drawing.Size(63, 20);
            this.nudLDSPAR.TabIndex = 6;
            this.nudLDSPAR.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // nudLDSPZU
            // 
            this.nudLDSPZU.Enabled = false;
            this.nudLDSPZU.Location = new System.Drawing.Point(61, 74);
            this.nudLDSPZU.Name = "nudLDSPZU";
            this.nudLDSPZU.Size = new System.Drawing.Size(63, 20);
            this.nudLDSPZU.TabIndex = 4;
            this.nudLDSPZU.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // nudLDSPAE
            // 
            this.nudLDSPAE.Enabled = false;
            this.nudLDSPAE.Location = new System.Drawing.Point(61, 48);
            this.nudLDSPAE.Name = "nudLDSPAE";
            this.nudLDSPAE.Size = new System.Drawing.Size(63, 20);
            this.nudLDSPAE.TabIndex = 2;
            this.nudLDSPAE.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // bRobotGetSettings
            // 
            this.bRobotGetSettings.Enabled = false;
            this.bRobotGetSettings.Location = new System.Drawing.Point(257, 387);
            this.bRobotGetSettings.Name = "bRobotGetSettings";
            this.bRobotGetSettings.Size = new System.Drawing.Size(75, 23);
            this.bRobotGetSettings.TabIndex = 17;
            this.bRobotGetSettings.Text = "Get Settings";
            this.bRobotGetSettings.UseVisualStyleBackColor = true;
            this.bRobotGetSettings.Click += new System.EventHandler(this.RobotGetSettings_Click);
            // 
            // bRobotSetSettings
            // 
            this.bRobotSetSettings.Enabled = false;
            this.bRobotSetSettings.Location = new System.Drawing.Point(176, 387);
            this.bRobotSetSettings.Name = "bRobotSetSettings";
            this.bRobotSetSettings.Size = new System.Drawing.Size(75, 23);
            this.bRobotSetSettings.TabIndex = 16;
            this.bRobotSetSettings.Text = "Set Settings";
            this.bRobotSetSettings.UseVisualStyleBackColor = true;
            this.bRobotSetSettings.Click += new System.EventHandler(this.RobotSetSettings_Click);
            // 
            // chRobotCmdGranted
            // 
            this.chRobotCmdGranted.AutoSize = true;
            this.chRobotCmdGranted.Checked = true;
            this.chRobotCmdGranted.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chRobotCmdGranted.Enabled = false;
            this.chRobotCmdGranted.Location = new System.Drawing.Point(15, 94);
            this.chRobotCmdGranted.Name = "chRobotCmdGranted";
            this.chRobotCmdGranted.Size = new System.Drawing.Size(125, 17);
            this.chRobotCmdGranted.TabIndex = 3;
            this.chRobotCmdGranted.Text = "Is Command Granted";
            this.chRobotCmdGranted.UseVisualStyleBackColor = true;
            // 
            // tbRobotDeviceState
            // 
            this.tbRobotDeviceState.BackColor = System.Drawing.SystemColors.Control;
            this.tbRobotDeviceState.Location = new System.Drawing.Point(15, 16);
            this.tbRobotDeviceState.Name = "tbRobotDeviceState";
            this.tbRobotDeviceState.ReadOnly = true;
            this.tbRobotDeviceState.Size = new System.Drawing.Size(147, 20);
            this.tbRobotDeviceState.TabIndex = 0;
            this.tbRobotDeviceState.Text = "Device State";
            this.tbRobotDeviceState.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // cbRobotDestinationSlot
            // 
            this.cbRobotDestinationSlot.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRobotDestinationSlot.Enabled = false;
            this.cbRobotDestinationSlot.FormattingEnabled = true;
            this.cbRobotDestinationSlot.Location = new System.Drawing.Point(176, 331);
            this.cbRobotDestinationSlot.Name = "cbRobotDestinationSlot";
            this.cbRobotDestinationSlot.Size = new System.Drawing.Size(156, 21);
            this.cbRobotDestinationSlot.TabIndex = 14;
            // 
            // cbRobotDestinationLocation
            // 
            this.cbRobotDestinationLocation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRobotDestinationLocation.Enabled = false;
            this.cbRobotDestinationLocation.FormattingEnabled = true;
            this.cbRobotDestinationLocation.Location = new System.Drawing.Point(176, 305);
            this.cbRobotDestinationLocation.Name = "cbRobotDestinationLocation";
            this.cbRobotDestinationLocation.Size = new System.Drawing.Size(156, 21);
            this.cbRobotDestinationLocation.TabIndex = 13;
            this.cbRobotDestinationLocation.SelectedValueChanged += new System.EventHandler(this.RobotDestinationLocation_SelectedValueChanged);
            // 
            // bRobotTransfer
            // 
            this.bRobotTransfer.Enabled = false;
            this.bRobotTransfer.Location = new System.Drawing.Point(176, 358);
            this.bRobotTransfer.Name = "bRobotTransfer";
            this.bRobotTransfer.Size = new System.Drawing.Size(156, 23);
            this.bRobotTransfer.TabIndex = 15;
            this.bRobotTransfer.Text = "Transfer";
            this.bRobotTransfer.UseVisualStyleBackColor = true;
            this.bRobotTransfer.Click += new System.EventHandler(this.RobotTransfer_Click);
            // 
            // bRobotPlace
            // 
            this.bRobotPlace.Location = new System.Drawing.Point(289, 188);
            this.bRobotPlace.Name = "bRobotPlace";
            this.bRobotPlace.Size = new System.Drawing.Size(43, 23);
            this.bRobotPlace.TabIndex = 12;
            this.bRobotPlace.Text = "Place";
            this.bRobotPlace.UseVisualStyleBackColor = true;
            this.bRobotPlace.Click += new System.EventHandler(this.RobotPlace_Click);
            // 
            // cbRobotArm
            // 
            this.cbRobotArm.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRobotArm.FormattingEnabled = true;
            this.cbRobotArm.Location = new System.Drawing.Point(176, 131);
            this.cbRobotArm.Name = "cbRobotArm";
            this.cbRobotArm.Size = new System.Drawing.Size(156, 21);
            this.cbRobotArm.TabIndex = 9;
            // 
            // cbRobotSourceSlot
            // 
            this.cbRobotSourceSlot.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRobotSourceSlot.FormattingEnabled = true;
            this.cbRobotSourceSlot.Location = new System.Drawing.Point(176, 104);
            this.cbRobotSourceSlot.Name = "cbRobotSourceSlot";
            this.cbRobotSourceSlot.Size = new System.Drawing.Size(156, 21);
            this.cbRobotSourceSlot.TabIndex = 8;
            // 
            // cbRobotSourceLocation
            // 
            this.cbRobotSourceLocation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRobotSourceLocation.FormattingEnabled = true;
            this.cbRobotSourceLocation.Location = new System.Drawing.Point(176, 77);
            this.cbRobotSourceLocation.Name = "cbRobotSourceLocation";
            this.cbRobotSourceLocation.Size = new System.Drawing.Size(156, 21);
            this.cbRobotSourceLocation.TabIndex = 7;
            this.cbRobotSourceLocation.SelectedValueChanged += new System.EventHandler(this.RobotSourceLocation_SelectedValueChanged);
            // 
            // bRobotPick
            // 
            this.bRobotPick.Location = new System.Drawing.Point(289, 159);
            this.bRobotPick.Name = "bRobotPick";
            this.bRobotPick.Size = new System.Drawing.Size(43, 23);
            this.bRobotPick.TabIndex = 11;
            this.bRobotPick.Text = "Pick";
            this.bRobotPick.UseVisualStyleBackColor = true;
            this.bRobotPick.Click += new System.EventHandler(this.RobotPick_Click);
            // 
            // bPreparePick
            // 
            this.bPreparePick.Location = new System.Drawing.Point(176, 159);
            this.bPreparePick.Name = "bPreparePick";
            this.bPreparePick.Size = new System.Drawing.Size(107, 23);
            this.bPreparePick.TabIndex = 10;
            this.bPreparePick.Text = "Prepare Pick";
            this.bPreparePick.UseVisualStyleBackColor = true;
            this.bPreparePick.Click += new System.EventHandler(this.RobotPreparePick_Click);
            // 
            // bRobotHome
            // 
            this.bRobotHome.Location = new System.Drawing.Point(176, 48);
            this.bRobotHome.Name = "bRobotHome";
            this.bRobotHome.Size = new System.Drawing.Size(156, 23);
            this.bRobotHome.TabIndex = 6;
            this.bRobotHome.Text = "Home";
            this.bRobotHome.UseVisualStyleBackColor = true;
            this.bRobotHome.Click += new System.EventHandler(this.RobotHome_Click);
            // 
            // bRobotInit
            // 
            this.bRobotInit.Location = new System.Drawing.Point(176, 19);
            this.bRobotInit.Name = "bRobotInit";
            this.bRobotInit.Size = new System.Drawing.Size(156, 23);
            this.bRobotInit.TabIndex = 5;
            this.bRobotInit.Text = "Initialization";
            this.bRobotInit.UseVisualStyleBackColor = true;
            this.bRobotInit.Click += new System.EventHandler(this.RobotInit_Click);
            // 
            // tbRobotLowerArmWaferPresence
            // 
            this.tbRobotLowerArmWaferPresence.BackColor = System.Drawing.SystemColors.Control;
            this.tbRobotLowerArmWaferPresence.Location = new System.Drawing.Point(15, 68);
            this.tbRobotLowerArmWaferPresence.Name = "tbRobotLowerArmWaferPresence";
            this.tbRobotLowerArmWaferPresence.ReadOnly = true;
            this.tbRobotLowerArmWaferPresence.Size = new System.Drawing.Size(147, 20);
            this.tbRobotLowerArmWaferPresence.TabIndex = 2;
            this.tbRobotLowerArmWaferPresence.Text = "Wafer Presence Lower Arm";
            this.tbRobotLowerArmWaferPresence.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbRobotUpperArmWaferPresence
            // 
            this.tbRobotUpperArmWaferPresence.BackColor = System.Drawing.SystemColors.Control;
            this.tbRobotUpperArmWaferPresence.Location = new System.Drawing.Point(15, 42);
            this.tbRobotUpperArmWaferPresence.Name = "tbRobotUpperArmWaferPresence";
            this.tbRobotUpperArmWaferPresence.ReadOnly = true;
            this.tbRobotUpperArmWaferPresence.Size = new System.Drawing.Size(147, 20);
            this.tbRobotUpperArmWaferPresence.TabIndex = 1;
            this.tbRobotUpperArmWaferPresence.Text = "Wafer Presence Upper Arm";
            this.tbRobotUpperArmWaferPresence.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // gbLLLoadPorts
            // 
            this.gbLLLoadPorts.Controls.Add(this.bLPGetCarrierType);
            this.gbLLLoadPorts.Controls.Add(this.tbLPCarrierType);
            this.gbLLLoadPorts.Controls.Add(this.bLPUnclamp);
            this.gbLLLoadPorts.Controls.Add(this.bLPClamp);
            this.gbLLLoadPorts.Controls.Add(this.tbLPWaferSize);
            this.gbLLLoadPorts.Controls.Add(this.bLPGetSize);
            this.gbLLLoadPorts.Controls.Add(this.chIsPurgeEnabled);
            this.gbLLLoadPorts.Controls.Add(this.gbE84);
            this.gbLLLoadPorts.Controls.Add(this.bLPMoveToWritePosition);
            this.gbLLLoadPorts.Controls.Add(this.bLPMoveToReadPosition);
            this.gbLLLoadPorts.Controls.Add(this.tbLPCarrierID);
            this.gbLLLoadPorts.Controls.Add(this.gbCarrierID);
            this.gbLLLoadPorts.Controls.Add(this.bLPRelease);
            this.gbLLLoadPorts.Controls.Add(this.bLPInAccess);
            this.gbLLLoadPorts.Controls.Add(this.tbLPMapping);
            this.gbLLLoadPorts.Controls.Add(this.tbLPDeviceState);
            this.gbLLLoadPorts.Controls.Add(this.bLPMap);
            this.gbLLLoadPorts.Controls.Add(this.bLPLastMapping);
            this.gbLLLoadPorts.Controls.Add(this.tbLPInAccess);
            this.gbLLLoadPorts.Controls.Add(this.tbLPPlacement);
            this.gbLLLoadPorts.Controls.Add(this.tbLPPresence);
            this.gbLLLoadPorts.Controls.Add(this.cbLLLoadPorts);
            this.gbLLLoadPorts.Controls.Add(this.bLPDock);
            this.gbLLLoadPorts.Controls.Add(this.bLPUndock);
            this.gbLLLoadPorts.Controls.Add(this.bLPInit);
            this.gbLLLoadPorts.Controls.Add(this.bLPOpen);
            this.gbLLLoadPorts.Controls.Add(this.bLPClose);
            this.gbLLLoadPorts.Controls.Add(this.bLPHome);
            this.gbLLLoadPorts.Controls.Add(this.bLPReadCarrierID);
            this.gbLLLoadPorts.Location = new System.Drawing.Point(6, 117);
            this.gbLLLoadPorts.Name = "gbLLLoadPorts";
            this.gbLLLoadPorts.Size = new System.Drawing.Size(419, 465);
            this.gbLLLoadPorts.TabIndex = 2;
            this.gbLLLoadPorts.TabStop = false;
            this.gbLLLoadPorts.Text = "Load Ports";
            // 
            // bLPGetCarrierType
            // 
            this.bLPGetCarrierType.Location = new System.Drawing.Point(295, 128);
            this.bLPGetCarrierType.Name = "bLPGetCarrierType";
            this.bLPGetCarrierType.Size = new System.Drawing.Size(75, 23);
            this.bLPGetCarrierType.TabIndex = 41;
            this.bLPGetCarrierType.Text = "Get Type";
            this.bLPGetCarrierType.UseVisualStyleBackColor = true;
            this.bLPGetCarrierType.Click += new System.EventHandler(this.bLPGetCarrierType_Click);
            // 
            // tbLPCarrierType
            // 
            this.tbLPCarrierType.BackColor = System.Drawing.SystemColors.Control;
            this.tbLPCarrierType.Enabled = false;
            this.tbLPCarrierType.Location = new System.Drawing.Point(22, 175);
            this.tbLPCarrierType.Name = "tbLPCarrierType";
            this.tbLPCarrierType.ReadOnly = true;
            this.tbLPCarrierType.Size = new System.Drawing.Size(100, 20);
            this.tbLPCarrierType.TabIndex = 40;
            this.tbLPCarrierType.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // bLPUnclamp
            // 
            this.bLPUnclamp.Location = new System.Drawing.Point(295, 99);
            this.bLPUnclamp.Name = "bLPUnclamp";
            this.bLPUnclamp.Size = new System.Drawing.Size(75, 23);
            this.bLPUnclamp.TabIndex = 39;
            this.bLPUnclamp.Text = "Unclamp";
            this.bLPUnclamp.UseVisualStyleBackColor = true;
            this.bLPUnclamp.Click += new System.EventHandler(this.bLPUnclamp_Click);
            // 
            // bLPClamp
            // 
            this.bLPClamp.Location = new System.Drawing.Point(215, 99);
            this.bLPClamp.Name = "bLPClamp";
            this.bLPClamp.Size = new System.Drawing.Size(75, 23);
            this.bLPClamp.TabIndex = 38;
            this.bLPClamp.Text = "Clamp";
            this.bLPClamp.UseVisualStyleBackColor = true;
            this.bLPClamp.Click += new System.EventHandler(this.bLPClamp_Click);
            // 
            // tbLPWaferSize
            // 
            this.tbLPWaferSize.BackColor = System.Drawing.SystemColors.Control;
            this.tbLPWaferSize.Enabled = false;
            this.tbLPWaferSize.Location = new System.Drawing.Point(22, 149);
            this.tbLPWaferSize.Name = "tbLPWaferSize";
            this.tbLPWaferSize.ReadOnly = true;
            this.tbLPWaferSize.Size = new System.Drawing.Size(100, 20);
            this.tbLPWaferSize.TabIndex = 37;
            this.tbLPWaferSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // bLPGetSize
            // 
            this.bLPGetSize.Location = new System.Drawing.Point(215, 128);
            this.bLPGetSize.Name = "bLPGetSize";
            this.bLPGetSize.Size = new System.Drawing.Size(75, 23);
            this.bLPGetSize.TabIndex = 36;
            this.bLPGetSize.Text = "Get Size";
            this.bLPGetSize.UseVisualStyleBackColor = true;
            this.bLPGetSize.Click += new System.EventHandler(this.bLPGetSize_Click);
            // 
            // chIsPurgeEnabled
            // 
            this.chIsPurgeEnabled.AutoSize = true;
            this.chIsPurgeEnabled.Enabled = false;
            this.chIsPurgeEnabled.Location = new System.Drawing.Point(295, 188);
            this.chIsPurgeEnabled.Name = "chIsPurgeEnabled";
            this.chIsPurgeEnabled.Size = new System.Drawing.Size(107, 17);
            this.chIsPurgeEnabled.TabIndex = 27;
            this.chIsPurgeEnabled.Text = "Is Purge Enabled";
            this.chIsPurgeEnabled.UseVisualStyleBackColor = true;
            // 
            // gbE84
            // 
            this.gbE84.Controls.Add(this.bAbortE84);
            this.gbE84.Controls.Add(this.bResetE84);
            this.gbE84.Controls.Add(this.bEnableE84);
            this.gbE84.Controls.Add(this.bDisableE84);
            this.gbE84.Controls.Add(this.bGetE84Inputs);
            this.gbE84.Controls.Add(this.bE84Stop);
            this.gbE84.Controls.Add(this.bE84Unload);
            this.gbE84.Controls.Add(this.bGetE84Outputs);
            this.gbE84.Location = new System.Drawing.Point(213, 328);
            this.gbE84.Name = "gbE84";
            this.gbE84.Size = new System.Drawing.Size(200, 132);
            this.gbE84.TabIndex = 35;
            this.gbE84.TabStop = false;
            this.gbE84.Text = "E84";
            // 
            // bAbortE84
            // 
            this.bAbortE84.Location = new System.Drawing.Point(6, 101);
            this.bAbortE84.Name = "bAbortE84";
            this.bAbortE84.Size = new System.Drawing.Size(69, 23);
            this.bAbortE84.TabIndex = 7;
            this.bAbortE84.Text = "Abort";
            this.bAbortE84.UseVisualStyleBackColor = true;
            this.bAbortE84.Click += new System.EventHandler(this.bAbortE84_Click);
            // 
            // bResetE84
            // 
            this.bResetE84.Location = new System.Drawing.Point(6, 71);
            this.bResetE84.Name = "bResetE84";
            this.bResetE84.Size = new System.Drawing.Size(69, 23);
            this.bResetE84.TabIndex = 6;
            this.bResetE84.Text = "Reset";
            this.bResetE84.UseVisualStyleBackColor = true;
            this.bResetE84.Click += new System.EventHandler(this.bResetE84_Click);
            // 
            // bEnableE84
            // 
            this.bEnableE84.Location = new System.Drawing.Point(6, 12);
            this.bEnableE84.Name = "bEnableE84";
            this.bEnableE84.Size = new System.Drawing.Size(69, 23);
            this.bEnableE84.TabIndex = 5;
            this.bEnableE84.Text = "Enable";
            this.bEnableE84.UseVisualStyleBackColor = true;
            this.bEnableE84.Click += new System.EventHandler(this.bEnableE84_Click);
            // 
            // bDisableE84
            // 
            this.bDisableE84.Location = new System.Drawing.Point(6, 42);
            this.bDisableE84.Name = "bDisableE84";
            this.bDisableE84.Size = new System.Drawing.Size(69, 23);
            this.bDisableE84.TabIndex = 4;
            this.bDisableE84.Text = "Disable";
            this.bDisableE84.UseVisualStyleBackColor = true;
            this.bDisableE84.Click += new System.EventHandler(this.bDisableE84_Click);
            // 
            // bGetE84Inputs
            // 
            this.bGetE84Inputs.Location = new System.Drawing.Point(84, 13);
            this.bGetE84Inputs.Name = "bGetE84Inputs";
            this.bGetE84Inputs.Size = new System.Drawing.Size(111, 23);
            this.bGetE84Inputs.TabIndex = 0;
            this.bGetE84Inputs.Text = "Get Inputs";
            this.bGetE84Inputs.UseVisualStyleBackColor = true;
            this.bGetE84Inputs.Click += new System.EventHandler(this.GetE84Inputs_Click);
            // 
            // bE84Stop
            // 
            this.bE84Stop.Enabled = false;
            this.bE84Stop.Location = new System.Drawing.Point(84, 101);
            this.bE84Stop.Name = "bE84Stop";
            this.bE84Stop.Size = new System.Drawing.Size(110, 23);
            this.bE84Stop.TabIndex = 3;
            this.bE84Stop.Text = "TransReq STOP";
            this.bE84Stop.UseVisualStyleBackColor = true;
            this.bE84Stop.Click += new System.EventHandler(this.E84Stop_Click);
            // 
            // bE84Unload
            // 
            this.bE84Unload.Enabled = false;
            this.bE84Unload.Location = new System.Drawing.Point(84, 72);
            this.bE84Unload.Name = "bE84Unload";
            this.bE84Unload.Size = new System.Drawing.Size(111, 23);
            this.bE84Unload.TabIndex = 2;
            this.bE84Unload.Text = "TransReq UNLOAD";
            this.bE84Unload.UseVisualStyleBackColor = true;
            this.bE84Unload.Click += new System.EventHandler(this.E84Unload_Click);
            // 
            // bGetE84Outputs
            // 
            this.bGetE84Outputs.Location = new System.Drawing.Point(84, 42);
            this.bGetE84Outputs.Name = "bGetE84Outputs";
            this.bGetE84Outputs.Size = new System.Drawing.Size(111, 23);
            this.bGetE84Outputs.TabIndex = 1;
            this.bGetE84Outputs.Text = "Get Outputs";
            this.bGetE84Outputs.UseVisualStyleBackColor = true;
            this.bGetE84Outputs.Click += new System.EventHandler(this.GetE84Outputs_Click);
            // 
            // bLPMoveToWritePosition
            // 
            this.bLPMoveToWritePosition.Enabled = false;
            this.bLPMoveToWritePosition.Location = new System.Drawing.Point(297, 271);
            this.bLPMoveToWritePosition.Name = "bLPMoveToWritePosition";
            this.bLPMoveToWritePosition.Size = new System.Drawing.Size(75, 23);
            this.bLPMoveToWritePosition.TabIndex = 32;
            this.bLPMoveToWritePosition.Text = "MveToWrite";
            this.bLPMoveToWritePosition.UseVisualStyleBackColor = true;
            this.bLPMoveToWritePosition.Click += new System.EventHandler(this.LPMoveToWritePosition_Click);
            // 
            // bLPMoveToReadPosition
            // 
            this.bLPMoveToReadPosition.Enabled = false;
            this.bLPMoveToReadPosition.Location = new System.Drawing.Point(295, 157);
            this.bLPMoveToReadPosition.Name = "bLPMoveToReadPosition";
            this.bLPMoveToReadPosition.Size = new System.Drawing.Size(75, 23);
            this.bLPMoveToReadPosition.TabIndex = 25;
            this.bLPMoveToReadPosition.Text = "MveToRead";
            this.bLPMoveToReadPosition.UseVisualStyleBackColor = true;
            this.bLPMoveToReadPosition.Click += new System.EventHandler(this.LPMoveToReadPosition_Click);
            // 
            // tbLPCarrierID
            // 
            this.tbLPCarrierID.BackColor = System.Drawing.SystemColors.Control;
            this.tbLPCarrierID.Enabled = false;
            this.tbLPCarrierID.Location = new System.Drawing.Point(22, 123);
            this.tbLPCarrierID.Name = "tbLPCarrierID";
            this.tbLPCarrierID.ReadOnly = true;
            this.tbLPCarrierID.Size = new System.Drawing.Size(100, 20);
            this.tbLPCarrierID.TabIndex = 4;
            this.tbLPCarrierID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // gbCarrierID
            // 
            this.gbCarrierID.Controls.Add(this.bCarrierIDEnter);
            this.gbCarrierID.Controls.Add(this.tbCarrierID);
            this.gbCarrierID.Enabled = false;
            this.gbCarrierID.Location = new System.Drawing.Point(7, 399);
            this.gbCarrierID.Name = "gbCarrierID";
            this.gbCarrierID.Size = new System.Drawing.Size(200, 44);
            this.gbCarrierID.TabIndex = 15;
            this.gbCarrierID.TabStop = false;
            this.gbCarrierID.Text = "CarrierID";
            // 
            // bCarrierIDEnter
            // 
            this.bCarrierIDEnter.Enabled = false;
            this.bCarrierIDEnter.Location = new System.Drawing.Point(125, 16);
            this.bCarrierIDEnter.Name = "bCarrierIDEnter";
            this.bCarrierIDEnter.Size = new System.Drawing.Size(69, 23);
            this.bCarrierIDEnter.TabIndex = 1;
            this.bCarrierIDEnter.Text = "Enter";
            this.bCarrierIDEnter.UseVisualStyleBackColor = true;
            this.bCarrierIDEnter.Click += new System.EventHandler(this.CarrierIDEnter_Click);
            // 
            // tbCarrierID
            // 
            this.tbCarrierID.Enabled = false;
            this.tbCarrierID.Location = new System.Drawing.Point(6, 18);
            this.tbCarrierID.Name = "tbCarrierID";
            this.tbCarrierID.Size = new System.Drawing.Size(113, 20);
            this.tbCarrierID.TabIndex = 0;
            // 
            // bLPRelease
            // 
            this.bLPRelease.Location = new System.Drawing.Point(215, 300);
            this.bLPRelease.Name = "bLPRelease";
            this.bLPRelease.Size = new System.Drawing.Size(156, 23);
            this.bLPRelease.TabIndex = 33;
            this.bLPRelease.Text = "Release";
            this.bLPRelease.UseVisualStyleBackColor = true;
            this.bLPRelease.Click += new System.EventHandler(this.LPRelease_Click);
            // 
            // bLPInAccess
            // 
            this.bLPInAccess.Location = new System.Drawing.Point(215, 214);
            this.bLPInAccess.Name = "bLPInAccess";
            this.bLPInAccess.Size = new System.Drawing.Size(156, 23);
            this.bLPInAccess.TabIndex = 28;
            this.bLPInAccess.Text = "In Access";
            this.bLPInAccess.UseVisualStyleBackColor = true;
            this.bLPInAccess.Click += new System.EventHandler(this.LPInAccess_Click);
            // 
            // tbLPMapping
            // 
            this.tbLPMapping.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbLPMapping.Location = new System.Drawing.Point(128, 43);
            this.tbLPMapping.Multiline = true;
            this.tbLPMapping.Name = "tbLPMapping";
            this.tbLPMapping.ReadOnly = true;
            this.tbLPMapping.Size = new System.Drawing.Size(81, 350);
            this.tbLPMapping.TabIndex = 17;
            // 
            // tbLPDeviceState
            // 
            this.tbLPDeviceState.BackColor = System.Drawing.SystemColors.Control;
            this.tbLPDeviceState.Location = new System.Drawing.Point(22, 17);
            this.tbLPDeviceState.Name = "tbLPDeviceState";
            this.tbLPDeviceState.ReadOnly = true;
            this.tbLPDeviceState.Size = new System.Drawing.Size(100, 20);
            this.tbLPDeviceState.TabIndex = 0;
            this.tbLPDeviceState.Text = "LP Device State";
            this.tbLPDeviceState.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // bLPMap
            // 
            this.bLPMap.Location = new System.Drawing.Point(215, 242);
            this.bLPMap.Name = "bLPMap";
            this.bLPMap.Size = new System.Drawing.Size(75, 23);
            this.bLPMap.TabIndex = 29;
            this.bLPMap.Text = "Map";
            this.bLPMap.UseVisualStyleBackColor = true;
            this.bLPMap.Click += new System.EventHandler(this.LPMap_Click);
            // 
            // bLPLastMapping
            // 
            this.bLPLastMapping.Location = new System.Drawing.Point(296, 242);
            this.bLPLastMapping.Name = "bLPLastMapping";
            this.bLPLastMapping.Size = new System.Drawing.Size(75, 23);
            this.bLPLastMapping.TabIndex = 30;
            this.bLPLastMapping.Text = "Last Mapping";
            this.bLPLastMapping.UseVisualStyleBackColor = true;
            this.bLPLastMapping.Click += new System.EventHandler(this.LPLastMapping_Click);
            // 
            // tbLPInAccess
            // 
            this.tbLPInAccess.BackColor = System.Drawing.SystemColors.Control;
            this.tbLPInAccess.Location = new System.Drawing.Point(22, 97);
            this.tbLPInAccess.Name = "tbLPInAccess";
            this.tbLPInAccess.ReadOnly = true;
            this.tbLPInAccess.Size = new System.Drawing.Size(100, 20);
            this.tbLPInAccess.TabIndex = 3;
            this.tbLPInAccess.Text = "LP In Access";
            this.tbLPInAccess.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbLPPlacement
            // 
            this.tbLPPlacement.BackColor = System.Drawing.SystemColors.Control;
            this.tbLPPlacement.Location = new System.Drawing.Point(22, 70);
            this.tbLPPlacement.Name = "tbLPPlacement";
            this.tbLPPlacement.ReadOnly = true;
            this.tbLPPlacement.Size = new System.Drawing.Size(100, 20);
            this.tbLPPlacement.TabIndex = 2;
            this.tbLPPlacement.Text = "LP Placement";
            this.tbLPPlacement.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbLPPresence
            // 
            this.tbLPPresence.BackColor = System.Drawing.SystemColors.Control;
            this.tbLPPresence.Location = new System.Drawing.Point(22, 44);
            this.tbLPPresence.Name = "tbLPPresence";
            this.tbLPPresence.ReadOnly = true;
            this.tbLPPresence.Size = new System.Drawing.Size(100, 20);
            this.tbLPPresence.TabIndex = 1;
            this.tbLPPresence.Text = "LP Presence";
            this.tbLPPresence.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // cbLLLoadPorts
            // 
            this.cbLLLoadPorts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLLLoadPorts.FormattingEnabled = true;
            this.cbLLLoadPorts.Location = new System.Drawing.Point(128, 16);
            this.cbLLLoadPorts.Name = "cbLLLoadPorts";
            this.cbLLLoadPorts.Size = new System.Drawing.Size(243, 21);
            this.cbLLLoadPorts.TabIndex = 16;
            this.cbLLLoadPorts.SelectedIndexChanged += new System.EventHandler(this.cbLLLoadPorts_SelectedIndexChanged);
            // 
            // bLPDock
            // 
            this.bLPDock.Enabled = false;
            this.bLPDock.Location = new System.Drawing.Point(215, 72);
            this.bLPDock.Name = "bLPDock";
            this.bLPDock.Size = new System.Drawing.Size(75, 23);
            this.bLPDock.TabIndex = 22;
            this.bLPDock.Text = "Dock";
            this.bLPDock.UseVisualStyleBackColor = true;
            this.bLPDock.Click += new System.EventHandler(this.LPDock_Click);
            // 
            // bLPUndock
            // 
            this.bLPUndock.Enabled = false;
            this.bLPUndock.Location = new System.Drawing.Point(295, 72);
            this.bLPUndock.Name = "bLPUndock";
            this.bLPUndock.Size = new System.Drawing.Size(75, 23);
            this.bLPUndock.TabIndex = 23;
            this.bLPUndock.Text = "Undock";
            this.bLPUndock.UseVisualStyleBackColor = true;
            this.bLPUndock.Click += new System.EventHandler(this.LPUndock_Click);
            // 
            // bLPInit
            // 
            this.bLPInit.Location = new System.Drawing.Point(215, 43);
            this.bLPInit.Name = "bLPInit";
            this.bLPInit.Size = new System.Drawing.Size(75, 23);
            this.bLPInit.TabIndex = 18;
            this.bLPInit.Text = "Initialization";
            this.bLPInit.UseVisualStyleBackColor = true;
            this.bLPInit.Click += new System.EventHandler(this.LPInit_Click);
            // 
            // bLPOpen
            // 
            this.bLPOpen.Enabled = false;
            this.bLPOpen.Location = new System.Drawing.Point(215, 185);
            this.bLPOpen.Name = "bLPOpen";
            this.bLPOpen.Size = new System.Drawing.Size(75, 23);
            this.bLPOpen.TabIndex = 26;
            this.bLPOpen.Text = "Open";
            this.bLPOpen.UseVisualStyleBackColor = true;
            this.bLPOpen.Click += new System.EventHandler(this.LPOpen_Click);
            // 
            // bLPClose
            // 
            this.bLPClose.Location = new System.Drawing.Point(215, 271);
            this.bLPClose.Name = "bLPClose";
            this.bLPClose.Size = new System.Drawing.Size(75, 23);
            this.bLPClose.TabIndex = 31;
            this.bLPClose.Text = "Close";
            this.bLPClose.UseVisualStyleBackColor = true;
            this.bLPClose.Click += new System.EventHandler(this.LPClose_Click);
            // 
            // bLPHome
            // 
            this.bLPHome.Enabled = false;
            this.bLPHome.Location = new System.Drawing.Point(295, 43);
            this.bLPHome.Name = "bLPHome";
            this.bLPHome.Size = new System.Drawing.Size(75, 23);
            this.bLPHome.TabIndex = 19;
            this.bLPHome.Text = "Home";
            this.bLPHome.UseVisualStyleBackColor = true;
            // 
            // bLPReadCarrierID
            // 
            this.bLPReadCarrierID.Location = new System.Drawing.Point(215, 157);
            this.bLPReadCarrierID.Name = "bLPReadCarrierID";
            this.bLPReadCarrierID.Size = new System.Drawing.Size(75, 23);
            this.bLPReadCarrierID.TabIndex = 24;
            this.bLPReadCarrierID.Text = "Read ID";
            this.bLPReadCarrierID.UseVisualStyleBackColor = true;
            this.bLPReadCarrierID.Click += new System.EventHandler(this.LPReadCarrierID_Click);
            // 
            // gbLLAligner
            // 
            this.gbLLAligner.Controls.Add(this.button1);
            this.gbLLAligner.Controls.Add(this.gbOcr);
            this.gbLLAligner.Controls.Add(this.bUnclampOnAligner);
            this.gbLLAligner.Controls.Add(this.bClampOnAligner);
            this.gbLLAligner.Controls.Add(this.tbAlignerDeviceState);
            this.gbLLAligner.Controls.Add(this.tbAlignerWaferPresence);
            this.gbLLAligner.Controls.Add(this.bAlignerInit);
            this.gbLLAligner.Controls.Add(this.bAlignerAlign);
            this.gbLLAligner.Controls.Add(this.tbAlignerAlignAngle);
            this.gbLLAligner.Location = new System.Drawing.Point(6, 14);
            this.gbLLAligner.Name = "gbLLAligner";
            this.gbLLAligner.Size = new System.Drawing.Size(676, 102);
            this.gbLLAligner.TabIndex = 0;
            this.gbLLAligner.TabStop = false;
            this.gbLLAligner.Text = "Aligner";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(219, 77);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(85, 23);
            this.button1.TabIndex = 8;
            this.button1.Text = "Centering";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.AlignerCentering_Click);
            // 
            // gbOcr
            // 
            this.gbOcr.Controls.Add(this.cbOcrBackSideRecipes);
            this.gbOcr.Controls.Add(this.tbOCRWaferIDBackSide);
            this.gbOcr.Controls.Add(this.tbOCRWaferIDFrontSide);
            this.gbOcr.Controls.Add(this.bOcrRead);
            this.gbOcr.Controls.Add(this.cbOcrWaferType);
            this.gbOcr.Controls.Add(this.cbOcrWaferSide);
            this.gbOcr.Controls.Add(this.cbOcrFrontSideRecipes);
            this.gbOcr.Controls.Add(this.bOcrGetRecipes);
            this.gbOcr.Location = new System.Drawing.Point(310, 5);
            this.gbOcr.Name = "gbOcr";
            this.gbOcr.Size = new System.Drawing.Size(360, 95);
            this.gbOcr.TabIndex = 7;
            this.gbOcr.TabStop = false;
            this.gbOcr.Text = "OCR";
            // 
            // cbOcrBackSideRecipes
            // 
            this.cbOcrBackSideRecipes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbOcrBackSideRecipes.FormattingEnabled = true;
            this.cbOcrBackSideRecipes.Location = new System.Drawing.Point(163, 67);
            this.cbOcrBackSideRecipes.Name = "cbOcrBackSideRecipes";
            this.cbOcrBackSideRecipes.Size = new System.Drawing.Size(106, 21);
            this.cbOcrBackSideRecipes.TabIndex = 42;
            // 
            // tbOCRWaferIDBackSide
            // 
            this.tbOCRWaferIDBackSide.BackColor = System.Drawing.SystemColors.Control;
            this.tbOCRWaferIDBackSide.Enabled = false;
            this.tbOCRWaferIDBackSide.Location = new System.Drawing.Point(6, 68);
            this.tbOCRWaferIDBackSide.Name = "tbOCRWaferIDBackSide";
            this.tbOCRWaferIDBackSide.ReadOnly = true;
            this.tbOCRWaferIDBackSide.Size = new System.Drawing.Size(151, 20);
            this.tbOCRWaferIDBackSide.TabIndex = 41;
            this.tbOCRWaferIDBackSide.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbOCRWaferIDFrontSide
            // 
            this.tbOCRWaferIDFrontSide.BackColor = System.Drawing.SystemColors.Control;
            this.tbOCRWaferIDFrontSide.Enabled = false;
            this.tbOCRWaferIDFrontSide.Location = new System.Drawing.Point(6, 41);
            this.tbOCRWaferIDFrontSide.Name = "tbOCRWaferIDFrontSide";
            this.tbOCRWaferIDFrontSide.ReadOnly = true;
            this.tbOCRWaferIDFrontSide.Size = new System.Drawing.Size(151, 20);
            this.tbOCRWaferIDFrontSide.TabIndex = 40;
            this.tbOCRWaferIDFrontSide.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // bOcrRead
            // 
            this.bOcrRead.Location = new System.Drawing.Point(83, 14);
            this.bOcrRead.Name = "bOcrRead";
            this.bOcrRead.Size = new System.Drawing.Size(74, 23);
            this.bOcrRead.TabIndex = 4;
            this.bOcrRead.Text = "Read";
            this.bOcrRead.UseVisualStyleBackColor = true;
            this.bOcrRead.Click += new System.EventHandler(this.bOcrRead_Click);
            // 
            // cbOcrWaferType
            // 
            this.cbOcrWaferType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbOcrWaferType.FormattingEnabled = true;
            this.cbOcrWaferType.Location = new System.Drawing.Point(275, 14);
            this.cbOcrWaferType.Name = "cbOcrWaferType";
            this.cbOcrWaferType.Size = new System.Drawing.Size(75, 21);
            this.cbOcrWaferType.TabIndex = 3;
            // 
            // cbOcrWaferSide
            // 
            this.cbOcrWaferSide.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbOcrWaferSide.FormattingEnabled = true;
            this.cbOcrWaferSide.Location = new System.Drawing.Point(163, 14);
            this.cbOcrWaferSide.Name = "cbOcrWaferSide";
            this.cbOcrWaferSide.Size = new System.Drawing.Size(106, 21);
            this.cbOcrWaferSide.TabIndex = 2;
            this.cbOcrWaferSide.SelectedIndexChanged += new System.EventHandler(this.cbOcrWaferSide_SelectedIndexChanged);
            // 
            // cbOcrFrontSideRecipes
            // 
            this.cbOcrFrontSideRecipes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbOcrFrontSideRecipes.FormattingEnabled = true;
            this.cbOcrFrontSideRecipes.Location = new System.Drawing.Point(163, 40);
            this.cbOcrFrontSideRecipes.Name = "cbOcrFrontSideRecipes";
            this.cbOcrFrontSideRecipes.Size = new System.Drawing.Size(106, 21);
            this.cbOcrFrontSideRecipes.TabIndex = 1;
            // 
            // bOcrGetRecipes
            // 
            this.bOcrGetRecipes.Location = new System.Drawing.Point(6, 14);
            this.bOcrGetRecipes.Name = "bOcrGetRecipes";
            this.bOcrGetRecipes.Size = new System.Drawing.Size(74, 23);
            this.bOcrGetRecipes.TabIndex = 0;
            this.bOcrGetRecipes.Text = "Get Recipes";
            this.bOcrGetRecipes.UseVisualStyleBackColor = true;
            this.bOcrGetRecipes.Click += new System.EventHandler(this.bOcrGetRecipes_Click);
            // 
            // bUnclampOnAligner
            // 
            this.bUnclampOnAligner.Location = new System.Drawing.Point(219, 19);
            this.bUnclampOnAligner.Name = "bUnclampOnAligner";
            this.bUnclampOnAligner.Size = new System.Drawing.Size(85, 23);
            this.bUnclampOnAligner.TabIndex = 6;
            this.bUnclampOnAligner.Text = "Chuck Off";
            this.bUnclampOnAligner.UseVisualStyleBackColor = true;
            this.bUnclampOnAligner.Click += new System.EventHandler(this.bChuckOffAligner_Click);
            // 
            // bClampOnAligner
            // 
            this.bClampOnAligner.Location = new System.Drawing.Point(128, 77);
            this.bClampOnAligner.Name = "bClampOnAligner";
            this.bClampOnAligner.Size = new System.Drawing.Size(85, 23);
            this.bClampOnAligner.TabIndex = 5;
            this.bClampOnAligner.Text = "Chuck On";
            this.bClampOnAligner.UseVisualStyleBackColor = true;
            this.bClampOnAligner.Click += new System.EventHandler(this.bChuckOnAligner_Click);
            // 
            // tbAlignerDeviceState
            // 
            this.tbAlignerDeviceState.BackColor = System.Drawing.SystemColors.Control;
            this.tbAlignerDeviceState.Location = new System.Drawing.Point(22, 19);
            this.tbAlignerDeviceState.Name = "tbAlignerDeviceState";
            this.tbAlignerDeviceState.ReadOnly = true;
            this.tbAlignerDeviceState.Size = new System.Drawing.Size(100, 20);
            this.tbAlignerDeviceState.TabIndex = 0;
            this.tbAlignerDeviceState.Text = "Device State";
            this.tbAlignerDeviceState.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbAlignerWaferPresence
            // 
            this.tbAlignerWaferPresence.BackColor = System.Drawing.SystemColors.Control;
            this.tbAlignerWaferPresence.Location = new System.Drawing.Point(22, 51);
            this.tbAlignerWaferPresence.Name = "tbAlignerWaferPresence";
            this.tbAlignerWaferPresence.ReadOnly = true;
            this.tbAlignerWaferPresence.Size = new System.Drawing.Size(100, 20);
            this.tbAlignerWaferPresence.TabIndex = 2;
            this.tbAlignerWaferPresence.Text = "Wafer Presence";
            this.tbAlignerWaferPresence.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // bAlignerInit
            // 
            this.bAlignerInit.Location = new System.Drawing.Point(128, 19);
            this.bAlignerInit.Name = "bAlignerInit";
            this.bAlignerInit.Size = new System.Drawing.Size(85, 23);
            this.bAlignerInit.TabIndex = 1;
            this.bAlignerInit.Text = "Initialization";
            this.bAlignerInit.UseVisualStyleBackColor = true;
            this.bAlignerInit.Click += new System.EventHandler(this.AlignerInit_Click);
            // 
            // bAlignerAlign
            // 
            this.bAlignerAlign.Location = new System.Drawing.Point(128, 48);
            this.bAlignerAlign.Name = "bAlignerAlign";
            this.bAlignerAlign.Size = new System.Drawing.Size(85, 23);
            this.bAlignerAlign.TabIndex = 3;
            this.bAlignerAlign.Text = "Align";
            this.bAlignerAlign.UseVisualStyleBackColor = true;
            this.bAlignerAlign.Click += new System.EventHandler(this.AlignerAlign_Click);
            // 
            // tbAlignerAlignAngle
            // 
            this.tbAlignerAlignAngle.Location = new System.Drawing.Point(222, 51);
            this.tbAlignerAlignAngle.Name = "tbAlignerAlignAngle";
            this.tbAlignerAlignAngle.Size = new System.Drawing.Size(69, 20);
            this.tbAlignerAlignAngle.TabIndex = 4;
            this.tbAlignerAlignAngle.Text = "Angle (°)";
            this.tbAlignerAlignAngle.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // bClear
            // 
            this.bClear.BackColor = System.Drawing.Color.Transparent;
            this.bClear.Location = new System.Drawing.Point(999, 553);
            this.bClear.Name = "bClear";
            this.bClear.Size = new System.Drawing.Size(204, 23);
            this.bClear.TabIndex = 2;
            this.bClear.Text = "Clear Logs";
            this.bClear.UseVisualStyleBackColor = false;
            this.bClear.Click += new System.EventHandler(this.Clear_Click);
            // 
            // tLoadPorts
            // 
            this.tLoadPorts.Controls.Add(this.bSetCarrierType);
            this.tLoadPorts.Controls.Add(this.lCarrierType);
            this.tLoadPorts.Controls.Add(this.nudCarrierType);
            this.tLoadPorts.Controls.Add(this.bLPE84Abort);
            this.tLoadPorts.Controls.Add(this.bLPE84Reset);
            this.tLoadPorts.Controls.Add(this.bSetE84Timeout);
            this.tLoadPorts.Controls.Add(this.lTP5);
            this.tLoadPorts.Controls.Add(this.nudTP5);
            this.tLoadPorts.Controls.Add(this.lTP4);
            this.tLoadPorts.Controls.Add(this.nudTP4);
            this.tLoadPorts.Controls.Add(this.lTP3);
            this.tLoadPorts.Controls.Add(this.nudTP3);
            this.tLoadPorts.Controls.Add(this.lTP2);
            this.tLoadPorts.Controls.Add(this.nudTP2);
            this.tLoadPorts.Controls.Add(this.lTP1);
            this.tLoadPorts.Controls.Add(this.nudTP1);
            this.tLoadPorts.Controls.Add(this.bLPE84AutoHandling);
            this.tLoadPorts.Controls.Add(this.bLPE84ManualHandling);
            this.tLoadPorts.Controls.Add(this.bLPAuto);
            this.tLoadPorts.Controls.Add(this.bLPManual);
            this.tLoadPorts.Controls.Add(this.tbLP3AccessMode);
            this.tLoadPorts.Controls.Add(this.tbLP3ServiceState);
            this.tLoadPorts.Controls.Add(this.tbLP2AccessMode);
            this.tLoadPorts.Controls.Add(this.tbLP2ServiceState);
            this.tLoadPorts.Controls.Add(this.tbLP1AccessMode);
            this.tLoadPorts.Controls.Add(this.tbLP1ServiceState);
            this.tLoadPorts.Controls.Add(this.bLPInService);
            this.tLoadPorts.Controls.Add(this.bLPNotInService);
            this.tLoadPorts.Controls.Add(this.cbLoadPorts);
            this.tLoadPorts.Location = new System.Drawing.Point(4, 22);
            this.tLoadPorts.Name = "tLoadPorts";
            this.tLoadPorts.Padding = new System.Windows.Forms.Padding(3);
            this.tLoadPorts.Size = new System.Drawing.Size(1208, 595);
            this.tLoadPorts.TabIndex = 5;
            this.tLoadPorts.Text = "Load Ports";
            this.tLoadPorts.UseVisualStyleBackColor = true;
            // 
            // bSetCarrierType
            // 
            this.bSetCarrierType.Location = new System.Drawing.Point(908, 153);
            this.bSetCarrierType.Name = "bSetCarrierType";
            this.bSetCarrierType.Size = new System.Drawing.Size(128, 23);
            this.bSetCarrierType.TabIndex = 29;
            this.bSetCarrierType.Text = "Set Carrier Type";
            this.bSetCarrierType.UseVisualStyleBackColor = true;
            this.bSetCarrierType.Click += new System.EventHandler(this.bSetCarrierType_Click);
            // 
            // nudCarrierType
            // 
            this.nudCarrierType.Location = new System.Drawing.Point(973, 127);
            this.nudCarrierType.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.nudCarrierType.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudCarrierType.Name = "nudCarrierType";
            this.nudCarrierType.Size = new System.Drawing.Size(63, 20);
            this.nudCarrierType.TabIndex = 28;
            this.nudCarrierType.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // bLPE84Abort
            // 
            this.bLPE84Abort.Enabled = false;
            this.bLPE84Abort.Location = new System.Drawing.Point(566, 150);
            this.bLPE84Abort.Name = "bLPE84Abort";
            this.bLPE84Abort.Size = new System.Drawing.Size(128, 23);
            this.bLPE84Abort.TabIndex = 26;
            this.bLPE84Abort.Text = "E84 Abort";
            this.bLPE84Abort.UseVisualStyleBackColor = true;
            this.bLPE84Abort.Click += new System.EventHandler(this.bLPE84Abort_Click);
            // 
            // bLPE84Reset
            // 
            this.bLPE84Reset.Enabled = false;
            this.bLPE84Reset.Location = new System.Drawing.Point(432, 150);
            this.bLPE84Reset.Name = "bLPE84Reset";
            this.bLPE84Reset.Size = new System.Drawing.Size(128, 23);
            this.bLPE84Reset.TabIndex = 25;
            this.bLPE84Reset.Text = "E84 Reset";
            this.bLPE84Reset.UseVisualStyleBackColor = true;
            this.bLPE84Reset.Click += new System.EventHandler(this.bLPE84Reset_Click);
            // 
            // bSetE84Timeout
            // 
            this.bSetE84Timeout.Location = new System.Drawing.Point(730, 153);
            this.bSetE84Timeout.Name = "bSetE84Timeout";
            this.bSetE84Timeout.Size = new System.Drawing.Size(128, 23);
            this.bSetE84Timeout.TabIndex = 24;
            this.bSetE84Timeout.Text = "Set E84 Timeouts";
            this.bSetE84Timeout.UseVisualStyleBackColor = true;
            this.bSetE84Timeout.Click += new System.EventHandler(this.bSetE84Timeout_Click);
            // 
            // nudTP5
            // 
            this.nudTP5.Location = new System.Drawing.Point(786, 127);
            this.nudTP5.Name = "nudTP5";
            this.nudTP5.Size = new System.Drawing.Size(63, 20);
            this.nudTP5.TabIndex = 23;
            this.nudTP5.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // nudTP4
            // 
            this.nudTP4.Location = new System.Drawing.Point(786, 101);
            this.nudTP4.Name = "nudTP4";
            this.nudTP4.Size = new System.Drawing.Size(63, 20);
            this.nudTP4.TabIndex = 21;
            this.nudTP4.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // nudTP3
            // 
            this.nudTP3.Location = new System.Drawing.Point(786, 75);
            this.nudTP3.Name = "nudTP3";
            this.nudTP3.Size = new System.Drawing.Size(63, 20);
            this.nudTP3.TabIndex = 19;
            this.nudTP3.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            // 
            // nudTP2
            // 
            this.nudTP2.Location = new System.Drawing.Point(786, 49);
            this.nudTP2.Name = "nudTP2";
            this.nudTP2.Size = new System.Drawing.Size(63, 20);
            this.nudTP2.TabIndex = 17;
            this.nudTP2.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // nudTP1
            // 
            this.nudTP1.Location = new System.Drawing.Point(786, 23);
            this.nudTP1.Name = "nudTP1";
            this.nudTP1.Size = new System.Drawing.Size(63, 20);
            this.nudTP1.TabIndex = 15;
            this.nudTP1.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // bLPE84AutoHandling
            // 
            this.bLPE84AutoHandling.Enabled = false;
            this.bLPE84AutoHandling.Location = new System.Drawing.Point(566, 121);
            this.bLPE84AutoHandling.Name = "bLPE84AutoHandling";
            this.bLPE84AutoHandling.Size = new System.Drawing.Size(128, 23);
            this.bLPE84AutoHandling.TabIndex = 12;
            this.bLPE84AutoHandling.Text = "E84 Auto Handling";
            this.bLPE84AutoHandling.UseVisualStyleBackColor = true;
            this.bLPE84AutoHandling.Click += new System.EventHandler(this.bLPE84AutoHandling_Click);
            // 
            // bLPE84ManualHandling
            // 
            this.bLPE84ManualHandling.Enabled = false;
            this.bLPE84ManualHandling.Location = new System.Drawing.Point(432, 121);
            this.bLPE84ManualHandling.Name = "bLPE84ManualHandling";
            this.bLPE84ManualHandling.Size = new System.Drawing.Size(128, 23);
            this.bLPE84ManualHandling.TabIndex = 11;
            this.bLPE84ManualHandling.Text = "E84 Manual Handling";
            this.bLPE84ManualHandling.UseVisualStyleBackColor = true;
            this.bLPE84ManualHandling.Click += new System.EventHandler(this.bLPE84ManualHandling_Click);
            // 
            // bLPAuto
            // 
            this.bLPAuto.Enabled = false;
            this.bLPAuto.Location = new System.Drawing.Point(314, 121);
            this.bLPAuto.Name = "bLPAuto";
            this.bLPAuto.Size = new System.Drawing.Size(90, 23);
            this.bLPAuto.TabIndex = 8;
            this.bLPAuto.Text = "Auto";
            this.bLPAuto.UseVisualStyleBackColor = true;
            this.bLPAuto.Click += new System.EventHandler(this.LPAuto_Click);
            // 
            // bLPManual
            // 
            this.bLPManual.Enabled = false;
            this.bLPManual.Location = new System.Drawing.Point(218, 121);
            this.bLPManual.Name = "bLPManual";
            this.bLPManual.Size = new System.Drawing.Size(90, 23);
            this.bLPManual.TabIndex = 7;
            this.bLPManual.Text = "Manual";
            this.bLPManual.UseVisualStyleBackColor = true;
            this.bLPManual.Click += new System.EventHandler(this.LPManual_Click);
            // 
            // tbLP3AccessMode
            // 
            this.tbLP3AccessMode.BackColor = System.Drawing.SystemColors.Control;
            this.tbLP3AccessMode.Enabled = false;
            this.tbLP3AccessMode.Location = new System.Drawing.Point(8, 190);
            this.tbLP3AccessMode.Name = "tbLP3AccessMode";
            this.tbLP3AccessMode.ReadOnly = true;
            this.tbLP3AccessMode.Size = new System.Drawing.Size(140, 20);
            this.tbLP3AccessMode.TabIndex = 4;
            this.tbLP3AccessMode.Text = "LP3 Access Mode";
            this.tbLP3AccessMode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbLP3ServiceState
            // 
            this.tbLP3ServiceState.BackColor = System.Drawing.SystemColors.Control;
            this.tbLP3ServiceState.Enabled = false;
            this.tbLP3ServiceState.Location = new System.Drawing.Point(8, 216);
            this.tbLP3ServiceState.Name = "tbLP3ServiceState";
            this.tbLP3ServiceState.ReadOnly = true;
            this.tbLP3ServiceState.Size = new System.Drawing.Size(140, 20);
            this.tbLP3ServiceState.TabIndex = 5;
            this.tbLP3ServiceState.Text = "LP3 Enable State";
            this.tbLP3ServiceState.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbLP2AccessMode
            // 
            this.tbLP2AccessMode.BackColor = System.Drawing.SystemColors.Control;
            this.tbLP2AccessMode.Enabled = false;
            this.tbLP2AccessMode.Location = new System.Drawing.Point(8, 107);
            this.tbLP2AccessMode.Name = "tbLP2AccessMode";
            this.tbLP2AccessMode.ReadOnly = true;
            this.tbLP2AccessMode.Size = new System.Drawing.Size(140, 20);
            this.tbLP2AccessMode.TabIndex = 2;
            this.tbLP2AccessMode.Text = "LP2 Access Mode";
            this.tbLP2AccessMode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbLP2ServiceState
            // 
            this.tbLP2ServiceState.BackColor = System.Drawing.SystemColors.Control;
            this.tbLP2ServiceState.Enabled = false;
            this.tbLP2ServiceState.Location = new System.Drawing.Point(8, 133);
            this.tbLP2ServiceState.Name = "tbLP2ServiceState";
            this.tbLP2ServiceState.ReadOnly = true;
            this.tbLP2ServiceState.Size = new System.Drawing.Size(140, 20);
            this.tbLP2ServiceState.TabIndex = 3;
            this.tbLP2ServiceState.Text = "LP2 Enable State";
            this.tbLP2ServiceState.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbLP1AccessMode
            // 
            this.tbLP1AccessMode.BackColor = System.Drawing.SystemColors.Control;
            this.tbLP1AccessMode.Enabled = false;
            this.tbLP1AccessMode.Location = new System.Drawing.Point(8, 26);
            this.tbLP1AccessMode.Name = "tbLP1AccessMode";
            this.tbLP1AccessMode.ReadOnly = true;
            this.tbLP1AccessMode.Size = new System.Drawing.Size(140, 20);
            this.tbLP1AccessMode.TabIndex = 0;
            this.tbLP1AccessMode.Text = "LP1 Access Mode";
            this.tbLP1AccessMode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbLP1ServiceState
            // 
            this.tbLP1ServiceState.BackColor = System.Drawing.SystemColors.Control;
            this.tbLP1ServiceState.Enabled = false;
            this.tbLP1ServiceState.Location = new System.Drawing.Point(8, 52);
            this.tbLP1ServiceState.Name = "tbLP1ServiceState";
            this.tbLP1ServiceState.ReadOnly = true;
            this.tbLP1ServiceState.Size = new System.Drawing.Size(140, 20);
            this.tbLP1ServiceState.TabIndex = 1;
            this.tbLP1ServiceState.Text = "LP1 Enable State";
            this.tbLP1ServiceState.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // bLPInService
            // 
            this.bLPInService.Enabled = false;
            this.bLPInService.Location = new System.Drawing.Point(218, 150);
            this.bLPInService.Name = "bLPInService";
            this.bLPInService.Size = new System.Drawing.Size(90, 23);
            this.bLPInService.TabIndex = 9;
            this.bLPInService.Text = "In Service";
            this.bLPInService.UseVisualStyleBackColor = true;
            this.bLPInService.Click += new System.EventHandler(this.LPInService_Click);
            // 
            // bLPNotInService
            // 
            this.bLPNotInService.Enabled = false;
            this.bLPNotInService.Location = new System.Drawing.Point(314, 150);
            this.bLPNotInService.Name = "bLPNotInService";
            this.bLPNotInService.Size = new System.Drawing.Size(90, 23);
            this.bLPNotInService.TabIndex = 10;
            this.bLPNotInService.Text = "OutOfService";
            this.bLPNotInService.UseVisualStyleBackColor = true;
            this.bLPNotInService.Click += new System.EventHandler(this.LPOutOfService_Click);
            // 
            // cbLoadPorts
            // 
            this.cbLoadPorts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLoadPorts.FormattingEnabled = true;
            this.cbLoadPorts.Location = new System.Drawing.Point(218, 94);
            this.cbLoadPorts.Name = "cbLoadPorts";
            this.cbLoadPorts.Size = new System.Drawing.Size(186, 21);
            this.cbLoadPorts.TabIndex = 6;
            // 
            // tIO
            // 
            this.tIO.Controls.Add(this.gbSignals);
            this.tIO.Controls.Add(this.gbLPIndicators);
            this.tIO.Controls.Add(this.gbTower);
            this.tIO.Controls.Add(this.gbBuzzer);
            this.tIO.Location = new System.Drawing.Point(4, 22);
            this.tIO.Name = "tIO";
            this.tIO.Padding = new System.Windows.Forms.Padding(3);
            this.tIO.Size = new System.Drawing.Size(1208, 595);
            this.tIO.TabIndex = 4;
            this.tIO.Text = "I/O";
            this.tIO.UseVisualStyleBackColor = true;
            // 
            // gbSignals
            // 
            this.gbSignals.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSignals.Controls.Add(this.pgSignals);
            this.gbSignals.Location = new System.Drawing.Point(321, 6);
            this.gbSignals.Name = "gbSignals";
            this.gbSignals.Size = new System.Drawing.Size(879, 589);
            this.gbSignals.TabIndex = 3;
            this.gbSignals.TabStop = false;
            this.gbSignals.Text = "Signals";
            // 
            // pgSignals
            // 
            this.pgSignals.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.pgSignals.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgSignals.LineColor = System.Drawing.SystemColors.ControlDark;
            this.pgSignals.Location = new System.Drawing.Point(3, 16);
            this.pgSignals.Name = "pgSignals";
            this.pgSignals.Size = new System.Drawing.Size(873, 570);
            this.pgSignals.TabIndex = 0;
            // 
            // gbLPIndicators
            // 
            this.gbLPIndicators.Controls.Add(this.tbAlarmIndicator);
            this.gbLPIndicators.Controls.Add(this.bAlarmLightBlink);
            this.gbLPIndicators.Controls.Add(this.bAlarmLightOff);
            this.gbLPIndicators.Controls.Add(this.bAlarmLightOn);
            this.gbLPIndicators.Controls.Add(this.tbReserveIndicator);
            this.gbLPIndicators.Controls.Add(this.bReserveLightBlink);
            this.gbLPIndicators.Controls.Add(this.bReserveLightOff);
            this.gbLPIndicators.Controls.Add(this.bReserveLightOn);
            this.gbLPIndicators.Controls.Add(this.tbAutoIndicator);
            this.gbLPIndicators.Controls.Add(this.bAutoLightBlink);
            this.gbLPIndicators.Controls.Add(this.bAutoLightOff);
            this.gbLPIndicators.Controls.Add(this.bAutoLightOn);
            this.gbLPIndicators.Controls.Add(this.cbIOLoadPorts);
            this.gbLPIndicators.Controls.Add(this.tbAccessIndicator);
            this.gbLPIndicators.Controls.Add(this.bOpAccessLightBlink);
            this.gbLPIndicators.Controls.Add(this.bOpAccessLightOff);
            this.gbLPIndicators.Controls.Add(this.bOpAccessLightOn);
            this.gbLPIndicators.Controls.Add(this.tbManualIndicator);
            this.gbLPIndicators.Controls.Add(this.bManualLightBlink);
            this.gbLPIndicators.Controls.Add(this.bManualLightOff);
            this.gbLPIndicators.Controls.Add(this.bManualLightOn);
            this.gbLPIndicators.Controls.Add(this.tbUnloadIndicator);
            this.gbLPIndicators.Controls.Add(this.bUnloadLightBlink);
            this.gbLPIndicators.Controls.Add(this.bUnloadLightOff);
            this.gbLPIndicators.Controls.Add(this.bUnloadLightOn);
            this.gbLPIndicators.Controls.Add(this.tbLoadIndicator);
            this.gbLPIndicators.Controls.Add(this.bLoadLightBlink);
            this.gbLPIndicators.Controls.Add(this.bLoadLightOff);
            this.gbLPIndicators.Controls.Add(this.bLoadLightOn);
            this.gbLPIndicators.Location = new System.Drawing.Point(8, 339);
            this.gbLPIndicators.Name = "gbLPIndicators";
            this.gbLPIndicators.Size = new System.Drawing.Size(307, 425);
            this.gbLPIndicators.TabIndex = 2;
            this.gbLPIndicators.TabStop = false;
            this.gbLPIndicators.Text = "Load Port Indicators";
            // 
            // tbAlarmIndicator
            // 
            this.tbAlarmIndicator.BackColor = System.Drawing.Color.Red;
            this.tbAlarmIndicator.Location = new System.Drawing.Point(6, 372);
            this.tbAlarmIndicator.Name = "tbAlarmIndicator";
            this.tbAlarmIndicator.Size = new System.Drawing.Size(295, 20);
            this.tbAlarmIndicator.TabIndex = 25;
            this.tbAlarmIndicator.Text = "Alarm";
            this.tbAlarmIndicator.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // bAlarmLightBlink
            // 
            this.bAlarmLightBlink.Enabled = false;
            this.bAlarmLightBlink.Location = new System.Drawing.Point(216, 398);
            this.bAlarmLightBlink.Name = "bAlarmLightBlink";
            this.bAlarmLightBlink.Size = new System.Drawing.Size(85, 23);
            this.bAlarmLightBlink.TabIndex = 28;
            this.bAlarmLightBlink.Text = "Blink";
            this.bAlarmLightBlink.UseVisualStyleBackColor = true;
            // 
            // bAlarmLightOff
            // 
            this.bAlarmLightOff.Enabled = false;
            this.bAlarmLightOff.Location = new System.Drawing.Point(109, 398);
            this.bAlarmLightOff.Name = "bAlarmLightOff";
            this.bAlarmLightOff.Size = new System.Drawing.Size(85, 23);
            this.bAlarmLightOff.TabIndex = 27;
            this.bAlarmLightOff.Text = "Off";
            this.bAlarmLightOff.UseVisualStyleBackColor = true;
            this.bAlarmLightOff.Click += new System.EventHandler(this.AlarmLight_Click);
            // 
            // bAlarmLightOn
            // 
            this.bAlarmLightOn.Enabled = false;
            this.bAlarmLightOn.Location = new System.Drawing.Point(6, 398);
            this.bAlarmLightOn.Name = "bAlarmLightOn";
            this.bAlarmLightOn.Size = new System.Drawing.Size(85, 23);
            this.bAlarmLightOn.TabIndex = 26;
            this.bAlarmLightOn.Text = "On";
            this.bAlarmLightOn.UseVisualStyleBackColor = true;
            this.bAlarmLightOn.Click += new System.EventHandler(this.AlarmLight_Click);
            // 
            // tbReserveIndicator
            // 
            this.tbReserveIndicator.BackColor = System.Drawing.Color.Orange;
            this.tbReserveIndicator.Location = new System.Drawing.Point(6, 317);
            this.tbReserveIndicator.Name = "tbReserveIndicator";
            this.tbReserveIndicator.Size = new System.Drawing.Size(295, 20);
            this.tbReserveIndicator.TabIndex = 21;
            this.tbReserveIndicator.Text = "Reserve";
            this.tbReserveIndicator.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // bReserveLightBlink
            // 
            this.bReserveLightBlink.Enabled = false;
            this.bReserveLightBlink.Location = new System.Drawing.Point(216, 343);
            this.bReserveLightBlink.Name = "bReserveLightBlink";
            this.bReserveLightBlink.Size = new System.Drawing.Size(85, 23);
            this.bReserveLightBlink.TabIndex = 24;
            this.bReserveLightBlink.Text = "Blink";
            this.bReserveLightBlink.UseVisualStyleBackColor = true;
            // 
            // bReserveLightOff
            // 
            this.bReserveLightOff.Enabled = false;
            this.bReserveLightOff.Location = new System.Drawing.Point(109, 343);
            this.bReserveLightOff.Name = "bReserveLightOff";
            this.bReserveLightOff.Size = new System.Drawing.Size(85, 23);
            this.bReserveLightOff.TabIndex = 23;
            this.bReserveLightOff.Text = "Off";
            this.bReserveLightOff.UseVisualStyleBackColor = true;
            this.bReserveLightOff.Click += new System.EventHandler(this.ReserveLight_Click);
            // 
            // bReserveLightOn
            // 
            this.bReserveLightOn.Enabled = false;
            this.bReserveLightOn.Location = new System.Drawing.Point(6, 343);
            this.bReserveLightOn.Name = "bReserveLightOn";
            this.bReserveLightOn.Size = new System.Drawing.Size(85, 23);
            this.bReserveLightOn.TabIndex = 22;
            this.bReserveLightOn.Text = "On";
            this.bReserveLightOn.UseVisualStyleBackColor = true;
            this.bReserveLightOn.Click += new System.EventHandler(this.OpAccessLight_Click);
            // 
            // tbAutoIndicator
            // 
            this.tbAutoIndicator.BackColor = System.Drawing.Color.Blue;
            this.tbAutoIndicator.Location = new System.Drawing.Point(6, 261);
            this.tbAutoIndicator.Name = "tbAutoIndicator";
            this.tbAutoIndicator.Size = new System.Drawing.Size(295, 20);
            this.tbAutoIndicator.TabIndex = 17;
            this.tbAutoIndicator.Text = "Auto";
            this.tbAutoIndicator.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // bAutoLightBlink
            // 
            this.bAutoLightBlink.Enabled = false;
            this.bAutoLightBlink.Location = new System.Drawing.Point(216, 287);
            this.bAutoLightBlink.Name = "bAutoLightBlink";
            this.bAutoLightBlink.Size = new System.Drawing.Size(85, 23);
            this.bAutoLightBlink.TabIndex = 20;
            this.bAutoLightBlink.Text = "Blink";
            this.bAutoLightBlink.UseVisualStyleBackColor = true;
            // 
            // bAutoLightOff
            // 
            this.bAutoLightOff.Enabled = false;
            this.bAutoLightOff.Location = new System.Drawing.Point(109, 287);
            this.bAutoLightOff.Name = "bAutoLightOff";
            this.bAutoLightOff.Size = new System.Drawing.Size(85, 23);
            this.bAutoLightOff.TabIndex = 19;
            this.bAutoLightOff.Text = "Off";
            this.bAutoLightOff.UseVisualStyleBackColor = true;
            this.bAutoLightOff.Click += new System.EventHandler(this.AutoLight_Click);
            // 
            // bAutoLightOn
            // 
            this.bAutoLightOn.Enabled = false;
            this.bAutoLightOn.Location = new System.Drawing.Point(6, 287);
            this.bAutoLightOn.Name = "bAutoLightOn";
            this.bAutoLightOn.Size = new System.Drawing.Size(85, 23);
            this.bAutoLightOn.TabIndex = 18;
            this.bAutoLightOn.Text = "On";
            this.bAutoLightOn.UseVisualStyleBackColor = true;
            this.bAutoLightOn.Click += new System.EventHandler(this.AutoLight_Click);
            // 
            // cbIOLoadPorts
            // 
            this.cbIOLoadPorts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbIOLoadPorts.FormattingEnabled = true;
            this.cbIOLoadPorts.Location = new System.Drawing.Point(6, 14);
            this.cbIOLoadPorts.Name = "cbIOLoadPorts";
            this.cbIOLoadPorts.Size = new System.Drawing.Size(295, 21);
            this.cbIOLoadPorts.TabIndex = 0;
            // 
            // tbAccessIndicator
            // 
            this.tbAccessIndicator.BackColor = System.Drawing.Color.Orange;
            this.tbAccessIndicator.Location = new System.Drawing.Point(6, 207);
            this.tbAccessIndicator.Name = "tbAccessIndicator";
            this.tbAccessIndicator.Size = new System.Drawing.Size(295, 20);
            this.tbAccessIndicator.TabIndex = 13;
            this.tbAccessIndicator.Text = "Operator Access";
            this.tbAccessIndicator.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // bOpAccessLightBlink
            // 
            this.bOpAccessLightBlink.Enabled = false;
            this.bOpAccessLightBlink.Location = new System.Drawing.Point(216, 233);
            this.bOpAccessLightBlink.Name = "bOpAccessLightBlink";
            this.bOpAccessLightBlink.Size = new System.Drawing.Size(85, 23);
            this.bOpAccessLightBlink.TabIndex = 16;
            this.bOpAccessLightBlink.Text = "Blink";
            this.bOpAccessLightBlink.UseVisualStyleBackColor = true;
            // 
            // bOpAccessLightOff
            // 
            this.bOpAccessLightOff.Enabled = false;
            this.bOpAccessLightOff.Location = new System.Drawing.Point(109, 233);
            this.bOpAccessLightOff.Name = "bOpAccessLightOff";
            this.bOpAccessLightOff.Size = new System.Drawing.Size(85, 23);
            this.bOpAccessLightOff.TabIndex = 15;
            this.bOpAccessLightOff.Text = "Off";
            this.bOpAccessLightOff.UseVisualStyleBackColor = true;
            this.bOpAccessLightOff.Click += new System.EventHandler(this.OpAccessLight_Click);
            // 
            // bOpAccessLightOn
            // 
            this.bOpAccessLightOn.Enabled = false;
            this.bOpAccessLightOn.Location = new System.Drawing.Point(6, 233);
            this.bOpAccessLightOn.Name = "bOpAccessLightOn";
            this.bOpAccessLightOn.Size = new System.Drawing.Size(85, 23);
            this.bOpAccessLightOn.TabIndex = 14;
            this.bOpAccessLightOn.Text = "On";
            this.bOpAccessLightOn.UseVisualStyleBackColor = true;
            this.bOpAccessLightOn.Click += new System.EventHandler(this.OpAccessLight_Click);
            // 
            // tbManualIndicator
            // 
            this.tbManualIndicator.BackColor = System.Drawing.Color.Orange;
            this.tbManualIndicator.Location = new System.Drawing.Point(6, 152);
            this.tbManualIndicator.Name = "tbManualIndicator";
            this.tbManualIndicator.Size = new System.Drawing.Size(295, 20);
            this.tbManualIndicator.TabIndex = 9;
            this.tbManualIndicator.Text = "Manual";
            this.tbManualIndicator.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // bManualLightBlink
            // 
            this.bManualLightBlink.Enabled = false;
            this.bManualLightBlink.Location = new System.Drawing.Point(216, 178);
            this.bManualLightBlink.Name = "bManualLightBlink";
            this.bManualLightBlink.Size = new System.Drawing.Size(85, 23);
            this.bManualLightBlink.TabIndex = 12;
            this.bManualLightBlink.Text = "Blink";
            this.bManualLightBlink.UseVisualStyleBackColor = true;
            // 
            // bManualLightOff
            // 
            this.bManualLightOff.Location = new System.Drawing.Point(109, 178);
            this.bManualLightOff.Name = "bManualLightOff";
            this.bManualLightOff.Size = new System.Drawing.Size(85, 23);
            this.bManualLightOff.TabIndex = 11;
            this.bManualLightOff.Text = "Off";
            this.bManualLightOff.UseVisualStyleBackColor = true;
            this.bManualLightOff.Click += new System.EventHandler(this.ManualLight_Click);
            // 
            // bManualLightOn
            // 
            this.bManualLightOn.Location = new System.Drawing.Point(6, 178);
            this.bManualLightOn.Name = "bManualLightOn";
            this.bManualLightOn.Size = new System.Drawing.Size(85, 23);
            this.bManualLightOn.TabIndex = 10;
            this.bManualLightOn.Text = "On";
            this.bManualLightOn.UseVisualStyleBackColor = true;
            this.bManualLightOn.Click += new System.EventHandler(this.ManualLight_Click);
            // 
            // tbUnloadIndicator
            // 
            this.tbUnloadIndicator.BackColor = System.Drawing.Color.Blue;
            this.tbUnloadIndicator.ForeColor = System.Drawing.Color.Black;
            this.tbUnloadIndicator.Location = new System.Drawing.Point(6, 97);
            this.tbUnloadIndicator.Name = "tbUnloadIndicator";
            this.tbUnloadIndicator.Size = new System.Drawing.Size(295, 20);
            this.tbUnloadIndicator.TabIndex = 5;
            this.tbUnloadIndicator.Text = "Unload";
            this.tbUnloadIndicator.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // bUnloadLightBlink
            // 
            this.bUnloadLightBlink.Enabled = false;
            this.bUnloadLightBlink.Location = new System.Drawing.Point(216, 123);
            this.bUnloadLightBlink.Name = "bUnloadLightBlink";
            this.bUnloadLightBlink.Size = new System.Drawing.Size(85, 23);
            this.bUnloadLightBlink.TabIndex = 8;
            this.bUnloadLightBlink.Text = "Blink";
            this.bUnloadLightBlink.UseVisualStyleBackColor = true;
            // 
            // bUnloadLightOff
            // 
            this.bUnloadLightOff.Location = new System.Drawing.Point(109, 123);
            this.bUnloadLightOff.Name = "bUnloadLightOff";
            this.bUnloadLightOff.Size = new System.Drawing.Size(85, 23);
            this.bUnloadLightOff.TabIndex = 7;
            this.bUnloadLightOff.Text = "Off";
            this.bUnloadLightOff.UseVisualStyleBackColor = true;
            this.bUnloadLightOff.Click += new System.EventHandler(this.UnloadLight_Click);
            // 
            // bUnloadLightOn
            // 
            this.bUnloadLightOn.Location = new System.Drawing.Point(6, 123);
            this.bUnloadLightOn.Name = "bUnloadLightOn";
            this.bUnloadLightOn.Size = new System.Drawing.Size(85, 23);
            this.bUnloadLightOn.TabIndex = 6;
            this.bUnloadLightOn.Text = "On";
            this.bUnloadLightOn.UseVisualStyleBackColor = true;
            this.bUnloadLightOn.Click += new System.EventHandler(this.UnloadLight_Click);
            // 
            // tbLoadIndicator
            // 
            this.tbLoadIndicator.BackColor = System.Drawing.Color.Lime;
            this.tbLoadIndicator.Location = new System.Drawing.Point(6, 42);
            this.tbLoadIndicator.Name = "tbLoadIndicator";
            this.tbLoadIndicator.Size = new System.Drawing.Size(295, 20);
            this.tbLoadIndicator.TabIndex = 1;
            this.tbLoadIndicator.Text = "Load";
            this.tbLoadIndicator.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // bLoadLightBlink
            // 
            this.bLoadLightBlink.Enabled = false;
            this.bLoadLightBlink.Location = new System.Drawing.Point(216, 68);
            this.bLoadLightBlink.Name = "bLoadLightBlink";
            this.bLoadLightBlink.Size = new System.Drawing.Size(85, 23);
            this.bLoadLightBlink.TabIndex = 4;
            this.bLoadLightBlink.Text = "Blink";
            this.bLoadLightBlink.UseVisualStyleBackColor = true;
            // 
            // bLoadLightOff
            // 
            this.bLoadLightOff.Location = new System.Drawing.Point(109, 68);
            this.bLoadLightOff.Name = "bLoadLightOff";
            this.bLoadLightOff.Size = new System.Drawing.Size(85, 23);
            this.bLoadLightOff.TabIndex = 3;
            this.bLoadLightOff.Text = "Off";
            this.bLoadLightOff.UseVisualStyleBackColor = true;
            this.bLoadLightOff.Click += new System.EventHandler(this.LoadLight_Click);
            // 
            // bLoadLightOn
            // 
            this.bLoadLightOn.Location = new System.Drawing.Point(6, 68);
            this.bLoadLightOn.Name = "bLoadLightOn";
            this.bLoadLightOn.Size = new System.Drawing.Size(85, 23);
            this.bLoadLightOn.TabIndex = 2;
            this.bLoadLightOn.Text = "On";
            this.bLoadLightOn.UseVisualStyleBackColor = true;
            this.bLoadLightOn.Click += new System.EventHandler(this.LoadLight_Click);
            // 
            // gbTower
            // 
            this.gbTower.Controls.Add(this.tbRedLightStatus);
            this.gbTower.Controls.Add(this.bRedLightBlink);
            this.gbTower.Controls.Add(this.bRedLightOff);
            this.gbTower.Controls.Add(this.bRedLightOn);
            this.gbTower.Controls.Add(this.tbOrangeLightStatus);
            this.gbTower.Controls.Add(this.bOrangeLightBlink);
            this.gbTower.Controls.Add(this.bOrangeLightOff);
            this.gbTower.Controls.Add(this.bOrangeLightOn);
            this.gbTower.Controls.Add(this.tbBlueLightStatus);
            this.gbTower.Controls.Add(this.bBlueLightBlink);
            this.gbTower.Controls.Add(this.bBlueLightOff);
            this.gbTower.Controls.Add(this.bBlueLightOn);
            this.gbTower.Controls.Add(this.tbGreenLightStatus);
            this.gbTower.Controls.Add(this.bGreenLightBlink);
            this.gbTower.Controls.Add(this.bGreenLightOff);
            this.gbTower.Controls.Add(this.bGreenLightOn);
            this.gbTower.Location = new System.Drawing.Point(8, 90);
            this.gbTower.Name = "gbTower";
            this.gbTower.Size = new System.Drawing.Size(307, 243);
            this.gbTower.TabIndex = 1;
            this.gbTower.TabStop = false;
            this.gbTower.Text = "Tower";
            // 
            // tbRedLightStatus
            // 
            this.tbRedLightStatus.BackColor = System.Drawing.Color.Red;
            this.tbRedLightStatus.Location = new System.Drawing.Point(6, 183);
            this.tbRedLightStatus.Name = "tbRedLightStatus";
            this.tbRedLightStatus.Size = new System.Drawing.Size(295, 20);
            this.tbRedLightStatus.TabIndex = 12;
            this.tbRedLightStatus.Text = "Red";
            this.tbRedLightStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // bRedLightBlink
            // 
            this.bRedLightBlink.Location = new System.Drawing.Point(216, 209);
            this.bRedLightBlink.Name = "bRedLightBlink";
            this.bRedLightBlink.Size = new System.Drawing.Size(85, 23);
            this.bRedLightBlink.TabIndex = 15;
            this.bRedLightBlink.Text = "Blink";
            this.bRedLightBlink.UseVisualStyleBackColor = true;
            this.bRedLightBlink.Click += new System.EventHandler(this.Light_Click);
            // 
            // bRedLightOff
            // 
            this.bRedLightOff.Location = new System.Drawing.Point(109, 209);
            this.bRedLightOff.Name = "bRedLightOff";
            this.bRedLightOff.Size = new System.Drawing.Size(85, 23);
            this.bRedLightOff.TabIndex = 14;
            this.bRedLightOff.Text = "Off";
            this.bRedLightOff.UseVisualStyleBackColor = true;
            this.bRedLightOff.Click += new System.EventHandler(this.Light_Click);
            // 
            // bRedLightOn
            // 
            this.bRedLightOn.Location = new System.Drawing.Point(6, 209);
            this.bRedLightOn.Name = "bRedLightOn";
            this.bRedLightOn.Size = new System.Drawing.Size(85, 23);
            this.bRedLightOn.TabIndex = 13;
            this.bRedLightOn.Text = "On";
            this.bRedLightOn.UseVisualStyleBackColor = true;
            this.bRedLightOn.Click += new System.EventHandler(this.Light_Click);
            // 
            // tbOrangeLightStatus
            // 
            this.tbOrangeLightStatus.BackColor = System.Drawing.Color.Yellow;
            this.tbOrangeLightStatus.Location = new System.Drawing.Point(6, 129);
            this.tbOrangeLightStatus.Name = "tbOrangeLightStatus";
            this.tbOrangeLightStatus.Size = new System.Drawing.Size(295, 20);
            this.tbOrangeLightStatus.TabIndex = 8;
            this.tbOrangeLightStatus.Text = "Orange";
            this.tbOrangeLightStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // bOrangeLightBlink
            // 
            this.bOrangeLightBlink.Location = new System.Drawing.Point(216, 155);
            this.bOrangeLightBlink.Name = "bOrangeLightBlink";
            this.bOrangeLightBlink.Size = new System.Drawing.Size(85, 23);
            this.bOrangeLightBlink.TabIndex = 11;
            this.bOrangeLightBlink.Text = "Blink";
            this.bOrangeLightBlink.UseVisualStyleBackColor = true;
            this.bOrangeLightBlink.Click += new System.EventHandler(this.Light_Click);
            // 
            // bOrangeLightOff
            // 
            this.bOrangeLightOff.Location = new System.Drawing.Point(109, 155);
            this.bOrangeLightOff.Name = "bOrangeLightOff";
            this.bOrangeLightOff.Size = new System.Drawing.Size(85, 23);
            this.bOrangeLightOff.TabIndex = 10;
            this.bOrangeLightOff.Text = "Off";
            this.bOrangeLightOff.UseVisualStyleBackColor = true;
            this.bOrangeLightOff.Click += new System.EventHandler(this.Light_Click);
            // 
            // bOrangeLightOn
            // 
            this.bOrangeLightOn.Location = new System.Drawing.Point(6, 155);
            this.bOrangeLightOn.Name = "bOrangeLightOn";
            this.bOrangeLightOn.Size = new System.Drawing.Size(85, 23);
            this.bOrangeLightOn.TabIndex = 9;
            this.bOrangeLightOn.Text = "On";
            this.bOrangeLightOn.UseVisualStyleBackColor = true;
            this.bOrangeLightOn.Click += new System.EventHandler(this.Light_Click);
            // 
            // tbBlueLightStatus
            // 
            this.tbBlueLightStatus.BackColor = System.Drawing.Color.Blue;
            this.tbBlueLightStatus.ForeColor = System.Drawing.Color.White;
            this.tbBlueLightStatus.Location = new System.Drawing.Point(6, 74);
            this.tbBlueLightStatus.Name = "tbBlueLightStatus";
            this.tbBlueLightStatus.Size = new System.Drawing.Size(295, 20);
            this.tbBlueLightStatus.TabIndex = 4;
            this.tbBlueLightStatus.Text = "Blue";
            this.tbBlueLightStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // bBlueLightBlink
            // 
            this.bBlueLightBlink.Location = new System.Drawing.Point(216, 100);
            this.bBlueLightBlink.Name = "bBlueLightBlink";
            this.bBlueLightBlink.Size = new System.Drawing.Size(85, 23);
            this.bBlueLightBlink.TabIndex = 7;
            this.bBlueLightBlink.Text = "Blink";
            this.bBlueLightBlink.UseVisualStyleBackColor = true;
            this.bBlueLightBlink.Click += new System.EventHandler(this.Light_Click);
            // 
            // bBlueLightOff
            // 
            this.bBlueLightOff.Location = new System.Drawing.Point(109, 100);
            this.bBlueLightOff.Name = "bBlueLightOff";
            this.bBlueLightOff.Size = new System.Drawing.Size(85, 23);
            this.bBlueLightOff.TabIndex = 6;
            this.bBlueLightOff.Text = "Off";
            this.bBlueLightOff.UseVisualStyleBackColor = true;
            this.bBlueLightOff.Click += new System.EventHandler(this.Light_Click);
            // 
            // bBlueLightOn
            // 
            this.bBlueLightOn.Location = new System.Drawing.Point(6, 100);
            this.bBlueLightOn.Name = "bBlueLightOn";
            this.bBlueLightOn.Size = new System.Drawing.Size(85, 23);
            this.bBlueLightOn.TabIndex = 5;
            this.bBlueLightOn.Text = "On";
            this.bBlueLightOn.UseVisualStyleBackColor = true;
            this.bBlueLightOn.Click += new System.EventHandler(this.Light_Click);
            // 
            // tbGreenLightStatus
            // 
            this.tbGreenLightStatus.BackColor = System.Drawing.Color.Lime;
            this.tbGreenLightStatus.Location = new System.Drawing.Point(6, 19);
            this.tbGreenLightStatus.Name = "tbGreenLightStatus";
            this.tbGreenLightStatus.Size = new System.Drawing.Size(295, 20);
            this.tbGreenLightStatus.TabIndex = 0;
            this.tbGreenLightStatus.Text = "Green";
            this.tbGreenLightStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // bGreenLightBlink
            // 
            this.bGreenLightBlink.Location = new System.Drawing.Point(216, 45);
            this.bGreenLightBlink.Name = "bGreenLightBlink";
            this.bGreenLightBlink.Size = new System.Drawing.Size(85, 23);
            this.bGreenLightBlink.TabIndex = 3;
            this.bGreenLightBlink.Text = "Blink";
            this.bGreenLightBlink.UseVisualStyleBackColor = true;
            this.bGreenLightBlink.Click += new System.EventHandler(this.Light_Click);
            // 
            // bGreenLightOff
            // 
            this.bGreenLightOff.Location = new System.Drawing.Point(109, 45);
            this.bGreenLightOff.Name = "bGreenLightOff";
            this.bGreenLightOff.Size = new System.Drawing.Size(85, 23);
            this.bGreenLightOff.TabIndex = 2;
            this.bGreenLightOff.Text = "Off";
            this.bGreenLightOff.UseVisualStyleBackColor = true;
            this.bGreenLightOff.Click += new System.EventHandler(this.Light_Click);
            // 
            // bGreenLightOn
            // 
            this.bGreenLightOn.Location = new System.Drawing.Point(6, 45);
            this.bGreenLightOn.Name = "bGreenLightOn";
            this.bGreenLightOn.Size = new System.Drawing.Size(85, 23);
            this.bGreenLightOn.TabIndex = 1;
            this.bGreenLightOn.Text = "On";
            this.bGreenLightOn.UseVisualStyleBackColor = true;
            this.bGreenLightOn.Click += new System.EventHandler(this.Light_Click);
            // 
            // gbBuzzer
            // 
            this.gbBuzzer.Controls.Add(this.tbBuzzerStatus);
            this.gbBuzzer.Controls.Add(this.bBuzzerOff);
            this.gbBuzzer.Controls.Add(this.bBuzzerOn);
            this.gbBuzzer.Location = new System.Drawing.Point(8, 6);
            this.gbBuzzer.Name = "gbBuzzer";
            this.gbBuzzer.Size = new System.Drawing.Size(200, 77);
            this.gbBuzzer.TabIndex = 0;
            this.gbBuzzer.TabStop = false;
            this.gbBuzzer.Text = "Buzzer";
            // 
            // tbBuzzerStatus
            // 
            this.tbBuzzerStatus.BackColor = System.Drawing.Color.Lime;
            this.tbBuzzerStatus.Location = new System.Drawing.Point(6, 17);
            this.tbBuzzerStatus.Name = "tbBuzzerStatus";
            this.tbBuzzerStatus.Size = new System.Drawing.Size(188, 20);
            this.tbBuzzerStatus.TabIndex = 0;
            this.tbBuzzerStatus.Text = "Buzzer";
            this.tbBuzzerStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // bBuzzerOff
            // 
            this.bBuzzerOff.Location = new System.Drawing.Point(109, 43);
            this.bBuzzerOff.Name = "bBuzzerOff";
            this.bBuzzerOff.Size = new System.Drawing.Size(85, 23);
            this.bBuzzerOff.TabIndex = 2;
            this.bBuzzerOff.Text = "Off";
            this.bBuzzerOff.UseVisualStyleBackColor = true;
            this.bBuzzerOff.Click += new System.EventHandler(this.Buzzer_Click);
            // 
            // bBuzzerOn
            // 
            this.bBuzzerOn.Location = new System.Drawing.Point(6, 43);
            this.bBuzzerOn.Name = "bBuzzerOn";
            this.bBuzzerOn.Size = new System.Drawing.Size(85, 23);
            this.bBuzzerOn.TabIndex = 1;
            this.bBuzzerOn.Text = "On";
            this.bBuzzerOn.UseVisualStyleBackColor = true;
            this.bBuzzerOn.Click += new System.EventHandler(this.Buzzer_Click);
            // 
            // tComLogs
            // 
            this.tComLogs.Controls.Add(this.bClearComLogs);
            this.tComLogs.Controls.Add(this.tbComLogs);
            this.tComLogs.Location = new System.Drawing.Point(4, 22);
            this.tComLogs.Name = "tComLogs";
            this.tComLogs.Size = new System.Drawing.Size(1208, 595);
            this.tComLogs.TabIndex = 6;
            this.tComLogs.Text = "Com logs";
            this.tComLogs.UseVisualStyleBackColor = true;
            // 
            // bClearComLogs
            // 
            this.bClearComLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bClearComLogs.BackColor = System.Drawing.Color.Transparent;
            this.bClearComLogs.Location = new System.Drawing.Point(993, 3);
            this.bClearComLogs.Name = "bClearComLogs";
            this.bClearComLogs.Size = new System.Drawing.Size(212, 23);
            this.bClearComLogs.TabIndex = 0;
            this.bClearComLogs.Text = "Clear Logs";
            this.bClearComLogs.UseVisualStyleBackColor = false;
            this.bClearComLogs.Click += new System.EventHandler(this.ClearComLogs_Click);
            // 
            // tbComLogs
            // 
            this.tbComLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbComLogs.Location = new System.Drawing.Point(0, 29);
            this.tbComLogs.Multiline = true;
            this.tbComLogs.Name = "tbComLogs";
            this.tbComLogs.ReadOnly = true;
            this.tbComLogs.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbComLogs.Size = new System.Drawing.Size(1205, 566);
            this.tbComLogs.TabIndex = 1;
            // 
            // tPostmanCom
            // 
            this.tPostmanCom.Controls.Add(this.bClearPostman);
            this.tPostmanCom.Controls.Add(this.tbPostman);
            this.tPostmanCom.Location = new System.Drawing.Point(4, 22);
            this.tPostmanCom.Name = "tPostmanCom";
            this.tPostmanCom.Padding = new System.Windows.Forms.Padding(3);
            this.tPostmanCom.Size = new System.Drawing.Size(1208, 595);
            this.tPostmanCom.TabIndex = 7;
            this.tPostmanCom.Text = "Postman Logs";
            this.tPostmanCom.UseVisualStyleBackColor = true;
            // 
            // bClearPostman
            // 
            this.bClearPostman.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bClearPostman.BackColor = System.Drawing.Color.Transparent;
            this.bClearPostman.Location = new System.Drawing.Point(993, 3);
            this.bClearPostman.Name = "bClearPostman";
            this.bClearPostman.Size = new System.Drawing.Size(212, 23);
            this.bClearPostman.TabIndex = 1;
            this.bClearPostman.Text = "Clear Logs";
            this.bClearPostman.UseVisualStyleBackColor = false;
            this.bClearPostman.Click += new System.EventHandler(this.bClearPostman_Click);
            // 
            // tbPostman
            // 
            this.tbPostman.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPostman.Location = new System.Drawing.Point(0, 29);
            this.tbPostman.Multiline = true;
            this.tbPostman.Name = "tbPostman";
            this.tbPostman.ReadOnly = true;
            this.tbPostman.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbPostman.Size = new System.Drawing.Size(1208, 566);
            this.tbPostman.TabIndex = 2;
            // 
            // tSendMessage
            // 
            this.tSendMessage.Controls.Add(this.rtbSendMsgDesc);
            this.tSendMessage.Controls.Add(this.tbMessageToSend3);
            this.tSendMessage.Controls.Add(this.tbMessageToSend2);
            this.tSendMessage.Controls.Add(this.tbMessageToSend);
            this.tSendMessage.Controls.Add(this.bSend3);
            this.tSendMessage.Controls.Add(this.lMessage3);
            this.tSendMessage.Controls.Add(this.bSend2);
            this.tSendMessage.Controls.Add(this.lMessage2);
            this.tSendMessage.Controls.Add(this.bSend);
            this.tSendMessage.Controls.Add(this.lMessage);
            this.tSendMessage.Location = new System.Drawing.Point(4, 22);
            this.tSendMessage.Name = "tSendMessage";
            this.tSendMessage.Padding = new System.Windows.Forms.Padding(3);
            this.tSendMessage.Size = new System.Drawing.Size(1208, 595);
            this.tSendMessage.TabIndex = 8;
            this.tSendMessage.Text = "Send msg";
            this.tSendMessage.UseVisualStyleBackColor = true;
            // 
            // rtbSendMsgDesc
            // 
            this.rtbSendMsgDesc.Location = new System.Drawing.Point(26, 6);
            this.rtbSendMsgDesc.Name = "rtbSendMsgDesc";
            this.rtbSendMsgDesc.ReadOnly = true;
            this.rtbSendMsgDesc.Size = new System.Drawing.Size(454, 49);
            this.rtbSendMsgDesc.TabIndex = 24;
            this.rtbSendMsgDesc.Text = "Here you can send custom TCP messages to the EFEM Controller. All textboxes have " +
    "the same behavior. They allow to memorize a message and to make it easy to send " +
    "consecutively several messages.";
            // 
            // tbMessageToSend3
            // 
            this.tbMessageToSend3.Location = new System.Drawing.Point(87, 121);
            this.tbMessageToSend3.Name = "tbMessageToSend3";
            this.tbMessageToSend3.Size = new System.Drawing.Size(312, 20);
            this.tbMessageToSend3.TabIndex = 21;
            // 
            // tbMessageToSend2
            // 
            this.tbMessageToSend2.Location = new System.Drawing.Point(87, 92);
            this.tbMessageToSend2.Name = "tbMessageToSend2";
            this.tbMessageToSend2.Size = new System.Drawing.Size(312, 20);
            this.tbMessageToSend2.TabIndex = 18;
            // 
            // tbMessageToSend
            // 
            this.tbMessageToSend.Location = new System.Drawing.Point(87, 63);
            this.tbMessageToSend.Name = "tbMessageToSend";
            this.tbMessageToSend.Size = new System.Drawing.Size(312, 20);
            this.tbMessageToSend.TabIndex = 15;
            // 
            // bSend3
            // 
            this.bSend3.Location = new System.Drawing.Point(405, 119);
            this.bSend3.Name = "bSend3";
            this.bSend3.Size = new System.Drawing.Size(75, 23);
            this.bSend3.TabIndex = 22;
            this.bSend3.Text = "Send";
            this.bSend3.UseVisualStyleBackColor = true;
            this.bSend3.Click += new System.EventHandler(this.bSend_Click);
            // 
            // lMessage3
            // 
            this.lMessage3.AutoSize = true;
            this.lMessage3.Location = new System.Drawing.Point(23, 124);
            this.lMessage3.Name = "lMessage3";
            this.lMessage3.Size = new System.Drawing.Size(53, 13);
            this.lMessage3.TabIndex = 20;
            this.lMessage3.Text = "Message:";
            // 
            // bSend2
            // 
            this.bSend2.Location = new System.Drawing.Point(405, 90);
            this.bSend2.Name = "bSend2";
            this.bSend2.Size = new System.Drawing.Size(75, 23);
            this.bSend2.TabIndex = 19;
            this.bSend2.Text = "Send";
            this.bSend2.UseVisualStyleBackColor = true;
            this.bSend2.Click += new System.EventHandler(this.bSend_Click);
            // 
            // lMessage2
            // 
            this.lMessage2.AutoSize = true;
            this.lMessage2.Location = new System.Drawing.Point(23, 95);
            this.lMessage2.Name = "lMessage2";
            this.lMessage2.Size = new System.Drawing.Size(53, 13);
            this.lMessage2.TabIndex = 17;
            this.lMessage2.Text = "Message:";
            // 
            // bSend
            // 
            this.bSend.Location = new System.Drawing.Point(405, 61);
            this.bSend.Name = "bSend";
            this.bSend.Size = new System.Drawing.Size(75, 23);
            this.bSend.TabIndex = 16;
            this.bSend.Text = "Send";
            this.bSend.UseVisualStyleBackColor = true;
            this.bSend.Click += new System.EventHandler(this.bSend_Click);
            // 
            // lMessage
            // 
            this.lMessage.AutoSize = true;
            this.lMessage.Location = new System.Drawing.Point(23, 66);
            this.lMessage.Name = "lMessage";
            this.lMessage.Size = new System.Drawing.Size(53, 13);
            this.lMessage.TabIndex = 14;
            this.lMessage.Text = "Message:";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // FormTester
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1216, 884);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(1232, 829);
            this.Name = "FormTester";
            this.Text = "Equipment Controller Simulator";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormTester_FormClosing);
            this.Load += new System.EventHandler(this.FormTester_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tcMain.ResumeLayout(false);
            this.tStart.ResumeLayout(false);
            this.tStart.PerformLayout();
            this.tControl.ResumeLayout(false);
            this.gbHighLevel.ResumeLayout(false);
            this.gbHighLevel.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.gbHLMacro.ResumeLayout(false);
            this.gbHLMacro.PerformLayout();
            this.gbLowLevel.ResumeLayout(false);
            this.gbLowLevel.PerformLayout();
            this.gbFfu.ResumeLayout(false);
            this.gbFfu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFfuSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudFfuSetPoint)).EndInit();
            this.gbEfem.ResumeLayout(false);
            this.gbEfem.PerformLayout();
            this.gbEfemSize.ResumeLayout(false);
            this.gbLLProcessModule.ResumeLayout(false);
            this.gbLLProcessModule.PerformLayout();
            this.gbLLRobot.ResumeLayout(false);
            this.gbLLRobot.PerformLayout();
            this.gbSetRobotSpeed.ResumeLayout(false);
            this.gbSetRobotSpeed.PerformLayout();
            this.gbRobotArmSpeed.ResumeLayout(false);
            this.gbRobotArmSpeed.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLDAEWT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudULSPAR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudULSPZD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudULSPAE)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLDSPAR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLDSPZU)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLDSPAE)).EndInit();
            this.gbLLLoadPorts.ResumeLayout(false);
            this.gbLLLoadPorts.PerformLayout();
            this.gbE84.ResumeLayout(false);
            this.gbCarrierID.ResumeLayout(false);
            this.gbCarrierID.PerformLayout();
            this.gbLLAligner.ResumeLayout(false);
            this.gbLLAligner.PerformLayout();
            this.gbOcr.ResumeLayout(false);
            this.gbOcr.PerformLayout();
            this.tLoadPorts.ResumeLayout(false);
            this.tLoadPorts.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCarrierType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTP5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTP4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTP3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTP2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTP1)).EndInit();
            this.tIO.ResumeLayout(false);
            this.gbSignals.ResumeLayout(false);
            this.gbLPIndicators.ResumeLayout(false);
            this.gbLPIndicators.PerformLayout();
            this.gbTower.ResumeLayout(false);
            this.gbTower.PerformLayout();
            this.gbBuzzer.ResumeLayout(false);
            this.gbBuzzer.PerformLayout();
            this.tComLogs.ResumeLayout(false);
            this.tComLogs.PerformLayout();
            this.tPostmanCom.ResumeLayout(false);
            this.tPostmanCom.PerformLayout();
            this.tSendMessage.ResumeLayout(false);
            this.tSendMessage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.OpenFileDialog ofdHard;
        private System.Windows.Forms.OpenFileDialog ofdJob;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TextBox tbLog;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.TabControl tcMain;
        private System.Windows.Forms.TabPage tStart;
        private System.Windows.Forms.TextBox tbPort;
        private System.Windows.Forms.TextBox tbIPAddress;
        private System.Windows.Forms.Label lPort;
        private System.Windows.Forms.Label lIpAddress;
        private System.Windows.Forms.Button bDisconnect;
        private System.Windows.Forms.Button bConnect;
        private System.Windows.Forms.Button bCreate;
        private System.Windows.Forms.PropertyGrid pgEFEM;
        private System.Windows.Forms.TabPage tControl;
        private System.Windows.Forms.GroupBox gbHighLevel;
        private System.Windows.Forms.Button bClearErrors;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button bHLJobStop;
        private System.Windows.Forms.Button bHLJobAbortActivity;
        private System.Windows.Forms.CheckBox chColdInit;
        private System.Windows.Forms.GroupBox gbHLMacro;
        private System.Windows.Forms.ComboBox tbMacroFiducial;
        private System.Windows.Forms.TextBox tbMacroAlignAngle;
        private System.Windows.Forms.ComboBox cbHLSlotMacro;
        private System.Windows.Forms.ComboBox cbHLLoadPortsMacro;
        private System.Windows.Forms.Button bHLLoadWaferMacro;
        private System.Windows.Forms.Button bHLUnloadWaferMacro;
        private System.Windows.Forms.Button bEmergencyStop;
        private System.Windows.Forms.Button bHLInit;
        private System.Windows.Forms.GroupBox gbLowLevel;
        private System.Windows.Forms.GroupBox gbEfem;
        private System.Windows.Forms.Button bEfemInitialization;
        private System.Windows.Forms.Button bEfemStop;
        private System.Windows.Forms.TextBox tbEFEMDeviceState;
        private System.Windows.Forms.GroupBox gbEfemSize;
        private System.Windows.Forms.ComboBox cbEfemSizeLocation;
        private System.Windows.Forms.ComboBox cbEfemSizeTarget;
        private System.Windows.Forms.Button bEfemGetSize;
        private System.Windows.Forms.Button bEfemSetSize;
        private System.Windows.Forms.Button bEfemResume;
        private System.Windows.Forms.Button bEfemPause;
        private System.Windows.Forms.GroupBox gbLLProcessModule;
        internal System.Windows.Forms.CheckBox chPMHotTemp;
        private System.Windows.Forms.ComboBox cbPMSize;
        private System.Windows.Forms.CheckBox chPMPresence;
        private System.Windows.Forms.Button bPMWaferDeposit;
        private System.Windows.Forms.TextBox tbPMWaferPresence;
        private System.Windows.Forms.GroupBox gbLLRobot;
        private System.Windows.Forms.GroupBox gbSetRobotSpeed;
        private System.Windows.Forms.ComboBox cbSelectRobotSpeed;
        private System.Windows.Forms.Button bSetRobotSpeed;
        private System.Windows.Forms.Button bPreparePlace;
        private System.Windows.Forms.GroupBox gbRobotArmSpeed;
        private System.Windows.Forms.Label lLDAEWT;
        private System.Windows.Forms.NumericUpDown nudLDAEWT;
        private System.Windows.Forms.ComboBox cbRobotLoadLockTarget;
        private System.Windows.Forms.Label lULSPAR;
        private System.Windows.Forms.NumericUpDown nudULSPAR;
        private System.Windows.Forms.Label lULSPZD;
        private System.Windows.Forms.NumericUpDown nudULSPZD;
        private System.Windows.Forms.Label lULSPAE;
        private System.Windows.Forms.NumericUpDown nudULSPAE;
        private System.Windows.Forms.Label lLDSPAR;
        private System.Windows.Forms.NumericUpDown nudLDSPAR;
        private System.Windows.Forms.Label lLDSPZU;
        private System.Windows.Forms.NumericUpDown nudLDSPZU;
        private System.Windows.Forms.Label lLDSPAE;
        private System.Windows.Forms.NumericUpDown nudLDSPAE;
        private System.Windows.Forms.Button bRobotGetSettings;
        private System.Windows.Forms.Button bRobotSetSettings;
        private System.Windows.Forms.CheckBox chRobotCmdGranted;
        private System.Windows.Forms.TextBox tbRobotDeviceState;
        private System.Windows.Forms.ComboBox cbRobotDestinationSlot;
        private System.Windows.Forms.ComboBox cbRobotDestinationLocation;
        private System.Windows.Forms.Button bRobotTransfer;
        private System.Windows.Forms.Button bRobotPlace;
        private System.Windows.Forms.ComboBox cbRobotArm;
        private System.Windows.Forms.ComboBox cbRobotSourceSlot;
        private System.Windows.Forms.ComboBox cbRobotSourceLocation;
        private System.Windows.Forms.Button bRobotPick;
        private System.Windows.Forms.Button bPreparePick;
        private System.Windows.Forms.Button bRobotHome;
        private System.Windows.Forms.Button bRobotInit;
        private System.Windows.Forms.TextBox tbRobotLowerArmWaferPresence;
        private System.Windows.Forms.TextBox tbRobotUpperArmWaferPresence;
        private System.Windows.Forms.GroupBox gbLLLoadPorts;
        private System.Windows.Forms.CheckBox chIsPurgeEnabled;
        private System.Windows.Forms.GroupBox gbE84;
        private System.Windows.Forms.Button bGetE84Inputs;
        private System.Windows.Forms.Button bE84Stop;
        private System.Windows.Forms.Button bE84Unload;
        private System.Windows.Forms.Button bGetE84Outputs;
        private System.Windows.Forms.Button bLPMoveToWritePosition;
        private System.Windows.Forms.Button bLPMoveToReadPosition;
        private System.Windows.Forms.TextBox tbLPCarrierID;
        private System.Windows.Forms.GroupBox gbCarrierID;
        private System.Windows.Forms.Button bCarrierIDEnter;
        private System.Windows.Forms.TextBox tbCarrierID;
        private System.Windows.Forms.Button bLPRelease;
        private System.Windows.Forms.Button bLPInAccess;
        private System.Windows.Forms.TextBox tbLPMapping;
        private System.Windows.Forms.TextBox tbLPDeviceState;
        private System.Windows.Forms.Button bLPMap;
        private System.Windows.Forms.Button bLPLastMapping;
        private System.Windows.Forms.TextBox tbLPInAccess;
        private System.Windows.Forms.TextBox tbLPPlacement;
        private System.Windows.Forms.TextBox tbLPPresence;
        private System.Windows.Forms.ComboBox cbLLLoadPorts;
        private System.Windows.Forms.Button bLPDock;
        private System.Windows.Forms.Button bLPUndock;
        private System.Windows.Forms.Button bLPInit;
        private System.Windows.Forms.Button bLPOpen;
        private System.Windows.Forms.Button bLPClose;
        private System.Windows.Forms.Button bLPHome;
        private System.Windows.Forms.Button bLPReadCarrierID;
        private System.Windows.Forms.GroupBox gbLLAligner;
        private System.Windows.Forms.TextBox tbAlignerDeviceState;
        private System.Windows.Forms.TextBox tbAlignerWaferPresence;
        private System.Windows.Forms.Button bAlignerInit;
        private System.Windows.Forms.Button bAlignerAlign;
        private System.Windows.Forms.TextBox tbAlignerAlignAngle;
        private System.Windows.Forms.Button bClear;
        private System.Windows.Forms.TabPage tLoadPorts;
        private System.Windows.Forms.Button bLPE84AutoHandling;
        private System.Windows.Forms.Button bLPE84ManualHandling;
        private System.Windows.Forms.Button bLPAuto;
        private System.Windows.Forms.Button bLPManual;
        private System.Windows.Forms.TextBox tbLP3AccessMode;
        private System.Windows.Forms.TextBox tbLP3ServiceState;
        private System.Windows.Forms.TextBox tbLP2AccessMode;
        private System.Windows.Forms.TextBox tbLP2ServiceState;
        private System.Windows.Forms.TextBox tbLP1AccessMode;
        private System.Windows.Forms.TextBox tbLP1ServiceState;
        private System.Windows.Forms.Button bLPInService;
        private System.Windows.Forms.Button bLPNotInService;
        private System.Windows.Forms.ComboBox cbLoadPorts;
        private System.Windows.Forms.TabPage tIO;
        private System.Windows.Forms.GroupBox gbSignals;
        private System.Windows.Forms.PropertyGrid pgSignals;
        private System.Windows.Forms.GroupBox gbLPIndicators;
        private System.Windows.Forms.TextBox tbAlarmIndicator;
        private System.Windows.Forms.Button bAlarmLightBlink;
        private System.Windows.Forms.Button bAlarmLightOff;
        private System.Windows.Forms.Button bAlarmLightOn;
        private System.Windows.Forms.TextBox tbReserveIndicator;
        private System.Windows.Forms.Button bReserveLightBlink;
        private System.Windows.Forms.Button bReserveLightOff;
        private System.Windows.Forms.Button bReserveLightOn;
        private System.Windows.Forms.TextBox tbAutoIndicator;
        private System.Windows.Forms.Button bAutoLightBlink;
        private System.Windows.Forms.Button bAutoLightOff;
        private System.Windows.Forms.Button bAutoLightOn;
        private System.Windows.Forms.ComboBox cbIOLoadPorts;
        private System.Windows.Forms.TextBox tbAccessIndicator;
        private System.Windows.Forms.Button bOpAccessLightBlink;
        private System.Windows.Forms.Button bOpAccessLightOff;
        private System.Windows.Forms.Button bOpAccessLightOn;
        private System.Windows.Forms.TextBox tbManualIndicator;
        private System.Windows.Forms.Button bManualLightBlink;
        private System.Windows.Forms.Button bManualLightOff;
        private System.Windows.Forms.Button bManualLightOn;
        private System.Windows.Forms.TextBox tbUnloadIndicator;
        private System.Windows.Forms.Button bUnloadLightBlink;
        private System.Windows.Forms.Button bUnloadLightOff;
        private System.Windows.Forms.Button bUnloadLightOn;
        private System.Windows.Forms.TextBox tbLoadIndicator;
        private System.Windows.Forms.Button bLoadLightBlink;
        private System.Windows.Forms.Button bLoadLightOff;
        private System.Windows.Forms.Button bLoadLightOn;
        private System.Windows.Forms.GroupBox gbTower;
        private System.Windows.Forms.TextBox tbRedLightStatus;
        private System.Windows.Forms.Button bRedLightBlink;
        private System.Windows.Forms.Button bRedLightOff;
        private System.Windows.Forms.Button bRedLightOn;
        private System.Windows.Forms.TextBox tbOrangeLightStatus;
        private System.Windows.Forms.Button bOrangeLightBlink;
        private System.Windows.Forms.Button bOrangeLightOff;
        private System.Windows.Forms.Button bOrangeLightOn;
        private System.Windows.Forms.TextBox tbBlueLightStatus;
        private System.Windows.Forms.Button bBlueLightBlink;
        private System.Windows.Forms.Button bBlueLightOff;
        private System.Windows.Forms.Button bBlueLightOn;
        private System.Windows.Forms.TextBox tbGreenLightStatus;
        private System.Windows.Forms.Button bGreenLightBlink;
        private System.Windows.Forms.Button bGreenLightOff;
        private System.Windows.Forms.Button bGreenLightOn;
        private System.Windows.Forms.GroupBox gbBuzzer;
        private System.Windows.Forms.TextBox tbBuzzerStatus;
        private System.Windows.Forms.Button bBuzzerOff;
        private System.Windows.Forms.Button bBuzzerOn;
        private System.Windows.Forms.TabPage tComLogs;
        private System.Windows.Forms.Button bClearComLogs;
        private System.Windows.Forms.TextBox tbComLogs;
        private System.Windows.Forms.TabPage tPostmanCom;
        private System.Windows.Forms.Button bClearPostman;
        private System.Windows.Forms.TextBox tbPostman;
        private System.Windows.Forms.TabPage tSendMessage;
        private System.Windows.Forms.RichTextBox rtbSendMsgDesc;
        private System.Windows.Forms.TextBox tbMessageToSend3;
        private System.Windows.Forms.TextBox tbMessageToSend2;
        private System.Windows.Forms.TextBox tbMessageToSend;
        private System.Windows.Forms.Button bSend3;
        private System.Windows.Forms.Label lMessage3;
        private System.Windows.Forms.Button bSend2;
        private System.Windows.Forms.Label lMessage2;
        private System.Windows.Forms.Button bSend;
        private System.Windows.Forms.Label lMessage;
        private System.Windows.Forms.Button bUnclampWaferOnArm;
        private System.Windows.Forms.Button bClampWaferOnArm;
        private System.Windows.Forms.Button bUnclampOnAligner;
        private System.Windows.Forms.Button bClampOnAligner;
        private System.Windows.Forms.TextBox tbRobotSpeed;
        private System.Windows.Forms.Button bGetWaferPresenceOnArm;
        private System.Windows.Forms.Button bLPGetSize;
        private System.Windows.Forms.TextBox tbLPWaferSize;
        private System.Windows.Forms.Button bEnableE84;
        private System.Windows.Forms.Button bDisableE84;
        private System.Windows.Forms.GroupBox gbOcr;
        private System.Windows.Forms.ComboBox cbOcrFrontSideRecipes;
        private System.Windows.Forms.Button bOcrGetRecipes;
        private System.Windows.Forms.ComboBox cbOcrWaferSide;
        private System.Windows.Forms.Button bLPUnclamp;
        private System.Windows.Forms.Button bLPClamp;
        private System.Windows.Forms.Button bEfemGetStat;
        private System.Windows.Forms.Button bEfemGetPressure;
        private System.Windows.Forms.TextBox tbEFEMPressure;
        private System.Windows.Forms.Button bEfemSetTime;
        private System.Windows.Forms.TextBox tbEFEMTime;
        private System.Windows.Forms.GroupBox gbFfu;
        private System.Windows.Forms.Button bFfuStop;
        private System.Windows.Forms.Button bFfuSetRpm;
        private System.Windows.Forms.Label lFfuSetPoint;
        private System.Windows.Forms.NumericUpDown nudFfuSetPoint;
        private System.Windows.Forms.NumericUpDown nudFfuSpeed;
        private System.Windows.Forms.Label lFfuSpeed;
        private System.Windows.Forms.Button bFfuGetRpm;
        private System.Windows.Forms.Button bOcrRead;
        private System.Windows.Forms.ComboBox cbOcrWaferType;
        private System.Windows.Forms.TextBox tbOCRWaferIDBackSide;
        private System.Windows.Forms.TextBox tbOCRWaferIDFrontSide;
        private System.Windows.Forms.ComboBox cbOcrBackSideRecipes;
        private System.Windows.Forms.Button bSetE84Timeout;
        private System.Windows.Forms.Label lTP5;
        private System.Windows.Forms.NumericUpDown nudTP5;
        private System.Windows.Forms.Label lTP4;
        private System.Windows.Forms.NumericUpDown nudTP4;
        private System.Windows.Forms.Label lTP3;
        private System.Windows.Forms.NumericUpDown nudTP3;
        private System.Windows.Forms.Label lTP2;
        private System.Windows.Forms.NumericUpDown nudTP2;
        private System.Windows.Forms.Label lTP1;
        private System.Windows.Forms.NumericUpDown nudTP1;
        private System.Windows.Forms.Button bLPE84Abort;
        private System.Windows.Forms.Button bLPE84Reset;
        private System.Windows.Forms.Button bAbortE84;
        private System.Windows.Forms.Button bResetE84;
        private System.Windows.Forms.Button bLPGetCarrierType;
        private System.Windows.Forms.TextBox tbLPCarrierType;
        private System.Windows.Forms.Button bSetCarrierType;
        private System.Windows.Forms.Label lCarrierType;
        private System.Windows.Forms.NumericUpDown nudCarrierType;
        private System.Windows.Forms.Button button1;
    }
}

