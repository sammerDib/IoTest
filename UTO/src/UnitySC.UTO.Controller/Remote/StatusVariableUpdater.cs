using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Agileo.Common.Logging;
using Agileo.EquipmentModeling;
using Agileo.Semi.Communication.Abstractions.E5;
using Agileo.Semi.Gem.Abstractions.E30;

using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.DriveableProcessModule;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.UTO.Controller.Counters;
using UnitySC.UTO.Controller.Remote.Constants;
using UnitySC.UTO.Controller.Remote.Services;

namespace UnitySC.UTO.Controller.Remote
{
    internal class StatusVariableUpdater : E30StandardSupport
    {
        private List<string> _registeredPmList;

        #region Constructors

        public StatusVariableUpdater(IE30Standard e30Standard, ILogger logger) : base(e30Standard, logger)
        {
        }

        #endregion Constructors

        #region IInstanciableDevice Support

        public override void OnSetup(Agileo.EquipmentModeling.Equipment equipment)
        {
            base.OnSetup(equipment);

            _registeredPmList = new List<string>();

            var variableCallbackPairs = new List<VariableCallbackPair>
            {
                // custom code here as following example
                // new VariableCallbackPair(E30Standard.DataServices.GetVariableIDByName(SVs.LoadPortId), UpdateStatusVariable),
                new(E30Standard.DataServices.GetVariableIDByName(SVs.EqpSerialNum),
                    variable => variable.Value = DataItemFactory.NewDataItem<V>(
                        App.ControllerInstance.ControllerConfig.EquipmentIdentityConfig.EqpSerialNum)),
                new(E30Standard.DataServices.GetVariableIDByName(SVs.E30EquipmentSupplier),
                    variable => variable.Value = DataItemFactory.NewDataItem<V>(
                        App.ControllerInstance.ControllerConfig.EquipmentIdentityConfig.E30EquipmentSupplier)),
                new(E30Standard.DataServices.GetVariableIDByName(SVs.EfemFanSpeedRpmValue),
                    variable => variable.Value = DataItemFactory.NewDataItem<V>(
                        App.ControllerInstance.ControllerEquipmentManager.Ffu.FanSpeed.RevolutionsPerMinute)),
                new(E30Standard.DataServices.GetVariableIDByName(SVs.EfemPressureDifferentialValue),
                    variable => variable.Value = DataItemFactory.NewDataItem<V>(
                        App.ControllerInstance.ControllerEquipmentManager.Ffu.DifferentialPressure.Value)),
                new(E30Standard.DataServices.GetVariableIDByName(SVs.AirGaugeValue),
                    variable => variable.Value = DataItemFactory.NewDataItem<V>(
                        App.ControllerInstance.ControllerEquipmentManager.Efem.AirSensor)),
                new(E30Standard.DataServices.GetVariableIDByName(SVs.RobotSpeed),
                    variable => variable.Value = DataItemFactory.NewDataItem<V>(
                        App.ControllerInstance.ControllerEquipmentManager.Robot.Speed.Percent)),
                new(E30Standard.DataServices.GetVariableIDByName(SVs.RobotVacuumGaugeValue),
                    variable =>
                    {
                        var value =
                            App.ControllerInstance.ControllerEquipmentManager.Robot.UpperArmClamped
                            || App.ControllerInstance.ControllerEquipmentManager.Robot.LowerArmClamped;

                        return variable.Value = DataItemFactory.NewDataItem<V>(value);
                    }),
                new(E30Standard.DataServices.GetVariableIDByName(SVs.AlignerVacuumGaugeValue),
                    variable => variable.Value = DataItemFactory.NewDataItem<V>(
                        App.ControllerInstance.ControllerEquipmentManager.Aligner.IsClamped)),
                new(E30Standard.DataServices.GetVariableIDByName(SVs.OcrProfiles),
                    variable =>
                    {
                        var dataList = new DataList();
                        var values = App.ControllerInstance.ControllerConfig.OcrProfiles.Select(p => p.Name);
                        foreach (var value in values)
                        {
                            dataList.AddItem<V>(value);
                        }
                        return variable.Value = dataList;
                    }),

                new(E30Standard.DataServices.GetVariableIDByName(SVs.UTO_OnlineSettings),
                    variable => variable.Value = E30Standard.EquipmentConstantsServices.GetValueByWellKnownName(WellknownNames.ECs.EstablishCommunicationTimeout)),
                new(E30Standard.DataServices.GetVariableIDByName(SVs.UTO_Uptime),
                    variable => variable.Value = DataItemFactory.NewDataItem<V>(App.ControllerInstance.CountersManager.Uptime)),
                new(E30Standard.DataServices.GetVariableIDByName(SVs.UTO_StartCounter),
                    variable => variable.Value = GetCounterInformation(CounterDefinition.StartCounter)),
                new(E30Standard.DataServices.GetVariableIDByName(SVs.UTO_FatalErrorCounter),
                    variable => variable.Value = GetCounterInformation(CounterDefinition.FatalErrorCounter)),
                new(E30Standard.DataServices.GetVariableIDByName(SVs.UTO_JobCounter),
                    variable => variable.Value = GetCounterInformation(CounterDefinition.JobCounter)),
                new(E30Standard.DataServices.GetVariableIDByName(SVs.UTO_AbortCounter),
                    variable => variable.Value = GetCounterInformation(CounterDefinition.AbortCounter)),
                new(E30Standard.DataServices.GetVariableIDByName(SVs.UTO_ProcessedSubstrateCounter),
                    variable => variable.Value = GetCounterInformation(CounterDefinition.ProcessedSubstrateCounter)),
                new(E30Standard.DataServices.GetVariableIDByName(SVs.EfemSwVersion),
                    variable => variable.Value = DataItemFactory.NewDataItem<V>(
                        App.ControllerInstance.ControllerConfig.EquipmentIdentityConfig.SOFTREV)),
                new(E30Standard.DataServices.GetVariableIDByName(SVs.RobotModelName_Firmware),
                    variable =>
                    {
                        var robot = App.UtoInstance.ControllerEquipmentManager.Robot;
                        var model = robot.DeviceType;
                        var version = "N/A";

                        if (robot is IVersionedDevice versionedDevice)
                        {
                            version = versionedDevice.Version;
                        }

                        var sv = DataItemFactory.NewDataItem<V>($"{model.Name},{version}");
                        return variable.Value = sv;
                    }),
                new(E30Standard.DataServices.GetVariableIDByName(SVs.PM1ModelName_Firmware),
                    variable => variable.Value = GetProcessModuleInformation(1)),
                new(E30Standard.DataServices.GetVariableIDByName(SVs.PM2ModelName_Firmware),
                    variable => variable.Value = GetProcessModuleInformation(2)),
                new(E30Standard.DataServices.GetVariableIDByName(SVs.PM3ModelName_Firmware),
                    variable => variable.Value = GetProcessModuleInformation(3)),
                new(E30Standard.DataServices.GetVariableIDByName(SVs.Lp1Model_Firmware),
                    variable => variable.Value = GetLoadPortInformation(1)),
                new(E30Standard.DataServices.GetVariableIDByName(SVs.Lp2Model_Firmware),
                    variable => variable.Value = GetLoadPortInformation(2)),
                new(E30Standard.DataServices.GetVariableIDByName(SVs.Lp3Model_Firmware),
                    variable => variable.Value = GetLoadPortInformation(3)),
                new(E30Standard.DataServices.GetVariableIDByName(SVs.Lp4Model_Firmware),
                    variable => variable.Value = GetLoadPortInformation(4)),
                new(E30Standard.DataServices.GetVariableIDByName(SVs.UTO_JobProcessMax),
                    variable => variable.Value = DataItemFactory.NewDataItem<V>(App.UtoInstance.GemController.E40Std.QueueAvailableSpace))
            };

            E30Standard.DataServices.RegisterCallbacksForSVUpdate(variableCallbackPairs);

            foreach (var processModule in App.ControllerInstance.ControllerEquipmentManager.ProcessModules.Values)
            {
                if (processModule.IsCommunicating)
                {
                    RegisterPmCallBack(processModule);
                    _registeredPmList.Add(processModule.Name);
                }
                else
                {
                    processModule.StatusValueChanged += ProcessModule_StatusValueChanged;
                }

            }
        }

