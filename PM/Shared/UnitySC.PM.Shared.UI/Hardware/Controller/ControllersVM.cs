using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.PM.Shared.Hardware.ClientProxy.Chamber;
using UnitySC.PM.Shared.Hardware.Service.Interface.Controller;
using UnitySC.PM.Shared.Hardware.Service.Interface.IOComponent;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.PM.Shared.UI.Hardware.Controller
{
    public class ControllersVM : ViewModelBaseExt, IDisposable
    {
        private readonly IControllerService _controllersSupervisor;
        private GlobalStatusSupervisor _globalStatusSupervisor;
        private readonly ILogger _logger;

        public List<DataAttributeObject> InputList { get; set; } = new List<DataAttributeObject>();
        public List<DataAttributeObject> OutputList { get; set; } = new List<DataAttributeObject>();

        public Dictionary<string, List<DataAttributeObject>> InputsDisplayed { get; set; } = new Dictionary<string, List<DataAttributeObject>>();
        public Dictionary<string, List<DataAttributeObject>> OutputsDisplayed { get; set; } = new Dictionary<string, List<DataAttributeObject>>();

        public ControllersVM(IControllerService controllersSupervisor, ILogger logger)
        {
            _controllersSupervisor = controllersSupervisor;
            _globalStatusSupervisor = ClassLocator.Default.GetInstance<SharedSupervisors>().GetGlobalStatusSupervisor(ActorType.ANALYSE);
            _logger = logger;
            InitIoList();
        }

        private void InitIoList()
        {
            var dataAttributes = new List<DataAttribute>();
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                //Load Controllers IOs
                LoadControllersIOs(dataAttributes);

                //Update Input and Output list
                UpdateListsOfIODisplayed(dataAttributes);
            }));
        }
        private void LoadControllersIOs(List<DataAttribute> dataAttributes)
        {
            try
            {
                var list = _controllersSupervisor.GetControllersIOs()?.Result;
                if (list == null)
                    return;
                foreach (var iOControllerConfig in list)
                {
                    foreach (var io in iOControllerConfig.IOList)
                    {
                        if (!io.IsEnabled)
                        {
                            continue;
                        }
                        DataAttribute dataIO = new DataAttribute();
                        dataIO.Name = io.Name;
                        dataIO.ControllerName = iOControllerConfig.DeviceID;
                        dataIO.Module = io.Address.Module;
                        dataIO.Channel = io.Address.Channel;
                        dataIO.CommandName = io.CommandName;

                        switch (io)
                        {
                            case DigitalInput _:
                                {
                                    dataIO.Identifier = "DI";
                                    dataIO.Type = AttributeType.DigitalIO;
                                    dataIO.DigitalValue = _controllersSupervisor.GetDigitalIoState(iOControllerConfig.DeviceID, io.CommandName ?? io.Name)?.Result ?? false;
                                    break;
                                }
                            case DigitalOutput _:
                                {
                                    dataIO.Identifier = "DO";
                                    dataIO.Type = AttributeType.DigitalIO;
                                    dataIO.DigitalValue = false;//les Output sont tous à false par defaut sauf le ffu
                                    break;
                                }
                            case AnalogInput _:
                                {
                                    dataIO.Identifier = "AI";
                                    dataIO.Type = AttributeType.AnalogicIO;
                                    dataIO.AnalogValue = _controllersSupervisor.GetAnalogIoValue(iOControllerConfig.DeviceID, io.CommandName)?.Result ?? double.NaN;
                                    break;
                                }
                            case AnalogOutput _:
                                {
                                    dataIO.Identifier = "AO";
                                    dataIO.Type = AttributeType.AnalogicIO;
                                    dataIO.AnalogValue = 0.0;//les Output analog sont tous à 0 par defaut.
                                    break;
                                }
                            default:
                                throw (new Exception("This IO type is not associated to DataAttribute"));
                        }
                        dataAttributes.Add(dataIO);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, $"IO Init Failed");
            }
        }

        public void UpdateListsOfIODisplayed(List<DataAttribute> dataAttributes)
        {
            try
            {
                var inputData = dataAttributes.Where(x => (x.Identifier == "DI" || x.Identifier == "AI"));
                foreach (var item in inputData)
                {
                    UpdateDataAttributes(item, InputList);
                }
                var outputData = dataAttributes.Where(x => (x.Identifier == "DO" || x.Identifier == "AO"));
                foreach (var item in outputData)
                {
                    UpdateDataAttributes(item, OutputList);
                }

                InputsDisplayed = InputList.GroupBy(x => x.ControllerId).ToDictionary(g => g.Key, g => g.ToList<DataAttributeObject>());
                OnPropertyChanged(nameof(InputsDisplayed));
                OutputsDisplayed = OutputList.GroupBy(x => x.ControllerId).ToDictionary(g => g.Key, g => g.ToList<DataAttributeObject>());
                OnPropertyChanged(nameof(OutputsDisplayed));
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, $"The update of the displayed IO lists Failed");
                _globalStatusSupervisor.SendUIMessage(new Message(MessageLevel.Error, "Error while getting the list of IO"));
            }

        }

        private void UpdateDataAttributes(DataAttribute item, List<DataAttributeObject> ioList)
        {
            DataAttributeObject foundDataAttributeObj = (ioList.Find(x => (item.Name.ToString() == x.Name) && (item.Identifier.ToUpper() == x.Identifier.ToUpper())));
            if (foundDataAttributeObj == null)
            {
                DataAttributeObject newDataAttribute = new DataAttributeObject();
                newDataAttribute.Identifier = item.Identifier;
                newDataAttribute.Name = item.Name.ToString();
                newDataAttribute.ControllerId = item.ControllerName;
                newDataAttribute.CommandName = item.CommandName;

                if (item.Type == AttributeType.DigitalIO)
                {
                    newDataAttribute.Value = item.DigitalValue;
                }
                else if (item.Type == AttributeType.AnalogicIO)
                {
                    newDataAttribute.AnalogicValue = item.AnalogValue;
                }

                ioList.Add(newDataAttribute);
            }
            else
            {
                if (foundDataAttributeObj.Identifier.ToUpper() == "DO"
                    || foundDataAttributeObj.Identifier.ToUpper() == "DI")
                {
                    foundDataAttributeObj.UpdateValue(item.DigitalValue);
                }
                else if (foundDataAttributeObj.Identifier.ToUpper() == "AO"
                    || foundDataAttributeObj.Identifier.ToUpper() == "AI")
                {
                    foundDataAttributeObj.UpdateAnalogicValue(item.AnalogValue);
                }
            }
        }

        private void SetDigitalValue(string deviceId, string name, bool value)
        {
            Task.Run(() => _controllersSupervisor.SetDigitalIoState(deviceId, name, value));
        }

        private void SetAnalogicValue(string deviceId, string name, double value)
        {
            Task.Run(() => _controllersSupervisor.SetAnalogIoValue(deviceId, name, value));
        }

        private AutoRelayCommand<DataAttributeObject> _enableIoCommand;
        public AutoRelayCommand<DataAttributeObject> EnableIoCommand
        {
            get
            {
                return _enableIoCommand ?? (_enableIoCommand = new AutoRelayCommand<DataAttributeObject>(
                    (selectedIOParam) =>
                    {
                        try
                        {
                            if (selectedIOParam.Identifier.ToUpper() == "DO"
                            || selectedIOParam.Identifier.ToUpper() == "DI")
                            {
                                SetDigitalValue(selectedIOParam.ControllerId, selectedIOParam.CommandName ?? selectedIOParam.Name, true);
                            }
                        }
                        catch (Exception e)
                        {
                            _globalStatusSupervisor.SendUIMessage(new UnitySC.Shared.Tools.Service.Message(MessageLevel.Error, e.Message));
                        }
                    },
                    (selectedIOParam) => selectedIOParam != null
                    ));
            }
        }
        private AutoRelayCommand<DataAttributeObject> _disableIoCommand;
        public AutoRelayCommand<DataAttributeObject> DisableIoCommand
        {
            get
            {
                return _disableIoCommand ?? (_disableIoCommand = new AutoRelayCommand<DataAttributeObject>(
                    (selectedIOParam) =>
                    {
                        try
                        {

                            if (selectedIOParam.Identifier.ToUpper() == "DO"
                            || selectedIOParam.Identifier.ToUpper() == "DI")
                            {
                                SetDigitalValue(selectedIOParam.ControllerId, selectedIOParam.CommandName ?? selectedIOParam.Name, false);
                            }
                        }
                        catch (Exception e)
                        {
                            _globalStatusSupervisor.SendUIMessage(new UnitySC.Shared.Tools.Service.Message(MessageLevel.Error, e.Message));
                        }
                    },
                    (selectedIOParam) => selectedIOParam != null
                    ));
            }
        }
        private AutoRelayCommand<DataAttributeObject> _setAnalogOutputValueCommand;
        public AutoRelayCommand<DataAttributeObject> SetAnalogOutputValueCommand
        {
            get
            {
                return _setAnalogOutputValueCommand ?? (_setAnalogOutputValueCommand = new AutoRelayCommand<DataAttributeObject>(
                    (selectedIOParam) =>
                    {
                        try
                        {
                            if (selectedIOParam.Identifier.ToUpper() == "AO"
                            || selectedIOParam.Identifier.ToUpper() == "AI")
                            {
                                SetAnalogicValue(selectedIOParam.ControllerId, selectedIOParam.CommandName ?? selectedIOParam.Name, selectedIOParam.AnalogicValue);
                            }
                        }
                        catch (Exception e)
                        {
                            _globalStatusSupervisor.SendUIMessage(new UnitySC.Shared.Tools.Service.Message(MessageLevel.Error, e.Message));
                        }
                    },
                    (selectedIOParam) => selectedIOParam != null
                    ));
            }
        }

        public void Dispose()
        {
            InputList.Clear();
            OutputList.Clear();
            InputsDisplayed.Clear();
            OutputsDisplayed.Clear();
            OnDeactivated();
        }
    }
}
