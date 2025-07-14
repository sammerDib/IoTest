using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Agileo.Common.Logging;
using Agileo.EquipmentModeling;
using Agileo.Semi.Communication.Abstractions.E5;
using Agileo.Semi.Gem.Abstractions.E30;
using Agileo.Semi.Gem300.Abstractions.E40;
using Agileo.Semi.Gem300.Abstractions.E87;
using Agileo.Semi.Gem300.Abstractions.E90;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager;
using UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager.EventArgs;
using UnitySC.Equipment.Abstractions.Devices.Aligner;
using UnitySC.Equipment.Abstractions.Devices.DriveableProcessModule;
using UnitySC.Equipment.Abstractions.Devices.Efem;
using UnitySC.Equipment.Abstractions.Devices.LoadPort;
using UnitySC.Equipment.Abstractions.Devices.Robot;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Shared.Tools.Collection;
using UnitySC.ToolControl.ProcessModules.Devices.ToolControlManager;
using UnitySC.ToolControl.ProcessModules.Devices.ToolControlManager.EventArgs;
using UnitySC.ToolControl.ProcessModules.Drivers.ToolControl.Interfaces.SecsGem;
using UnitySC.UTO.Controller.Remote.Constants;
using UnitySC.UTO.Controller.Remote.E5.DataItems;
using UnitySC.UTO.Controller.Remote.E5.MessageDescriptions;
using UnitySC.UTO.Controller.Remote.Services;

using CollectionEventEventArgs = UnitySC.ToolControl.ProcessModules.Devices.ToolControlManager.EventArgs.CollectionEventEventArgs;
using LoadPort = UnitySC.Equipment.Abstractions.Devices.LoadPort.LoadPort;
using MaterialType = Agileo.Semi.Gem300.Abstractions.E40.MaterialType;
using SecsVariable = UnitySC.Shared.Data.SecsGem.SecsVariable;
using SlotState = UnitySC.Equipment.Abstractions.Material.SlotState;

namespace UnitySC.UTO.Controller.Remote.Observers
{
    internal class EquipmentObserver : E30StandardSupport
    {
        #region Fields

        private readonly Equipment.Devices.Controller.Controller _controller;
        private readonly AbstractDataFlowManager _dataFlowManager;
        private readonly Robot _robot;
        private readonly Aligner _aligner;

        private static IE90Standard E90Std => App.ControllerInstance.GemController.E90Std;
        private static IE40Standard E40Std => App.ControllerInstance.GemController.E40Std;
        private static IE87Standard E87Std => App.ControllerInstance.GemController.E87Std;

        #endregion Fields

        #region Constructors

        public EquipmentObserver(IE30Standard e30Standard, ILogger logger)
            : base(e30Standard, logger)
        {
            _controller =
                App.ControllerInstance.ControllerEquipmentManager.Controller as
                    UnitySC.Equipment.Devices.Controller.Controller;
            _dataFlowManager = _controller.TryGetDevice<AbstractDataFlowManager>();

            var efem = _controller.TryGetDevice<Efem>();
            _robot = efem.TryGetDevice<Robot>();
            _aligner = efem.TryGetDevice<Aligner>();
        }

        #endregion Constructors

        #region IInstanciableDevice Support

        public override void OnSetup(Agileo.EquipmentModeling.Equipment equipment)
        {
            base.OnSetup(equipment);

            if (App.ControllerInstance.GemController.IsSetupDone)
            {
                E30Standard.EquipmentConstantsServices.EquipmentConstantChanged +=
                    EquipmentConstantsServices_EquipmentConstantChanged;
            }

            foreach (var loadPort in App.ControllerInstance.ControllerEquipmentManager.LoadPorts
                         .Values)
            {
                loadPort.CommandExecutionStateChanged += LoadPort_CommandExecutionStateChanged;
                loadPort.CarrierPlaced += LoadPort_CarrierPlaced;
                loadPort.CarrierRemoved += LoadPort_CarrierRemoved;
                loadPort.CarrierIdChanged += LoadPort_CarrierIdChanged;
            }

            _dataFlowManager.ProcessModuleRecipeStarted +=
                DataFlowManager_ProcessModuleRecipeStarted;
            _dataFlowManager.ProcessModuleRecipeCompleted +=
                DataFlowManager_ProcessModuleRecipeCompleted;
            _dataFlowManager.DataFlowRecipeAdded += DataFlowManager_DataFlowRecipeAdded;
            _dataFlowManager.DataFlowRecipeModified += DataFlowManager_DataFlowRecipeModified;
            _dataFlowManager.DataFlowRecipeDeleted += DataFlowManager_DataFlowRecipeDeleted;
            _dataFlowManager.PropertyChanged += DataFlowManager_PropertyChanged;
            _dataFlowManager.FdcCollectionChanged += DataFlowManager_FdcCollectionChanged;
            _dataFlowManager.CollectionEventRaised += DataFlowManager_CollectionEventRaised;

            if (_dataFlowManager is ToolControlManager toolControlManager)
            {
                toolControlManager.S13F13Raised += ToolControlManager_S13F13Raised;
                toolControlManager.S13F16Raised += ToolControlManager_S13F16Raised;
                toolControlManager.ToolControlCollectionEventRaised +=
                    ToolControlManager_ToolControlCollectionEventRaised;
            }

            _controller.WaferAlignStart += Controller_WaferAlignStart;
            _controller.WaferAlignEnd += Controller_WaferAlignEnd;
            _controller.CommandExecutionStateChanged += Controller_CommandExecutionStateChanged;

            foreach (var processModule in App.ControllerInstance.ControllerEquipmentManager
                         .ProcessModules.Values)
            {
                processModule.CollectionEventRaised += ProcessModule_CollectionEventRaised;
            }

            _robot.CommandExecutionStateChanged += Robot_CommandExecutionStateChanged;
        }

        #endregion IInstanciableDevice Support

        #region Event Handlers

