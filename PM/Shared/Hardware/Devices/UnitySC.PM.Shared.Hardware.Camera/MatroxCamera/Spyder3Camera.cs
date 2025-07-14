using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

using Matrox.MatroxImagingLibrary;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.LibMIL;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Communication;

namespace UnitySC.PM.Shared.Hardware.Camera.MatroxCamera
{
    public class Spyder3Camera : MatroxCameraBase
    {
        public override string SerialNumber { get => MdigInquireStringFeature("DeviceSerialNumber"); }

        private SerialPortCommunication _serialPort;
        private readonly Spyder3MatroxCameraConfig _config;

        public Spyder3Camera(Spyder3MatroxCameraConfig config, IGlobalStatusServer globalStatusServer, ILogger logger) : base(config, globalStatusServer, logger)
        {
            _config = config;
            _serialPort = new SerialPortCommunication(_config.Com.Port, _config.Com.BaudRate);
            _serialPort.ConsistentResponsePattern = new Regex(@"(.*\r\n)");
        }

        public override void Init(MilSystem milSystem, int devNumber)
        {
            Invoke(() =>
            {
                base.Init(milSystem, devNumber);
                Logger?.Information("******************************** SPYDER 3 CAMERA STARTUP ********************************");
                lock (DigitizerId_lock)
                {
                    MIL.MdigAlloc(milSystem, devNumber, Path.Combine(_config.DcfFolderPath, _config.DCFName), MIL.M_DEFAULT, ref DigitizerId_lock.Value);

                    MIL.MdigControl(DigitizerId_lock.Value, MIL.M_GRAB_TIMEOUT, _config.GrabTimeout * 1000);
                    MIL.MdigControl(DigitizerId_lock.Value, MIL.M_CORRUPTED_FRAME_ERROR, MIL.M_ENABLE);

                    // Connect to the camera & check we got response and try to set target baud rate if needed
                    Connect();

                    // Camera infos
                    Model = GetModel();
                    Version = GetVersion();

                    // Getting Exposure time range
                    (MinExposureTimeMs, MaxExposureTimeMs) = GetExposureRange();

                    // Getting Analog Gain range
                    (MinGain, MaxGain) = GetAnalogGainRange();

                    Width = 1024;
                    Height = 4096;

                    ColorModes = GetColorModes();

                    SetTriggerMode(TriggerMode.Hardware);

                    // During calibration phase, we configure white and black compensation.
                    // Rebooting the camera causes the camera to load default value (aka nothing)
                    LoadCompensationAlgorithm();
                }

                // Buffer allocation
                AllocateGrabBuffers(20);

                State = new DeviceState(DeviceStatus.Ready);
            });
        }

        #region Public API

        public override double GetExposureTimeMs()
        {
            try
            {
                // TODO : Check if returned exposure time unit is ms
                return double.Parse(_serialPort.Query(new SerialPortQuery<string> { Message = "get set\r", ResponsePattern = new Regex(@"([0-9]{1,}\.[0-9])") }));
            }
            catch (Exception e)
            {
                throw new Exception("Unable to parse ExposureTime: ", e);
            }
        }

        public override void SetExposureTimeMs(double exposureTime_ms)
        {
            // TODO : Check if exposure time unit is ms
            if (exposureTime_ms < MinExposureTimeMs || exposureTime_ms > MaxExposureTimeMs)
                throw new Exception($"Exposure time is out of bound: {exposureTime_ms} ({MinExposureTimeMs}<->{MaxExposureTimeMs})");
            try
            {
                string response = _serialPort.Query(new SerialPortQuery<string> { Message = $"sef {exposureTime_ms}\r", ResponsePattern = new Regex("(.*)") });
                CheckResultOrThrowError(response);
            }
            catch (Exception e)
            {
                throw new Exception("Unable to set exposure time with error: ", e);
            }
        }

        public override double GetGain()
        {
            try
            {
                return double.Parse(_serialPort.Query(new SerialPortQuery<string> { Message = "get sag\r", ResponsePattern = new Regex(@"([0-9]{1,}\.[0-9])") }));
            }
            catch (Exception e)
            {
                throw new Exception("Unable to get gain", e);
            }
        }

        public override void SetGain(double gain)
        {
            if (gain < MinGain || gain > MaxGain)
                throw new Exception($"Gain is out of bound: {gain} ({MinGain}<->{MaxGain})");
            try
            {
                string response = _serialPort.Query(new SerialPortQuery<string> { Message = $"sag 0 {gain}\r", ResponsePattern = new Regex("(.*)") });
                CheckResultOrThrowError(response);
            }
            catch (Exception e)
            {
                throw new Exception("Unable to set set gain ", e);
            }
        }

