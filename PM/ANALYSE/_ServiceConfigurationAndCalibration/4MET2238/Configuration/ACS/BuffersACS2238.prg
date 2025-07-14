#/ Controller version = 2.29
#/ Date = 25-Jul-23 10:36
#/ User remarks = 
#1
! ---------Auto Startup routine-----------
AUTOEXEC:  
! ---------Auto Startup routine-----------

!
!-----------Change history----------------
!
!1.0 Initial Release	
!2.0 LandingAirBearing	
!2.1 Unified control settings XY	
!2.2 Transfer machine specific parameters to flash	
!2.3 Adaptations wrt FPMS interface
!2.4 Current Limitations
!2.5 Lower and Upper Optical Head optimization
!2.6 Machine specific Homing Offset: Lower and Upper Optical Head
!2.7 Additional_ Optimizations of OH control structure


!-----------Change history----------------


!Disable at startup all Interuptroutines because those are not initialized yet
DISABLEON 
!After intialization the Interupt routines are enabled in the concerning Buffer

real BeginTime				!Local variable used in WaferClamp activation

READ MachineNumber			!!Read Machine Number from flash memory (User Variable) 
READ Skew_Offset			!!Read Skew_Offset from flash memory (User Variable) 
READ AxisLOH_focus_DeltaHomingOffset !Read deltaOffset from flash memory (User Variable)
READ AxisUOH_focus_DeltaHomingOffset !Read deltaOffset from flash memory (User Variable)
READ AxisY_DeltaHomingOffset !Read deltaOffset from flash memory (User Variable)
READ AxisX_DeltaHomingOffset !Read deltaOffset from flash memory (User Variable)

XARRSIZE=1000000 !maximum Array elements to be reserved in memory (elements= line x columns)

!axis definition
AxisY0 			= 0
AxisY1 			= 1
Gantry_Y 		= 0
Gantry_Yaw		= 1
AxisX 			= 2
AxisLOH_focus	= 4
AxisLOH_Load 	= 6
AxisUOH_focus 	= 5
AxisUOH_Load 	= 7

!Y0-Range = [-150 , 175]
!Y1-Range = [-150 , 175]
!X-Range = [-150 , 215]
!WEX position X = 215 , Y=175

!KD ==Kill and then Disable
SAFETYCONF (Gantry_Y,Gantry_Yaw,AxisX),#RL,"K" !Overrule default Errorhandler
SAFETYCONF (Gantry_Y,Gantry_Yaw,AxisX),#LL,"K" !Overrule default Errorhandler
SAFETYCONF (Gantry_Y,Gantry_Yaw,AxisX),#NT,"K" !Overrule default Errorhandler
SAFETYCONF (Gantry_Y,Gantry_Yaw,AxisX),#HOT,"K" !Overrule default Errorhandler
SAFETYCONF (Gantry_Y,Gantry_Yaw,AxisX),#SRL,"K" !Overrule default Errorhandler
SAFETYCONF (Gantry_Y,Gantry_Yaw,AxisX),#SLL,"K" !Overrule default Errorhandler
SAFETYCONF (Gantry_Y,Gantry_Yaw,AxisX),#ENCNC,"K" !Overrule default Errorhandler
SAFETYCONF (Gantry_Y,Gantry_Yaw,AxisX),#ENC2NC,"K" !Overrule default Errorhandler
SAFETYCONF (Gantry_Y,Gantry_Yaw,AxisX),#DRIVE,"K" !Overrule default Errorhandler
!SAFETYCONF (Gantry_Y,Gantry_Yaw,AxisX),#ENC,"D" ! FIXED Errorhandler
!SAFETYCONF (Gantry_Y,Gantry_Yaw,AxisX),#ENC2,"D" !FIXED Errorhandler
SAFETYCONF (Gantry_Y,Gantry_Yaw,AxisX),#PE,"K" !Overrule default Errorhandler
!SAFETYCONF (Gantry_Y,Gantry_Yaw,AxisX),#CPE,"D" !FIXED  Errorhandler
SAFETYCONF (Gantry_Y,Gantry_Yaw,AxisX),#VL,"K" !Overrule default Errorhandler
SAFETYCONF (Gantry_Y,Gantry_Yaw,AxisX),#AL,"K" !Overrule default Errorhandler
SAFETYCONF (Gantry_Y,Gantry_Yaw,AxisX),#CL,"K" !Default Errorhandler
!SAFETYCONF (Gantry_Y,Gantry_Yaw,AxisX),#SP,"D" !FIXED  Errorhandler

!SYSTEM ERRORS for all axis
SAFETYCONF (Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus),#TEMP,"K" !Overrule default Errorhandler
SAFETYCONF (Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus),#PROG,"K" !Overrule default Errorhandler
SAFETYCONF (Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus),#MEM,"K" !Overrule default Errorhandler
SAFETYCONF (Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus),#TIME,"K" !Overrule default Errorhandler
SAFETYCONF (Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus),#ES,"K" !Overrule default Errorhandler
SAFETYCONF (Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus),#INT,"K" !Overrule default Errorhandler
SAFETYCONF (Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus),#INTGR,"K" !Overrule default Errorhandler
!SAFETYCONF (Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus),#FAILURE,"KD" !Only applicable for MC4U

!SAFETYGROUP (Gantry_Y,Gantry_Yaw,AxisX) !Axis are in same errorhandler group. If one axis is triggered by a Fault, the other axis will respond also
SAFETYGROUP (Gantry_Y,Gantry_Yaw,AxisX) !Axis are in same errorhandler group. If one axis is triggered by a Fault, the other axis will respond also

!Disable/Enable default errorhandling because this is done in this program
FDEF(Gantry_Y).#RL=1 !Disable/Enable Default Errorhandler
FDEF(Gantry_Y).#LL=1 !Disable/Enable Default Errorhandler
FDEF(Gantry_Y).#NT=1 !Disable/Enable Default Errorhandler
FDEF(Gantry_Y).#HOT=0 !Disable/Enable Default Errorhandler
FDEF(Gantry_Y).#SRL=1 !Disable/Enable Default Errorhandler
FDEF(Gantry_Y).#SLL=1 !Disable/Enable Default Errorhandler
FDEF(Gantry_Y).#ENCNC=1 !Disable/Enable Default Errorhandler
FDEF(Gantry_Y).#ENC2NC=1 !Disable/Enable Default Errorhandler
FDEF(Gantry_Y).#DRIVE=0 !Disable/Enable Default Errorhandler
!FDEF(Gantry_Y,Gantry_Yaw,AxisX) #ENC,"D" ! FIXED Errorhandler
!FDEF(Gantry_Y,Gantry_Yaw,AxisX) #ENC2,"D" !FIXED Errorhandler
FDEF(Gantry_Y).#PE=0 !Disable/Enable Default Errorhandler
!FDEF(Gantry_Y,Gantry_Yaw,AxisX) #CPE,"D" !FIXED  Errorhandler
FDEF(Gantry_Y).#VL=0 !Disable/Enable Default Errorhandler
FDEF(Gantry_Y).#AL=0 !Disable/Enable Default Errorhandler
!FDEF(Gantry_Y).#CL=0 !Default Errorhandler
!FDEF(Gantry_Y,Gantry_Yaw,AxisX) #SP,"D" !FIXED  Errorhandler
!SYSTEM ERRORS for all axis
FDEF(Gantry_Y).#TEMP=1 !Disable/Enable Default Errorhandler
FDEF(Gantry_Y).#PROG=1 !Disable/Enable Default Errorhandler
FDEF(Gantry_Y).#MEM=1 !Disable/Enable Default Errorhandler
FDEF(Gantry_Y).#TIME=1 !Disable/Enable Default Errorhandler
FDEF(Gantry_Y).#ES=0 !Disable/Enable Default Errorhandler
FDEF(Gantry_Y).#INT=1 !Disable/Enable Default Errorhandler
FDEF(Gantry_Y).#INTGR=1 !Disable/Enable Default Errorhandler

FDEF(Gantry_Yaw).#RL=1 !Disable/Enable Default Errorhandler
FDEF(Gantry_Yaw).#LL=1 !Disable/Enable Default Errorhandler
FDEF(Gantry_Yaw).#NT=1 !Disable/Enable Default Errorhandler
FDEF(Gantry_Yaw).#HOT=0 !Disable/Enable Default Errorhandler
FDEF(Gantry_Yaw).#SRL=1 !Disable/Enable Default Errorhandler
FDEF(Gantry_Yaw).#SLL=1 !Disable/Enable Default Errorhandler
FDEF(Gantry_Yaw).#ENCNC=1 !Disable/Enable Default Errorhandler
FDEF(Gantry_Yaw).#ENC2NC=1 !Disable/Enable Default Errorhandler
FDEF(Gantry_Yaw).#DRIVE=0 !Disable/Enable Default Errorhandler
!FDEF(Gantry_Yaw,Gantry_Yawaw,AxisX) .#ENC,"D" ! FIXED Errorhandler
!FDEF(Gantry_Yaw,Gantry_Yawaw,AxisX) .#ENC2,"D" !FIXED Errorhandler
FDEF(Gantry_Yaw).#PE=0 !Disable/Enable Default Errorhandler
!FDEF(Gantry_Yaw,Gantry_Yawaw,AxisX) .#CPE,"D" !FIXED  Errorhandler
FDEF(Gantry_Yaw).#VL=0 !Disable/Enable Default Errorhandler
FDEF(Gantry_Yaw).#AL=0 !Disable/Enable Default Errorhandler
!FDEF(Gantry_Yaw).#CL=0 !Default Errorhandler
!FDEF(Gantry_Yaw,Gantry_Yawaw,AxisX) .#SP,"D" !FIXED  Errorhandler
!SYSTEM ERRORS for all axis
FDEF(Gantry_Yaw).#TEMP=1 !Disable/Enable Default Errorhandler
FDEF(Gantry_Yaw).#PROG=1 !Disable/Enable Default Errorhandler
FDEF(Gantry_Yaw).#MEM=1 !Disable/Enable Default Errorhandler
FDEF(Gantry_Yaw).#TIME=1 !Disable/Enable Default Errorhandler
FDEF(Gantry_Yaw).#ES=0 !Disable/Enable Default Errorhandler
FDEF(Gantry_Yaw).#INT=1 !Disable/Enable Default Errorhandler
FDEF(Gantry_Yaw).#INTGR=1 !Disable/Enable Default Errorhandler
!FDEF(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus) .#FAILURE,"KD" !Only applicable for MC4U

FDEF(AxisX).#RL=1 !Disable/Enable Default Errorhandler
FDEF(AxisX).#LL=1 !Disable/Enable Default Errorhandler
FDEF(AxisX).#NT=1 !Disable/Enable Default Errorhandler
FDEF(AxisX).#HOT=0 !Disable/Enable Default Errorhandler
FDEF(AxisX).#SRL=1 !Disable/Enable Default Errorhandler
FDEF(AxisX).#SLL=1 !Disable/Enable Default Errorhandler
FDEF(AxisX).#ENCNC=1 !Disable/Enable Default Errorhandler
FDEF(AxisX).#ENC2NC=1 !Disable/Enable Default Errorhandler
FDEF(AxisX).#DRIVE=0 !Disable/Enable Default Errorhandler
!FDEF(AxisX,AxisXaw,AxisX) .#ENC,"D" ! FIXED Errorhandler
!FDEF(AxisX,AxisXaw,AxisX) .#ENC2,"D" !FIXED Errorhandler
FDEF(AxisX).#PE=0 !Disable/Enable Default Errorhandler
!FDEF(AxisX,AxisXaw,AxisX) .#CPE,"D" !FIXED  Errorhandler
FDEF(AxisX).#VL=0 !Disable/Enable Default Errorhandler
FDEF(AxisX).#AL=0 !Disable/Enable Default Errorhandler
!FDEF(AxisX).#CL=0 !Default Errorhandler
!FDEF(AxisX,AxisXaw,AxisX) .#SP,"D" !FIXED  Errorhandler
!SYSTEM ERRORS for all axis
FDEF(AxisX).#TEMP=1 !Disable/Enable Default Errorhandler
FDEF(AxisX).#PROG=1 !Disable/Enable Default Errorhandler
FDEF(AxisX).#MEM=1 !Disable/Enable Default Errorhandler
FDEF(AxisX).#TIME=1 !Disable/Enable Default Errorhandler
FDEF(AxisX).#ES=0 !Disable/Enable Default Errorhandler
FDEF(AxisX).#INT=1 !Disable/Enable Default Errorhandler
FDEF(AxisX).#INTGR=1 !Disable/Enable Default Errorhandler

FDEF(AxisLOH_focus).#RL=0 !Disable/Enable Default Errorhandler
FDEF(AxisLOH_focus).#LL=0 !Disable/Enable Default Errorhandler
FDEF(AxisLOH_focus).#NT=1 !Disable/Enable Default Errorhandler
FDEF(AxisLOH_focus).#HOT=0 !Disable/Enable Default Errorhandler
FDEF(AxisLOH_focus).#SRL=0 !Disable/Enable Default Errorhandler
FDEF(AxisLOH_focus).#SLL=0 !Disable/Enable Default Errorhandler
FDEF(AxisLOH_focus).#ENCNC=1 !Disable/Enable Default Errorhandler
FDEF(AxisLOH_focus).#ENC2NC=1 !Disable/Enable Default Errorhandler
FDEF(AxisLOH_focus).#DRIVE=0 !Disable/Enable Default Errorhandler
!FDEF(AxisLOH_focus,AxisLOH_focusaw,AxisLOH_focus) .#ENC,"D" ! FIXED Errorhandler
!FDEF(AxisLOH_focus,AxisLOH_focusaw,AxisLOH_focus) .#ENC2,"D" !FIXED Errorhandler
FDEF(AxisLOH_focus).#PE=0 !Disable/Enable Default Errorhandler
!FDEF(AxisLOH_focus,AxisLOH_focusaw,AxisLOH_focus) .#CPE,"D" !FIXED  Errorhandler
FDEF(AxisLOH_focus).#VL=0 !Disable/Enable Default Errorhandler
FDEF(AxisLOH_focus).#AL=0 !Disable/Enable Default Errorhandler
!FDEF(AxisLOH_focus).#CL=0 !Default Errorhandler
!FDEF(AxisLOH_focus,AxisLOH_focusaw,AxisLOH_focus) .#SP,"D" !FIXED  Errorhandler
!SYSTEM ERRORS for all axis
FDEF(AxisLOH_focus).#TEMP=1 !Disable/Enable Default Errorhandler
FDEF(AxisLOH_focus).#PROG=1 !Disable/Enable Default Errorhandler
FDEF(AxisLOH_focus).#MEM=1 !Disable/Enable Default Errorhandler
FDEF(AxisLOH_focus).#TIME=1 !Disable/Enable Default Errorhandler
FDEF(AxisLOH_focus).#ES=0 !Disable/Enable Default Errorhandler
FDEF(AxisLOH_focus).#INT=1 !Disable/Enable Default Errorhandler
FDEF(AxisLOH_focus).#INTGR=1 !Disable/Enable Default Errorhandler

FDEF(AxisUOH_focus).#RL=0 !Disable/Enable Default Errorhandler
FDEF(AxisUOH_focus).#LL=0 !Disable/Enable Default Errorhandler
FDEF(AxisUOH_focus).#NT=1 !Disable/Enable Default Errorhandler
FDEF(AxisUOH_focus).#HOT=0 !Disable/Enable Default Errorhandler
FDEF(AxisUOH_focus).#SRL=0 !Disable/Enable Default Errorhandler
FDEF(AxisUOH_focus).#SLL=0 !Disable/Enable Default Errorhandler
FDEF(AxisUOH_focus).#ENCNC=1 !Disable/Enable Default Errorhandler
FDEF(AxisUOH_focus).#ENC2NC=1 !Disable/Enable Default Errorhandler
FDEF(AxisUOH_focus).#DRIVE=0 !Disable/Enable Default Errorhandler
!FDEF(AxisUOH_focus,AxisUOH_focusaw,AxisUOH_focus) .#ENC,"D" ! FIXED Errorhandler
!FDEF(AxisUOH_focus,AxisUOH_focusaw,AxisUOH_focus) .#ENC2,"D" !FIXED Errorhandler
FDEF(AxisUOH_focus).#PE=0 !Disable/Enable Default Errorhandler
!FDEF(AxisUOH_focus,AxisUOH_focusaw,AxisUOH_focus) .#CPE,"D" !FIXED  Errorhandler
FDEF(AxisUOH_focus).#VL=0 !Disable/Enable Default Errorhandler
FDEF(AxisUOH_focus).#AL=0 !Disable/Enable Default Errorhandler
!FDEF(AxisUOH_focus).#CL=0 !Default Errorhandler
!FDEF(AxisUOH_focus,AxisUOH_focusaw,AxisUOH_focus) .#SP,"D" !FIXED  Errorhandler
!SYSTEM ERRORS for all axis
FDEF(AxisUOH_focus).#TEMP=1 !Disable/Enable Default Errorhandler
FDEF(AxisUOH_focus).#PROG=1 !Disable/Enable Default Errorhandler
FDEF(AxisUOH_focus).#MEM=1 !Disable/Enable Default Errorhandler
FDEF(AxisUOH_focus).#TIME=1 !Disable/Enable Default Errorhandler
FDEF(AxisUOH_focus).#ES=0 !Disable/Enable Default Errorhandler
FDEF(AxisUOH_focus).#INT=1 !Disable/Enable Default Errorhandler
FDEF(AxisUOH_focus).#INTGR=1 !Disable/Enable Default Errorhandler
!----------------------------------------------------------------------------

TimeOut=10000
!Mask Faults from non used axis in the application
!Mask Axis 2 & 3,4,5,6,
INT nonAxis
nonAxis=3
FMASK(nonAxis)=65536 !Mask all bits (10000000000000000) of the FAULT concerning axis 

nonAxis=8
WHILE nonAxis<16
	FMASK(nonAxis)=65536 !Mask all bits (10000000000000000) of the FAULT concerning axis 
	nonAxis = nonAxis+1
END
!NOTE: Unmask axis by FMASK(axis)=131071

SLLIMIT(AxisY0)=-1E7		!Disable Software Left Limit at startup
SRLIMIT(AxisY0)=1E7			!Disable Software Right Limit at startup 
SLLIMIT(AxisY1)=-1E7		!Disable Software Left Limit at startup
SRLIMIT(AxisY1)=1E7			!Disable Software Right Limit at startup 
SLLIMIT(AxisX)=-1E7			!Disable Software Left Limit at startup 
SRLIMIT(AxisX)=1E7			!Disabl Software Right Limit at startup 
SLLIMIT(AxisLOH_focus)=-1E7		!Disable Software Left Limit at startup
SRLIMIT(AxisLOH_focus)=1E7		!Disable Software Right Limit at startup 
SLLIMIT(AxisUOH_focus)=-1E7		!Disable Software Left Limit at startup 
SRLIMIT(AxisUOH_focus)=1E7		!Disable Software Right Limit at startup 