        #endregion IInstanciableDevice Support

        private void ProcessModule_StatusValueChanged(object sender, StatusChangedEventArgs e)
        {
            if (sender is not DriveableProcessModule processModule)
            {
                return;
            }

            if (e.Status.Name.Equals(nameof(DriveableProcessModule.IsCommunicating))
                && (bool)e.NewValue
                && !_registeredPmList.Contains(processModule.Name))
            {
                RegisterPmCallBack(processModule);
                _registeredPmList.Add(processModule.Name);
                processModule.StatusValueChanged -= ProcessModule_StatusValueChanged;
            }
        }

        private void RegisterPmCallBack(DriveableProcessModule processModule)
        {
            var variableCallbackPairs = new List<VariableCallbackPair>();

            var statusVariables = processModule.GetStatusVariables(new List<int>()).ToList();
            if (statusVariables.Count > 0)
            {
                foreach (var svName in statusVariables.Select(sv => sv?.Name))
                {
                    var fullSvName = $"ProcessModule{processModule.InstanceId}{svName}";
                    if (E30Standard.DataServices.TryGetVariableByName(fullSvName, out _))
                    {
                        variableCallbackPairs.Add(new VariableCallbackPair(E30Standard.DataServices.GetVariableIDByName(fullSvName),
                            variable =>
                            {
                                object value = null;
                                if (processModule.IsCommunicating)
                                {
                                    var sv = processModule.GetStatusVariables(new List<int>())
                                        .First(sv => sv.Name.Equals(svName));

                                    switch (sv.DataType)
                                    {
                                        case VidDataType.String:
                                            value = sv.ValueAsString;
                                            break;
                                        case VidDataType.F4:
                                            value = float.Parse(sv.ValueAsString, NumberStyles.Any, CultureInfo.InvariantCulture);
                                            break;
                                        case VidDataType.F8:
                                            value = double.Parse(sv.ValueAsString, NumberStyles.Any, CultureInfo.InvariantCulture);
                                            break;
                                        case VidDataType.I1:
                                            value = sbyte.Parse(sv.ValueAsString);
                                            break;
                                        case VidDataType.I2:
                                            value = short.Parse(sv.ValueAsString);
                                            break;
                                        case VidDataType.I4:
                                            value = int.Parse(sv.ValueAsString);
                                            break;
                                        case VidDataType.I8:
                                            value = long.Parse(sv.ValueAsString);
                                            break;
                                        case VidDataType.U1:
                                            value = byte.Parse(sv.ValueAsString);
                                            break;
                                        case VidDataType.U2:
                                            value = ushort.Parse(sv.ValueAsString);
                                            break;
                                        case VidDataType.U4:
                                            value = uint.Parse(sv.ValueAsString);
                                            break;
                                        case VidDataType.U8:
                                            value = ulong.Parse(sv.ValueAsString);
                                            break;
                                        case VidDataType.Boolean:
                                            value = bool.Parse(sv.ValueAsString);
                                            break;
                                    }
                                }

                                return variable.Value = DataItemFactory.NewDataItem<V>(value);
                            }));
                    }
                }
            }

            E30Standard.DataServices.RegisterCallbacksForSVUpdate(variableCallbackPairs);
        }