        private void ProcessModule_CollectionEventRaised(
            object sender,
            Equipment.Abstractions.Devices.AbstractDataFlowManager.EventArgs.CollectionEventEventArgs e)
        {
            if (sender is not DriveableProcessModule pm)
            {
                return;
            }

            var eventName = $"ProcessModule{e.CollectionEventName}";
            if (E30Standard.DataServices.GetEventID(eventName) == -1)
            {
                return;
            }

            var variablesUpdate = e.DataVariables;
            variablesUpdate.Add(new SecsVariable(DVs.ChamberId, new Shared.Data.SecsGem.SecsItem(Shared.Data.SecsGem.SecsFormat.UInt1, pm.InstanceId)));

            SendSecsVariableList(eventName, variablesUpdate);
        }

        private void EquipmentConstantsServices_EquipmentConstantChanged(
            object sender,
            VariableEventArgs e)
        {
            switch (e.Variable.Name)
            {
                case E90WellknownNames.ECs.SubstrateReaderEnabled:
                case E90WellknownNames.ECs.SubstrateIDVerificationEnabled:
                    _controller.UpdateReaderBehavior(
                        E90Std.SubstrateReaderEnabled,
                        E90Std.SubstrateIDVerificationEnabled);
                    break;
            }
        }

        private void Controller_CommandExecutionStateChanged(
            object sender,
            CommandExecutionEventArgs e)
        {
            if (sender is not Equipment.Devices.Controller.Controller controller)
            {
                return;
            }

            if (e.Execution.Context.Command.Name.Equals(
                    nameof(UnitySC.Equipment.Devices.Controller.IController.Initialize)))
            {
                switch (e.NewState)
                {
                    case ExecutionState.Running:
                        App.UtoInstance.JobQueueManager.SaveJobs();
                        break;
                    case ExecutionState.Success:
                        {
                            Task.Run(
                                () =>
                                {
                                    controller.UpdateReaderBehavior(
                                        E90Std.SubstrateReaderEnabled,
                                        E90Std.SubstrateIDVerificationEnabled);

                                    App.UtoInstance.GemController.E94Observer.ClearControlJobs();
                                    App.UtoInstance.GemController.E40Observer.ClearProcessJob();
                                    App.UtoInstance.GemController.E87Observer
                                        .RefreshLoadPortState();
                                    App.UtoInstance.GemController.E90Observer.ClearSubstrates();
                                    App.UtoInstance.JobQueueManager.RestoreJobs();
                                });
                            break;
                        }
                }
            }
        }

        private void LoadPort_CommandExecutionStateChanged(
            object sender,
            CommandExecutionEventArgs e)
        {
            if (sender is not LoadPort loadPort)
            {
                return;
            }

            if (e.Execution.Context.Command.Name.Equals(nameof(ILoadPort.ReadCarrierId)))
            {
                switch (e.NewState)
                {
                    case ExecutionState.Running:
                        {
                            E30Standard.DataServices.SendEvent(
                                CEIDs.CustomEvents.CarrierIdReadStart,
                                new List<VariableUpdate>()
                                {
                                    new(E87WellknownNames.DVs.PortID, (byte)loadPort.InstanceId)
                                });
                            break;
                        }
                    case ExecutionState.Success:
                        {
                            E30Standard.DataServices.SendEvent(
                                CEIDs.CustomEvents.CarrierIdReadEnd,
                                new List<VariableUpdate>()
                                {
                                    new(E87WellknownNames.DVs.PortID, (byte)loadPort.InstanceId)
                                });
                            break;
                        }
                }
            }

            if (e.Execution.Context.Command.Name.Equals(nameof(ILoadPort.Map))
                || (e.Execution.Context.Command.Name.Equals(nameof(ILoadPort.Open))
                    && e.Execution.Context.Arguments[0].Name == "performMapping"
                    && (bool)e.Execution.Context.Arguments[0].Value))
            {
                switch (e.NewState)
                {
                    case ExecutionState.Running:
                        {
                            E30Standard.DataServices.SendEvent(
                                CEIDs.CustomEvents.SlotMapReadStart,
                                new List<VariableUpdate>()
                                {
                                    new(E87WellknownNames.DVs.PortID, (byte)loadPort.InstanceId)
                                });
                            break;
                        }
                    case ExecutionState.Success:
                        {
                            E30Standard.DataServices.SendEvent(
                                CEIDs.CustomEvents.SlotMapReadEnd,
                                new List<VariableUpdate>()
                                {
                                    new(E87WellknownNames.DVs.PortID, (byte)loadPort.InstanceId)
                                });
                            break;
                        }
                }
            }
        }

        private void Controller_WaferAlignStart(object sender, EventArgs e)
        {
            E30Standard.DataServices.SendEvent(
                CEIDs.CustomEvents.WaferAlignerStart,
                new List<VariableUpdate>()
                {
                    new(E40WellknownNames.DVs.PRJobID, _aligner.Location.Wafer.ProcessJobId),
                    new(E90WellknownNames.DVs.SubstID, _aligner.Location.Wafer.SubstrateId),
                    new(E90WellknownNames.DVs.SubstLotID, _aligner.Location.Wafer.LotId)
                });
        }

        private void Controller_WaferAlignEnd(object sender, EventArgs e)
        {
            E30Standard.DataServices.SendEvent(
                CEIDs.CustomEvents.WaferAlignerEnd,
                new List<VariableUpdate>()
                {
                    new(E40WellknownNames.DVs.PRJobID, _aligner.Location.Wafer.ProcessJobId),
                    new(E90WellknownNames.DVs.SubstID, _aligner.Location.Wafer.SubstrateId),
                    new(E90WellknownNames.DVs.SubstLotID, _aligner.Location.Wafer.LotId)
                });
        }

