using System;
using System.Collections.Generic;

using ACS.SPiiPlusNET;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Controllers.Controllers.Light
{
    public class ACSLightController : LightController
    {
        protected new ACSLightControllerConfig ControllerConfig => (ACSLightControllerConfig)base.ControllerConfig;

        private Api _channel;

        public ACSLightController(ControllerConfig controllerConfig, IGlobalStatusServer globalStatusServer, ILogger logger) 
            : base(controllerConfig, globalStatusServer, logger)
        {
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
            OpenEthernetConnection();
            if (_channel == null)
                throw new Exception(FormatMessage("Controller initialization Process fails"));
        }

        public override void Connect(string deviceID)
        {
            Connect();
        }

        public override void Disconnect()
        {
            try
            {
                if (_channel != null)
                {
                    _channel.CloseMessageBuffer();

                    base.Logger?.Information("ACSLightController. Close communication controller");
                    _channel.CloseComm();
                    _channel = null;
                }
            }
            catch
            {
                base.Logger?.Error("Close com ACSLightController failed");
            }
        }

        public override void Disconnect(string deviceID)
        {
            Disconnect();
        }

        public override bool ResetController()
        {
            return false;
        }

        private void OpenEthernetConnection()
        {
            try
            {
                string ethernetIP = ControllerConfig.EthernetCom != null && !string.IsNullOrWhiteSpace(ControllerConfig.EthernetCom.IP) ? ControllerConfig.EthernetCom.IP : string.Empty;
                int ethernetPort = ControllerConfig.EthernetCom != null ? ControllerConfig.EthernetCom.Port : 0;
                _channel = new Api();

                CheckCommunicationChannelToACS();

                if (string.IsNullOrEmpty(ethernetIP))
                    throw (new Exception(FormatMessage("String ethernetIP is empty")));

                if (ethernetPort == 0)
                    throw (new Exception(FormatMessage("ethernetPort is empty")));

                lock (_channel)
                {
                    _channel.OpenCommEthernetTCP(ethernetIP, ethernetPort);
                }
            }
            catch (ACSException ACSEx)
            {
                base.Logger?.Error(FormatMessage("OpenEthernetConnection - ACSException: " + ACSEx.Message));
                throw;
            }
            catch (Exception Ex)
            {
                base.Logger?.Error(FormatMessage("OpenEthernetConnection - Exception: " + Ex.Message));
                throw;
            }
        }

        public override double GetIntensity(string lightID)
        {
            return (double)_channel.ReadVariable(lightID);
        }

        public override void SetIntensity(string lightID, double intensity)
        {
            _channel.WriteVariable(intensity, lightID);
        }

        private void CheckCommunicationChannelToACS()
        {
            if (_channel != null)
                return;

            string errorMsg = FormatMessage("Communication channel to ACS is null");
            base.Logger?.Error(errorMsg);
            throw (new Exception(errorMsg));
        }

        public void SetPower(string lightID, int power)
        {
            _channel.WriteVariable(power, lightID);
        }        
    }
}
