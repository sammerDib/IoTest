using System;
using System.Collections.Generic;
using System.Timers;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.IOComponent;
using UnitySC.PM.Shared.Hardware.Service.Interface.Controller;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using System.Threading.Tasks;

namespace UnitySC.PM.Shared.Hardware.Controllers
{
    public class NICouplerDummyController : ControllerBase, IControllerIO
    {
        private readonly NICouplerControllerConfig _config;
        private readonly List<IO> _ios;

        private Dictionary<string, DataAttribute> _dataAttributeValues;
        private Timer _refreshTimer;
        public Dictionary<string, Input> NameToInput { get; set; }
        public Dictionary<string, Output> NameToOutput { get; set; }

        public NICouplerDummyController(NICouplerControllerConfig config, IGlobalStatusServer globalStatusServer, ILogger logger) :
                   base(config, globalStatusServer, logger)
        {
            _config = config ?? throw new ArgumentNullException("RackNI configuration is null");
            _ios = _config.IOList;
            if (_ios.Count == 0)
            {
                throw new Exception("List input and output is empty");
            }
            //Init Input and Output list
            if (config is IOControllerConfig ioControllerConfig && ioControllerConfig != null)
            {
                NameToInput = ioControllerConfig.GetInputs();
                NameToOutput = ioControllerConfig.GetOutputs();
            }
        }

        ~NICouplerDummyController()
        {
        }

        public List<IO> GetIOs()
        {
            return _ios;
        }

        private bool DeviceIsFound()
        {
            return true;
        }

        private bool DeviceIsFound(string deviceID)
        {
            return true;
        }

        private void RefreshTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _refreshTimer.Stop();
            var dataAttributesChanged = new List<DataAttribute>();

            foreach (var io in _ios)
            {
                if (io is DigitalInput digitalInput)
                {
                    var value = _dataAttributeValues[io.Name];
                    bool newValue = DigitalRead(digitalInput);
                    value.Changed = value.DigitalValue != newValue;
                    value.DigitalValue = newValue;
                    if (value.Changed)
                    {
                        dataAttributesChanged.Add(value);
                    }
                }
            }
            if (dataAttributesChanged.Count > 0)
            {
                Messenger.Send(new DataAttributesControllerMessage() { DataAttributes = dataAttributesChanged });
            }

            _refreshTimer.Start();
        }

        private bool ResetDevice(string deviceId)
        {
            foreach (var io in _ios)
            {
                if (io.Name == deviceId)
                {
                    return true;
                }
            }
            return false;
        }

        #region override

        private IMessenger _messenger;

        public IMessenger Messenger
        {
            get
            {
                if (_messenger == null)
                    _messenger = ClassLocator.Default.GetInstance<IMessenger>();
                return _messenger;
            }
        }

        private void BuildIODictionnary(IO io)
        {
            if (io is DigitalInput)
            {
                _dataAttributeValues.Add(io.Name,
                    new DataAttribute(io.Name, AttributeType.DigitalIO, "DI", _config.DeviceID, io.Address.Module, io.Address.Channel));
            }
            else if (io is DigitalOutput)
            {
                _dataAttributeValues.Add(io.Name,
                    new DataAttribute(io.Name, AttributeType.DigitalIO, "DO", _config.DeviceID, io.Address.Module, io.Address.Channel));
            }
            else if (io is AnalogInput)
            {
                _dataAttributeValues.Add(io.Name,
                    new DataAttribute(io.Name, AttributeType.AnalogicIO, "AI", _config.DeviceID, io.Address.Module, io.Address.Channel));
            }
            else if (io is AnalogOutput)
            {
                _dataAttributeValues.Add(io.Name,
                    new DataAttribute(io.Name, AttributeType.AnalogicIO, "AO", _config.DeviceID, io.Address.Module, io.Address.Channel));
            }
        }

        public override void Init(List<Message> initErrors)
        {
            _dataAttributeValues = new Dictionary<string, DataAttribute>();
            Logger.Information("Init NICouplerController as dummy");

            foreach (var io in _ios)
            {
                BuildIODictionnary(io);
            }

            int time_ms = 3000;
            _refreshTimer = new Timer(time_ms);
            _refreshTimer.Elapsed += RefreshTimer_Elapsed;

            try
            {
                Connect();
            }
            catch (Exception Ex)
            {
                initErrors.Add(new Message(MessageLevel.Error, "Connection failed : " + Ex.Message, DeviceID));
            }
        }

        public override bool ResetController()
        {
            return true;
        }

        public override void Connect()
        {
            if (!DeviceIsFound())
            {
                throw new Exception($"Device {_config.DeviceID} is not found at address {_config.Port}.");
            }
            _refreshTimer.Start();
        }

        public override void Connect(string deviceId)
        {
            if (!DeviceIsFound(deviceId))
            {
                throw new Exception($"Device {_config.DeviceID} is not found at address {_config.Port}.");
            }
            _refreshTimer.Start();
        }

        public override void Disconnect()
        {
            ResetController();
        }

        public override void Disconnect(string deviceID)
        {
            ResetDevice(deviceID);
        }

        #endregion override

        #region Digital

        public bool DigitalRead(DigitalInput digitalInput)
        {
            var dataAttributeValue = _dataAttributeValues[digitalInput.Name];
            return dataAttributeValue.DigitalValue;
        }

        public void DigitalWrite(DigitalOutput digitalOutput, bool value)
        {
            var dataAttributeValue = _dataAttributeValues[digitalOutput.Name];

            dataAttributeValue.Changed = dataAttributeValue.DigitalValue != value;
            dataAttributeValue.DigitalValue = value;
            if (dataAttributeValue.Changed)
            {
                Messenger.Send(new DataAttributesControllerMessage()
                {
                    DataAttributes = new List<DataAttribute> { dataAttributeValue }
                });
            }
        }

        public double AnalogRead(AnalogInput input)
        {
            var dataAttributeValue = _dataAttributeValues[input.Name];
            return dataAttributeValue.AnalogValue;
        }

        public void AnalogWrite(AnalogOutput output, double value)
        {
            var dataAttributeValue = _dataAttributeValues[output.Name];

            dataAttributeValue.Changed = dataAttributeValue.AnalogValue != value;
            dataAttributeValue.AnalogValue = value;
            if (dataAttributeValue.Changed)
            {
                Messenger.Send(new DataAttributesControllerMessage()
                {
                    DataAttributes = new List<DataAttribute> { dataAttributeValue }
                });
            }
        }

        #endregion Digital

        public Input GetInput(string name)
        {
            if (!NameToInput.ContainsKey(name))
            {
                Logger.Error($"{name} is not known in the configuration. Check in the AnaHardwareConfiguration file.");
            }
            return NameToInput[name];
        }

        public Output GetOutput(string name)
        {
            if (NameToOutput.ContainsKey(name))
            {
                return NameToOutput[name];
            }
            throw new Exception(
                $"A device tried to get an Output named: {name} but it does not exist in the {ControllerConfiguration.Name} controller configuration");
        }

        public Task StartRefreshIOStatesTask()
        {
            throw new NotImplementedException();
        }

        public void StopRefreshIOStatesTask()
        {
            throw new NotImplementedException();
        }
    }
}