        private void LoadPort_CarrierIdChanged(object sender, CarrierIdChangedEventArgs e)
        {
            if (sender is not LoadPort
                || App.ControllerInstance.ControllerEquipmentManager.Controller.State
                is OperatingModes.Maintenance or OperatingModes.Engineering)
            {
                return;
            }

            if (e.Status != CommandStatusCode.Ok)
            {
                GUI.Common.App.Instance.AlarmCenter.Services.SetAlarm(
                    App.ControllerInstance.GemController,
                    AlNames.CarrierIDReadFail);
            }
        }

        private void LoadPort_CarrierPlaced(
            object sender,
            Equipment.Abstractions.Material.CarrierEventArgs e)
        {
            if (sender is not LoadPort)
            {
                return;
            }

            var carrier = e?.Carrier;
            if (carrier == null)
            {
                return;
            }

            carrier.SlotMapChanged += Carrier_SlotMapChanged;
        }

        private void LoadPort_CarrierRemoved(
            object sender,
            Equipment.Abstractions.Material.CarrierEventArgs e)
        {
            if (sender is not LoadPort)
            {
                return;
            }

            var carrier = e?.Carrier;
            if (carrier != null)
            {
                carrier.SlotMapChanged -= Carrier_SlotMapChanged;
            }
        }

        private void Carrier_SlotMapChanged(
            object sender,
            Equipment.Abstractions.Material.SlotMapEventArgs e)
        {
            if (e.SlotMap.Any(s => s == SlotState.DoubleWafer))
            {
                GUI.Common.App.Instance.AlarmCenter.Services.SetAlarm(
                    App.ControllerInstance.GemController,
                    AlNames.DoubleSlotDetected);
            }
        }

        private void DataFlowManager_DataFlowRecipeDeleted(object sender, DataFlowRecipeEventArgs e)
        {
            E30Standard.ProcessRecipeServices.NotifyProcessProgramChanged(
                _dataFlowManager.GetRecipeName(e.Recipe),
                PPChangeStatus.Deleted);
        }

        private void DataFlowManager_DataFlowRecipeModified(
            object sender,
            DataFlowRecipeEventArgs e)
        {
            E30Standard.ProcessRecipeServices.NotifyProcessProgramChanged(
                _dataFlowManager.GetRecipeName(e.Recipe),
                PPChangeStatus.Edited);
        }

        private void DataFlowManager_DataFlowRecipeAdded(object sender, DataFlowRecipeEventArgs e)
        {
            E30Standard.ProcessRecipeServices.NotifyProcessProgramChanged(
                _dataFlowManager.GetRecipeName(e.Recipe),
                PPChangeStatus.Created);
        }

        private void DataFlowManager_ProcessModuleRecipeCompleted(
            object sender,
            ProcessModuleRecipeEventArgs e)
        {
            E30Standard.DataServices.SendEvent(
                CEIDs.CustomEvents.ProcessModuleRecipeCompleted,
                new List<VariableUpdate>()
                {
                    new(DVs.ChamberId, e.ProcessModule.InstanceId),
                    new(DVs.ChamberRecipeName, e.RecipeName)
                });
        }

        private void DataFlowManager_ProcessModuleRecipeStarted(
            object sender,
            ProcessModuleRecipeEventArgs e)
        {
            E30Standard.DataServices.SendEvent(
                CEIDs.CustomEvents.ProcessModuleRecipeStarted,
                new List<VariableUpdate>()
                {
                    new(DVs.ChamberId, e.ProcessModule.InstanceId),
                    new(DVs.ChamberRecipeName, e.RecipeName)
                });
        }

        private void Robot_CommandExecutionStateChanged(object sender, CommandExecutionEventArgs e)
        {
            //Check Command => Place / Dest => LP / Status => Done
            var commandName = e.Execution.Context.Command.Name;
            var destinationDevice = (IMaterialLocationContainer)e.Execution.Context.Arguments
                .FirstOrDefault(arg => arg.Name.Equals("destinationDevice"))
                ?.Value;

            if (!commandName.Equals(nameof(IRobot.Place))
                || destinationDevice is not LoadPort loadPort
                || e.NewState != ExecutionState.Success)
            {
                return;
            }

            //Get job queue
            var jobQueue = App.ControllerInstance.JobQueueManager.JobQueue;
            if (jobQueue.Count == 0)
            {
                return;
            }

            //Get current job and check execution number
            var currentJob =
                jobQueue.FirstOrDefault(job => job.ProcessJob.JobState is JobState.PROCESSING);
            if (currentJob == null
                || currentJob.CurrentExecution != currentJob.NumberOfExecutions
                || currentJob.LoopMode)
            {
                return;
            }

            //Check if the job queue still needs this carrier
            if (jobQueue.Any(
                    job => job.ProcessJob.CarrierIDSlotsAssociation.Any(
                               element => element.CarrierID.Equals(loadPort.Carrier.Id))
                           && !job.ProcessJob.ObjID.Equals(currentJob.ProcessJob.ObjID)))
            {
                return;
            }

            //Get E87 carrier
            var e87Carrier = E87Std.GetCarrierById(loadPort.Carrier.Id);
            if (e87Carrier == null)
            {
                return;
            }

            //Check that all wafers of the specified carrier have been processed
            if (!Helpers.Helpers.IsAllWaferAtDestination(currentJob.ProcessJob, e87Carrier))
            {
                return;
            }

            //Check that all wafers results of the specified carrier have been received
            var controllerJob =
                _controller.Jobs.FirstOrDefault(j => j.Name == currentJob.ProcessJob.ObjID);
            if (controllerJob == null ||
                !GetSubstrateIdList(currentJob.ProcessJob,e87Carrier)
                    .All(substId=> controllerJob.WaferResultReceived.Contains(substId)))
            {
                return;
            }

            //Check if the carrier is being used by other PJs (required for Pj/Cj created by the host).
            if (Helpers.Helpers.IsCarrierUsedByQueuedPj(e87Carrier.ObjID))
            {
                return;
            }

            //LoadPort can be released
            if (e87Carrier.CarrierAccessingStatus == AccessingStatus.InAccess)
            {
                E87Std.IntegrationServices.NotifyCarrierAccessingHasBeenFinished(
                    e87Carrier.ObjID,
                    currentJob.ProcessJob.JobState == JobState.PROCESSING);
            }
        }