!---------------EtherCAT adress of Beckhoff Modules in E-cabinet----------------------
EC_AnOutEL4104_0 = 348 !Anout0
EC_AnOutEL4104_1 = 350 !AnOut1
EC_AnOutEL4104_2 = 352 !Anout2
EC_AnOutEL4104_3 = 354 !AnOut3

EC_DigOutEL2008_0 = 364		!8 port DigOutput (xxx.0---xxx.7)

EC_DigInEL1008_0 = 411		!8 port DigInput (xxx.0---xxx.7)

!-----------------------------------------------

!--------------EtherCAT adress of Beckhoff Modules in Metrology module----------------
ECUNMAP ! Unmap EtherCAT configurations before mapping

EC_TempInEL3204_0 = 381		!TempInput0 
EC_TempInEL3204_1 = 385		!TempInput1 
EC_TempInEL3204_2 = 389		!TempInput2 
EC_TempInEL3204_3 = 393		!TempInput3 

EC_AnOutEL4124_0 = 356 		!Anout0 4-20mA 
EC_AnOutEL4124_1 = 358 		!Anout1 4-20mA 
EC_AnOutEL4124_2 = 360 		!Anout2 4-20mA 
EC_AnOutEL4124_3 = 362 		!Anout3 4-20mA 

EC_AnInEL3054_0 = 397		!AnIn0 4-20mA
EC_AnInEL3054_1 = 401		!AnIn1 4-20mA
EC_AnInEL3054_2 = 405		!AnIn2 4-20mA
EC_AnInEL3054_3 = 409		!AnIn3 4-20mA

EC_DigOutEL2008_1 = 365		!8 port DigOutput (xxx.0---xxx.7)

EC_DigInEL1008_1 = 412		!8 port DigInput (xxx.0---xxx.7)

EC_DigInEL1008_2 = 413		!8 port DigInput (xxx.0---xxx.7)

EC_DigOutEL2008_2 = 366	!8 port DigOutput (xxx.0---xxx.7)

EC_DigInEL1008_3 = 414		!8 port DigInput (xxx.0---xxx.7)
!-----------------------------------------------

!Init variables--------------------------------------------------------

ACS_Stat_Process				=	0		!Status variable Process
ACS_Stat_Airbearing 			=	0		!Status variable Airbearing
ACS_Stat_WaferStage				=	0		!Status variable WaferStage
ACS_Stat_LOH_focus				=	0		!Status variable LOH_focus
ACS_Stat_UOH_focus				=	0		!Status variable UOH_focus

ACS_Init_Process				=	0		!Start Initialization Process
ACS_Init_Airbearing 			=	0		!Start Initialization Airbearing
ACS_Init_WaferStage				=	0		!Start Initiatization  WaferStage
ACS_Init_LOH_focus				=	0		!Start Initialization LOH_focus
ACS_Init_UOH_focus				=	0		!Start Initialization UOH_focus
softEmergency_Stop				=	0		!Stop all axis, retract LOH, UOH focus axis and disable

AirbearingPressureControl 		= 	4.5
AirbearingVacuumControl0		=	-0.5
AirbearingVacuumControl1		=	-0.45

PressureLimit0					=	3.6		!PressureLimit
VacuumSensor0Limit				=	-0.45	!VacuumLimit
VacuumSensor1Limit				=	-0.405	!VacuumLimit

!StageLift						=	0		!StageLift output

MotorTemperatureY0_Limit		=	50!75		!Motor temperature limit
MotorTemperatureY1_Limit		=	50!75		!Motor temperature limit
MotorTemperatureX_Limit			=	50!75		!Motor temperature limit

vel_focus_service				=	10 		!max focus velocity in Service Mode = 10 mm/s
vel_gantry_service				=	50 		!max Gantry velocity in Service Mode = 100 mm/s

LandAirbearingTime				=	600		!Time in ms needed to land the Airbearing on the Granite

EFEM_InterfacePower				=	1		!At startup EFEM Interface Power is set, to enable the EFEM Robot Retract safety functionality

!PowerOK_24V					=	0		!Power Ok power supply
!PowerOK_5V						=	0		!Power Ok power supply
!PowerOK_48V_OH_focus			=	0		!Power Ok power supply

ResetFAULT						=	0		
P_FAULT							=	0		!Process Error codes
AirbearingPressure_ErrorCode	= 	1		!Airbearing Pressure Not Ok
AirbearingVacuum0_ErrorCode		= 	2		!Airbearing Vacuum0 Not Ok
AirbearingVacuum1_ErrorCode		= 	4		!Airbearing Vacuum1 Not Ok
AirSupplyPresent_ErrorCode		= 	8		!Air supply pressure Not Ok 
VacuumSupplyPresent_ErrorCode	= 	16		!Vacuum supply pressure Not Ok
MotorTemperatureY0_ErrorCode	= 	32		!Motortemperature Y0 too high
MotorTemperatureY1_ErrorCode	= 	64		!Motortemperature Y1 too high
MotorTemperatureX_ErrorCode		= 	128		!Motortemperature X too high
WaferClamp_ErrorCode			= 	256		!WaferClamp not Ok during clamping)
Power_ErrorCode					= 	1024	!One or more of the Power Supplies Not Ok
softEmergencyStop_ErrorCode		= 	2048	!softEmergencyStop triggered by software

FFU_Error 						=	0;		!FOGALE USER DEFINED!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!													


!Buffer definitions
Buffer_Commutation	=	0			!0: Commutation Algorithm
Buffer_AutoExe		=	1			!1: StartUp Buffer with Application Variables
Buffer_SafetyErrorhandling=	2			!2:	Autoroutines with safety and error handling
Buffer_Process		=	3			!3: Process functions
Buffer_WaferStage	=	4			!4: Initialization of WaferStage
Buffer_AxisLOH_focus=	5			!5:	Initialization of AxisLOH_focus
Buffer_AxisUOH_focus=	6			!6:	Initialization of AxisLOH_focus
Buffer_Gainscheduling=	7			!7: Gainscheduling
Buffer_Test			=	8			!8: For Testpurposes
Buffer_ServiceProgram = 10			!10: Service Mode program
Buffer_EFEM			=	11			!11: !Enable safety monitoring of EFEM-robot arm extraction
Buffer_LandAirbearing = 12			!12 Land Airbearing


!End of Init variables--------------------------------------------------------

! Mapping EtherCAT communication channels to variables

!---------------E-cabinet----------------------
ECOUT(EC_AnOutEL4104_0,AnOutEL4104_0)
ECOUT(EC_AnOutEL4104_1,AnOutEL4104_1)
ECOUT(EC_AnOutEL4104_2,AnOutEL4104_2)
ECOUT(EC_AnOutEL4104_3,AnOutEL4104_3)

ECOUT(EC_DigOutEL2008_0,DigOutEL2008_0)

ECIN(EC_DigInEL1008_0,DigInEL1008_0)
!-----------------------------------------------


!---------------Metrology module----------------
ECIN(EC_TempInEL3204_0,TempInEL3204_0)
ECIN(EC_TempInEL3204_1,TempInEL3204_1)
ECIN(EC_TempInEL3204_2,TempInEL3204_2)
ECIN(EC_TempInEL3204_3,TempInEL3204_3)
COEWRITE/2 (7,0x8000,0x19,5) !CONFIGURE EL3204-0200 module to 10K NTC resistor measurements 5: NTC 10k 
COEWRITE/2 (7,0x8010,0x19,5) !CONFIGURE EL3204-0200 module to 10K NTC resistor measurements 5: NTC 10k 
COEWRITE/2 (7,0x8020,0x19,5) !CONFIGURE EL3204-0200 module to 10K NTC resistor measurements 5: NTC 10k 
COEWRITE/2 (7,0x8030,0x19,5) !CONFIGURE EL3204-0200 module to 10K NTC resistor measurements 5: NTC 10k 


ECOUT(EC_AnOutEL4124_0,AnOutEL4124_0)
ECOUT(EC_AnOutEL4124_1,AnOutEL4124_1)
ECOUT(EC_AnOutEL4124_2,AnOutEL4124_2)
ECOUT(EC_AnOutEL4124_3,AnOutEL4124_3)

ECIN(EC_AnInEL3054_0,AnInEL3054_0)
ECIN(EC_AnInEL3054_1,AnInEL3054_1)
ECIN(EC_AnInEL3054_2,AnInEL3054_2)
ECIN(EC_AnInEL3054_3,AnInEL3054_3)


ECOUT(EC_DigOutEL2008_1,DigOutEL2008_1)

ECIN(EC_DigInEL1008_1,DigInEL1008_1)

ECIN(EC_DigInEL1008_2,DigInEL1008_2)

ECOUT(EC_DigOutEL2008_2,DigOutEL2008_2)

ECIN(EC_DigInEL1008_3,DigInEL1008_3)

! ---------------------------------------------



!-----------------!Autoroutines in Buffers below are started -----------------------------------------------------------------------------------------------------------------------------------------------------
ENABLEON Buffer_AutoExe				
ENABLEON Buffer_SafetyErrorhandling			
ENABLEON Buffer_ServiceProgram		
!Autoroutines in Buffers below are started (ENABLEON xxxx) according to:

!Buffer_Process		=	3			!3: Process functions								Started "IF ACS_Init_Process = 1"
!Buffer_WaferStage	=	4			!4: Initialization of WaferStage					Started "IF ACS_Init_WaferStage = 1"
!Buffer_AxisLOH_focus=	5			!5:	Initialization of AxisLOH_focus					Started "IF ACS_Init_LOH_focus=1"
!Buffer_AxisUOH_focus=	6			!6:	Initialization of AxisUOH_focus					Started "IF ACS_Init_UOH_focus=1"
!Buffer_Gainscheduling=	7			!7: Gainscheduling									Not yet implemented
!Buffer_EFEM			=	11		!11: !Enable safety monitoring of EFEM-robot arm extraction when Gantry becomes initialized Started "IF ACS_Init_WaferStage = 1"
!Buffer_LandAirbearing = 12			!12 Land Airbearing
!----------------------------------------------------------------------------------------------------------------------------------------------------------------------


while 1
	IF ACS_Init_Process = 1
			!call ACS_Init_WaferStage ACSPL script
			IF PST(Buffer_Process).#RUN=0 !Buffer is not running
				START Buffer_Process,1  !Start Buffer 3 at line 1 
			ELSE
				DISP"Buffer Process already running"
			END
		ACS_Init_Process = 0
	END
	
	IF ACS_Init_Airbearing = 1
			!call ACS_Init_WaferStage ACSPL script
			IF PST(Buffer_Process).#RUN=0 !Buffer is not running
				START Buffer_Process,Init_Airbearing  !Start Buffer 3 at line Airbearing
			ELSE
				DISP"Buffer Process-Init already running"
			END
		ACS_Init_Airbearing = 0
	END
	

	IF ACS_Init_LOH_focus=1
			!call ACS_Init_WaferStage ACSPL script
			IF PST(Buffer_AxisLOH_focus).#RUN=0 !Buffer is not running
				START Buffer_AxisLOH_focus,1  !Start Buffer 5 at line 1 
			ELSE
				DISP"Buffer LOH-Init already running"
			END
		ACS_Init_LOH_focus=0
	END

	
	IF ACS_Init_UOH_focus=1
			!call ACS_Init_WaferStage ACSPL script
			IF PST(Buffer_AxisUOH_focus).#RUN=0 !Buffer is not running
				START Buffer_AxisUOH_focus,1  !Start Buffer 6 at line 1
			ELSE
				DISP"Buffer UOH-Init already running"
			END
		ACS_Init_UOH_focus=0
	END
	
	IF ACS_Init_WaferStage = 1
			!call ACS_Init_WaferStage ACSPL script
			IF PST(Buffer_WaferStage).#RUN=0 !Buffer is not running
				START Buffer_WaferStage,1	!!Start Buffer 4 at line 1 :::
			ELSE
				DISP"Buffer WaferStage-Init already running"
			END
		ACS_Init_WaferStage = 0
	END	

	
	! Each MPU cycle the IO variables will be updated
	BLOCK
	
	
!-------------------E-cabinet----------------------
		AnOutEL4104_0	=	AnOut0
		AnOutEL4104_1	=	AnOut1
		AnOutEL4104_2	=	AnOut2
		AnOutEL4104_3	=	AnOut3
		
		DigOutEL2008_0.0	=	DigOut0
		DigOutEL2008_0.1	=	DigOut1
		DigOutEL2008_0.2	=	DigOut2
		DigOutEL2008_0.3	=	DigOut3
		DigOutEL2008_0.4	=	DigOut4
		DigOutEL2008_0.5	=	DigOut5
		DigOutEL2008_0.6	=	DigOut6
		DigOutEL2008_0.7	=	DigOut7
		
		DigIn0		=	DigInEL1008_0.0
		DigIn1		=	DigInEL1008_0.1
		DigIn2		=	DigInEL1008_0.2
		DigIn3		=	DigInEL1008_0.3
		DigIn4		=	DigInEL1008_0.4
		DigIn5		=	DigInEL1008_0.5
		DigIn6		=	DigInEL1008_0.6
		DigIn7		=	DigInEL1008_0.7
		
		
!------------------Metrology module----------------
		AnIn0		=	TempInEL3204_0
		AnIn1		=	TempInEL3204_1
		AnIn2		=	TempInEL3204_2
		AnIn3		=	TempInEL3204_3
		
		AnOutEL4124_0	=	AnOut4
		AnOutEL4124_1	=	AnOut5
		AnOutEL4124_2	=	AnOut6
		AnOutEL4124_3	=	AnOut7
		
		AnIn4		=	AnInEL3054_0
		AnIn5		=	AnInEL3054_1
		AnIn6		=	AnInEL3054_2
		AnIn7		=	AnInEL3054_3
		
		DigOutEL2008_1.0	=	DigOut8
		DigOutEL2008_1.1	=	DigOut9
		DigOutEL2008_1.2	=	DigOut10
		DigOutEL2008_1.3	=	DigOut11
		DigOutEL2008_1.4	=	DigOut12
		DigOutEL2008_1.5	=	DigOut13
		DigOutEL2008_1.6	=	DigOut14
		DigOutEL2008_1.7	=	DigOut15
		
		DigIn8		=	DigInEL1008_1.0
		DigIn9		=	DigInEL1008_1.1
		DigIn10		=	DigInEL1008_1.2
		DigIn11		=	DigInEL1008_1.3
		DigIn12		=	DigInEL1008_1.4
		DigIn13		=	DigInEL1008_1.5
		DigIn14		=	DigInEL1008_1.6
		DigIn15		=	DigInEL1008_1.7
		
		DigIn16		=	DigInEL1008_2.0
		DigIn17		=	DigInEL1008_2.1
		DigIn18		=	DigInEL1008_2.2
		DigIn19		=	DigInEL1008_2.3
		DigIn20		=	DigInEL1008_2.4
		DigIn21		=	DigInEL1008_2.5
		DigIn22		=	DigInEL1008_2.6
		DigIn23		=	DigInEL1008_2.7
		
		DigOutEL2008_2.0	=	DigOut16
		DigOutEL2008_2.1	=	DigOut17
		DigOutEL2008_2.2	=	DigOut18
		DigOutEL2008_2.3	=	DigOut19
		DigOutEL2008_2.4	=	DigOut20
		DigOutEL2008_2.5	=	DigOut21
		DigOutEL2008_2.6	=	DigOut22
		DigOutEL2008_2.7	=	DigOut23
		
		DigIn24		=	DigInEL1008_3.0
		DigIn25		=	DigInEL1008_3.1
		DigIn26		=	DigInEL1008_3.2
		DigIn27		=	DigInEL1008_3.3
		DigIn28		=	DigInEL1008_3.4
		DigIn29		=	DigInEL1008_3.5
		DigIn30		=	DigInEL1008_3.6
		DigIn31		=	DigInEL1008_3.7

	
	END
	! FOGALE USER DEFINED
	!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!11
	BLOCK
		EFEM_StageLoadPos = WEX_sensor
		EFEM_WaferPresent = ^WaferClamp_sensor	
	END
	!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!11
	BLOCK !Update variables each MPU cycle
 		
		!Just for simulation
		!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!AnIn4= AnOut4+RAND(1,767)
		!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!AnIn5=AnOut5+RAND(0,767)
		!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!AnIn6=AnOut6+RAND(100,400)
		!Just for simulation

