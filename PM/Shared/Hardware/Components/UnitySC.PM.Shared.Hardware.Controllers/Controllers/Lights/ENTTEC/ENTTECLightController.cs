using System;
using System.Collections.Generic;
using System.Threading;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Light
{
    public class ENTTECLightController : LightController
    {
        private readonly ENTTECLightControllerConfig _config;
        private readonly FTDI _ftdi;

        private bool _connected = false;
        private byte[] _writeBuffer = new byte[512];

        private const int TIME_REFRESH_MS = 30;

        private readonly Mutex _mutex;

        private readonly Thread _backgroundThread;

        private const byte BITS_8 = 8;
        private const byte STOP_BITS_2 = 2;
        private const byte PARITY_NONE = 0;
        private const UInt16 FLOW_NONE = 0;
        private const byte PURGE_RX = 1;
        private const byte PURGE_TX = 2;

        private readonly Dictionary<string, int> _lightIdToAddress = new Dictionary<string, int>();

        public ENTTECLightController(ControllerConfig controllerConfig, IGlobalStatusServer globalStatusServer, ILogger logger)
            : base(controllerConfig, globalStatusServer, logger)
        {
            _config = (ENTTECLightControllerConfig)controllerConfig;
            _ftdi = new FTDI();
            _mutex = new Mutex();
            _backgroundThread = new Thread(RefreshIO) { Name = "DMX Controller Polling Thread" };
            _backgroundThread.Start();

            foreach (var lightIdlink in _config.LightIdLinks)
            {
                _lightIdToAddress.Add(lightIdlink.LightID, lightIdlink.Address);
            }
        }

        public override void Init(List<Message> initErrors)
        {
            try
            {
                Name = ControllerConfig.Name;
                DeviceID = ControllerConfig.DeviceID;

                Connect();
            }
            catch (Exception Ex)
            {
                initErrors.Add(new Message(MessageLevel.Fatal, Ex.Message, DeviceName));
                return;
            }
        }

        public override void Connect()
        {
            try
            {
                //Always the same description for Enttec controller
                CheckStatus(_ftdi.OpenByDescription("FT232R USB UART"));

                //SETTINGS
                CheckStatus(_ftdi.ResetDevice());
                CheckStatus(_ftdi.SetDivisor(12));
                CheckStatus(_ftdi.SetDataCharacteristics(BITS_8, STOP_BITS_2, PARITY_NONE));
                CheckStatus(_ftdi.SetFlowControl(FLOW_NONE, 0, 0));
                CheckStatus(_ftdi.SetRTS(false));
                CheckStatus(_ftdi.Purge(PURGE_TX));
                CheckStatus(_ftdi.Purge(PURGE_RX));

                _connected = true;
            }
            catch (Exception e)
            {
                new List<Message>() { new Message(MessageLevel.Fatal, e.Message) };
            }
        }

        public override void Connect(string deviceID)
        {
            Connect();
        }

        public override void Disconnect()
        {
            CheckStatus(_ftdi.Close());
            _connected = false;
        }

        public override void Disconnect(string deviceID)
        {
            Disconnect();
        }

        public override bool ResetController()
        {
            return false;
        }

        public override double GetIntensity(string lightID)
        {
            return _writeBuffer[_lightIdToAddress[lightID]] / 255 * 100;
        }

        public override void SetIntensity(string lightID, double intensity)
        {
            if (intensity < 0)
            {
                intensity = 0;
            }
            else if (intensity > 100)
            {
                intensity = 100;
            }

            SetChannelValue(_lightIdToAddress[lightID], Convert.ToByte(intensity / 100 * 255));
        }

        private void SetChannelValue(int channel, byte value)
        {
            _mutex.WaitOne();
            if (_writeBuffer != null)
            {
                _writeBuffer[channel] = value;
            }
            _mutex.ReleaseMutex();
        }

        private void RefreshIO()
        {
            while (true)
            {
                Thread.Sleep(TIME_REFRESH_MS);
                Refresh();
            }
        }

        private void Refresh()
        {
            if (!_connected)
            {
                return;
            }
            try
            {
                _mutex.WaitOne();
                _ftdi.SetBreak(true);
                _ftdi.SetBreak(false);
                byte[] startCode = new byte[1] { 0x00 };
                uint bytesWritten = 0;
                _ftdi.Write(startCode, 1, ref bytesWritten);
                _ftdi.Write(_writeBuffer, 512, ref bytesWritten);
                _mutex.ReleaseMutex();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private static void CheckStatus(FTDI.FT_STATUS status)
        {
            if (status != FTDI.FT_STATUS.FT_OK)
            {
                throw new Exception(status.ToString());
            }
        }
    }
}