        private List<string> GetSubstrateIdList(IProcessJob pj, Carrier carrier)
        {
            switch (pj.MaterialType)
            {
                case MaterialType.Carriers:
                    var substIdList = new List<string>();
                    foreach (var materialNameListElement in pj.CarrierIDSlotsAssociation)
                    {
                        if (!materialNameListElement.CarrierID.Equals(carrier.ObjID)
                            || materialNameListElement.SlotIds == null)
                        {
                            continue;
                        }

                        foreach (var slotId in materialNameListElement.SlotIds)
                        {
                            substIdList.Add(carrier.ContentMap[slotId-1].SubstrateID);
                        }
                    }
                    return substIdList;
                case MaterialType.Substrates:
                    return pj.SubstrateNames.ToList();
                default:
                    return new List<string>();
            }
        }

        private void DataFlowManager_CollectionEventRaised(
            object sender,
            Equipment.Abstractions.Devices.AbstractDataFlowManager.EventArgs.CollectionEventEventArgs e)
        {
            SendSecsVariableList(e.CollectionEventName, e.DataVariables);
        }

        private void DataFlowManager_PropertyChanged(
            object sender,
            System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(IAbstractDataFlowManager.IsStopCancelAllJobsRequested)
                || !_dataFlowManager.IsStopCancelAllJobsRequested)
            {
                return;
            }

            foreach (var processJob in E40Std.ProcessJobs)
            {
                E40Std.StandardServices.Command(
                    processJob.ObjID,
                    CommandName.STOP,
                    new List<CommandParameter>());
            }
        }

