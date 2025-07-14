using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.IOComponent;
using UnitySC.PM.Shared.Hardware.Service.Interface.Controller;
using UnitySC.PM.Shared.ReformulationMessage;
using UnitySC.Shared.Data;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ControllerService : DuplexServiceBase<IControllerServiceCallback>, IControllerService
    {
        private readonly HardwareManager _hardwareManager;
        private readonly object _lock = new object();

        public ControllerService(ILogger logger, HardwareManager hardwareManager)
            : base(logger, ExceptionType.HardwareException)
        {
            _hardwareManager = hardwareManager;
            IMessenger messenger = ClassLocator.Default.GetInstance<IMessenger>();
            messenger.Register<DataAttributesControllerMessage>(this, (r, m) => { UpdateStatusOfIos(m.DataAttributes); });
        }

        private void UpdateStatusOfIos(List<DataAttribute> dataAttributes)
        {
            InvokeCallback(i => i.UpdateStatusOfIosCallback(dataAttributes));
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                lock (_lock)
                {
                    _logger.Information("Subscribed to controller change");
                    Subscribe();
                }
            });
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                lock (_lock)
                {
                    _logger.Information("Unsubscribed to controller change");
                    Unsubscribe();
                }
            });
        }

        public Response<List<string>> GetControllersIds()
        {
            return InvokeDataResponse(messageContainer =>
            {
                if (_hardwareManager.Controllers.Count == 0)
                    return null;

                List<string> controllers = (from value in _hardwareManager.Controllers.Values
                                            select value.DeviceID).ToList<string>();

                return controllers;
            });
        }

        public Response<List<IOControllerConfig>> GetControllersIOs()
        {
            return InvokeDataResponse(messageContainer =>
            {
                if (_hardwareManager.Controllers.Count == 0)
                    return null;

                var list = new List<IOControllerConfig>();

                foreach (var controller in _hardwareManager.Controllers.Values)
                {
                    var ioController = controller.ControllerConfiguration;
                    if (ioController is IOControllerConfig && ioController != null)
                    {
                        list.Add((IOControllerConfig)ioController);
                    }
                }

                return list;
            }
                );
        }

        public Response<ControllerConfig> GetControllerById(string deviceId)
        {
            return InvokeDataResponse(messageContainer =>
            {
                if (_hardwareManager.Controllers.Count == 0)
                    return null;

                var controllerQuery = (from value in _hardwareManager.Controllers.Values
                                       where value.DeviceID == deviceId
                                       select value.ControllerConfiguration).OfType<ControllerConfig>().FirstOrDefault();

                return controllerQuery;
            }
               );
        }

        public Response<bool> GetDigitalIoState(string deviceId, string ioId)
        {
            return InvokeDataResponse(messageContainer =>
            {
                bool bState = false;
                try
                {
                    var controllerBase = _hardwareManager.Controllers.Values.Where(x => x.DeviceID == deviceId)?.FirstOrDefault();
                    if ((controllerBase != null) && (controllerBase is IControllerIO controllerIO))
                    {
                        var digitalIo = (DigitalInput)controllerIO.GetInput(ioId);
                        if (digitalIo != null)
                        {
                            bState = controllerIO.DigitalRead(digitalIo);
                        }
                    }
                    return bState;
                }
                catch (Exception exception)
                {
                    ReformulationMessage(messageContainer, ioId, exception.Message);
                    _logger.Error(exception, "GetDigitalIoState");
                    return bState;
                }
            }
               );
        }

        public Response<double> GetAnalogIoValue(string deviceId, string ioId)
        {
            return InvokeDataResponse(messageContainer =>
            {
                double dState = 0.0;
                try
                {
                    var controllerBase = _hardwareManager.Controllers.Values.Where(x => x.DeviceID == deviceId)?.FirstOrDefault();
                    if ((controllerBase != null) && (controllerBase is IControllerIO controllerIO))
                    {
                        var analogicIO = (AnalogInput)controllerIO.GetInput(ioId);
                        if (analogicIO != null)
                        {
                            dState = controllerIO.AnalogRead(analogicIO);
                        }
                    }
                    return dState;
                }
                catch (Exception exception)
                {
                    ReformulationMessage(messageContainer, ioId, exception.Message);
                    _logger.Error(exception, "GetAnalogIoValue");
                    return dState;
                }
            }
               );
        }

        public Response<VoidResult> SetDigitalIoState(string deviceId, string ioId, bool value)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                try
                {
                    var controllerBase = _hardwareManager.Controllers.Values.Where(x => x.DeviceID == deviceId)?.FirstOrDefault();
                    if ((controllerBase != null) && (controllerBase is IControllerIO controllerIO))
                    {
                        var digitalIo = (DigitalOutput)controllerIO.GetOutput(ioId);
                        if (digitalIo != null)
                        {
                            controllerIO.DigitalWrite(digitalIo, value);
                        }
                    }
                }
                catch (Exception exception)
                {
                    ReformulationMessage(messageContainer, ioId, exception.Message);
                    _logger.Error(exception, "SetDigitalIoState");
                }
            });
        }

        public Response<VoidResult> SetAnalogIoValue(string deviceId, string ioId, double value)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                try
                {
                    var controllerBase = _hardwareManager.Controllers.Values.Where(x => x.DeviceID == deviceId)?.FirstOrDefault();
                    if ((controllerBase != null) && (controllerBase is IControllerIO controllerIO))
                    {
                        var analogicIo = (AnalogOutput)controllerIO.GetOutput(ioId);
                        if (analogicIo != null)
                        {
                            controllerIO.AnalogWrite(analogicIo, value);
                        }
                    }
                }
                catch (Exception exception)
                {
                    ReformulationMessage(messageContainer, ioId, exception.Message);
                    _logger.Error(exception, "SetAnalogIoValue");
                }
            });
        }

          public Response<VoidResult> StartIoRefreshTask()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                try
                {
                    var controllerIO = _hardwareManager.Controllers.Values.OfType<IControllerIO>().FirstOrDefault();
                    if(controllerIO == null)
                    {
                        _logger.Warning("[ControllerService][StartIoTask] Controller IO not found");
                    }
                    else
                    {
                        controllerIO.StartRefreshIOStatesTask();
                    }
                }
                catch (Exception exception)
                {
                    _logger.Error(exception, "StartIoRefreshTask");
                }
            });
        }

        public Response<VoidResult> StopIoRefreshTask()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                try
                {
                    var controllerIO = _hardwareManager.Controllers.Values.OfType<IControllerIO>().FirstOrDefault();
                    if(controllerIO == null)
                    {
                        _logger.Warning("[ControllerService][StopIoTask] Controller IO not found");
                    }
                    else
                    {
                        controllerIO.StopRefreshIOStatesTask();
                    }
                }
                catch (Exception exception)
                {
                    _logger.Error(exception, "StopIoRefreshTask");
                }
            });
        }
        private static void ReformulationMessage(List<Message> messageContainer, string ioId, string message, MessageLevel defaultLevel = MessageLevel.Error)
        {
            string userContent = ReformulationMessageManager.GetUserContent(ioId, message, message);
            var level = ReformulationMessageManager.GetLevel(ioId, message, defaultLevel);
            messageContainer.Add(new Message(level, userContent, message, ioId));
        }
    }
}