        private V GetLoadPortInformation(int instanceId)
        {
            var value = string.Empty;
            if (App.UtoInstance.ControllerEquipmentManager.LoadPorts.ContainsKey(instanceId))
            {
                var lp = App.UtoInstance.ControllerEquipmentManager.LoadPorts[instanceId];
                var model = lp.DeviceType;
                var version = "N/A";

                if (lp is IVersionedDevice versionedDevice)
                {
                    version = versionedDevice.Version;
                }

                value = $"{model.Name},{version}";
            }

            var sv = DataItemFactory.NewDataItem<V>(value);
            return sv;
        }

        private V GetProcessModuleInformation(int instanceId)
        {
            var value = string.Empty;
            if (App.UtoInstance.ControllerEquipmentManager.ProcessModules.ContainsKey(instanceId))
            {
                var pm = App.UtoInstance.ControllerEquipmentManager.ProcessModules[instanceId];
                var model = pm.DeviceType;
                var version = "N/A";

                value = $"{model.Name},{version}";
            }

            var sv = DataItemFactory.NewDataItem<V>(value);
            return sv;
        }

        private V GetCounterInformation(CounterDefinition counterDefinition)
        {
            var counters = App.ControllerInstance.CountersManager.PersistentCounters;
            ulong value;
            switch (counterDefinition)
            {
                case CounterDefinition.ProcessedSubstrateCounter:
                    value = counters.ProcessedSubstrateCounter;
                    break;
                case CounterDefinition.StartCounter:
                    value = counters.StartCounter;
                    break;
                case CounterDefinition.FatalErrorCounter:
                    value = counters.FatalErrorCounter;
                    break;
                case CounterDefinition.JobCounter:
                    value = counters.JobCounter;
                    break;
                case CounterDefinition.AbortCounter:
                    value = counters.AbortCounter;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(counterDefinition), counterDefinition, null);
            }

            return DataItemFactory.NewDataItem<V>(value);
        }
    }
}
