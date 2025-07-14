using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Agileo.EquipmentModeling;
using Agileo.EquipmentModeling.Grammar;
using Agileo.ModelingFramework;

namespace UnitySC.Equipment.Abstractions.Devices.SubstrateIdReader
{
    public abstract partial class SubstrateIdReader : UnitySC.Equipment.Abstractions.Devices.UnityCommunicatingDevice.UnityCommunicatingDevice, ISubstrateIdReader
    {
        public static new readonly DeviceType Type;

        static SubstrateIdReader()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            const string resource = "UnitySC.Equipment.Abstractions.Devices.SubstrateIdReader.SubstrateIdReader.device";
            using (Stream s = a.GetManifestResourceStream(resource))
            {
                if (s == null)
                {
                    throw new InvalidOperationException("Could not retrieve embedded resource " + resource);
                }

                Package package = Package.Load(s, null, true);
                Type = package.AllDeviceTypes().First(x => x.QualifiedName == "UnitySC.Equipment.Abstractions.Devices.SubstrateIdReader.SubstrateIdReader");
            }
        }

        public SubstrateIdReader(string name)
            : this(name, Type)
        {
        }

        protected SubstrateIdReader(string name, DeviceType type)
            : base(name, type)
        {
            InstanceInitialization();
        }

        protected override void InternalRun(CommandExecution execution)
        {
            switch (execution.Context.Command.Name)
            {
                case "RequestRecipes":
                    {
                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalRequestRecipes();
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateRequestRecipes(execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command RequestRecipes interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "Read":
                    {
                        System.String recipename = (System.String)execution.Context.GetArgument("recipeName");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalRead(recipename);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateRead(recipename, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command Read interrupted.");
                                }
                                else
                                {
                                    throw;
                                }
                            }
                        }

                        break;
                    }

                case "GetImage":
                    {
                        System.String imagepath = (System.String)execution.Context.GetArgument("imagePath");

                        if (execution.ExecutionMode == Agileo.EquipmentModeling.ExecutionMode.Real)
                        {
                            InternalGetImage(imagepath);
                        }
                        else
                        {
                            try
                            {
                                InternalSimulateGetImage(imagepath, execution.Tempomat);
                            }
                            catch (InvalidOperationException exception)
                            {
                                if (exception.Message.Equals("RaiseException was called", StringComparison.Ordinal))
                                {
                                    Logger.Debug("Simulated execution of command GetImage interrupted.");
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

        public void RequestRecipes()
        {
            CommandExecution execution = new CommandExecution(this, "RequestRecipes");
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task RequestRecipesAsync()
        {
            CommandExecution execution = new CommandExecution(this, "RequestRecipes");
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void Read(string recipeName)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("recipeName", recipeName));
            CommandExecution execution = new CommandExecution(this, "Read", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task ReadAsync(string recipeName)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("recipeName", recipeName));
            CommandExecution execution = new CommandExecution(this, "Read", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public void GetImage(string imagePath)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("imagePath", imagePath));
            CommandExecution execution = new CommandExecution(this, "GetImage", arguments);
            execution.ExecutionMode = ExecutionMode;
            Run(execution);
        }

        public Task GetImageAsync(string imagePath)
        {
            List<Argument> arguments = new List<Argument>();
            arguments.Add(new Argument("imagePath", imagePath));
            CommandExecution execution = new CommandExecution(this, "GetImage", arguments);
            execution.ExecutionMode = ExecutionMode;
            return RunAsync(execution);
        }

        public string SubstrateId
        {
            get { return (string)GetStatusValue("SubstrateId"); }
            protected set { SetStatusValue("SubstrateId", value); }
        }
    }
}
