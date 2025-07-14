using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.EquipmentModeling.Grammar;
using Agileo.ModelingFramework;

namespace UnitySC.Equipment.Abstractions.Devices.LoadPort
{
    public abstract partial class LoadPort : UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice.UnityCommunicatingDevice, ILoadPort
    {
        public static new readonly DeviceType Type;

        static LoadPort()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            const string resource = "UnitySC.Equipment.Abstractions.Devices.LoadPort.LoadPort.device";
            using (Stream s = a.GetManifestResourceStream(resource))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not retrieve embedded resource " + resource);
                }

                Package package = Package.Load(s, null, true);
                Type = package.AllDeviceTypes().First(x => x.QualifiedName == "UnitySC.Equipment.Abstractions.Devices.LoadPort.LoadPort");
            }
        }

        public LoadPort(string name)
            : this(name, Type)
        {
        }

        protected LoadPort(string name, DeviceType type)
            : base(name, type)
        {
            InstanceInitialization();
        }

        protected override void InternalRun(CommandExecution execution)
        {
            switch (execution.Context.Command.Name)
            {
                case "GetStatuses":
                    {
                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalGetStatuses();
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateGetStatuses(execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command GetStatuses interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "Clamp":
                    {
                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalClamp();
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateClamp(execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command Clamp interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "Unclamp":
                    {
                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalUnclamp();
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateUnclamp(execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command Unclamp interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "Dock":
                    {
                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalDock();
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateDock(execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command Dock interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "Undock":
                    {
                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalUndock();
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateUndock(execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command Undock interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "Open":
                    {
                        System.Boolean performmapping = (System.Boolean)execution.Context.GetArgument("performMapping");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalOpen(performmapping);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateOpen(performmapping, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command Open interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "PrepareForTransfer":
                    {
                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalPrepareForTransfer();
                        }
                        else
                        {
                            try
                            {
                                InternalSimulatePrepareForTransfer(execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command PrepareForTransfer interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "PostTransfer":
                    {
                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalPostTransfer();
                        }
                        else
                        {
                            try
                            {
                                InternalSimulatePostTransfer(execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command PostTransfer interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "Close":
                    {
                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalClose();
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateClose(execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command Close interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "Map":
                    {
                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalMap();
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateMap(execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command Map interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "ReadCarrierId":
                    {
                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalReadCarrierId();
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateReadCarrierId(execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command ReadCarrierId interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "ReleaseCarrier":
                    {
                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalReleaseCarrier();
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateReleaseCarrier(execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command ReleaseCarrier interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "SetLight":
                    {
                        UnitySC.Equipment.Abstractions.Devices.LoadPort.Configuration.LoadPortLightRoleType role = (UnitySC.Equipment.Abstractions.Devices.LoadPort.Configuration.LoadPortLightRoleType)execution.Context.GetArgument("role");

                        Agileo.SemiDefinitions.LightState lightstate = (Agileo.SemiDefinitions.LightState)execution.Context.GetArgument("lightState");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalSetLight(role, lightstate);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateSetLight(role, lightstate, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command SetLight interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "SetDateAndTime":
                    {
                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalSetDateAndTime();
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateSetDateAndTime(execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command SetDateAndTime interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "SetAccessMode":
                    {
                        Agileo.SemiDefinitions.LoadingType accessmode = (Agileo.SemiDefinitions.LoadingType)execution.Context.GetArgument("accessMode");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalSetAccessMode(accessmode);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateSetAccessMode(accessmode, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command SetAccessMode interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "RequestLoad":
                    {
                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalRequestLoad();
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateRequestLoad(execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command RequestLoad interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "RequestUnload":
                    {
                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalRequestUnload();
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateRequestUnload(execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command RequestUnload interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "SetCarrierType":
                    {
                        System.UInt32 carriertype = (System.UInt32)execution.Context.GetArgument("carrierType");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalSetCarrierType(carriertype);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateSetCarrierType(carriertype, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command SetCarrierType interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "EnableE84":
                    {
                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalEnableE84();
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateEnableE84(execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command EnableE84 interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "DisableE84":
                    {
                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalDisableE84();
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateDisableE84(execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command DisableE84 interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "ManageEsSignal":
                    {
                        System.Boolean isactive = (System.Boolean)execution.Context.GetArgument("isActive");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalManageEsSignal(isactive);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateManageEsSignal(isactive, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command ManageEsSignal interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "SetE84Timeouts":
                    {
                        System.Int32 tp1 = (System.Int32)execution.Context.GetArgument("tp1");

                        System.Int32 tp2 = (System.Int32)execution.Context.GetArgument("tp2");

                        System.Int32 tp3 = (System.Int32)execution.Context.GetArgument("tp3");

                        System.Int32 tp4 = (System.Int32)execution.Context.GetArgument("tp4");

                        System.Int32 tp5 = (System.Int32)execution.Context.GetArgument("tp5");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalSetE84Timeouts(tp1, tp2, tp3, tp4, tp5);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateSetE84Timeouts(tp1, tp2, tp3, tp4, tp5, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command SetE84Timeouts interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                default:
                    base.InternalRun(execution);
                    break;
            }
        }

        public void GetStatuses()
        {
            CommandExecution execution = new CommandExecution(this, "GetStatuses");
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task GetStatusesAsync()
        {
            CommandExecution execution = new CommandExecution(this, "GetStatuses");
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void Clamp()
        {
            CommandExecution execution = new CommandExecution(this, "Clamp");
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task ClampAsync()
        {
            CommandExecution execution = new CommandExecution(this, "Clamp");
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void Unclamp()
        {
            CommandExecution execution = new CommandExecution(this, "Unclamp");
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task UnclampAsync()
        {
            CommandExecution execution = new CommandExecution(this, "Unclamp");
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void Dock()
        {
            CommandExecution execution = new CommandExecution(this, "Dock");
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task DockAsync()
        {
            CommandExecution execution = new CommandExecution(this, "Dock");
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void Undock()
        {
            CommandExecution execution = new CommandExecution(this, "Undock");
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task UndockAsync()
        {
            CommandExecution execution = new CommandExecution(this, "Undock");
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void Open(bool performMapping)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("performMapping", performMapping));
            CommandExecution execution = new CommandExecution(this, "Open", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task OpenAsync(bool performMapping)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("performMapping", performMapping));
            CommandExecution execution = new CommandExecution(this, "Open", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void PrepareForTransfer()
        {
            CommandExecution execution = new CommandExecution(this, "PrepareForTransfer");
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task PrepareForTransferAsync()
        {
            CommandExecution execution = new CommandExecution(this, "PrepareForTransfer");
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void PostTransfer()
        {
            CommandExecution execution = new CommandExecution(this, "PostTransfer");
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task PostTransferAsync()
        {
            CommandExecution execution = new CommandExecution(this, "PostTransfer");
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void Close()
        {
            CommandExecution execution = new CommandExecution(this, "Close");
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task CloseAsync()
        {
            CommandExecution execution = new CommandExecution(this, "Close");
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void Map()
        {
            CommandExecution execution = new CommandExecution(this, "Map");
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task MapAsync()
        {
            CommandExecution execution = new CommandExecution(this, "Map");
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void ReadCarrierId()
        {
            CommandExecution execution = new CommandExecution(this, "ReadCarrierId");
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task ReadCarrierIdAsync()
        {
            CommandExecution execution = new CommandExecution(this, "ReadCarrierId");
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void ReleaseCarrier()
        {
            CommandExecution execution = new CommandExecution(this, "ReleaseCarrier");
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task ReleaseCarrierAsync()
        {
            CommandExecution execution = new CommandExecution(this, "ReleaseCarrier");
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void SetLight(UnitySC.Equipment.Abstractions.Devices.LoadPort.Configuration.LoadPortLightRoleType role,Agileo.SemiDefinitions.LightState lightState)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("role", role));
            arguments.Add(new Argument("lightState", lightState));
            CommandExecution execution = new CommandExecution(this, "SetLight", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task SetLightAsync(UnitySC.Equipment.Abstractions.Devices.LoadPort.Configuration.LoadPortLightRoleType role,Agileo.SemiDefinitions.LightState lightState)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("role", role));
            arguments.Add(new Argument("lightState", lightState));
            CommandExecution execution = new CommandExecution(this, "SetLight", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void SetDateAndTime()
        {
            CommandExecution execution = new CommandExecution(this, "SetDateAndTime");
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task SetDateAndTimeAsync()
        {
            CommandExecution execution = new CommandExecution(this, "SetDateAndTime");
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void SetAccessMode(Agileo.SemiDefinitions.LoadingType accessMode)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("accessMode", accessMode));
            CommandExecution execution = new CommandExecution(this, "SetAccessMode", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task SetAccessModeAsync(Agileo.SemiDefinitions.LoadingType accessMode)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("accessMode", accessMode));
            CommandExecution execution = new CommandExecution(this, "SetAccessMode", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void RequestLoad()
        {
            CommandExecution execution = new CommandExecution(this, "RequestLoad");
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task RequestLoadAsync()
        {
            CommandExecution execution = new CommandExecution(this, "RequestLoad");
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void RequestUnload()
        {
            CommandExecution execution = new CommandExecution(this, "RequestUnload");
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task RequestUnloadAsync()
        {
            CommandExecution execution = new CommandExecution(this, "RequestUnload");
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void SetCarrierType(uint carrierType)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("carrierType", carrierType));
            CommandExecution execution = new CommandExecution(this, "SetCarrierType", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task SetCarrierTypeAsync(uint carrierType)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("carrierType", carrierType));
            CommandExecution execution = new CommandExecution(this, "SetCarrierType", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void EnableE84()
        {
            CommandExecution execution = new CommandExecution(this, "EnableE84");
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task EnableE84Async()
        {
            CommandExecution execution = new CommandExecution(this, "EnableE84");
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void DisableE84()
        {
            CommandExecution execution = new CommandExecution(this, "DisableE84");
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task DisableE84Async()
        {
            CommandExecution execution = new CommandExecution(this, "DisableE84");
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void ManageEsSignal(bool isActive)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("isActive", isActive));
            CommandExecution execution = new CommandExecution(this, "ManageEsSignal", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task ManageEsSignalAsync(bool isActive)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("isActive", isActive));
            CommandExecution execution = new CommandExecution(this, "ManageEsSignal", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void SetE84Timeouts(int tp1,int tp2,int tp3,int tp4,int tp5)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("tp1", tp1));
            arguments.Add(new Argument("tp2", tp2));
            arguments.Add(new Argument("tp3", tp3));
            arguments.Add(new Argument("tp4", tp4));
            arguments.Add(new Argument("tp5", tp5));
            CommandExecution execution = new CommandExecution(this, "SetE84Timeouts", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task SetE84TimeoutsAsync(int tp1,int tp2,int tp3,int tp4,int tp5)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("tp1", tp1));
            arguments.Add(new Argument("tp2", tp2));
            arguments.Add(new Argument("tp3", tp3));
            arguments.Add(new Argument("tp4", tp4));
            arguments.Add(new Argument("tp5", tp5));
            CommandExecution execution = new CommandExecution(this, "SetE84Timeouts", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public Agileo.SemiDefinitions.CassettePresence CarrierPresence
        {
            get { return (Agileo.SemiDefinitions.CassettePresence)GetStatusValue("CarrierPresence"); }
            protected set { SetStatusValue("CarrierPresence", value); }
        }

        public Agileo.SemiDefinitions.LoadPortState PhysicalState
        {
            get { return (Agileo.SemiDefinitions.LoadPortState)GetStatusValue("PhysicalState"); }
            protected set { SetStatusValue("PhysicalState", value); }
        }

        public Agileo.SemiDefinitions.LoadingType AccessMode
        {
            get { return (Agileo.SemiDefinitions.LoadingType)GetStatusValue("AccessMode"); }
            protected set { SetStatusValue("AccessMode", value); }
        }

        public bool IsInService
        {
            get { return (bool)GetStatusValue("IsInService"); }
            protected set { SetStatusValue("IsInService", value); }
        }

        public bool IsClamped
        {
            get { return (bool)GetStatusValue("IsClamped"); }
            protected set { SetStatusValue("IsClamped", value); }
        }

        public bool IsDocked
        {
            get { return (bool)GetStatusValue("IsDocked"); }
            protected set { SetStatusValue("IsDocked", value); }
        }

        public bool IsDoorOpen
        {
            get { return (bool)GetStatusValue("IsDoorOpen"); }
            protected set { SetStatusValue("IsDoorOpen", value); }
        }

        public bool IsHandOffButtonPressed
        {
            get { return (bool)GetStatusValue("IsHandOffButtonPressed"); }
            protected set { SetStatusValue("IsHandOffButtonPressed", value); }
        }

        public Agileo.SemiDefinitions.LightState LoadLightState
        {
            get { return (Agileo.SemiDefinitions.LightState)GetStatusValue("LoadLightState"); }
            protected set { SetStatusValue("LoadLightState", value); }
        }

        public Agileo.SemiDefinitions.LightState UnloadLightState
        {
            get { return (Agileo.SemiDefinitions.LightState)GetStatusValue("UnloadLightState"); }
            protected set { SetStatusValue("UnloadLightState", value); }
        }

        public Agileo.SemiDefinitions.LightState ManualModeLightState
        {
            get { return (Agileo.SemiDefinitions.LightState)GetStatusValue("ManualModeLightState"); }
            protected set { SetStatusValue("ManualModeLightState", value); }
        }

        public Agileo.SemiDefinitions.LightState AutoModeLightState
        {
            get { return (Agileo.SemiDefinitions.LightState)GetStatusValue("AutoModeLightState"); }
            protected set { SetStatusValue("AutoModeLightState", value); }
        }

        public Agileo.SemiDefinitions.LightState ReservedLightState
        {
            get { return (Agileo.SemiDefinitions.LightState)GetStatusValue("ReservedLightState"); }
            protected set { SetStatusValue("ReservedLightState", value); }
        }

        public Agileo.SemiDefinitions.LightState ErrorLightState
        {
            get { return (Agileo.SemiDefinitions.LightState)GetStatusValue("ErrorLightState"); }
            protected set { SetStatusValue("ErrorLightState", value); }
        }

        public Agileo.SemiDefinitions.LightState HandOffLightState
        {
            get { return (Agileo.SemiDefinitions.LightState)GetStatusValue("HandOffLightState"); }
            protected set { SetStatusValue("HandOffLightState", value); }
        }

        public uint CarrierTypeNumber
        {
            get { return (uint)GetStatusValue("CarrierTypeNumber"); }
            protected set { SetStatusValue("CarrierTypeNumber", value); }
        }

        public string CarrierTypeName
        {
            get { return (string)GetStatusValue("CarrierTypeName"); }
            protected set { SetStatusValue("CarrierTypeName", value); }
        }

        public string CarrierTypeDescription
        {
            get { return (string)GetStatusValue("CarrierTypeDescription"); }
            protected set { SetStatusValue("CarrierTypeDescription", value); }
        }

        public uint CarrierTypeIndex
        {
            get { return (uint)GetStatusValue("CarrierTypeIndex"); }
            protected set { SetStatusValue("CarrierTypeIndex", value); }
        }

        public string CarrierProfileName
        {
            get { return (string)GetStatusValue("CarrierProfileName"); }
            protected set { SetStatusValue("CarrierProfileName", value); }
        }

        public bool I_VALID
        {
            get { return (bool)GetStatusValue("I_VALID"); }
            protected set { SetStatusValue("I_VALID", value); }
        }

        public bool I_CS_0
        {
            get { return (bool)GetStatusValue("I_CS_0"); }
            protected set { SetStatusValue("I_CS_0", value); }
        }

        public bool I_CS_1
        {
            get { return (bool)GetStatusValue("I_CS_1"); }
            protected set { SetStatusValue("I_CS_1", value); }
        }

        public bool I_TR_REQ
        {
            get { return (bool)GetStatusValue("I_TR_REQ"); }
            protected set { SetStatusValue("I_TR_REQ", value); }
        }

        public bool I_BUSY
        {
            get { return (bool)GetStatusValue("I_BUSY"); }
            protected set { SetStatusValue("I_BUSY", value); }
        }

        public bool I_COMPT
        {
            get { return (bool)GetStatusValue("I_COMPT"); }
            protected set { SetStatusValue("I_COMPT", value); }
        }

        public bool I_CONT
        {
            get { return (bool)GetStatusValue("I_CONT"); }
            protected set { SetStatusValue("I_CONT", value); }
        }

        public bool O_L_REQ
        {
            get { return (bool)GetStatusValue("O_L_REQ"); }
            protected set { SetStatusValue("O_L_REQ", value); }
        }

        public bool O_U_REQ
        {
            get { return (bool)GetStatusValue("O_U_REQ"); }
            protected set { SetStatusValue("O_U_REQ", value); }
        }

        public bool O_READY
        {
            get { return (bool)GetStatusValue("O_READY"); }
            protected set { SetStatusValue("O_READY", value); }
        }

        public bool O_HO_AVBL
        {
            get { return (bool)GetStatusValue("O_HO_AVBL"); }
            protected set { SetStatusValue("O_HO_AVBL", value); }
        }

        public bool O_ES
        {
            get { return (bool)GetStatusValue("O_ES"); }
            protected set { SetStatusValue("O_ES", value); }
        }

        public bool E84TransferInProgress
        {
            get { return (bool)GetStatusValue("E84TransferInProgress"); }
            protected set { SetStatusValue("E84TransferInProgress", value); }
        }

        public UnitySC.Equipment.Abstractions.Devices.LoadPort.E84Errors CurrentE84Error
        {
            get { return (UnitySC.Equipment.Abstractions.Devices.LoadPort.E84Errors)GetStatusValue("CurrentE84Error"); }
            protected set { SetStatusValue("CurrentE84Error", value); }
        }
    }
}