        private void DataFlowManager_FdcCollectionChanged(object sender, FdcCollectionEventArgs e)
        {
            if (e.FdcData?.IsEmpty() != false)
            {
                return;
            }

            foreach (var data in e.FdcData)
            {
                
                if (!E30Standard.DataServices.TryGetVariableByWellKnownName(data.Name, out var sv))
                {
                    continue;
                }

                try
                {
                    if (data.ValueFDC.Value is TimeSpan timeSpan)
                    {
                        switch (sv.Unit.ToLower())
                        {
                            case "ms":
                            case "millisecond":
                            case "milliseconds":
                                sv.SetValue(Convert.ToUInt64(timeSpan.TotalMilliseconds));
                                break;
                            case "sec":
                            case "second":
                            case "seconds":
                                sv.SetValue((timeSpan.TotalSeconds));
                                break;
                            case "min":
                            case "minute":
                            case "minutes":
                                sv.SetValue(Convert.ToUInt64(timeSpan.TotalMinutes));
                                break;
                            case "hour":
                            case "hours":
                                sv.SetValue(Convert.ToUInt64(timeSpan.TotalHours));
                                break;
                            case "day":
                            case "days":
                                sv.SetValue(Convert.ToUInt64(timeSpan.TotalDays));
                                break;
                            default:
                                sv.SetValue(data.ValueFDC.Value);
                                break;
                        }
                    }
                    else
                    {
                        switch (sv.Format)
                        {
                            case DataItemFormat.ASC:
                                sv.SetValue(data.ValueFDC.Value.ToString());
                                break;
                            case DataItemFormat.SI1:
                                sv.SetValue(Convert.ToSByte(data.ValueFDC.Value));
                                break;
                            case DataItemFormat.SI2:
                                sv.SetValue(Convert.ToInt16(data.ValueFDC.Value));
                                break;
                            case DataItemFormat.SI4:
                                sv.SetValue(Convert.ToInt32(data.ValueFDC.Value));
                                break;
                            case DataItemFormat.SI8:
                                sv.SetValue(Convert.ToInt64(data.ValueFDC.Value));
                                break;
                            case DataItemFormat.UI1:
                                sv.SetValue(Convert.ToByte(data.ValueFDC.Value));
                                break;
                            case DataItemFormat.UI2:
                                sv.SetValue(Convert.ToUInt16(data.ValueFDC.Value));
                                break;
                            case DataItemFormat.UI4:
                                sv.SetValue(Convert.ToUInt32(data.ValueFDC.Value));
                                break;
                            case DataItemFormat.UI8:
                                sv.SetValue(Convert.ToUInt64(data.ValueFDC.Value));
                                break;
                            case DataItemFormat.FP4:
                                sv.SetValue(Convert.ToSingle(data.ValueFDC.Value));
                                break;
                            case DataItemFormat.FP8:
                                sv.SetValue(Convert.ToDouble(data.ValueFDC.Value));
                                break;
                            default:
                                sv.SetValue(data.ValueFDC.Value);
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    if(sv == null)
                        throw new InvalidOperationException($"FDC collection update failed. Message : {ex.Message}");
                    else
                        throw new InvalidOperationException($"FDC collection update failed. {sv.Name} in error with message : {ex.Message}");
                }
            }
        }

        private void ToolControlManager_ToolControlCollectionEventRaised(
            object sender,
            CollectionEventEventArgs e)
        {
            SendSecsVariableList(e.CollectionEventName, e.DataVariables);
        }

        private void ToolControlManager_S13F16Raised(object sender, S13F16EventArgs e)
        {
            var message = S13F16.New(
                e.TableData.TableType,
                e.TableData.TableId,
                !e.TableData.TableAck
                    ? TBLACK.Success
                    : TBLACK.Failure);

            #region Message Attributes

            if (e.TableData.Attributes != null)
            {
                for (var iAttribute = 0; iAttribute < e.TableData.Attributes.Count; iAttribute++)
                {
                    var secsAttribute = e.TableData.Attributes[iAttribute];

                    switch (secsAttribute.Data.Format)
                    {
                        case SecsFormat.List:
                            var dataList = new DataList();
                            FillDataListWithSecsItemList(dataList, secsAttribute.Data.ItemList);
                            message.WithAttribute(secsAttribute.Id, dataList);
                            break;

                        case SecsFormat.Binary:
                            message.WithAttribute(secsAttribute.Id, secsAttribute.Data.BinaryValue);
                            break;

                        case SecsFormat.Boolean:
                            message.WithAttribute(
                                secsAttribute.Id,
                                secsAttribute.Data.BooleanValue);
                            break;

                        case SecsFormat.Ascii:
                        case SecsFormat.Character:
                            message.WithAttribute(secsAttribute.Id, secsAttribute.Data.StringValue);
                            break;

                        case SecsFormat.Int8:
                            message.WithAttribute(secsAttribute.Id, secsAttribute.Data.Int8Value);
                            break;

                        case SecsFormat.Int1:
                            message.WithAttribute(secsAttribute.Id, secsAttribute.Data.Int1Value);
                            break;

                        case SecsFormat.Int2:
                            message.WithAttribute(secsAttribute.Id, secsAttribute.Data.Int2Value);
                            break;

                        case SecsFormat.Int4:
                            message.WithAttribute(secsAttribute.Id, secsAttribute.Data.Int4Value);
                            break;

                        case SecsFormat.Float8:
                            message.WithAttribute(secsAttribute.Id, secsAttribute.Data.Float8Value);
                            break;

                        case SecsFormat.Float4:
                            message.WithAttribute(secsAttribute.Id, secsAttribute.Data.Float4Value);
                            break;

                        case SecsFormat.UInt8:
                            message.WithAttribute(secsAttribute.Id, secsAttribute.Data.Uint8Value);
                            break;

                        case SecsFormat.UInt1:
                            message.WithAttribute(secsAttribute.Id, secsAttribute.Data.Uint1Value);
                            break;

                        case SecsFormat.UInt2:
                            message.WithAttribute(secsAttribute.Id, secsAttribute.Data.Uint2Value);
                            break;

                        case SecsFormat.UInt4:
                            message.WithAttribute(secsAttribute.Id, secsAttribute.Data.Uint4Value);
                            break;
                    }
                }
            }

            #endregion

            #region Message Table Elements

            for (var iTableElement = 0;
                 iTableElement < e.TableData.TableElements.Count;
                 iTableElement++)
            {
                var elements = e.TableData.TableElements[iTableElement];

                var row = new List<TBLELT>();

                for (var iElement = 0; iElement < elements.Count; iElement++)
                {
                    var tableElement = elements[iElement];

                    if (!message.ColumnDefinitions.ToList()
                            .Contains(tableElement.ColumnElementDescription))
                    {
                        message.WithColumnDefinition(tableElement.ColumnElementDescription);
                    }

                    switch (tableElement.Data.Format)
                    {
                        case SecsFormat.List:
                            var dataList = new DataList();
                            FillDataListWithSecsItemList(dataList, tableElement.Data.ItemList);
                            row.Add(dataList);
                            break;

                        case SecsFormat.Binary:
                            row.Add(tableElement.Data.BinaryValue);
                            break;

                        case SecsFormat.Boolean:
                            row.Add(tableElement.Data.BooleanValue);
                            break;

                        case SecsFormat.Ascii:
                        case SecsFormat.Character:
                            row.Add(tableElement.Data.StringValue);
                            break;

                        case SecsFormat.Int8:
                            row.Add(tableElement.Data.Int8Value);
                            break;

                        case SecsFormat.Int1:
                            row.Add(tableElement.Data.Int1Value);
                            break;

                        case SecsFormat.Int2:
                            row.Add(tableElement.Data.Int2Value);
                            break;

                        case SecsFormat.Int4:
                            row.Add(tableElement.Data.Int4Value);
                            break;

                        case SecsFormat.Float8:
                            row.Add(tableElement.Data.Float8Value);
                            break;

                        case SecsFormat.Float4:
                            row.Add(tableElement.Data.Float4Value);
                            break;

                        case SecsFormat.UInt8:
                            row.Add(tableElement.Data.Uint8Value);
                            break;

                        case SecsFormat.UInt1:
                            row.Add(tableElement.Data.Uint1Value);
                            break;

                        case SecsFormat.UInt2:
                            row.Add(tableElement.Data.Uint2Value);
                            break;

                        case SecsFormat.UInt4:
                            row.Add(tableElement.Data.Uint4Value);
                            break;
                    }
                }

                message.WithRowDefinition(row);
            }

            #endregion

            #region Message Errors

            if (e.TableData.Errors != null)
            {
                for (var iError = 0; iError < e.TableData.Errors.Count; iError++)
                {
                    var secsError = e.TableData.Errors[iError];
                    message.AddError(ERRCODE.FromInt(secsError.Code), secsError.Text);
                }
            }

            #endregion

            E30Standard.MessageServices.SendMessage(message);
        }

        private void ToolControlManager_S13F13Raised(object sender, S13F13EventArgs e)
        {
            var message = S13F13.New(
                e.TableData.DataId,
                e.TableData.ObjSpec,
                e.TableData.TableType,
                e.TableData.TableId,
                e.TableData.TableCommand);

            #region Attributes

            if (e.TableData.Attributes != null)
            {
                for (var iAttribute = 0; iAttribute < e.TableData.Attributes.Count; iAttribute++)
                {
                    var secsAttribute = e.TableData.Attributes[iAttribute];

                    switch (secsAttribute.Data.Format)
                    {
                        case SecsFormat.List:
                            var dataList = new DataList();
                            FillDataListWithSecsItemList(dataList, secsAttribute.Data.ItemList);
                            message.WithAttribute(secsAttribute.Id, dataList);
                            break;

                        case SecsFormat.Binary:
                            message.WithAttribute(secsAttribute.Id, secsAttribute.Data.BinaryValue);
                            break;

                        case SecsFormat.Boolean:
                            message.WithAttribute(
                                secsAttribute.Id,
                                secsAttribute.Data.BooleanValue);
                            break;

                        case SecsFormat.Ascii:
                        case SecsFormat.Character:
                            message.WithAttribute(secsAttribute.Id, secsAttribute.Data.StringValue);
                            break;

                        case SecsFormat.Int8:
                            message.WithAttribute(secsAttribute.Id, secsAttribute.Data.Int8Value);
                            break;

                        case SecsFormat.Int1:
                            message.WithAttribute(secsAttribute.Id, secsAttribute.Data.Int1Value);
                            break;

                        case SecsFormat.Int2:
                            message.WithAttribute(secsAttribute.Id, secsAttribute.Data.Int2Value);
                            break;

                        case SecsFormat.Int4:
                            message.WithAttribute(secsAttribute.Id, secsAttribute.Data.Int4Value);
                            break;

                        case SecsFormat.Float8:
                            message.WithAttribute(secsAttribute.Id, secsAttribute.Data.Float8Value);
                            break;

                        case SecsFormat.Float4:
                            message.WithAttribute(secsAttribute.Id, secsAttribute.Data.Float4Value);
                            break;

                        case SecsFormat.UInt8:
                            message.WithAttribute(secsAttribute.Id, secsAttribute.Data.Uint8Value);
                            break;

                        case SecsFormat.UInt1:
                            message.WithAttribute(secsAttribute.Id, secsAttribute.Data.Uint1Value);
                            break;

                        case SecsFormat.UInt2:
                            message.WithAttribute(secsAttribute.Id, secsAttribute.Data.Uint2Value);
                            break;

                        case SecsFormat.UInt4:
                            message.WithAttribute(secsAttribute.Id, secsAttribute.Data.Uint4Value);
                            break;
                    }
                }
            }

            #endregion

            #region Message Table Elements

            for (var iTableElement = 0;
                 iTableElement < e.TableData.TableElements.Count;
                 iTableElement++)
            {
                var elements = e.TableData.TableElements[iTableElement];

                var row = new List<TBLELT>();

                for (var iElement = 0; iElement < elements.Count; iElement++)
                {
                    var tableElement = elements[iElement];

                    if (!message.ColumnDefinitions.ToList()
                            .Contains(tableElement.ColumnElementDescription))
                    {
                        message.WithColumnDefinition(tableElement.ColumnElementDescription);
                    }

                    switch (tableElement.Data.Format)
                    {
                        case SecsFormat.List:
                            var dataList = new DataList();
                            FillDataListWithSecsItemList(dataList, tableElement.Data.ItemList);
                            row.Add(dataList);
                            break;

                        case SecsFormat.Binary:
                            row.Add(tableElement.Data.BinaryValue);
                            break;

                        case SecsFormat.Boolean:
                            row.Add(tableElement.Data.BooleanValue);
                            break;

                        case SecsFormat.Ascii:
                        case SecsFormat.Character:
                            row.Add(tableElement.Data.StringValue);
                            break;

                        case SecsFormat.Int8:
                            row.Add(tableElement.Data.Int8Value);
                            break;

                        case SecsFormat.Int1:
                            row.Add(tableElement.Data.Int1Value);
                            break;

                        case SecsFormat.Int2:
                            row.Add(tableElement.Data.Int2Value);
                            break;

                        case SecsFormat.Int4:
                            row.Add(tableElement.Data.Int4Value);
                            break;

                        case SecsFormat.Float8:
                            row.Add(tableElement.Data.Float8Value);
                            break;

                        case SecsFormat.Float4:
                            row.Add(tableElement.Data.Float4Value);
                            break;

                        case SecsFormat.UInt8:
                            row.Add(tableElement.Data.Uint8Value);
                            break;

                        case SecsFormat.UInt1:
                            row.Add(tableElement.Data.Uint1Value);
                            break;

                        case SecsFormat.UInt2:
                            row.Add(tableElement.Data.Uint2Value);
                            break;

                        case SecsFormat.UInt4:
                            row.Add(tableElement.Data.Uint4Value);
                            break;
                    }
                }

                message.WithRowDefinition(row);
            }

            #endregion

            E30Standard.MessageServices.SendMessage(message);
        }

        #endregion Event Handlers

        #region Private Methods

        #region Unity.Shared

        private void SendSecsVariableList(string collectionEventName, Shared.Data.SecsGem.ISecsVariableList dataVariables)
        {
            var variableUpdates = new List<VariableUpdate>();

            for (int iDataVariable = 0; iDataVariable < dataVariables.Count(); iDataVariable++)
            {
                var secsVariable = dataVariables[iDataVariable];

                switch (secsVariable.Value.Format)
                {
                    case Shared.Data.SecsGem.SecsFormat.List:
                        var dataList = new DataList();
                        FillDataListWithSecsItemList(dataList, secsVariable.Value.ItemList);
                        variableUpdates.Add(new VariableUpdate(secsVariable.Name, dataList));
                        break;

                    case Shared.Data.SecsGem.SecsFormat.Binary:
                        variableUpdates.Add(new VariableUpdate(secsVariable.Name, secsVariable.Value.BinaryValue));
                        break;

                    case Shared.Data.SecsGem.SecsFormat.Boolean:
                        variableUpdates.Add(new VariableUpdate(secsVariable.Name, secsVariable.Value.BooleanValue));
                        break;

                    case Shared.Data.SecsGem.SecsFormat.Ascii:
                    case Shared.Data.SecsGem.SecsFormat.Character:
                        variableUpdates.Add(new VariableUpdate(secsVariable.Name, secsVariable.Value.StringValue));
                        break;

                    case Shared.Data.SecsGem.SecsFormat.Int8:
                        variableUpdates.Add(new VariableUpdate(secsVariable.Name, secsVariable.Value.Int8Value));
                        break;

                    case Shared.Data.SecsGem.SecsFormat.Int1:
                        variableUpdates.Add(new VariableUpdate(secsVariable.Name, secsVariable.Value.Int1Value));
                        break;

                    case Shared.Data.SecsGem.SecsFormat.Int2:
                        variableUpdates.Add(new VariableUpdate(secsVariable.Name, secsVariable.Value.Int2Value));
                        break;

                    case Shared.Data.SecsGem.SecsFormat.Int4:
                        variableUpdates.Add(new VariableUpdate(secsVariable.Name, secsVariable.Value.Int4Value));
                        break;

                    case Shared.Data.SecsGem.SecsFormat.Float8:
                        variableUpdates.Add(new VariableUpdate(secsVariable.Name, secsVariable.Value.Float8Value));
                        break;

                    case Shared.Data.SecsGem.SecsFormat.Float4:
                        variableUpdates.Add(new VariableUpdate(secsVariable.Name, secsVariable.Value.Float4Value));
                        break;

                    case Shared.Data.SecsGem.SecsFormat.UInt8:
                        variableUpdates.Add(new VariableUpdate(secsVariable.Name, secsVariable.Value.Uint8Value));
                        break;

                    case Shared.Data.SecsGem.SecsFormat.UInt1:
                        variableUpdates.Add(new VariableUpdate(secsVariable.Name, secsVariable.Value.Uint1Value));
                        break;

                    case Shared.Data.SecsGem.SecsFormat.UInt2:
                        variableUpdates.Add(new VariableUpdate(secsVariable.Name, secsVariable.Value.Uint2Value));
                        break;

                    case Shared.Data.SecsGem.SecsFormat.UInt4:
                        variableUpdates.Add(new VariableUpdate(secsVariable.Name, secsVariable.Value.Uint4Value));
                        break;
                }
            }

            E30Standard.DataServices.SendEvent(collectionEventName, variableUpdates);
        }

        private void FillDataListWithSecsItemList(DataList mainDataList, Shared.Data.SecsGem.ISecsItemList secsItemList)
        {
            foreach (var item in secsItemList)
            {
                if (item is not Shared.Data.SecsGem.SecsItem secsItem)
                {
                    continue;
                }

                switch (secsItem.Format)
                {
                    case Shared.Data.SecsGem.SecsFormat.List:
                        var dataList = new DataList();
                        FillDataListWithSecsItemList(dataList, secsItem.ItemList);
                        mainDataList.AddItem(dataList);
                        break;

                    case Shared.Data.SecsGem.SecsFormat.Binary:
                        mainDataList.AddItem(secsItem.BinaryValue);
                        break;

                    case Shared.Data.SecsGem.SecsFormat.Boolean:
                        mainDataList.AddItem(secsItem.BooleanValue);
                        break;

                    case Shared.Data.SecsGem.SecsFormat.Ascii:
                    case Shared.Data.SecsGem.SecsFormat.Character:
                        mainDataList.AddItem(secsItem.StringValue);
                        break;

                    case Shared.Data.SecsGem.SecsFormat.Int8:
                        mainDataList.AddItem(secsItem.Int8Value);
                        break;

                    case Shared.Data.SecsGem.SecsFormat.Int1:
                        mainDataList.AddItem(secsItem.Int1Value);
                        break;

                    case Shared.Data.SecsGem.SecsFormat.Int2:
                        mainDataList.AddItem(secsItem.Int2Value);
                        break;

                    case Shared.Data.SecsGem.SecsFormat.Int4:
                        mainDataList.AddItem(secsItem.Int4Value);
                        break;

                    case Shared.Data.SecsGem.SecsFormat.Float8:
                        mainDataList.AddItem(secsItem.Float8Value);
                        break;

                    case Shared.Data.SecsGem.SecsFormat.Float4:
                        mainDataList.AddItem(secsItem.Float4Value);
                        break;

                    case Shared.Data.SecsGem.SecsFormat.UInt8:
                        mainDataList.AddItem(secsItem.Uint8Value);
                        break;

                    case Shared.Data.SecsGem.SecsFormat.UInt1:
                        mainDataList.AddItem(secsItem.Uint1Value);
                        break;

                    case Shared.Data.SecsGem.SecsFormat.UInt2:
                        mainDataList.AddItem(secsItem.Uint2Value);
                        break;

                    case Shared.Data.SecsGem.SecsFormat.UInt4:
                        mainDataList.AddItem(secsItem.Uint4Value);
                        break;
                }
            }
        }

        #endregion

        #region ToolControl

        private void SendSecsVariableList(string collectionEventName, ISecsVariableList dataVariables)
        {
            var variableUpdates = new List<VariableUpdate>();

            for (int iDataVariable = 0; iDataVariable < dataVariables.Count; iDataVariable++)
            {
                var secsVariable = dataVariables[iDataVariable];

                switch (secsVariable.Value.Format)
                {
                    case SecsFormat.List:
                        var dataList = new DataList();
                        FillDataListWithSecsItemList(dataList, secsVariable.Value.ItemList);
                        variableUpdates.Add(new VariableUpdate(secsVariable.Name, dataList));
                        break;

                    case SecsFormat.Binary:
                        variableUpdates.Add(new VariableUpdate(secsVariable.Name, secsVariable.Value.BinaryValue));
                        break;

                    case SecsFormat.Boolean:
                        variableUpdates.Add(new VariableUpdate(secsVariable.Name, secsVariable.Value.BooleanValue));
                        break;

                    case SecsFormat.Ascii:
                    case SecsFormat.Character:
                        variableUpdates.Add(new VariableUpdate(secsVariable.Name, secsVariable.Value.StringValue));
                        break;

                    case SecsFormat.Int8:
                        variableUpdates.Add(new VariableUpdate(secsVariable.Name, secsVariable.Value.Int8Value));
                        break;

                    case SecsFormat.Int1:
                        variableUpdates.Add(new VariableUpdate(secsVariable.Name, secsVariable.Value.Int1Value));
                        break;

                    case SecsFormat.Int2:
                        variableUpdates.Add(new VariableUpdate(secsVariable.Name, secsVariable.Value.Int2Value));
                        break;

                    case SecsFormat.Int4:
                        variableUpdates.Add(new VariableUpdate(secsVariable.Name, secsVariable.Value.Int4Value));
                        break;

                    case SecsFormat.Float8:
                        variableUpdates.Add(new VariableUpdate(secsVariable.Name, secsVariable.Value.Float8Value));
                        break;

                    case SecsFormat.Float4:
                        variableUpdates.Add(new VariableUpdate(secsVariable.Name, secsVariable.Value.Float4Value));
                        break;

                    case SecsFormat.UInt8:
                        variableUpdates.Add(new VariableUpdate(secsVariable.Name, secsVariable.Value.Uint8Value));
                        break;

                    case SecsFormat.UInt1:
                        variableUpdates.Add(new VariableUpdate(secsVariable.Name, secsVariable.Value.Uint1Value));
                        break;

                    case SecsFormat.UInt2:
                        variableUpdates.Add(new VariableUpdate(secsVariable.Name, secsVariable.Value.Uint2Value));
                        break;

                    case SecsFormat.UInt4:
                        variableUpdates.Add(new VariableUpdate(secsVariable.Name, secsVariable.Value.Uint4Value));
                        break;
                }
            }

            E30Standard.DataServices.SendEvent(collectionEventName, variableUpdates);
        }

        private void FillDataListWithSecsItemList(DataList mainDataList, ISecsItemList secsItemList)
        {
            foreach (var item in secsItemList)
            {
                if (item is not SecsItem secsItem)
                {
                    continue;
                }

                switch (secsItem.Format)
                {
                    case SecsFormat.List:
                        var dataList = new DataList();
                        FillDataListWithSecsItemList(dataList, secsItem.ItemList);
                        mainDataList.AddItem(dataList);
                        break;

                    case SecsFormat.Binary:
                        mainDataList.AddItem(secsItem.BinaryValue);
                        break;

                    case SecsFormat.Boolean:
                        mainDataList.AddItem(secsItem.BooleanValue);
                        break;

                    case SecsFormat.Ascii:
                    case SecsFormat.Character:
                        mainDataList.AddItem(secsItem.StringValue);
                        break;

                    case SecsFormat.Int8:
                        mainDataList.AddItem(secsItem.Int8Value);
                        break;

                    case SecsFormat.Int1:
                        mainDataList.AddItem(secsItem.Int1Value);
                        break;

                    case SecsFormat.Int2:
                        mainDataList.AddItem(secsItem.Int2Value);
                        break;

                    case SecsFormat.Int4:
                        mainDataList.AddItem(secsItem.Int4Value);
                        break;

                    case SecsFormat.Float8:
                        mainDataList.AddItem(secsItem.Float8Value);
                        break;

                    case SecsFormat.Float4:
                        mainDataList.AddItem(secsItem.Float4Value);
                        break;

                    case SecsFormat.UInt8:
                        mainDataList.AddItem(secsItem.Uint8Value);
                        break;

                    case SecsFormat.UInt1:
                        mainDataList.AddItem(secsItem.Uint1Value);
                        break;

                    case SecsFormat.UInt2:
                        mainDataList.AddItem(secsItem.Uint2Value);
                        break;

                    case SecsFormat.UInt4:
                        mainDataList.AddItem(secsItem.Uint4Value);
                        break;
                }
            }
        }

        #endregion

        #endregion Private Methods

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            if (disposing && App.ControllerInstance.GemController.IsSetupDone)
            {
                E30Standard.EquipmentConstantsServices.EquipmentConstantChanged -=
                    EquipmentConstantsServices_EquipmentConstantChanged;

                foreach (var loadPort in App.ControllerInstance.ControllerEquipmentManager.LoadPorts
                             .Values)
                {
                    loadPort.CommandExecutionStateChanged -= LoadPort_CommandExecutionStateChanged;
                    loadPort.CarrierPlaced -= LoadPort_CarrierPlaced;
                    loadPort.CarrierRemoved -= LoadPort_CarrierRemoved;
                    loadPort.CarrierIdChanged -= LoadPort_CarrierIdChanged;
                }

                _dataFlowManager.ProcessModuleRecipeStarted -=
                    DataFlowManager_ProcessModuleRecipeStarted;
                _dataFlowManager.ProcessModuleRecipeCompleted -=
                    DataFlowManager_ProcessModuleRecipeCompleted;
                _dataFlowManager.DataFlowRecipeAdded -= DataFlowManager_DataFlowRecipeAdded;
                _dataFlowManager.DataFlowRecipeModified -= DataFlowManager_DataFlowRecipeModified;
                _dataFlowManager.DataFlowRecipeDeleted -= DataFlowManager_DataFlowRecipeDeleted;
                _dataFlowManager.PropertyChanged -= DataFlowManager_PropertyChanged;
                _dataFlowManager.FdcCollectionChanged -= DataFlowManager_FdcCollectionChanged;
                _dataFlowManager.CollectionEventRaised -= DataFlowManager_CollectionEventRaised;

                if (_dataFlowManager is ToolControlManager toolControlManager)
                {
                    toolControlManager.S13F13Raised -= ToolControlManager_S13F13Raised;
                    toolControlManager.S13F16Raised -= ToolControlManager_S13F16Raised;
                    toolControlManager.ToolControlCollectionEventRaised -=
                        ToolControlManager_ToolControlCollectionEventRaised;
                }

                _controller.WaferAlignStart -= Controller_WaferAlignStart;
                _controller.WaferAlignEnd -= Controller_WaferAlignEnd;
                _controller.CommandExecutionStateChanged -= Controller_CommandExecutionStateChanged;

                foreach (var processModule in App.ControllerInstance.ControllerEquipmentManager
                             .ProcessModules.Values)
                {
                    processModule.CollectionEventRaised -= ProcessModule_CollectionEventRaised;
                }

                _robot.CommandExecutionStateChanged -= Robot_CommandExecutionStateChanged;
            }

            base.Dispose(disposing);
        }

        #endregion Dispose
    }
}