        public override void SetTriggerMode(TriggerMode mode)
        {
            string value;
            switch (mode)
            {
                case TriggerMode.Off:
                    value = "2";
                    break;

                case TriggerMode.Hardware:
                    value = "6";
                    break;

                default:
                    throw new ApplicationException("unknown trigger mode: " + mode);
            }

            try
            {
                string response = _serialPort.Query(new SerialPortQuery<string> { Message = $"sem {value}\r", ResponsePattern = new Regex("(.*)") });
                CheckResultOrThrowError(response);
            }
            catch (Exception e)
            {
                throw new Exception("Unable to set set gain ", e);
            }
        }

        public override void SoftwareTrigger()
        {
            // Spyder3 don't seems to be able to be Software Trigger
        }

        public override List<string> GetColorModes()
        {
            return new List<string>();
        }

        #endregion Public API

        #region Private API

        private string GetModel()
        {
            // gcm ->
            // "Camera Model No.:               S3-14-01K40-00-R"
            try
            {
                return _serialPort.Query(new SerialPortQuery<string> { Message = "gcm\r", ResponsePattern = new Regex(@"(S3-[A-Z0-9-]+)") });
            }
            catch (Exception e)
            {
                throw new Exception("Unable to extract camera model", e);
            }
        }

        private string GetVersion()
        {
            /* gcv ->
              "Firmware Version:               03-081-20183-08
               CCI Version:                    03-110-20204-06
               FPGA Version:                   03-056-20325-03"
             */
            try
            {
                return _serialPort.Query(new SerialPortQuery<string> { Message = "gcv\r", ResponsePattern = new Regex(@"([0-9-]+)") });
            }
            catch (Exception e)
            {
                throw new Exception("Unable to get camera version", e);
            }
        }

        private string GetSerialNumber()
        {
            // Get serial number: gcs  -> "Camera Serial No.:              13117491"
            try
            {
                return _serialPort.Query(new SerialPortQuery<string> { Message = "gcs\r", ResponsePattern = new Regex(@"([0-9]+)") });
            }
            catch (Exception e)
            {
                throw new Exception("Unable to get camera version", e);
            }
        }

        private (double Min, double Max) GetExposureRange()
        {
            // Exposure range is not available in the documentation, it's just a rough estimation
            return (5.0, 5000.0);
        }

        private (double Min, double Max) GetAnalogGainRange()
        {
            // From Spyder3 documentation
            return (-10.0, 10.0);
        }

        private void LoadCompensationAlgorithm()
        {
            try
            {
                // Load the first set of algorithm compensation that we save during the calibration phase
                string response = _serialPort.Query(new SerialPortQuery<string> { Message = "lpc 1\r", ResponsePattern = new Regex("(.*)") });
                CheckResultOrThrowError(response);
            }
            catch (Exception e)
            {
                throw new Exception("Unable to load compensation algorithm ", e);
            }
        }

        private void Connect()
        {
            _serialPort.Connect();

            // check for camera presence
            try
            {
                _serialPort.Query(new SerialPortQuery<string> { Message = "\r", ResponsePattern = new Regex("(OK)") }); // Contains OK
            }
            catch (TimeoutException) // We probably failed to connect to the camera with the given baud rate, so we try to change it and reconnect with the target value
            {
                _serialPort.Disconnect();
                // Trying to reconnect with default factory value
                var temporarySerialPort = new SerialPortCommunication(_config.Com.Port, 9600); // <- Default factory value
                temporarySerialPort.Connect();
                temporarySerialPort.Query(new SerialPortQuery<string> { Message = "\r", ResponsePattern = new Regex("(OK)") }); // Contains OK
                // We set baud rate to the desired value
                temporarySerialPort.Command(new SerialPortCommand<string> { Message = $"sbr {_config.Com.BaudRate}\r" });
                temporarySerialPort.Disconnect();
                Thread.Sleep(1000);
                _serialPort = new SerialPortCommunication(_config.Com.Port, _config.Com.BaudRate);
                _serialPort.Connect();
            }
            catch (Exception e)
            {
                // Another error
                throw new Exception("Can connect to camera due to exception: ", e);
            }
        }

        #endregion Private API

        /**
         * HandleResponse: check the raw output of the serial command
         * * It logs the warnings
         * * And throw if it doesn't contains OK (It means the command isn't successful)
         */

        private void CheckResultOrThrowError(string rawText)
        {
            // Warning example: Warning 04: Related parameters adjusted

            string[] separators = { "\r\n" };
            string[] lines = rawText.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            if (!lines[lines.Length - 1].StartsWith("OK"))
            {
                throw new Exception($"Failed with error: {lines.Length - 1}");
            }

            foreach (string line in lines)
            {
                if (line.StartsWith("WARNING"))
                {
                    Logger.Warning($"{_config.CameraId} : {line}");
                }
            }
        }
    }
}