!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		!---------------E-cabinet----------------------
		!0 - 32767	==	0 - 10V  	=	0 - 100%
		AnOut0						=	Light_Control_Halogen_UOH*327.67			!Light_Control_Halogen_UOH 			
		AnOut1						=	Light_Control_Halogen_LOH*327.67			!Light_Control_Halogen_LOH
		AnOut2						=	Light_Control_VIS_RED_LED*327.67			!Light_Control_VIS_RED_LED
		AnOut3						=	Light_Control_VIS_WHITE_LED*327.67			!Light_Control_VIS_WHITE_LED
		
		DigOut0						=	Light_Power_Halogen_UOH						!Light_Power_Halogen_UOH On/Off
		DigOut1						=	Light_Power_Halogen_LOH						!Light_Power_Halogen_LOH On/Off
		DigOut2						=	Light_Power_VIS_RED_LED						!Light_Power_VIS_RED_LED On/Off
		DigOut3						=	Light_Power_VIS_WHITE_LED					!Light_Power_VIS_WHITE_LED On/Off
		DigOut4						=	FFU_Power									!Power FFU On/Off
		!DigOut5						=	
		!DigOut6						=	
		!DigOut7						=	
		
		PowerOK_24V					=	DigIn0						!Power Ok power supply
		PowerOK_5V					=	DigIn1						!Power Ok power supply
		PowerOK_48V_OH_focus		=	DigIn2						!Power Ok power supply
		PowerOK_48V_WaferStage		=	DigIn3						!Power Ok power supply
		PowerOK_24V_Aux				=	DigIn4						!Power Ok power supply
		!							=	DigIn5
		!							=	DigIn6
		!							=	DigIn7	
		
		!---------------Metrology module----------------
		!xxxxx				 		=	0 - 32767
		MotorTemperatureY0			= 	(AnIn0/10) 	 				!Motor temperature 
		MotorTemperatureY1			= 	(AnIn1/10) 					!Motor temperature 
		MotorTemperatureX			= 	(AnIn2/10) 					!Motor temperature 
		
		!0 - 32767	==	4 -20mA	 	= 	2.5 - 7 barg (==relative pressure wrt Atmosfer-->3.5 - 8 Bar absolute)
		!0 - 32767	==	4 -20mA	 	= 	3 - 6 barg (==relative pressure wrt Atmosfer-->4 - 7 Bar absolute)
		AnOut4						=	(AirbearingPressureControl-3)*(32767/3)	!AirPressureControl of Airbearing
		!0 - 32767	==	4 -20mA	 	= 	0 -  (-0.8 barg) ((==relative pressure wrt Atmosfer-->0 - 0.2 Bar absolute)
		AnOut5						=	AirbearingVacuumControl0*(32767/(-0.8))		!VacuumControl of Airbearing0
		AnOut6						=	AirbearingVacuumControl1*(32767/(-0.8))		!VacuumControl of Airbearing1
		
		! 2.5 - 7 barg	 = 4 -20mA	=	0 - 32767 
		! 3 - 6 barg	 = 4 -20mA	=	0 - 32767 
		AirbearingPressureSensor	= 	AnIn4*(3/32767)+3		!AirPressureSensor of Airbearing
		!0 - (-0.8) barg 	 = 4 -20mA	=	0 - 32767	

		AirbearingVacuumSensor0 	=	AnIn5*(-0.8/32767)			!VacuumSensor of Airbearing0
		AirbearingVacuumSensor1		=	AnIn6*(-0.8/32767)			!VacuumSensor of Airbearing1
		
		DigOut8						=	StageLift					!Airvalve to Lift the WaferStage
		DigOut9						=	nWaferClampDigOut			!AirValve to release the wafer	
		DigOut10					=	WaferClampDigOut			!AirValve to clamp the wafer
		!DigOut11					=	LandAirbearing				!AirValve to depressurise the airbearings See also Buffer12
		!DigOut14					=	ServiceLight				!ServiceLightOn/Off	See Buffer12
		!DigOut15					=	ServiceLight				!ServiceLightOn/Off	See Buffer12

		DigOut16					=	EFEM_InterfacePower
		DigOut17					=	EFEM_StageLoadPos
		DigOut18					=	EFEM_WaferPresent
		DigOut19					=	EFEM_StageIs200
		DigOut20					=	EFEM_WaferPresent300

		
		AirSupplyPresent			=	DigIn8						!Digital signal indicates presence of AirSupply
		VacuumSupplyPresent			=	DigIn9						!Digital signal indicates presence of VacuumSupply
		WaferClamp_sensor			=	DigIn10						!Digital signal indicates presence of Wafer
		WEX_sensor 					=	DigIn11						!Digital signal indicates presence of WaferStage at WEX position
		!							=	DigIn12
		!							=	DigIn13
		!							=	DigIn14
		!							=	DigIn15		
		
		DoorLeft0Closed				=	DigIn16						!If Door is closed, signal is active 	
		DoorLeft1Closed				=	DigIn17						!If Door is closed, signal is active 	
		DoorRight0Closed			=	DigIn18						!If Door is closed, signal is active 	
		DoorRight1Closed			=	DigIn19						!If Door is closed, signal is active 	
		DoorBack0Closed				=	DigIn20						!If Door is closed, signal is active 	
		DoorBack1Closed				=	DigIn21						!If Door is closed, signal is active 	
		ServiceModeOn				=	^DigIn22					!If DigIn22 is low then ServiceMode is active
		!							=	DigIn23
		EFEM_RobotArmIsExtended		=	^DigIn24					!If RobotArm of EFEM is extended, then motion must freeze preventing collision
		EFEM_RESERVED				=	DigIn25			
		!							=	DigIn26
		!							=	DigIn27
		!							=	DigIn28
		!							=	DigIn29	
		!							=	DigIn30
		!							=	DigIn31

	END
	
		
		! Update Error status
		!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!11
	BLOCK
		IF (AirbearingPressureSensor > (PressureLimit0+0.1)) !Hysteresis of 0.1 bar pressure preventing signal bouncing
			AirbearingPressure_OK = 1 !Airbearing Pressure OK
			IF (P_FAULT &AirbearingPressure_ErrorCode) =  AirbearingPressure_ErrorCode !Check if Error occurred previously
					P_FAULT = P_FAULT-AirbearingPressure_ErrorCode !Clear AirbearingPressure_Errorcode
			END	
		END
		IF (AirbearingPressureSensor < PressureLimit0)
			AirbearingPressure_OK = 0	!Airbearing Pressure  NOK
			IF (ACS_Stat_Airbearing=1) !Airbearing is initialized 
				IF (P_FAULT & AirbearingPressure_ErrorCode) <>  AirbearingPressure_ErrorCode !Check if Error occurs first time
					P_FAULT = P_FAULT+AirbearingPressure_ErrorCode !Add Errorcode
					DISP "Airbearing Pressure Not OK"
				END	
			END
		END

		
		IF (AirbearingVacuumSensor0 < (VacuumSensor0Limit-0.025))!Hysteresis of 0.025 bar vacuum preventing signal bouncing
			AirbearingVacuum0_OK = 1 !Vacuum Airbearing0 OK
			IF (P_FAULT & AirbearingVacuum0_ErrorCode) =  AirbearingVacuum0_ErrorCode !Check if Error occurred previously
				P_FAULT = P_FAULT-AirbearingVacuum0_ErrorCode !Clear AirbearingVacuum0_Errorcode
			END	
		END
		IF (AirbearingVacuumSensor0 > VacuumSensor0Limit)
			AirbearingVacuum0_OK = 0 !Vacuum Airbearing0 NOK
			IF (ACS_Stat_Airbearing=1) !Airbearing is initialized 
				IF (P_FAULT & AirbearingVacuum0_ErrorCode) <>  AirbearingVacuum0_ErrorCode !Check if Error occurs first time
					P_FAULT = P_FAULT+AirbearingVacuum0_ErrorCode !Add Errorcode
					DISP "Vacuum Airbearing0 Not OK"
				END
			END
		END
		
		
		IF (AirbearingVacuumSensor1 < (VacuumSensor1Limit-0.025))!Hysteresis of 0.025 bar vacuum preventing signal bouncing
			AirbearingVacuum1_OK = 1 !Vacuum Airbearing1 OK
			IF (P_FAULT & AirbearingVacuum1_ErrorCode) =  AirbearingVacuum1_ErrorCode !Check if Error occurred previously
				P_FAULT = P_FAULT-AirbearingVacuum1_ErrorCode !Clear AirbearingVacuum1_Errorcode
			END		
		END
		IF (AirbearingVacuumSensor1 > VacuumSensor1Limit)
			AirbearingVacuum1_OK = 0 !Vacuum Airbearing1 NOK
			IF (ACS_Stat_Airbearing=1) !Airbearing is initialized 
				IF (P_FAULT & AirbearingVacuum1_ErrorCode) <>  AirbearingVacuum1_ErrorCode !Check if Error occurs first time
					P_FAULT = P_FAULT+AirbearingVacuum1_ErrorCode !Add Errorcode
					DISP "Vacuum Airbearing1 Not OK" 
				END	
			END
		END
		
		
		IF AirSupplyPresent = 1 !Air supply OK
			IF (P_FAULT & AirSupplyPresent_ErrorCode) = AirSupplyPresent_ErrorCode !Check if Error occurred previously
				P_FAULT = P_FAULT-AirSupplyPresent_ErrorCode !Clear AirSupplyPresent_ErrorCode	
			END
		END
		
		IF AirSupplyPresent = 0 !Air supply NOK
			IF (ACS_Stat_Airbearing=1) !Airbearing is initialized 
				IF (P_FAULT & AirSupplyPresent_ErrorCode) <>  AirSupplyPresent_ErrorCode !Check if Error occurs first time
					P_FAULT = P_FAULT+AirSupplyPresent_ErrorCode !Add Errorcode			
					DISP "Air supply Not OK"
				END
			END
		END
		
		
		IF VacuumSupplyPresent = 1 !Vacuum supply OK
			IF (P_FAULT & VacuumSupplyPresent_ErrorCode) =  VacuumSupplyPresent_ErrorCode !Check if Error occurred previously
				P_FAULT = P_FAULT-VacuumSupplyPresent_ErrorCode !Clear VacuumSupplyPresent_Error code
			END
		END
		IF VacuumSupplyPresent = 0 !Vacuum supply NOK
			IF (ACS_Stat_Airbearing=1) !Airbearing is initialized 
				IF (P_FAULT & VacuumSupplyPresent_ErrorCode) <>  VacuumSupplyPresent_ErrorCode !Check if Error occurs first time
					P_FAULT = P_FAULT+VacuumSupplyPresent_ErrorCode !Add Errorcode			
					DISP "Vacuum supply Not OK"
				END
			END
		END
		

			
		IF PowerOK_24V&PowerOK_5V&PowerOK_48V_OH_focus&PowerOK_48V_WaferStage&PowerOK_24V_Aux=1
			Power_OK = 1 !Power Supply OK
			IF (P_FAULT & Power_ErrorCode) = Power_ErrorCode !Check if Error occurred previously
				P_FAULT = P_FAULT-Power_ErrorCode !Clear Power_Error code
			END
		ELSE
			Power_OK=0 !Power Supply NOK
			IF (P_FAULT & Power_ErrorCode) <>  Power_ErrorCode !Check if Error occurs first time
				P_FAULT = P_FAULT+Power_ErrorCode !Add Errorcode
				DISP "Power Supply not OK"
			END
		END			
		
		IF  softEmergency_Stop=1
			!Clear Reset (Preventing sEmergency_Stop reamaining active)
			IF (P_FAULT & softEmergencyStop_ErrorCode) <>  softEmergencyStop_ErrorCode !Check if Error occurs first time
				P_FAULT = P_FAULT+softEmergencyStop_ErrorCode !Add Errorcode
				DISP "Software Emergency Stop"
			END
		ELSE
			IF (P_FAULT & softEmergencyStop_ErrorCode) = softEmergencyStop_ErrorCode !Check if Error occurred previously
				P_FAULT = P_FAULT-softEmergencyStop_ErrorCode !Clear EmergencyStop_ErrorCode
			END
		END
		
		IF (MotorTemperatureY0	 < MotorTemperatureY0_Limit)
			MotorTemperatureY0_OK = 1 !Motor temperature is OK
			IF (P_FAULT & MotorTemperatureY0_ErrorCode) =  MotorTemperatureY0_ErrorCode !Check if Error occurred previously
				P_FAULT = P_FAULT-MotorTemperatureY0_ErrorCode !Clear AirbearingVacuum1_Errorcode
			END		
		ELSE
			MotorTemperatureY0_OK = 0 !Motor temperature is NOK
			IF (P_FAULT & MotorTemperatureY0_ErrorCode) <>  MotorTemperatureY0_ErrorCode !Check if Error occurs first time
				P_FAULT = P_FAULT+MotorTemperatureY0_ErrorCode !Add Errorcode
				DISP "Motor temperature Y0 Not OK" 
			END			
		END
		
		IF (MotorTemperatureY1	 < MotorTemperatureY1_Limit)
			MotorTemperatureY1_OK = 1 !Motor temperature is OK
			IF (P_FAULT & MotorTemperatureY1_ErrorCode) =  MotorTemperatureY1_ErrorCode !Check if Error occurred previously
				P_FAULT = P_FAULT-MotorTemperatureY1_ErrorCode !Clear AirbearingVacuum1_Errorcode
			END		
		ELSE
			MotorTemperatureY1 = 0 !Motor temperature is NOK
			IF (P_FAULT & MotorTemperatureY1_ErrorCode) <>  MotorTemperatureY1_ErrorCode !Check if Error occurs first time
				P_FAULT = P_FAULT+MotorTemperatureY1_ErrorCode !Add Errorcode
				DISP "Motor temperature Y1 Not OK" 
			END			
		END
		
		IF (MotorTemperatureX	 < MotorTemperatureX_Limit)
			MotorTemperatureX_OK = 1 !Motor temperature is OK
			IF (P_FAULT & MotorTemperatureX_ErrorCode) =  MotorTemperatureX_ErrorCode !Check if Error occurred previously
				P_FAULT = P_FAULT-MotorTemperatureX_ErrorCode !Clear AirbearingVacuum1_Errorcode
			END		
		ELSE
			MotorTemperatureX_OK = 0 !Motor temperature is NOK
			IF (P_FAULT & MotorTemperatureX_ErrorCode) <>  MotorTemperatureX_ErrorCode !Check if Error occurs first time
				P_FAULT = P_FAULT+MotorTemperatureX_ErrorCode !Add Errorcode
				DISP "Motor temperature X Not OK" 
			END			
		END
		
		
		IF WaferClamp=1! & WaferClamp valve is actuated 
			IF prevWaferClamp=0
				BeginTime=TIME
				prevWaferClamp=1
			END

			If TIME > BeginTime+4000 !wait first time 4 sec before checking WaferClamp sensor because vacuum has to build up				
				IF WaferClamp_sensor = 1
					WaferClamp_OK = 1					
				ELSE
					WaferClamp_OK = 0					
				END	
			END
		END
		
		IF WaferClamp=0! & WaferClamp valve is not actuated 
			WaferClamp_OK = 1
			prevWaferClamp=0				
		END
		
		
		IF WaferClamp_OK = 1
			IF (P_FAULT & WaferClamp_ErrorCode) = WaferClamp_ErrorCode !Check if Error occurred previously
				P_FAULT = P_FAULT-WaferClamp_ErrorCode !Clear WaferPresentOK_Errorcode	
			END
		ELSE
			IF (P_FAULT & WaferClamp_ErrorCode) <>  WaferClamp_ErrorCode !Check if Error occurs first time
				P_FAULT = P_FAULT+WaferClamp_ErrorCode !Add Errorcode
				DISP "WaferClamp Not OK" 
			END
		END
		
		!Store the last fault since a ResetFAULT
		IF (P_FAULT > LastFAULT)
			LastFAULT = P_FAULT
		END
		
	END	
	
	
	BLOCK ! Feedforward calculations
		!Kfst				!Static feedforward
		!Kfv				!Velocity feedforward (viscous friction) 
		!Kfc				!colomb feedforward (coulomb friction) 
		!SLAFF(Gantry) = default acceleration feedforward in ACS controller
		
		IF ACS_Stat_LOH_focus=1	!Lower Optical Head is initialized
			DCOM(AxisLOH_focus)=AxisLOH_focus_Kfst+AxisLOH_focus_Kfv*RVEL(AxisLOH_focus)+AxisLOH_focus_Kfc*sign(RVEL(AxisLOH_focus)) 
			!DCOM is INT [-100%,100%]==[equals with DOUT -->-32768...32768]==[-5Apk...5Apk]==[-3.54Arms...3.54Arms]
			
		END
		
		IF ACS_Stat_UOH_focus=1 !Upper Optical Head is initialized
			DCOM(AxisUOH_focus)=AxisUOH_focus_Kfst+AxisUOH_focus_Kfv*RVEL(AxisUOH_focus)+AxisUOH_focus_Kfc*sign(RVEL(AxisUOH_focus)) 
			!DCOM is INT [-100%,100%]==[equals with DOUT -->-32768...32768]==[-5Apk...5Apk]==[-3.54Arms...3.54Arms]
		END
		
		IF (ACS_Stat_WaferStage=1) !WaferStage stage is initialized
			DCOM(Gantry_Y)=Gantry_Y_Kfst+Gantry_Y_Kfv*RVEL(Gantry_Y)+Gantry_Y_Kfc*sign(RVEL(Gantry_Y)) 
			!DCOM is INT [-100%,100%]==[equals with DOUT -->-32768...32768]==[-10Apk...10Apk]==[-7.07Arms...7.07Arms]
		END
	END
	
END !End of While Loop

STOP

!-------------------------------------------------------------------------------------

!End of Program



















#2
!Safety and errorhandling
real BeginTime2
!TimeOut = 5000 !TimeOut = 10 sec as used in swEmergencyStop Interupt Routine

STOP


On (ResetFAULT = 1) !Reset Error and Enable axis if initialized
	softEmergency_Stop=0 ! Clear EmergencyStop variable
	
	STOP	Buffer_Process		!3: Process functions
	STOP	Buffer_WaferStage	!4: Initialization of WaferStage
	STOP	Buffer_AxisLOH_focus!5:	Initialization of AxisLOH_focus
	STOP	Buffer_AxisUOH_focus!6:	Initialization of AxisUOH_focus	
	
	IF	(P_FAULT = 0)&(S_FAULT=0) !On clearing FAULT, axes will be enabled again
		FCLEAR ALL !Reset MotionError and clear ACS-MERR variable
		DISP "Reset Error-State"
		LastFAULT=0 ! Reset LastFault status
			
		IF ACS_Stat_LOH_focus=1
			ENABLE AxisLOH_focus
			TILL (MST(AxisLOH_focus).#ENABLED)
			WAIT 1000
			IF (MST(AxisLOH_focus).#ENABLED)
				PTP/ve AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
			ELSE
				DISP "Error-State does not reset"
				GOTO EndOfProgram !Reset did not succeed
			END
		END
		IF ACS_Stat_UOH_focus=1
			ENABLE AxisUOH_focus
			TILL (MST(AxisUOH_focus).#ENABLED)
			WAIT 1000
			IF (MST(AxisUOH_focus).#ENABLED)
				PTP/ve AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
			ELSE
				DISP "Error-State does not reset"
				GOTO EndOfProgram !Reset did not succeed
			END
		END
		IF (ACS_Stat_WaferStage=1)
			WAIT 500
			ENABLE(Gantry_Y,Gantry_Yaw,AxisX)
			WAIT 500
			TILL ((MST(Gantry_Y).#ENABLED)&(MST(Gantry_Yaw).#ENABLED)&(MST(AxisX).#ENABLED)),TimeOut
			IF ((MST(Gantry_Y).#ENABLED)&(MST(Gantry_Yaw).#ENABLED)&(MST(AxisX).#ENABLED))
				PTP/e (Gantry_Yaw),0 !Set the Gantry straight	
			ELSE
				DISP "Error-State does not reset"
				GOTO EndOfProgram !Reset did not succeed
			END
		END	
		LandAirbearing = 0 !== always Lift Airbearing at Reset 
		MachineOK=1 !Machine status = ok
		
	ELSE
		DISP "Error-State does not reset"
	END
	
	EndOfProgram:
	ResetFAULT = 0 !Clear Reset (Preventing ResetFAULT reamaining active)



RET
	

! ----------------System Fauls for all axis------------------
ON S_FAULT.#TIME
	DISP "MPU overuse\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"

RET


ON S_FAULT.#ES
	DISP "Emergency Stop\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	!TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut 
	!Waiting for a standstill does not work because of the STO which takes action before motion has finished
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"

RET


ON LOCALST(Gantry_Y).#STO1=1 !To be tested if STO is triggered in sw
	DISP "Safe Torque Off\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	!TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut 
	!Waiting for a standstill does not work because of the STO which takes action before motion has finished
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"

RET

ON S_FAULT.#INT
	DISP "Servo Interupt\r"
	!All axis will be disabled by the controller
RET

ON S_FAULT.#INTGR
	DISP "File Integrity\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON S_FAULT.#FAILURE
	DISP "Component Failure of MC4U\r"
	!Not applicable because no MC4U is used
RET

! ----------------Gantry_Y--------------------------

ON FAULT(Gantry_Y).#NT
	DISP "Network error Gantry Y\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(Gantry_Y).#HOT
	DISP "Temperature too high Gantry Y\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(Gantry_Y).#ENCNC
	DISP "Encoder not connected Gantry Y\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(Gantry_Y).#ENC2NC
	DISP "Encoder2 not connected Gantry Y\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(Gantry_Y).#ENC
	DISP "Encoder Error Gantry Y\r"
	!KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	!Controller will disable the axis (non configurable) therefore a kill is not possible
	KILL(Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(Gantry_Y).#ENC2
	DISP "Encoder2 Error Gantry Y\r"
	!KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	!Controller will disable the axis (non configurable)
	KILL(Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(Gantry_Y).#DRIVE
	DISP "Drive Fault Gantry Y\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(Gantry_Y).#PE
	DISP "Position Error Gantry Y\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(Gantry_Y).#CPE
	DISP "Critical Position Error Gantry Y\r"
	!KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	!Controller will disable the axis (non configurable) therefore a kill is not possible
	KILL(Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(Gantry_Y).#VL
	DISP "Velocity limit exceeded Gantry Y\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(Gantry_Y).#AL
	DISP "Accleration limit exceeded Gantry Y\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(Gantry_Y).#CL
	DISP "Current limit exceeded Gantry Y\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(Gantry_Y).#SP
	DISP "Servo Processor Alarm Gantry Y\r"
	!KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	!Controller will disable the axis (non configurable) therefore a kill is not possible
	KILL(Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

! ----------------AxisX--------------------------

ON FAULT(AxisX).#NT
	DISP "Network error AxisX\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(AxisX).#HOT
	DISP "Temperature too high AxisX\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(AxisX).#ENCNC
	DISP "Encoder not connected AxisX\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(AxisX).#ENC2NC
	DISP "Encoder2 not connected AxisX\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(AxisX).#ENC
	DISP "Encoder Error AxisX\r"
	!KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	!Controller will disable the axis (non configurable) therefore a kill is not possible
	KILL(Gantry_Y,Gantry_Yaw,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(AxisX).#ENC2
	DISP "Encoder2 Error AxisX\r"
	!KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	!Controller will disable the axis (non configurable) therefore a kill is not possible
	KILL(Gantry_Y,Gantry_Yaw,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(AxisX).#DRIVE
	DISP "Drive Fault AxisX\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(AxisX).#PE
	DISP "Position Error AxisX\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(AxisX).#CPE
	DISP "Critical Position Error AxisX\r"
	!KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	!Controller will disable the axis (non configurable) therefore a kill is not possible
	KILL(Gantry_Y,Gantry_Yaw,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(AxisX).#VL
	DISP "Velocity limit exceeded AxisX\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(AxisX).#AL
	DISP "Accleration limit exceeded AxisX\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(AxisX).#CL
	DISP "Current limit exceeded AxisX\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(AxisX).#SP
	DISP "Servo Processor Alarm AxisX\r"
	!KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	!Controller will disable the axis (non configurable) therefore a kill is not possible
	KILL(Gantry_Y,Gantry_Yaw,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

! ----------------Gantry_Yaw--------------------------

ON FAULT(Gantry_Yaw).#NT
	DISP "Network error Gantry Yaw\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(Gantry_Yaw).#HOT
	DISP "Temperature too high Gantry_Yaw\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET


ON FAULT(Gantry_Yaw).#ENCNC
	DISP "Encoder not connected Gantry_Yaw\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(Gantry_Yaw).#ENC2NC
	DISP "Encoder2 not connected Gantry_Yaw\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(Gantry_Yaw).#ENC
	DISP "Encoder Error Gantry_Yaw\r"
	!KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	!Controller will disable the axis (non configurable) therefore a kill is not possible
	KILL(Gantry_Y,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(Gantry_Yaw).#ENC2
	DISP "Encoder2 Error Gantry_Yaw\r"
	!KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	!Controller will disable the axis (non configurable) therefore a kill is not possible
	KILL(Gantry_Y,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(Gantry_Yaw).#DRIVE
	DISP "Drive Fault Gantry_Yaw\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(Gantry_Yaw).#PE
	DISP "Position Error Gantry_Yaw\r",PE(Gantry_Yaw)
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(Gantry_Yaw).#CPE
	DISP "Critical Position Error Gantry_Yaw\r"
	!KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	!Controller will disable the axis (non configurable) therefore a kill is not possible
	KILL(Gantry_Y,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(Gantry_Yaw).#VL
	DISP "Velocity limit exceeded Gantry_Yaw\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(Gantry_Yaw).#AL
	DISP "Accleration limit exceeded Gantry_Yaw\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(Gantry_Yaw).#CL
	DISP "Current limit exceeded Gantry_Yaw\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(Gantry_Yaw).#SP
	DISP "Servo Processor Alarm Gantry_Yaw\r"
	!KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	!Controller will disable the axis (non configurable) therefore a kill is not possible
	KILL(Gantry_Y,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

! ----------------AxisUOH_focus--------------------------
!Retraction of the OH axis while an error occurs on the correspoding axis is not possible

ON FAULT(AxisUOH_focus).#NT
	DISP "Network error AxisUOH_focus\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(AxisUOH_focus).#HOT
	DISP "Temperature too high AxisUOH_focus\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(AxisUOH_focus).#ENCNC
	DISP "Encoder not connected AxisUOH_focus\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(AxisUOH_focus).#ENC2NC
	DISP "Encoder2 not connected AxisUOH_focus\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(AxisUOH_focus).#DRIVE
	DISP "Drive Fault AxisUOH_focus\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(AxisUOH_focus).#ENC!Fixed errorhandler at ACS
	DISP "Encoder Error AxisUOH_focus\r"
	!KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	!Controller will disable the axis (non configurable) therefore a kill is not possible
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(AxisUOH_focus).#ENC2!Fixed errorhandler at ACS 
	DISP "Encoder2 Error AxisUOH_focus\r"
	!KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	!Controller will disable the axis (non configurable) therefore a kill is not possible
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(AxisUOH_focus).#PE
	DISP "Position Error AxisUOH_focus\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(AxisUOH_focus).#CPE !Fixed errorhandler at ACS 
	DISP "Critical Position Error AxisUOH_focus\r"
	!KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	!Controller will disable the axis (non configurable) therefore a kill is not possible
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(AxisUOH_focus).#VL
	DISP "Velocity limit exceeded AxisUOH_focus\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(AxisUOH_focus).#AL
	DISP "Accleration limit exceeded AxisUOH_focus\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(AxisUOH_focus).#CL !Fixed errorhandler at ACS
	DISP "Current limit exceeded AxisUOH_focus\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(AxisUOH_focus).#SP!Fixed errorhandler at ACS 
	DISP "Servo Processor Alarm AxisUOH_focus\r"
	!KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	!Controller will disable the axis (non configurable) therefore a kill is not possible
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET


! ----------------AxisLOH_focus--------------------------
!Retraction of the OH axis while an error occurs on the correspoding axis is not possible

ON FAULT(AxisLOH_focus).#NT
	DISP "Network error AxisLOH_focus\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(AxisLOH_focus).#HOT
	DISP "Temperature too high AxisLOH_focus\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(AxisLOH_focus).#ENCNC
	DISP "Encoder not connected AxisLOH_focus\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(AxisLOH_focus).#ENC2NC
	DISP "Encoder2 not connected AxisLOH_focus\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(AxisLOH_focus).#DRIVE
	DISP "Drive Fault AxisLOH_focus\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(AxisLOH_focus).#ENC!Fixed errorhandler at ACS, 
	DISP "Encoder Error AxisLOH_focus\r"
	!KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	!Controller will disable the axis (non configurable) therefore a kill is not possible
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(AxisLOH_focus).#ENC2!Fixed errorhandler at ACS, 
	DISP "Encoder2 Error AxisLOH_focus\r"
	!KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	!Controller will disable the axis (non configurable) therefore a kill is not possible
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(AxisLOH_focus).#PE
	DISP "Position Error AxisLOH_focus\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(AxisLOH_focus).#CPE !Fixed errorhandler at ACS, 
	DISP "Critical Position Error AxisLOH_focus\r"
	!KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	!Controller will disable the axis (non configurable) therefore a kill is not possible
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(AxisLOH_focus).#VL
	DISP "Velocity limit exceeded AxisLOH_focus\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(AxisLOH_focus).#AL
	DISP "Accleration limit exceeded AxisLOH_focus\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(AxisLOH_focus).#CL !Fixed errorhandler at ACS, 
	DISP "Current limit exceeded AxisLOH_focus\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(AxisLOH_focus).#SP!Fixed errorhandler in ACS, 
	DISP "Servo Processor Alarm AxisLOH_focus\r"
	!KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	!Controller will disable the axis (non configurable) therefore a kill is not possible
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON USAGE > 80 ! If MPU Processor Load is above 80% then stop application
	DISP "MPU processor load above 80%\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET


On (WaferClamp = 1)
	!Set WaferClampDigOut
	WaferClampDigOut=1
	wait 1000 !Pulse time of WaferStage Lifting
	WaferClampDigOut=0
RET
	
On (WaferClamp = 0)
	!Set WaferClampDigOut
	nWaferClampDigOut=1
	wait 1000 !Pulse time of WaferStage Lifting
	nWaferClampDigOut=0
RET	

On (ServiceLight = 1)
	!Set ServiceLightOn
	DigOut14=1
	DigOut15=1
RET	

On (ServiceLight = 0)
	!Set ServiceLightOff
	DigOut14=0
	DigOut15=0
RET	

!If porgram buffer error occurs then corresponding buffer stops executing and clears error
ON PERR(Buffer_AutoExe)
	DISP "Program Error in Buffer 1 occurred\r"
	STOP Buffer_AutoExe
	MachineOK=0 !Machine status = nok
RET

ON PERR(Buffer_SafetyErrorhandling)
	DISP "Program Error in Buffer 2 occurred\r"
	STOP Buffer_SafetyErrorhandling
	ResetFAULT = 0 !Clear Reset (Preventing ResetFAULT reamaining active)
RET

ON PERR(Buffer_Process )
	DISP "Program Error in Buffer 3 occurred\r"
	STOP Buffer_Process
RET

ON PERR(Buffer_WaferStage )
	DISP "Program Error in Buffer 4 occurred\r"
	STOP Buffer_WaferStage
RET

ON PERR( Buffer_AxisLOH_focus)
	DISP "Program Error in Buffer 5 occurred\r"
	STOP Buffer_AxisLOH_focus
RET

ON PERR( Buffer_AxisUOH_focus)
	DISP "Program Error in Buffer 6  occurred\r"
	STOP Buffer_AxisUOH_focus
RET

ON PERR( Buffer_ServiceProgram)
	DISP "Program Error in Buffer 10 occurred\r"
	STOP Buffer_ServiceProgram
RET

ON PERR(Buffer_EFEM )
	DISP "Program Error in Buffer 11 occurred\r"
	STOP Buffer_EFEM
RET

ON PERR(Buffer_LandAirbearing )
	DISP "Program Error in Buffer 12 occurred\r"
	STOP Buffer_LandAirbearing
RET












#3
!Initialization of Process functions

! Process is only intialized if following conditions are met
!If needed set the necessary IO
!DigOutx = 1
!DigOutx+1=1

real BeginTime3
real TimeOut3

real BeginTime3a
real TimeOut3a

BeginTime3=TIME
TimeOut3 = 10000 !in ms


!Check if Power and Pressures are ok
while ((  (Power_OK = 0) |(AirSupplyPresent= 0)|(VacuumSupplyPresent= 0) ) & (TIME < (BeginTime3+TimeOut3)))
	
		ACS_Stat_Process=0 !Process functions is not initialized				

end

if  (Power_OK = 1) &(AirSupplyPresent= 1)& (VacuumSupplyPresent= 1)
		ACS_Stat_Process=1 !Process functions initialized		
		DISP "Process functions initialized"
else
		ACS_Stat_Process=0 !Process functions not initialized	
		DISP "Process functions not initialized"
		DISP "TimeOut Process initialization"
end

goto EndOfProgram



!Initialization of Airbearing functions
Init_Airbearing:

AirbearingPressureControl=4.5 !Set Pressure of Airbearing
AirbearingVacuumControl0=-0.5 !Set Vacuum of Airbearing
AirbearingVacuumControl1=-0.45 !Set Vacuum of Airbearing


StageLift=1 !Extract WaferStage lifter actuator

BeginTime3a=TIME
WAIT 750

TimeOut3a = 120000 !in ms

while ((  (AirbearingVacuum0_OK = 0) | (AirbearingVacuum1_OK = 0) | (AirbearingPressure_OK = 0 )  ) &  (TIME < (BeginTime3a+TimeOut3a)) )
		
		ACS_Stat_Airbearing=0 !Airbearing is not initialized
end 

! Airbearing is initialized or TimeOut is expired

if (AirbearingVacuum0_OK = 1 & AirbearingVacuum1_OK = 1   & AirbearingPressure_OK = 1 )
		ACS_Stat_Airbearing=1 !Airbearing is initialized
		DISP "Airbearing is initialized"
else
	ACS_Stat_Airbearing=0 !Airbearing is not initialized
	DISP "Airbearing not initialized"
	DISP "TimeOut airbearing"
end

StageLift=0 !Retract WaferStage lifter actuator

EndOfProgram:
ENABLEON Buffer_Process !Enable the Interuptroutine in this Buffer --> P_FAULT

STOP

ON ((P_FAULT <> 0)|(S_FAULT <> 0)) !in case MachineError occurs, lift airbearing 
	!Pressurize and lift Airbearing 
	DISP "Machine error occurred."
	LandAirbearing = 0 !== Lift Airbearing  
	MachineOK=0
RET


ON P_FAULT <> 0
	! Process Error
	IF (ACS_Stat_Airbearing=1) !Airbearing is initialized 
		DISP "Proces Fault"
		!Errorhandler in case a Process Fault occurs.
		! A Reset_Fault is needed to clear this ErrorState.
		KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
		DISP "Gantry and OH axis KILLED"
		TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
		IF MST(AxisLOH_focus).#ENABLED
			PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
			DISP "Retract LOH Focus axis"
		END
		IF MST(AxisUOH_focus).#ENABLED
			 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
			 DISP "Retract UOH Focus axis"
		END
		TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
		WAIT 1000	!Wait till Gantry is at rest
		DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
		DISP "Gantry Axis Disabled"
		TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
		DISABLE (AxisLOH_focus,AxisUOH_focus)
		DISP "Focus Axis Disabled"
	END
		
RET



















#4
!Initialization of Gantry Stage --> Becomes program in Buffer 4
DISABLEON Buffer_WaferStage ! Disable additional Position Limit Interupt routines of Waferstage axes (YYX)
ENABLEON Buffer_EFEM !Enable safety monitoring of EFEM-robot arm extraction before the Gantry stage starts initializing

DISABLE(AxisY0,AxisY1,AxisX)

MFLAGS(AxisY0).#GANTRY=0  !Set motor in NON-Gantry mode
MFLAGS(AxisY1).#GANTRY=0  !Set motor in NON-Gantry mode

!Axis are in same errorhandler group. If one axis is triggered by a Fault, the other axis will respond also
!UnGroup axes by following:
SAFETYGROUP (AxisY0) !Ungroup by defining a group with the first axis in the previous group 

! Disable the default response of the hardware limits
FDEF(AxisY0).#LL=0; FDEF(AxisY0).#RL=0
FDEF(AxisY1).#LL=0; FDEF(AxisY1).#RL=0 
FDEF(AxisX).#LL=0;  FDEF(AxisX).#RL=0 
! Disable the default response of the hardware limits

!Disable SW limit switches
FMASK(AxisY0).#SRL=0; FMASK(AxisY0).#SLL=0
FMASK(AxisY1).#SRL=0; FMASK(AxisY1).#SLL=0
FMASK(AxisX).#SRL=0;  FMASK(AxisX).#SLL=0
!Disable SW limit switches

!Initial settings of the Y axes when it is NOT in Gantry
MFLAGS(AxisY0).#NOTCH	=	1;				MFLAGS(AxisY1).#NOTCH	=	1 
MFLAGS(AxisY0).#NOFILT	=	0;				MFLAGS(AxisY1).#NOFILT	=	0
MFLAGS(AxisY0).#BI_QUAD	=	0;				MFLAGS(AxisY1).#BI_QUAD	=	0
MFLAGS(AxisY0).#BI_QUAD1=	0;				MFLAGS(AxisY1).#BI_QUAD1=	0
!SLPKP(AxisY0)	=	50						SLPKP(AxisY1)	=	5
SLPKP(AxisY0)	=	5						
SLPKP(AxisY1)	=	5
SLPKI(AxisY0)	=	0						
SLPKI(AxisY1)	=	0
SLVKP(AxisY0)	=	26.28;					SLVKP(AxisY1)	=	50
SLVKI(AxisY0)	=	772;					SLVKI(AxisY1)	=	463
XVEL(AxisY0)	=	650;					XVEL(AxisY1)	=	650
XACC(AxisY0)	=	5500;					XACC(AxisY1)	=	5500
SLVSOF(AxisY0)	=	450						SLVSOF(AxisY1)	=	450
SLVSOFD(AxisY0)	=	7.07E-01;				SLVSOFD(AxisY1)	=	7.07E-01
SLVNFRQ(AxisY0)	=	5.00E+02;				SLVNFRQ(AxisY1)	=	5.00E+02
SLVNWID(AxisY0)	=	50;						SLVNWID(AxisY1)	=	50
SLVNATT(AxisY0)	=	5;						SLVNATT(AxisY1)	=	5
SLIKP(AxisY0) = 2.500000E+2;				SLIKP(AxisY1) = 2.500000E+2;
SLIKI(AxisY0) = 1.000000E+4;				SLIKI(AxisY1) = 1.000000E+4;	
SLAFF(AxisY0) =		0;						SLAFF(AxisY1)	=	0


CERRA(AxisY0)	= 2			!Critical Position Error during acceleration  
CERRI(AxisY0)	= 1			!Critical Position Error during idle  
CERRV(AxisY0)	= 1			!Critical Position Error during constant velocity
ERRA(Gantry_Y)	= 1			!Position Error during acceleration
ERRI(Gantry_Y)	= 1			!Position Error during idle
ERRV(Gantry_Y)	= 1			!Position Error during constant velocity

CERRA(AxisY1)	= 2			!Critical Position Error during acceleration  
CERRI(AxisY1)	= 1			!Critical Position Error during idle 
CERRV(AxisY1)	= 1			!Critical Position Error during constant velocity
ERRA(AxisY1)	= 1			!Position Error during acceleration
ERRI(AxisY1)	= 1			!Position Error during idle
ERRV(AxisY1)	= 1			!Position Error during constant velocity

!Limit settings 
!XCURV = 100% x (max peak motor current)/ (max peak drive current) 
!max peak motor current = 10Arms (Technotion UM06S)
!max peak drive current = 10Atop sinus = 7.07 Arms
!XCURV =100 * 10/7.07 == > 100% 
XCURV(AxisY0)	=	100;  	!Limit Maximum Output during motion of AxisY0: 
XCURI(AxisY0)	=	50;		!Limit Maximum Output during IDLE of AxisY0 
XCURV(AxisY1)	=	100;  	!Limit Maximum Output during motion of AxisY1
XCURI(AxisY1)	=	50;		!Limit Maximum Output during IDLE of AxisY1 


!XRMS = (nominal motor current) / (peak drive current)
!max nominal motor current = 2.9Arms (Technotion UM06S)
!max peak drive current = 10Atop sinus = 7.07 Arms

!Set maximum continous current to 1.4 Arms
!XRMS = 100 x 1.4 Arms / 7.07Arms =20%


XRMS(AxisY0)	=	20 		!max current during Homing
XRMS(AxisY1)	=	20 		!max current during Homing


READ Skew_Offset !Read Skew_Offset from flash memory (User Variable)
READ AxisY_DeltaHomingOffset !Read deltaOffset from flash memory (User Variable)
READ AxisX_DeltaHomingOffset !Read deltaOffset from flash memory (User Variable)

AxisY0_HomingOffset=156.009	+  AxisY_DeltaHomingOffset(0)	!Homing Offset
!Range = [-150 , 175]
AxisY1_HomingOffset=156.009	+  AxisY_DeltaHomingOffset(0)	!Homing Offset
!Skew_Offset = 0.89				!Skew-Offset M#1
!Skew_Offset = 1.09				!Skew-Offset M#2
!Skew_Offset = -0.3751			!Skew-Offset M#3
!Skew_Offset = 0.4499			!Skew-Offset M#3
!CALL Set_Skew !Go to sub routine at which Skew Offset is defined

!Y-Range = [-150 , 175]
AxisX_HomingOffset=152.9815 + AxisX_DeltaHomingOffset(0)	!Homing Offset
!X-Range = [-150 , 215]


!**********************commutatation***********************
DISABLE (AxisY0,AxisY1,AxisX);  

!JUST FOR TESTING ALWAYS COMMUTATE BY MEANS OF FLAG = 0 (NOT COMMUTATED)
!MFLAGS(AxisY0).#BRUSHOK=0
!MFLAGS(AxisY1).#BRUSHOK=0


IF ^MFLAGS(AxisY0).#BRUSHOK ! If motor is NOT commutated
	ENABLE (AxisY0);  
	COMMUT AxisY0,20  !20% = 1.4A
	TILL MFLAGS(AxisY0).#BRUSHOK !wait till Commutation is succeeded
	DISP "commutation Y0 succeeded"
	wait 50
	DISABLE (AxisY0); 
END

IF ^MFLAGS(AxisY1).#BRUSHOK ! If motor is NOT commutated
	ENABLE (AxisY1);  
	COMMUT AxisY1,20  !20% = 1.4A
	TILL MFLAGS(AxisY1).#BRUSHOK !wait till Commutation is succeeded
	DISP "commutation Y1 succeeded"
	wait 50
	DISABLE (AxisY1); 
END
!********************************************************************


!*************************Homing Y*************************************

!Homing parameters
VEL(AxisY0) = 20; ACC(AxisY0) =  30; DEC(AxisY0) =  100; JERK(AxisY0) =  1000 ; KDEC(AxisY0) = 1000;
VEL(AxisY1) = 20; ACC(AxisY1) =  30; DEC(AxisY1) =  100; JERK(AxisY1) =  1000 ; KDEC(AxisY1) = 1000;

! Enable the two axis
ENABLE (AxisY0); 
ENABLE (AxisY1); 


IST(AxisY0).#IND	=	0; 	! Prepare for finding indexes 
IST(AxisY1).#IND	=	0; 	! Prepare for finding indexes 
! Move to index  at normal speed using a JOG command
!VEL(AxisY0) = 20;
CALL HomingY
IST(AxisY0).#IND	=	0; 	! Prepare for finding indexes 
IST(AxisY1).#IND	=	0; 	! Prepare for finding indexes 
VEL(AxisY0) 		= 	2; 
VEL(AxisY1) 		= 	2;
! Move to index  at slow speed using a JOG command
CALL HomingY

! Enable the default response of the hardware limits
FDEF(AxisY0).#LL=1; FDEF(AxisY0).#RL=1                        
FDEF(AxisY1).#LL=1; FDEF(AxisY1).#RL=1                        

IF (IST(AxisY0).#IND=0)| (IST(AxisY1).#IND=0) !If Index not found
	DISP "Homing Y not succeeded"
	ACS_Stat_WaferStage=0
	DISABLE(AxisY0,AxisY1)
	GOTO EndOfProgram
ELSE
	DISP "Homing Y succeeded"
END
!******************************************************************************


!Determine Skew_Offset (Offset between Y0 and Y1) when Gantry is in rest
DISABLE(AxisY0,AxisY1)
TILL (^MST(AxisY0).#ENABLED)&(^MST(AxisY1).#ENABLED)
!Skew_Offset = IND(AxisY0)-IND(AxisY1)  !Determine the Offset between Y0 and Y1 in rest wrt Index position

! Set positions according to 2 index marks
! unlike SPii{lus Gantry the NT family handling HomeOffsets in the Gantry mode, so ZERROs may be define before switching to the Gnatry Mode
! Becarful with HomeOffsets on the position and the index puls, it is very easy to set your gantry under a angle and damage you system
! 
SET FPOS(AxisY0)=FPOS(AxisY0)-IND(AxisY0)-AxisY0_HomingOffset
SET FPOS(AxisY1)=FPOS(AxisY1)-IND(AxisY1)-AxisY1_HomingOffset+Skew_Offset(0)  !minus HomingOfsset due to Flipped Y direction


!******************************************************************************
!Set to Gantry mode and therefore axis names become from now on Gantry_Y and Gantry_Yaw
!******************************************************************************

Gantry_Y=AxisY0
Gantry_Yaw=AxisY1

MFLAGS(Gantry_Y).#GANTRY	=	1 	!Set AxisY0 into Gantry mode
MFLAGS(Gantry_Yaw).#GANTRY	=	1 	!Set AxisY1 into Gantry mode


!------------Control parameters Gantry_Y------------------------	

SLPKP(Gantry_Y)		=		40;		!Position Control Loop
SLPKI(Gantry_Y)		=		550;
SLPLI(Gantry_Y)		=		100;	


SLVKP(Gantry_Y)		=		350;	!Velocity Control Loop
SLVKI(Gantry_Y)		=		100;	! Proto 1 =350  Proto 2 =100;
!Additional settings (Settling and Iddle Factors for Kp and KI
SLVKPIF(Gantry_Y)	=		1		!Idle Factor to the SLVKP (Proportional Gain - Velocity) variable [0 100%]
SLVKPSF(Gantry_Y)	=		1		!Settle Factor to the SLVKP variable [0 100%]
SLVKISF(Gantry_Y)	=		5		!Settle Factor to the SLVLI variable [0 100%]
SLVKIIF(Gantry_Y)	=		1		!Idle Factor to the SLVLI (Integrator Gain - Velocity) variable [0 100%]


MFLAGS(Gantry_Y).#NOFILT=	0;		!LP filter DISable
SLVSOF(Gantry_Y)	=		100;
SLVSOFD(Gantry_Y) 	=		0.9;

MFLAGS(Gantry_Y).#NOTCH	=	1;		!Notch filter Enable
SLVNATT(Gantry_Y) 	=		15;
SLVNFRQ(Gantry_Y) 	=		150;	! Proto 1 = 183.33; Proto2 = 150
SLVNWID(Gantry_Y) 	=		30;

MFLAGS(Gantry_Y).#BI_QUAD=	1;		!BiQuad filter Enable
SLVB0DD(Gantry_Y)	=		0.7
SLVB0DF(Gantry_Y)	=		380
SLVB0ND(Gantry_Y)	=		0.25
SLVB0NF(Gantry_Y)			=560

MFLAGS(Gantry_Y).#BI_QUAD1=	1;		!BiQuad1 filter Enable
SLVB1DD(Gantry_Y)	=		0.5
SLVB1DF(Gantry_Y)	=		120
SLVB1ND(Gantry_Y)	=		0.5
SLVB1NF(Gantry_Y)	=		50

SLIKI(Gantry_Y) 	= 		1.000000E+4;	!Current control loop
SLIKP(Gantry_Y)		= 		2.500000E+2;	!Current control loop

SLAFF(Gantry_Y) 	=		1179.1/2	!Acceleration Feedforward
Gantry_Y_Kfst		=		0			!Static feedforward
Gantry_Y_Kfv		=		0			!Velocity feedforward (viscous friction)
Gantry_Y_Kfc		=		0!2.44		!colomb feedforward (coulomb friction)  == DOUT [800]==2.44%

!Disturbance Rejection Algorithm
SLDRA(Gantry_Y) 	=		40			!Should be set to the Open Loop crossover frequency	[Hz]
SLDRAIF(Gantry_Y)	=		1
SLDRX(Gantry_Y) 	=  		0.4			! This parameter stands for maximum DRA correction and specified in mm/sec.


!------------Control parameters Gantry_Yaw------------------------	
SLPKP(Gantry_Yaw)	=		50;			!Position Control Loop
SLPKI(Gantry_Yaw)	=		175;
SLPLI(Gantry_Yaw)	=		100;

SLVKP(Gantry_Yaw)	=		225;		!Velocity Control Loop
SLVKI(Gantry_Yaw)	=		10;

MFLAGS(Gantry_Yaw).#NOFILT=	0;			!LP filter DISable
SLVSOF(Gantry_Yaw)	=		350;
SLVSOFD(Gantry_Yaw) =		0.6;

MFLAGS(Gantry_Yaw).#NOTCH	=	1;		!Notch filter Enable
SLVNATT(Gantry_Yaw) =		15;	
SLVNFRQ(Gantry_Yaw) =		185;
SLVNWID(Gantry_Yaw) =		100;

MFLAGS(Gantry_Yaw).#BI_QUAD=	0;		!BiQuad filter Enable
SLVB0DD(Gantry_Yaw)	=		0.9
SLVB0DF(Gantry_Yaw)	=		400
SLVB0ND(Gantry_Yaw)	=		0.1
SLVB0NF(Gantry_Yaw)	=		400

MFLAGS(Gantry_Yaw).#BI_QUAD1=	0;		!BiQuad1 filter Enable
SLVB1DD(Gantry_Yaw)	=		0.9
SLVB1DF(Gantry_Yaw)	=		400
SLVB1ND(Gantry_Yaw)	=		0.1
SLVB1NF(Gantry_Yaw)	=		400

SLIKI(Gantry_Yaw) 	= 		1.000000E+4;	!Current control loop
SLIKP(Gantry_Yaw) 	= 		2.500000E+2;	!Current control loop

SLAFF(Gantry_Yaw)	=		90.092			!Acceleration Feedforward


!------------Limits safety parameters------------------------	

!DO NOT CHANGE THESE PARAMETERS IN GANTRY MODE OTHERWISE SYSTEM GETS INSTABLE!!!!!!!!!!!!!!!!!!!!!
!XVEL(AxisY0)	=	600
!XACC(AxisY0)	=	3000
!XVEL(AxisY1)	=	600
!XACC(AxisY1)	=	3000
 
 

!Total XCURV in gantry must be max 100% (e.g. Gantry_Y = 70 & Gantry_Yaw = 30)
XCURV(Gantry_Y)		=	60;  	!Limit Maximum Output during movingof Gantry_Y 
XCURI(Gantry_Y)		=	25;		!Limit Maximum Output during standstill of Gantry_Y 
XCURV(Gantry_Yaw)	=	40;  	!Limit Maximum Output during moving of Gantry_Yaw
XCURI(Gantry_Yaw)	=	20;		!Limit Maximum Output during standstill of Gantry_Yaw 

XRMS(Gantry_Y)		=	29		!XRMS is in Gantry mode still the maximum RMS current for the physical axes Y0 and Y1
XRMS(Gantry_Yaw)	=	29 		! it is not converted to Gantry_Y and Gantry_Yaw

!------------Normal moving parameters------------------------	

VEL(Gantry_Y) 		=	500 
ACC(Gantry_Y) 		=	4000 
DEC(Gantry_Y) 		=	4000 
JERK(Gantry_Y)	 	=	40000  
KDEC(Gantry_Y) 		=	4000
VEL(Gantry_Yaw) 	=	1
ACC(Gantry_Yaw) 	=	1 
DEC(Gantry_Yaw) 	=	1
JERK(Gantry_Yaw) 	=	1
KDEC(Gantry_Yaw) 	=	1
XACC(Gantry_Y)		=	4400;					
XACC(Gantry_Yaw)	=	1

!! SoftwareLimits for the axis in gantry mode
SRLIMIT_Gantry_Y 	= 	+183		!RightLimit of Gantry_y !-->Right and Left changed due to Flipped Y direction
SLLIMIT_Gantry_Y	= 	-158		!LeftLimit of Gantry_y  !--> Left and Right changed due to Flipped Y direction

SRLIMIT(Gantry_Y)	=	SRLIMIT_Gantry_Y! sw Limit switch 
SLLIMIT(Gantry_Y)	=	SLLIMIT_Gantry_Y ! sw Limit switch 

SRLIMIT(Gantry_Yaw)	=	2! sw Limit switch max Skew
SLLIMIT(Gantry_Yaw)	=	-2 ! sw Limit switch max Skew

!Enable SW limit switches                 
FDEF(Gantry_Y).#SRL		=	1; FDEF(Gantry_Y).#SLL	=	1;                     
FDEF(Gantry_Yaw).#SRL	=	1; FDEF(Gantry_Yaw).#SLL=	1; 
FMASK(Gantry_Y).#SRL	=	1
FMASK(Gantry_Y).#SLL	=	1
FMASK(Gantry_Yaw).#SRL	=	1
FMASK(Gantry_Yaw).#SLL	=	1
!Enable SW limit switches

!---------------Safety Limits & Error handling------------------------------------------------------------
CERRA(Gantry_Y)	= 1			!Critical Position Error during acceleration  
CERRI(Gantry_Y)	= 0.2			!Critical Position Error during idle  
CERRV(Gantry_Y)	= 0.5			!Critical Position Error during constant velocity
ERRA(Gantry_Y)	= 0.9			!Position Error during acceleration
ERRI(Gantry_Y)	= 0.1			!Position Error during idle
ERRV(Gantry_Y)	= 0.4			!Position Error during constant velocity

CERRA(Gantry_Yaw)	= 1			!Critical Position Error during acceleration  
CERRI(Gantry_Yaw)	= 0.5			!Critical Position Error during idle 
CERRV(Gantry_Yaw)	= 0.5			!Critical Position Error during constant velocity
ERRA(Gantry_Yaw)	= 0.5			!Position Error during acceleration
ERRI(Gantry_Yaw)	= 0.5			!Position Error during idle
ERRV(Gantry_Yaw)	= 0.5			!Position Error during constant velocity


MFLAGS(Gantry_Y).#BRAKE=0 		!No brake functionality
!BONTIME(Gantry_Y)=1000 		!Wait 1000 ms after a Disable of an axis to disable, meanwhile it blocks other motion commands
MFLAGS(Gantry_Yaw).#BRAKE=0	!No brake functionality
!BONTIME(Gantry_Yaw)=1000 		!Wait 1000 ms after a Disable of an axis to disable, meanwhile it blocks other motion commands


WAIT 5000
ENABLE(Gantry_Y,Gantry_Yaw)
WAIT 1000
TILL ((MST(Gantry_Y).#ENABLED)&(MST(Gantry_Yaw).#ENABLED))
PTP/e (Gantry_Yaw),0 !Set the Gantry straight


!--------------------X axis Commutation-----------------------------------------------------

IF ^MFLAGS(AxisX).#BRUSHOK ! If motor is NOT commutated
	ENABLE (AxisX);  
	COMMUT AxisX,20  !20% OR XRMS (MAX current of motor)
	TILL MFLAGS(AxisX).#BRUSHOK !wait till Commutation is succeeded
	DISP "commutation X succeeded"
	wait 50
	DISABLE (AxisX); 
END

!------------Control parameters AxisX------------------------	

SLPKP(AxisX)			=	100;		!Position Control Loop
SLPKI(AxisX)			=	250;
SLPLI(AxisX)			=	100;

SLVKP(AxisX)			=	275;		!Proto1 = 200! Proto2 = 250 !Velocity Control Loop
SLVKI(AxisX)			=	100;		!Proto1 = 350! Proto2=100

MFLAGS(AxisX).#NOFILT	=	0;			!LP filter DISable
SLVSOF(AxisX)			=	200;
SLVSOFD(AxisX) 			=	0.6;
MFLAGS(AxisX).#NOTCH	=	1;			!Notch filter Enable
SLVNATT(AxisX) 			=	10;	
SLVNFRQ(AxisX) 			=	150;
SLVNWID(AxisX) 			=	15;

MFLAGS(AxisX).#BI_QUAD	=	0;			!BiQuad filter Enable
SLVB0DD(AxisX)			=	0.707
SLVB0DF(AxisX)			=	400
SLVB0ND(AxisX)			=	0.707
SLVB0NF(AxisX)			=	400

MFLAGS(AxisX).#BI_QUAD1	=	0;			!BiQuad1 filter Enable
SLVB1DD(AxisX)			=	0.707
SLVB1DF(AxisX)			=	400
SLVB1ND(AxisX)			=	0.707
SLVB1NF(AxisX)			=	50

SLIKI(AxisX) 			= 	1.000000E+4;	!Current control loop
SLIKP(AxisX) 			= 	2.500000E+2;	!Current control loop

SLAFF(AxisX) 			=	1546.1		!Acceleration Feedforward
AxisX_Kfst				=	0			!Static feedforward
AxisX_Kfv				=	0			!Velocity feedforward (viscous friction)
AxisX_Kfc				=	0!2.44		!colomb feedforward (coulomb friction)  == DOUT [800]==2.44%

SRLIMIT_AxisX			=	223			!RightLimit of AxisX
SLLIMIT_AxisX			=	-158		!LeftLimit of AxisX

MFLAGS(AxisX).#BRAKE	=	0 			!No brake functionality
!BONTIME(AxisX)=1000 					!Wait 1000 ms after a Disable of an axis to disable, meanwhile it blocks other motion commands
CERRA(AxisX)			= 	1			!Critical Position Error during acceleration  
CERRI(AxisX)			= 	0.2			!Critical Position Error during idle  
CERRV(AxisX)			= 	0.5			!Critical Position Error during constant velocity
ERRA(AxisX)				= 	0.9			!Position Error during acceleration
ERRI(AxisX)				= 	0.1			!Position Error during idle
ERRV(AxisX)				= 	0.4			!Position Error during constant velocity

!XCURV = 100% x (max peak motor current)/ (max peak drive current) 
!max peak motor current = 10Arms (Technotion UM06S)
!max peak drive current = 10Atop sinus = 7.07 Arms
!XCURV =100 * 10/7.07 == > 100% 
XCURV(AxisX)			=	100  		!Limit Maximum Output during motion of AxisY1 
XCURI(AxisX)			=	50			!Limit Maximum Output during IDLE of AxisY1

!XRMS = (nominal motor current) / (peak drive current)
!max nominal motor current = 2.9Arms (Technotion UM06S)
!max peak drive current = 10Atop sinus = 7.07 Arms
!XRMS = 100 x 2.9A / 7.07A =41%

!Set maximum continous current to 1.4Arms
!XRMS = 100 x 1.4Arms / 7.07Arms =20%

XRMS(AxisX)	=	20 			!max current during homing

!Homing parameters
VEL(AxisX) = 20; ACC(AxisX) =  30; DEC(AxisX) =  100; JERK(AxisX) =  1000 ; KDEC(AxisX) = 1000;

!*************************Homing X *************************************
! Enable X-axis

ENABLE (AxisX); 

IST(AxisX).#IND	=	0; ! Prepare for finding indexes 
! Move to index  at normal speed using a JOG command
CALL HomingX
IST(AxisX).#IND	=	0; ! Prepare for finding indexes 
VEL(AxisX) = 2; 
! Move to index  at slow speed using a JOG command
CALL HomingX

! Enable the default response of the hardware limits
FDEF(AxisX).#LL=1; FDEF(AxisX).#RL=1                        

WAIT 750
SET FPOS(AxisX)=FPOS(AxisX)-IND(AxisX)-AxisX_HomingOffset

IF (IST(AxisX).#IND=0)| (IST(AxisX).#IND=0) !If Index not found
	DISP "Homing X not succeeded"
	ACS_Stat_WaferStage=0
	DISABLE(AxisX)
	GOTO EndOfProgram
ELSE
	DISP "Homing X succeeded"
	
END

!------------Normal moving parameters------------------------	

VEL(AxisX) 		=	500 
ACC(AxisX) 		=	4000 
DEC(AxisX) 		=	4000 
JERK(AxisX)	 	=	40000  
KDEC(AxisX) 	=	4000
XACC(AxisX)		=	4400;
XACC(AxisX)		=	4400;
SRLIMIT_AxisX	=	223				!RightLimit of AxisX
SLLIMIT_AxisX	=	-158			!LeftLimit of AxisX
SRLIMIT(AxisX)=SRLIMIT_AxisX 		! sw Limit switch 
SLLIMIT(AxisX)=SLLIMIT_AxisX 		! sw Limit switch

!Set maximum continous current to 2Arms
!XRMS = 100 x 2Arms / 7.07Arms =29%

XRMS(AxisX)		=	29 				!max continous current

SAFETYGROUP (Gantry_Y,Gantry_Yaw,AxisX) !Axis are in same errorhandler group. If one axis is triggered by a Fault, the other axis will respond also

!Set the Gantry at home position
ENABLE(Gantry_Y,Gantry_Yaw,AxisX)
WAIT 1000
TILL ((MST(Gantry_Y).#ENABLED)&(MST(Gantry_Yaw).#ENABLED)&(MST(AxisX).#ENABLED))
PTP/wv (Gantry_Y),0,49 				!Move with Service Mode limit speed  
PTP/wv (AxisX),0,49 				!Move with Service Mode limit speed  
GO (Gantry_Y,AxisX)

ACS_Stat_WaferStage=1 				!WaferStage stage is initialized
DISP "Waferstage Initialized"

EndOfProgram: 

! Enable the default response of the software limits 
FDEF(AxisX).#SRL=1; FDEF(AxisX).#SLL=1 
FMASK(AxisX).#SRL=1
FMASK(AxisX).#SLL=1


ENABLEON Buffer_WaferStage 			! Enable additional Position Limit Interupt routines of Waferstage axes (YYX)
ENABLEON Buffer_LandAirbearing		! Enable Land Airbearing functionality
MachineOK=1 !Machine status is ok

STOP
!--------------------End of Program----------------------------------------------------------------------
!--------------------------------------------------------------------------------------------------------



!--------------------Subroutine HomingY------------------------------------------------------------------
HomingY:
JOG (AxisY0,AxisY1),--; 
TILL ((IST(AxisY0).#IND)&(IST(AxisY1).#IND)) |  (FAULT(AxisY0).#RL)|(FAULT(AxisY1).#RL) !Jog till Index is found or Limit Switch
HALT (AxisY0,AxisY1)

TILL  (^AST(AxisY0).#MOVE)&(^AST(AxisY1).#MOVE)

IF (FAULT(AxisY0).#RL)
	DISP "Limit switch Y0"
END
IF (FAULT(AxisY1).#RL)
	DISP "Limit switch Y1"
END

VEL(AxisY0) = 15; 
VEL(AxisY1) = 15;
PTP/rw (AxisY0),+10 !/w=Wait till GO command--> plus sign due to Flipped Y direction
PTP/rw (AxisY1),+10 !/w=Wait till GO command--> plus sign due to Flipped Y direction

GO (AxisY0,AxisY1)
TILL  (^AST(AxisY0).#MOVE)&(^AST(AxisY1).#MOVE)

RET

!--------------------Subroutine HomingX-------------------------------------------------------------------
HomingX:
JOG (AxisX),-; !Move in negative direction
TILL ((IST(AxisX).#IND) |  (FAULT(AxisX).#LL)) !Jog till Index is found or Limit Switch or HardStop
HALT (AxisX)

TILL (^AST(AxisX).#MOVE)

IF (FAULT(AxisX).#LL)
	DISP "Limit switch X"
END

VEL(AxisX) = 15; 
PTP/rw (AxisX),10 !Wait till GO command

GO (AxisX)
TILL  (^AST(AxisX).#MOVE)

RET
!---------------------------------------------------------------------------------------------------



!--------------------------------------------------------------------------------------------------
!----------------Position Limit Interupt routines--------------------------------------------------
!--------------------------------------------------------------------------------------------------

! ----------------Gantry_Y--------------------------
ON FAULT(Gantry_Y).#LL
	DISP "Left Limit Error Gantry Y\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "OH Focus Axis Disabled"
RET

ON FAULT(Gantry_Y).#RL
	DISP "Right Limit Error Gantry Y\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "OH Focus Axis Disabled"
RET

ON FAULT(Gantry_Y).#SLL
	DISP "Software Left Limit Error Gantry Y\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "OH Focus Axis Disabled"
RET


ON FAULT(Gantry_Y).#SRL
	DISP "Software Right Limit Error Gantry Y\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/ev AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis?"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "OH Focus Axis Disabled"
RET

! ----------------Gantry_Yaw--------------------------

ON FAULT(Gantry_Yaw).#LL
	DISP "Left Limit Error Gantry Yaw\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "OH Focus Axis Disabled"
RET

ON FAULT(Gantry_Yaw).#RL
	DISP "Right Limit Error Gantry Yaw\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "OH Focus Axis Disabled"
RET

ON FAULT(Gantry_Yaw).#SLL
	DISP "Software Left Limit Error Gantry_Yaw\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "OH Focus Axis Disabled"
RET

ON FAULT(Gantry_Yaw).#SRL
	DISP "Software Right Limit Error Gantry_Yaw\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "OH Focus Axis Disabled"
RET

! ----------------AxisX--------------------------

ON FAULT(AxisX).#LL
	DISP "Left Limit Error AxisX\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "OH Focus Axis Disabled"
RET

ON FAULT(AxisX).#RL
	DISP "Right Limit Error AxisX\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "OH Focus Axis Disabled"
RET

ON FAULT(AxisX).#SLL
	DISP "Software Left Limit Error AxisX\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "OH Focus Axis Disabled"
RET

ON FAULT(AxisX).#SRL
	DISP "Software Right Limit Error AxisX\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "OH Focus Axis Disabled"
RET

!---------------------------------------------------------------------------------------------------------------------------------------
!Skew Offset subroutine
Set_Skew:
!Skew_Offset =0
RET
!End of Program



















#5
DISABLEON Buffer_AxisLOH_focus ! Disable additional Position Limit Interupt routines of AxisUOH_focus
DISABLE(AxisLOH_focus)
WAIT 1000

FDEF(AxisLOH_focus).#LL=0;  !Disable Position Left Limit error
FDEF(AxisLOH_focus).#RL=0;	!Disable Position Right Limit error
FDEF(AxisLOH_focus).#SLL=0;  !Disable Software Position Left Limit error
FDEF(AxisLOH_focus).#SRL=0;	!Disable Software Position Right Limit error
SLLIMIT(AxisLOH_focus)=-1E7		!Disable Software Left Limit at startup
SRLIMIT(AxisLOH_focus)=1E7		!Disable Software Right Limit at startup 

WAIT 1000

IF ^MFLAGS(AxisLOH_focus).#BRUSHOK ! If motor is NOT commutated
	DISP "Error: AxisLOH_focus not commutated"
	GOTO EndOfProgram
ELSE 
	DISP "AxisLOH_focus succesful commutated"
END


!Activate Brake
SETCONF(29,040108,2)  !Set Brake at Axis04, DigIO 01.08 Activate 2:: Deactivate =,0
!
!	Motor Enabled -> DigOut = 1 (Brake deactivated)
!	Motor Disabled -> DigOut = 0 (Brake activated)
MFLAGS(AxisLOH_focus).#BRAKE=1
BOFFTIME(AxisLOH_focus)=50	!Brake activation time in ms
BONTIME(AxisLOH_focus)=50	!Brake release time in ms

!Precondtion: AxisLOH_focus was connected with a single motor cable and incremental encoder to controller AND have been tuned properly. NEXT steps should connect the controller into dual control loop. 
!Motor and hall sensors are connected to Axis0 of the UDMsd drive 
!Biss encoder is connected also to Axis0
!Incemental encoder is connected to to Axis2

!Limit settings 
!XCURV = 100% x (max peak motor current)/ (max peak drive current) 
!max peak motor current = 9Arms (Parker SMB040)
!max peak drive current = 5Atop sinus = 3.54 Arms
!XCURV =100 * 9/3.54 == > 100% 
XCURV(AxisLOH_focus)=100;  !Limit Maximum Output during motion of AxisY0: 
XCURI(AxisLOH_focus)=50;	!Limit Maximum Output during IDLE of AxisY0 
XCURV(AxisLOH_focus)=100;  !Limit Maximum Output during motion of AxisY1
XCURI(AxisLOH_focus)=50;	!Limit Maximum Output during IDLE of AxisY1 


!XRMS = (nominal motor current) / (peak drive current)
!max nominal motor current =  2.5Arms (Parker SMB040)
!max peak drive current = 5Atop sinus = 3.54 Arms
!XRMS = 100 x 2.5A / 3.54A =41%
XRMS(AxisLOH_focus)=70 !max current 
 
EFAC(AxisLOH_Load)=2/(4*1024)						![mm/incr] One motor-encoder increment corresponds with 0.00048828 mm  (spindle-lead of 2mm /rev and 4 x 1024 lines/rev]
													!0 : AxisLOH to absolute encoder
													!1 : AxisUOH to absolute encoder
													!2 : AxisLOH-commutation and current control to Quadrature encoder at motor (201)
													!3 : AxisUOH--commutation and current control to Quadrature encoder at motor (301)

!------------Control parameters AxisLOH_focus------------------------	
SLPKP(AxisLOH_focus)	=		275			!org 350 Position Control Loop
SLPKI(AxisLOH_focus)	=		250			!225 at Machine 1 & 2;
SLPLI(AxisLOH_focus)	=		100

SLVKP(AxisLOH_focus)	=		100			!org 50		!Velocity Control Loop
SLVKI(AxisLOH_focus)	=		0			!org 10

SLAFF(AxisLOH_focus) 	=		0! 1073.4			

MFLAGS(AxisLOH_focus).#NOFILT=	1;			!LP filter DISable
SLVSOF(AxisLOH_focus)=			250			
SLVSOFD(AxisLOH_focus) =		0.9;

MFLAGS(AxisLOH_focus).#NOTCH	=	1;			!Notch filter Enable
SLVNATT(AxisLOH_focus) =		15;	
SLVNFRQ(AxisLOH_focus) =		210;
SLVNWID(AxisLOH_focus) =		50;

MFLAGS(AxisLOH_focus).#BI_QUAD=	1;					!BiQuad filter Enable
SLVB0DD(AxisLOH_focus)=0.9
SLVB0DF(AxisLOH_focus)=190
SLVB0ND(AxisLOH_focus)=0.1
SLVB0NF(AxisLOH_focus)=190

MFLAGS(AxisLOH_focus).#BI_QUAD1=	1;				!BiQuad1 filter Enable
SLVB1DD(AxisLOH_focus)=0.7
SLVB1DF(AxisLOH_focus)=400
SLVB1ND(AxisLOH_focus)=0.7
SLVB1NF(AxisLOH_focus)=600
		
SLIKI(AxisLOH_focus) = 		1.000000E+4;			!Current control loop
SLIKP(AxisLOH_focus) = 		65;						!Current control loop

FVFIL(AxisLOH_focus)=95								!Filtering velocity signal

XVEL(AxisLOH_focus) = 55							
XACC(AxisLOH_focus)=1100							!Setting Max speed of motor to max speed of AxisLOH_Load  in dualloop at [mm/s]

!Omzetten van de Absolute encoder als positie input
EFAC(AxisLOH_focus)=0.000001						![mm/incr] One linear encoder increment corresponds with 0.000001mm [1nm]
E_TYPE(AxisLOH_focus)=13							!Set position encoder type at Biss-C
SLABITS(AxisLOH_focus)=32
E_PAR_A(AxisLOH_focus)=5							!Encoder BiSS frequency
E_PAR_B(AxisLOH_focus)=0							!Encoder BiSS CRC Polynominal
E_PAR_C(AxisLOH_focus)=0							!Encoder BiSS Sample Rate (0 == 50us)
SLCPRD(AxisLOH_focus)= 10000000						!10000000 CNTS / mm
!MFLAGS(AxisLOH_focus).#INVENC = 0					!Invert encoder (up is positive)

!If Encoder-scale is mirrored, this encoder has to be set to 1. Furthermore SLVRAT has to be inverted to
!MFLAGS(AxisLOH_focus).#INVENC = 1					!Invert encoder (up is positive)

SLPROUT(AxisLOH_focus) = 0							!Use Biss encoder to set the feedback routing  for the Position Control
SLVROUT(AxisLOH_focus) = 0							!Use Biss encoder to set the feedback routing  for the Velocity Control
SLCROUT(AxisLOH_focus)=201							!Use incremental rotation encoder for commutation and current control

!Commutation settings: In the Wizzard this is set not Correct 

SLCPRD(AxisLOH_focus)=4096							! Feedback counts per revolution at the motor: In the Wizzard this is set not Correct 
SLCPRD(AxisLOH_focus)=SLCPRD(AxisLOH_focus)*20		! Workaround to overcome Error 5104: "ENABLE failed because the motor is moving"
SLCNP(AxisLOH_focus)=8								!Number of polepairs
SLCNP(AxisLOH_focus)=SLCNP(AxisLOH_focus)*20		! Workaround to overcome Error 5104: "ENABLE failed because the motor is moving"

!SLCHALL(AxisLOH_focus)=1
MFLAGS(AxisLOH_focus).#INVHALL=0					!Invert Hall sensor direction
SLCOFFS(AxisLOH_focus)=0.000000000000000e+000
SLCORG(AxisLOH_focus)=0.000000000000000e+000
SLCPA(AxisLOH_focus)=0.000000000000000e+000

!second encoder configuration

!MFLAGS(AxisUOH_Load ).#INVENC = 1							!Invert encoder (up is positive)

SLVRAT(AxisLOH_focus)=1								!Ratio between position loop encoder and velocity loop encoder = 1
MFLAGS(AxisLOH_focus).#DUALLOOP = 1 				!DUALLOOP activated for LOH axis

!If Encoder-scale is mirrored, the ratio has to be set negative. 
!Furthermore MFLAGS(AxisLOH_focus).#INVENC = 1 has to be inverted
CERRA(AxisLOH_focus)	= 1		!Critical Position Error during acceleration 
CERRI(AxisLOH_focus)	= 0.25		!Critical Position Error during idle
CERRV(AxisLOH_focus)	= 0.5		!Critical Position Error during constant velocity
ERRA(AxisLOH_focus)	= 0.5			!Position Error during acceleration
ERRI(AxisLOH_focus)	= 0.125			!Position Error during idle
ERRV(AxisLOH_focus)	= 0.25			!Position Error during constant velocity
TARGRAD(AxisLOH_focus)=100E-6		!Settling window set LARGE --> to be changed

AxisLOH_focus_HomingOffset=-39.8116!
READ AxisLOH_focus_DeltaHomingOffset

WAIT 1000
SET FPOS(AxisLOH_focus)=FPOS(AxisLOH_focus)+AxisLOH_focus_HomingOffset+AxisLOH_focus_DeltaHomingOffset(0)	!Correct with Position Offset
WAIT 1000

StartupLOH=1

!Feedforward formula::
!DCOM(AxisLOH_focus)=AxisLOH_focus_Kfst+AxisLOH_focus_Kfv*RVEL(AxisLOH_focus)+AxisLOH_focus_Kfc*sign(RVEL(AxisLOH_focus)) 
!DCOM is INT [-100%,100%]==[equals with DOUT -->-32768...32768]==[-5Apk...5Apk]==[-3.54Arms...3.54Arms]

AxisLOH_focus_Kfst=0!(2000/32768)*100 !Determined at very Low Speed (in scope: DOUT)
AxisLOH_focus_Kfc=0!(3000/32768)*100	!Determined at very Low Speed (in scope: DOUT)
AxisLOH_focus_Kfv=0					!Velocity feedforward (viscous friction)
SLAFF(AxisLOH_focus)=0	!1126.5		!Acceleration Feedforward

VEL(AxisLOH_focus)=50		!mm/s
ACC(AxisLOH_focus)=1000		!mm/s2
DEC(AxisLOH_focus)=1000		!mm/s2
JERK(AxisLOH_focus)=10000	!mm/s3
KDEC(AxisLOH_focus)	= 1000 	!mm/s2 Deceleration during KILL operation
vel_focus_max = 50 			!mm/s focus speed
LOH_focus_Pos_Retract	=-15 !Retract position of LOH 
vel_focus_retract=9.9 		!mm/s retract speed

ENABLE(AxisLOH_focus)
TILL (MST(AxisLOH_focus).#ENABLED)

PTP/ev(AxisLOH_focus),LOH_focus_Pos_Retract,vel_focus_retract
!TILL ^MST(AxisLOH_focus).#MOVE
WAIT 1000

FDEF(AxisLOH_focus).#LL=1;  !Enable Position Left Limit error
FDEF(AxisLOH_focus).#RL=1;	!Enable Position Right Limit error
FDEF(AxisLOH_focus).#SLL=1;  !Disable Software Position Left Limit error
FDEF(AxisLOH_focus).#SRL=1;	!Disable Software Position Right Limit error

!Normal Motion parameters
SLLIMIT(AxisLOH_focus)=-25			!Software Left Limit 
SRLIMIT(AxisLOH_focus)=3			!Software Right Limit 
!Range = [-25 , 3]
ENABLEON Buffer_AxisLOH_focus ! Enable additional Position Limit Interupt routines of AxisLOH_focus

ACS_Stat_LOH_focus=1 !LOH-focus axis is initialized


EndOfProgram:
STOP


!---------------------------------------------------------------------------------------------------------------------------------------
!----------------Position Limit Interupt routines---------------------------------------------------------------------------------------
!---------------------------------------------------------------------------------------------------------------------------------------

ON FAULT(AxisLOH_focus).#LL
	DISP "Left Limit Error AxisLOH_focus\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(AxisLOH_focus).#RL
	DISP "Right Limit Error AxisLOH_focus\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(AxisLOH_focus).#SLL
	DISP "Software Left Limit Error AxisLOH_focus\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(AxisLOH_focus).#SRL
	DISP "Software Right Limit Error AxisLOH_focus\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

!---------------------------------------------------------------------------------------------------------------------------------------












#6
DISABLEON Buffer_AxisUOH_focus ! Disable additional Position Limit Interupt routines of AxisUOH_focus
DISABLE(AxisUOH_focus)
WAIT 1000

FDEF(AxisUOH_focus).#LL=0;  !Disable Position Left Limit error
FDEF(AxisUOH_focus).#RL=0;	!Disable Position Right Limit error
FDEF(AxisUOH_focus).#SLL=0;  !Disable Software Position Left Limit error
FDEF(AxisUOH_focus).#SRL=0;	!Disable Software Position Right Limit error
SLLIMIT(AxisUOH_focus)=-1E7		!Disable Software Left Limit at startup 
SRLIMIT(AxisUOH_focus)=1E7		!Disable Software Right Limit at startup 

WAIT 1000

IF ^MFLAGS(AxisUOH_focus).#BRUSHOK ! If motor is NOT commutated
	DISP "Error: AxisUOH_focus not commutated"
	GOTO EndOfProgram
ELSE 
	DISP "AxisUOH_focus succesful commutated"
END


!Activate Brake
SETCONF(29,050109,2) !Set Brake at Axis05, DigIO 01.09 Activate 2:: Deactivate =,0
!
!	Motor Enabled -> DigOut = 1 (Brake deactivated)
!	Motor Disabled -> DigOut = 0 (Brake activated)
MFLAGS(AxisUOH_focus).#BRAKE=1
BOFFTIME(AxisUOH_focus)=50	!Brake activation time in ms
BONTIME(AxisUOH_focus)=50	!Brake release time in ms

!Precondtion: AxisUOH_focus was connected with a single motor cable and incremental encoder to controller AND have been tuned properly. NEXT steps should connect the controller into dual control loop. 
!Motor and hall sensors are connected to Axis0 of the UDMsd drive 
!Biss encoder is connected also to Axis0
!Incemental encoder is connected to to Axis2

!Limit settings 
!XCURV = 100% x (max peak motor current)/ (max peak drive current) 
!max peak motor current = 9Arms (Parker SMB040)
!max peak drive current = 5Atop sinus = 3.54 Arms
!XCURV =100 * 9/3.54 == > 100% 
XCURV(AxisUOH_focus)=100;  !Limit Maximum Output during motion of AxisY0: 
XCURI(AxisUOH_focus)=50;	!Limit Maximum Output during IDLE of AxisY0 
XCURV(AxisUOH_focus)=100;  !Limit Maximum Output during motion of AxisY1
XCURI(AxisUOH_focus)=50;	!Limit Maximum Output during IDLE of AxisY1 


!XRMS = (nominal motor current) / (peak drive current)
!max nominal motor current =  2.5Arms (Parker SMB040)
!max peak drive current = 5Atop sinus = 3.54 Arms
!XRMS = 100 x 2.5A / 3.54A =41%
XRMS(AxisUOH_focus)=70 !max current 

EFAC(AxisUOH_Load)=2/(4*1024)						![mm/incr] One motor-encoder increment corresponds with 0.00048828 mm  (spindle-lead of 2mm /rev and 4 x 1024 lines/rev]
													!0 : AxisUOH to absolute encoder
													!1 : AxisUOH to absolute encoder
													!2 : AxisUOH-velocity to Quadrature encoder at motor (201)
													!3 : AxisUOH-velocity to Quadrature encoder at motor (301)

!------------Control parameters AxisUOH_focus------------------------	
SLPKP(AxisUOH_focus)	=		275			!Position Control Loop
SLPKI(AxisUOH_focus)	=		225				! org 500
SLPLI(AxisUOH_focus)	=		100			

SLVKP(AxisUOH_focus)	=		50;	! org 100	!Velocity Control Loop
SLVKI(AxisUOH_focus)	=		0

SLAFF(AxisUOH_focus) 	=		0	!1185.1			


MFLAGS(AxisUOH_focus).#NOFILT=	0;				!LP filter DISable
SLVSOF(AxisUOH_focus)=			100;			!org 150
SLVSOFD(AxisUOH_focus) =		0.9;

MFLAGS(AxisUOH_focus).#NOTCH	=	0;			!Notch filter Enable
SLVNATT(AxisUOH_focus) =		10;	
SLVNFRQ(AxisUOH_focus) =		180;
SLVNWID(AxisUOH_focus) =		50;

MFLAGS(AxisUOH_focus).#BI_QUAD=	1;					!BiQuad filter Enable
SLVB0DD(AxisUOH_focus)=0.9
SLVB0DF(AxisUOH_focus)=250							!org 200
SLVB0ND(AxisUOH_focus)=0.1
SLVB0NF(AxisUOH_focus)=100							!org 200

MFLAGS(AxisUOH_focus).#BI_QUAD1=	0;				!BiQuad1 filter Enable
SLVB1DD(AxisUOH_focus)=0.7
SLVB1DF(AxisUOH_focus)=150
SLVB1ND(AxisUOH_focus)=0.7
SLVB1NF(AxisUOH_focus)=400
		
SLIKI(AxisUOH_focus) = 		1.000000E+4;			!Current control loop
SLIKP(AxisUOH_focus) = 		65;						!Current control loop

FVFIL(AxisUOH_focus)=95								!Filtering velocity signal

XVEL(AxisUOH_focus) = 55							
XACC(AxisUOH_focus)=1100							!Setting Max speed of motor to max speed of AxisUOH_Load  in dualloop at [mm/s]

!Omzetten van de Absolute encoder als positie input
EFAC(AxisUOH_focus)=0.000001						![mm/incr] One linear encoder increment corresponds with 0.000001mm [1nm]
E_TYPE(AxisUOH_focus)=13							!Set position encoder type at Biss-C
SLABITS(AxisUOH_focus)=32
E_PAR_A(AxisUOH_focus)=5							!Encoder BiSS frequency
E_PAR_B(AxisUOH_focus)=0							!Encoder BiSS CRC Polynominal
E_PAR_C(AxisUOH_focus)=0							!Encoder BiSS Sample Rate (0 == 50us)
SLCPRD(AxisUOH_focus)= 10000000						!10000000 CNTS / mm
MFLAGS(AxisUOH_focus).#INVENC = 1					!Invert encoder (up is positive)

!If Encoder-scale is mirrored, this encoder has to be set to 1. Furthermore SLVRAT has to be inverted to
!MFLAGS(AxisUOH_focus).#INVENC = 1					!Invert encoder (up is positive)

SLPROUT(AxisUOH_focus) = 0							!Use Biss encoder to set the feedback routing of the Position Control
SLVROUT(AxisUOH_focus) = 0							!Use Biss encoder to set the feedback routing of the Velocity Control
SLCROUT(AxisUOH_focus)=301							!Use incremental rotation encoder for commutation and current control

!Commutation settings: In the Wizzard this is set not Correct 

SLCPRD(AxisUOH_focus)=4096							! Feedback counts per revolution at the motor: In the Wizzard this is set not Correct 
SLCPRD(AxisUOH_focus)=SLCPRD(AxisUOH_focus)*20		! Workaround to overcome Error 5104: "ENABLE failed because the motor is moving"
SLCNP(AxisUOH_focus)=8								! Number of polepairs
SLCNP(AxisUOH_focus)=SLCNP(AxisUOH_focus)*20		! Workaround to overcome Error 5104: "ENABLE failed because the motor is moving"

SLCHALL(AxisUOH_focus)=1
!MFLAGS(AxisUOH_focus).#INVHALL=0					!Invert Hall sensor direction
SLCOFFS(AxisUOH_focus)=0.000000000000000e+000
SLCORG(AxisUOH_focus)=0.000000000000000e+000
SLCPA(AxisUOH_focus)=0.000000000000000e+000

!second encoder configuration
MFLAGS(AxisUOH_Load ).#INVENC = 1							!Invert encoder (up is positive)

SLVRAT(AxisUOH_focus)=1										!Ratio between position loop encoder and velocity loop encoder = 1
MFLAGS(AxisUOH_focus).#DUALLOOP = 1 						!DUALLOOP activated for UOH axis 

!If Encoder-scale is mirrored, the ratio has to be set negative. 
!Furthermore MFLAGS(AxisUOH_focus).#INVENC = 1 has to be inverted

CERRA(AxisUOH_focus)	= 1			!Critical Position Error during acceleration 
CERRI(AxisUOH_focus)	= 0.25		!Critical Position Error during idle
CERRV(AxisUOH_focus)	= 0.5		!Critical Position Error during constant velocity
ERRA(AxisUOH_focus)	= 0.5			!Position Error during acceleration
ERRI(AxisUOH_focus)	= 0.125			!Position Error during idle
ERRV(AxisUOH_focus)	= 0.25			!Position Error during constant velocity
TARGRAD(AxisUOH_focus)=100E-6		!Settling window set LARGE --> to be changed

AxisUOH_focus_HomingOffset=25.6362
READ AxisUOH_focus_DeltaHomingOffset

WAIT 1000
SET FPOS(AxisUOH_focus)=FPOS(AxisUOH_focus)+AxisUOH_focus_HomingOffset+AxisUOH_focus_DeltaHomingOffset(0)	!Correct with Position Offset
WAIT 1000

StartupUOH=1

!Feedforward formula::
!DCOM(AxisUOH_focus)=AxisUOH_focus_Kfst+AxisUOH_focus_Kfv*RVEL(AxisUOH_focus)+AxisUOH_focus_Kfc*sign(RVEL(AxisUOH_focus)) 
!DCOM is INT [-100%,100%]==[equals with DOUT -->-32768...32768]==[-5Apk...5Apk]==[-3.54Arms...3.54Arms]

AxisUOH_focus_Kfst=0 				! To Determine at very Low Speed (in scope: DOUT)
AxisUOH_focus_Kfc=0					!To  Determine at very Low Speed (in scope: DOUT)
AxisUOH_focus_Kfv=0					!Velocity feedforward (viscous friction)
SLAFF(AxisUOH_focus)=0!1018.7			!Acceleration Feedforward

VEL(AxisUOH_focus)=50		!mm/s
ACC(AxisUOH_focus)=1000		!mm/s2
DEC(AxisUOH_focus)=1000		!mm/s2
JERK(AxisUOH_focus)=10000	!mm/s3
KDEC(AxisUOH_focus)	= 1000 	! mm/s2 Deceleration during KILL operation
vel_focus_max = 50 			!mm/s focus speed
UOH_focus_Pos_Retract	=15 !Retract position of UOH  
vel_focus_retract=9.9 		!mm/s retract speed

ENABLE(AxisUOH_focus)
TILL (MST(AxisUOH_focus).#ENABLED)

PTP/ev(AxisUOH_focus),UOH_focus_Pos_Retract,vel_focus_retract
!TILL ^MST(AxisUOH_focus).#MOVE
WAIT 1000

FDEF(AxisUOH_focus).#LL=1;  !Disable Position Left Limit error
FDEF(AxisUOH_focus).#RL=1;	!Disable Position Right Limit error
FDEF(AxisUOH_focus).#SLL=1;  !Disable Software Position Left Limit error
FDEF(AxisUOH_focus).#SRL=1;	!Disable Software Position Right Limit error

!Normal Motion parameters
SLLIMIT(AxisUOH_focus)=-9			!Software Left Limit 
SRLIMIT(AxisUOH_focus)=20			!Software Right Limit 
!Range = [20 , -9]
ENABLEON Buffer_AxisUOH_focus ! Enable additional Position Limit Interupt routines of AxisUOH_focus

ACS_Stat_UOH_focus=1 !UOH-focus axis is initialized


EndOfProgram:
STOP


!---------------------------------------------------------------------------------------------------------------------------------------
!----------------Position Limit Interupt routines---------------------------------------------------------------------------------------
!---------------------------------------------------------------------------------------------------------------------------------------

ON FAULT(AxisUOH_focus).#LL
	DISP "Left Limit Error AxisUOH_focus\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"

RET



ON FAULT(AxisUOH_focus).#RL
	DISP "Right Limit Error AxisUOH_focus\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(AxisUOH_focus).#SLL
	DISP "Software Left Limit Error AxisUOH_focus\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

ON FAULT(AxisUOH_focus).#SRL
	DISP "Software Right Limit Error AxisUOH_focus\r"
	KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
	DISP "Gantry and OH axis KILLED"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	IF MST(AxisLOH_focus).#ENABLED
		PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
		DISP "Retract LOH Focus axis"
	END
	IF MST(AxisUOH_focus).#ENABLED
		 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
		 DISP "Retract UOH Focus axis"
	END
	TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
	WAIT 1000	!Wait till Gantry is at rest
	DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
	DISP "Gantry Axis Disabled"
	TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
	DISABLE (AxisLOH_focus,AxisUOH_focus)
	DISP "Focus Axis Disabled"
RET

!---------------------------------------------------------------------------------------------------------------------------------------












#7
!Gain Scheduling in  Gantry Mode

! Servo tuning for the axis in gantry mode

!------------------------------------------
!function of FPOS(AxisX)
!------------------------------------------

!SLPKP(Gantry_Y)=120;SLVKP(Gantry_Y)=400;SLVKI(Gantry_Y)= 400;

SLPKP(Gantry_Y)=62.8319+0*FPOS(AxisX)				!proportional coefficient of the position control loop
SLPKI(Gantry_Y)=0*FPOS(AxisX)						!integral coefficient of the position control loop
SLVLI(Gantry_Y)=50+ 0*FPOS(AxisX)					!Integral limit [ 0 -100%] for anti-windup
SLVKP(Gantry_Y)=30+0*FPOS(AxisX)					!proportional coefficient of the velocity control loop

SLAFF(Gantry_Y)=17+0*FPOS(AxisX)					!Defines acceleration feed forward for a given axis
SLVRAT(Gantry_Y)=1+0*FPOS(AxisX)					!Defines velocity feed forward ratio for a given axis
SLFRCD(Gantry_Y)=0*FPOS(AxisX)						!dynamic friction compensation (0 - 50%) x VEL

MFLAGS(Gantry_Y).15=0+0*FPOS(AxisX); 				!The control algorithm includes a second order filter (1 = bypassed)
SLVSOF(Gantry_Y)=1100+0*FPOS(AxisX);				!second order filter bandwidth for the velocity control loop
SLVSOFD(Gantry_Y)=0.707+0*FPOS(AxisX); 				!second order filter damping                     

MFLAGS(Gantry_Y).14=1+0*FPOS(AxisX); 				!Notch filter is enabled (0=disabled, 1= enabled)
SLVNFRQ(Gantry_Y)=765+0*FPOS(AxisX); 				!Notch filter frequency in Hz
SLVNWID(Gantry_Y)=50+0*FPOS(AxisX);					!Notch filter width in Hz (3dB points)
SLVNATT(Gantry_Y)=10+0*FPOS(AxisX)        			!Notch filter attenuation 

MFLAGS(Gantry_Y).16=1+0*FPOS(AxisX); 				!First BiQuad Filter is enabled (0=disabled, 1= enabled)
SLVB0NF(Gantry_Y)=1111+0*FPOS(AxisX); 				!Numerator value of the Bi-Quad filter
SLVB0ND(Gantry_Y)=0.045+0*FPOS(AxisX); 				!Numerator value of the Bi-Quad damping ratio
SLVB0DF(Gantry_Y)=1111+0*FPOS(AxisX); 				!Denominator value of the Bi-Quad filter
SLVB0DD(Gantry_Y)=0.44+0*FPOS(AxisX)   				!Denominator value of the Bi-Quad damping ratio 





!SLPKP(Gantry_Yaw)=100;SLVKP(Gantry_Yaw)=150; SLVKI(Gantry_Yaw)=200
SLPKP(Gantry_Yaw)=60+0*FPOS(AxisX)					!proportional coefficient of the position control loop
SLPKI(Gantry_Yaw)=0+0*FPOS(AxisX)					!integral coefficient of the position control loop
SLVLI(Gantry_Yaw)=50+0*FPOS(AxisX)					!Integral limit [ 0 -100%] for anti-windup
SLVKP(Gantry_Yaw)=30+0*FPOS(AxisX)					!proportional coefficient of the velocity control loop

SLAFF(Gantry_Yaw)=0+0*FPOS(AxisX)					!Defines acceleration feed forward for a given axis
SLVRAT(Gantry_Yaw)=1+0*FPOS(AxisX)					!Defines velocity feed forward ratio for a given axis
SLFRCD(Gantry_Yaw)=0+0*FPOS(AxisX)					!Dynamic friction compensation (0 - 50%) x VEL

MFLAGS(Gantry_Yaw).15=0+0*FPOS(AxisX);				!The control algorithm includes a second order filter (1 = bypassed)
SLVSOF(Gantry_Yaw)=1000+0*FPOS(AxisX)				!second order filter bandwidth for the velocity control loop
SLVSOFD(Gantry_Yaw)=0.707+0*FPOS(AxisX)  			!second order filter damping   

MFLAGS(Gantry_Yaw).14=1+0*FPOS(AxisX)				!Notch filter is enabled (0=disabled, 1= enabled)
SLVNFRQ(Gantry_Yaw)=613+0*FPOS(AxisX); 				!Notch filter frequency in Hz
SLVNWID(Gantry_Yaw)=70+0*FPOS(AxisX)				!Notch filter width in Hz	(3dB points)
SLVNATT(Gantry_Yaw)=7+0*FPOS(AxisX)       			!Notch filter attenuation

MFLAGS(Gantry_Yaw).16=0+0*FPOS(AxisX) 				!First BiQuad Filter is enabled (0=disabled, 1= enabled)
SLVB0NF(Gantry_Yaw)=1111+0*FPOS(AxisX) 				!Numerator value of the Bi-Quad filter [Hz]
SLVB0ND(Gantry_Yaw)=0.045+0*FPOS(AxisX) 			!Numerator value of the Bi-Quad damping ratio
SLVB0DF(Gantry_Yaw)=1111+0*FPOS(AxisX) 				!Denominator value of the Bi-Quad filter
SLVB0DD(Gantry_Yaw)=0.44+0*FPOS(AxisX)   			!Denominator value of the Bi-Quad damping ratio 
























#10
!Service Mode program

!Checking Safe Limits in Service Mode 

ON ((ABS(RVEL(AxisLOH_focus)) > vel_focus_service)&(ServiceModeOn))
		DISP "Service Velocity limit exceeded LOH focus axis\r"
		KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
		DISP "Gantry and OH axis KILLED"
		TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
		IF MST(AxisLOH_focus).#ENABLED
			PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
			DISP "Retract LOH Focus axis"
		END
		IF MST(AxisUOH_focus).#ENABLED
			 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
			 DISP "Retract UOH Focus axis"
		END
		TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
		WAIT 1000	!Wait till Gantry is at rest
		DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
		DISP "Gantry Axis Disabled"
		TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
		DISABLE (AxisLOH_focus,AxisUOH_focus)
		DISP "Focus Axis Disabled"
		MachineOK=0
RET
ON ((ABS(RVEL(AxisUOH_focus)) > vel_focus_service)&(ServiceModeOn))
		DISP "Service Velocity limit exceeded UOH focus axis\r"
		KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
		DISP "Gantry and OH axis KILLED"
		TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
		IF MST(AxisLOH_focus).#ENABLED
			PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
			DISP "Retract LOH Focus axis"
		END
		IF MST(AxisUOH_focus).#ENABLED
			 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
			 DISP "Retract UOH Focus axis"
		END
		TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
		WAIT 1000	!Wait till Gantry is at rest
		DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
		DISP "Gantry Axis Disabled"
		TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
		DISABLE (AxisLOH_focus,AxisUOH_focus)
		DISP "Focus Axis Disabled"
		MachineOK=0

RET

ON ((ABS(RVEL(Gantry_Y)) > vel_gantry_service)&(ServiceModeOn))
		DISP "Service Velocity limit exceeded Gantry axis\r"
		KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
		DISP "Gantry and OH axis KILLED"
		TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
		IF MST(AxisLOH_focus).#ENABLED
			PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
			DISP "Retract LOH Focus axis"
		END
		IF MST(AxisUOH_focus).#ENABLED
			 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
			 DISP "Retract UOH Focus axis"
		END
		TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
		WAIT 1000	!Wait till Gantry is at rest
		DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
		DISP "Gantry Axis Disabled"
		TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
		DISABLE (AxisLOH_focus,AxisUOH_focus)
		DISP "Focus Axis Disabled"
		MachineOK=0

RET

ON ((ABS(RVEL(AxisX)) > vel_gantry_service)&(ServiceModeOn))
		DISP "Service Velocity limit exceeded X-axis\r"
		KILL(Gantry_Y,Gantry_Yaw,AxisX,AxisLOH_focus,AxisUOH_focus)
		DISP "Gantry and OH axis KILLED"
		TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
		IF MST(AxisLOH_focus).#ENABLED
			PTP/v AxisLOH_focus,LOH_focus_Pos_Retract,vel_focus_retract !Retract LOH focus axis
			DISP "Retract LOH Focus axis"
		END
		IF MST(AxisUOH_focus).#ENABLED
			 PTP/v AxisUOH_focus,UOH_focus_Pos_Retract,vel_focus_retract !Retract UOH focus axis
			 DISP "Retract UOH Focus axis"
		END
		TILL ^MST(AxisX).#MOVE & ^MST(Gantry_Y).#MOVE & ^MST(Gantry_Yaw).#MOVE,TimeOut
		WAIT 1000	!Wait till Gantry is at rest
		DISABLE (AxisX,Gantry_Y,Gantry_Yaw)
		DISP "Gantry Axis Disabled"
		TILL ^MST(AxisLOH_focus).#MOVE & ^MST(AxisUOH_focus).#MOVE,TimeOut
		DISABLE (AxisLOH_focus,AxisUOH_focus)
		DISP "Focus Axis Disabled"
		MachineOK=0

RET


STOP
		
	


#A
!axisdef X=0,Y=1,Z=2,T=3,A=4,B=5,C=6,D=7
!axisdef x=0,y=1,z=2,t=3,a=4,b=5,c=6,d=7
global int I(100),I0,I1,I2,I3,I4,I5,I6,I7,I8,I9,I90,I91,I92,I93,I94,I95,I96,I97,I98,I99
global real V(100),V0,V1,V2,V3,V4,V5,V6,V7,V8,V9,V90,V91,V92,V93,V94,V95,V96,V97,V98,V99

!---------------E-cabinet----------------------						
global	int	EC_AnOutEL4104_0			!EtherCAT adress	
global	int	EC_AnOutEL4104_1			!EtherCAT adress	
global	int	EC_AnOutEL4104_2			!EtherCAT adress	
global	int	EC_AnOutEL4104_3			!EtherCAT adress	
						
global	int	EC_DigOutEL2008_0			!EtherCAT adress	
						
global	int	EC_DigInEL1008_0			!EtherCAT adress	
!-----------------------------------------------						
						
!---------------Metrology	module----------------					
global	int	EC_TempInEL3204_0				!EtherCAT adress	
global	int	EC_TempInEL3204_1				!EtherCAT adress	
global	int	EC_TempInEL3204_2				!EtherCAT adress	
global	int	EC_TempInEL3204_3				!EtherCAT adress	
global int  AnInStatus
						
global	int	EC_AnOutEL4124_0			!EtherCAT adress	
global	int	EC_AnOutEL4124_1			!EtherCAT adress	
global	int	EC_AnOutEL4124_2			!EtherCAT adress	
global	int	EC_AnOutEL4124_3			!EtherCAT adress	
						
global	int	EC_AnInEL3054_0				!EtherCAT adress	
global	int	EC_AnInEL3054_1				!EtherCAT adress	
global	int	EC_AnInEL3054_2				!EtherCAT adress	
global	int	EC_AnInEL3054_3				!EtherCAT adress	
						
global	int	EC_DigOutEL2008_1			!EtherCAT adress	
						
global	int	EC_DigInEL1008_1			!EtherCAT adress	
						
global	int	EC_DigInEL1008_2			!EtherCAT adress	
						
global	int	EC_DigOutEL2008_2			!EtherCAT adress	
						
global	int	EC_DigInEL1008_3			!EtherCAT adress	
!-----------------------------------------------						


!---------------E-cabinet----------------------
global int AnOutEL4104_0  	!Anout0
global int AnOutEL4104_1  	!AnOut1
global int AnOutEL4104_2 	!Anout2
global int AnOutEL4104_3 	!AnOut3

global int DigOutEL2008_0	!8 port DigOutput (xxx.0---xxx.7)

global int DigInEL1008_0	!8 port DigInput (xxx.0---xxx.7)
!-----------------------------------------------

!---------------Metrology module----------------
global int TempInEL3204_0 	!TempInput0 
global int TempInEL3204_1	!TempInput1 
global int TempInEL3204_2 	!TempInput2 
global int TempInEL3204_3 	!TempInput3 

global int AnOutEL4124_0 	!Anout0 4-20mA 
global int AnOutEL4124_1	!Anout1 4-20mA 
global int AnOutEL4124_2	!Anout2 4-20mA 
global int AnOutEL4124_3 	!Anout3 4-20mA 

global int AnInEL3054_0 	!AnIn0 4-20mA
global int AnInEL3054_1		!AnIn1 4-20mA
global int AnInEL3054_2 	!AnIn2 4-20mA
global int AnInEL3054_3		!AnIn3 4-20mA

global int DigOutEL2008_1 	!8 port DigOutput (xxx.0---xxx.7)

global int DigInEL1008_1	!8 port DigInput (xxx.0---xxx.7)

global int DigInEL1008_2 	!8 port DigInput (xxx.0---xxx.7)

global int DigOutEL2008_2 	!8 port DigOutput (xxx.0---xxx.7)

global int DigInEL1008_3 	!8 port DigInput (xxx.0---xxx.7)
!-----------------------------------------------



global real TimeOut
global int	DigIn0
global int	DigIn1
global int	DigIn2
global int	DigIn3
global int	DigIn4
global int	DigIn5
global int	DigIn6
global int	DigIn7
global int	DigIn8
global int	DigIn9
global int	DigIn10
global int	DigIn11
global int	DigIn12
global int	DigIn13
global int	DigIn14
global int	DigIn15
global int	DigIn16
global int	DigIn17
global int	DigIn18
global int	DigIn19
global int	DigIn20
global int	DigIn21
global int	DigIn22
global int	DigIn23
global int	DigIn24
global int	DigIn25
global int	DigIn26
global int	DigIn27
global int	DigIn28
global int	DigIn29
global int	DigIn30
global int	DigIn31


global int	DigOut0
global int	DigOut1
global int	DigOut2
global int	DigOut3
global int	DigOut4
global int	DigOut5
global int	DigOut6
global int	DigOut7
global int	DigOut8
global int	DigOut9
global int	DigOut10
global int	DigOut11
global int	DigOut12
global int	DigOut13
global int	DigOut14
global int	DigOut15
global int	DigOut16
global int	DigOut17
global int	DigOut18
global int	DigOut19
global int	DigOut20
global int	DigOut21
global int	DigOut22
global int	DigOut23


global int	AnIn0						!0 - 32767
global int	AnIn1						!0 - 32767
global int	AnIn2						!0 - 32767
global int	AnIn3						!0 - 32767
global int	AnIn4						!0 - 32767
global int	AnIn5						!0 - 32767
global int	AnIn6						!0 - 32767
global int	AnIn7						!0 - 32767


global int	AnOut0						!0 - 32767
global int	AnOut1						!0 - 32767
global int	AnOut2						!0 - 32767
global int	AnOut3						!0 - 32767
global int	AnOut4						!0 - 32767
global int	AnOut5						!0 - 32767
global int	AnOut6						!0 - 32767
global int	AnOut7						!0 - 32767

global int ACS_Stat_Process				!Status variable Process
global int ACS_Stat_Airbearing			!Status variable Airbearings
global int ACS_Stat_WaferStage			!Status variable WaferStage
global int ACS_Stat_LOH_focus			!Status variable LOH_focus
global int ACS_Stat_UOH_focus			!Status variable UOH_focus
global int softEmergency_Stop			!Stop all axis, retract LOH, UOH focus axis and disable

global int ACS_Init_Process				!Start Initialization Process
global int ACS_Init_Airbearing			!Start Initialization Airbearings
global int ACS_Init_WaferStage			!Start Initiatization WaferStage
global int ACS_Init_LOH_focus			!Start Initialization LOH_focus
global int ACS_Init_UOH_focus			!Start Initialization UOH_focus


!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!1
global real	AirbearingPressureControl	!AirPressureControl of Airbearing
global real	AirbearingVacuumControl0	!VacuumControl of Airbearing0
global real	AirbearingVacuumControl1	!VacuumControl of Airbearing1
	
global real	AirbearingPressureSensor	!AirPressureSensor of Airbearing
global real	AirbearingVacuumSensor0 	!VacuumSensor of Airbearing0
global real	AirbearingVacuumSensor1		!VacuumSensor of Airbearing1

global int	StageLift					!Airvalve to Lift the WaferStage
global int	ServiceLight				!ServiceLightOn/Off

global int 	WaferClamp					!WaferClamp state variable
global int  prevWaferClamp				!previous state of variable
global int	WaferClampDigOut			!Left AirValve to clamp the wafer
global int	nWaferClampDigOut			!Right AirValve to release the wafer
global int 	FFU_Power					!Power FFU On/Off
global int 	FFU_Error					!Error on FFU (Not use atm) ! FOGALE USER DEFINED !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
global int	ServiceModeOn				!If ServiceModethen go into ServiceMode (Low speed)
global int  LandAirbearing				!Host Command to lower the stage onto the granite during measurements
global int 	LandAirbearingStatus		!Acutal Status of the Airbearing indicating if it is landed of lifted 
global int  LandAirbearingTime			!Time in ms needed to land the Airbearing on the Granite
global int  FreezeMotion				!If Airbearings are lowered, motion commands are blocked

global int	VacuumSupplyPresent			!Digital signal indicates presence of VacuumSupply
global int	AirSupplyPresent			!Digital signal indicates presence of AirSupply
global int	WaferClamp_sensor			!Digital signal indicates if wafer is present
global int	WEX_sensor 					!Digital signal indicates presence of WaferStage at WEX position

global int 	EFEM_RobotArmIsExtended		!Digital signal indicating when Robot Arm of EFEM is extended
global int	EFEM_InterfacePower
global int	EFEM_StageLoadPos
global int	EFEM_WaferPresent
global int	EFEM_StageIs200
global int	EFEM_WaferPresent300
global int	EFEM_RESERVED

global real	Light_Control_Halogen_UOH	!Light_Control_Halogen_UOH 
global real	Light_Control_Halogen_LOH	!Light_Control_Halogen_LOH
global real	Light_Control_VIS_RED_LED	!Light_Control_VIS_RED_LED
global real	Light_Control_VIS_WHITE_LED	!Light_Control_VIS_WHITE_LED
global real	Light_Power_Halogen_UOH		!Light_Power_Halogen_UOH On/Off
global real	Light_Power_Halogen_LOH		!Light_Power_Halogen_LOH On/Off
global real	Light_Power_VIS_RED_LED		!Light_Power_VIS_RED_LED On/Off
global real	Light_Power_VIS_WHITE_LED	!Light_Power_VIS_WHITE_LED On/Off

global int 	AirbearingPressure_OK		 
global int  AirbearingVacuum0_OK
global real VacuumSensor0Limit
global int  AirbearingVacuum1_OK
global real VacuumSensor1Limit
global real MotorTemperatureY0
global int  MotorTemperatureY0_OK
global real MotorTemperatureY0_Limit
global real MotorTemperatureY1
global int  MotorTemperatureY1_OK
global real MotorTemperatureY1_Limit
global real MotorTemperatureX
global int  MotorTemperatureX_OK
global real MotorTemperatureX_Limit
global int  WaferClamp_OK
	
								
global int	PowerOK_24V					!Power Ok power supply
global int	PowerOK_24V_Aux				!Power Ok power supply
global int	PowerOK_5V					!Power Ok power supply
global int	PowerOK_48V_OH_focus		!Power Ok power supply
global int	PowerOK_48V_WaferStage		!Power Ok power supply
global int 	Power_OK					!Power Ok if all power supplies are OK

global int	DoorLeft0Closed				!If Door is closed, signal is active 
global int	DoorLeft1Closed				!If Door is closed, signal is active 
global int	DoorRight0Closed			!If Door is closed, signal is active 
global int	DoorRight1Closed			!If Door is closed, signal is active 
global int	DoorBack0Closed			!If Door is closed, signal is active 
global int	DoorBack1Closed			!If Door is closed, signal is active 
  
!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!1	


global real PressureLimit0				!PressureLimit0
global real PressureLimit1				!PressureLimit1
global real VacuumSensorLimit			!VacuumLimit

global int	AxisY0						!AxisY0 if waferstage is not in Gantry mode
global int	AxisY1						!AxisY1 if waferstage is not in Gantry mode
global int	Gantry_Y 					!In Gantry mode, axis is tranformed into a Y axis and a Yaw axis
global int	Gantry_Yaw 					!In Gantry mode, axis is tranformed into a Y axis and a Yaw axis
global int 	AxisX						!AxisX of waferstage
global int 	AxisLOH_focus				!focus axis of Lower Optical Head
global int  AxisLOH_Load 				!Dual encoder config axis
global int 	AxisUOH_focus				!focus axis of Upper Optical Head
global int  AxisUOH_Load 				!Dual encoder config axis

global real	vel_focus_max				!maximum retraction speed of focus axis in case of EMO
global real LOH_focus_Pos_Retract		!Retract position of LOH focus
global real UOH_focus_Pos_Retract		!Retract position of LOH focus
global real vel_focus_retract			!Retract velocity of OH focus
global real AxisY0_HomingOffset			!Homing Offset
global real AxisY1_HomingOffset			!Homing Offset
!global real Skew_Offset					!Offset between AxisY0-AxisY1
global real AxisX_HomingOffset			!Homing Offset
global real AxisLOH_focus_HomingOffset	!Homing Offset
global real AxisUOH_focus_HomingOffset	!Homing Offset
global real vel_focus_service			!maximum velocity in Service Mode
global real vel_gantry_service			!maximum velocity in Service Mode
global real SRLIMIT_Gantry_Y			!RightLimit of Gantry_y
global real SLLIMIT_Gantry_Y			!LeftLimit of Gantry_y
global real SRLIMIT_AxisX				!RightLimit of AxisX
global real SLLIMIT_AxisX				!LeftLimit of AxisX

global int	Buffer_Commutation			!0: Commutation Algorithm
global int	Buffer_AutoExe				!1: StartUp Buffer with Application Variables
global int 	Buffer_SafetyErrorhandling	!2:	Autoroutines with safety and error handling
global int 	Buffer_Process				!3: Process functions
global int	Buffer_WaferStage			!4: Initialization of WaferStage
global int	Buffer_AxisLOH_focus		!5:	Initialization of AxisLOH_focus
global int 	Buffer_AxisUOH_focus		!6:	Initialization of AxisLOH_focus
global int	Buffer_Gainscheduling		!7: Gainscheduling
global int	Buffer_Test					!8: Test purposes
global int 	Buffer_ServiceProgram		!10: Service mode Program
global int	Buffer_EFEM					!11: EFEM Robot Arm Extracted
global int  Buffer_LandAirbearing		!12 Land Airbearing

global int 	StartupLOH	!Buffer LOH runned once
global int 	StartupUOH	!Buffer UOH runned once

global real Gantry_Y_Kfst				!Static feedforward
global real Gantry_Y_Kfv				!Velocity feedforward (viscous friction) 
global real Gantry_Y_Kfc					!colomb feedforward (coulomb friction) 
global real AxisX_Kfst				!Static feedforward
global real AxisX_Kfv					!Velocity feedforward (viscous friction) 
global real AxisX_Kfc					!colomb feedforward (coulomb friction) 
global real AxisLOH_focus_Kfst			!Static feedforward
global real AxisLOH_focus_Kfv			!Velocity feedforward (viscous friction) 
global real AxisLOH_focus_Kfc			!colomb feedforward (coulomb friction) 
global real AxisUOH_focus_Kfst			!Static feedforward
global real AxisUOH_focus_Kfv			!Velocity feedforward (viscous friction) 
global real AxisUOH_focus_Kfc			!colomb feedforward (coulomb friction) 


global int 	P_FAULT						!Process Error code
global int	ResetFAULT					!Reset FAULT status of ACS Motion MERR faults and (custom) Proces P_FAULT  (1= reset, 0 = none)
global int	LastFAULT					!stored FAULT status since ResetFAULT
global int	MachineOK					!status of machine. If no Fault --> MachineOK=1

global int	AirbearingPressure_ErrorCode									
global int	AirbearingVacuum0_ErrorCode		
global int	AirbearingVacuum1_ErrorCode		
global int	AirSupplyPresent_ErrorCode		
global int	VacuumSupplyPresent_ErrorCode	
global int	MotorTemperatureY0_ErrorCode
global int	MotorTemperatureY1_ErrorCode	
global int	MotorTemperatureX_ErrorCode	
global int	Power_ErrorCode	
global int	softEmergencyStop_ErrorCode
global int  WaferClamp_ErrorCode

global int MachineNumber(1)
global real Skew_Offset(1)
global real AxisUOH_focus_DeltaHomingOffset(1)
global real AxisLOH_focus_DeltaHomingOffset(1)
global real AxisY_DeltaHomingOffset(1)
global real AxisX_DeltaHomingOffset(1)


















