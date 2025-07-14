using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

using CommunityToolkit.Mvvm.Messaging;

using NationalInstruments.DAQmx;

using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Chambers;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chamber;
using UnitySC.PM.Shared.Hardware.Service.Interface.IOComponent;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Controllers
{
    public class NICouplerController : ChamberController, IChamberController
    {
        /*Allows to read or write the "devices" connected to the RackNI.
         * To use it you need a configuration file with all the necessary information.
         * Retrieve the desired instance with the getInput() or get Output() depending on the type of IO.
         * Examples are available in the associated functional tests
         * Then use writeValue() to write on outputs or readValue() for inputs.
         */

        private readonly NICouplerControllerConfig _config;
        private readonly List<IO> _ios;

        private Dictionary<string, Task> _ioToTask = new Dictionary<string, Task>();

        private Dictionary<string, DataAttribute> _dataAttributeValues;
        private Timer _refreshTimer;
        private Dictionary<string, Task> _diTaskDico;
        private Dictionary<string, Task> _doTaskDico;
        private Dictionary<string, Task> _aiTaskDico;
        private Dictionary<string, Task> _aoTaskDico;
        private readonly object _lockObject = new object();


        /*
         * Information for developers.
         * If an exception is thrown telling you that it cannot find the daqmx.dll, common.dll or a dll related to NationnalInstruments.
         * Here is a hint for the procedure to follow.
         * Uninstall the NI suite with the package uninstaller.
         * Then go to the NI site and download NIDAQmx version 20.1. Make sure that everything related to dotNet is checked in the installation.
         * If it still doesn't work, go to all Programs x64/NationalInstruments/Measurement StudioVS2010/DotNet/Assenblies(64)/Current
         * and replace the existing dll in the project.  If there is still an exception, try with the other dll of MeasurementStudio or MeasurementStudioVS2012
         */

        public NICouplerController(NICouplerControllerConfig config, IGlobalStatusServer globalStatusServer, ILogger logger) :
                   base(config, globalStatusServer, logger)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config), "RackNI configuration is null");
            _ios = _config.IOList;
            if (_ios.Count == 0)
            {
                throw new InvalidOperationException("List input and output is empty");
            }

            //Init Input and Output list
            if (_config is IOControllerConfig ioControllerConfig && ioControllerConfig != null)
            {
                NameToInput = ioControllerConfig.GetInputs();
                NameToOutput = ioControllerConfig.GetOutputs();
            }
        }

        public override void Init(List<Message> initErrors)
        {
            _dataAttributeValues = new Dictionary<string, DataAttribute>();
            _diTaskDico = new Dictionary<string, Task>();
            _doTaskDico = new Dictionary<string, Task>();
            _aiTaskDico = new Dictionary<string, Task>();
            _aoTaskDico = new Dictionary<string, Task>();

            foreach (var io in _ios)
            {
                CreateTaskForChannel(io);
                BuildIODictionnary(io);
                OpenChannel(io);
                InitOutputValues(io);
            }

            int time_ms = 300;
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
            lock (_lockObject)
            {
                _ioToTask.Values.ToList().ForEach(task => { task.Stop(); task.Dispose(); });
                _ioToTask = null;
                _refreshTimer.Stop();
                return true;
            }
        }

        public override void Connect()
        {
            //Attention, if you are unable to open the channel,
            //it is necessary to verify using the NIMAX tool that the port is
            //the same as the one specified in the configuration (DEV1 or DEV2, etc.).
            if (!DeviceIsFound())
            {
                throw new ArgumentNullException($"Device {_config.DeviceID} is not found at address {_config.Port}.");
            }
            _refreshTimer.Start();
        }

        public override void Connect(string deviceId)
        {
            if (!DeviceIsFound(deviceId))
            {
                throw new ArgumentNullException($"Device {_config.DeviceID} is not found at address {_config.Port}.");
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

        public bool CdaIsReady()
        {
            throw new NotImplementedException();
        }

        public bool IsInMaintenance()
        {
            throw new NotImplementedException();
        }

        public bool PrepareToTransferState()
        {
            throw new NotImplementedException();
        }

        private bool DeviceIsFound()
        {
            bool deviceIsFound = false;

            foreach (string devicesName in DaqSystem.Local.Devices)
            {
                if (devicesName == _config.Port)
                {
                    deviceIsFound = true;
                }
            }
            return deviceIsFound;
        }

        private bool DeviceIsFound(string deviceID)
        {
            bool deviceIsFound = false;

            foreach (string devicesName in DaqSystem.Local.Devices)
            {
                if (devicesName == deviceID)
                {
                    deviceIsFound = true;
                }
            }
            return deviceIsFound;
        }

        private void InitOutputValues(IO io)
        {
            if (io is DigitalOutput digitalOutput)
            {
                DigitalWrite(digitalOutput, false);
            }
            if (io is AnalogOutput analogOutput)
            {
                AnalogWrite(analogOutput, 0);
            }
        }

        private void CreateTaskForChannel(IO io)
        {
            string taskName = "Task" + io.Name;
            var task = new Task(taskName);
            _ioToTask.Add(io.Name, task);
            if (io is DigitalInput)
            {
                _diTaskDico.Add(io.Name, task);
            }
            else if (io is DigitalOutput)
            {
                _doTaskDico.Add(io.Name, task);
            }
            else if (io is AnalogInput)
            {
                _aiTaskDico.Add(io.Name, task);
            }
            else if (io is AnalogOutput)
            {
                _aoTaskDico.Add(io.Name, task);
            }
        }

        private void OpenChannel(IO io)
        {
            _ioToTask.TryGetValue(io.Name, out var task);

            string channelName = CreateLine(io);

            // Create digital input or outup channel to measure digital signals.
            if (io is Input)
            {
                task.DIChannels.CreateChannel(channelName, "", 0);
            }
            else
            {
                task.DOChannels.CreateChannel(channelName, "", 0);
            }

            task.Start();
        }

        private string CreateLine(IO io)
        {
            if (io == null)
            {
                throw new InvalidOperationException("Channel(s) definition(s)  are not referenced in the configuration");
            }

            string device = _config.Port;
            string portNumber = io.Address.Module.ToString();
            string lineNumber = io.Address.Channel.ToString();

            if (!VerifyExistenceOfChannel(portNumber, lineNumber))
            {
                throw new ArgumentException($"The channel{io.Name} cannot be created because the specified " +
                    $"port: {portNumber} or line: {lineNumber} does not exist in the current RackNI configuration");
            }

            return $"{device}/port{portNumber}/line{lineNumber}";
        }

        private bool VerifyExistenceOfChannel(string port, string line)
        {
            string[] physicalChannel = DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.DOLine, PhysicalChannelAccess.All);
            foreach (string channel in physicalChannel)
            {
                if (channel[9].ToString() == port && channel[15].ToString() == line)
                {
                    return true;
                }
            }
            return false;
        }

        private void ResetDevice(string deviceId)
        {
            foreach (var keyValue in _ioToTask)
            {
                if (keyValue.Key == deviceId)
                {
                    keyValue.Value.Dispose();
                }
            }
        }

        private void RefreshTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _refreshTimer.Stop();
            var dataAttributesChanged = new List<DataAttribute>();

            foreach (var di in _diTaskDico)
            {
                var task = di.Value;
                var value = _dataAttributeValues[di.Key];
                lock (_lockObject)
                {
                    bool newValue = DigitalRead(task.Stream);

                    value.Changed = value.DigitalValue != newValue;
                    value.DigitalValue = newValue;
                }
                if (value.Changed)
                {
                    dataAttributesChanged.Add(value);
                }
            }
            foreach (var ai in _aiTaskDico)
            {
                var task = ai.Value;
                var value = _dataAttributeValues[ai.Key];
                lock (_lockObject)
                {
                    double newValue = AnalogRead(task.Stream);

                    value.Changed = value.AnalogValue != newValue;
                    value.AnalogValue = newValue;
                }
                if (value.Changed)
                {
                    dataAttributesChanged.Add(value);
                }
            }
            if (dataAttributesChanged.Count > 0)
            {
                Messenger.Send(new DataAttributesControllerMessage() { DataAttributes = dataAttributesChanged });
            }

            _refreshTimer.Start();
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

        private Task GetTask(string name, Dictionary<string, Task> dico)
        {
            bool succes = dico.TryGetValue(name, out var task);
            if (!succes)
            {
                throw new InvalidOperationException($"Could not get task {name}.");
            }
            return task;
        }

        private bool DigitalRead(DaqStream stream)
        {
            var reader = new DigitalSingleChannelReader(stream);
            return reader.ReadSingleSampleSingleLine();
        }

        private void DigitalWrite(DaqStream stream, bool value)
        {
            var writer = new DigitalSingleChannelWriter(stream);
            writer.WriteSingleSampleSingleLine(true, value);
        }

        private double AnalogRead(DaqStream stream)
        {
            var reader = new AnalogSingleChannelReader(stream);
            return reader.ReadSingleSample();
        }

        private void AnalogWrite(DaqStream stream, double value)
        {
            var writer = new AnalogSingleChannelWriter(stream);
            writer.WriteSingleSample(true, (uint)value);
        }

        public override void TriggerUpdateEvent()
        {
           
        }
    }
}
