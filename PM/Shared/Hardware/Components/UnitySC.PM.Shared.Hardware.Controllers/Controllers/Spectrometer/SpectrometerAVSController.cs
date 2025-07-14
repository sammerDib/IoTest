using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Spectrometer;
using UnitySC.PM.Shared.Hardware.Spectrometer;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

using static UnitySC.PM.Shared.Hardware.Spectrometer.Avaspec;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Spectrometer
{
    public class SpectrometerAVSController : ControllerBase
    {
        private readonly SpectrometerAvantesControllerConfig _config;
        private int _handleDevice;
        private DeviceConfigType _deviceConfig;
        private AvsIdentityType _device;
        private PixelArrayType _spectrum;
        private object _signallock = new object();
        private SpectroSignal _spectroSignal;
        private uint _time = 0;

        private const long TimeoutSpinTrig = 15000; //ms
        private long _timeoutWaitMeasure = TimeoutSpinTrig;

        private CancellationTokenSource _ctsAcquisition;
        private Task _handleTaskAcquisition;

        public short NbScan { get; set; } = 1;
        public bool IsTriggered { get; set; } = false;

        public SpectrometerAVSController(ControllerConfig controllerConfig, IGlobalStatusServer globalStatusServer, ILogger logger) : base(controllerConfig, globalStatusServer, logger)
        {
            _handleDevice = Avaspec.INVALID_AVS_HANDLE_VALUE;
            _config = (SpectrometerAvantesControllerConfig)controllerConfig;
            _spectroSignal = new SpectroSignal();
        }

        public override void Connect()
        {
            if(_config.Port == 0)
            {
                Logger.Warning($"[SpectrometerAVSController] Port for spectrometer controller in configuration file is empty");
            }
            Logger.Information($"[SpectrometerAVSController]Initialissation spectrometer started. Communication mode is: {_config.CommunicationMode}. Port for connexion is : {_config.Port}");
            int avsPort = AVS_Init(_config.Port);

            const int MAX_NR_DEVICES = 20;
            var answer = new BroadcastAnswerType[MAX_NR_DEVICES];
            string errorMessage = "";
            if (avsPort > 0)
            {
                var requiredSize = GetDeviceListSize(answer);
                if (requiredSize > 0)
                {
                    // Invoke AVS_GetList() twice, the first call is only needed to determine the Id array size used within the DLL
                    int nbDevices = AVS_GetList(0, ref requiredSize, null);
                    uint size = requiredSize;
                    var devicesList = new AvsIdentityType[nbDevices];
                    nbDevices = AVS_GetList(size, ref requiredSize, devicesList);
                    if (nbDevices <= 0)
                    {
                        errorMessage = "Spectrometer not found";
                        Logger.Error(errorMessage);
                        throw new Exception(errorMessage);
                    }
                    _device = (
                                from d in devicesList.ToList()
                                where d.m_SerialNumber == _config.SerialNumber
                                select d
                           ).FirstOrDefault();

                    //Select the spectrometers you want to use
                    if (_device.m_SerialNumber == null)
                    {
                        errorMessage = $" No spectrometer found for this serial number {_config.SerialNumber}";
                        Logger.Error(errorMessage);
                        throw new Exception(errorMessage);
                    }
                    _device.m_UserFriendlyName = _device.m_SerialNumber.ToString();

                    //Activate Spectrometer for communication
                    _handleDevice = AVS_Activate(ref _device);
                    if (_handleDevice == INVALID_AVS_HANDLE_VALUE)
                    {
                        errorMessage = $"Error opening spectrometer {_config.SerialNumber}";
                        Logger.Error(errorMessage);
                        throw new Exception(errorMessage);
                    }

                    //Set to 16 bit mode
                    if (AVS_UseHighResAdc(_handleDevice, true) != ERR_SUCCESS)
                    {
                        errorMessage = $"High Res mode not supported for spectrometer {_config.SerialNumber}";
                        Logger.Error(errorMessage);
                        throw new Exception(errorMessage);
                    }

                    GetWave();

                    //Get parameters
                    _deviceConfig = new DeviceConfigType();

                    int responseCode = AVS_GetParameter(_handleDevice, size, ref size, ref _deviceConfig);

                    // Size output by reference
                    if (responseCode == ERR_INVALID_SIZE)
                        responseCode = AVS_GetParameter(_handleDevice, size, ref size, ref _deviceConfig);
                    if (responseCode != ERR_SUCCESS)
                    {
                        errorMessage = "[SpectrometerController] GetParameters failed";
                        Logger.Error(errorMessage);
                        throw new Exception(errorMessage);
                    }

                    //Register a notification window handle with AVS_Register() to detect device attachment/removal
                    AVS_Register(IntPtr.Zero);
                }
            }
            else
            {
                if (avsPort == 0)
                {
                    errorMessage = "No spectrometer found on network!";
                }
                else if (avsPort == ERR_ETHCONN_REUSE)
                {
                    errorMessage = "Server error; another instance is running!";
                }
                else
                {
                    errorMessage = "Server error; open communication failed with AVS_Init()";
                }
                Logger.Error(errorMessage);
                AVS_Done();
                throw new Exception(errorMessage);
            }
        }

        private void GetWave()
        {
            ushort nrpixel = 0;
            var resp = AVS_GetNumPixels(_handleDevice, ref nrpixel);
            if (resp != ERR_SUCCESS)
            {
                throw new Exception("[SpectrometerAvantes] GetNumPixel() error");
            }
            PixelArrayType m_Lambda = new PixelArrayType();
            resp = AVS_GetLambda(_handleDevice, ref m_Lambda);
            if (resp != ERR_SUCCESS)
            {
                throw new Exception("[SpectrometerAvantes] GetLambda() error");
            }
            lock (_signallock)
            { _spectroSignal.Wave = (from _ in m_Lambda.Value select _).Take(nrpixel).ToList(); }
        }

        private uint GetDeviceListSize(BroadcastAnswerType[] answer)
        {
            uint requiredSize = 0;
            if (CommunicationMode.USB == _config.CommunicationMode)
            {
                int nbDevices = AVS_UpdateUSBDevices();
                Array.Resize(ref answer, nbDevices);
                return (uint)nbDevices;
            }
            else if (CommunicationMode.Ethernet == _config.CommunicationMode)
            {
                int ethDevListSize = AVS_UpdateETHDevices(0, ref requiredSize, null);
                Array.Resize(ref answer, ethDevListSize);
                uint size = requiredSize;
                AVS_UpdateETHDevices(size, ref requiredSize, answer);
                return requiredSize;
            }
            return requiredSize;
        }

        public override void Connect(string deviceId)
        {
            Connect();
        }

        public override void Disconnect()
        {
            AVS_Done();
        }

        public override void Disconnect(string deviceID)
        {
            Disconnect();
        }

        public override void Init(List<Message> initErrors)
        {
            if (!(_config is SpectrometerAvantesControllerConfig))
                throw new Exception("Invalid spectrometer controller configuration");

            Name = _config.Name;
            DeviceID = _config.DeviceID;

            Logger.Information("Init the device " + _config.DeviceID);
            Connect();
        }

        public override bool ResetController()
        {
            int l_Res = AVS_ResetDevice(_handleDevice);
            if (ERR_SUCCESS != l_Res)
            {
                Logger.Error($"Error in AVS_ResetDevice, code: {l_Res}");
                return false;
            }
            Logger.Information($"Spectrometer {_config.Name} Successfully reset");
            return true;
        }

        public SpectroSignal DoMeasure(SpectrometerParamBase param)
        {
            PrepareMeasure(param);
            var signal = Measure();
            NotifyRawSignalUpdated(signal);
            return signal;
        }

        private SpectroSignal Measure()
        {
            _spectrum = new PixelArrayType();
            int res = AVS_Measure(_handleDevice, IntPtr.Zero, NbScan);
            if (ERR_SUCCESS != res)
            {
                Logger.Error("Error in AVS_Measure: " + res.ToString());
                throw new Exception("[SpectrometerAvantesController] AVS_Measure failure. ");
            }

            bool dataIsReady = false;
            Stopwatch sw = Stopwatch.StartNew();
            do
            {
                Thread.Sleep(5); // avoid 100% cpu overload
                dataIsReady = AVS_PollScan(_handleDevice) != 0;
            }
            while (!dataIsReady && sw.ElapsedMilliseconds < _timeoutWaitMeasure);

            if (!dataIsReady)
            {
                Logger.Error("Timeout data is not ready");
                throw new Exception("[SpectrometerAVSController] AVS_PollScan data not ready or measurement has been stopped. ");
            }

            var rep = AVS_GetScopeData(_handleDevice, ref _time, ref _spectrum);
            if (rep != ERR_SUCCESS)
            {
                Logger.Error("Error getScopeData");
                throw new Exception("[SpectrometerAVSController] Error getScopeData. ");
            }
            Logger.Verbose("Measure is completed");

            SpectroSignal spectrosignal;
            lock (_signallock)
            {
                _spectroSignal.RawValues = _spectrum.Value.ToList();
                spectrosignal = (SpectroSignal)_spectroSignal.Clone();
            }
            return spectrosignal;
        }

        public void StartMeasure(SpectrometerParamBase param)
        {
            //StopMeasure();
            //Thread.Sleep(500);

            PrepareMeasure(param);

            _spectrum = new PixelArrayType();
            int res = AVS_Measure(_handleDevice, IntPtr.Zero, NbScan); // INFINITY of measurement == -1
            if (ERR_SUCCESS != res)
            {
                Logger.Error("Error in AVS_Measure: " + res.ToString());
                throw new Exception($"[SpectrometerAvantesController] StartMeasure continous error : {res.ToString()}. ");
            }

            _ctsAcquisition = new CancellationTokenSource();
            _handleTaskAcquisition = new Task(() => { ContinuousMeasure(); }, _ctsAcquisition.Token, TaskCreationOptions.LongRunning);
            _handleTaskAcquisition.Start();
        }

        private void ContinuousMeasure()
        {
            try
            {
                var token = _ctsAcquisition.Token;
                while (!token.IsCancellationRequested)
                {
                    bool dataIsReady = false;
                    do
                    {
                        Thread.Sleep(20); // avoid 100% cpu overload
                        dataIsReady = AVS_PollScan(_handleDevice) != 0;
                    }
                    while (!dataIsReady && !token.IsCancellationRequested);

                    if (!token.IsCancellationRequested)
                    {
                        var rep = AVS_GetScopeData(_handleDevice, ref _time, ref _spectrum);
                        if (rep != ERR_SUCCESS)
                        {
                            throw new Exception($"continous Error getScopeData : {rep}");
                        }

                        lock (_signallock)
                        {
                            _spectroSignal.RawValues = _spectrum.Value.ToList();
                        }
                        NotifyRawSignalUpdated(_spectroSignal);
                    }

                    var cancellationTriggered = token.WaitHandle.WaitOne(200);
                    if (cancellationTriggered)
                    {
                        // Cleanup
                        break;
                    }

                    // rearm acquisition
                    int res = AVS_Measure(_handleDevice, IntPtr.Zero, NbScan); // INFINITY of measurement == -1
                    if (ERR_SUCCESS != res)
                    {
                        throw new Exception($"rearm acq error : {res}. ");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"in continous task {ex.Message}");

                StopMeasure();
            }
            finally
            {
                _ctsAcquisition?.Cancel();
            }
        }

        public SpectroSignal GetLastMeasure()
        {
            SpectroSignal spectrosignal;
            lock (_signallock)
            {
                spectrosignal = (SpectroSignal)_spectroSignal.Clone();
            }
            return spectrosignal;
        }

        public void StopMeasure()
        {
            _ctsAcquisition?.Cancel();

            int res = AVS_StopMeasure(_handleDevice);
            if (ERR_SUCCESS != res)
                Logger.Error("Error in AVS_StopMeasure: " + res.ToString());
        }

        private void PrepareMeasure(SpectrometerParamBase param, bool darkEnable = false)
        {
            var prepareMeasData = _deviceConfig.m_StandAlone.m_Meas;
            prepareMeasData.m_IntegrationTime = (float)param.IntegrationTime_ms;
            prepareMeasData.m_NrAverages = (uint)param.NbAverage;
            prepareMeasData.m_CorDynDark.m_Enable = darkEnable ? (byte)1 : (byte)0;

            if (IsTriggered)
            {
                prepareMeasData.m_Trigger.m_Mode = 1;
                _timeoutWaitMeasure = (NbScan > 1) ? TimeoutSpinTrig : (long)Math.Ceiling((prepareMeasData.m_IntegrationTime * prepareMeasData.m_NrAverages) + 500);
            }
            else
            {
                prepareMeasData.m_Trigger.m_Mode = 0;
                _timeoutWaitMeasure = (long)Math.Ceiling((prepareMeasData.m_IntegrationTime * prepareMeasData.m_NrAverages) + 1000);
            }
            prepareMeasData.m_Trigger.m_Source = 0;
            prepareMeasData.m_Trigger.m_SourceType = 0;

            int res = AVS_PrepareMeasure(_handleDevice, ref prepareMeasData);
            if (ERR_SUCCESS != res)
            {
                Logger.Error("Error in AVS_PrepareMeasure: " + res.ToString());
            }
        }

        public void Active(bool active)
        {
            if (active)
                Activate();
            else
                Deactivate();
        }

        private void Activate()
        {
            _handleDevice = AVS_Activate(ref _device);
        }

        public void Deactivate()
        {
            AVS_Deactivate(_handleDevice);
            _handleDevice = Avaspec.INVALID_AVS_HANDLE_VALUE;
        }

        private void NotifyRawSignalUpdated(SpectroSignal newRawSignal)
        {
            var spectroServiceCallback = ClassLocator.Default.GetInstance<ISpectroServiceCallbackProxy>();
            spectroServiceCallback.RawMeasuresCallback(newRawSignal);
        }
    }
}
